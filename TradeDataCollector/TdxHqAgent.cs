using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeDataCollector
{
    public class TdxHqAgent
    {
        private string ip;
        private short port;
        private static TdxHqAgent instance = null;
        private TdxHqAgent() { }
        public static TdxHqAgent Instance
        {
            get
            {
                if (TdxHqAgent.instance == null)
                {
                    TdxHqAgent.instance = new TdxHqAgent();
                }
                return TdxHqAgent.instance;
            }
        }
        public ReportArgs Connect(string ip, short port)
        {
            this.ip = ip;
            this.port = port;
            ReportArgs reportArgs = new ReportArgs();
            StringBuilder errInfo = new StringBuilder(256);
            StringBuilder result = new StringBuilder(1024 * 1024);
            if (TdxHqWrapper.TdxHq_Connect(ip, port, result, errInfo))
            {
                reportArgs.Succeeded = true;
                reportArgs.Result = result.ToString();
            }
            else
            {
                reportArgs.Succeeded = false;
                reportArgs.ErrorInfo = errInfo.ToString();
            }
            return reportArgs;
        }
        public void Disconnect()
        {
            TdxHqWrapper.TdxHq_Disconnect();
        }
        public ReportArgs GetQuotes(byte[] marketIDs,string[] securityIDs)
        {
            short count = (short)securityIDs.Length;
            ReportArgs reportArgs = new ReportArgs();
            StringBuilder errInfo = new StringBuilder(256);
            StringBuilder result = new StringBuilder(1024 * 1024);
            if (TdxHqWrapper.TdxHq_GetSecurityQuotes(marketIDs, securityIDs, ref count, result, errInfo))
            {
                reportArgs.Succeeded = true;
                List<string[]> data = this.pickUp(result);
                reportArgs.Result = data;
            }
            else
            {
                reportArgs.Succeeded = false;
                reportArgs.ErrorInfo = errInfo.ToString();
            }
            return reportArgs;
        }
        
        public ReportArgs GetLastTrades(byte marketID, string securityID, short start, short count)
        {
            ReportArgs reportArgs = new ReportArgs();
            StringBuilder errInfo = new StringBuilder(256);
            StringBuilder result = new StringBuilder(1024 * 1024);
            if (TdxHqWrapper.TdxHq_GetTransactionData( marketID, securityID, start, ref count, result, errInfo))
            {
                List<string[]> data = this.pickUp(result);
                reportArgs.Succeeded = true;
                reportArgs.Result = data;
            }
            else
            {
                reportArgs.Succeeded = false;
                reportArgs.ErrorInfo = errInfo.ToString();
            }
            return reportArgs;
        }

        private List<string[]> pickUp(StringBuilder result)
        {
            List<string[]> records = new List<string[]>();
            string text = result.ToString();
            string[] lines = text.Split('\n');
            foreach (string aline in lines)
            {
                string[] fields = aline.Split(new char[] { ' ', '\t' });
                records.Add(fields);
            }
            return records;
        }
    }
    public class ReportArgs
    {
        private bool succeeded = false;
        public bool Succeeded
        {
            get { return this.succeeded; }
            set { this.succeeded = value; }
        }
        private string errorInfo = "";
        public string ErrorInfo
        {
            get { return this.errorInfo; }
            set { this.errorInfo = value; }
        }
        private object result = null;
        public object Result
        {
            get { return this.result; }
            set { this.result = value; }
        }
        public ReportArgs() { }
    }
}

