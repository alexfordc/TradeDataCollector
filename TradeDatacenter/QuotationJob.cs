using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeDataCollector;
using TradeDataAccess;

namespace TradeDatacenter
{
    public class QuotationJob:BaseDataJob,IJob
    {
        public QuotationJob(string methodName, string className, IEnumerable<string> symbols,DateTime? dataDate) : base("QuotationJob", methodName, className, symbols,dataDate) { }
        
        public bool Execute()
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
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
