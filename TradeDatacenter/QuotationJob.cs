using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using HuaQuant.TradeDataCollector;
using HuaQuant.TradeDataAccess;

namespace HuaQuant.TradeDatacenter
{
    public class QuotationJob:BaseDataJob
    {
        public QuotationJob(string methodName, string className, IEnumerable<string> symbols,DateTime? dataDate) : base("QuotationJob", methodName, className, symbols,dataDate) { }

        public override bool Execute(CancellationToken token)
        {
            int batchSize = 50;
            IEnumerable<string> remains = this.symbols;
            IEnumerable<string> currents;
            do
            {
                token.ThrowIfCancellationRequested();
                currents = remains.Take(batchSize);
                object[] parameters = new object[] { currents };
                Dictionary<string, Tick> data = (Dictionary<string, Tick>)this.invokeMethod(parameters);
                int lost = currents.Count() - data.Count;
                if (lost > 0) Console.WriteLine("{0}：丢失数据 {1} 条", this.Name, lost);
                TradeDataAccessor.StoreCurrentTicks(data);
                remains = remains.Skip(batchSize);
            } while (currents.Count() == batchSize);
            return true;
        }
    }
}
