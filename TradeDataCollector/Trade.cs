using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeDataCollector
{
    public class Trade
    {
        public DateTime DateTime;
        public float Price;
        public int Volume;
        public double Amount;
        public char BuyOrSell;
        public override string ToString()
        {
            string str = String.Format("DateTime:{0},Price:{1},Volume:{2},Amount:{3},BuyOrSell:{4}",
                DateTime, Price, Volume, Amount, BuyOrSell);
            return str;
        }
    }
}
