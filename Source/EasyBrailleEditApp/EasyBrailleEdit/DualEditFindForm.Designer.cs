namespace EasyBrailleEdit
{
	partial class DualEditFindForm
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
            label1 = new System.Windows.Forms.Label();
            txtTarget = new System.Windows.Forms.TextBox();
            btnFind = new System.Windows.Forms.Button();
            chkCaseSensitive = new System.Windows.Forms.CheckBox();
            btnClose = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(18, 32);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(94, 19);
            label1.TabIndex = 0;
            label1.Text = "尋找目標(&N):";
            // 
            // txtTarget
            // 
            txtTarget.Location = new System.Drawing.Point(155, 28);
            txtTarget.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtTarget.MaxLength = 20;
            txtTarget.Name = "txtTarget";
            txtTarget.Size = new System.Drawing.Size(226, 27);
            txtTarget.TabIndex = 1;
            // 
            // btnFind
            // 
            btnFind.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnFind.Location = new System.Drawing.Point(22, 158);
            btnFind.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnFind.Name = "btnFind";
            btnFind.Size = new System.Drawing.Size(169, 46);
            btnFind.TabIndex = 3;
            btnFind.Text = "尋找下一筆(&F)";
            btnFind.UseVisualStyleBackColor = true;
            btnFind.Click += btnFind_Click;
            // 
            // chkCaseSensitive
            // 
            chkCaseSensitive.AutoSize = true;
            chkCaseSensitive.Location = new System.Drawing.Point(21, 94);
            chkCaseSensitive.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            chkCaseSensitive.Name = "chkCaseSensitive";
            chkCaseSensitive.Size = new System.Drawing.Size(103, 23);
            chkCaseSensitive.TabIndex = 2;
            chkCaseSensitive.Text = "大小寫相符";
            chkCaseSensitive.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            btnClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnClose.Location = new System.Drawing.Point(269, 158);
            btnClose.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(112, 46);
            btnClose.TabIndex = 4;
            btnClose.Text = "關閉";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // DualEditFindForm
            // 
            AcceptButton = btnFind;
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btnClose;
            ClientSize = new System.Drawing.Size(407, 219);
            Controls.Add(btnClose);
            Controls.Add(chkCaseSensitive);
            Controls.Add(btnFind);
            Controls.Add(txtTarget);
            Controls.Add(label1);
            Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            Name = "DualEditFindForm";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "尋找";
            FormClosing += DualEditFindForm_FormClosing;
            Load += DualEditFindForm_Load;
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtTarget;
		private System.Windows.Forms.Button btnFind;
		private System.Windows.Forms.CheckBox chkCaseSensitive;
        private System.Windows.Forms.Button btnClose;
    }
}