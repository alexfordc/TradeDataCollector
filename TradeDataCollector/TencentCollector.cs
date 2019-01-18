using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TradeDataCollector
{
    public class TencentCollector : BaseCollector
    {
        private WebClient webClient;
        private Dictionary<string, FixedSizeTradeQueue> dictTradeCache = new Dictionary<string, FixedSizeTradeQueue>();
        private Dictionary<string, string> dictGMToTensent = new Dictionary<string, string>();
        private Dictionary<string, DateTime> dictLastTradeDate = new Dictionary<string, DateTime>();
        public TencentCollector()
        {
            this.webClient = new WebClient();
        }
        public override Dictionary<string, Tick> Current(IEnumerable<string> symbols)
        {
            Dictionary<string, Tick> ret = new Dictionary<string, Tick>();
            string url = "http://qt.gtimg.cn/q=";
            foreach (string symbol in symbols)
            {
                string tensentSymbol = this.getTencentSymbol(symbol);
                url += tensentSymbol + ",";
            }
            List<string> tickStrings = new List<string>();

            Stream stream = this.webClient.OpenRead(url);
            StreamReader reader = new StreamReader(stream);
            string tickString;
            while ((tickString = reader.ReadLine()) != null) tickStrings.Add(tickString);
            stream.Close();

            int i = 0;
            foreach (string symbol in symbols)
            {
                if (i >= tickStrings.Count) break;
                string[] data = tickStrings[i].Split('~');
                if (!data[0].Contains(this.dictGMToTensent[symbol])) continue;
                if (data.Length >= 48)//保证有数据
                {
                    Tick aTick = new Tick
                    {
                        Price = Utils.ParseFloat(data[3]),
                        LastClose = Utils.ParseFloat(data[4]),
                        Open = Utils.ParseFloat(data[5]),
                        High = Utils.ParseFloat(data[33]),
                        Low = Utils.ParseFloat(data[34]),
                        UpperLimit = Utils.ParseFloat(data[47]),
                        LowerLimit = Utils.ParseFloat(data[48]),
                        DateTime = DateTime.ParseExact(data[30], "yyyyMMddHHmmss", null) //此为快照生成时间，不是最新交易时间
                    };
                    this.dictLastTradeDate[symbol] = aTick.DateTime.Date;//当前交易日期
                    for (int k = 0; k < 5; k++)
                    {
                        aTick.Quotes[k] = new Quote
                        {
                            BidPrice = Utils.ParseFloat(data[9 + k * 2]),
                            BidVolume = Utils.ParseLong(data[10 + k * 2]) * 100,
                            AskPrice = Utils.ParseFloat(data[19 + k * 2]),
                            AskVolume = Utils.ParseLong(data[20 + k * 2]) * 100
                        };
                    }

                    string[] tradeStrings = data[29].Split('|');
                    if (tradeStrings.Length < 1) continue;
                    FixedSizeTradeQueue tradeQueue = this.getTradeQueue(symbol);
                    for (int k = tradeStrings.Length - 1; k >= 0; k--)
                    {
                        string[] temp = tradeStrings[k].Split('/');
                        Trade aTrade = new Trade
                        {
                            DateTime = aTick.DateTime.Date.Add(TimeSpan.Parse(temp[0])),
                            Price = Utils.ParseFloat(temp[1]),
                            Volume = Utils.ParseInt(temp[2]) * 100,
                            BuyOrSell = temp[3][0],
                            Amount = Utils.ParseDouble(temp[4])
                        };
                        tradeQueue.Add(aTrade);
                    }
                    Trade lastTrade = tradeQueue.LastTrade;
                    aTick.Volume = lastTrade.Volume;
                    aTick.Amount = lastTrade.Amount;
                    aTick.BuyOrSell = lastTrade.BuyOrSell;
                    aTick.DateTime = lastTrade.DateTime;
                    aTick.CumVolume = Utils.ParseDouble(data[36]) * 100;
                    aTick.CumAmount = Utils.ParseDouble(data[37]) * 10000;
                    ret.Add(symbol, aTick);
                }
                i++;
            }
            return ret;
        }

        public override List<Bar> HistoryBars(string symbol, int size, string startTime, string endTime)
        {
            throw new NotImplementedException();
        }

        public override List<Bar> HistoryBarsN(string symbol, int size, int n, string endTime)
        {
            throw new NotImplementedException();
        }

        public override List<Trade> HistoryTrades(string symbol, string startTime, string endTime="")
        {
            DateTime time1 = DateTime.Parse(startTime);
            DateTime time2 = DateTime.Now;
            if (endTime != "") time2 = DateTime.Parse(endTime);

            FixedSizeTradeQueue tradeQueue = this.getTradeQueue(symbol);
            List<Trade> ret = new List<Trade>();
            if (time1 < tradeQueue.MinTime || time1 > tradeQueue.MaxTime)
            {
                Console.WriteLine("start time not in range of cache trades! ");
            }
            else{
                foreach (Trade aTrade in tradeQueue.Values)
                {
                    if (aTrade.DateTime >= time1 && aTrade.DateTime <= time2) ret.Add(aTrade);
                }
            }
            return ret;
        }

        public override List<Trade> HistoryTradesN(string symbol, int n, string endTime="")
        {
            throw new NotImplementedException();
        }
        public override List<Trade> LastDayTrades(string symbol)
        {
            List<Trade> ret = new List<Trade>();
            string tensentSymbol = this.getTencentSymbol(symbol);
            DateTime lastTradeDate = this.getLastTradeDate(symbol);
            string url = "http://stock.gtimg.cn/data/index.php?appn=detail&action=data&c=" + tensentSymbol;
            int page = 0;
            string dataString;
            do
            {
                Stream stream = this.webClient.OpenRead(url + String.Format("&p={0}", page));
                StreamReader reader = new StreamReader(stream);
                if ((dataString = reader.ReadToEnd()) != "")
                {
                    string[] tradeStrings = dataString.Split('|');
                    int len = tradeStrings.Length;
                    if (len < 1) continue;
                    for (int k = 0; k < len; k++)
                    {
                        string[] temp = tradeStrings[k].Split('/');
                        Trade aTrade = new Trade
                        {
                            DateTime = lastTradeDate.Add(TimeSpan.Parse(temp[1])),
                            Price = Utils.ParseFloat(temp[2]),
                            Volume = Utils.ParseInt(temp[4]) * 100,
                            BuyOrSell = temp[6][0],
                            Amount = Utils.ParseDouble(temp[5])
                        };
                        ret.Add(aTrade);
                    }
                }
                else break;
                stream.Close();
                page++;
            } while (dataString != null);
            return ret;
        }
  
        private string getTencentSymbol(string symbol)
        {
            string tensentSymbol;
            if (!this.dictGMToTensent.TryGetValue(symbol, out tensentSymbol))
            {
                tensentSymbol = Utils.GMToTencent(symbol);
                this.dictGMToTensent[symbol] = tensentSymbol;
            }
            return tensentSymbol;
        }
        private FixedSizeTradeQueue getTradeQueue(string symbol)
        {
            FixedSizeTradeQueue tradeQueue;
            if (!this.dictTradeCache.TryGetValue(symbol, out tradeQueue))
            {
                tradeQueue = new FixedSizeTradeQueue(30);
                this.dictTradeCache[symbol] = tradeQueue;
            }
            return tradeQueue;
        }
        private DateTime getLastTradeDate(string symbol)
        {
            DateTime lastTradeDate;
            if (!this.dictLastTradeDate.TryGetValue(symbol,out lastTradeDate))
            {
                lastTradeDate = DateTime.Today;
                this.dictLastTradeDate.Add(symbol, lastTradeDate);
            }
            return lastTradeDate;
        }

    }
}
