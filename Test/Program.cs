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

            string[] testSymbols = new string[] { "SZSE.002361", "SHSE.603186"};
            //腾讯数据测试
            Console.WriteLine("tencent testing....");
            TencentCollector tc = new TencentCollector();
            Dictionary<string, Tick> data = tc.Current(testSymbols);
            foreach (KeyValuePair<string, Tick> kvp in data)
            {
                Console.WriteLine("{0}:{1}", kvp.Key, kvp.Value);
            }
            //Console.ReadKey();
            Console.WriteLine();
            //List<Trade> data1 = tc.HistoryTrades("SZSE.002361", "2019-01-15 11:26:50");
            //foreach (Trade trade in data1)
            //{
            //    Console.WriteLine(trade.ToString());
            //}
            //Console.ReadKey();
            //Console.WriteLine();
            //data1 = tc.HistoryTrades("SZSE.002361", "2019-01-15 11:26:50");
            //foreach (Trade trade in data1)
            //{
            //    Console.WriteLine(trade.ToString());
            //}
            //Console.ReadKey();
            //Console.WriteLine();
            //data1 = tc.HistoryTrades("SZSE.002361", "2019-01-15 11:26:50");
            //foreach (Trade trade in data1)
            //{
            //    Console.WriteLine(trade.ToString());
            //}
            //Console.ReadKey();
            //Console.WriteLine();
            //掘金数据测试
            //Console.WriteLine("gm testing....");
            //GMCollector gc = new GMCollector();
            //data = gc.Current(testSymbols);
            //foreach (KeyValuePair<string, Tick> kvp in data)
            //{
            //    Console.WriteLine("{0}:{1}", kvp.Key, kvp.Value);
            //}
            //Console.WriteLine();
            //data1 = gc.HistoryTrades("SZSE.002361", "2019-01-15 11:26:50");
            //foreach (Trade trade in data1)
            //{
            //    Console.WriteLine(trade.ToString());
            //}
            //Console.ReadKey();
            //Console.WriteLine();
            Console.WriteLine("neteasy testing....");
            NeteasyCollector nc = new NeteasyCollector();
            data = nc.Current(testSymbols);
            foreach (KeyValuePair<string, Tick> kvp in data)
            {
                Console.WriteLine("{0}:{1}", kvp.Key, kvp.Value);
            }
            Console.WriteLine();
            Console.WriteLine("sina testing....");
            SinaCollector sc = new SinaCollector();
            data = sc.Current(testSymbols);
            foreach (KeyValuePair<string, Tick> kvp in data)
            {
                Console.WriteLine("{0}:{1}", kvp.Key, kvp.Value);
            }
           
            EastMoneyCollector ec = new EastMoneyCollector();
            List<Trade> data1 = ec.LastDayTrades("SHSE.600025");
            foreach (Trade trade in data1)
            {
                Console.WriteLine(trade.ToString());
            }
            Console.ReadKey();
            Console.WriteLine();
        }
    }
}
