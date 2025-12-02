using System.Windows.Forms;

namespace EasyBrailleEdit
{
    public partial class AboutForm : Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            btnClose = new Button();
            panel1 = new Panel();
            lblVersionLicense = new Label();
            label1 = new Label();
            linkLabel1 = new LinkLabel();
            pictureBox1 = new PictureBox();
            lblProductName = new Label();
            lblVesion = new Label();
            panel2 = new Panel();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClose.DialogResult = DialogResult.OK;
            btnClose.Location = new System.Drawing.Point(389, 10);
            btnClose.Margin = new Padding(4);
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(111, 36);
            btnClose.TabIndex = 2;
            btnClose.Text = "關閉(&X)";
            btnClose.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Controls.Add(lblVersionLicense);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(linkLabel1);
            panel1.Controls.Add(pictureBox1);
            panel1.Controls.Add(lblProductName);
            panel1.Controls.Add(lblVesion);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Margin = new Padding(4);
            panel1.Name = "panel1";
            panel1.Padding = new Padding(4);
            panel1.Size = new System.Drawing.Size(510, 229);
            panel1.TabIndex = 0;
            // 
            // lblVersionLicense
            // 
            lblVersionLicense.AutoSize = true;
            lblVersionLicense.Location = new System.Drawing.Point(189, 15);
            lblVersionLicense.Name = "lblVersionLicense";
            lblVersionLicense.Size = new System.Drawing.Size(51, 19);
            lblVersionLicense.TabIndex = 14;
            lblVersionLicense.Text = "label3";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Microsoft JhengHei", 10.8F);
            label1.Location = new System.Drawing.Point(8, 122);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(93, 19);
            label1.TabIndex = 9;
            label1.Text = "Project URL:";
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.Cursor = Cursors.Hand;
            linkLabel1.Font = new System.Drawing.Font("Microsoft JhengHei", 10.2F);
            linkLabel1.Location = new System.Drawing.Point(112, 123);
            linkLabel1.Margin = new Padding(4, 0, 4, 0);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new System.Drawing.Size(123, 18);
            linkLabel1.TabIndex = 8;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "The project's URL";
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (System.Drawing.Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new System.Drawing.Point(4, 6);
            pictureBox1.Margin = new Padding(4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(66, 64);
            pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox1.TabIndex = 6;
            pictureBox1.TabStop = false;
            // 
            // lblProductName
            // 
            lblProductName.AutoSize = true;
            lblProductName.Font = new System.Drawing.Font("Microsoft JhengHei", 10.8F);
            lblProductName.Location = new System.Drawing.Point(96, 15);
            lblProductName.Margin = new Padding(4, 0, 4, 0);
            lblProductName.Name = "lblProductName";
            lblProductName.Size = new System.Drawing.Size(69, 19);
            lblProductName.TabIndex = 3;
            lblProductName.Text = "易點雙視";
            // 
            // lblVesion
            // 
            lblVesion.AutoSize = true;
            lblVesion.Font = new System.Drawing.Font("Microsoft JhengHei", 10.8F);
            lblVesion.Location = new System.Drawing.Point(96, 50);
            lblVesion.Margin = new Padding(5, 0, 5, 0);
            lblVesion.Name = "lblVesion";
            lblVesion.Size = new System.Drawing.Size(108, 19);
            lblVesion.TabIndex = 2;
            lblVesion.Text = "1.0.2008.0103";
            // 
            // panel2
            // 
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Controls.Add(btnClose);
            panel2.Dock = DockStyle.Bottom;
            panel2.Location = new System.Drawing.Point(0, 173);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(510, 56);
            panel2.TabIndex = 3;
            // 
            // AboutForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnClose;
            ClientSize = new System.Drawing.Size(510, 229);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Font = new System.Drawing.Font("Microsoft JhengHei", 10.8F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(5);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "關於 EasyBrailleEdit";
            Load += AboutForm_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblProductName;
		private System.Windows.Forms.Label lblVesion;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.LinkLabel linkLabel1;
        private Label label1;
        private Panel panel2;
        private Label lblVersionLicense;
    }
}