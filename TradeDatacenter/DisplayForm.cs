using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TradeDataAccess;
using TradeDataCollector;

namespace TradeDatacenter
{
    public partial class DisplayForm : Form
    {
        public DisplayForm()
        {
            InitializeComponent();
        }

        private void BtnShowQuotation_Click(object sender, EventArgs e)
        {
            List<Instrument> instruments = TradeDataAccessor.GetInstruments();
            Dictionary<string, Tick> currentTicks = TradeDataAccessor.GetCurrentTicks();

            QuotationDataTable qdt = new QuotationDataTable();
            foreach(Instrument inst in instruments)
            {
                DataRow dr = qdt.NewRow();
                dr["Symbol"] = inst.Symbol;
                dr["LastClose"] = inst.LastClose;
                dr["UpperLimit"] = inst.UpperLimit;
                dr["LowerLimit"] = inst.LowerLimit;
                dr["AdjFactor"] = inst.AdjFactor;
                dr["CreatedAt"] = inst.CreatedAt;
                if (currentTicks.TryGetValue(inst.Symbol, out Tick tick))
                {
                    dr["Open"] = tick.Open;
                    dr["High"] = tick.High;
                    dr["Low"] = tick.Low;
                    dr["Price"] = tick.Price;
                    dr["Volume"] = tick.Volume;
                    dr["Amount"] = tick.Amount;
                    dr["CumVolume"] = tick.CumVolume;
                    dr["CumAmount"] = tick.CumAmount;
                    dr["BuyOrSell"] = tick.BuyOrSell;
                    for (int i = 0; i < tick.Quotes.Length; i++)
                    {
                        dr["BidPrice" + i.ToString()] = tick.Quotes[i].BidPrice;
                        dr["BidVolume" + i.ToString()] = tick.Quotes[i].BidVolume;
                        dr["AskPrice" + i.ToString()] = tick.Quotes[i].AskPrice;
                        dr["AskVolume" + i.ToString()] = tick.Quotes[i].AskVolume;
                    }
                    dr["DateTime"] =Utils.DateTimeToString(tick.DateTime);
                    dr["Source"] = tick.Source;
                }
                qdt.Rows.Add(dr);
            }
            this.dataGridView1.DataSource = qdt;
            this.dataGridView1.Columns[35].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
            this.dataGridView1.Columns[35].Width = 160;
        }

        private void BtnShowMin1Bar_Click(object sender, EventArgs e)
        {
            string symbol = this.TxtSymbol.Text;
            if (symbol == "") MessageBox.Show("please input the symbol!");
            else
            {
                List<Bar> bars = TradeDataAccessor.GetMin1Bars(symbol);
                BarDataTable bdt = new BarDataTable();
                foreach (Bar bar in bars)
                {
                    DataRow dr = bdt.NewRow();
                    dr["BeginTime"] = bar.BeginTime;
                    dr["EndTime"] = bar.EndTime;
                    dr["LastClose"] = bar.LastClose;
                    dr["Open"] = bar.Open;
                    dr["High"] = bar.High;
                    dr["Low"] = bar.Low;
                    dr["Close"] = bar.Close;
                    dr["Volume"] = bar.Volume;
                    dr["Amount"] = bar.Amount;
                    dr["Size"] = bar.Size;
                    bdt.Rows.Add(dr);
                }
                this.dataGridView1.DataSource = bdt;
            }
        }
    }
    public class QuotationDataTable : DataTable
    {
        public QuotationDataTable()
        {
            this.Columns.Add(new DataColumn("Symbol", typeof(string)));
            this.Columns.Add(new DataColumn("LastClose", typeof(float)));
            this.Columns.Add(new DataColumn("UpperLimit", typeof(float)));
            this.Columns.Add(new DataColumn("LowerLimit", typeof(float)));
            this.Columns.Add(new DataColumn("AdjFactor", typeof(float)));
            this.Columns.Add(new DataColumn("CreatedAt", typeof(DateTime)));
            this.Columns.Add(new DataColumn("Open", typeof(float)));
            this.Columns.Add(new DataColumn("High", typeof(float)));
            this.Columns.Add(new DataColumn("Low", typeof(float)));
            this.Columns.Add(new DataColumn("Price", typeof(float)));
            this.Columns.Add(new DataColumn("Volume", typeof(int)));
            this.Columns.Add(new DataColumn("Amount", typeof(double)));
            this.Columns.Add(new DataColumn("CumVolume", typeof(double)));
            this.Columns.Add(new DataColumn("CumAmount", typeof(double)));

            this.Columns.Add(new DataColumn("BidPrice0", typeof(float)));
            this.Columns.Add(new DataColumn("BidVolume0", typeof(int)));
            this.Columns.Add(new DataColumn("BuyOrSell", typeof(char)));
            this.Columns.Add(new DataColumn("BidPrice1", typeof(float)));
            this.Columns.Add(new DataColumn("BidVolume1", typeof(int)));
            this.Columns.Add(new DataColumn("BidPrice2", typeof(float)));
            this.Columns.Add(new DataColumn("BidVolume2", typeof(int)));
            this.Columns.Add(new DataColumn("BidPrice3", typeof(float)));
            this.Columns.Add(new DataColumn("BidVolume3", typeof(int)));
            this.Columns.Add(new DataColumn("BidPrice4", typeof(float)));
            this.Columns.Add(new DataColumn("BidVolume4", typeof(int)));

            this.Columns.Add(new DataColumn("AskPrice0", typeof(float)));
            this.Columns.Add(new DataColumn("AskVolume0", typeof(int)));
            this.Columns.Add(new DataColumn("AskPrice1", typeof(float)));
            this.Columns.Add(new DataColumn("AskVolume1", typeof(int)));
            this.Columns.Add(new DataColumn("AskPrice2", typeof(float)));
            this.Columns.Add(new DataColumn("AskVolume2", typeof(int)));
            this.Columns.Add(new DataColumn("AskPrice3", typeof(float)));
            this.Columns.Add(new DataColumn("AskVolume3", typeof(int)));
            this.Columns.Add(new DataColumn("AskPrice4", typeof(float)));
            this.Columns.Add(new DataColumn("AskVolume4", typeof(int)));
            
            this.Columns.Add(new DataColumn("DateTime", typeof(DateTime)));
            this.Columns.Add(new DataColumn("Source",typeof(string)));
        }
    }
    public class BarDataTable : DataTable
    {
        public BarDataTable()
        {
            this.Columns.Add(new DataColumn("BeginTime", typeof(DateTime)));
            this.Columns.Add(new DataColumn("EndTime", typeof(DateTime)));
            this.Columns.Add(new DataColumn("LastClose", typeof(float)));
            this.Columns.Add(new DataColumn("Open", typeof(float)));
            this.Columns.Add(new DataColumn("High", typeof(float)));
            this.Columns.Add(new DataColumn("Low", typeof(float)));
            this.Columns.Add(new DataColumn("Close", typeof(float)));
            this.Columns.Add(new DataColumn("Volume", typeof(double)));
            this.Columns.Add(new DataColumn("Amount", typeof(double)));
            this.Columns.Add(new DataColumn("Size", typeof(int)));
        }
    }
}
