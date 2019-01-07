using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeDataCollector
{
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
    }
}
