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
        private string lastTime;
        
        public Min1Job(string methodName, string className, IEnumerable<string> symbols,DateTime? dataDate) : base("Min1Job",methodName, className, symbols,dataDate) {

            if (this.dataDate == null) this.dataDate = DateTime.Today;
            this.lastTime = Utils.DateTimeToString((DateTime)this.dataDate);
        }
        
        protected override bool doJob()
        {
            try
            {
                string beginTime = this.lastTime;
                string endTime = Utils.DateTimeToString(DateTime.Now);
                foreach (string symbol in this.symbols)
                {
                    object[] parameters = new object[] { symbol, 60, beginTime, endTime };
                    List<Bar> data = (List<Bar>)this.invokeMethod(parameters);
                    TradeDataAccessor.StoreMin1Bars(symbol, data);
                }
                this.lastTime = endTime;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
