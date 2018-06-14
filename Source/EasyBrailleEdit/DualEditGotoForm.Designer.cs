namespace EasyBrailleEdit
{
	partial class DualEditGotoForm
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
            this.rdoLine = new System.Windows.Forms.RadioButton();
            this.rdoPage = new System.Windows.Forms.RadioButton();
            this.numPosition = new System.Windows.Forms.NumericUpDown();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numPosition)).BeginInit();
            this.SuspendLayout();
            // 
            // rdoLine
            // 
            this.rdoLine.AutoSize = true;
            this.rdoLine.Checked = true;
            this.rdoLine.Location = new System.Drawing.Point(141, 30);
            this.rdoLine.Name = "rdoLine";
            this.rdoLine.Size = new System.Drawing.Size(77, 23);
            this.rdoLine.TabIndex = 1;
            this.rdoLine.TabStop = true;
            this.rdoLine.Text = "列 (&L)";
            this.rdoLine.UseVisualStyleBackColor = true;
            // 
            // rdoPage
            // 
            this.rdoPage.AutoSize = true;
            this.rdoPage.Location = new System.Drawing.Point(234, 30);
            this.rdoPage.Name = "rdoPage";
            this.rdoPage.Size = new System.Drawing.Size(76, 23);
            this.rdoPage.TabIndex = 2;
            this.rdoPage.Text = "頁 (&P)";
            this.rdoPage.UseVisualStyleBackColor = true;
            // 
            // numPosition
            // 
            this.numPosition.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.numPosition.Location = new System.Drawing.Point(46, 26);
            this.numPosition.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numPosition.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numPosition.Name = "numPosition";
            this.numPosition.Size = new System.Drawing.Size(79, 30);
            this.numPosition.TabIndex = 0;
            this.numPosition.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // btnOk
            // 
            this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(87, 91);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(81, 43);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "確定";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(174, 91);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(84, 43);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 19);
            this.label1.TabIndex = 5;
            this.label1.Text = "第";
            // 
            // DualEditGotoForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(341, 146);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.numPosition);
            this.Controls.Add(this.rdoPage);
            this.Controls.Add(this.rdoLine);
            this.Font = new System.Drawing.Font("PMingLiU", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DualEditGotoForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "移動至";
            this.Shown += new System.EventHandler(this.DualEditGotoForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.numPosition)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RadioButton rdoLine;
		private System.Windows.Forms.RadioButton rdoPage;
		private System.Windows.Forms.NumericUpDown numPosition;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
    }
}