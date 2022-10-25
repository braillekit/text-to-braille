namespace LicenseLayout
{
    partial class Form1
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnRandom = new System.Windows.Forms.Button();
            this.btnChksum = new System.Windows.Forms.Button();
            this.btnFulltest = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(72, 234);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(154, 37);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "OK";
            // 
            // btnRandom
            // 
            this.btnRandom.Location = new System.Drawing.Point(174, 160);
            this.btnRandom.Name = "btnRandom";
            this.btnRandom.Size = new System.Drawing.Size(154, 37);
            this.btnRandom.TabIndex = 11;
            this.btnRandom.Text = "Random";
            this.btnRandom.Click += new System.EventHandler(this.btnRandom_Click);
            // 
            // btnChksum
            // 
            this.btnChksum.Location = new System.Drawing.Point(174, 25);
            this.btnChksum.Name = "btnChksum";
            this.btnChksum.Size = new System.Drawing.Size(154, 37);
            this.btnChksum.TabIndex = 12;
            this.btnChksum.Text = "ChkSum";
            this.btnChksum.Click += new System.EventHandler(this.btnChksum_Click);
            // 
            // btnFulltest
            // 
            this.btnFulltest.Location = new System.Drawing.Point(174, 86);
            this.btnFulltest.Name = "btnFulltest";
            this.btnFulltest.Size = new System.Drawing.Size(154, 37);
            this.btnFulltest.TabIndex = 14;
            this.btnFulltest.Text = "Full Test";
            this.btnFulltest.Click += new System.EventHandler(this.btnFulltest_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(277, 234);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(153, 37);
            this.btnCancel.TabIndex = 15;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // Form1
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(8, 20);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(490, 314);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnFulltest);
            this.Controls.Add(this.btnChksum);
            this.Controls.Add(this.btnRandom);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Layout";
            this.ResumeLayout(false);

        }

        #endregion
    }
}