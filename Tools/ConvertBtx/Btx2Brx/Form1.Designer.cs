namespace Btx2Brx
{
    partial class Form1
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
            this.txtBtxPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnConvert = new System.Windows.Forms.Button();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtBtxPath
            // 
            this.txtBtxPath.Location = new System.Drawing.Point(38, 69);
            this.txtBtxPath.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtBtxPath.Name = "txtBtxPath";
            this.txtBtxPath.Size = new System.Drawing.Size(601, 29);
            this.txtBtxPath.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(225, 19);
            this.label1.TabIndex = 1;
            this.label1.Text = ".BTX 檔案所在的資料夾：";
            // 
            // btnConvert
            // 
            this.btnConvert.Location = new System.Drawing.Point(38, 129);
            this.btnConvert.Name = "btnConvert";
            this.btnConvert.Size = new System.Drawing.Size(683, 54);
            this.btnConvert.TabIndex = 2;
            this.btnConvert.Text = "把上述資料夾裡面的 .btx 檔案全部轉換成新的 .brx 格式";
            this.btnConvert.UseVisualStyleBackColor = true;
            this.btnConvert.Click += new System.EventHandler(this.btnConvert_Click);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(645, 67);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(76, 29);
            this.btnBrowse.TabIndex = 3;
            this.btnBrowse.Text = "瀏覽";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(34, 218);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 19);
            this.label2.TabIndex = 4;
            this.label2.Text = "轉換結果：";
            // 
            // txtLog
            // 
            this.txtLog.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtLog.Location = new System.Drawing.Point(38, 256);
            this.txtLog.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(683, 258);
            this.txtLog.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(752, 547);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.btnConvert);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtBtxPath);
            this.Font = new System.Drawing.Font("PMingLiU", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "把易點雙視 1.x 版的 .btx  檔案轉換成 3.x 版的  .brx 格式";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtBtxPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnConvert;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLog;
    }
}

