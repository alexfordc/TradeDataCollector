﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMSDK;
namespace TradeDataCollector
{
    public class GMCollector:BaseCollector
    {
        public GMCollector()
        {
            GMApi.SetToken("48da7cef9d32b7f33bf043f493c3feec4a26958b");
        }
        public override Dictionary<string, Tick> Current(IEnumerable<string> symbols)
        {
            Dictionary<string, Tick> ret = new Dictionary<string, Tick>();
            string symbolString = "";
            foreach (string symbol in symbols) symbolString += symbol + ",";
            GMDataList<GMSDK.Tick> dataList = GMApi.Current(symbolString);
            if (dataList.status != 0)
            {
                throw new Exception(this.GetErrorMsg(dataList.status));
            }
            foreach(GMSDK.Tick gmTick in dataList.data)
            {
                Tick aTick = new Tick
                {
                    DateTime = gmTick.createdAt,
                    Open = gmTick.open,
                    High = gmTick.high,
                    Low = gmTick.low,
                    Price = gmTick.price,
                    Volume = gmTick.lastVolume,
                    Amount = gmTick.lastAmount,
                    CumVolume = gmTick.cumVolume,
                    CumAmount = gmTick.cumAmount
                };
                int i = 0;
                foreach(GMSDK.Quote gmQuote in gmTick.quotes)
                {
                    if (gmQuote == null) continue;
                    Quote aQuote = new Quote
                    {
                        BidPrice = gmQuote.bidPrice,
                        BidVolume = gmQuote.bidVolume,
                        AskPrice = gmQuote.askPrice,
                        AskVolume = gmQuote.askVolume
                    };
                    aTick.Quotes[i] = aQuote;
                    i++;
                }
 
                switch (gmTick.tradeType)
                {
                    case 7:
                        aTick.BuyOrSell = 'B';
                        break;
                    case 8:
                        aTick.BuyOrSell = 'S';
                        break;
                    default:
                        aTick.BuyOrSell = 'N';
                        break;
                }
                ret.Add(gmTick.symbol, aTick);
            }
            return ret;
        }

        public override List<Bar> HistoryBars(string symbol, int size, string startTime, string endTime="")
        {
            throw new NotImplementedException();
        }

        public override List<Bar> HistoryBarsN(string symbol, int size, int n, string endTime="")
        {
            throw new NotImplementedException();
        }

        public override List<Trade> HistoryTrades(string symbol, string startTime, string endTime="")
        {
            List<Trade> ret=new List<Trade>();
            if (endTime=="") endTime=DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            GMDataList<GMSDK.Tick> dataList=GMApi.HistoryTicks(symbol,startTime,endTime);
            if (dataList.status != 0)
            {
                throw new Exception(this.GetErrorMsg(dataList.status));
            }
            foreach(GMSDK.Tick gmTick in dataList.data)
            {
                if (gmTick.lastVolume<=0) continue;
                Trade aTrade = new Trade
                {
                    DateTime = gmTick.createdAt,
                    Price = gmTick.price,
                    Volume = gmTick.lastVolume,
                    Amount = gmTick.lastAmount,
                };
                
                switch (gmTick.tradeType)
                {
                    case 7:
                        aTrade.BuyOrSell = 'B';
                        break;
                    case 8:
                        aTrade.BuyOrSell = 'S';
                        break;
                    default:
                        aTrade.BuyOrSell = 'N';
                        break;
                }
                ret.Add(aTrade);
            }
            return ret;    
        }

        public override List<Trade> HistoryTradesN(string symbol, int n, string endTime="")
        {
            List<Trade> ret=new List<Trade>();
            if (endTime == "") endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            GMDataList<GMSDK.Tick> dataList=GMApi.HistoryTicksN(symbol,n,endTime);
            if (dataList.status != 0)
            {
                throw new Exception(this.GetErrorMsg(dataList.status));
            }
            foreach(GMSDK.Tick gmTick in dataList.data)
            {
                Trade aTrade = new Trade
                {
                    DateTime = gmTick.createdAt,
                    Price = gmTick.price,
                    Volume = gmTick.lastVolume,
                    Amount = gmTick.lastAmount,
                };
                
                switch (gmTick.tradeType)
                {
                    case 7:
                        aTrade.BuyOrSell = 'B';
                        break;
                    case 8:
                        aTrade.BuyOrSell = 'S';
                        break;
                    default:
                        aTrade.BuyOrSell = 'N';
                        break;
                }
                ret.Add(aTrade);
            }
            return ret;    
        }

        public override List<Trade> LastDayTrades(string symbol)
        {
            throw new NotImplementedException();
        }

        private string GetErrorMsg(int errorCode)
        {
            switch (errorCode)
            {
                case 1000:
                    return "错误或无效的token";
                case 1001:
                    return "无法连接到终端服务";
                case 1010:
                    return "无法获取掘金服务器地址列表";
                case 1011:
                    return "消息包解析错误";
                case 1012:
                    return "网络消息包解析错误";
                case 1014:
                    return "历史行情服务调用错误";
                case 1016:
                    return "动态参数调用错误";
                case 1017:
                    return "基本面数据服务调用错误";
                case 1020:
                    return "无效的ACCOUNT_ID";
                case 1021:
                    return "非法日期格式";
                default:
                    return "未知错误";
            }
        }
    }
}
