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
            this.brGrid = new SourceGrid.Grid();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAbortEdit = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // brGrid
            // 
            this.brGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.brGrid.ColumnsCount = 1;
            this.brGrid.CustomSort = true;
            this.brGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.brGrid.EnableSort = true;
            this.brGrid.FixedColumns = 1;
            this.brGrid.FixedRows = 1;
            this.brGrid.Location = new System.Drawing.Point(0, 61);
            this.brGrid.Margin = new System.Windows.Forms.Padding(4);
            this.brGrid.Name = "brGrid";
            this.brGrid.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.brGrid.RowsCount = 1;
            this.brGrid.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.brGrid.Size = new System.Drawing.Size(899, 495);
            this.brGrid.TabIndex = 3;
            this.brGrid.TabStop = true;
            this.brGrid.ToolTipText = "";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnAbortEdit);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(899, 61);
            this.panel1.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.ForeColor = System.Drawing.Color.Maroon;
            this.label1.Location = new System.Drawing.Point(300, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(242, 34);
            this.label1.TabIndex = 8;
            this.label1.Text = "頁標題的長度最好不要超過 30 方，否則列印時可能會截斷。";
            // 
            // btnAbortEdit
            // 
            this.btnAbortEdit.Location = new System.Drawing.Point(153, 10);
            this.btnAbortEdit.Name = "btnAbortEdit";
            this.btnAbortEdit.Size = new System.Drawing.Size(128, 37);
            this.btnAbortEdit.TabIndex = 7;
            this.btnAbortEdit.Text = "放棄修改(&X)";
            this.btnAbortEdit.UseVisualStyleBackColor = true;
            this.btnAbortEdit.Click += new System.EventHandler(this.btnAbortEdit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(10, 10);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(128, 37);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "儲存並離開(&S)";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // DualEditTitleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(899, 556);
            this.Controls.Add(this.brGrid);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("PMingLiU", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "DualEditTitleForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "編輯頁標題";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.DualEditTitleForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DualEditTitleForm_KeyDown);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private SourceGrid.Grid brGrid;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button btnAbortEdit;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Label label1;
	}
}