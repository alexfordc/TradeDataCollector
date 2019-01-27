using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeDataCollector
{
    [Serializable]
    public class Tick
    {
        public DateTime DateTime;
        public float LastClose;
        public float Open;
        public float High;
        public float Low;
        public float Price;
        public int Volume;
        public double Amount;
        public double CumVolume;
        public double CumAmount;
        public char BuyOrSell;//买卖标志
        public float UpperLimit;//涨停价
        public float LowerLimit;//跌停价
        public Quote[] Quotes=new Quote[5];//五档报价

        public override string ToString()
        {
            string str = String.Format("DateTime:{0},LastClose:{1},Open:{2},High:{3},Low:{4},Price:{5}," +
                "Volume:{6},Amount:{7},CumVolume:{8},CumAmount:{9},BuyOrSell:{10},UpperLimit:{11}," +
                "LowerLimit:{12},", DateTime, LastClose, Open, High, Low, Price, Volume, Amount, CumVolume, CumAmount
                , BuyOrSell, UpperLimit, LowerLimit);
            str += "Quotes:[";
            for(int i= 0; i<Quotes.Length;i++)
            {
                str +='{'+ Quotes[i].ToString()+'}';
                if (i < Quotes.Length - 1) str += ',';
            }
            str += ']';
            return str;
        }
    }
}
