using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TradeDataAccess
{
    public partial class MainForm : Form
    {
        private Config config;
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

        
     

        private void showMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            this.myIcon.Visible = false;
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {

        }
    }
}
