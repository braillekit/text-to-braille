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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnPageSetup = new System.Windows.Forms.Button();
            this.cboPrinters = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cboDoubleSideEffect = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnPageSetup);
            this.panel1.Controls.Add(this.cboPrinters);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.cboDoubleSideEffect);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(519, 96);
            this.panel1.TabIndex = 0;
            // 
            // btnPageSetup
            // 
            this.btnPageSetup.Location = new System.Drawing.Point(359, 14);
            this.btnPageSetup.Name = "btnPageSetup";
            this.btnPageSetup.Size = new System.Drawing.Size(146, 30);
            this.btnPageSetup.TabIndex = 7;
            this.btnPageSetup.Text = "設定列印格式(&P)";
            this.btnPageSetup.UseVisualStyleBackColor = true;
            this.btnPageSetup.Click += new System.EventHandler(this.btnPageSetup_Click);
            // 
            // cboPrinters
            // 
            this.cboPrinters.FormattingEnabled = true;
            this.cboPrinters.Location = new System.Drawing.Point(102, 17);
            this.cboPrinters.Name = "cboPrinters";
            this.cboPrinters.Size = new System.Drawing.Size(251, 27);
            this.cboPrinters.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 19);
            this.label4.TabIndex = 5;
            this.label4.Text = "印表機:";
            // 
            // cboDoubleSideEffect
            // 
            this.cboDoubleSideEffect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDoubleSideEffect.FormattingEnabled = true;
            this.cboDoubleSideEffect.Items.AddRange(new object[] {
            "無",
            "只印奇數頁",
            "只印偶數頁"});
            this.cboDoubleSideEffect.Location = new System.Drawing.Point(102, 56);
            this.cboDoubleSideEffect.Name = "cboDoubleSideEffect";
            this.cboDoubleSideEffect.Size = new System.Drawing.Size(113, 27);
            this.cboDoubleSideEffect.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 19);
            this.label3.TabIndex = 8;
            this.label3.Text = "雙面列印:";
            // 
            // ConfigTextPrinterPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("PMingLiU", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "ConfigTextPrinterPanel";
            this.Size = new System.Drawing.Size(519, 96);
            this.Load += new System.EventHandler(this.ConfigTextPrinterPanel_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

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
