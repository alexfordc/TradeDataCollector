using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeDataCollector
{
    public class TdxCollector : BaseCollector
    {
        internal static DateTime startTimeUTC = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(2004, 1, 1));
        private TdxHqAgent tdxHq;
        private bool isConnected = false;
        private Dictionary<string, TdxSymbol> dictGMToTdx = new Dictionary<string, TdxSymbol>();
        public TdxCollector()
        {
            tdxHq = TdxHqAgent.Instance;
            ReportArgs rags = tdxHq.Connect("121.14.110.200", 443);
            if (rags.Succeeded)
            {
                this.isConnected = true;
                Console.WriteLine("Tdx data connected!");
            }
            else
            {
                this.isConnected = false;
                Console.WriteLine(rags.ErrorInfo);
            }
        }
        public override Dictionary<string, Tick> Current(IEnumerable<string> symbols)
        {
            Dictionary<string, Tick> ret = new Dictionary<string, Tick>();
            if (this.isConnected)
            {
                short count = (short)symbols.Count();
                if (count <= 0) return ret;
                byte[] marketIDs = new byte[count];
                string[] securityIDs = new string[count];
                short i = 0;
                foreach(string symbol in symbols)
                {
                    TdxSymbol tdxSymbol = this.getTdxSymbol(symbol);
                    marketIDs[i] = tdxSymbol.MarketID;
                    securityIDs[i] = tdxSymbol.SecurityID;
                    i++;
                }
                ReportArgs reportArgs = tdxHq.GetQuotes(marketIDs, securityIDs);
                if (reportArgs.Succeeded)
                {
                    List<string[]> data = (List<string[]>)reportArgs.Result;
                    for(int j= 1;j <data.Count;j++)
                    {
                        string[] record=data[j];
                        Tick aTick = new Tick();
                        aTick.Price= float.Parse(record[3]);
                        aTick.LastClose= float.Parse(record[4]);
                        aTick.Open= float.Parse(record[5]);
                        aTick.High= float.Parse(record[6]);
                        aTick.Low= float.Parse(record[7]);
                        aTick.DateTime=DateTime.Parse(record[8]);
                        aTick.CumVolume= double.Parse(record[10])*100;
                        aTick.Volume= int.Parse(record[11])*100;
                        aTick.CumAmount = double.Parse(record[12]);
                        for(int k = 0; k < 5; k++)
                        {
                            aTick.Quotes[k] = new Quote
                            {
                                BidPrice = float.Parse(record[17 + k * 4]),
                                BidVolume = long.Parse(record[19 + k * 4])*100,
                                AskPrice = float.Parse(record[18 + k * 4]),
                                AskVolume = long.Parse(record[20 + k * 4])*100
                            };
                        }
                        byte marketID = byte.Parse(record[0]);
                        string securityID = record[1];
                        ret.Add(Utils.TdxToGM(marketID, securityID), aTick);
                    }
                }else
                {
                    Console.WriteLine(reportArgs.ErrorInfo);
                }
                return ret;
            }
            else throw new Exception("Tdx data is not connected!");
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

        private TdxSymbol getTdxSymbol(string symbol)
        {
            TdxSymbol tdxSymbol;
            if (!this.dictGMToTdx.TryGetValue(symbol,out tdxSymbol))
            {
                tdxSymbol = Utils.GMToTdx(symbol);
                this.dictGMToTdx.Add(symbol, tdxSymbol);
            }
            return tdxSymbol;
        }
    }
    
}
