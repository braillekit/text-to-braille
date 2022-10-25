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
			this.lbxInvalidChars = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// lbxInvalidChars
			// 
			this.lbxInvalidChars.BackColor = System.Drawing.Color.LightGoldenrodYellow;
			this.lbxInvalidChars.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbxInvalidChars.Font = new System.Drawing.Font("PMingLiU", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.lbxInvalidChars.FormattingEnabled = true;
			this.lbxInvalidChars.ItemHeight = 15;
			this.lbxInvalidChars.Location = new System.Drawing.Point(0, 0);
			this.lbxInvalidChars.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.lbxInvalidChars.Name = "lbxInvalidChars";
			this.lbxInvalidChars.Size = new System.Drawing.Size(134, 439);
			this.lbxInvalidChars.TabIndex = 0;
			this.lbxInvalidChars.SelectedIndexChanged += new System.EventHandler(this.lbxInvalidChars_SelectedIndexChanged);
			// 
			// InvalidCharForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(134, 440);
			this.Controls.Add(this.lbxInvalidChars);
			this.Font = new System.Drawing.Font("PMingLiU", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.Name = "InvalidCharForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "轉換失敗的字元";
			this.TopMost = true;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InvalidCharForm_FormClosing);
			this.Load += new System.EventHandler(this.InvalidCharForm_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListBox lbxInvalidChars;
	}
}