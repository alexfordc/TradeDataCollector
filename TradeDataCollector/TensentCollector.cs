using System;
using System.Collections.Generic;
using System.Net;
using System.IO;

namespace TradeDataCollector
{
    public class TensentCollector : BaseCollector
    {
        private WebClient webClient;
        private Dictionary<string,FixedSizeTradeQueue> dictTradeCache=new Dictionary<string, FixedSizeTradeQueue>();
        private Dictionary<string, string> dictGMToTensent = new Dictionary<string, string>();
        
        public TensentCollector()
        {
            this.webClient = new WebClient();
        }
        public override Dictionary<string, Tick> Current(IEnumerable<string> symbols)
        {
            Dictionary<string, Tick> ret = new Dictionary<string, Tick>();
            string url = "http://qt.gtimg.cn/q=";
            foreach (string symbol in symbols)
            {
                string tensentSymbol;
                if (!this.dictGMToTensent.TryGetValue(symbol, out tensentSymbol))
                {
                    tensentSymbol = this.ConvertGMSymbolToTensent(symbol);
                    this.dictGMToTensent[symbol] = tensentSymbol;
                }
                url += tensentSymbol + ",";
            }
            List<string> tickStrings = new List<string>();

            Stream stream = this.webClient.OpenRead(url);
            StreamReader reader = new StreamReader(stream);
            //Console.WriteLine(url);
            string tickString;
            while ((tickString = reader.ReadLine()) != null)
            {
                tickStrings.Add(tickString);
            }
            stream.Close();
            int i = 0;
            foreach (string symbol in symbols)
            {
                if (i >= tickStrings.Count) break;
                string[] data = tickStrings[i].Split('~');
                if (!data[0].Contains(this.dictGMToTensent[symbol])) continue;
                Tick aTick = new Tick
                {
                    Price = float.Parse(data[3]),
                    LastClose = float.Parse(data[4]),
                    Open = float.Parse(data[5]),
                    High = float.Parse(data[33]),
                    Low = float.Parse(data[34]),
                    UpperLimit = float.Parse(data[47]),
                    LowerLimit = float.Parse(data[48])
                };
                for (int k = 0; k < 5; k++)
                {
                    aTick.Quotes[k] = new Quote
                    {
                        BidPrice = float.Parse(data[9 + k * 2]),
                        BidVolume = long.Parse(data[10 + k * 2]) * 100,
                        AskPrice = float.Parse(data[19 + k * 2]),
                        AskVolume = long.Parse(data[20 + k * 2]) * 100
                    };
                }

                string[] tradeStrings = data[29].Split('|');
                if (tradeStrings.Length < 1) continue;
                FixedSizeTradeQueue tradeQueue;
                if (!this.dictTradeCache.TryGetValue(symbol, out tradeQueue))
                {
                    tradeQueue = new FixedSizeTradeQueue(30);
                    this.dictTradeCache[symbol] = tradeQueue;
                }
                
                for (int k = tradeStrings.Length - 1; k >= 0; k--)
                {
                    string[] temp = tradeStrings[k].Split('/');
                    Trade aTrade = new Trade
                    {
                        DateTime = DateTime.Today.Add(TimeSpan.Parse(temp[0])),
                        Price = float.Parse(temp[1]),
                        Volume = int.Parse(temp[2])*100,
                        BuyOrSell = temp[3][0],
                        Amount = double.Parse(temp[4])
                    };
                    tradeQueue.Add(aTrade);
                }
                Trade lastTrade = tradeQueue.LastTrade;
                aTick.Volume = lastTrade.Volume;
                aTick.Amount = lastTrade.Amount;
                aTick.BuyOrSell = lastTrade.BuyOrSell;
                aTick.DateTime = lastTrade.DateTime;
                aTick.CumVolume = double.Parse(data[36]) * 100;
                aTick.CumAmount = double.Parse(data[37]) *10000;
                ret.Add(symbol, aTick);
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

        public override List<Trade> HistoryTicks(string symbol, string startTime, string endTime="")
        {
            DateTime time1 = DateTime.Parse(startTime);
            DateTime time2 = DateTime.Now;
            if (endTime != "") time2 = DateTime.Parse(endTime);

            FixedSizeTradeQueue tradeQueue;
            if (!this.dictTradeCache.TryGetValue(symbol, out tradeQueue))
            {
                tradeQueue = new FixedSizeTradeQueue(30);
                this.dictTradeCache[symbol] = tradeQueue;
            }
            List<Trade> ret = new List<Trade>();
            if (time1 < tradeQueue.MinTime || time1 > tradeQueue.MaxTime)
            {
                //读取今天的tick
            } else
            {
                foreach (Trade aTrade in tradeQueue.Values)
                {
                    if (aTrade.DateTime >= time1 && aTrade.DateTime <= time2) ret.Add(aTrade);
                }
            }
            return ret;
        }

        public override List<Trade> HistoryTicksN(string symbol, int n, string endTime="")
        {
            throw new NotImplementedException();
        }

        private string ConvertGMSymbolToTensent(string gmSymbol)
        {
            string[] strArray = gmSymbol.Split('.');
            return strArray[0].Substring(0, 2).ToLower() + strArray[1];
        }
    }
}
