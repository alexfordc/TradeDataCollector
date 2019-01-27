using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeDataCollector;

namespace TradeDatacenter
{
    public class Min1Job:BaseDataJob,IJob
    {
        private string lastTime;
        
        public Min1Job(string methodName, string className, IEnumerable<string> symbols,DateTime? dataDate) : base(methodName, className, symbols,dataDate) {

            if (this.dataDate == null) this.dataDate = DateTime.Today;
            this.lastTime = Utils.DateTimeToString((DateTime)this.dataDate);
        }
        
        public bool Execute()
        {
            try
            {
                string beginTime = this.lastTime;
                string endTime = Utils.DateTimeToString(DateTime.Now);
                foreach (string symbol in this.symbols)
                {
                    object[] parameters = new object[] { symbol, 60, beginTime, endTime };
                    List<Bar> data = (List<Bar>)this.invokeMethod(parameters);
                    TradeDataAccessor.Store1MinBars(symbol, data);
                }
                this.lastTime = endTime;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
