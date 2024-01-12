namespace EasyBrailleEdit.Forms
{
    partial class BrailleDocPropertiesForm
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
            numStartPageNumber = new System.Windows.Forms.NumericUpDown();
            label2 = new System.Windows.Forms.Label();
            lblCellsPerLine = new System.Windows.Forms.Label();
            btnOk = new System.Windows.Forms.Button();
            label3 = new System.Windows.Forms.Label();
            btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)numStartPageNumber).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(23, 56);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(84, 19);
            label1.TabIndex = 2;
            label1.Text = "起始頁碼：";
            // 
            // numStartPageNumber
            // 
            numStartPageNumber.Location = new System.Drawing.Point(115, 54);
            numStartPageNumber.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            numStartPageNumber.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            numStartPageNumber.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numStartPageNumber.Name = "numStartPageNumber";
            numStartPageNumber.Size = new System.Drawing.Size(74, 27);
            numStartPageNumber.TabIndex = 3;
            numStartPageNumber.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(23, 25);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(84, 19);
            label2.TabIndex = 0;
            label2.Text = "每列方數：";
            // 
            // lblCellsPerLine
            // 
            lblCellsPerLine.AutoSize = true;
            lblCellsPerLine.Location = new System.Drawing.Point(115, 25);
            lblCellsPerLine.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblCellsPerLine.Name = "lblCellsPerLine";
            lblCellsPerLine.Size = new System.Drawing.Size(51, 19);
            lblCellsPerLine.TabIndex = 1;
            lblCellsPerLine.Text = "label3";
            // 
            // btnOk
            // 
            btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnOk.Location = new System.Drawing.Point(283, 119);
            btnOk.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            btnOk.Name = "btnOk";
            btnOk.Size = new System.Drawing.Size(88, 43);
            btnOk.TabIndex = 5;
            btnOk.Text = "確定";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += btnOk_Click;
            // 
            // label3
            // 
            label3.ForeColor = System.Drawing.Color.Red;
            label3.Location = new System.Drawing.Point(216, 25);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(271, 74);
            label3.TabIndex = 4;
            label3.Text = "注意：若有修改屬性值，再關閉此視窗後，還必須再執行存檔操作，這些修改內容才會保存在檔案裡。";
            // 
            // btnCancel
            // 
            btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(379, 119);
            btnCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(88, 43);
            btnCancel.TabIndex = 6;
            btnCancel.Text = "取消";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // BrailleDocPropertiesForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(501, 175);
            Controls.Add(btnCancel);
            Controls.Add(label3);
            Controls.Add(btnOk);
            Controls.Add(lblCellsPerLine);
            Controls.Add(label2);
            Controls.Add(numStartPageNumber);
            Controls.Add(label1);
            Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            Name = "BrailleDocPropertiesForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "雙視文件的屬性";
            ((System.ComponentModel.ISupportInitialize)numStartPageNumber).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numStartPageNumber;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblCellsPerLine;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnCancel;
    }
}