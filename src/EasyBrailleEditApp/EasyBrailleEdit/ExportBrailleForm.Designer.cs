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
            gboxOptions = new GroupBox();
            chkChangeStartPageNum = new CheckBox();
            txtStartPageNumber = new TextBox();
            chkPrintPageFoot = new CheckBox();
            btnOk = new Button();
            txtBrailleFileName = new DevAge.Windows.Forms.DevAgeTextBoxButton();
            label1 = new Label();
            lblCellsPerLine = new Label();
            lblLinesPerPage = new Label();
            label7 = new Label();
            label5 = new Label();
            btnClose = new Button();
            gboxOptions.SuspendLayout();
            SuspendLayout();
            // 
            // gboxOptions
            // 
            gboxOptions.Controls.Add(chkChangeStartPageNum);
            gboxOptions.Controls.Add(txtStartPageNumber);
            gboxOptions.Controls.Add(chkPrintPageFoot);
            gboxOptions.Location = new System.Drawing.Point(30, 112);
            gboxOptions.Margin = new Padding(4, 4, 4, 4);
            gboxOptions.Name = "gboxOptions";
            gboxOptions.Padding = new Padding(4, 4, 4, 4);
            gboxOptions.Size = new System.Drawing.Size(268, 129);
            gboxOptions.TabIndex = 2;
            gboxOptions.TabStop = false;
            gboxOptions.Text = "選項";
            // 
            // chkChangeStartPageNum
            // 
            chkChangeStartPageNum.AutoSize = true;
            chkChangeStartPageNum.Location = new System.Drawing.Point(21, 79);
            chkChangeStartPageNum.Margin = new Padding(4, 4, 4, 4);
            chkChangeStartPageNum.Name = "chkChangeStartPageNum";
            chkChangeStartPageNum.Size = new System.Drawing.Size(118, 23);
            chkChangeStartPageNum.TabIndex = 2;
            chkChangeStartPageNum.Text = "指定起始頁碼";
            chkChangeStartPageNum.UseVisualStyleBackColor = true;
            // 
            // txtStartPageNumber
            // 
            txtStartPageNumber.Location = new System.Drawing.Point(164, 77);
            txtStartPageNumber.Margin = new Padding(4, 4, 4, 4);
            txtStartPageNumber.Name = "txtStartPageNumber";
            txtStartPageNumber.RightToLeft = RightToLeft.No;
            txtStartPageNumber.Size = new System.Drawing.Size(50, 27);
            txtStartPageNumber.TabIndex = 3;
            txtStartPageNumber.Text = "1";
            txtStartPageNumber.TextAlign = HorizontalAlignment.Right;
            // 
            // chkPrintPageFoot
            // 
            chkPrintPageFoot.AutoSize = true;
            chkPrintPageFoot.Location = new System.Drawing.Point(21, 46);
            chkPrintPageFoot.Margin = new Padding(4, 4, 4, 4);
            chkPrintPageFoot.Name = "chkPrintPageFoot";
            chkPrintPageFoot.Size = new System.Drawing.Size(193, 23);
            chkPrintPageFoot.TabIndex = 1;
            chkPrintPageFoot.Text = "輸出頁尾（標題、頁碼）";
            chkPrintPageFoot.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnOk.Location = new System.Drawing.Point(369, 285);
            btnOk.Margin = new Padding(4, 4, 4, 4);
            btnOk.Name = "btnOk";
            btnOk.Size = new System.Drawing.Size(92, 43);
            btnOk.TabIndex = 7;
            btnOk.Text = "確定";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += btnOk_Click;
            // 
            // txtBrailleFileName
            // 
            txtBrailleFileName.BackColor = System.Drawing.Color.Transparent;
            txtBrailleFileName.Font = new System.Drawing.Font("Microsoft JhengHei", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            txtBrailleFileName.Location = new System.Drawing.Point(30, 52);
            txtBrailleFileName.Name = "txtBrailleFileName";
            txtBrailleFileName.Size = new System.Drawing.Size(531, 29);
            txtBrailleFileName.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(30, 30);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(84, 19);
            label1.TabIndex = 0;
            label1.Text = "檔案名稱：";
            // 
            // lblCellsPerLine
            // 
            lblCellsPerLine.AutoSize = true;
            lblCellsPerLine.Location = new System.Drawing.Point(447, 194);
            lblCellsPerLine.Margin = new Padding(4, 0, 4, 0);
            lblCellsPerLine.Name = "lblCellsPerLine";
            lblCellsPerLine.Size = new System.Drawing.Size(23, 19);
            lblCellsPerLine.TabIndex = 6;
            lblCellsPerLine.Text = "xx";
            // 
            // lblLinesPerPage
            // 
            lblLinesPerPage.AutoSize = true;
            lblLinesPerPage.Location = new System.Drawing.Point(447, 158);
            lblLinesPerPage.Margin = new Padding(4, 0, 4, 0);
            lblLinesPerPage.Name = "lblLinesPerPage";
            lblLinesPerPage.Size = new System.Drawing.Size(23, 19);
            lblLinesPerPage.TabIndex = 4;
            lblLinesPerPage.Text = "xx";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(354, 194);
            label7.Margin = new Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(72, 19);
            label7.TabIndex = 5;
            label7.Text = "每列幾方:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(354, 158);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(72, 19);
            label5.TabIndex = 3;
            label5.Text = "每頁幾列:";
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClose.DialogResult = DialogResult.Cancel;
            btnClose.Location = new System.Drawing.Point(469, 285);
            btnClose.Margin = new Padding(4, 4, 4, 4);
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(92, 43);
            btnClose.TabIndex = 8;
            btnClose.Text = "關閉";
            btnClose.UseVisualStyleBackColor = true;
            // 
            // ExportBrailleForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(591, 352);
            Controls.Add(btnClose);
            Controls.Add(lblCellsPerLine);
            Controls.Add(lblLinesPerPage);
            Controls.Add(label7);
            Controls.Add(label5);
            Controls.Add(label1);
            Controls.Add(txtBrailleFileName);
            Controls.Add(btnOk);
            Controls.Add(gboxOptions);
            Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4, 4, 4, 4);
            Name = "ExportBrailleForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "匯出點字檔";
            Load += ExportBrailleForm_Load;
            gboxOptions.ResumeLayout(false);
            gboxOptions.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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