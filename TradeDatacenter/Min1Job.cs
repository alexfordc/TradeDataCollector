using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeDataCollector;
using TradeDataAccess;

namespace TradeDatacenter
{
    public class Min1Job:BaseDataJob
    {
        private string beginTime ="1970-01-01";
        private string endTime = "9999-01-01";
        public Min1Job(string methodName, string className, IEnumerable<string> symbols,string beginTime,string endTime) : base(methodName, className, symbols) {
            this.beginTime = beginTime;
            this.endTime = endTime;
        }
        public override object Do()
        {
            try
            {
                foreach (string symbol in this.symbols)
                {
                    object[] parameters = new object[] { symbol, 60, beginTime, endTime };
                    List<Bar> data = (List<Bar>)this.invokeMethod(parameters);
                    TradeDataAccessor.Store1MinBars(symbol, data);
                }
                return true;
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
