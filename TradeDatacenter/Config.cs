using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace TradeDatacenter
{
    public class Config
    {
        public int RefreshInterval;
        public string StartTime;
        public string EndTime;
        public List<DataSource> DataSources;

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
    public class DataSource
    {
        public string Name;
        public List<string> Motheds;
        public List<float> Weights;
    }
}
