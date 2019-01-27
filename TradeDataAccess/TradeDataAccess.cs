using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeDataCollector;
using InfluxData.Net.InfluxDb.Models;
namespace TradeDatacenter
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
                    Timestamp=aTrade.DateTime.ToUniversalTime(),
                };
                await InfluxHelper.WriteAsync(aPoint, "Finance");
            }
        }
        public static async void Store1MinBars(string symbol,List<Bar> bars) {
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
                    Timestamp = aBar.BeginTime.ToUniversalTime()
                };
                await InfluxHelper.WriteAsync(aPoint, "Finance");
            }
        }
        public static async void Store1DayBars(string symbol, List<Bar> bars) {
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
                    Timestamp = aBar.BeginTime.ToUniversalTime()
                };
                await InfluxHelper.WriteAsync(aPoint, "Finance");
            }
        }
    }
}
