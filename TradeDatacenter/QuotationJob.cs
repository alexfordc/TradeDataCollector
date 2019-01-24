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
        public QuotationJob(string methodName, string className, object[] parameters) : base(methodName, className, parameters) { };
        public override object Do()
        {
            try
            {
                Dictionary<string, Tick> data = (Dictionary<string, Tick>)base.Do();
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
