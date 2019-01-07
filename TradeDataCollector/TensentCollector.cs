using System;
using System.Collections.Generic;
using System.Net;
using System.IO;

namespace TradeDataCollector
{
    public class TensentCollector : BaseCollector
    {
        private WebClient webClient;
        private Dictionary<string, string> dictGMToTensent = new Dictionary<string, string>();
        
        public TensentCollector()
        {
            this.webClient = new WebClient();
        }
        public override Dictionary<string, Tick> Current(List<string> symbols)
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

            for (int i = 0; i < symbols.Count; i++)
            {
                string[] data = tickStrings[i].Split('~');
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
                for (int k = 0; i < 5; i++) { 
                    aTick.Quotes[k] = new Quote
                    {
                        BidPrice = float.Parse(data[9+k*2]),
                        BidVolume = long.Parse(data[10+k*2]) * 100,
                        AskPrice = float.Parse(data[19+k*2]),
                        AskVolume = long.Parse(data[20+k*2]) * 100
                    };
                }
                string[] temp = data[35].Split('/');
                aTick.CumVolume = double.Parse(temp[1]) * 100;
                aTick.CumAmount = double.Parse(temp[2]);
                string[] tradeStrings = data[29].Split('|');
                List<Trade> lastTrades = new List<Trade>();

                foreach(string tradeString in tradeStrings)
                {
                    temp = tradeString.Split('/');
                    Trade aTrade = new Trade
                    {
                        DateTime = DateTime.Today.Add(TimeSpan.Parse(temp[0])),
                        Price = float.Parse(temp[1]),
                        Volume = int.Parse(temp[2]),
                        BuyOrSell = temp[3][0],
                        Amount = double.Parse(temp[4])
                    };
                    lastTrades.Add(aTrade);
                }
                aTick.Volume = lastTrades[0].Volume;
                aTick.Amount = lastTrades[0].Amount;
                aTick.BuyOrSell = lastTrades[0].BuyOrSell;
                ret.Add(symbols[i], aTick);
            }
            return ret;
            //throw new NotImplementedException();
        }

        public override List<Bar> HistoryBars(string symbol, int size, string startTime, string endTime)
        {
            throw new NotImplementedException();
        }

        public override List<Bar> HistoryBarsN(string symbol, int size, int n, string endTime)
        {
            throw new NotImplementedException();
        }

        public override List<Trade> HistoryTicks(string symbol, string startTime, string endTime)
        {
            throw new NotImplementedException();
        }

        public override List<Trade> HistoryTicksN(string symbol, int n, string endTime)
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
