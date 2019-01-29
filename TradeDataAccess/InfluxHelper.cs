using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfluxData.Net.Common;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxData;
using InfluxData.Net.InfluxDb;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;
using InfluxData.Net.InfluxDb.ClientSubModules;

namespace TradeDataAccess
{
    public class InfluxHelper
    {
        private static string influxUrl = "http://localhost:8086/";
        private static string username = "admin";
        private static string password = "admin";
        private static object _locker = new Object();
        private static InfluxDbClient _instance = null;

        public static InfluxDbClient Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new InfluxDbClient(influxUrl, username, password, InfluxDbVersion.Latest);
                        }
                    }
                }
                return _instance;
            }
        }

        static InfluxHelper() { }

        public static void SetConnectParameters(string influxUrl,string username,string password)
        {
            InfluxHelper.influxUrl = influxUrl;
            InfluxHelper.username = username;
            InfluxHelper.password = password;
        }
        public static async Task WriteAsync(Point pointToWrite,string dbName)
        {
            await Instance.Client.WriteAsync(pointToWrite,dbName);
        }
        public static IEnumerable<Serie> QueryAsync(string query,string dbName,object parameters=null)
        {
            IEnumerable<Serie> response;
            if (parameters != null)
            {
                response = Instance.Client.QueryAsync(query,parameters,dbName).Result;
            }
            else
            {
                response = Instance.Client.QueryAsync(query,dbName).Result;
            }
            return response;
        }
        public static async Task CreateDatabaseAsync(string dbName)
        {
            await Instance.Database.CreateDatabaseAsync(dbName);
        }
        public static async Task<IEnumerable<Database>> GetDatabaseAsync()
        {
            IEnumerable<Database> response = await Instance.Database.GetDatabasesAsync();
            return response;
        }
        public static IBatchWriter CreateBatchWriter(string dbName)
        {
            IBatchWriter batchWriter = Instance.Serie.CreateBatchWriter("yourDbName");
            return batchWriter;
        }
    }
}
