using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeDataCollector
{
    public class Quote
    {
        public float BidPrice;
        public long BidVolume;
        public float AskPrice;
        public long AskVolume;

        public override string ToString()
        {
            string str = String.Format("BidPrice:{0},BidVolume:{1},AskPrice:{2},AskVolume:{3}",
                BidPrice, BidVolume, AskPrice, AskVolume);
            return str;
        }
    }

}
