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
            // UndoBufferForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(307, 365);
            this.Controls.Add(this.lbxUndoableOperations);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.Name = "UndoBufferForm";
            this.Text = "可復原的操作";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UndoBufferForm_FormClosing);
            this.Load += new System.EventHandler(this.UndoBufferForm_Load);
            this.Shown += new System.EventHandler(this.UndoBufferForm_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lbxUndoableOperations;
    }
}