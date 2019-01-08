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
            TensentCollector tc = new TensentCollector();
            Dictionary<string, Tick> data = tc.Current(new string[] { "SZSE.002361", "SHSE.603186", });
            foreach (KeyValuePair<string, Tick> kvp in data)
            {
                Console.WriteLine("{0}:{1}", kvp.Key, kvp.Value);
            }
            Console.ReadKey();
            Console.WriteLine();
            //掘金数据测试
            GMCollector gc = new GMCollector();
            Dictionary<string, Tick>  data1 = gc.Current(new string[] { "SZSE.002361", "SHSE.603186", });
            foreach (KeyValuePair<string, Tick> kvp in data1)
            {
                Console.WriteLine("{0}:{1}", kvp.Key, kvp.Value);
            }
            Console.ReadKey();
        }
    }
}
