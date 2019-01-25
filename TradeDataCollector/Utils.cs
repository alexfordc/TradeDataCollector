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
        public static string GMToEastMoney(string symbol)
        {
            string newSymbol;
            string[] temp = symbol.Split('.');
            byte marketID = 0;
            switch (temp[0])
            {
                case "SHSE":
                    marketID = 1;
                    break;
                case "SZSE":
                    marketID = 2;
                    break;
            }
            newSymbol = temp[1]+String.Format("{0}", marketID);
            return newSymbol;
        }
        public static float ParseFloat(string dataStr)
        {
            if (float.TryParse(dataStr, out float ret)) return ret;
            else return 0;
        }
        public static double ParseDouble(string dataStr)
        {
            if (double.TryParse(dataStr, out double ret)) return ret;
            else return 0.0;
        }
        public static int ParseInt(string dataStr)
        {
            if (int.TryParse(dataStr, out int ret)) return ret;
            else return 0;
        }
        public static long ParseLong(string dataStr)
        {
            if (long.TryParse(dataStr, out long ret)) return ret;
            else return 0L;
        }

        public static string DateTimeToString(DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public static string DateToString(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }
        public static DateTime StringToDateTime(string timeStr,string key)
        {
            switch (key)
            {
                case "SINA":
                case "EASTMONEY":
                    return DateTime.ParseExact(timeStr, "yyyy-MM-dd HH:mm:ss", null);
                case "NETEASY":
                    return DateTime.ParseExact(timeStr, "yyyy/MM/dd HH:mm:ss", null);
                case "TENCENT":
                    return DateTime.ParseExact(timeStr, "yyyyMMddHHmmss", null);
                default:
                    return DateTime.Parse(timeStr);
            }
        }
    }
}
