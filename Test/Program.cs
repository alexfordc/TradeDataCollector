﻿using System;
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
            List<Trade> data1 = tc.HistoryTrades("SZSE.002361", "2019-01-11 14:56:50");
            foreach(Trade trade in data1)
            {
                Console.WriteLine(trade.ToString());
            }
            Console.ReadKey();
            Console.WriteLine();
            data1 = tc.HistoryTrades("SZSE.002361", "2019-01-11 14:56:00");
            foreach (Trade trade in data1)
            {
                Console.WriteLine(trade.ToString());
            }
            Console.ReadKey();
            Console.WriteLine();
            data1 = tc.HistoryTrades("SZSE.002361", "2019-01-11 14:56:00");
            foreach (Trade trade in data1)
            {
                Console.WriteLine(trade.ToString());
            }
            Console.ReadKey();
            Console.WriteLine();
            掘金数据测试
            GMCollector gc = new GMCollector();
            data = gc.Current(new string[] { "SZSE.002361", "SHSE.603186", });
            foreach (KeyValuePair<string, Tick> kvp in data)
            {
                Console.WriteLine("{0}:{1}", kvp.Key, kvp.Value);
            }
            Console.ReadKey();
            Console.WriteLine();
            data1 = gc.HistoryTrades("SZSE.002361", "2019-01-11 14:56:00");
            foreach (Trade trade in data1)
            {
                Console.WriteLine(trade.ToString());
            }
            Console.ReadKey();
            Console.WriteLine();

        }
    }
}
