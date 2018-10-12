namespace EasyBrailleEdit.Forms
{
    partial class UndoBufferForm
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
            this.lbxUndoableOperations = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkAlwaysShowThis = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbxUndoableOperations
            // 
            this.lbxUndoableOperations.BackColor = System.Drawing.SystemColors.Info;
            this.lbxUndoableOperations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbxUndoableOperations.FormattingEnabled = true;
            this.lbxUndoableOperations.ItemHeight = 15;
            this.lbxUndoableOperations.Location = new System.Drawing.Point(0, 0);
            this.lbxUndoableOperations.Name = "lbxUndoableOperations";
            this.lbxUndoableOperations.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lbxUndoableOperations.Size = new System.Drawing.Size(307, 365);
            this.lbxUndoableOperations.TabIndex = 0;
            this.lbxUndoableOperations.Click += new System.EventHandler(this.lbxUndoableOperations_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.chkAlwaysShowThis);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 320);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(307, 45);
            this.panel1.TabIndex = 1;
            // 
            // chkAlwaysShowThis
            // 
            this.chkAlwaysShowThis.AutoSize = true;
            this.chkAlwaysShowThis.Location = new System.Drawing.Point(12, 15);
            this.chkAlwaysShowThis.Name = "chkAlwaysShowThis";
            this.chkAlwaysShowThis.Size = new System.Drawing.Size(134, 19);
            this.chkAlwaysShowThis.TabIndex = 0;
            this.chkAlwaysShowThis.Text = "總是顯示此視窗";
            this.chkAlwaysShowThis.UseVisualStyleBackColor = true;
            this.chkAlwaysShowThis.CheckedChanged += new System.EventHandler(this.chkAlwaysShowThis_CheckedChanged);
            // 
            // UndoBufferForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(307, 365);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lbxUndoableOperations);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.Name = "UndoBufferForm";
            this.Text = "可復原的操作";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UndoBufferForm_FormClosing);
            this.Load += new System.EventHandler(this.UndoBufferForm_Load);
            this.Shown += new System.EventHandler(this.UndoBufferForm_Shown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lbxUndoableOperations;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox chkAlwaysShowThis;
    }
}