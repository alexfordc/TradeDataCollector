using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeDataCollector;
using TradeDatacenter;
using System.Reflection;
using InfluxData.Net.Common;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxData;
using InfluxData.Net.InfluxDb;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;
using InfluxData.Net.InfluxDb.ClientSubModules;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {

            //string[] testSymbols = new string[] { "SZSE.002434", "SZSE.888888", "SHSE.603186", "SHSE.000001" };
            ////腾讯数据测试
            //Console.WriteLine("tencent testing....");
            //TencentCollector tc = new TencentCollector();
            //Dictionary<string, Tick> data = tc.Current(testSymbols);
            //foreach (KeyValuePair<string, Tick> kvp in data)
            //{
            //    Console.WriteLine("{0}:{1}", kvp.Key, kvp.Value);
            //}
            //Console.ReadKey();
            //Console.WriteLine();
            ////List<Trade> data1 = tc.LastDayTrades("SZSE.002361");
            ////foreach (Trade trade in data1)
            ////{
            ////    Console.WriteLine(trade.ToString());
            ////}
            ////Console.ReadKey();
            ////Console.WriteLine();
            ////掘金数据测试
            //Console.WriteLine("gm testing....");
            //GMCollector gc = new GMCollector();
            //data = gc.Current(testSymbols);
            //foreach (KeyValuePair<string, Tick> kvp in data)
            //{
            //    Console.WriteLine("{0}:{1}", kvp.Key, kvp.Value);
            //}
            //Console.WriteLine();
            ////data1 = gc.HistoryTrades("SZSE.002361", "2019-01-18 11:26:50");
            ////foreach (Trade trade in data1)
            ////{
            ////    Console.WriteLine(trade.ToString());
            ////}
            ////Console.ReadKey();
            ////Console.WriteLine();
            //Console.WriteLine("neteasy testing....");
            //NeteasyCollector nc = new NeteasyCollector();
            //data = nc.Current(testSymbols);
            //foreach (KeyValuePair<string, Tick> kvp in data)
            //{
            //    Console.WriteLine("{0}:{1}", kvp.Key, kvp.Value);
            //}
            //Console.WriteLine();
            //Console.WriteLine("sina testing....");
            //SinaCollector sc = new SinaCollector();
            //data = sc.Current(testSymbols);
            //foreach (KeyValuePair<string, Tick> kvp in data)
            //{
            //    Console.WriteLine("{0}:{1}", kvp.Key, kvp.Value);
            //}

            //Console.WriteLine("eastmoney testing....");
            //EastMoneyCollector ec = new EastMoneyCollector();
            ////data1 = ec.LastDayTrades("SZSE.002361");
            ////foreach (Trade trade in data1)
            ////{
            ////    Console.WriteLine(trade.ToString());
            ////}
            ////data = ec.Current(testSymbols);
            //foreach (KeyValuePair<string, Tick> kvp in data)
            //{
            //    Console.WriteLine("{0}:{1}", kvp.Key, kvp.Value);
            //}
            //Console.WriteLine();
            //Console.ReadKey();
            //Console.WriteLine();
            //RedisHelper.Set("test1", 123);
            //RedisHelper.Set("test2", 456);
            //RedisHelper.Set("test3", 789);
            //RedisHelper.Set("sr", 321);


            //foreach (var key in RedisHelper.GetKeys( "*test*"))
            //{
            //    Console.WriteLine(key);
            //}

            //Console.ReadKey();
            //Console.WriteLine();
            //IJob job = new Min1Job("HistoryBars", "TradeDataCollector.SinaCollector", new string[] { "SHSE.600025" }, DateTime.Today);
             string influxUrl = "http://localhost:8086/";
             string username = "admin";
             string password = "admin";
             InfluxDbClient _instance = new InfluxDbClient(influxUrl, username, password, InfluxDbVersion.Latest);
            string query = "select * from \"Bar.60\" where \"Symbol\"='SHSE.600025'" ;
            var series = _instance.Client.QueryAsync(query, "Finance").Result;
            Console.WriteLine(series.Count());
            foreach(var serie in series)
            {
                Console.WriteLine(serie.Values.Count());
            }
            Console.ReadKey();
            IEnumerable<SerieSet> ret = _instance.Serie.GetSeriesAsync("Finance").Result;
            foreach (SerieSet ss in ret)
            {
                Console.WriteLine(ss.Name);
                Console.WriteLine(ss.Series[0].Key);
            }
            Console.ReadKey();
        }
    }
}
