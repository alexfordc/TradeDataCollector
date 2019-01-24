using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeDataCollector;
using TradeDataAccess;

namespace TradeDatacenter
{
    public class Min1Job:BaseDataJob
    {
        public Min1Job(string methodName, string className, object[] parameters) : base(methodName, className, parameters) { };
        public override object Do()
        {
            try
            {
                List<Bar> data=(List<Bar>)base.Do();
                //TradeDataAccessor.Store1MinBars()
                return true;
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
