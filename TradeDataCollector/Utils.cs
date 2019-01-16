using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeDataCollector
{
    public static class Utils
    {       
        public static string GMToTencent(string symbol)
        {
            string[] temp = symbol.Split('.');
            temp[0] = temp[0].Substring(0, 2).ToLower();
            return string.Join("", temp);
        }
        public static string GMToSina(string symbol)
        {
            string[] temp = symbol.Split('.');
            temp[0] = temp[0].Substring(0, 2).ToLower();
            return string.Join("", temp);
        }
        public static string GMToNeteasy(string symbol)
        {
            string newSymbol;
            string[] temp = symbol.Split('.');
            byte marketID = 0;
            switch (temp[0])
            {
                case "SHSE":
                    marketID = 0;
                    break;
                case "SZSE":
                    marketID = 1;
                    break;
            }
            newSymbol = String.Format("{0}", marketID) + temp[1];
            return newSymbol;
        }
    }
}
