using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeDataCollector;
namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {

            string[] testSymbols = new string[] { "SZSE.002361","SZSE.888888", "SHSE.603186","SHSE.000001"};
            //腾讯数据测试
            Console.WriteLine("tencent testing....");
            TencentCollector tc = new TencentCollector();
            //Dictionary<string, Tick> data = tc.Current(testSymbols);
            //foreach (KeyValuePair<string, Tick> kvp in data)
            //{
            //    Console.WriteLine("{0}:{1}", kvp.Key, kvp.Value);
            //}
            //Console.ReadKey();
            //Console.WriteLine();
            List<Trade> data1 = tc.LastDayTrades("SZSE.002361");
            //foreach (Trade trade in data1)
            //{
            //    Console.WriteLine(trade.ToString());
            //}
            //Console.ReadKey();
            Console.WriteLine();
            //掘金数据测试
            //Console.WriteLine("gm testing....");
            //GMCollector gc = new GMCollector();
            //data = gc.Current(testSymbols);
            //foreach (KeyValuePair<string, Tick> kvp in data)
            //{
            //    Console.WriteLine("{0}:{1}", kvp.Key, kvp.Value);
            //}
            //Console.WriteLine();
            //data1 = gc.HistoryTrades("SZSE.002361", "2019-01-18 11:26:50");
            //foreach (Trade trade in data1)
            //{
            //    Console.WriteLine(trade.ToString());
            //}
            //Console.ReadKey();
            //Console.WriteLine();
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

            Console.WriteLine("eastmoney testing....");
            EastMoneyCollector ec = new EastMoneyCollector();
            data1 = ec.LastDayTrades("SZSE.002361");
            //foreach (Trade trade in data1)
            //{
            //    Console.WriteLine(trade.ToString());
            //}
            //data = ec.Current(testSymbols);
            //foreach (KeyValuePair<string, Tick> kvp in data)
            //{
            //    Console.WriteLine("{0}:{1}", kvp.Key, kvp.Value);
            //}
            //Console.WriteLine();
            Console.ReadKey();
            Console.WriteLine();
        }
    }
}
