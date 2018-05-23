using System.Windows.Forms;

namespace EasyBrailleEdit
{
    partial class ExportBrailleForm : Form
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
            this.gboxOptions = new System.Windows.Forms.GroupBox();
            this.chkChangeStartPageNum = new System.Windows.Forms.CheckBox();
            this.txtStartPageNumber = new System.Windows.Forms.TextBox();
            this.chkPrintPageFoot = new System.Windows.Forms.CheckBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.txtBrailleFileName = new DevAge.Windows.Forms.DevAgeTextBoxButton();
            this.label1 = new System.Windows.Forms.Label();
            this.lblCellsPerLine = new System.Windows.Forms.Label();
            this.lblLinesPerPage = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.gboxOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // gboxOptions
            // 
            this.gboxOptions.Controls.Add(this.chkChangeStartPageNum);
            this.gboxOptions.Controls.Add(this.txtStartPageNumber);
            this.gboxOptions.Controls.Add(this.chkPrintPageFoot);
            this.gboxOptions.Location = new System.Drawing.Point(30, 107);
            this.gboxOptions.Name = "gboxOptions";
            this.gboxOptions.Size = new System.Drawing.Size(275, 122);
            this.gboxOptions.TabIndex = 2;
            this.gboxOptions.TabStop = false;
            this.gboxOptions.Text = "選項";
            // 
            // chkChangeStartPageNum
            // 
            this.chkChangeStartPageNum.AutoSize = true;
            this.chkChangeStartPageNum.Location = new System.Drawing.Point(16, 69);
            this.chkChangeStartPageNum.Name = "chkChangeStartPageNum";
            this.chkChangeStartPageNum.Size = new System.Drawing.Size(145, 23);
            this.chkChangeStartPageNum.TabIndex = 2;
            this.chkChangeStartPageNum.Text = "指定起始頁碼";
            this.chkChangeStartPageNum.UseVisualStyleBackColor = true;
            // 
            // txtStartPageNumber
            // 
            this.txtStartPageNumber.Location = new System.Drawing.Point(205, 67);
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
            this.chkPrintPageFoot.Text = "輸出頁尾（標題、頁碼）";
            this.chkPrintPageFoot.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(339, 262);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(106, 43);
            this.btnOk.TabIndex = 7;
            this.btnOk.Text = "確定";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // txtBrailleFileName
            // 
            this.txtBrailleFileName.BackColor = System.Drawing.Color.Transparent;
            this.txtBrailleFileName.Font = new System.Drawing.Font("Microsoft JhengHei", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtBrailleFileName.Location = new System.Drawing.Point(30, 52);
            this.txtBrailleFileName.Name = "txtBrailleFileName";
            this.txtBrailleFileName.Size = new System.Drawing.Size(531, 29);
            this.txtBrailleFileName.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "檔案名稱：";
            // 
            // lblCellsPerLine
            // 
            this.lblCellsPerLine.AutoSize = true;
            this.lblCellsPerLine.Font = new System.Drawing.Font("PMingLiU", 11F);
            this.lblCellsPerLine.Location = new System.Drawing.Point(451, 165);
            this.lblCellsPerLine.Name = "lblCellsPerLine";
            this.lblCellsPerLine.Size = new System.Drawing.Size(27, 19);
            this.lblCellsPerLine.TabIndex = 6;
            this.lblCellsPerLine.Text = "xx";
            // 
            // lblLinesPerPage
            // 
            this.lblLinesPerPage.AutoSize = true;
            this.lblLinesPerPage.Font = new System.Drawing.Font("PMingLiU", 11F);
            this.lblLinesPerPage.Location = new System.Drawing.Point(451, 137);
            this.lblLinesPerPage.Name = "lblLinesPerPage";
            this.lblLinesPerPage.Size = new System.Drawing.Size(27, 19);
            this.lblLinesPerPage.TabIndex = 4;
            this.lblLinesPerPage.Text = "xx";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("PMingLiU", 11F);
            this.label7.Location = new System.Drawing.Point(355, 165);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(90, 19);
            this.label7.TabIndex = 5;
            this.label7.Text = "每列幾方:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("PMingLiU", 11F);
            this.label5.Location = new System.Drawing.Point(355, 137);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(90, 19);
            this.label5.TabIndex = 3;
            this.label5.Text = "每頁幾列:";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(455, 262);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(106, 43);
            this.btnClose.TabIndex = 8;
            this.btnClose.Text = "關閉";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // ExportBrailleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(591, 330);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblCellsPerLine);
            this.Controls.Add(this.lblLinesPerPage);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtBrailleFileName);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.gboxOptions);
            this.Font = new System.Drawing.Font("PMingLiU", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ExportBrailleForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "匯出點字檔";
            this.Load += new System.EventHandler(this.ExportBrailleForm_Load);
            this.gboxOptions.ResumeLayout(false);
            this.gboxOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gboxOptions;
        private System.Windows.Forms.CheckBox chkChangeStartPageNum;
        private System.Windows.Forms.TextBox txtStartPageNumber;
        private System.Windows.Forms.CheckBox chkPrintPageFoot;
        private System.Windows.Forms.Button btnOk;
        private DevAge.Windows.Forms.DevAgeTextBoxButton txtBrailleFileName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblCellsPerLine;
        private System.Windows.Forms.Label lblLinesPerPage;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private Button btnClose;
    }
}