using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace TradeDataCollector
{
    public class SinaCollector : ICollector
    {
        private WebClient webClient;
        private int batchSize = 100;
        private Dictionary<string, string> dictGMToSina = new Dictionary<string, string>();
        public SinaCollector()
        {
            this.webClient = new WebClient();
        }
        public Dictionary<string, Tick> Current(IEnumerable<string> symbols)
        {
            Dictionary<string, Tick> ret = new Dictionary<string, Tick>();
            string baseUrl = "http://hq.sinajs.cn/list=";
            int i = 0, si = 0;
            string symbolsString = "";
            List<string> tickStrings = new List<string>();
            try
            {
                foreach (string symbol in symbols)
                {
                    string sinaSymbol = this.getSinaSymbol(symbol);
                    symbolsString += sinaSymbol + ",";
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
                i = 0;
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
                        aTick.DateTime = Utils.StringToDateTime(data[30] + " " + data[31], "SINA");
                        aTick.CumVolume = Utils.ParseDouble(data[8]);
                        aTick.CumAmount = Utils.ParseDouble(data[9]);
                        aTick.Source = "Sina";
                        ret.Add(symbol, aTick);
                    }
                    i++;
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
