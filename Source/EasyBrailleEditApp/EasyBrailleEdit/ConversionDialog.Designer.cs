namespace EasyBrailleEdit
{
    partial class ConversionDialog
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
            this.grpPhrases = new System.Windows.Forms.GroupBox();
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.btnMoveUp = new System.Windows.Forms.Button();
            this.btnRemovePhraseTbl = new System.Windows.Forms.Button();
            this.btnAddPhraseTbl = new System.Windows.Forms.Button();
            this.clbPhraseTbl = new System.Windows.Forms.CheckedListBox();
            this.chkRemember = new System.Windows.Forms.CheckBox();
            this.btnConvert = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnCancel = new System.Windows.Forms.Button();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.numLinesPerPage = new System.Windows.Forms.NumericUpDown();
            this.numCellsPerLine = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.grpPhrases.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLinesPerPage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCellsPerLine)).BeginInit();
            this.SuspendLayout();
            // 
            // grpPhrases
            // 
            this.grpPhrases.Controls.Add(this.btnMoveDown);
            this.grpPhrases.Controls.Add(this.btnMoveUp);
            this.grpPhrases.Controls.Add(this.btnRemovePhraseTbl);
            this.grpPhrases.Controls.Add(this.btnAddPhraseTbl);
            this.grpPhrases.Controls.Add(this.clbPhraseTbl);
            this.grpPhrases.Location = new System.Drawing.Point(16, 76);
            this.grpPhrases.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grpPhrases.Name = "grpPhrases";
            this.grpPhrases.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grpPhrases.Size = new System.Drawing.Size(556, 237);
            this.grpPhrases.TabIndex = 4;
            this.grpPhrases.TabStop = false;
            this.grpPhrases.Text = "注音詞庫";
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.Location = new System.Drawing.Point(463, 179);
            this.btnMoveDown.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(76, 37);
            this.btnMoveDown.TabIndex = 4;
            this.btnMoveDown.Text = "下移";
            this.btnMoveDown.UseVisualStyleBackColor = true;
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.Location = new System.Drawing.Point(463, 134);
            this.btnMoveUp.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(76, 37);
            this.btnMoveUp.TabIndex = 3;
            this.btnMoveUp.Text = "上移";
            this.btnMoveUp.UseVisualStyleBackColor = true;
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // btnRemovePhraseTbl
            // 
            this.btnRemovePhraseTbl.Location = new System.Drawing.Point(463, 88);
            this.btnRemovePhraseTbl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnRemovePhraseTbl.Name = "btnRemovePhraseTbl";
            this.btnRemovePhraseTbl.Size = new System.Drawing.Size(76, 37);
            this.btnRemovePhraseTbl.TabIndex = 2;
            this.btnRemovePhraseTbl.Text = "移除";
            this.btnRemovePhraseTbl.UseVisualStyleBackColor = true;
            this.btnRemovePhraseTbl.Click += new System.EventHandler(this.btnRemovePhraseTbl_Click);
            // 
            // btnAddPhraseTbl
            // 
            this.btnAddPhraseTbl.Location = new System.Drawing.Point(463, 43);
            this.btnAddPhraseTbl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAddPhraseTbl.Name = "btnAddPhraseTbl";
            this.btnAddPhraseTbl.Size = new System.Drawing.Size(76, 37);
            this.btnAddPhraseTbl.TabIndex = 1;
            this.btnAddPhraseTbl.Text = "加入";
            this.btnAddPhraseTbl.UseVisualStyleBackColor = true;
            this.btnAddPhraseTbl.Click += new System.EventHandler(this.btnAddPhraseTbl_Click);
            // 
            // clbPhraseTbl
            // 
            this.clbPhraseTbl.FormattingEnabled = true;
            this.helpProvider1.SetHelpString(this.clbPhraseTbl, "您可以自訂注音詞庫，以調整特定詞彙的注音拼音。注意：詞庫的套用順序是由上而下，因此若有重複定義的詞彙，將以最後套用的詞庫為準。");
            this.clbPhraseTbl.Location = new System.Drawing.Point(17, 43);
            this.clbPhraseTbl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.clbPhraseTbl.Name = "clbPhraseTbl";
            this.helpProvider1.SetShowHelp(this.clbPhraseTbl, true);
            this.clbPhraseTbl.Size = new System.Drawing.Size(440, 172);
            this.clbPhraseTbl.TabIndex = 0;
            this.clbPhraseTbl.SelectedIndexChanged += new System.EventHandler(this.clbPhraseTbl_SelectedIndexChanged);
            // 
            // chkRemember
            // 
            this.chkRemember.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkRemember.AutoSize = true;
            this.chkRemember.Checked = true;
            this.chkRemember.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRemember.Location = new System.Drawing.Point(22, 333);
            this.chkRemember.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkRemember.Name = "chkRemember";
            this.chkRemember.Size = new System.Drawing.Size(140, 24);
            this.chkRemember.TabIndex = 5;
            this.chkRemember.Text = "記住這次的設定";
            this.chkRemember.UseVisualStyleBackColor = true;
            // 
            // btnConvert
            // 
            this.btnConvert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConvert.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnConvert.Location = new System.Drawing.Point(304, 333);
            this.btnConvert.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnConvert.Name = "btnConvert";
            this.btnConvert.Size = new System.Drawing.Size(151, 45);
            this.btnConvert.TabIndex = 6;
            this.btnConvert.Text = "執行轉換(&G)";
            this.btnConvert.UseVisualStyleBackColor = true;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "phf";
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "詞庫檔|*.phf";
            this.openFileDialog1.Multiselect = true;
            this.openFileDialog1.Title = "加入詞庫檔";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(461, 333);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(93, 45);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // numLinesPerPage
            // 
            this.numLinesPerPage.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.numLinesPerPage.Location = new System.Drawing.Point(142, 28);
            this.numLinesPerPage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numLinesPerPage.Name = "numLinesPerPage";
            this.numLinesPerPage.Size = new System.Drawing.Size(76, 29);
            this.numLinesPerPage.TabIndex = 1;
            // 
            // numCellsPerLine
            // 
            this.numCellsPerLine.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.numCellsPerLine.Location = new System.Drawing.Point(379, 28);
            this.numCellsPerLine.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numCellsPerLine.Name = "numCellsPerLine";
            this.numCellsPerLine.Size = new System.Drawing.Size(76, 29);
            this.numCellsPerLine.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(259, 31);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 20);
            this.label7.TabIndex = 2;
            this.label7.Text = "每列幾方:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(22, 31);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 20);
            this.label5.TabIndex = 0;
            this.label5.Text = "每頁幾列:";
            // 
            // ConversionDialog
            // 
            this.AcceptButton = this.btnConvert;
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(590, 392);
            this.Controls.Add(this.numLinesPerPage);
            this.Controls.Add(this.numCellsPerLine);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnConvert);
            this.Controls.Add(this.chkRemember);
            this.Controls.Add(this.grpPhrases);
            this.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConversionDialog";
            this.helpProvider1.SetShowHelp(this, true);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "將明眼字轉為點字";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConvertionDialog_FormClosing);
            this.Load += new System.EventHandler(this.ConvertionDialog_Load);
            this.grpPhrases.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numLinesPerPage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCellsPerLine)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpPhrases;
        private System.Windows.Forms.Button btnMoveUp;
        private System.Windows.Forms.Button btnRemovePhraseTbl;
        private System.Windows.Forms.Button btnAddPhraseTbl;
        private System.Windows.Forms.CheckedListBox clbPhraseTbl;
        private System.Windows.Forms.Button btnMoveDown;
        private System.Windows.Forms.CheckBox chkRemember;
        private System.Windows.Forms.Button btnConvert;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.NumericUpDown numLinesPerPage;
        private System.Windows.Forms.NumericUpDown numCellsPerLine;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
    }
}