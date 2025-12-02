namespace EasyBrailleEdit
{
	partial class DualEditTitleForm
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
            brGrid = new SourceGrid.Grid();
            panel1 = new System.Windows.Forms.Panel();
            label1 = new System.Windows.Forms.Label();
            btnAbortEdit = new System.Windows.Forms.Button();
            btnSave = new System.Windows.Forms.Button();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            statMessage = new System.Windows.Forms.ToolStripStatusLabel();
            statusCurrentText = new System.Windows.Forms.StatusStrip();
            statusLabelCurrentWord = new System.Windows.Forms.ToolStripStatusLabel();
            statusLabelCurrentLine = new System.Windows.Forms.ToolStripStatusLabel();
            panel1.SuspendLayout();
            statusStrip1.SuspendLayout();
            statusCurrentText.SuspendLayout();
            SuspendLayout();
            // 
            // brGrid
            // 
            brGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            brGrid.ColumnsCount = 1;
            brGrid.CustomSort = true;
            brGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            brGrid.DragOffset = 0;
            brGrid.EnableSmoothScrolling = false;
            brGrid.EnableSort = true;
            brGrid.FixedColumns = 1;
            brGrid.FixedRows = 1;
            brGrid.HScrollBarVisible = false;
            brGrid.IsCustomAreaAutoScrollEnabled = false;
            brGrid.Location = new System.Drawing.Point(0, 76);
            brGrid.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            brGrid.Name = "brGrid";
            brGrid.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            brGrid.RowsCount = 1;
            brGrid.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            brGrid.Size = new System.Drawing.Size(1011, 575);
            brGrid.TabIndex = 3;
            brGrid.TabStop = true;
            brGrid.ToolTipText = "";
            brGrid.VScrollBarVisible = false;
            // 
            // panel1
            // 
            panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            panel1.Controls.Add(label1);
            panel1.Controls.Add(btnAbortEdit);
            panel1.Controls.Add(btnSave);
            panel1.Dock = System.Windows.Forms.DockStyle.Top;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(1011, 76);
            panel1.TabIndex = 4;
            // 
            // label1
            // 
            label1.Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label1.ForeColor = System.Drawing.Color.Maroon;
            label1.Location = new System.Drawing.Point(338, 16);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(584, 43);
            label1.TabIndex = 8;
            label1.Text = "頁標題的長度最好不要超過 30 方，否則列印時可能會截斷。";
            // 
            // btnAbortEdit
            // 
            btnAbortEdit.Location = new System.Drawing.Point(152, 13);
            btnAbortEdit.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnAbortEdit.Name = "btnAbortEdit";
            btnAbortEdit.Size = new System.Drawing.Size(125, 46);
            btnAbortEdit.TabIndex = 7;
            btnAbortEdit.Text = "放棄修改(&X)";
            btnAbortEdit.UseVisualStyleBackColor = true;
            btnAbortEdit.Click += btnAbortEdit_Click;
            // 
            // btnSave
            // 
            btnSave.Location = new System.Drawing.Point(11, 13);
            btnSave.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(125, 46);
            btnSave.TabIndex = 6;
            btnSave.Text = "儲存並離開(&S)";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { statMessage });
            statusStrip1.Location = new System.Drawing.Point(0, 682);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            statusStrip1.Size = new System.Drawing.Size(1011, 22);
            statusStrip1.TabIndex = 5;
            statusStrip1.Text = "statusStrip1";
            // 
            // statMessage
            // 
            statMessage.Name = "statMessage";
            statMessage.Size = new System.Drawing.Size(0, 17);
            // 
            // statusCurrentText
            // 
            statusCurrentText.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            statusCurrentText.ImageScalingSize = new System.Drawing.Size(20, 20);
            statusCurrentText.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { statusLabelCurrentWord, statusLabelCurrentLine });
            statusCurrentText.Location = new System.Drawing.Point(0, 651);
            statusCurrentText.Name = "statusCurrentText";
            statusCurrentText.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            statusCurrentText.Size = new System.Drawing.Size(1011, 31);
            statusCurrentText.TabIndex = 7;
            statusCurrentText.Text = "statusStrip2";
            // 
            // statusLabelCurrentWord
            // 
            statusLabelCurrentWord.AutoSize = false;
            statusLabelCurrentWord.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom;
            statusLabelCurrentWord.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            statusLabelCurrentWord.Name = "statusLabelCurrentWord";
            statusLabelCurrentWord.Size = new System.Drawing.Size(520, 26);
            statusLabelCurrentWord.Text = "toolStripStatusLabel1";
            statusLabelCurrentWord.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            statusLabelCurrentWord.ToolTipText = "目前選取的字";
            // 
            // statusLabelCurrentLine
            // 
            statusLabelCurrentLine.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom;
            statusLabelCurrentLine.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            statusLabelCurrentLine.Name = "statusLabelCurrentLine";
            statusLabelCurrentLine.Size = new System.Drawing.Size(165, 26);
            statusLabelCurrentLine.Text = "statusLabelCurrentLine";
            statusLabelCurrentLine.ToolTipText = "游標所在的那一行的明眼字";
            // 
            // DualEditTitleForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1011, 704);
            Controls.Add(brGrid);
            Controls.Add(statusCurrentText);
            Controls.Add(statusStrip1);
            Controls.Add(panel1);
            Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            KeyPreview = true;
            Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            Name = "DualEditTitleForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "編輯頁標題";
            WindowState = System.Windows.Forms.FormWindowState.Maximized;
            Load += DualEditTitleForm_Load;
            KeyDown += DualEditTitleForm_KeyDown;
            panel1.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            statusCurrentText.ResumeLayout(false);
            statusCurrentText.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private SourceGrid.Grid brGrid;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button btnAbortEdit;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Label label1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statMessage;
        private System.Windows.Forms.StatusStrip statusCurrentText;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelCurrentWord;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelCurrentLine;
    }
}