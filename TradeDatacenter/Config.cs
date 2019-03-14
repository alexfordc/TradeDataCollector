using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace HuaQuant.TradeDatacenter
{
    public class Config
    {
        
        public List<DataJobConfig> DataJobConfigs;
        public string Markets;
        private Config() { }

        public static Config ReadFromFile(string fileName)
        {
            FileStream fileStream = new FileStream(fileName, FileMode.Open);
            TextReader textReader = new StreamReader(fileStream);
            JsonTextReader jsonTextReader = new JsonTextReader(textReader);
            JsonSerializer serializer = new JsonSerializer();
            Config con = (Config)serializer.Deserialize(jsonTextReader, typeof(Config));
            return con;
        }
    }
    public class DataJobConfig
    {
        public string ClassName;
        public TriggerConfig Trigger;
        public int MaxTaskNumber = 1;
        public List<DataCollector> DataCollectors;
        public List<DataJobConfig> SubJobs=null;
    }
    public class DataCollector
    {
        public string ClassName;
        public string MothedName;
        public float Weight;
    }
    public class TriggerConfig
    {
        public string TimeInterval = null;
        public string BeginTime = null;
        public string EndTime = null;
        public int? Times = null;
    }
}
