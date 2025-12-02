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
            components = new System.ComponentModel.Container();
            btnPrintText = new System.Windows.Forms.Button();
            btnPrintBraille = new System.Windows.Forms.Button();
            panel2 = new System.Windows.Forms.Panel();
            btnToDefaultTextLineHeight = new System.Windows.Forms.Button();
            txtTextLineHeight = new System.Windows.Forms.TextBox();
            label8 = new System.Windows.Forms.Label();
            btnToDefaultBrailleWidth = new System.Windows.Forms.Button();
            txtBrailleCellWdith = new System.Windows.Forms.TextBox();
            label6 = new System.Windows.Forms.Label();
            tabControl1 = new System.Windows.Forms.TabControl();
            tabText = new System.Windows.Forms.TabPage();
            textPrintCopies = new System.Windows.Forms.NumericUpDown();
            label1 = new System.Windows.Forms.Label();
            btnPageSetup = new System.Windows.Forms.Button();
            cboPrinters = new System.Windows.Forms.ComboBox();
            label4 = new System.Windows.Forms.Label();
            cboPrintTextManualDoubleSide = new System.Windows.Forms.ComboBox();
            label3 = new System.Windows.Forms.Label();
            tabBraille = new System.Windows.Forms.TabPage();
            lblCellsPerLine = new System.Windows.Forms.Label();
            lblLinesPerPage = new System.Windows.Forms.Label();
            chkSendPageBreakAtEof = new System.Windows.Forms.CheckBox();
            cboPrintersForBraille = new System.Windows.Forms.ComboBox();
            chkPrintBraille = new System.Windows.Forms.CheckBox();
            txtBrailleFileName = new DevAge.Windows.Forms.DevAgeTextBoxButton();
            chkPrintBrailleToFile = new System.Windows.Forms.CheckBox();
            label7 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            gboxRange = new System.Windows.Forms.GroupBox();
            label2 = new System.Windows.Forms.Label();
            txtPageRange = new System.Windows.Forms.TextBox();
            rdoPrintRange = new System.Windows.Forms.RadioButton();
            rdoPrintAll = new System.Windows.Forms.RadioButton();
            gboxOptions = new System.Windows.Forms.GroupBox();
            chkChangeStartPageNum = new System.Windows.Forms.CheckBox();
            txtStartPageNumber = new System.Windows.Forms.TextBox();
            chkPrintPageFoot = new System.Windows.Forms.CheckBox();
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            panel1 = new System.Windows.Forms.Panel();
            btnClose = new System.Windows.Forms.Button();
            chkRememberOptions = new System.Windows.Forms.CheckBox();
            panel2.SuspendLayout();
            tabControl1.SuspendLayout();
            tabText.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)textPrintCopies).BeginInit();
            tabBraille.SuspendLayout();
            gboxRange.SuspendLayout();
            gboxOptions.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // btnPrintText
            // 
            btnPrintText.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnPrintText.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            btnPrintText.Location = new System.Drawing.Point(424, 104);
            btnPrintText.Margin = new System.Windows.Forms.Padding(4);
            btnPrintText.Name = "btnPrintText";
            btnPrintText.Size = new System.Drawing.Size(140, 40);
            btnPrintText.TabIndex = 7;
            btnPrintText.Text = "預覽列印(&T)";
            btnPrintText.UseVisualStyleBackColor = false;
            btnPrintText.Click += btnPrintText_Click;
            // 
            // btnPrintBraille
            // 
            btnPrintBraille.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnPrintBraille.BackColor = System.Drawing.Color.LightCyan;
            btnPrintBraille.Location = new System.Drawing.Point(448, 108);
            btnPrintBraille.Margin = new System.Windows.Forms.Padding(4);
            btnPrintBraille.Name = "btnPrintBraille";
            btnPrintBraille.Size = new System.Drawing.Size(124, 43);
            btnPrintBraille.TabIndex = 9;
            btnPrintBraille.Text = "列印點字(&P)";
            btnPrintBraille.UseVisualStyleBackColor = false;
            btnPrintBraille.Click += btnPrintBraille_Click;
            // 
            // panel2
            // 
            panel2.Controls.Add(btnToDefaultTextLineHeight);
            panel2.Controls.Add(txtTextLineHeight);
            panel2.Controls.Add(label8);
            panel2.Controls.Add(btnToDefaultBrailleWidth);
            panel2.Controls.Add(txtBrailleCellWdith);
            panel2.Controls.Add(label6);
            panel2.Controls.Add(tabControl1);
            panel2.Controls.Add(gboxRange);
            panel2.Controls.Add(gboxOptions);
            panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            panel2.Location = new System.Drawing.Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(633, 482);
            panel2.TabIndex = 0;
            // 
            // btnToDefaultTextLineHeight
            // 
            btnToDefaultTextLineHeight.Location = new System.Drawing.Point(328, 176);
            btnToDefaultTextLineHeight.Name = "btnToDefaultTextLineHeight";
            btnToDefaultTextLineHeight.Size = new System.Drawing.Size(124, 31);
            btnToDefaultTextLineHeight.TabIndex = 7;
            btnToDefaultTextLineHeight.Text = "恢復預設值";
            toolTip1.SetToolTip(btnToDefaultTextLineHeight, "設定紙張、邊界、字型");
            btnToDefaultTextLineHeight.UseVisualStyleBackColor = true;
            btnToDefaultTextLineHeight.Click += btnToDefaultTextLineHeight_Click;
            // 
            // txtTextLineHeight
            // 
            txtTextLineHeight.ImeMode = System.Windows.Forms.ImeMode.Off;
            txtTextLineHeight.Location = new System.Drawing.Point(240, 176);
            txtTextLineHeight.Name = "txtTextLineHeight";
            txtTextLineHeight.Size = new System.Drawing.Size(80, 27);
            txtTextLineHeight.TabIndex = 6;
            toolTip1.SetToolTip(txtTextLineHeight, "也就是「字高」加上「列距」。");
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(20, 180);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(159, 19);
            label8.TabIndex = 5;
            label8.Text = "每一列明眼字的高度：";
            // 
            // btnToDefaultBrailleWidth
            // 
            btnToDefaultBrailleWidth.Location = new System.Drawing.Point(328, 140);
            btnToDefaultBrailleWidth.Name = "btnToDefaultBrailleWidth";
            btnToDefaultBrailleWidth.Size = new System.Drawing.Size(124, 31);
            btnToDefaultBrailleWidth.TabIndex = 4;
            btnToDefaultBrailleWidth.Text = "恢復預設值";
            toolTip1.SetToolTip(btnToDefaultBrailleWidth, "設定紙張、邊界、字型");
            btnToDefaultBrailleWidth.UseVisualStyleBackColor = true;
            btnToDefaultBrailleWidth.Click += btnToDefaultBrailleWidth_Click;
            // 
            // txtBrailleCellWdith
            // 
            txtBrailleCellWdith.ImeMode = System.Windows.Forms.ImeMode.Off;
            txtBrailleCellWdith.Location = new System.Drawing.Point(240, 141);
            txtBrailleCellWdith.Name = "txtBrailleCellWdith";
            txtBrailleCellWdith.Size = new System.Drawing.Size(80, 27);
            txtBrailleCellWdith.TabIndex = 3;
            toolTip1.SetToolTip(txtBrailleCellWdith, "此參數也會影響每個明眼字的列印寬度。");
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(20, 145);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(174, 19);
            label6.TabIndex = 2;
            label6.Text = "每一方點字的列印寬度：";
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabText);
            tabControl1.Controls.Add(tabBraille);
            tabControl1.ItemSize = new System.Drawing.Size(60, 25);
            tabControl1.Location = new System.Drawing.Point(16, 216);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(596, 196);
            tabControl1.TabIndex = 8;
            // 
            // tabText
            // 
            tabText.Controls.Add(textPrintCopies);
            tabText.Controls.Add(label1);
            tabText.Controls.Add(btnPageSetup);
            tabText.Controls.Add(btnPrintText);
            tabText.Controls.Add(cboPrinters);
            tabText.Controls.Add(label4);
            tabText.Controls.Add(cboPrintTextManualDoubleSide);
            tabText.Controls.Add(label3);
            tabText.Location = new System.Drawing.Point(4, 29);
            tabText.Name = "tabText";
            tabText.Padding = new System.Windows.Forms.Padding(3);
            tabText.Size = new System.Drawing.Size(588, 163);
            tabText.TabIndex = 0;
            tabText.Text = "明眼字";
            // 
            // textPrintCopies
            // 
            textPrintCopies.Location = new System.Drawing.Point(108, 96);
            textPrintCopies.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
            textPrintCopies.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            textPrintCopies.Name = "textPrintCopies";
            textPrintCopies.Size = new System.Drawing.Size(62, 27);
            textPrintCopies.TabIndex = 6;
            textPrintCopies.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 100);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(72, 19);
            label1.TabIndex = 5;
            label1.Text = "列印份數:";
            // 
            // btnPageSetup
            // 
            btnPageSetup.Location = new System.Drawing.Point(406, 14);
            btnPageSetup.Name = "btnPageSetup";
            btnPageSetup.Size = new System.Drawing.Size(164, 31);
            btnPageSetup.TabIndex = 2;
            btnPageSetup.Text = "設定列印格式(&P)";
            toolTip1.SetToolTip(btnPageSetup, "設定紙張、邊界、字型");
            btnPageSetup.UseVisualStyleBackColor = true;
            btnPageSetup.Click += btnPageSetup_Click;
            // 
            // cboPrinters
            // 
            cboPrinters.FormattingEnabled = true;
            cboPrinters.Location = new System.Drawing.Point(96, 16);
            cboPrinters.Name = "cboPrinters";
            cboPrinters.Size = new System.Drawing.Size(299, 27);
            cboPrinters.TabIndex = 1;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(13, 21);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(69, 19);
            label4.TabIndex = 0;
            label4.Text = "印表機：";
            // 
            // cboPrintTextManualDoubleSide
            // 
            cboPrintTextManualDoubleSide.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboPrintTextManualDoubleSide.FormattingEnabled = true;
            cboPrintTextManualDoubleSide.Items.AddRange(new object[] { "不需要", "只印奇數頁", "只印偶數頁" });
            cboPrintTextManualDoubleSide.Location = new System.Drawing.Point(160, 56);
            cboPrintTextManualDoubleSide.Name = "cboPrintTextManualDoubleSide";
            cboPrintTextManualDoubleSide.Size = new System.Drawing.Size(113, 27);
            cboPrintTextManualDoubleSide.TabIndex = 4;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(13, 60);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(114, 19);
            label3.TabIndex = 3;
            label3.Text = "手動雙面列印：";
            // 
            // tabBraille
            // 
            tabBraille.Controls.Add(lblCellsPerLine);
            tabBraille.Controls.Add(lblLinesPerPage);
            tabBraille.Controls.Add(chkSendPageBreakAtEof);
            tabBraille.Controls.Add(cboPrintersForBraille);
            tabBraille.Controls.Add(chkPrintBraille);
            tabBraille.Controls.Add(txtBrailleFileName);
            tabBraille.Controls.Add(chkPrintBrailleToFile);
            tabBraille.Controls.Add(btnPrintBraille);
            tabBraille.Controls.Add(label7);
            tabBraille.Controls.Add(label5);
            tabBraille.Location = new System.Drawing.Point(4, 29);
            tabBraille.Name = "tabBraille";
            tabBraille.Padding = new System.Windows.Forms.Padding(3);
            tabBraille.Size = new System.Drawing.Size(588, 163);
            tabBraille.TabIndex = 1;
            tabBraille.Text = "點字";
            // 
            // lblCellsPerLine
            // 
            lblCellsPerLine.AutoSize = true;
            lblCellsPerLine.Location = new System.Drawing.Point(521, 33);
            lblCellsPerLine.Name = "lblCellsPerLine";
            lblCellsPerLine.Size = new System.Drawing.Size(23, 19);
            lblCellsPerLine.TabIndex = 4;
            lblCellsPerLine.Text = "xx";
            // 
            // lblLinesPerPage
            // 
            lblLinesPerPage.AutoSize = true;
            lblLinesPerPage.Location = new System.Drawing.Point(521, 13);
            lblLinesPerPage.Name = "lblLinesPerPage";
            lblLinesPerPage.Size = new System.Drawing.Size(23, 19);
            lblLinesPerPage.TabIndex = 2;
            lblLinesPerPage.Text = "xx";
            // 
            // chkSendPageBreakAtEof
            // 
            chkSendPageBreakAtEof.AutoSize = true;
            chkSendPageBreakAtEof.Location = new System.Drawing.Point(17, 20);
            chkSendPageBreakAtEof.Name = "chkSendPageBreakAtEof";
            chkSendPageBreakAtEof.Size = new System.Drawing.Size(193, 23);
            chkSendPageBreakAtEof.TabIndex = 0;
            chkSendPageBreakAtEof.Text = "在文件結尾輸出跳頁符號";
            chkSendPageBreakAtEof.UseVisualStyleBackColor = true;
            // 
            // cboPrintersForBraille
            // 
            cboPrintersForBraille.FormattingEnabled = true;
            cboPrintersForBraille.Location = new System.Drawing.Point(206, 111);
            cboPrintersForBraille.Name = "cboPrintersForBraille";
            cboPrintersForBraille.Size = new System.Drawing.Size(208, 27);
            cboPrintersForBraille.TabIndex = 8;
            // 
            // chkPrintBraille
            // 
            chkPrintBraille.AutoSize = true;
            chkPrintBraille.Checked = true;
            chkPrintBraille.CheckState = System.Windows.Forms.CheckState.Checked;
            chkPrintBraille.Location = new System.Drawing.Point(17, 114);
            chkPrintBraille.Name = "chkPrintBraille";
            chkPrintBraille.Size = new System.Drawing.Size(148, 23);
            chkPrintBraille.TabIndex = 7;
            chkPrintBraille.Text = "輸出至點字印表機";
            chkPrintBraille.UseVisualStyleBackColor = true;
            chkPrintBraille.CheckedChanged += chkPrintBraille_CheckedChanged;
            // 
            // txtBrailleFileName
            // 
            txtBrailleFileName.BackColor = System.Drawing.Color.Transparent;
            txtBrailleFileName.Location = new System.Drawing.Point(152, 64);
            txtBrailleFileName.Name = "txtBrailleFileName";
            txtBrailleFileName.Size = new System.Drawing.Size(416, 32);
            txtBrailleFileName.TabIndex = 6;
            // 
            // chkPrintBrailleToFile
            // 
            chkPrintBrailleToFile.AutoSize = true;
            chkPrintBrailleToFile.Location = new System.Drawing.Point(16, 68);
            chkPrintBrailleToFile.Name = "chkPrintBrailleToFile";
            chkPrintBrailleToFile.Size = new System.Drawing.Size(103, 23);
            chkPrintBrailleToFile.TabIndex = 5;
            chkPrintBrailleToFile.Text = "輸出至檔案";
            toolTip1.SetToolTip(chkPrintBrailleToFile, "若勾選此項，會將列印的點字資料輸出至您指定的檔案。");
            chkPrintBrailleToFile.UseVisualStyleBackColor = true;
            chkPrintBrailleToFile.CheckedChanged += chkPrintBrailleToFile_CheckedChanged;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(434, 33);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(72, 19);
            label7.TabIndex = 3;
            label7.Text = "每列幾方:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(434, 13);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(72, 19);
            label5.TabIndex = 1;
            label5.Text = "每頁幾列:";
            // 
            // gboxRange
            // 
            gboxRange.Controls.Add(label2);
            gboxRange.Controls.Add(txtPageRange);
            gboxRange.Controls.Add(rdoPrintRange);
            gboxRange.Controls.Add(rdoPrintAll);
            gboxRange.Location = new System.Drawing.Point(16, 12);
            gboxRange.Name = "gboxRange";
            gboxRange.Size = new System.Drawing.Size(308, 112);
            gboxRange.TabIndex = 0;
            gboxRange.TabStop = false;
            gboxRange.Text = "範圍";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Microsoft JhengHei", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label2.Location = new System.Drawing.Point(89, 79);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(166, 17);
            label2.TabIndex = 3;
            label2.Text = "輸入頁碼範圍，例如: 1-6。";
            // 
            // txtPageRange
            // 
            txtPageRange.ImeMode = System.Windows.Forms.ImeMode.Off;
            txtPageRange.Location = new System.Drawing.Point(92, 43);
            txtPageRange.Name = "txtPageRange";
            txtPageRange.Size = new System.Drawing.Size(100, 27);
            txtPageRange.TabIndex = 2;
            // 
            // rdoPrintRange
            // 
            rdoPrintRange.AutoSize = true;
            rdoPrintRange.Location = new System.Drawing.Point(18, 49);
            rdoPrintRange.Name = "rdoPrintRange";
            rdoPrintRange.Size = new System.Drawing.Size(57, 23);
            rdoPrintRange.TabIndex = 1;
            rdoPrintRange.TabStop = true;
            rdoPrintRange.Text = "頁數";
            rdoPrintRange.UseVisualStyleBackColor = true;
            rdoPrintRange.CheckedChanged += rdoPrintRange_CheckedChanged;
            // 
            // rdoPrintAll
            // 
            rdoPrintAll.AutoSize = true;
            rdoPrintAll.Checked = true;
            rdoPrintAll.Location = new System.Drawing.Point(18, 24);
            rdoPrintAll.Name = "rdoPrintAll";
            rdoPrintAll.Size = new System.Drawing.Size(57, 23);
            rdoPrintAll.TabIndex = 0;
            rdoPrintAll.TabStop = true;
            rdoPrintAll.Text = "全部";
            rdoPrintAll.UseVisualStyleBackColor = true;
            // 
            // gboxOptions
            // 
            gboxOptions.Controls.Add(chkChangeStartPageNum);
            gboxOptions.Controls.Add(txtStartPageNumber);
            gboxOptions.Controls.Add(chkPrintPageFoot);
            gboxOptions.Location = new System.Drawing.Point(345, 12);
            gboxOptions.Name = "gboxOptions";
            gboxOptions.Size = new System.Drawing.Size(267, 112);
            gboxOptions.TabIndex = 1;
            gboxOptions.TabStop = false;
            gboxOptions.Text = "選項";
            // 
            // chkChangeStartPageNum
            // 
            chkChangeStartPageNum.AutoSize = true;
            chkChangeStartPageNum.Location = new System.Drawing.Point(16, 65);
            chkChangeStartPageNum.Name = "chkChangeStartPageNum";
            chkChangeStartPageNum.Size = new System.Drawing.Size(148, 23);
            chkChangeStartPageNum.TabIndex = 2;
            chkChangeStartPageNum.Text = "重新指定起始頁碼";
            chkChangeStartPageNum.UseVisualStyleBackColor = true;
            chkChangeStartPageNum.CheckStateChanged += chkChangeStartPageNum_CheckStateChanged;
            // 
            // txtStartPageNumber
            // 
            txtStartPageNumber.ImeMode = System.Windows.Forms.ImeMode.Off;
            txtStartPageNumber.Location = new System.Drawing.Point(205, 63);
            txtStartPageNumber.Name = "txtStartPageNumber";
            txtStartPageNumber.RightToLeft = System.Windows.Forms.RightToLeft.No;
            txtStartPageNumber.Size = new System.Drawing.Size(40, 27);
            txtStartPageNumber.TabIndex = 3;
            txtStartPageNumber.Text = "1";
            txtStartPageNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // chkPrintPageFoot
            // 
            chkPrintPageFoot.AutoSize = true;
            chkPrintPageFoot.Location = new System.Drawing.Point(16, 34);
            chkPrintPageFoot.Name = "chkPrintPageFoot";
            chkPrintPageFoot.Size = new System.Drawing.Size(193, 23);
            chkPrintPageFoot.TabIndex = 1;
            chkPrintPageFoot.Text = "列印頁尾（標題、頁碼）";
            chkPrintPageFoot.UseVisualStyleBackColor = true;
            chkPrintPageFoot.CheckStateChanged += chkPrintPageNumber_CheckStateChanged;
            // 
            // panel1
            // 
            panel1.Controls.Add(btnClose);
            panel1.Controls.Add(chkRememberOptions);
            panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            panel1.Location = new System.Drawing.Point(0, 426);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(633, 56);
            panel1.TabIndex = 1;
            // 
            // btnClose
            // 
            btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnClose.Location = new System.Drawing.Point(498, 12);
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(110, 32);
            btnClose.TabIndex = 1;
            btnClose.Text = "關閉(&X)";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // chkRememberOptions
            // 
            chkRememberOptions.AutoSize = true;
            chkRememberOptions.Checked = true;
            chkRememberOptions.CheckState = System.Windows.Forms.CheckState.Checked;
            chkRememberOptions.Location = new System.Drawing.Point(12, 12);
            chkRememberOptions.Name = "chkRememberOptions";
            chkRememberOptions.Size = new System.Drawing.Size(133, 23);
            chkRememberOptions.TabIndex = 0;
            chkRememberOptions.Text = "記住這次的設定";
            chkRememberOptions.UseVisualStyleBackColor = true;
            // 
            // DualPrintDialog
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            CancelButton = btnClose;
            ClientSize = new System.Drawing.Size(633, 482);
            Controls.Add(panel1);
            Controls.Add(panel2);
            Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Margin = new System.Windows.Forms.Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DualPrintDialog";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "列印";
            FormClosing += DualPrintDialog_FormClosing;
            Load += DualPrintDialog_Load;
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            tabControl1.ResumeLayout(false);
            tabText.ResumeLayout(false);
            tabText.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)textPrintCopies).EndInit();
            tabBraille.ResumeLayout(false);
            tabBraille.PerformLayout();
            gboxRange.ResumeLayout(false);
            gboxRange.PerformLayout();
            gboxOptions.ResumeLayout(false);
            gboxOptions.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
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
        private System.Windows.Forms.TextBox txtBrailleCellWdith;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnToDefaultBrailleWidth;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnToDefaultTextLineHeight;
        private System.Windows.Forms.TextBox txtTextLineHeight;
    }
}