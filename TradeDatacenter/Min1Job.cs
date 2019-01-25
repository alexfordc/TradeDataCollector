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
        private string lastTime;
        
        public Min1Job(string methodName, string className, IEnumerable<string> symbols) : base(methodName, className, symbols) {
            this.lastTime = DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public override object Execute()
        {
            try
            {
                string beginTime = this.lastTime;
                string endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                foreach (string symbol in this.symbols)
                {
                    object[] parameters = new object[] { symbol, 60, beginTime, endTime };
                    List<Bar> data = (List<Bar>)this.invokeMethod(parameters);
                    TradeDataAccessor.Store1MinBars(symbol, data);
                }
                this.lastTime = endTime;
                return true;
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
