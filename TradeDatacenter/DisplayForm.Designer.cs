namespace TradeDatacenter
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
            this.TxtSymbol = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
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
            // TxtSymbol
            // 
            this.TxtSymbol.Location = new System.Drawing.Point(695, 232);
            this.TxtSymbol.Name = "TxtSymbol";
            this.TxtSymbol.Size = new System.Drawing.Size(125, 21);
            this.TxtSymbol.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(695, 214);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "请输入要查询的证券：";
            // 
            // DisplayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(828, 450);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TxtSymbol);
            this.Controls.Add(this.BtnShowDay1Bar);
            this.Controls.Add(this.BtnShowMin1Bar);
            this.Controls.Add(this.BtnShowQuotation);
            this.Controls.Add(this.dataGridView1);
            this.Name = "DisplayForm";
            this.Text = "数据查看";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button BtnShowQuotation;
        private System.Windows.Forms.Button BtnShowMin1Bar;
        private System.Windows.Forms.Button BtnShowDay1Bar;
        private System.Windows.Forms.TextBox TxtSymbol;
        private System.Windows.Forms.Label label1;
    }
}