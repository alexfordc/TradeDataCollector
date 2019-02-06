using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuaQuant.TradeDataCollector
{
    public interface ICollector
    {
        Dictionary<string, Tick> Current(IEnumerable<string> symbols);
        List<Trade> HistoryTrades(string symbol, string startTime, string endTime="");
        List<Trade> HistoryTradesN(string symbol, int n, string endTime="");
        List<Bar> HistoryBars(string symbol, int size,string startTime, string endTime="");
        List<Bar> HistoryBarsN(string symbol, int size, int n, string endTime="");
        List<Trade> LastDayTrades(string symbol);
    }
}
