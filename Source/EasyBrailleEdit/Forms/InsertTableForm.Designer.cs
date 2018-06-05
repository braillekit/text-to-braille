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
            this.label1 = new System.Windows.Forms.Label();
            this.numRows = new System.Windows.Forms.NumericUpDown();
            this.numColumns = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numCellsPerColumn = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numRows)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numColumns)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCellsPerColumn)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "需要幾個橫列？";
            // 
            // numRows
            // 
            this.numRows.Location = new System.Drawing.Point(168, 14);
            this.numRows.Name = "numRows";
            this.numRows.Size = new System.Drawing.Size(83, 29);
            this.numRows.TabIndex = 1;
            // 
            // numColumns
            // 
            this.numColumns.Location = new System.Drawing.Point(168, 61);
            this.numColumns.Name = "numColumns";
            this.numColumns.Size = new System.Drawing.Size(83, 29);
            this.numColumns.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(142, 19);
            this.label2.TabIndex = 2;
            this.label2.Text = "需要幾個直欄？";
            // 
            // numCellsPerColumn
            // 
            this.numCellsPerColumn.Location = new System.Drawing.Point(480, 61);
            this.numCellsPerColumn.Name = "numCellsPerColumn";
            this.numCellsPerColumn.Size = new System.Drawing.Size(83, 29);
            this.numCellsPerColumn.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(274, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(199, 19);
            this.label3.TabIndex = 4;
            this.label3.Text = "每個直欄要幾個空方？";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.numCellsPerColumn);
            this.panel1.Controls.Add(this.numRows);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.numColumns);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(618, 127);
            this.panel1.TabIndex = 6;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnCancel);
            this.panel2.Controls.Add(this.btnOK);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 127);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(618, 72);
            this.panel2.TabIndex = 7;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(480, 13);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(108, 47);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(355, 13);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(108, 47);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "確定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // InsertTableForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(618, 199);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Font = new System.Drawing.Font("PMingLiU", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "InsertTableForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "插入表格";
            this.Load += new System.EventHandler(this.InsertTableForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numRows)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numColumns)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCellsPerColumn)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

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