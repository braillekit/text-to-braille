namespace EasyBrailleEdit
{
    partial class DualPrintDialog
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
            this.components = new System.ComponentModel.Container();
            this.btnPrintText = new System.Windows.Forms.Button();
            this.btnPrintBraille = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabText = new System.Windows.Forms.TabPage();
            this.btnPageSetup = new System.Windows.Forms.Button();
            this.cboPrinters = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cboPrintTextManualDoubleSide = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tabBraille = new System.Windows.Forms.TabPage();
            this.lblCellsPerLine = new System.Windows.Forms.Label();
            this.lblLinesPerPage = new System.Windows.Forms.Label();
            this.chkSendPageBreakAtEof = new System.Windows.Forms.CheckBox();
            this.cboPrintersForBraille = new System.Windows.Forms.ComboBox();
            this.chkPrintBraille = new System.Windows.Forms.CheckBox();
            this.txtBrailleFileName = new DevAge.Windows.Forms.DevAgeTextBoxButton();
            this.chkPrintBrailleToFile = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.gboxRange = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPageRange = new System.Windows.Forms.TextBox();
            this.rdoPrintRange = new System.Windows.Forms.RadioButton();
            this.rdoPrintAll = new System.Windows.Forms.RadioButton();
            this.gboxOptions = new System.Windows.Forms.GroupBox();
            this.chkChangeStartPageNum = new System.Windows.Forms.CheckBox();
            this.txtStartPageNumber = new System.Windows.Forms.TextBox();
            this.chkPrintPageFoot = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.chkRememberOptions = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textPrintCopies = new System.Windows.Forms.NumericUpDown();
            this.panel2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabText.SuspendLayout();
            this.tabBraille.SuspendLayout();
            this.gboxRange.SuspendLayout();
            this.gboxOptions.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textPrintCopies)).BeginInit();
            this.SuspendLayout();
            // 
            // btnPrintText
            // 
            this.btnPrintText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPrintText.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            this.btnPrintText.Location = new System.Drawing.Point(16, 130);
            this.btnPrintText.Margin = new System.Windows.Forms.Padding(4);
            this.btnPrintText.Name = "btnPrintText";
            this.btnPrintText.Size = new System.Drawing.Size(140, 35);
            this.btnPrintText.TabIndex = 5;
            this.btnPrintText.Text = "預覽列印(&T)";
            this.btnPrintText.UseVisualStyleBackColor = false;
            this.btnPrintText.Click += new System.EventHandler(this.btnPrintText_Click);
            // 
            // btnPrintBraille
            // 
            this.btnPrintBraille.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPrintBraille.BackColor = System.Drawing.Color.LightCyan;
            this.btnPrintBraille.Location = new System.Drawing.Point(445, 134);
            this.btnPrintBraille.Margin = new System.Windows.Forms.Padding(4);
            this.btnPrintBraille.Name = "btnPrintBraille";
            this.btnPrintBraille.Size = new System.Drawing.Size(120, 43);
            this.btnPrintBraille.TabIndex = 9;
            this.btnPrintBraille.Text = "列印點字(&P)";
            this.btnPrintBraille.UseVisualStyleBackColor = false;
            this.btnPrintBraille.Click += new System.EventHandler(this.btnPrintBraille_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tabControl1);
            this.panel2.Controls.Add(this.gboxRange);
            this.panel2.Controls.Add(this.gboxOptions);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(633, 437);
            this.panel2.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabText);
            this.tabControl1.Controls.Add(this.tabBraille);
            this.tabControl1.ItemSize = new System.Drawing.Size(60, 25);
            this.tabControl1.Location = new System.Drawing.Point(16, 147);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(596, 217);
            this.tabControl1.TabIndex = 2;
            // 
            // tabText
            // 
            this.tabText.Controls.Add(this.textPrintCopies);
            this.tabText.Controls.Add(this.label1);
            this.tabText.Controls.Add(this.btnPageSetup);
            this.tabText.Controls.Add(this.btnPrintText);
            this.tabText.Controls.Add(this.cboPrinters);
            this.tabText.Controls.Add(this.label4);
            this.tabText.Controls.Add(this.cboPrintTextManualDoubleSide);
            this.tabText.Controls.Add(this.label3);
            this.tabText.Location = new System.Drawing.Point(4, 29);
            this.tabText.Name = "tabText";
            this.tabText.Padding = new System.Windows.Forms.Padding(3);
            this.tabText.Size = new System.Drawing.Size(588, 184);
            this.tabText.TabIndex = 0;
            this.tabText.Text = "明眼字";
            // 
            // btnPageSetup
            // 
            this.btnPageSetup.Location = new System.Drawing.Point(395, 14);
            this.btnPageSetup.Name = "btnPageSetup";
            this.btnPageSetup.Size = new System.Drawing.Size(146, 30);
            this.btnPageSetup.TabIndex = 2;
            this.btnPageSetup.Text = "設定列印格式(&P)";
            this.toolTip1.SetToolTip(this.btnPageSetup, "設定紙張、邊界、字型");
            this.btnPageSetup.UseVisualStyleBackColor = true;
            this.btnPageSetup.Click += new System.EventHandler(this.btnPageSetup_Click);
            // 
            // cboPrinters
            // 
            this.cboPrinters.FormattingEnabled = true;
            this.cboPrinters.Location = new System.Drawing.Point(90, 17);
            this.cboPrinters.Name = "cboPrinters";
            this.cboPrinters.Size = new System.Drawing.Size(299, 26);
            this.cboPrinters.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 19);
            this.label4.TabIndex = 0;
            this.label4.Text = "印表機:";
            // 
            // cboPrintTextManualDoubleSide
            // 
            this.cboPrintTextManualDoubleSide.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPrintTextManualDoubleSide.FormattingEnabled = true;
            this.cboPrintTextManualDoubleSide.Items.AddRange(new object[] {
            "不需要",
            "只印奇數頁",
            "只印偶數頁"});
            this.cboPrintTextManualDoubleSide.Location = new System.Drawing.Point(147, 57);
            this.cboPrintTextManualDoubleSide.Name = "cboPrintTextManualDoubleSide";
            this.cboPrintTextManualDoubleSide.Size = new System.Drawing.Size(113, 26);
            this.cboPrintTextManualDoubleSide.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(128, 19);
            this.label3.TabIndex = 3;
            this.label3.Text = "手動雙面列印:";
            // 
            // tabBraille
            // 
            this.tabBraille.Controls.Add(this.lblCellsPerLine);
            this.tabBraille.Controls.Add(this.lblLinesPerPage);
            this.tabBraille.Controls.Add(this.chkSendPageBreakAtEof);
            this.tabBraille.Controls.Add(this.cboPrintersForBraille);
            this.tabBraille.Controls.Add(this.chkPrintBraille);
            this.tabBraille.Controls.Add(this.txtBrailleFileName);
            this.tabBraille.Controls.Add(this.chkPrintBrailleToFile);
            this.tabBraille.Controls.Add(this.btnPrintBraille);
            this.tabBraille.Controls.Add(this.label7);
            this.tabBraille.Controls.Add(this.label5);
            this.tabBraille.Location = new System.Drawing.Point(4, 29);
            this.tabBraille.Name = "tabBraille";
            this.tabBraille.Padding = new System.Windows.Forms.Padding(3);
            this.tabBraille.Size = new System.Drawing.Size(588, 184);
            this.tabBraille.TabIndex = 1;
            this.tabBraille.Text = "點字";
            // 
            // lblCellsPerLine
            // 
            this.lblCellsPerLine.AutoSize = true;
            this.lblCellsPerLine.Font = new System.Drawing.Font("PMingLiU", 11F);
            this.lblCellsPerLine.Location = new System.Drawing.Point(521, 33);
            this.lblCellsPerLine.Name = "lblCellsPerLine";
            this.lblCellsPerLine.Size = new System.Drawing.Size(27, 19);
            this.lblCellsPerLine.TabIndex = 4;
            this.lblCellsPerLine.Text = "xx";
            // 
            // lblLinesPerPage
            // 
            this.lblLinesPerPage.AutoSize = true;
            this.lblLinesPerPage.Font = new System.Drawing.Font("PMingLiU", 11F);
            this.lblLinesPerPage.Location = new System.Drawing.Point(521, 13);
            this.lblLinesPerPage.Name = "lblLinesPerPage";
            this.lblLinesPerPage.Size = new System.Drawing.Size(27, 19);
            this.lblLinesPerPage.TabIndex = 2;
            this.lblLinesPerPage.Text = "xx";
            // 
            // chkSendPageBreakAtEof
            // 
            this.chkSendPageBreakAtEof.AutoSize = true;
            this.chkSendPageBreakAtEof.Location = new System.Drawing.Point(17, 20);
            this.chkSendPageBreakAtEof.Name = "chkSendPageBreakAtEof";
            this.chkSendPageBreakAtEof.Size = new System.Drawing.Size(240, 23);
            this.chkSendPageBreakAtEof.TabIndex = 0;
            this.chkSendPageBreakAtEof.Text = "在文件結尾輸出跳頁符號";
            this.chkSendPageBreakAtEof.UseVisualStyleBackColor = true;
            // 
            // cboPrintersForBraille
            // 
            this.cboPrintersForBraille.FormattingEnabled = true;
            this.cboPrintersForBraille.Location = new System.Drawing.Point(206, 111);
            this.cboPrintersForBraille.Name = "cboPrintersForBraille";
            this.cboPrintersForBraille.Size = new System.Drawing.Size(208, 26);
            this.cboPrintersForBraille.TabIndex = 8;
            // 
            // chkPrintBraille
            // 
            this.chkPrintBraille.AutoSize = true;
            this.chkPrintBraille.Checked = true;
            this.chkPrintBraille.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPrintBraille.Location = new System.Drawing.Point(17, 114);
            this.chkPrintBraille.Name = "chkPrintBraille";
            this.chkPrintBraille.Size = new System.Drawing.Size(183, 23);
            this.chkPrintBraille.TabIndex = 7;
            this.chkPrintBraille.Text = "輸出至點字印表機";
            this.chkPrintBraille.UseVisualStyleBackColor = true;
            this.chkPrintBraille.CheckedChanged += new System.EventHandler(this.chkPrintBraille_CheckedChanged);
            // 
            // txtBrailleFileName
            // 
            this.txtBrailleFileName.BackColor = System.Drawing.Color.Transparent;
            this.txtBrailleFileName.Location = new System.Drawing.Point(149, 70);
            this.txtBrailleFileName.Name = "txtBrailleFileName";
            this.txtBrailleFileName.Size = new System.Drawing.Size(416, 32);
            this.txtBrailleFileName.TabIndex = 6;
            // 
            // chkPrintBrailleToFile
            // 
            this.chkPrintBrailleToFile.AutoSize = true;
            this.chkPrintBrailleToFile.Location = new System.Drawing.Point(17, 74);
            this.chkPrintBrailleToFile.Name = "chkPrintBrailleToFile";
            this.chkPrintBrailleToFile.Size = new System.Drawing.Size(126, 23);
            this.chkPrintBrailleToFile.TabIndex = 5;
            this.chkPrintBrailleToFile.Text = "輸出至檔案";
            this.toolTip1.SetToolTip(this.chkPrintBrailleToFile, "若勾選此項，會將列印的點字資料輸出至您指定的檔案。");
            this.chkPrintBrailleToFile.UseVisualStyleBackColor = true;
            this.chkPrintBrailleToFile.CheckedChanged += new System.EventHandler(this.chkPrintBrailleToFile_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("PMingLiU", 11F);
            this.label7.Location = new System.Drawing.Point(434, 33);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(90, 19);
            this.label7.TabIndex = 3;
            this.label7.Text = "每列幾方:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("PMingLiU", 11F);
            this.label5.Location = new System.Drawing.Point(434, 13);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(90, 19);
            this.label5.TabIndex = 1;
            this.label5.Text = "每頁幾列:";
            // 
            // gboxRange
            // 
            this.gboxRange.Controls.Add(this.label2);
            this.gboxRange.Controls.Add(this.txtPageRange);
            this.gboxRange.Controls.Add(this.rdoPrintRange);
            this.gboxRange.Controls.Add(this.rdoPrintAll);
            this.gboxRange.Location = new System.Drawing.Point(16, 12);
            this.gboxRange.Name = "gboxRange";
            this.gboxRange.Size = new System.Drawing.Size(308, 120);
            this.gboxRange.TabIndex = 0;
            this.gboxRange.TabStop = false;
            this.gboxRange.Text = "範圍";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("PMingLiU", 10F);
            this.label2.Location = new System.Drawing.Point(89, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(207, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "輸入頁碼範圍，例如: 1-6。";
            // 
            // txtPageRange
            // 
            this.txtPageRange.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.txtPageRange.Location = new System.Drawing.Point(92, 43);
            this.txtPageRange.Name = "txtPageRange";
            this.txtPageRange.Size = new System.Drawing.Size(100, 29);
            this.txtPageRange.TabIndex = 2;
            // 
            // rdoPrintRange
            // 
            this.rdoPrintRange.AutoSize = true;
            this.rdoPrintRange.Location = new System.Drawing.Point(18, 49);
            this.rdoPrintRange.Name = "rdoPrintRange";
            this.rdoPrintRange.Size = new System.Drawing.Size(68, 23);
            this.rdoPrintRange.TabIndex = 1;
            this.rdoPrintRange.TabStop = true;
            this.rdoPrintRange.Text = "頁數";
            this.rdoPrintRange.UseVisualStyleBackColor = true;
            this.rdoPrintRange.CheckedChanged += new System.EventHandler(this.rdoPrintRange_CheckedChanged);
            // 
            // rdoPrintAll
            // 
            this.rdoPrintAll.AutoSize = true;
            this.rdoPrintAll.Checked = true;
            this.rdoPrintAll.Location = new System.Drawing.Point(18, 24);
            this.rdoPrintAll.Name = "rdoPrintAll";
            this.rdoPrintAll.Size = new System.Drawing.Size(68, 23);
            this.rdoPrintAll.TabIndex = 0;
            this.rdoPrintAll.TabStop = true;
            this.rdoPrintAll.Text = "全部";
            this.rdoPrintAll.UseVisualStyleBackColor = true;
            // 
            // gboxOptions
            // 
            this.gboxOptions.Controls.Add(this.chkChangeStartPageNum);
            this.gboxOptions.Controls.Add(this.txtStartPageNumber);
            this.gboxOptions.Controls.Add(this.chkPrintPageFoot);
            this.gboxOptions.Location = new System.Drawing.Point(345, 12);
            this.gboxOptions.Name = "gboxOptions";
            this.gboxOptions.Size = new System.Drawing.Size(267, 111);
            this.gboxOptions.TabIndex = 1;
            this.gboxOptions.TabStop = false;
            this.gboxOptions.Text = "選項";
            // 
            // chkChangeStartPageNum
            // 
            this.chkChangeStartPageNum.AutoSize = true;
            this.chkChangeStartPageNum.Location = new System.Drawing.Point(16, 65);
            this.chkChangeStartPageNum.Name = "chkChangeStartPageNum";
            this.chkChangeStartPageNum.Size = new System.Drawing.Size(183, 23);
            this.chkChangeStartPageNum.TabIndex = 2;
            this.chkChangeStartPageNum.Text = "重新指定起始頁碼";
            this.chkChangeStartPageNum.UseVisualStyleBackColor = true;
            this.chkChangeStartPageNum.CheckStateChanged += new System.EventHandler(this.chkChangeStartPageNum_CheckStateChanged);
            // 
            // txtStartPageNumber
            // 
            this.txtStartPageNumber.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.txtStartPageNumber.Location = new System.Drawing.Point(205, 63);
            this.txtStartPageNumber.Name = "txtStartPageNumber";
            this.txtStartPageNumber.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtStartPageNumber.Size = new System.Drawing.Size(40, 29);
            this.txtStartPageNumber.TabIndex = 3;
            this.txtStartPageNumber.Text = "1";
            this.txtStartPageNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // chkPrintPageFoot
            // 
            this.chkPrintPageFoot.AutoSize = true;
            this.chkPrintPageFoot.Location = new System.Drawing.Point(16, 34);
            this.chkPrintPageFoot.Name = "chkPrintPageFoot";
            this.chkPrintPageFoot.Size = new System.Drawing.Size(240, 23);
            this.chkPrintPageFoot.TabIndex = 1;
            this.chkPrintPageFoot.Text = "列印頁尾（標題、頁碼）";
            this.chkPrintPageFoot.UseVisualStyleBackColor = true;
            this.chkPrintPageFoot.CheckStateChanged += new System.EventHandler(this.chkPrintPageNumber_CheckStateChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.chkRememberOptions);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 381);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(633, 56);
            this.panel1.TabIndex = 1;
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(498, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(110, 32);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "關閉(&X)";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // chkRememberOptions
            // 
            this.chkRememberOptions.AutoSize = true;
            this.chkRememberOptions.Checked = true;
            this.chkRememberOptions.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRememberOptions.Location = new System.Drawing.Point(12, 12);
            this.chkRememberOptions.Name = "chkRememberOptions";
            this.chkRememberOptions.Size = new System.Drawing.Size(164, 23);
            this.chkRememberOptions.TabIndex = 0;
            this.chkRememberOptions.Text = "記住這次的設定";
            this.chkRememberOptions.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(299, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 19);
            this.label1.TabIndex = 6;
            this.label1.Text = "列印份數:";
            // 
            // textPrintCopies
            // 
            this.textPrintCopies.Location = new System.Drawing.Point(395, 57);
            this.textPrintCopies.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.textPrintCopies.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.textPrintCopies.Name = "textPrintCopies";
            this.textPrintCopies.Size = new System.Drawing.Size(62, 29);
            this.textPrintCopies.TabIndex = 7;
            this.textPrintCopies.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // DualPrintDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(633, 437);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Font = new System.Drawing.Font("PMingLiU", 11F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DualPrintDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "列印";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DualPrintDialog_FormClosing);
            this.Load += new System.EventHandler(this.DualPrintDialog_Load);
            this.panel2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabText.ResumeLayout(false);
            this.tabText.PerformLayout();
            this.tabBraille.ResumeLayout(false);
            this.tabBraille.PerformLayout();
            this.gboxRange.ResumeLayout(false);
            this.gboxRange.PerformLayout();
            this.gboxOptions.ResumeLayout(false);
            this.gboxOptions.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textPrintCopies)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnPrintBraille;
        private System.Windows.Forms.Button btnPrintText;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox gboxOptions;
		private System.Windows.Forms.TextBox txtStartPageNumber;
        private System.Windows.Forms.CheckBox chkPrintPageFoot;
        private System.Windows.Forms.GroupBox gboxRange;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPageRange;
        private System.Windows.Forms.RadioButton rdoPrintRange;
        private System.Windows.Forms.RadioButton rdoPrintAll;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabText;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cboPrintTextManualDoubleSide;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabPage tabBraille;
        private System.Windows.Forms.ComboBox cboPrinters;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkPrintBrailleToFile;
        private System.Windows.Forms.ToolTip toolTip1;
        private DevAge.Windows.Forms.DevAgeTextBoxButton txtBrailleFileName;
		private System.Windows.Forms.CheckBox chkPrintBraille;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox chkRememberOptions;
		private System.Windows.Forms.Button btnPageSetup;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.CheckBox chkChangeStartPageNum;
        private System.Windows.Forms.ComboBox cboPrintersForBraille;
        private System.Windows.Forms.CheckBox chkSendPageBreakAtEof;
        private System.Windows.Forms.Label lblCellsPerLine;
        private System.Windows.Forms.Label lblLinesPerPage;
        private System.Windows.Forms.NumericUpDown textPrintCopies;
        private System.Windows.Forms.Label label1;
    }
}