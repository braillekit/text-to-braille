namespace EasyBrailleEdit
{
    partial class RegForm
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
            label2 = new System.Windows.Forms.Label();
            txtCustomerName = new System.Windows.Forms.TextBox();
            btnReg = new System.Windows.Forms.Button();
            btnClose = new System.Windows.Forms.Button();
            txtLicenseKey = new System.Windows.Forms.TextBox();
            label5 = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.ForeColor = System.Drawing.Color.Maroon;
            label1.Location = new System.Drawing.Point(32, 143);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(432, 38);
            label1.TabIndex = 12;
            label1.Text = "注意：註冊程序需要連接網際網路。";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(32, 42);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(94, 19);
            label2.TabIndex = 0;
            label2.Text = "*公司或姓名:";
            // 
            // txtCustomerName
            // 
            txtCustomerName.Location = new System.Drawing.Point(132, 39);
            txtCustomerName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtCustomerName.MaxLength = 40;
            txtCustomerName.Name = "txtCustomerName";
            txtCustomerName.Size = new System.Drawing.Size(292, 27);
            txtCustomerName.TabIndex = 1;
            // 
            // btnReg
            // 
            btnReg.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnReg.Location = new System.Drawing.Point(231, 203);
            btnReg.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnReg.Name = "btnReg";
            btnReg.Size = new System.Drawing.Size(114, 52);
            btnReg.TabIndex = 13;
            btnReg.Text = "註冊(&R)";
            btnReg.UseVisualStyleBackColor = true;
            btnReg.Click += btnReg_Click;
            // 
            // btnClose
            // 
            btnClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnClose.Location = new System.Drawing.Point(351, 203);
            btnClose.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(114, 52);
            btnClose.TabIndex = 14;
            btnClose.Text = "取消";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // txtLicenseKey
            // 
            txtLicenseKey.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            txtLicenseKey.Location = new System.Drawing.Point(132, 85);
            txtLicenseKey.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtLicenseKey.MaxLength = 20;
            txtLicenseKey.Name = "txtLicenseKey";
            txtLicenseKey.Size = new System.Drawing.Size(292, 27);
            txtLicenseKey.TabIndex = 11;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(77, 88);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(49, 19);
            label5.TabIndex = 10;
            label5.Text = "*序號:";
            // 
            // RegForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btnClose;
            ClientSize = new System.Drawing.Size(477, 268);
            Controls.Add(txtLicenseKey);
            Controls.Add(label5);
            Controls.Add(btnClose);
            Controls.Add(btnReg);
            Controls.Add(txtCustomerName);
            Controls.Add(label2);
            Controls.Add(label1);
            Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "RegForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "註冊";
            Load += RegForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtCustomerName;
        private System.Windows.Forms.Button btnReg;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox txtLicenseKey;
        private System.Windows.Forms.Label label5;
    }
}

