namespace HuaQuant.TradeDatacenter
{
    partial class DisplayForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.BtnShowQuotation = new System.Windows.Forms.Button();
            this.BtnShowMin1Bar = new System.Windows.Forms.Button();
            this.BtnShowDay1Bar = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.CmbSymbol = new System.Windows.Forms.ComboBox();
            this.BtnClearRedis = new System.Windows.Forms.Button();
            this.DtpBeginTime = new System.Windows.Forms.DateTimePicker();
            this.DtpEndTime = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.BtnDownloadMin1Bar = new System.Windows.Forms.Button();
            this.BtnDownloadDailyBar = new System.Windows.Forms.Button();
            this.PgbDisplay = new System.Windows.Forms.ProgressBar();
            this.lblDisplay = new System.Windows.Forms.Label();
            this.BtnStopDownload = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(662, 356);
            this.dataGridView1.TabIndex = 0;
            // 
            // BtnShowQuotation
            // 
            this.BtnShowQuotation.Location = new System.Drawing.Point(695, 42);
            this.BtnShowQuotation.Name = "BtnShowQuotation";
            this.BtnShowQuotation.Size = new System.Drawing.Size(75, 23);
            this.BtnShowQuotation.TabIndex = 1;
            this.BtnShowQuotation.Text = "显示行情";
            this.BtnShowQuotation.UseVisualStyleBackColor = true;
            this.BtnShowQuotation.Click += new System.EventHandler(this.BtnShowQuotation_Click);
            // 
            // BtnShowMin1Bar
            // 
            this.BtnShowMin1Bar.Location = new System.Drawing.Point(695, 71);
            this.BtnShowMin1Bar.Name = "BtnShowMin1Bar";
            this.BtnShowMin1Bar.Size = new System.Drawing.Size(75, 23);
            this.BtnShowMin1Bar.TabIndex = 2;
            this.BtnShowMin1Bar.Text = "查看分线";
            this.BtnShowMin1Bar.UseVisualStyleBackColor = true;
            this.BtnShowMin1Bar.Click += new System.EventHandler(this.BtnShowMin1Bar_Click);
            // 
            // BtnShowDay1Bar
            // 
            this.BtnShowDay1Bar.Location = new System.Drawing.Point(695, 100);
            this.BtnShowDay1Bar.Name = "BtnShowDay1Bar";
            this.BtnShowDay1Bar.Size = new System.Drawing.Size(75, 23);
            this.BtnShowDay1Bar.TabIndex = 3;
            this.BtnShowDay1Bar.Text = "查看日线";
            this.BtnShowDay1Bar.UseVisualStyleBackColor = true;
            this.BtnShowDay1Bar.Click += new System.EventHandler(this.BtnShowDay1Bar_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(693, 136);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "请选择要查询的证券：";
            // 
            // CmbSymbol
            // 
            this.CmbSymbol.FormattingEnabled = true;
            this.CmbSymbol.Location = new System.Drawing.Point(695, 155);
            this.CmbSymbol.Name = "CmbSymbol";
            this.CmbSymbol.Size = new System.Drawing.Size(121, 20);
            this.CmbSymbol.TabIndex = 6;
            // 
            // BtnClearRedis
            // 
            this.BtnClearRedis.Location = new System.Drawing.Point(695, 188);
            this.BtnClearRedis.Name = "BtnClearRedis";
            this.BtnClearRedis.Size = new System.Drawing.Size(99, 23);
            this.BtnClearRedis.TabIndex = 7;
            this.BtnClearRedis.Text = "清空Redis数据";
            this.BtnClearRedis.UseVisualStyleBackColor = true;
            this.BtnClearRedis.Click += new System.EventHandler(this.BtnClearRedis_Click);
            // 
            // DtpBeginTime
            // 
            this.DtpBeginTime.Location = new System.Drawing.Point(87, 383);
            this.DtpBeginTime.Name = "DtpBeginTime";
            this.DtpBeginTime.Size = new System.Drawing.Size(138, 21);
            this.DtpBeginTime.TabIndex = 8;
            // 
            // DtpEndTime
            // 
            this.DtpEndTime.Location = new System.Drawing.Point(310, 383);
            this.DtpEndTime.Name = "DtpEndTime";
            this.DtpEndTime.Size = new System.Drawing.Size(138, 21);
            this.DtpEndTime.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 387);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "起始日期：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(235, 387);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 11;
            this.label3.Text = "终止日期：";
            // 
            // BtnDownloadMin1Bar
            // 
            this.BtnDownloadMin1Bar.Location = new System.Drawing.Point(695, 387);
            this.BtnDownloadMin1Bar.Name = "BtnDownloadMin1Bar";
            this.BtnDownloadMin1Bar.Size = new System.Drawing.Size(75, 23);
            this.BtnDownloadMin1Bar.TabIndex = 12;
            this.BtnDownloadMin1Bar.Text = "下载分线";
            this.BtnDownloadMin1Bar.UseVisualStyleBackColor = true;
            this.BtnDownloadMin1Bar.Click += new System.EventHandler(this.BtnDownloadMin1Bar_Click);
            // 
            // BtnDownloadDailyBar
            // 
            this.BtnDownloadDailyBar.Location = new System.Drawing.Point(695, 429);
            this.BtnDownloadDailyBar.Name = "BtnDownloadDailyBar";
            this.BtnDownloadDailyBar.Size = new System.Drawing.Size(75, 23);
            this.BtnDownloadDailyBar.TabIndex = 12;
            this.BtnDownloadDailyBar.Text = "下载日线";
            this.BtnDownloadDailyBar.UseVisualStyleBackColor = true;
            this.BtnDownloadDailyBar.Click += new System.EventHandler(this.BtnDownloadDailyBar_Click);
            // 
            // PgbDisplay
            // 
            this.PgbDisplay.Location = new System.Drawing.Point(12, 432);
            this.PgbDisplay.Name = "PgbDisplay";
            this.PgbDisplay.Size = new System.Drawing.Size(662, 27);
            this.PgbDisplay.TabIndex = 13;
            // 
            // lblDisplay
            // 
            this.lblDisplay.AutoSize = true;
            this.lblDisplay.Location = new System.Drawing.Point(12, 410);
            this.lblDisplay.Name = "lblDisplay";
            this.lblDisplay.Size = new System.Drawing.Size(65, 12);
            this.lblDisplay.TabIndex = 14;
            this.lblDisplay.Text = "当前进度：";
            // 
            // BtnStopDownload
            // 
            this.BtnStopDownload.Location = new System.Drawing.Point(788, 387);
            this.BtnStopDownload.Name = "BtnStopDownload";
            this.BtnStopDownload.Size = new System.Drawing.Size(75, 23);
            this.BtnStopDownload.TabIndex = 15;
            this.BtnStopDownload.Text = "停止下载";
            this.BtnStopDownload.UseVisualStyleBackColor = true;
            this.BtnStopDownload.Click += new System.EventHandler(this.BtnStopDownload_Click);
            // 
            // DisplayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(888, 476);
            this.Controls.Add(this.BtnStopDownload);
            this.Controls.Add(this.lblDisplay);
            this.Controls.Add(this.PgbDisplay);
            this.Controls.Add(this.BtnDownloadDailyBar);
            this.Controls.Add(this.BtnDownloadMin1Bar);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.DtpEndTime);
            this.Controls.Add(this.DtpBeginTime);
            this.Controls.Add(this.BtnClearRedis);
            this.Controls.Add(this.CmbSymbol);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BtnShowDay1Bar);
            this.Controls.Add(this.BtnShowMin1Bar);
            this.Controls.Add(this.BtnShowQuotation);
            this.Controls.Add(this.dataGridView1);
            this.Name = "DisplayForm";
            this.Text = "数据查看";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DisplayForm_FormClosing);
            this.Load += new System.EventHandler(this.DisplayForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button BtnShowQuotation;
        private System.Windows.Forms.Button BtnShowMin1Bar;
        private System.Windows.Forms.Button BtnShowDay1Bar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox CmbSymbol;
        private System.Windows.Forms.Button BtnClearRedis;
        private System.Windows.Forms.DateTimePicker DtpBeginTime;
        private System.Windows.Forms.DateTimePicker DtpEndTime;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button BtnDownloadMin1Bar;
        private System.Windows.Forms.Button BtnDownloadDailyBar;
        private System.Windows.Forms.ProgressBar PgbDisplay;
        private System.Windows.Forms.Label lblDisplay;
        private System.Windows.Forms.Button BtnStopDownload;
    }
}