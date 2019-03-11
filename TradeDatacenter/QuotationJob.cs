using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HuaQuant.TradeDataCollector;
using HuaQuant.TradeDataAccess;

namespace HuaQuant.TradeDatacenter
{
    public class QuotationJob:BaseDataJob
    {
        public QuotationJob(string methodName, string className, IEnumerable<string> symbols,DateTime? dataDate) : base("QuotationJob", methodName, className, symbols,dataDate) { }

        public override bool Execute()
        {
            object[] parameters = new object[] { this.symbols };
            Dictionary<string, Tick> data = (Dictionary<string, Tick>)this.invokeMethod(parameters);
            int lost = this.symbols.Count() - data.Count;
            //Console.WriteLine("{0} get data {1} ,lost data {2}", this.Name, data.Count,lost);
            if (lost > 0) Console.WriteLine("{0} lost data {1}", this.Name, lost);
            TradeDataAccessor.StoreCurrentTicks(data);
            return true;
        }
    }
}
