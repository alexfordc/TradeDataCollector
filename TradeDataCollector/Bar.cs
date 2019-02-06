using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuaQuant.TradeDataCollector
{
    [Serializable]
    public class Bar
    {
        public DateTime BeginTime;
        public DateTime EndTime;
        public float LastClose;
        public float Open;
        public float High;
        public float Low;
        public float Close;
        public double Volume;
        public double Amount;
        public int Size;
    }
}
