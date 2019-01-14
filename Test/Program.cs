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

            //腾讯数据测试
            TencentCollector tc = new TencentCollector();
            Dictionary<string, Tick> data = tc.Current(new string[] { "SZSE.002361", "SHSE.603186", });
            foreach (KeyValuePair<string, Tick> kvp in data)
            {
                Console.WriteLine("{0}:{1}", kvp.Key, kvp.Value);
            }
            Console.ReadKey();
            Console.WriteLine();
            List<Trade> data1 = tc.HistoryTrades("SZSE.002361", "2019-01-14 14:56:50");
            foreach (Trade trade in data1)
            {
                Console.WriteLine(trade.ToString());
            }
            Console.ReadKey();
            Console.WriteLine();
            data1 = tc.HistoryTrades("SZSE.002361", "2019-01-14 14:56:00");
            foreach (Trade trade in data1)
            {
                Console.WriteLine(trade.ToString());
            }
            Console.ReadKey();
            Console.WriteLine();
            data1 = tc.HistoryTrades("SZSE.002361", "2019-01-14 14:56:00");
            foreach (Trade trade in data1)
            {
                Console.WriteLine(trade.ToString());
            }
            Console.ReadKey();
            Console.WriteLine();
            //掘金数据测试
            GMCollector gc = new GMCollector();
            data = gc.Current(new string[] { "SZSE.002361", "SHSE.603186", });
            foreach (KeyValuePair<string, Tick> kvp in data)
            {
                Console.WriteLine("{0}:{1}", kvp.Key, kvp.Value);
            }
            Console.ReadKey();
            Console.WriteLine();
            data1 = gc.HistoryTrades("SZSE.002361", "2019-01-14 14:56:00");
            foreach (Trade trade in data1)
            {
                Console.WriteLine(trade.ToString());
            }
            Console.ReadKey();
            Console.WriteLine();
            TdxHqAgent tha = TdxHqAgent.Instance;
            ReportArgs rags = tha.Connect("121.14.110.200", 443);
            if (rags.Succeeded)
            {
                Console.WriteLine(rags.Result);

                rags = tha.GetQuotes(new byte[] { 0, 1 }, new string[] { "002361", "603186" });
                if (rags.Succeeded)
                {
                    foreach (string[] record in (List<string[]>)rags.Result)
                    {
                        foreach (string field in record) Console.Write("{0}  ", field);
                        Console.WriteLine();
                    }
                }
                Console.ReadKey();
                Console.WriteLine();
                rags = tha.GetLastTrades(0, "002361", 0,60);
                if (rags.Succeeded)
                {
                    foreach (string[] record in (List<string[]>)rags.Result)
                    {
                        foreach (string field in record) Console.Write("{0}  ", field);
                        Console.WriteLine();
                    }
                }
                Console.ReadKey();
                Console.WriteLine();
                TdxCollector tdxc = new TdxCollector();
                data = tdxc.Current(new string[] { "SZSE.002361", "SHSE.603186", });
                foreach (KeyValuePair<string, Tick> kvp in data)
                {
                    Console.WriteLine("{0}:{1}", kvp.Key, kvp.Value);
                }
                Console.ReadKey();
                Console.WriteLine();

            }
        }
    }
}
