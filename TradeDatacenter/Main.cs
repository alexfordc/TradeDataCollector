using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TradeDatacenter
{
    public partial class MainForm : Form
    {
        private Config config;
        private GlobalJob gJob;
        private JobRunner jRun;
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Console.SetOut(new TextBoxWriter(this.txtLog));
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
            gJob = new GlobalJob(config);
            DateTime curDay = DateTime.Today;
            jRun = new JobRunner(1000, 1, curDay.Add(new TimeSpan(9, 15, 0)), new TimeSpan(1, 0, 0, 0));
            jRun.AddJob(gJob);
            jRun.Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            jRun.Stop();
            gJob.StopAllJobs();
        }
    }
}
