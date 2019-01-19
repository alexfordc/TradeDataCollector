using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace TradeDataAccess
{
    public class Config
    {
        public string StartTime;
        public string EndTime;
        public List<DataMethod> DataMethods;

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
    public class DataMethod
    {
        public string Name;
        public int CallInterval;
        public List<ImplementClass> ImplementClasses;
        
    }
    public class ImplementClass
    {
        public string Name;
        public float Weights;
    }
}
