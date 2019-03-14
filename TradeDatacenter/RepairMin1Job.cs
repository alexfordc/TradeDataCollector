using System;
using System.Collections.Generic;
using System.Threading;
using HuaQuant.TradeDataCollector;
using HuaQuant.TradeDataAccess;
namespace HuaQuant.TradeDatacenter
{
    public class RepairMin1Job : BaseDataJob
    {
        public RepairMin1Job(string methodName, string className, IEnumerable<string> symbols, DateTime? dataDate) : base("RepairMin1Job", methodName, className, symbols, dataDate)
        {
            if (this.dataDate == null) this.dataDate = DateTime.Today;
        }
        public override bool Execute(CancellationToken token)
        {
            string beginTime = Utils.DateTimeToString((DateTime)this.dataDate);
            string endTime = Utils.DateTimeToString(((DateTime)this.dataDate).Date.AddDays(1));
            foreach (string symbol in this.symbols)
            {
                token.ThrowIfCancellationRequested();
                object[] parameters = new object[] { symbol, 60, beginTime, endTime };
                List<Bar> data = (List<Bar>)this.invokeMethod(parameters);
                if (data.Count > 0)
                {
                    TradeDataAccessor.StoreMin1Bars(symbol, data);
                    Console.WriteLine("{0}：{1} 得到数据 {2} 条", this.Name, symbol, data.Count);
                }
                else
                {
                    Console.WriteLine("{0}：{1} 没有数据", this.Name, symbol);
                }
            }
            return true;
        }
    }
}
