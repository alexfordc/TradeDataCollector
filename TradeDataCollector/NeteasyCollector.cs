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
    public class NeteasyCollector:BaseCollector
    {
        private WebClient webClient;
        private Dictionary<string, string> dictGMToNeteasy = new Dictionary<string, string>();
        public NeteasyCollector()
        {
            this.webClient = new WebClient();

        }

        public override Dictionary<string, Tick> Current(IEnumerable<string> symbols)
        {
            Dictionary<string, Tick> ret = new Dictionary<string, Tick>();
            string url = "http://api.money.126.net/data/feed/";
            foreach (string symbol in symbols)
            {
                string neteasySymbol = this.getNeteasySymbol(symbol);
                url += neteasySymbol + ",";
            }
            List<string> tickStrings = new List<string>();
            Stream stream = this.webClient.OpenRead(url);
            StreamReader reader = new StreamReader(stream);
            string tickString;
            while ((tickString = reader.ReadLine()) != null) tickStrings.Add(tickString);
            stream.Close();
            string pattern = @"^_ntes_quote_callback\((.+)\);$";
            Match mat = Regex.Match(tickStrings[0], pattern);
            if (mat.Groups.Count > 1)
            {
                string json = mat.Groups[1].Value;
                JObject data = (JObject)JsonConvert.DeserializeObject(json);
                foreach (string symbol in symbols)
                {
                    if (!data.ContainsKey(this.dictGMToNeteasy[symbol])) continue;
                    JObject record =(JObject)data[this.dictGMToNeteasy[symbol]];
                    Tick aTick = new Tick
                    {
                        Price = (float)record["price"],
                        LastClose = (float)record["yestclose"],
                        Open = (float)record["open"],
                        High = (float)record["high"],
                        Low = (float)record["low"],
                       // UpperLimit = float.Parse(data[47]),
                       // LowerLimit = float.Parse(data[48]),
                        DateTime = DateTime.ParseExact(record["time"].ToString(), "yyyy/MM/dd HH:mm:ss", null)
                    };
                    
                    for (int k = 0; k < 5; k++)
                    {
                        
                        aTick.Quotes[k] = new Quote
                        {
                            BidPrice = (float)record[String.Format("bid{0}",k+1)],
                            BidVolume = (long)record[String.Format("bidvol{0}", k + 1)],
                            AskPrice = (float)record[String.Format("ask{0}", k + 1)],
                            AskVolume = (long)record[String.Format("askvol{0}", k + 1)]
                        };
                    }
                    aTick.CumVolume = (double)record["volume"];
                    aTick.CumAmount = (double)record["turnover"];
                    ret.Add(symbol, aTick);
                }
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
