namespace EasyBrailleEdit
{
	partial class ConversionProgressForm
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
            this.lblMsg = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lblInvalidChar = new System.Windows.Forms.Label();
            this.txtInvalidChar = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblMsg
            // 
            this.lblMsg.AutoSize = true;
            this.lblMsg.Location = new System.Drawing.Point(16, 22);
            this.lblMsg.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(121, 15);
            this.lblMsg.TabIndex = 0;
            this.lblMsg.Text = "正在進行轉換......";
            this.lblMsg.UseWaitCursor = true;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(19, 51);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4);
            this.progressBar1.Maximum = 1000;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(394, 20);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 1;
            this.progressBar1.UseWaitCursor = true;
            this.progressBar1.Value = 20;
            // 
            // lblInvalidChar
            // 
            this.lblInvalidChar.AutoSize = true;
            this.lblInvalidChar.Location = new System.Drawing.Point(16, 100);
            this.lblInvalidChar.Name = "lblInvalidChar";
            this.lblInvalidChar.Size = new System.Drawing.Size(127, 15);
            this.lblInvalidChar.TabIndex = 2;
            this.lblInvalidChar.Text = "無法轉換的字元：";
            this.lblInvalidChar.UseWaitCursor = true;
            // 
            // txtInvalidChar
            // 
            this.txtInvalidChar.Location = new System.Drawing.Point(19, 129);
            this.txtInvalidChar.Multiline = true;
            this.txtInvalidChar.Name = "txtInvalidChar";
            this.txtInvalidChar.ReadOnly = true;
            this.txtInvalidChar.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtInvalidChar.Size = new System.Drawing.Size(394, 122);
            this.txtInvalidChar.TabIndex = 3;
            this.txtInvalidChar.UseWaitCursor = true;
            // 
            // ConvertionProgressForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(431, 272);
            this.ControlBox = false;
            this.Controls.Add(this.txtInvalidChar);
            this.Controls.Add(this.lblInvalidChar);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.lblMsg);
            this.Font = new System.Drawing.Font("PMingLiU", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConvertionProgressForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "訊息";
            this.TopMost = true;
            this.UseWaitCursor = true;
            this.Load += new System.EventHandler(this.ConvertionProgressForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblMsg;
		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.Label lblInvalidChar;
		private System.Windows.Forms.TextBox txtInvalidChar;
	}
}