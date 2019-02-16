using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HuaQuant.TradeDataCollector;
using HuaQuant.TradeDataAccess;
namespace HuaQuant.TradeDatacenter
{
    class DailyJob:BaseDataJob
    {
        public DailyJob(string methodName, string className, IEnumerable<string> symbols, DateTime? dataDate) : base("DailyJob", methodName, className, symbols, dataDate)
        {
            if (this.dataDate == null) this.dataDate = DateTime.Today;
        }
        protected override bool doJob()
        {
            string beginTime = Utils.DateTimeToString((DateTime)this.dataDate);
            string endTime = Utils.DateTimeToString(((DateTime)this.dataDate).Date.AddDays(1));
            foreach (string symbol in this.symbols)
            {
                object[] parameters = new object[] { symbol, 86400, beginTime, endTime };
                List<Bar> data = (List<Bar>)this.invokeMethod(parameters);
                if (data.Count > 0)
                {
                    TradeDataAccessor.StoreDay1Bars(symbol,data);
                    Console.WriteLine("{0} get data {1} of {2}", this.Name, data.Count, symbol);
                }
            }
            Console.WriteLine("{0} run {1} times", this.Name, this.Times+1);
            return true;
        }
    }
}
