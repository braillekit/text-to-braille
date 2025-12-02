namespace EasyBrailleEdit.Printing
{
    partial class ConfigTextPrinterPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel1 = new System.Windows.Forms.Panel();
            btnPageSetup = new System.Windows.Forms.Button();
            cboPrinters = new System.Windows.Forms.ComboBox();
            label4 = new System.Windows.Forms.Label();
            cboDoubleSideEffect = new System.Windows.Forms.ComboBox();
            label3 = new System.Windows.Forms.Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(btnPageSetup);
            panel1.Controls.Add(cboPrinters);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(cboDoubleSideEffect);
            panel1.Controls.Add(label3);
            panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Margin = new System.Windows.Forms.Padding(4);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(584, 122);
            panel1.TabIndex = 0;
            // 
            // btnPageSetup
            // 
            btnPageSetup.Location = new System.Drawing.Point(379, 17);
            btnPageSetup.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnPageSetup.Name = "btnPageSetup";
            btnPageSetup.Size = new System.Drawing.Size(146, 31);
            btnPageSetup.TabIndex = 7;
            btnPageSetup.Text = "設定列印格式(&P)";
            btnPageSetup.UseVisualStyleBackColor = true;
            btnPageSetup.Click += btnPageSetup_Click;
            // 
            // cboPrinters
            // 
            cboPrinters.FormattingEnabled = true;
            cboPrinters.Location = new System.Drawing.Point(91, 21);
            cboPrinters.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            cboPrinters.Name = "cboPrinters";
            cboPrinters.Size = new System.Drawing.Size(282, 27);
            cboPrinters.TabIndex = 6;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(28, 25);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(57, 19);
            label4.TabIndex = 5;
            label4.Text = "印表機:";
            // 
            // cboDoubleSideEffect
            // 
            cboDoubleSideEffect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboDoubleSideEffect.FormattingEnabled = true;
            cboDoubleSideEffect.Items.AddRange(new object[] { "無", "只印奇數頁", "只印偶數頁" });
            cboDoubleSideEffect.Location = new System.Drawing.Point(91, 67);
            cboDoubleSideEffect.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            cboDoubleSideEffect.Name = "cboDoubleSideEffect";
            cboDoubleSideEffect.Size = new System.Drawing.Size(127, 27);
            cboDoubleSideEffect.TabIndex = 9;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(13, 70);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(72, 19);
            label3.TabIndex = 8;
            label3.Text = "雙面列印:";
            // 
            // ConfigTextPrinterPanel
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(panel1);
            Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Margin = new System.Windows.Forms.Padding(4);
            Name = "ConfigTextPrinterPanel";
            Size = new System.Drawing.Size(584, 122);
            Load += ConfigTextPrinterPanel_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnPageSetup;
        private System.Windows.Forms.ComboBox cboPrinters;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cboDoubleSideEffect;
        private System.Windows.Forms.Label label3;
    }
}
