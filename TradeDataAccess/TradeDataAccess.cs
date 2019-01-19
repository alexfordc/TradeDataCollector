using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeDataAccess
{
    public class TradeDataAccess
    {
        public static void SetRedisConnectString(string connStr)
        {
            RedisHelper.SetConnectString(connStr);
        }
        public static void SetInfluxConnectParameters(string influxUrl, string username, string password)
        {
            InfluxHelper.SetConnectParameters(influxUrl, username, password);
        }
        public static void StoreCurrentTicks() { }
        public static void StoreTicks() { }
        public static void Store1MinBars() { }
        public static void Store1DayBars() { }
    }
}
