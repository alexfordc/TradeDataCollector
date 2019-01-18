using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace TradeDataCollector
{
    public class SinaCollector : BaseCollector
    {
        private WebClient webClient;
        private Dictionary<string, string> dictGMToSina = new Dictionary<string, string>();
        public SinaCollector()
        {
            this.webClient = new WebClient();
        }
        public override Dictionary<string, Tick> Current(IEnumerable<string> symbols)
        {
            Dictionary<string, Tick> ret = new Dictionary<string, Tick>();
            string url = "http://hq.sinajs.cn/list=";
            foreach (string symbol in symbols)
            {
                string sinaSymbol = this.getSinaSymbol(symbol);
                url += sinaSymbol + ",";
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
                string[] data = tickStrings[i].Split(',');
                if (!data[0].Contains(this.dictGMToSina[symbol])) continue;
                if (data.Length >= 31)//保证有数据
                {
                    Tick aTick = new Tick
                    {
                        Price = Utils.ParseFloat(data[3]),
                        LastClose = Utils.ParseFloat(data[2]),
                        Open = Utils.ParseFloat(data[1]),
                        High = Utils.ParseFloat(data[4]),
                        Low = Utils.ParseFloat(data[5]),
                    };

                    for (int k = 0; k < 5; k++)
                    {
                        aTick.Quotes[k] = new Quote
                        {
                            BidPrice = Utils.ParseFloat(data[11 + k * 2]),
                            BidVolume = Utils.ParseLong(data[10 + k * 2]),
                            AskPrice = Utils.ParseFloat(data[21 + k * 2]),
                            AskVolume = Utils.ParseLong(data[20 + k * 2])
                        };
                    }
                    aTick.DateTime = DateTime.Parse(data[30] + " " + data[31]);
                    aTick.CumVolume = Utils.ParseDouble(data[8]);
                    aTick.CumAmount = Utils.ParseDouble(data[9]);
                    ret.Add(symbol, aTick);
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
            throw new NotImplementedException();
        }

        private string getSinaSymbol(string symbol)
        {
            string sinaSymbol;
            if (!this.dictGMToSina.TryGetValue(symbol, out sinaSymbol))
            {
                sinaSymbol = Utils.GMToTencent(symbol);
                this.dictGMToSina[symbol] = sinaSymbol;
            }
            return sinaSymbol;
        }
    }
}
