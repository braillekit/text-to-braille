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
            rdoLine = new System.Windows.Forms.RadioButton();
            rdoPage = new System.Windows.Forms.RadioButton();
            numPosition = new System.Windows.Forms.NumericUpDown();
            btnOk = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)numPosition).BeginInit();
            SuspendLayout();
            // 
            // rdoLine
            // 
            rdoLine.AutoSize = true;
            rdoLine.Checked = true;
            rdoLine.Location = new System.Drawing.Point(147, 33);
            rdoLine.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            rdoLine.Name = "rdoLine";
            rdoLine.Size = new System.Drawing.Size(64, 23);
            rdoLine.TabIndex = 1;
            rdoLine.TabStop = true;
            rdoLine.Text = "列 (&L)";
            rdoLine.UseVisualStyleBackColor = true;
            // 
            // rdoPage
            // 
            rdoPage.AutoSize = true;
            rdoPage.Location = new System.Drawing.Point(227, 33);
            rdoPage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            rdoPage.Name = "rdoPage";
            rdoPage.Size = new System.Drawing.Size(65, 23);
            rdoPage.TabIndex = 2;
            rdoPage.Text = "頁 (&P)";
            rdoPage.UseVisualStyleBackColor = true;
            // 
            // numPosition
            // 
            numPosition.ImeMode = System.Windows.Forms.ImeMode.Off;
            numPosition.Location = new System.Drawing.Point(52, 33);
            numPosition.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            numPosition.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numPosition.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numPosition.Name = "numPosition";
            numPosition.Size = new System.Drawing.Size(58, 27);
            numPosition.TabIndex = 0;
            numPosition.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // btnOk
            // 
            btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            btnOk.Location = new System.Drawing.Point(75, 108);
            btnOk.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnOk.Name = "btnOk";
            btnOk.Size = new System.Drawing.Size(91, 37);
            btnOk.TabIndex = 3;
            btnOk.Text = "確定";
            btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(172, 108);
            btnCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(94, 37);
            btnCancel.TabIndex = 4;
            btnCancel.Text = "取消";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(22, 37);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(24, 19);
            label1.TabIndex = 5;
            label1.Text = "第";
            // 
            // DualEditGotoForm
            // 
            AcceptButton = btnOk;
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new System.Drawing.Size(339, 158);
            Controls.Add(label1);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(numPosition);
            Controls.Add(rdoPage);
            Controls.Add(rdoLine);
            Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DualEditGotoForm";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "移動至";
            Shown += DualEditGotoForm_Shown;
            ((System.ComponentModel.ISupportInitialize)numPosition).EndInit();
            ResumeLayout(false);
            PerformLayout();
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