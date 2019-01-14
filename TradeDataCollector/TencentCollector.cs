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
        private Dictionary<string, PageTimeList> dictPageTimeList = new Dictionary<string, PageTimeList>();
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
                Tick aTick = new Tick
                {
                    Price = float.Parse(data[3]),
                    LastClose = float.Parse(data[4]),
                    Open = float.Parse(data[5]),
                    High = float.Parse(data[33]),
                    Low = float.Parse(data[34]),
                    UpperLimit = float.Parse(data[47]),
                    LowerLimit = float.Parse(data[48]),
                    DateTime = DateTime.ParseExact(data[30], "yyyyMMddHHmmss",null) //此为快照生成时间，不是最新交易时间
                };
                this.dictLastTradeDate[symbol] = aTick.DateTime.Date;//当前交易日期
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
                FixedSizeTradeQueue tradeQueue=this.getTradeQueue(symbol);
                for (int k = tradeStrings.Length - 1; k >= 0; k--)
                {
                    string[] temp = tradeStrings[k].Split('/');
                    Trade aTrade = new Trade
                    {
                        DateTime = aTick.DateTime.Date.Add(TimeSpan.Parse(temp[0])),
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
                //aTick.DateTime = lastTrade.DateTime;
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

        public override List<Trade> HistoryTrades(string symbol, string startTime, string endTime="")
        {
            DateTime time1 = DateTime.Parse(startTime);
            DateTime time2 = DateTime.Now;
            if (endTime != "") time2 = DateTime.Parse(endTime);

            FixedSizeTradeQueue tradeQueue = this.getTradeQueue(symbol);
            List<Trade> ret = new List<Trade>();
            if (time1 < tradeQueue.MinTime || time1 > tradeQueue.MaxTime)
            {
                int startPage = -1;
                PageTimeList pageTimeList=this.getPageTimeList(symbol);
                startPage = pageTimeList.FindPageByTime(time1);//本地页表查找
                
                string tensentSymbol=this.getTencentSymbol(symbol);
                DateTime lastTradeDate = this.getLastTradeDate(symbol);
                string url = "http://stock.gtimg.cn/data/index.php?appn=detail&action=data&c="+ tensentSymbol;
                if (startPage == -1) startPage = this.searchStartPage(url,lastTradeDate,time1);//远程请求数据查找
               
                bool finished = false;
                while (!finished)
                {
                    Stream stream = this.webClient.OpenRead(url + String.Format("&p={0}",startPage));
                    StreamReader reader = new StreamReader(stream);
                    string dataString;
                    if ((dataString = reader.ReadLine()) != null)
                    {
                        string[] tradeStrings = dataString.Split('|');
                        int len = tradeStrings.Length;
                        if (len < 1) continue;
                        else
                        {
                            //保存分页时间表
                            string[] temp = tradeStrings[0].Split('/');
                            DateTime tempTime = lastTradeDate.Add(TimeSpan.Parse(temp[1]));
                            string pattern = @"\[(\d{1,}),";
                            Match mat = Regex.Match(temp[0], pattern);
                            if (mat.Groups.Count>1)
                            {
                                int page = int.Parse(mat.Groups[1].Value);
                                pageTimeList[page] = tempTime;
                            }
                        }
                        for (int k = 0; k < len; k++)
                        {
                            string[] temp = tradeStrings[k].Split('/');
                            Trade aTrade = new Trade
                            {
                                DateTime = lastTradeDate.Add(TimeSpan.Parse(temp[1])),
                                Price = float.Parse(temp[2]),
                                Volume = int.Parse(temp[4]) * 100,
                                BuyOrSell = temp[6][0],
                                Amount = double.Parse(temp[5])
                            };
                            if (aTrade.DateTime >= time1 && aTrade.DateTime <= time2) ret.Add(aTrade);
                            if (aTrade.DateTime > time2)
                            {
                                finished = true;
                                break;
                            }
                        }
                    }
                    else finished = true;
                    stream.Close();
                    startPage++;
                }
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

        

       
        private int searchStartPage(string url,DateTime lastTradeDate, DateTime timeToSearch)
        {
            int left = 0;
            int right = 100;
            while (left < right)
            {
                int mid = (left + right) / 2;
                Stream stream = this.webClient.OpenRead(url + String.Format("&p={0}", mid));
                StreamReader reader = new StreamReader(stream);
                string dataString;
                if ((dataString = reader.ReadLine()) != null)
                {
                    string[] tradeStrings = dataString.Split('|');
                    int len = tradeStrings.Length;
                    if (len < 1) continue;
                    string[] temp = tradeStrings[0].Split('/');
                    DateTime first = lastTradeDate.Add(TimeSpan.Parse(temp[1]));
                    if (timeToSearch < first)
                    {
                        right = mid - 1;
                    }
                    else
                    {
                        temp = tradeStrings[len-1].Split('/');
                        DateTime last = lastTradeDate.Add(TimeSpan.Parse(temp[1]));
                        if (timeToSearch <= last) return mid;
                        else left = mid+1;
                    }
                }
                else right=mid-1;
                stream.Close();
            }
            return left;
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
        private PageTimeList getPageTimeList(string symbol)
        {
            PageTimeList pageTimeList;
            if (!this.dictPageTimeList.TryGetValue(symbol, out pageTimeList))
            {
                pageTimeList = new PageTimeList();
                this.dictPageTimeList.Add(symbol, pageTimeList);
            }
            return pageTimeList;
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
