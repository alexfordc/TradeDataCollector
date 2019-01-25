using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeDataCollector;
using TradeDataAccess;

namespace TradeDatacenter
{
    public class QuotationJob:BaseDataJob
    {
        public QuotationJob(string methodName, string className, IEnumerable<string> symbols) : base(methodName, className, symbols) { }
        public override object Execute()
        {
            try
            {
                object[] parameters = new object[] { this.symbols };
                Dictionary<string, Tick> data = (Dictionary<string, Tick>)this.invokeMethod(parameters);
                TradeDataAccessor.StoreCurrentTicks(data);
                return true;
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }
    }
}
