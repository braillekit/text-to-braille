namespace EasyBrailleEdit.Printing
{
    partial class ConfigTextPrinterForm
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
            panel1 = new System.Windows.Forms.Panel();
            btnCancel = new System.Windows.Forms.Button();
            btnOK = new System.Windows.Forms.Button();
            configTextPrinterPanel = new ConfigTextPrinterPanel();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(btnCancel);
            panel1.Controls.Add(btnOK);
            panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            panel1.Location = new System.Drawing.Point(0, 140);
            panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(595, 68);
            panel1.TabIndex = 1;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(460, 13);
            btnCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(109, 41);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "取消(&C)";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnOK.Location = new System.Drawing.Point(348, 13);
            btnOK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnOK.Name = "btnOK";
            btnOK.Size = new System.Drawing.Size(106, 41);
            btnOK.TabIndex = 0;
            btnOK.Text = "確定(&O)";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // configTextPrinterPanel
            // 
            configTextPrinterPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            configTextPrinterPanel.Location = new System.Drawing.Point(0, 0);
            configTextPrinterPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            configTextPrinterPanel.Name = "configTextPrinterPanel";
            configTextPrinterPanel.Size = new System.Drawing.Size(595, 208);
            configTextPrinterPanel.TabIndex = 0;
            // 
            // ConfigTextPrinterForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(595, 208);
            Controls.Add(panel1);
            Controls.Add(configTextPrinterPanel);
            Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            Name = "ConfigTextPrinterForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "設定明眼字印表機";
            Load += ConfigTextPrinterForm_Load;
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ConfigTextPrinterPanel configTextPrinterPanel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}