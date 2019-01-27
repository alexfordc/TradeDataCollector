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
    public class EastMoneyCollector : ICollector
    {
        private WebClient webClient;
        private Dictionary<string, string> dictGMToEastMoney = new Dictionary<string, string>();
        private Dictionary<string, DateTime> dictLastTradeDate = new Dictionary<string, DateTime>();
        public EastMoneyCollector()
        {
            this.webClient = new WebClient();
        }
        public Dictionary<string, Tick> Current(IEnumerable<string> symbols)
        {
            Dictionary<string, Tick> ret = new Dictionary<string, Tick>();
            string url = "http://nuff.eastmoney.com/EM_Finance2015TradeInterface/JS.ashx?id=";
            List<string> tickStrings = new List<string>();
            //只能一个个的请求，无法成批发送请求
            foreach (string symbol in symbols)
            {
                string eastMoneySymbol = this.getEastMoneySymbol(symbol);
                Stream stream = this.webClient.OpenRead(url+eastMoneySymbol);
                StreamReader reader = new StreamReader(stream);
                string tickString;
                if ((tickString = reader.ReadLine()) != null) tickStrings.Add(tickString);
                stream.Close();
            }
            int i = 0;
            foreach (string symbol in symbols)
            {
                if (i >= tickStrings.Count) break;
                string pattern = @"^callback\((.+)\)$";
                Match mat = Regex.Match(tickStrings[i], pattern);
                if (mat.Groups.Count > 1)
                {
                    string json = mat.Groups[1].Value;
                    json.Replace("-", "");
                    JObject obj = (JObject)JsonConvert.DeserializeObject(json);
                    JArray data = (JArray)obj["Value"];
                    if (data.Count >= 30) //保证有数据
                    {
                        if ((string)data[1] != symbol.Substring(5)) continue;
                        Tick aTick = new Tick
                        {
                            UpperLimit = Utils.ParseFloat((string)data[23]),
                            LowerLimit = Utils.ParseFloat((string)data[24]),
                            Price = Utils.ParseFloat((string)data[25]),
                            Open = Utils.ParseFloat((string)data[28]),
                            High = Utils.ParseFloat((string)data[30]),
                            CumVolume = Utils.ParseDouble((string)data[31]) * 100,
                            Low = Utils.ParseFloat((string)data[32]),
                            Volume = Utils.ParseInt((string)data[33]) * 100,
                            LastClose = Utils.ParseFloat((string)data[34]),
                            CumAmount = Utils.ParseDouble(((string)data[35]).Replace('万', ' '))*10000,
                        };
                        for (int k = 0; k < 5; k++)
                        {
                            aTick.Quotes[k] = new Quote
                            {
                                BidPrice = Utils.ParseFloat((string)data[3 + k]),
                                BidVolume = Utils.ParseLong((string)data[13 + k]) * 100,
                                AskPrice = Utils.ParseFloat((string)data[8 + k]),
                                AskVolume = Utils.ParseLong((string)data[18 + k]) * 100
                            };
                        }
                        aTick.DateTime = Utils.StringToDateTime((string)data[49], "EASTMONEY");
                        this.dictLastTradeDate[symbol] = aTick.DateTime.Date;//当前交易日期
                        ret.Add(symbol, aTick);
                    }
                }
                i++;
            }
            return ret;
        }

        public List<Bar> HistoryBars(string symbol, int size, string startTime, string endTime = "")
        {
            throw new NotImplementedException();
        }

        public List<Bar> HistoryBarsN(string symbol, int size, int n, string endTime = "")
        {
            throw new NotImplementedException();
        }

        public List<Trade> HistoryTrades(string symbol, string startTime, string endTime = "")
        {
            throw new NotImplementedException();
        }

        public List<Trade> HistoryTradesN(string symbol, int n, string endTime = "")
        {
            throw new NotImplementedException();
        }

        public List<Trade> LastDayTrades(string symbol)
        {
            List<Trade> ret = new List<Trade>();
            string url = "http://mdfm.eastmoney.com/EM_UBG_MinuteApi/Js/Get?dtype=all&rows=10000&id="
                + this.getEastMoneySymbol(symbol);
            DateTime lastTradeDate = this.getLastTradeDate(symbol);
            Stream stream = this.webClient.OpenRead(url);
            StreamReader reader = new StreamReader(stream);
            string dataString;
            if ((dataString = reader.ReadToEnd()) != "")
            {
                string pattern = @"^\((.+)\)$";
                Match mat = Regex.Match(dataString, pattern);
              
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
                                Price = Utils.ParseFloat((string)temp[1]),
                                Volume = Utils.ParseInt((string)temp[2])*100,
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
