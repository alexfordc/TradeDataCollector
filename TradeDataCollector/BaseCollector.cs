using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeDataCollector
{
    public abstract class BaseCollector
    {
        public abstract Dictionary<string, Tick> Current(IEnumerable<string> symbols);
        public abstract List<Trade> HistoryTicks(string symbol, string startTime, string endTime);
        public abstract List<Trade> HistoryTicksN(string symbol, int n, string endTime);
        public abstract List<Bar> HistoryBars(string symbol, int size,string startTime, string endTime);
        public abstract List<Bar> HistoryBarsN(string symbol, int size, int n, string endTime);
    }
}
