using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HuaQuant.TradeDataCollector;
using HuaQuant.TradeDataAccess;

namespace HuaQuant.TradeDatacenter
{
    public class Min1Job:BaseDataJob
    {
        private Dictionary<string, string> lastTimes = new Dictionary<string, string>();

        public Min1Job(string methodName, string className, IEnumerable<string> symbols,DateTime? dataDate) : base("Min1Job",methodName, className, symbols,dataDate) {

            if (this.dataDate == null) this.dataDate = DateTime.Today;
            string lastTime = Utils.DateTimeToString((DateTime)this.dataDate);
            foreach(string symbol in symbols)
            {
                this.lastTimes.Add(symbol, lastTime);
            }
        }
        
        protected override bool doJob()
        {
            string endTime = Utils.DateTimeToString(DateTime.Now);
            foreach (string symbol in this.symbols)
            {
                string beginTime = this.lastTimes[symbol];
                object[] parameters = new object[] { symbol, 60, beginTime, endTime };
                List<Bar> data = (List<Bar>)this.invokeMethod(parameters);
                
                if (data.Count > 0)
                {
                    this.lastTimes[symbol] = Utils.DateTimeToString(data.Last().BeginTime);
                    TradeDataAccessor.BatchStoreMin1Bars(symbol, data);
                    //Console.WriteLine("{0} get data {1}", this.Name, data.Count);
                }
            }
            Console.WriteLine("{0} run {1} times", this.Name,this.Times);
            return true;
        }
    }
}
