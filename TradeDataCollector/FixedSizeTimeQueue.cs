using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeDataCollector
{
    public class FixedSizeTradeQueue:SortedList<DateTime,Trade>
    {
        public FixedSizeTradeQueue(int capacity = 10)
        {
            this.Capacity = capacity;
        }
        public bool Add(Trade aTrade)
        {
            if (this.Count > 0 && aTrade.DateTime <= this.Keys.Last()) return false;
            else
            {
                while (this.Count >= this.Capacity) this.RemoveAt(0);
                base.Add(aTrade.DateTime, aTrade);
                return true;
            }
        }
        
    }
}
