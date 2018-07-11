namespace BrlReformat
{
    partial class MainForm
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
            this.txtSrc = new System.Windows.Forms.TextBox();
            this.txtFormatted = new System.Windows.Forms.TextBox();
            this.btnFormat = new System.Windows.Forms.Button();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtSrc
            // 
            this.txtSrc.Location = new System.Drawing.Point(33, 77);
            this.txtSrc.Multiline = true;
            this.txtSrc.Name = "txtSrc";
            this.txtSrc.Size = new System.Drawing.Size(572, 190);
            this.txtSrc.TabIndex = 0;
            // 
            // txtFormatted
            // 
            this.txtFormatted.Location = new System.Drawing.Point(33, 291);
            this.txtFormatted.Multiline = true;
            this.txtFormatted.Name = "txtFormatted";
            this.txtFormatted.Size = new System.Drawing.Size(572, 246);
            this.txtFormatted.TabIndex = 1;
            // 
            // btnFormat
            // 
            this.btnFormat.Location = new System.Drawing.Point(611, 112);
            this.btnFormat.Name = "btnFormat";
            this.btnFormat.Size = new System.Drawing.Size(120, 37);
            this.btnFormat.TabIndex = 2;
            this.btnFormat.Text = "格式化";
            this.btnFormat.UseVisualStyleBackColor = true;
            this.btnFormat.Click += new System.EventHandler(this.btnFormat_Click);
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Location = new System.Drawing.Point(611, 46);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(120, 37);
            this.btnOpenFile.TabIndex = 3;
            this.btnOpenFile.Text = "開啟文字檔";
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "原始文件：";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(611, 187);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(120, 37);
            this.btnClear.TabIndex = 5;
            this.btnClear.Text = "清除";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(611, 291);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(120, 37);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "另存新檔";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(805, 569);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnOpenFile);
            this.Controls.Add(this.btnFormat);
            this.Controls.Add(this.txtFormatted);
            this.Controls.Add(this.txtSrc);
            this.Font = new System.Drawing.Font("MingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Text = "重新調整文字斷行以符合點字";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.TextBox txtSrc;
		private System.Windows.Forms.TextBox txtFormatted;
		private System.Windows.Forms.Button btnFormat;
		private System.Windows.Forms.Button btnOpenFile;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnSave;
    }
}

