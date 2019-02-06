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
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(650, 371);
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
            this.BtnShowMin1Bar.Location = new System.Drawing.Point(695, 90);
            this.BtnShowMin1Bar.Name = "BtnShowMin1Bar";
            this.BtnShowMin1Bar.Size = new System.Drawing.Size(75, 23);
            this.BtnShowMin1Bar.TabIndex = 2;
            this.BtnShowMin1Bar.Text = "查看分线";
            this.BtnShowMin1Bar.UseVisualStyleBackColor = true;
            this.BtnShowMin1Bar.Click += new System.EventHandler(this.BtnShowMin1Bar_Click);
            // 
            // BtnShowDay1Bar
            // 
            this.BtnShowDay1Bar.Location = new System.Drawing.Point(695, 139);
            this.BtnShowDay1Bar.Name = "BtnShowDay1Bar";
            this.BtnShowDay1Bar.Size = new System.Drawing.Size(75, 23);
            this.BtnShowDay1Bar.TabIndex = 3;
            this.BtnShowDay1Bar.Text = "查看日线";
            this.BtnShowDay1Bar.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(695, 214);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "请选择要查询的证券：";
            // 
            // CmbSymbol
            // 
            this.CmbSymbol.FormattingEnabled = true;
            this.CmbSymbol.Location = new System.Drawing.Point(695, 240);
            this.CmbSymbol.Name = "CmbSymbol";
            this.CmbSymbol.Size = new System.Drawing.Size(121, 20);
            this.CmbSymbol.TabIndex = 6;
            // 
            // BtnClearRedis
            // 
            this.BtnClearRedis.Location = new System.Drawing.Point(697, 317);
            this.BtnClearRedis.Name = "BtnClearRedis";
            this.BtnClearRedis.Size = new System.Drawing.Size(119, 23);
            this.BtnClearRedis.TabIndex = 7;
            this.BtnClearRedis.Text = "清空Redis数据";
            this.BtnClearRedis.UseVisualStyleBackColor = true;
            this.BtnClearRedis.Click += new System.EventHandler(this.BtnClearRedis_Click);
            // 
            // DisplayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(828, 450);
            this.Controls.Add(this.BtnClearRedis);
            this.Controls.Add(this.CmbSymbol);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BtnShowDay1Bar);
            this.Controls.Add(this.BtnShowMin1Bar);
            this.Controls.Add(this.BtnShowQuotation);
            this.Controls.Add(this.dataGridView1);
            this.Name = "DisplayForm";
            this.Text = "数据查看";
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
    }
}