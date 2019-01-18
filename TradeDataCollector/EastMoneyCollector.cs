using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TradeDataCollector
{
    public class EastMoneyCollector : BaseCollector
    {
        private WebClient webClient;
        private Dictionary<string, string> dictGMToEastMoney = new Dictionary<string, string>();
        private Dictionary<string, DateTime> dictLastTradeDate = new Dictionary<string, DateTime>();
        public EastMoneyCollector()
        {
            this.webClient = new WebClient();
        }
        public override Dictionary<string, Tick> Current(IEnumerable<string> symbols)
        {
            Dictionary<string, Tick> ret = new Dictionary<string, Tick>();
            string url = "http://nuff.eastmoney.com/EM_Finance2015TradeInterface/JS.ashx?id=";
            List<string> tickStrings = new List<string>();
            //只能一个个的请求，无法成批发送请求
            foreach (string symbol in symbols)
            {
                string eastMoneySymbol = this.getEastMoneySymbol(symbol);
                url += eastMoneySymbol ;
                Stream stream = this.webClient.OpenRead(url);
                StreamReader reader = new StreamReader(stream);
                string tickString;
                if  ((tickString = reader.ReadLine()) != null) tickStrings.Add(tickString);
                stream.Close();
            }
            int i = 0;
            foreach (string symbol in symbols)
            {
                if (i >= tickStrings.Count) break;
                string[] data = tickStrings[i].Split(',');
                if (data[2] != symbol.Substring(5)) continue;
                if (data.Length>=30) //保证有数据
                {
                    Tick aTick = new Tick
                    {
                        UpperLimit = float.Parse(data[24]),
                        LowerLimit = float.Parse(data[25]),
                        Price = float.Parse(data[26]),
                        Open = float.Parse(data[29]),
                        High = float.Parse(data[31]),
                        CumVolume = double.Parse(data[32]) * 100,
                        Low = float.Parse(data[33]),
                        Volume = int.Parse(data[34]) * 100,
                        LastClose = float.Parse(data[35]),
                        CumAmount = double.Parse(data[36]),
                    };
                    for (int k = 0; k < 5; k++)
                    {
                        aTick.Quotes[k] = new Quote
                        {
                            BidPrice = float.Parse(data[4 + k ]),
                            BidVolume = long.Parse(data[14 + k ]) * 100,
                            AskPrice = float.Parse(data[9 + k ]),
                            AskVolume = long.Parse(data[19 + k ]) * 100
                        };
                    }
                    aTick.DateTime = DateTime.ParseExact(data[50], "yyyy-MM-dd HH:mm:ss", null);
                    this.dictLastTradeDate[symbol] = aTick.DateTime.Date;//当前交易日期
                    ret.Add(symbol,aTick);
                }
                i++;
            }
            return ret;
        }

        public override List<Bar> HistoryBars(string symbol, int size, string startTime, string endTime = "")
        {
            throw new NotImplementedException();
        }

        public override List<Bar> HistoryBarsN(string symbol, int size, int n, string endTime = "")
        {
            throw new NotImplementedException();
        }

        public override List<Trade> HistoryTrades(string symbol, string startTime, string endTime = "")
        {
            throw new NotImplementedException();
        }

        public override List<Trade> HistoryTradesN(string symbol, int n, string endTime = "")
        {
            throw new NotImplementedException();
        }

        public override List<Trade> LastDayTrades(string symbol)
        {
            List<Trade> ret = new List<Trade>();
            string url = "http://mdfm.eastmoney.com/EM_UBG_MinuteApi/Js/Get?dtype=all&rows=10000&id="
                + this.getEastMoneySymbol(symbol);
            DateTime lastTradeDate = this.getLastTradeDate(symbol);
            Stream stream = this.webClient.OpenRead(url);
            StreamReader reader = new StreamReader(stream);
            string dataString;
            if ((dataString = reader.ReadToEnd()) != null)
            {
                string pattern = @"^\((.+)\)$";
                Match mat = Regex.Match(dataString, pattern);
                Console.WriteLine(mat.Groups.Count);
                if (mat.Groups.Count > 1)
                {
                    string json = mat.Groups[1].Value;
                    JObject data = (JObject)JsonConvert.DeserializeObject(json);
                    //Console.WriteLine(data.ToString());
                    if ((string)data["message"] == "ok")
                    {
                        JArray tradeStrings = (JArray)data["value"]["data"];
                        foreach(string tradeStr in tradeStrings)
                        {
                            string[] temp = tradeStr.Split(',');
                            Trade aTrade = new Trade
                            {
                                DateTime = lastTradeDate.Add(TimeSpan.Parse(temp[0])),
                                Price = float.Parse(temp[1]),
                                Volume = int.Parse(temp[2]),
                            };
                            switch(int.Parse(temp[3])){
                                case 1:
                                    aTrade.BuyOrSell = 'S';
                                    break;
                                case 2:
                                    aTrade.BuyOrSell = 'B';
                                    break;
                                default:
                                    aTrade.BuyOrSell = 'N';
                                    break;
                            }
                            ret.Add(aTrade);
                        }
                    }
                }
            }
            return ret;
        }
        private string getEastMoneySymbol(string symbol)
        {
            string eastMoneySymbol;
            if (!this.dictGMToEastMoney.TryGetValue(symbol, out eastMoneySymbol))
            {
                eastMoneySymbol = Utils.GMToEastMoney(symbol);
                this.dictGMToEastMoney[symbol] = eastMoneySymbol;
            }
            return eastMoneySymbol;
        }
        private DateTime getLastTradeDate(string symbol)
        {
            DateTime lastTradeDate;
            if (!this.dictLastTradeDate.TryGetValue(symbol, out lastTradeDate))
            {
                lastTradeDate = DateTime.Today;
                this.dictLastTradeDate.Add(symbol, lastTradeDate);
            }
            return lastTradeDate;
        }
    }
}
