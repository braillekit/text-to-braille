namespace EasyBrailleEdit
{
    partial class TextPageSetupDialog
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
            cboPaper = new System.Windows.Forms.ComboBox();
            groupBox1 = new System.Windows.Forms.GroupBox();
            rdoUserDefinedPaper = new System.Windows.Forms.RadioButton();
            rdoProgramDefinedPaper = new System.Windows.Forms.RadioButton();
            grpTextMargins1 = new System.Windows.Forms.GroupBox();
            txtTextMarginRight = new System.Windows.Forms.TextBox();
            txtTextMarginBottom = new System.Windows.Forms.TextBox();
            txtTextMarginLeft = new System.Windows.Forms.TextBox();
            txtTextMarginTop = new System.Windows.Forms.TextBox();
            label14 = new System.Windows.Forms.Label();
            label11 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            label10 = new System.Windows.Forms.Label();
            grpTextMargins2 = new System.Windows.Forms.GroupBox();
            txtTextMarginRightEven = new System.Windows.Forms.TextBox();
            txtTextMarginBottomEven = new System.Windows.Forms.TextBox();
            txtTextMarginLeftEven = new System.Windows.Forms.TextBox();
            txtTextMarginTopEven = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            btnOk = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            cboPaperSource = new System.Windows.Forms.ComboBox();
            numFontSize = new System.Windows.Forms.NumericUpDown();
            label13 = new System.Windows.Forms.Label();
            cboFontName = new System.Windows.Forms.ComboBox();
            label12 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            groupBox1.SuspendLayout();
            grpTextMargins1.SuspendLayout();
            grpTextMargins2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numFontSize).BeginInit();
            SuspendLayout();
            // 
            // cboPaper
            // 
            cboPaper.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboPaper.Enabled = false;
            cboPaper.FormattingEnabled = true;
            cboPaper.Location = new System.Drawing.Point(125, 62);
            cboPaper.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            cboPaper.Name = "cboPaper";
            cboPaper.Size = new System.Drawing.Size(307, 27);
            cboPaper.TabIndex = 1;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(rdoUserDefinedPaper);
            groupBox1.Controls.Add(rdoProgramDefinedPaper);
            groupBox1.Controls.Add(cboPaper);
            groupBox1.Location = new System.Drawing.Point(17, 52);
            groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            groupBox1.Size = new System.Drawing.Size(460, 109);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "紙張大小";
            // 
            // rdoUserDefinedPaper
            // 
            rdoUserDefinedPaper.AutoSize = true;
            rdoUserDefinedPaper.Location = new System.Drawing.Point(22, 62);
            rdoUserDefinedPaper.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            rdoUserDefinedPaper.Name = "rdoUserDefinedPaper";
            rdoUserDefinedPaper.Size = new System.Drawing.Size(87, 23);
            rdoUserDefinedPaper.TabIndex = 3;
            rdoUserDefinedPaper.Text = "自行指定";
            rdoUserDefinedPaper.UseVisualStyleBackColor = true;
            rdoUserDefinedPaper.CheckedChanged += rdoUserDefinedPaper_CheckedChanged;
            // 
            // rdoProgramDefinedPaper
            // 
            rdoProgramDefinedPaper.AutoSize = true;
            rdoProgramDefinedPaper.Checked = true;
            rdoProgramDefinedPaper.Location = new System.Drawing.Point(22, 30);
            rdoProgramDefinedPaper.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            rdoProgramDefinedPaper.Name = "rdoProgramDefinedPaper";
            rdoProgramDefinedPaper.Size = new System.Drawing.Size(132, 23);
            rdoProgramDefinedPaper.TabIndex = 2;
            rdoProgramDefinedPaper.TabStop = true;
            rdoProgramDefinedPaper.Text = "由程式自動設定";
            rdoProgramDefinedPaper.UseVisualStyleBackColor = true;
            // 
            // grpTextMargins1
            // 
            grpTextMargins1.Controls.Add(txtTextMarginRight);
            grpTextMargins1.Controls.Add(txtTextMarginBottom);
            grpTextMargins1.Controls.Add(txtTextMarginLeft);
            grpTextMargins1.Controls.Add(txtTextMarginTop);
            grpTextMargins1.Controls.Add(label14);
            grpTextMargins1.Controls.Add(label11);
            grpTextMargins1.Controls.Add(label9);
            grpTextMargins1.Controls.Add(label10);
            grpTextMargins1.Location = new System.Drawing.Point(17, 190);
            grpTextMargins1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            grpTextMargins1.Name = "grpTextMargins1";
            grpTextMargins1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            grpTextMargins1.Size = new System.Drawing.Size(218, 119);
            grpTextMargins1.TabIndex = 13;
            grpTextMargins1.TabStop = false;
            grpTextMargins1.Text = "奇數頁邊界";
            // 
            // txtTextMarginRight
            // 
            txtTextMarginRight.Location = new System.Drawing.Point(147, 70);
            txtTextMarginRight.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtTextMarginRight.Name = "txtTextMarginRight";
            txtTextMarginRight.Size = new System.Drawing.Size(54, 27);
            txtTextMarginRight.TabIndex = 19;
            // 
            // txtTextMarginBottom
            // 
            txtTextMarginBottom.Location = new System.Drawing.Point(47, 70);
            txtTextMarginBottom.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtTextMarginBottom.Name = "txtTextMarginBottom";
            txtTextMarginBottom.Size = new System.Drawing.Size(54, 27);
            txtTextMarginBottom.TabIndex = 18;
            // 
            // txtTextMarginLeft
            // 
            txtTextMarginLeft.Location = new System.Drawing.Point(147, 30);
            txtTextMarginLeft.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtTextMarginLeft.Name = "txtTextMarginLeft";
            txtTextMarginLeft.Size = new System.Drawing.Size(54, 27);
            txtTextMarginLeft.TabIndex = 17;
            // 
            // txtTextMarginTop
            // 
            txtTextMarginTop.Location = new System.Drawing.Point(47, 30);
            txtTextMarginTop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtTextMarginTop.Name = "txtTextMarginTop";
            txtTextMarginTop.Size = new System.Drawing.Size(54, 27);
            txtTextMarginTop.TabIndex = 16;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new System.Drawing.Point(114, 73);
            label14.Name = "label14";
            label14.Size = new System.Drawing.Size(27, 19);
            label14.TabIndex = 14;
            label14.Text = "右:";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new System.Drawing.Point(14, 73);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(27, 19);
            label11.TabIndex = 12;
            label11.Text = "下:";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(14, 33);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(27, 19);
            label9.TabIndex = 8;
            label9.Text = "上:";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new System.Drawing.Point(114, 33);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(27, 19);
            label10.TabIndex = 10;
            label10.Text = "左:";
            // 
            // grpTextMargins2
            // 
            grpTextMargins2.Controls.Add(txtTextMarginRightEven);
            grpTextMargins2.Controls.Add(txtTextMarginBottomEven);
            grpTextMargins2.Controls.Add(txtTextMarginLeftEven);
            grpTextMargins2.Controls.Add(txtTextMarginTopEven);
            grpTextMargins2.Controls.Add(label2);
            grpTextMargins2.Controls.Add(label3);
            grpTextMargins2.Controls.Add(label4);
            grpTextMargins2.Controls.Add(label5);
            grpTextMargins2.Location = new System.Drawing.Point(259, 190);
            grpTextMargins2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            grpTextMargins2.Name = "grpTextMargins2";
            grpTextMargins2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            grpTextMargins2.Size = new System.Drawing.Size(218, 119);
            grpTextMargins2.TabIndex = 14;
            grpTextMargins2.TabStop = false;
            grpTextMargins2.Text = "偶數頁邊界";
            // 
            // txtTextMarginRightEven
            // 
            txtTextMarginRightEven.Location = new System.Drawing.Point(147, 70);
            txtTextMarginRightEven.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtTextMarginRightEven.Name = "txtTextMarginRightEven";
            txtTextMarginRightEven.Size = new System.Drawing.Size(54, 27);
            txtTextMarginRightEven.TabIndex = 19;
            // 
            // txtTextMarginBottomEven
            // 
            txtTextMarginBottomEven.Location = new System.Drawing.Point(47, 70);
            txtTextMarginBottomEven.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtTextMarginBottomEven.Name = "txtTextMarginBottomEven";
            txtTextMarginBottomEven.Size = new System.Drawing.Size(54, 27);
            txtTextMarginBottomEven.TabIndex = 18;
            // 
            // txtTextMarginLeftEven
            // 
            txtTextMarginLeftEven.Location = new System.Drawing.Point(147, 30);
            txtTextMarginLeftEven.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtTextMarginLeftEven.Name = "txtTextMarginLeftEven";
            txtTextMarginLeftEven.Size = new System.Drawing.Size(54, 27);
            txtTextMarginLeftEven.TabIndex = 17;
            // 
            // txtTextMarginTopEven
            // 
            txtTextMarginTopEven.Location = new System.Drawing.Point(47, 30);
            txtTextMarginTopEven.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtTextMarginTopEven.Name = "txtTextMarginTopEven";
            txtTextMarginTopEven.Size = new System.Drawing.Size(54, 27);
            txtTextMarginTopEven.TabIndex = 16;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(111, 73);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(27, 19);
            label2.TabIndex = 14;
            label2.Text = "右:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(14, 73);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(27, 19);
            label3.TabIndex = 12;
            label3.Text = "下:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(14, 33);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(27, 19);
            label4.TabIndex = 8;
            label4.Text = "上:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(111, 33);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(27, 19);
            label5.TabIndex = 10;
            label5.Text = "左:";
            // 
            // btnOk
            // 
            btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnOk.Location = new System.Drawing.Point(302, 467);
            btnOk.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnOk.Name = "btnOk";
            btnOk.Size = new System.Drawing.Size(84, 37);
            btnOk.TabIndex = 15;
            btnOk.Text = "確定";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += btnOk_Click;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(393, 467);
            btnCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(84, 37);
            btnCancel.TabIndex = 16;
            btnCancel.Text = "取消";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(14, 19);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(72, 19);
            label1.TabIndex = 17;
            label1.Text = "紙張來源:";
            // 
            // cboPaperSource
            // 
            cboPaperSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboPaperSource.FormattingEnabled = true;
            cboPaperSource.Location = new System.Drawing.Point(100, 15);
            cboPaperSource.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            cboPaperSource.Name = "cboPaperSource";
            cboPaperSource.Size = new System.Drawing.Size(193, 27);
            cboPaperSource.TabIndex = 18;
            // 
            // numFontSize
            // 
            numFontSize.Location = new System.Drawing.Point(334, 336);
            numFontSize.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            numFontSize.Maximum = new decimal(new int[] { 15, 0, 0, 0 });
            numFontSize.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            numFontSize.Name = "numFontSize";
            numFontSize.Size = new System.Drawing.Size(65, 27);
            numFontSize.TabIndex = 22;
            numFontSize.Value = new decimal(new int[] { 12, 0, 0, 0 });
            numFontSize.ValueChanged += numFontSize_ValueChanged;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new System.Drawing.Point(281, 339);
            label13.Name = "label13";
            label13.Size = new System.Drawing.Size(42, 19);
            label13.TabIndex = 21;
            label13.Text = "大小:";
            // 
            // cboFontName
            // 
            cboFontName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboFontName.FormattingEnabled = true;
            cboFontName.Location = new System.Drawing.Point(64, 336);
            cboFontName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            cboFontName.Name = "cboFontName";
            cboFontName.Size = new System.Drawing.Size(193, 27);
            cboFontName.TabIndex = 20;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new System.Drawing.Point(14, 338);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(42, 19);
            label12.TabIndex = 19;
            label12.Text = "字型:";
            // 
            // label6
            // 
            label6.Font = new System.Drawing.Font("Microsoft JhengHei", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label6.ForeColor = System.Drawing.Color.Maroon;
            label6.Location = new System.Drawing.Point(14, 400);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(464, 63);
            label6.TabIndex = 23;
            label6.Text = "注意: 變更字型大小時，通常也必須一併修改上邊界，否則明眼字的 Y 軸位置可能會與點字重疊或相隔太遠。調整邊界時，建議先以 5 點為單位做調整，再視情況增減。";
            // 
            // TextPageSetupDialog
            // 
            AcceptButton = btnOk;
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new System.Drawing.Size(502, 522);
            Controls.Add(label6);
            Controls.Add(numFontSize);
            Controls.Add(label13);
            Controls.Add(cboFontName);
            Controls.Add(label12);
            Controls.Add(cboPaperSource);
            Controls.Add(label1);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(grpTextMargins2);
            Controls.Add(grpTextMargins1);
            Controls.Add(groupBox1);
            Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "TextPageSetupDialog";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "設定列印格式";
            Load += TextPageSetupDialog_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            grpTextMargins1.ResumeLayout(false);
            grpTextMargins1.PerformLayout();
            grpTextMargins2.ResumeLayout(false);
            grpTextMargins2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numFontSize).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ComboBox cboPaper;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdoUserDefinedPaper;
        private System.Windows.Forms.RadioButton rdoProgramDefinedPaper;
        private System.Windows.Forms.GroupBox grpTextMargins1;
        private System.Windows.Forms.TextBox txtTextMarginRight;
        private System.Windows.Forms.TextBox txtTextMarginBottom;
        private System.Windows.Forms.TextBox txtTextMarginLeft;
        private System.Windows.Forms.TextBox txtTextMarginTop;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox grpTextMargins2;
        private System.Windows.Forms.TextBox txtTextMarginRightEven;
        private System.Windows.Forms.TextBox txtTextMarginBottomEven;
        private System.Windows.Forms.TextBox txtTextMarginLeftEven;
        private System.Windows.Forms.TextBox txtTextMarginTopEven;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboPaperSource;
        private System.Windows.Forms.NumericUpDown numFontSize;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox cboFontName;
        private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label6;
    }
}