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

namespace HuaQuant.TradeDataCollector
{
    public class NeteasyCollector:ICollector
    {
        private WebClient webClient;
        private int batchSize = 50;
        private Dictionary<string, string> dictGMToNeteasy = new Dictionary<string, string>();
        public NeteasyCollector()
        {
            this.webClient = new WebClient();

        }

        public Dictionary<string, Tick> Current(IEnumerable<string> symbols)
        {
            Dictionary<string, Tick> ret = new Dictionary<string, Tick>();
            string baseUrl = "http://api.money.126.net/data/feed/";
            int i = 0, si = 0;
            string symbolsString = "";
            List<string> tickStrings = new List<string>();
            try
            {
                foreach (string symbol in symbols)
                {
                    string neteasySymbol = this.getNeteasySymbol(symbol);
                    symbolsString += neteasySymbol + ",";
                    i++; si++;
                    if (i >= this.batchSize || si >= symbols.Count())
                    {
                        Stream stream = this.webClient.OpenRead(baseUrl + symbolsString);
                        StreamReader reader = new StreamReader(stream);
                        string tickString;
                        while ((tickString = reader.ReadLine()) != null) tickStrings.Add(tickString);
                        stream.Close();
                        symbolsString = "";
                        i = 0;
                    }
                }
                string pattern = @"^_ntes_quote_callback\((.+)\);$";
                foreach (string tickString in tickStrings)
                {
                    Match mat = Regex.Match(tickString, pattern);
                    if (mat.Groups.Count > 1)
                    {
                        string json = mat.Groups[1].Value;
                        JObject data = (JObject)JsonConvert.DeserializeObject(json);
                        foreach (string symbol in symbols)
                        {
                            if (!data.ContainsKey(this.dictGMToNeteasy[symbol])) continue;
                            JObject record = (JObject)data[this.dictGMToNeteasy[symbol]];
                            Tick aTick = new Tick
                            {
                                Price = Utils.ParseFloat((string)record["price"]),
                                LastClose = Utils.ParseFloat((string)record["yestclose"]),
                                Open = Utils.ParseFloat((string)record["open"]),
                                High = Utils.ParseFloat((string)record["high"]),
                                Low = Utils.ParseFloat((string)record["low"]),
                                // UpperLimit = float.Parse(data[47]),
                                // LowerLimit = float.Parse(data[48]),
                                DateTime = Utils.StringToDateTime(record["time"].ToString(), "NETEASY")
                            };

                            for (int k = 0; k < 5; k++)
                            {
                                aTick.Quotes[k] = new Quote
                                {
                                    BidPrice = Utils.ParseFloat((string)record[String.Format("bid{0}", k + 1)]),
                                    BidVolume = Utils.ParseLong((string)record[String.Format("bidvol{0}", k + 1)]),
                                    AskPrice = Utils.ParseFloat((string)record[String.Format("ask{0}", k + 1)]),
                                    AskVolume = Utils.ParseLong((string)record[String.Format("askvol{0}", k + 1)])
                                };
                            }
                            aTick.CumVolume = Utils.ParseDouble((string)record["volume"]);
                            aTick.CumAmount = Utils.ParseDouble((string)record["turnover"]);
                            aTick.Source = "Neteasy";
                            ret.Add(symbol, aTick);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}的{1}发生错误：{2}", ex.Source, ex.TargetSite.Name, ex.Message);
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
            throw new NotImplementedException();
        }

        private string getNeteasySymbol(string symbol)
        {
            string neteasySymbol;
            if (!this.dictGMToNeteasy.TryGetValue(symbol, out neteasySymbol))
            {
                neteasySymbol = Utils.GMToNeteasy(symbol);
                this.dictGMToNeteasy[symbol] = neteasySymbol;
            }
            return neteasySymbol;
        }
    }
}
