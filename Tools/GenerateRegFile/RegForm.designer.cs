namespace GenerateRegFile
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
            this.label2 = new System.Windows.Forms.Label();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.btnSaveFile = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtTel = new System.Windows.Forms.TextBox();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.txtLicenseKey = new System.Windows.Forms.TextBox();
            this.txtContactName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cboVersionName = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.dateExpired = new System.Windows.Forms.DateTimePicker();
            this.btnGenKey = new System.Windows.Forms.Button();
            this.btnDecryptRegFile = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(42, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(142, 19);
            this.label2.TabIndex = 0;
            this.label2.Text = "*公司/組織名稱:";
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.Location = new System.Drawing.Point(190, 30);
            this.txtCustomerName.MaxLength = 40;
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.Size = new System.Drawing.Size(260, 29);
            this.txtCustomerName.TabIndex = 1;
            // 
            // btnSaveFile
            // 
            this.btnSaveFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveFile.Location = new System.Drawing.Point(409, 423);
            this.btnSaveFile.Name = "btnSaveFile";
            this.btnSaveFile.Size = new System.Drawing.Size(88, 41);
            this.btnSaveFile.TabIndex = 13;
            this.btnSaveFile.Text = "存檔";
            this.btnSaveFile.UseVisualStyleBackColor = true;
            this.btnSaveFile.Click += new System.EventHandler(this.btnReg_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(132, 136);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 19);
            this.label3.TabIndex = 6;
            this.label3.Text = "電話:";
            // 
            // txtTel
            // 
            this.txtTel.Location = new System.Drawing.Point(190, 136);
            this.txtTel.MaxLength = 20;
            this.txtTel.Name = "txtTel";
            this.txtTel.Size = new System.Drawing.Size(147, 29);
            this.txtTel.TabIndex = 7;
            // 
            // txtAddress
            // 
            this.txtAddress.Location = new System.Drawing.Point(190, 171);
            this.txtAddress.MaxLength = 60;
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(329, 29);
            this.txtAddress.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(132, 174);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 19);
            this.label4.TabIndex = 8;
            this.label4.Text = "地址:";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(503, 423);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(88, 41);
            this.btnClose.TabIndex = 14;
            this.btnClose.Text = "關閉";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(119, 247);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 19);
            this.label5.TabIndex = 10;
            this.label5.Text = "序號:";
            // 
            // txtLicenseKey
            // 
            this.txtLicenseKey.Location = new System.Drawing.Point(179, 243);
            this.txtLicenseKey.MaxLength = 20;
            this.txtLicenseKey.Name = "txtLicenseKey";
            this.txtLicenseKey.Size = new System.Drawing.Size(260, 29);
            this.txtLicenseKey.TabIndex = 11;
            // 
            // txtContactName
            // 
            this.txtContactName.Location = new System.Drawing.Point(190, 65);
            this.txtContactName.MaxLength = 30;
            this.txtContactName.Name = "txtContactName";
            this.txtContactName.Size = new System.Drawing.Size(147, 29);
            this.txtContactName.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(75, 68);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(109, 19);
            this.label6.TabIndex = 2;
            this.label6.Text = "聯絡人姓名:";
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(190, 100);
            this.txtEmail.MaxLength = 50;
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(260, 29);
            this.txtEmail.TabIndex = 5;
            this.txtEmail.Text = "請填寫正確的電子郵件位址";
            this.txtEmail.Enter += new System.EventHandler(this.txtEmail_Enter);
            this.txtEmail.Leave += new System.EventHandler(this.txtEmail_Leave);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(119, 103);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 19);
            this.label7.TabIndex = 4;
            this.label7.Text = "E-Mail:";
            // 
            // txtFileName
            // 
            this.txtFileName.Location = new System.Drawing.Point(179, 351);
            this.txtFileName.MaxLength = 40;
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(260, 29);
            this.txtFileName.TabIndex = 16;
            this.txtFileName.Text = "C:\\Temp\\userreg.dat";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(83, 354);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 19);
            this.label1.TabIndex = 15;
            this.label1.Text = "檔案名稱:";
            // 
            // cboVersionName
            // 
            this.cboVersionName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboVersionName.FormattingEnabled = true;
            this.cboVersionName.Items.AddRange(new object[] {
            "CMU",
            "ENT",
            "PRO",
            "STD",
            "TRI"});
            this.cboVersionName.Location = new System.Drawing.Point(179, 313);
            this.cboVersionName.Name = "cboVersionName";
            this.cboVersionName.Size = new System.Drawing.Size(200, 26);
            this.cboVersionName.TabIndex = 20;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(119, 316);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(52, 19);
            this.label8.TabIndex = 19;
            this.label8.Text = "版本:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(83, 285);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(90, 19);
            this.label9.TabIndex = 18;
            this.label9.Text = "有效期限:";
            // 
            // dateExpired
            // 
            this.dateExpired.Location = new System.Drawing.Point(179, 278);
            this.dateExpired.Name = "dateExpired";
            this.dateExpired.Size = new System.Drawing.Size(200, 29);
            this.dateExpired.TabIndex = 17;
            // 
            // btnGenKey
            // 
            this.btnGenKey.Location = new System.Drawing.Point(445, 240);
            this.btnGenKey.Name = "btnGenKey";
            this.btnGenKey.Size = new System.Drawing.Size(63, 32);
            this.btnGenKey.TabIndex = 21;
            this.btnGenKey.Text = "產生";
            this.btnGenKey.UseVisualStyleBackColor = true;
            this.btnGenKey.Click += new System.EventHandler(this.btnGenKey_Click);
            // 
            // btnDecryptRegFile
            // 
            this.btnDecryptRegFile.Location = new System.Drawing.Point(445, 347);
            this.btnDecryptRegFile.Name = "btnDecryptRegFile";
            this.btnDecryptRegFile.Size = new System.Drawing.Size(63, 33);
            this.btnDecryptRegFile.TabIndex = 22;
            this.btnDecryptRegFile.Text = "反解";
            this.btnDecryptRegFile.UseVisualStyleBackColor = true;
            this.btnDecryptRegFile.Click += new System.EventHandler(this.btnDecryptRegFile_Click);
            // 
            // RegForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(625, 476);
            this.Controls.Add(this.btnDecryptRegFile);
            this.Controls.Add(this.btnGenKey);
            this.Controls.Add(this.cboVersionName);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.dateExpired);
            this.Controls.Add(this.txtFileName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtContactName);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtLicenseKey);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.txtAddress);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtTel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnSaveFile);
            this.Controls.Add(this.txtCustomerName);
            this.Controls.Add(this.label2);
            this.Font = new System.Drawing.Font("PMingLiU", 11F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RegForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "註冊檔案產生器";
            this.Load += new System.EventHandler(this.RegForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtCustomerName;
        private System.Windows.Forms.Button btnSaveFile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtTel;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtLicenseKey;
        private System.Windows.Forms.TextBox txtContactName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboVersionName;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.DateTimePicker dateExpired;
        private System.Windows.Forms.Button btnGenKey;
        private System.Windows.Forms.Button btnDecryptRegFile;
    }
}

