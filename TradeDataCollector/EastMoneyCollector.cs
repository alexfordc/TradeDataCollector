using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace TradeDataCollector
{
    public class EastMoneyCollector : BaseCollector
    {
        private WebClient webClient;
        private Dictionary<string, string> dictGMToEastMoney = new Dictionary<string, string>();
        public EastMoneyCollector()
        {
            this.webClient = new WebClient();
        }
        public override Dictionary<string, Tick> Current(IEnumerable<string> symbols)
        {
            Dictionary<string, Tick> ret = new Dictionary<string, Tick>();
            string url = "http://nuff.eastmoney.com/EM_Finance2015TradeInterface/JS.ashx?id=";
            List<string> tickStrings = new List<string>();
            //只能一个个的请求，无法成批发送请求
            foreach (string symbol in symbols)
            {
                string eastMoneySymbol = this.getEastMoneySymbol(symbol);
                url += eastMoneySymbol ;
                Stream stream = this.webClient.OpenRead(url);
                StreamReader reader = new StreamReader(stream);
                string tickString;
                while ((tickString = reader.ReadLine()) != null) tickStrings.Add(tickString);
                stream.Close();
            }
            return ret;
        }

        public override List<Bar> HistoryBars(string symbol, int size, string startTime, string endTime = "")
        {
            throw new NotImplementedException();
        }

        public override List<Bar> HistoryBarsN(string symbol, int size, int n, string endTime = "")
        {
            throw new NotImplementedException();
        }

        public override List<Trade> HistoryTrades(string symbol, string startTime, string endTime = "")
        {
            throw new NotImplementedException();
        }

        public override List<Trade> HistoryTradesN(string symbol, int n, string endTime = "")
        {
            throw new NotImplementedException();
        }

        private string getEastMoneySymbol(string symbol)
        {
            string eastMoneySymbol;
            if (!this.dictGMToEastMoney.TryGetValue(symbol, out eastMoneySymbol))
            {
                eastMoneySymbol = Utils.GMToTencent(symbol);
                this.dictGMToEastMoney[symbol] = eastMoneySymbol;
            }
            return eastMoneySymbol;
        }
    }
}
