using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HuaQuant.TradeDataCollector;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;
using InfluxData.Net.InfluxDb.ClientSubModules;
namespace HuaQuant.TradeDataAccess
{
    public static class TradeDataAccessor
    {
        private static Dictionary<string, IBatchWriter> batchWriters = new Dictionary<string, IBatchWriter>();
        private static object _locker = new Object();
        public static void SetRedisConnectString(string connStr)
        {
            RedisHelper.SetConnectString(connStr);
        }
        public static void SetInfluxConnectParameters(string influxUrl, string username, string password)
        {
            InfluxHelper.SetConnectParameters(influxUrl, username, password);
        }
        public static async Task StoreInstruments(List<Instrument> instruments)
        {
            foreach(Instrument inst in instruments)
            {
                string key = inst.Symbol + ".Instrument";
                await RedisHelper.SetAsync(key, inst);
            }
        }
        public static async Task StoreCurrentTicks(Dictionary<string,Tick> dictTicks) {
            foreach(KeyValuePair<string,Tick> kvp in dictTicks)
            {
                await RedisHelper.SetAsync(kvp.Key, kvp.Value);
            }
        }
        public static async Task StoreTrades(string symbol,List<Trade> trades) {
            List<Point> points = new List<Point>();
            foreach (Trade aTrade in trades)
            {
                Point aPoint = new Point
                {
                    Name = "Trade",
                    Fields = new Dictionary<string, object>
                    {
                        {"Price",aTrade.Price },
                        {"Volume",aTrade.Volume },
                        {"Amount",aTrade.Amount }
                    },
                    Tags=new Dictionary<string, object>
                    {
                        {"Symbol",symbol }
                    },
                    Timestamp=aTrade.DateTime
                };
                points.Add(aPoint);
            }
            await InfluxHelper.WriteAsync(points, dbName);
        }
        public static async Task StoreMin1Bars(string symbol, List<Bar> bars)
        {
            List<Point> points = new List<Point>();
            foreach (Bar aBar in bars)
            {
                Point aPoint = new Point
                {
                    Name = "Bar.60",
                    Fields = new Dictionary<string, object>
                    {
                        {"LastClose",aBar.LastClose},
                        {"Open",aBar.Open },
                        {"High",aBar.High },
                        {"Low",aBar.Low },
                        {"Close",aBar.Close },
                        {"Volume",aBar.Volume },
                        {"Amount",aBar.Amount }
                    },
                    Tags = new Dictionary<string, object>
                    {
                        {"Symbol",symbol }
                    },
                    Timestamp = aBar.BeginTime
                };
                points.Add(aPoint);
            }
            await InfluxHelper.WriteAsync(points, dbName,min1BarPolicyName);
        }

        public static async Task StoreDay1Bars(string symbol, List<Bar> bars)
        {
            List<Point> points = new List<Point>();
            foreach (Bar aBar in bars)
            {
                Point aPoint = new Point
                {
                    Name = "Bar.Daily",
                    Fields = new Dictionary<string, object>
                    {
                        {"LastClose",aBar.LastClose},
                        {"Open",aBar.Open },
                        {"High",aBar.High },
                        {"Low",aBar.Low },
                        {"Close",aBar.Close },
                        {"Volume",aBar.Volume },
                        {"Amount",aBar.Amount }
                    },
                    Tags = new Dictionary<string, object>
                    {
                        {"Symbol",symbol }
                    },
                    Timestamp = aBar.BeginTime
                };
                points.Add(aPoint);
            }
            await InfluxHelper.WriteAsync(points, dbName);
        }

