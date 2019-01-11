using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TradeDataCollector
{
    public class TensentCollector : BaseCollector
    {
        private WebClient webClient;
        private Dictionary<string,FixedSizeTradeQueue> dictTradeCache=new Dictionary<string, FixedSizeTradeQueue>();
        private Dictionary<string, string> dictGMToTensent = new Dictionary<string, string>();
        private Dictionary<string, SortedList<int, DateTime>> dictPageTimeList = new Dictionary<string, SortedList<int, DateTime>>();
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
                int startPage = -1;
                SortedList<int, DateTime> pageTimeList;
                if (this.dictPageTimeList.TryGetValue(symbol,out pageTimeList))
                {
                    startPage = this.searchStartPage(pageTimeList, time1);//本地页表查找
                }
                else
                {
                    pageTimeList = new SortedList<int, DateTime>();
                    this.dictPageTimeList.Add(symbol, pageTimeList);
                }
                string tensentSymbol;
                if (!this.dictGMToTensent.TryGetValue(symbol, out tensentSymbol))
                {
                    tensentSymbol = this.ConvertGMSymbolToTensent(symbol);
                    this.dictGMToTensent[symbol] = tensentSymbol;
                }
                string url = "http://stock.gtimg.cn/data/index.php?appn=detail&action=data&c="+ tensentSymbol;
                if (startPage == -1)
                {
                    startPage = this.searchStartPage(url, time1);//远程请求数据查找
                }
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
                            string[] temp = tradeStrings[0].Split('/');
                            DateTime tempTime = DateTime.Today.Add(TimeSpan.Parse(temp[1]));
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
                                DateTime = DateTime.Today.Add(TimeSpan.Parse(temp[1])),
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

        public override List<Trade> HistoryTicksN(string symbol, int n, string endTime="")
        {
            throw new NotImplementedException();
        }

        private string ConvertGMSymbolToTensent(string gmSymbol)
        {
            string[] strArray = gmSymbol.Split('.');
            return strArray[0].Substring(0, 2).ToLower() + strArray[1];
        }

        private int searchStartPage(SortedList<int,DateTime> pageTimeList,DateTime time)
        {
            int left = pageTimeList.Keys.First();
            int right = pageTimeList.Keys.Last();
          
            while (left < right)
            {
                int mid = (left + right) / 2;
                DateTime cur = pageTimeList[mid];
                if (time < cur)
                {
                    right = mid-1;
                }else
                {                   
                    DateTime next=pageTimeList[mid+1];
                    if (time<next) return mid;
                    else left = mid+1;  
                }
            }
            if (time < pageTimeList[left]) return -1;
            else return left;
        }
        private int searchStartPage(string url, DateTime time)
        {
            int left = 0;
            int right = 100;
            while (left < right)
            {
                int mid = (left + right) / 2;
                Stream stream = this.webClient.OpenRead(url + String.Format("&p={0}", mid));
                //Console.WriteLine(url + String.Format("&p={0}", mid));
                StreamReader reader = new StreamReader(stream);
                string dataString;
                if ((dataString = reader.ReadLine()) != null)
                {
                    string[] tradeStrings = dataString.Split('|');
                    int len = tradeStrings.Length;
                    if (len < 1) continue;
                    string[] temp = tradeStrings[0].Split('/');
                    DateTime first = DateTime.Today.Add(TimeSpan.Parse(temp[1]));
                    if (time < first)
                    {
                        right = mid - 1;
                    }
                    else
                    {
                        temp = tradeStrings[len-1].Split('/');
                        DateTime last = DateTime.Today.Add(TimeSpan.Parse(temp[1]));
                        if (time<=last) return mid;
                        else left = mid+1;
                    }
                }
                else right=mid-1;
                stream.Close();
            }
            return left;
        }
    }
}
