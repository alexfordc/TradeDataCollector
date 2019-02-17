using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HuaQuant.TradeDataAccess;
using HuaQuant.TradeDataCollector;

namespace HuaQuant.TradeDatacenter
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            TradeDataAccessor.SetRedisConnectString(Properties.Settings.Default.RedisConnString);
            TradeDataAccessor.SetInfluxConnectParameters(Properties.Settings.Default.InfluxUrl,
                Properties.Settings.Default.InfluxUser, Properties.Settings.Default.InfluxPassword);
            TradeDataAccessor.DatabaseInit();
            GMCollector.SetToken(Properties.Settings.Default.GMToken);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
