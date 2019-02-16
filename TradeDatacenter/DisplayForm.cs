using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HuaQuant.TradeDataAccess;
using HuaQuant.TradeDataCollector;
using System.Threading;

namespace HuaQuant.TradeDatacenter
{
    public partial class DisplayForm : Form
    {
        private List<Instrument> instruments;
        private Thread showDataThread = null;
        private Thread downloadThread = null;

        public DisplayForm()
        {
            InitializeComponent();
        }

        private void BtnShowQuotation_Click(object sender, EventArgs e)
        {
            if (showDataThread != null &&
                showDataThread.ThreadState != ThreadState.Stopped &&
                showDataThread.ThreadState != ThreadState.Aborted)
            {
                showDataThread.Abort();
            }
            curSymbol = this.CmbSymbol.Text;
            showDataThread = new Thread(new ThreadStart(ShowQuotation));
            showDataThread.Start();
        }

        private void ShowQuotation()
        {
            Dictionary<string, Tick> currentTicks = TradeDataAccessor.GetCurrentTicks();

            QuotationDataTable qdt = new QuotationDataTable();
            foreach (Instrument inst in this.instruments)
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
                    dr["DateTime"] = Utils.DateTimeToString(tick.DateTime);
                    dr["Source"] = tick.Source;
                }
                qdt.Rows.Add(dr);
            }
            this.RefreshData(this.dataGridView1, qdt);
            this.ChangeColumnStyle(this.dataGridView1.Columns[35]);
        }
        private void BtnShowMin1Bar_Click(object sender, EventArgs e)
        {
            if (showDataThread != null &&
                showDataThread.ThreadState != ThreadState.Stopped &&
                showDataThread.ThreadState != ThreadState.Aborted)
            {
                showDataThread.Abort();               
            }
            curSymbol = this.CmbSymbol.Text;
            showDataThread = new Thread(new ThreadStart(ShowMin1Bar));
            showDataThread.Start();
        }
        private string curSymbol="";
        private void ShowMin1Bar()
        {
            if (curSymbol == "") MessageBox.Show("请选择证券!");
            else
            {
                List<Bar> bars = TradeDataAccessor.GetMin1Bars(curSymbol);
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
                RefreshData(this.dataGridView1, bdt);
            }
        }
        private void BtnShowDay1Bar_Click(object sender, EventArgs e)
        {
            if (showDataThread != null &&
                showDataThread.ThreadState != ThreadState.Stopped &&
                showDataThread.ThreadState != ThreadState.Aborted)
            {
                showDataThread.Abort();
            }
            curSymbol = this.CmbSymbol.Text;
            showDataThread = new Thread(new ThreadStart(ShowDay1Bar));
            showDataThread.Start();
        }
        private void ShowDay1Bar()
        {
            if (curSymbol == "") MessageBox.Show("请选择证券!");
            else
            {
                List<Bar> bars = TradeDataAccessor.GetDay1Bars(curSymbol);
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
                RefreshData(this.dataGridView1, bdt);
            }
        }
        private void DisplayForm_Load(object sender, EventArgs e)
        {
            this.DtpBeginTime.Value = DateTime.Today.AddDays(-7);
            this.DtpEndTime.Value = DateTime.Today;
            Thread curThread = new Thread(new ThreadStart(LoadInstruments));
            curThread.Start();
        }
        private void LoadInstruments()
        {
            RefreshLabel(lblDisplay, "当前进度：正在读取证券信息......");
            instruments = TradeDataAccessor.GetInstruments();
            List<string> symbols = instruments.Select(i => i.Symbol).OrderBy(i => i).ToList();
            RefreshComboBox(this.CmbSymbol, symbols);
            RefreshLabel(lblDisplay, "当前进度：读取证券信息完毕。");
        }
        private void BtnClearRedis_Click(object sender, EventArgs e)
        {
            TradeDataAccessor.ClearRedis();
        }

        private void ChangeColumnStyle(DataGridViewColumn column)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new ChangeColumnStyleDelegate(this.ChangeColumnStyle), column);
            }
            else
            {
                column.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
                column.Width = 160;
            }
        }

        private void RefreshComboBox(ComboBox cmb,List<string> data)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new RefreshComboBoxDelegate(this.RefreshComboBox), cmb, data);
            }else
            {
                cmb.DataSource = data;
            }
        }
        
        private void RefreshLabel(Label lbl, string text)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new RefreshLabelDelegate(this.RefreshLabel), lbl, text);
            }
            else
            {
                lbl.Text = text;
            }
        }
        private void RefreshProgressBar(ProgressBar pgb, int value)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new RefreshProgressBarDelegate(this.RefreshProgressBar), pgb, value);
            }else
            {
                pgb.Value = value;
            }
        }
        private void RefreshData(DataGridView dgv, DataTable dt)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new RefreshDataDelegate(this.RefreshData), dgv, dt);
            }
            else
            {
                dgv.DataSource = dt;
            }
        }
        private delegate void ChangeColumnStyleDelegate(DataGridViewColumn column);
        private delegate void RefreshComboBoxDelegate(ComboBox cmb, List<string> data);
        private delegate void RefreshProgressBarDelegate(ProgressBar pgb, int value);
        private delegate void RefreshDataDelegate(DataGridView dgv, DataTable dt);
        private delegate void RefreshLabelDelegate(Label lbl, string text);
        
        private void BtnDownloadMin1Bar_Click(object sender, EventArgs e)
        {
            if (downloadThread!=null&&
                downloadThread.ThreadState!=ThreadState.Stopped&&
                downloadThread.ThreadState != ThreadState.Aborted)
            {
                DialogResult answer = MessageBox.Show("已有一个下载线程正在运行中，是否中止它开启新的下载？",
                    "询问", MessageBoxButtons.OKCancel);
                if (answer == DialogResult.OK)
                {
                    downloadThread.Abort();
                }
                else return;
            }
            this.PgbDisplay.Maximum = this.CmbSymbol.Items.Count;
            this.PgbDisplay.Value = 0;
            downloadThread = new Thread(new ThreadStart(DownLoadMin1Bar));
            downloadThread.Start();
        }
        private void DownLoadMin1Bar()
        {
            string text= "当前进度：开始下载1分线.....";
            RefreshLabel(this.lblDisplay, text);
            GMCollector gmc = new GMCollector();
            string beginTimeString = Utils.DateTimeToString(this.DtpBeginTime.Value);
            string endTimeString = Utils.DateTimeToString(this.DtpEndTime.Value);
            int i = 0;
            foreach (var item in this.CmbSymbol.Items)
            {
                string symbol = item.ToString();
                text = string.Format("当前进度：下载<{0}>的1分线",symbol);
                RefreshLabel(this.lblDisplay, text);
                List<Bar> bars = gmc.HistoryBars(symbol, 60,beginTimeString,endTimeString);
                if (bars.Count > 0)
                {
                    TradeDataAccessor.StoreMin1Bars(symbol, bars);
                }
                i++;
                RefreshProgressBar(this.PgbDisplay, i);
            }
            text = "当前进度：1分线下载完毕。";
            RefreshLabel(this.lblDisplay, text);
        }

        private void BtnStopDownload_Click(object sender, EventArgs e)
        {
            if (downloadThread != null &&
                downloadThread.ThreadState != ThreadState.Stopped &&
                downloadThread.ThreadState != ThreadState.Aborted)
            {
                downloadThread.Abort();
            }
        }

        private void BtnDownloadDailyBar_Click(object sender, EventArgs e)
        {
            if (downloadThread != null &&
                downloadThread.ThreadState != ThreadState.Stopped &&
                downloadThread.ThreadState != ThreadState.Aborted)
            {
                DialogResult answer = MessageBox.Show("已有一个下载线程正在运行中，是否中止它开启新的下载？",
                    "询问", MessageBoxButtons.OKCancel);
                if (answer == DialogResult.OK)
                {
                    downloadThread.Abort();
                }
                else return;
            }
            this.PgbDisplay.Maximum = this.CmbSymbol.Items.Count;
            this.PgbDisplay.Value = 0;
            downloadThread = new Thread(new ThreadStart(DownloadDailyBar));
            downloadThread.Start();
        }
        private void DownloadDailyBar()
        {
            string text = "当前进度：开始下载日线.....";
            RefreshLabel(this.lblDisplay, text);
            GMCollector gmc = new GMCollector();
            string beginTimeString = Utils.DateTimeToString(this.DtpBeginTime.Value);
            string endTimeString = Utils.DateTimeToString(this.DtpEndTime.Value);
            int i = 0;
            foreach (var item in this.CmbSymbol.Items)
            {
                string symbol = item.ToString();
                text = string.Format("当前进度：下载<{0}>的日线", symbol);
                RefreshLabel(this.lblDisplay, text);
                List<Bar> bars = gmc.HistoryBars(symbol, 86400, beginTimeString, endTimeString);
                if (bars.Count > 0)
                {
                    TradeDataAccessor.StoreDay1Bars(symbol, bars);
                }
                i++;
                RefreshProgressBar(this.PgbDisplay, i);
            }
            text = "当前进度：日线下载完毕。";
            RefreshLabel(this.lblDisplay, text);
        }

        

        private void DisplayForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (downloadThread != null &&
                downloadThread.ThreadState != ThreadState.Stopped &&
                downloadThread.ThreadState != ThreadState.Aborted)
            {
                downloadThread.Abort();
                downloadThread.Join();
            }
            if (showDataThread != null &&
                showDataThread.ThreadState != ThreadState.Stopped &&
                showDataThread.ThreadState != ThreadState.Aborted)
            {
                showDataThread.Abort();
                showDataThread.Join();
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
