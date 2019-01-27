﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace TradeDatacenter
{
    public class Config
    {
        
        public List<DataJobConfig> DataJobConfigs;

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
        public int CallInterval;
        public string BeginTime=null;
        public string EndTime=null;
        public int Times=0;
        public string TimeSpan=null;
        public List<DataCollector> DataCollectors; 
    }
    public class DataCollector
    {
        public string ClassName;
        public string MothedName;
        public float Weight;
    }
}
