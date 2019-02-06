using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuaQuant.TradeDataCollector
{
    [Serializable]
    public class Instrument
    {
        public string Symbol;
        public int Level;
        public bool IsSuspended;
        public double LastClose;
        public double UpperLimit;//涨停价
        public double LowerLimit;//跌停价
        public double AdjFactor;
        public DateTime CreatedAt;
    }
}
