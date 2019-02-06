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
        
        protected override bool doJob()
        {
            try
            {
                object[] parameters = new object[] { this.symbols };
                Dictionary<string, Tick> data = (Dictionary<string, Tick>)this.invokeMethod(parameters);
                int lost = this.symbols.Count() - data.Count;
                if (lost > 0) Console.WriteLine("{0} lost data {1}", this.type.Name, lost);
                TradeDataAccessor.StoreCurrentTicks(data);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
