using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeDataCollector
{
    public class FixedSizeTradeQueue:SortedList<DateTime,Trade>
    {
        public DateTime MaxTime
        {
            get
            {
                if (this.Count > 0) return this.Keys.Last();
                else return new DateTime(1970, 1, 1);
            }
        }
        public DateTime MinTime
        {
            get
            {
                if (this.Count > 0) return this.Keys.First();
                else return new DateTime(9999, 1, 1);
            }
        }
        public Trade LastTrade
        {
            get
            {
                if (this.Count > 0) return this.Values.Last();
                else return null;
            }
        }
        public FixedSizeTradeQueue(int capacity = 10)
        {
            this.Capacity = capacity;
        }
        public void Add(Trade aTrade)
        {
            if (!this.ContainsKey(aTrade.DateTime))
            {
                while (this.Count >= this.Capacity) this.RemoveAt(0);
                base.Add(aTrade.DateTime, aTrade);
            }
        }
        
    }
}
