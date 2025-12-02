namespace EasyBrailleEdit
{
	partial class UpdateProgressForm
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
            progressBar1 = new System.Windows.Forms.ProgressBar();
            txtMsg = new System.Windows.Forms.TextBox();
            SuspendLayout();
            // 
            // progressBar1
            // 
            progressBar1.Location = new System.Drawing.Point(15, 309);
            progressBar1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new System.Drawing.Size(418, 28);
            progressBar1.TabIndex = 0;
            // 
            // txtMsg
            // 
            txtMsg.Location = new System.Drawing.Point(15, 16);
            txtMsg.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            txtMsg.Multiline = true;
            txtMsg.Name = "txtMsg";
            txtMsg.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            txtMsg.Size = new System.Drawing.Size(562, 264);
            txtMsg.TabIndex = 1;
            // 
            // UpdateProgressForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(600, 366);
            ControlBox = false;
            Controls.Add(txtMsg);
            Controls.Add(progressBar1);
            Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            Name = "UpdateProgressForm";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "自動更新";
            TopMost = true;
            Load += UpdateProgressForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.TextBox txtMsg;
	}
}