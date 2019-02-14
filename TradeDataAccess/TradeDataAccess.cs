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
        private static IBatchWriter batchWriter = null;
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
            await InfluxHelper.WriteAsync(points, "Finance");
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
            await InfluxHelper.WriteAsync(points, "Finance");
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
            await InfluxHelper.WriteAsync(points, "Finance");
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
            string query =string.Format("select * from \"Bar.60\" where \"Symbol\"=\'{0}\'",symbol);
            IEnumerable<Serie> series=InfluxHelper.Query(query, "Finance");
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
            IEnumerable<Serie> series = InfluxHelper.Query(query, "Finance");
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

        public static void StartBatchWriter()
        {
            if (batchWriter == null)
            {
                lock (_locker)
                {
                    if (batchWriter == null)
                    {
                        batchWriter = InfluxHelper.CreateBatchWriter("Finance");
                        batchWriter.OnError += BatchWriter_OnError;
                        batchWriter.Start(1000, true,10000);
                    }
                }
            }
        }

        public static void StopBatchWriter()
        {
            if (batchWriter != null)
            {
                lock (_locker)
                {
                    if (batchWriter != null)
                    {
                        batchWriter.Stop();
                        batchWriter = null;
                    }
                }
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
            batchWriter.AddPoints(points);
        }
        // OnError handler method
        private static void BatchWriter_OnError(object sender, Exception e)
        {
            // Handle the error here
            Console.WriteLine("在{0}中:{1}",sender.ToString(),e.InnerException.Message);
        }
    }
}