        public static List<Instrument> GetInstruments()
        {
            List<Instrument> ret = new List<Instrument>();
            List<string> keys = RedisHelper.GetKeys("??SE.??????.Instrument");
            foreach(string key in keys)
            {
                ret.Add((Instrument)RedisHelper.Get(key));
            }
            return ret;
        }
        public static Dictionary<string,Tick> GetCurrentTicks()
        {
            Dictionary<string, Tick> ret = new Dictionary<string, Tick>();
            List<string> keys = RedisHelper.GetKeys("??SE.??????");
            foreach (string key in keys)
            {
                ret.Add(key, (Tick)RedisHelper.Get(key));
            }
            return ret;
        }
        public static List<Bar> GetMin1Bars(string symbol)
        {
            string query =string.Format("select * from \"{0}\".\"Bar.60\" where \"Symbol\"=\'{1}\'",min1BarPolicyName,symbol);
            IEnumerable<Serie> series=InfluxHelper.Query(query, dbName);
            List<Bar> ret = new List<Bar>();
            foreach(Serie serie in series)
            {
                foreach(var value in serie.Values)
                {
                    Bar aBar = new Bar
                    {
                        BeginTime = Convert.ToDateTime(value[0]),
                        Amount = Utils.ParseDouble(value[1].ToString()),
                        Close = Utils.ParseFloat(value[2].ToString()),
                        High = Utils.ParseFloat(value[3].ToString()),
                        LastClose = Utils.ParseFloat(value[4].ToString()),
                        Low = Utils.ParseFloat(value[5].ToString()),
                        Open = Utils.ParseFloat(value[6].ToString()),
                        Volume = Utils.ParseDouble(value[8].ToString()),
                        Size = 60
                    };
                    ret.Add(aBar);
                }
            }
            return ret;
        }
        public static List<Bar> GetDay1Bars(string symbol)
        {
            string query = string.Format("select * from \"Bar.Daily\" where \"Symbol\"=\'{0}\'", symbol);
            IEnumerable<Serie> series = InfluxHelper.Query(query, dbName);
            List<Bar> ret = new List<Bar>();
            foreach (Serie serie in series)
            {
                foreach (var value in serie.Values)
                {
                    Bar aBar = new Bar
                    {
                        BeginTime = Convert.ToDateTime(value[0]),
                        Amount = Utils.ParseDouble(value[1].ToString()),
                        Close = Utils.ParseFloat(value[2].ToString()),
                        High = Utils.ParseFloat(value[3].ToString()),
                        LastClose = Utils.ParseFloat(value[4].ToString()),
                        Low = Utils.ParseFloat(value[5].ToString()),
                        Open = Utils.ParseFloat(value[6].ToString()),
                        Volume = Utils.ParseDouble(value[8].ToString()),
                        Size = 86400
                    };
                    ret.Add(aBar);
                }
            }
            return ret;
        }
        public static void ClearRedis()
        {
            List<string> keys = RedisHelper.GetKeys("*");
            foreach (string key in keys) RedisHelper.Remove(key);
        }

        private static IBatchWriter StartBatchWriter(string policyName)
        {
            IBatchWriter batchWriter = null;
            lock (_locker)
            {
                if (!batchWriters.ContainsKey(policyName))
                {
                    batchWriter = InfluxHelper.CreateBatchWriter(dbName, policyName);
                    batchWriter.OnError += BatchWriter_OnError;
                    batchWriter.Start(1000, true);
                    batchWriters.Add(policyName, batchWriter);
                }else
                {
                    batchWriter = batchWriters[policyName];
                }
            }
            return batchWriter;
        }

        public static void StopBatchWriters()
        {
            foreach(KeyValuePair<string,IBatchWriter> kvp in batchWriters)
            {
                kvp.Value.Stop();
            }
            lock (batchWriters)
            {
                batchWriters.Clear();
            }
        }

        public static void BatchStoreMin1Bars(string symbol, List<Bar> bars)
        {
            List<Point> points = new List<Point>();
            foreach (Bar aBar in bars)
            {
                Point aPoint = new Point
                {
                    Name = "Bar.60",
                    Fields = new Dictionary<string, object>
                    {
                        {"LastClose",aBar.LastClose},
                        {"Open",aBar.Open },
                        {"High",aBar.High },
                        {"Low",aBar.Low },
                        {"Close",aBar.Close },
                        {"Volume",aBar.Volume },
                        {"Amount",aBar.Amount }
                    },
                    Tags = new Dictionary<string, object>
                    {
                        {"Symbol",symbol }
                    },
                    Timestamp = aBar.BeginTime
                };
                points.Add(aPoint);
            }
            IBatchWriter batchWirter = null;
            if (!batchWriters.TryGetValue(min1BarPolicyName,out batchWirter))
            {
                batchWirter =StartBatchWriter(min1BarPolicyName);
            }
            batchWirter.AddPoints(points);
        }
        // OnError handler method
        private static void BatchWriter_OnError(object sender, Exception e)
        {
            // Handle the error here
            Console.WriteLine("在{0}中:{1}",sender.ToString(),e.InnerException.Message);
        }

        private static string dbName = "Stock";
        private static string min1BarPolicyName = "OneYear";
        private static string tradePolicyName = "OneMonth";
        public static void DatabaseInit()
        {
            
            IEnumerable<Database> databases = InfluxHelper.GetDatabase();
            bool has = false;
            foreach(Database database in databases)
            {
                if (database.Name == dbName)
                {
                    has = true;
                    break;
                }
            }
            if (!has)
            {
                if (InfluxHelper.CreateDatabase(dbName))
                    Console.WriteLine("Database {0} created.", dbName);
                if (InfluxHelper.CreateRetentionPolicy(dbName, min1BarPolicyName, "365d", 1))
                    Console.WriteLine("Retention policy {0} created.", min1BarPolicyName);
                if (InfluxHelper.CreateRetentionPolicy(dbName, tradePolicyName, "30d", 1))
                    Console.WriteLine("Retention policy {0} created.", tradePolicyName);
            }
        }
    }
}
