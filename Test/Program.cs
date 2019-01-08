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
            TensentCollector tc = new TensentCollector();
            Dictionary<string, Tick> data = tc.Current(new string[] { "SZSE.600001" ,"SHSE.600025", });
            foreach(KeyValuePair<string,Tick> kvp in data)
            {
                Console.WriteLine("{0}:{1}", kvp.Key, kvp.Value);
            }
            Console.ReadKey();
        }
    }
}
