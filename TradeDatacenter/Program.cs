using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace TradeDatacenter
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Config con = Config.ReadFromFile("config.json");
            Console.WriteLine(con.DataMotheds[0].Name);
            Console.WriteLine(con.DataMotheds[0].CallInterval);
            Console.WriteLine(con.DataMotheds[0].ImplementClasses[0].Name);
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new MainForm());
           
        }
    }
}
