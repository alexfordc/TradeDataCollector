using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeDataCollector
{
    public static class Utils
    {
        public static TdxSymbol GMToTdx(string symbol)
        {
            TdxSymbol tdxSymbol = new TdxSymbol();
            string[] temp = symbol.Split('.');
            string market = temp[0];
            switch (market)
            {
                case "SHSE":
                    tdxSymbol.MarketID = 1;
                    break;
                case "SZSE":
                    tdxSymbol.MarketID = 0;
                    break;
            }
            tdxSymbol.SecurityID = temp[1];
            return tdxSymbol;
        }
        public static string TdxToGM(byte marketID,string securityID)
        {
            string exchange = "";
            switch (marketID)
            {
                case 0:
                    exchange = "SZSE";
                    break;
                case 1:
                    exchange = "SHSE";
                    break;
            }
            return exchange + '.' + securityID;
        }
        public static string GMToTencent(string symbol)
        {
            string tensentSymbol;
            string[] strArray = symbol.Split('.');
            tensentSymbol = strArray[0].Substring(0, 2).ToLower() + strArray[1];
            return tensentSymbol;
        }
    }

    public struct TdxSymbol
    {
        public byte MarketID;
        public string SecurityID;
    }
}
