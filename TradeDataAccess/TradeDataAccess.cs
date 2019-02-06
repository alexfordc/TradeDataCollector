using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HuaQuant.TradeDataCollector;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;
namespace HuaQuant.TradeDataAccess
{
    public static class TradeDataAccessor
    {
        public static void SetRedisConnectString(string connStr)
        {
            RedisHelper.SetConnectString(connStr);
        }
        public static void SetInfluxConnectParameters(string influxUrl, string username, string password)
        {
            InfluxHelper.SetConnectParameters(influxUrl, username, password);
        }
        public static async void StoreInstruments(List<Instrument> instruments)
        {
            foreach(Instrument inst in instruments)
            {
                string key = inst.Symbol + ".Instrument";
                await RedisHelper.SetAsync(key, inst);
            }
        }
        public static async void StoreCurrentTicks(Dictionary<string,Tick> dictTicks) {
            foreach(KeyValuePair<string,Tick> kvp in dictTicks)
            {
                await RedisHelper.SetAsync(kvp.Key, kvp.Value);
            }
        }
        public static async void StoreTrades(string symbol,List<Trade> trades) {
            foreach(Trade aTrade in trades)
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
                await InfluxHelper.WriteAsync(aPoint, "Finance");
            }
        }
        public static async void StoreMin1Bars(string symbol,List<Bar> bars) {
            foreach(Bar aBar in bars)
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
                await InfluxHelper.WriteAsync(aPoint, "Finance");
            }
        }
        public static async void StoreDay1Bars(string symbol, List<Bar> bars) {
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
                await InfluxHelper.WriteAsync(aPoint, "Finance");
            }
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
            IEnumerable<Serie> series=InfluxHelper.QueryAsync(query, "Finance");
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

        public static void ClearRedis()
        {
            List<string> keys = RedisHelper.GetKeys("*");
            foreach (string key in keys) RedisHelper.Remove(key);
        }
    }
}
