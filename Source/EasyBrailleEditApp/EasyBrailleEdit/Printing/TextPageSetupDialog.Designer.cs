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
			this.cboPaper = new System.Windows.Forms.ComboBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.rdoUserDefinedPaper = new System.Windows.Forms.RadioButton();
			this.rdoProgramDefinedPaper = new System.Windows.Forms.RadioButton();
			this.grpTextMargins1 = new System.Windows.Forms.GroupBox();
			this.txtTextMarginRight = new System.Windows.Forms.TextBox();
			this.txtTextMarginBottom = new System.Windows.Forms.TextBox();
			this.txtTextMarginLeft = new System.Windows.Forms.TextBox();
			this.txtTextMarginTop = new System.Windows.Forms.TextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.grpTextMargins2 = new System.Windows.Forms.GroupBox();
			this.txtTextMarginRightEven = new System.Windows.Forms.TextBox();
			this.txtTextMarginBottomEven = new System.Windows.Forms.TextBox();
			this.txtTextMarginLeftEven = new System.Windows.Forms.TextBox();
			this.txtTextMarginTopEven = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.cboPaperSource = new System.Windows.Forms.ComboBox();
			this.numFontSize = new System.Windows.Forms.NumericUpDown();
			this.label13 = new System.Windows.Forms.Label();
			this.cboFontName = new System.Windows.Forms.ComboBox();
			this.label12 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.grpTextMargins1.SuspendLayout();
			this.grpTextMargins2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numFontSize)).BeginInit();
			this.SuspendLayout();
			// 
			// cboPaper
			// 
			this.cboPaper.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboPaper.Enabled = false;
			this.cboPaper.FormattingEnabled = true;
			this.cboPaper.Location = new System.Drawing.Point(111, 49);
			this.cboPaper.Name = "cboPaper";
			this.cboPaper.Size = new System.Drawing.Size(273, 23);
			this.cboPaper.TabIndex = 1;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.rdoUserDefinedPaper);
			this.groupBox1.Controls.Add(this.rdoProgramDefinedPaper);
			this.groupBox1.Controls.Add(this.cboPaper);
			this.groupBox1.Location = new System.Drawing.Point(15, 41);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(409, 86);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "紙張大小";
			// 
			// rdoUserDefinedPaper
			// 
			this.rdoUserDefinedPaper.AutoSize = true;
			this.rdoUserDefinedPaper.Location = new System.Drawing.Point(20, 49);
			this.rdoUserDefinedPaper.Name = "rdoUserDefinedPaper";
			this.rdoUserDefinedPaper.Size = new System.Drawing.Size(85, 19);
			this.rdoUserDefinedPaper.TabIndex = 3;
			this.rdoUserDefinedPaper.Text = "自行指定";
			this.rdoUserDefinedPaper.UseVisualStyleBackColor = true;
			this.rdoUserDefinedPaper.CheckedChanged += new System.EventHandler(this.rdoUserDefinedPaper_CheckedChanged);
			// 
			// rdoProgramDefinedPaper
			// 
			this.rdoProgramDefinedPaper.AutoSize = true;
			this.rdoProgramDefinedPaper.Checked = true;
			this.rdoProgramDefinedPaper.Location = new System.Drawing.Point(20, 24);
			this.rdoProgramDefinedPaper.Name = "rdoProgramDefinedPaper";
			this.rdoProgramDefinedPaper.Size = new System.Drawing.Size(130, 19);
			this.rdoProgramDefinedPaper.TabIndex = 2;
			this.rdoProgramDefinedPaper.TabStop = true;
			this.rdoProgramDefinedPaper.Text = "由程式自動設定";
			this.rdoProgramDefinedPaper.UseVisualStyleBackColor = true;
			// 
			// grpTextMargins1
			// 
			this.grpTextMargins1.Controls.Add(this.txtTextMarginRight);
			this.grpTextMargins1.Controls.Add(this.txtTextMarginBottom);
			this.grpTextMargins1.Controls.Add(this.txtTextMarginLeft);
			this.grpTextMargins1.Controls.Add(this.txtTextMarginTop);
			this.grpTextMargins1.Controls.Add(this.label14);
			this.grpTextMargins1.Controls.Add(this.label11);
			this.grpTextMargins1.Controls.Add(this.label9);
			this.grpTextMargins1.Controls.Add(this.label10);
			this.grpTextMargins1.Location = new System.Drawing.Point(15, 150);
			this.grpTextMargins1.Name = "grpTextMargins1";
			this.grpTextMargins1.Size = new System.Drawing.Size(194, 94);
			this.grpTextMargins1.TabIndex = 13;
			this.grpTextMargins1.TabStop = false;
			this.grpTextMargins1.Text = "奇數頁邊界";
			// 
			// txtTextMarginRight
			// 
			this.txtTextMarginRight.Location = new System.Drawing.Point(131, 55);
			this.txtTextMarginRight.Name = "txtTextMarginRight";
			this.txtTextMarginRight.Size = new System.Drawing.Size(48, 25);
			this.txtTextMarginRight.TabIndex = 19;
			// 
			// txtTextMarginBottom
			// 
			this.txtTextMarginBottom.Location = new System.Drawing.Point(42, 55);
			this.txtTextMarginBottom.Name = "txtTextMarginBottom";
			this.txtTextMarginBottom.Size = new System.Drawing.Size(48, 25);
			this.txtTextMarginBottom.TabIndex = 18;
			// 
			// txtTextMarginLeft
			// 
			this.txtTextMarginLeft.Location = new System.Drawing.Point(131, 24);
			this.txtTextMarginLeft.Name = "txtTextMarginLeft";
			this.txtTextMarginLeft.Size = new System.Drawing.Size(48, 25);
			this.txtTextMarginLeft.TabIndex = 17;
			// 
			// txtTextMarginTop
			// 
			this.txtTextMarginTop.Location = new System.Drawing.Point(42, 24);
			this.txtTextMarginTop.Name = "txtTextMarginTop";
			this.txtTextMarginTop.Size = new System.Drawing.Size(48, 25);
			this.txtTextMarginTop.TabIndex = 16;
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(99, 62);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(26, 15);
			this.label14.TabIndex = 14;
			this.label14.Text = "右:";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(12, 62);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(26, 15);
			this.label11.TabIndex = 12;
			this.label11.Text = "下:";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(12, 31);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(26, 15);
			this.label9.TabIndex = 8;
			this.label9.Text = "上:";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(99, 31);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(26, 15);
			this.label10.TabIndex = 10;
			this.label10.Text = "左:";
			// 
			// grpTextMargins2
			// 
			this.grpTextMargins2.Controls.Add(this.txtTextMarginRightEven);
			this.grpTextMargins2.Controls.Add(this.txtTextMarginBottomEven);
			this.grpTextMargins2.Controls.Add(this.txtTextMarginLeftEven);
			this.grpTextMargins2.Controls.Add(this.txtTextMarginTopEven);
			this.grpTextMargins2.Controls.Add(this.label2);
			this.grpTextMargins2.Controls.Add(this.label3);
			this.grpTextMargins2.Controls.Add(this.label4);
			this.grpTextMargins2.Controls.Add(this.label5);
			this.grpTextMargins2.Location = new System.Drawing.Point(230, 150);
			this.grpTextMargins2.Name = "grpTextMargins2";
			this.grpTextMargins2.Size = new System.Drawing.Size(194, 94);
			this.grpTextMargins2.TabIndex = 14;
			this.grpTextMargins2.TabStop = false;
			this.grpTextMargins2.Text = "偶數頁邊界";
			// 
			// txtTextMarginRightEven
			// 
			this.txtTextMarginRightEven.Location = new System.Drawing.Point(131, 55);
			this.txtTextMarginRightEven.Name = "txtTextMarginRightEven";
			this.txtTextMarginRightEven.Size = new System.Drawing.Size(48, 25);
			this.txtTextMarginRightEven.TabIndex = 19;
			// 
			// txtTextMarginBottomEven
			// 
			this.txtTextMarginBottomEven.Location = new System.Drawing.Point(42, 55);
			this.txtTextMarginBottomEven.Name = "txtTextMarginBottomEven";
			this.txtTextMarginBottomEven.Size = new System.Drawing.Size(48, 25);
			this.txtTextMarginBottomEven.TabIndex = 18;
			// 
			// txtTextMarginLeftEven
			// 
			this.txtTextMarginLeftEven.Location = new System.Drawing.Point(131, 24);
			this.txtTextMarginLeftEven.Name = "txtTextMarginLeftEven";
			this.txtTextMarginLeftEven.Size = new System.Drawing.Size(48, 25);
			this.txtTextMarginLeftEven.TabIndex = 17;
			// 
			// txtTextMarginTopEven
			// 
			this.txtTextMarginTopEven.Location = new System.Drawing.Point(42, 24);
			this.txtTextMarginTopEven.Name = "txtTextMarginTopEven";
			this.txtTextMarginTopEven.Size = new System.Drawing.Size(48, 25);
			this.txtTextMarginTopEven.TabIndex = 16;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(99, 62);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(26, 15);
			this.label2.TabIndex = 14;
			this.label2.Text = "右:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 62);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(26, 15);
			this.label3.TabIndex = 12;
			this.label3.Text = "下:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 31);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(26, 15);
			this.label4.TabIndex = 8;
			this.label4.Text = "上:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(99, 31);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(26, 15);
			this.label5.TabIndex = 10;
			this.label5.Text = "左:";
			// 
			// btnOk
			// 
			this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOk.Location = new System.Drawing.Point(268, 369);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 29);
			this.btnOk.TabIndex = 15;
			this.btnOk.Text = "確定";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(349, 369);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 29);
			this.btnCancel.TabIndex = 16;
			this.btnCancel.Text = "取消";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(71, 15);
			this.label1.TabIndex = 17;
			this.label1.Text = "紙張來源:";
			// 
			// cboPaperSource
			// 
			this.cboPaperSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboPaperSource.FormattingEnabled = true;
			this.cboPaperSource.Location = new System.Drawing.Point(89, 12);
			this.cboPaperSource.Name = "cboPaperSource";
			this.cboPaperSource.Size = new System.Drawing.Size(172, 23);
			this.cboPaperSource.TabIndex = 18;
			// 
			// numFontSize
			// 
			this.numFontSize.Location = new System.Drawing.Point(297, 265);
			this.numFontSize.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
			this.numFontSize.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.numFontSize.Name = "numFontSize";
			this.numFontSize.Size = new System.Drawing.Size(58, 25);
			this.numFontSize.TabIndex = 22;
			this.numFontSize.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
			this.numFontSize.ValueChanged += new System.EventHandler(this.numFontSize_ValueChanged);
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(250, 268);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(41, 15);
			this.label13.TabIndex = 21;
			this.label13.Text = "大小:";
			// 
			// cboFontName
			// 
			this.cboFontName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboFontName.FormattingEnabled = true;
			this.cboFontName.Location = new System.Drawing.Point(57, 265);
			this.cboFontName.Name = "cboFontName";
			this.cboFontName.Size = new System.Drawing.Size(172, 23);
			this.cboFontName.TabIndex = 20;
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(12, 267);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(41, 15);
			this.label12.TabIndex = 19;
			this.label12.Text = "字型:";
			// 
			// label6
			// 
			this.label6.Font = new System.Drawing.Font("MingLiU", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.label6.ForeColor = System.Drawing.Color.Maroon;
			this.label6.Location = new System.Drawing.Point(12, 316);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(412, 50);
			this.label6.TabIndex = 23;
			this.label6.Text = "注意: 變更字型大小時，通常也必須一併修改上邊界，否則明眼字的 Y 軸位置可能會與點字重疊或相隔太遠。調整邊界時，建議先以 5 點為單位做調整，再視情況增減。";
			// 
			// TextPageSetupDialog
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(446, 412);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.numFontSize);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.cboFontName);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.cboPaperSource);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.grpTextMargins2);
			this.Controls.Add(this.grpTextMargins1);
			this.Controls.Add(this.groupBox1);
			this.Font = new System.Drawing.Font("PMingLiU", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TextPageSetupDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "設定列印格式";
			this.Load += new System.EventHandler(this.TextPageSetupDialog_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.grpTextMargins1.ResumeLayout(false);
			this.grpTextMargins1.PerformLayout();
			this.grpTextMargins2.ResumeLayout(false);
			this.grpTextMargins2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numFontSize)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

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