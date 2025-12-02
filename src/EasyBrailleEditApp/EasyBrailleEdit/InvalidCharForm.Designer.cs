namespace EasyBrailleEdit
{
	partial class InvalidCharForm
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
            lbxInvalidChars = new System.Windows.Forms.ListBox();
            SuspendLayout();
            // 
            // lbxInvalidChars
            // 
            lbxInvalidChars.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            lbxInvalidChars.Dock = System.Windows.Forms.DockStyle.Fill;
            lbxInvalidChars.Font = new System.Drawing.Font("PMingLiU", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lbxInvalidChars.FormattingEnabled = true;
            lbxInvalidChars.ItemHeight = 15;
            lbxInvalidChars.Location = new System.Drawing.Point(0, 0);
            lbxInvalidChars.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            lbxInvalidChars.Name = "lbxInvalidChars";
            lbxInvalidChars.Size = new System.Drawing.Size(151, 557);
            lbxInvalidChars.TabIndex = 0;
            lbxInvalidChars.SelectedIndexChanged += lbxInvalidChars_SelectedIndexChanged;
            // 
            // InvalidCharForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(151, 557);
            Controls.Add(lbxInvalidChars);
            Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            Name = "InvalidCharForm";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "轉換失敗的字元";
            TopMost = true;
            FormClosing += InvalidCharForm_FormClosing;
            Load += InvalidCharForm_Load;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ListBox lbxInvalidChars;
	}
}