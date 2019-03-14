using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        
        public override bool Execute(CancellationToken token)
        {           
            foreach (string symbol in this.symbols)
            {
                token.ThrowIfCancellationRequested();
                string beginTime = this.lastTimes[symbol];
                string endTime = Utils.DateTimeToString(DateTime.Now);
                object[] parameters = new object[] { symbol, 60, beginTime, endTime };
                List<Bar> data = (List<Bar>)this.invokeMethod(parameters);
                
                if (data.Count > 0)
                {
                    this.lastTimes[symbol] = Utils.DateTimeToString(data.Last().BeginTime);
                    TradeDataAccessor.BatchStoreMin1Bars(symbol, data);
                }
            }
            Console.WriteLine("{0}：在 {1} 时请求完一遍分线", this.Name, DateTime.Now);
            return true;
        }
    }
}
