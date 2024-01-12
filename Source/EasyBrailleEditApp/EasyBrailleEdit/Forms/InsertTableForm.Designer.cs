namespace EasyBrailleEdit.Forms
{
    partial class InsertTableForm
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
            numRows = new System.Windows.Forms.NumericUpDown();
            numColumns = new System.Windows.Forms.NumericUpDown();
            label2 = new System.Windows.Forms.Label();
            numCellsPerColumn = new System.Windows.Forms.NumericUpDown();
            label3 = new System.Windows.Forms.Label();
            panel1 = new System.Windows.Forms.Panel();
            panel2 = new System.Windows.Forms.Panel();
            btnCancel = new System.Windows.Forms.Button();
            btnOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)numRows).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numColumns).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numCellsPerColumn).BeginInit();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(26, 24);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(114, 19);
            label1.TabIndex = 0;
            label1.Text = "需要幾個橫列？";
            // 
            // numRows
            // 
            numRows.Location = new System.Drawing.Point(148, 22);
            numRows.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            numRows.Name = "numRows";
            numRows.Size = new System.Drawing.Size(90, 27);
            numRows.TabIndex = 1;
            // 
            // numColumns
            // 
            numColumns.Location = new System.Drawing.Point(148, 62);
            numColumns.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            numColumns.Name = "numColumns";
            numColumns.Size = new System.Drawing.Size(90, 27);
            numColumns.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(26, 64);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(114, 19);
            label2.TabIndex = 2;
            label2.Text = "需要幾個直欄？";
            // 
            // numCellsPerColumn
            // 
            numCellsPerColumn.Location = new System.Drawing.Point(447, 62);
            numCellsPerColumn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            numCellsPerColumn.Name = "numCellsPerColumn";
            numCellsPerColumn.Size = new System.Drawing.Size(89, 27);
            numCellsPerColumn.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(280, 64);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(159, 19);
            label3.TabIndex = 4;
            label3.Text = "每個直欄要幾個空方？";
            // 
            // panel1
            // 
            panel1.Controls.Add(label1);
            panel1.Controls.Add(numCellsPerColumn);
            panel1.Controls.Add(numRows);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(numColumns);
            panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(565, 127);
            panel1.TabIndex = 6;
            // 
            // panel2
            // 
            panel2.Controls.Add(btnCancel);
            panel2.Controls.Add(btnOK);
            panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            panel2.Location = new System.Drawing.Point(0, 127);
            panel2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(565, 73);
            panel2.TabIndex = 7;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(466, 15);
            btnCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(86, 45);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "取消";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnOK.Location = new System.Drawing.Point(372, 15);
            btnOK.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            btnOK.Name = "btnOK";
            btnOK.Size = new System.Drawing.Size(86, 45);
            btnOK.TabIndex = 0;
            btnOK.Text = "確定";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // InsertTableForm
            // 
            AcceptButton = btnOK;
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new System.Drawing.Size(565, 200);
            Controls.Add(panel1);
            Controls.Add(panel2);
            Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            Name = "InsertTableForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "插入表格";
            Load += InsertTableForm_Load;
            ((System.ComponentModel.ISupportInitialize)numRows).EndInit();
            ((System.ComponentModel.ISupportInitialize)numColumns).EndInit();
            ((System.ComponentModel.ISupportInitialize)numCellsPerColumn).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numRows;
        private System.Windows.Forms.NumericUpDown numColumns;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numCellsPerColumn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
    }
}