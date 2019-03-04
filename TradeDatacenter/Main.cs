using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using HuaQuant.JobSchedule;

namespace HuaQuant.TradeDatacenter
{
    public partial class MainForm : Form
    {
        private Config config;
        private GlobalJob gJob;
        private JobSchedule.JobSchedule jobSche;
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Console.SetOut(new TextBoxWriter(this.txtLog));
            if (Program.ErrorMsg.Length > 0) Console.WriteLine(Program.ErrorMsg);
            try
            {
                config = Config.ReadFromFile("config.json");
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.myIcon.Visible = true;
                this.Hide();
                this.ShowInTaskbar = false;
            }
        }

        
     

        private void ShowMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            this.myIcon.Visible = false;
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            this.jobSche = new JobSchedule.JobSchedule();
            gJob = new GlobalJob(config,this.jobSche);
            DateTime curDay = DateTime.Today;
            JobTrigger trigger = new JobTrigger(curDay.Add(new TimeSpan(9, 20, 0)),null, 0, new TimeSpan(1, 0, 0, 0));
            trigger.IntervalBaseOnBeginTime = true;
            this.jobSche.Add(gJob, trigger);
            this.jobSche.Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (jobSche != null) jobSche.Stop();
        }

        private void BtnDisplay_Click(object sender, EventArgs e)
        {
            Form displayForm = new DisplayForm();
            displayForm.Show();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Console.WriteLine("中止所有作业执行,请等待......");
            if (jobSche!=null) jobSche.Stop();
            Console.Out.Close();
        }
    }
}
