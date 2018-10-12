using System;
using System.Windows.Forms;
using LicenseKeyBuilder;

namespace LicenseLayout
{
	/// <summary>
	/// Summary description for CheckSum.
	/// </summary>
	public class CheckSumUI : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtInput;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.TextBox txtchksum;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton rdobtn16;
		private System.Windows.Forms.RadioButton rdobtn10;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton rdoBtnChksum1;
		private System.Windows.Forms.RadioButton rdoBtnChksum2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtLength;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public CheckSumUI()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// Initialze the radio butons. 
			//
			rdobtn10.Checked = true;
			rdoBtnChksum1.Checked = true;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.txtInput = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.txtchksum = new System.Windows.Forms.TextBox();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.rdobtn16 = new System.Windows.Forms.RadioButton();
			this.rdobtn10 = new System.Windows.Forms.RadioButton();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.rdoBtnChksum2 = new System.Windows.Forms.RadioButton();
			this.rdoBtnChksum1 = new System.Windows.Forms.RadioButton();
			this.label3 = new System.Windows.Forms.Label();
			this.txtLength = new System.Windows.Forms.TextBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "String";
			// 
			// txtInput
			// 
			this.txtInput.Location = new System.Drawing.Point(96, 16);
			this.txtInput.Name = "txtInput";
			this.txtInput.Size = new System.Drawing.Size(224, 20);
			this.txtInput.TabIndex = 1;
			this.txtInput.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(64, 16);
			this.label2.TabIndex = 2;
			this.label2.Text = "ChkSum";
			// 
			// txtchksum
			// 
			this.txtchksum.Location = new System.Drawing.Point(96, 48);
			this.txtchksum.Name = "txtchksum";
			this.txtchksum.Size = new System.Drawing.Size(224, 20);
			this.txtchksum.TabIndex = 3;
			this.txtchksum.Text = "";
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(101, 264);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(48, 24);
			this.btnOK.TabIndex = 4;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(189, 264);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(48, 24);
			this.btnCancel.TabIndex = 5;
			this.btnCancel.Text = "Cancel";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.rdobtn16);
			this.groupBox1.Controls.Add(this.rdobtn10);
			this.groupBox1.Location = new System.Drawing.Point(40, 144);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(83, 72);
			this.groupBox1.TabIndex = 7;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Base";
			// 
			// rdobtn16
			// 
			this.rdobtn16.Location = new System.Drawing.Point(16, 40);
			this.rdobtn16.Name = "rdobtn16";
			this.rdobtn16.Size = new System.Drawing.Size(48, 24);
			this.rdobtn16.TabIndex = 1;
			this.rdobtn16.Text = "16";
			// 
			// rdobtn10
			// 
			this.rdobtn10.Location = new System.Drawing.Point(16, 16);
			this.rdobtn10.Name = "rdobtn10";
			this.rdobtn10.Size = new System.Drawing.Size(48, 16);
			this.rdobtn10.TabIndex = 0;
			this.rdobtn10.Text = "10";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.rdoBtnChksum2);
			this.groupBox2.Controls.Add(this.rdoBtnChksum1);
			this.groupBox2.Location = new System.Drawing.Point(200, 144);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(91, 72);
			this.groupBox2.TabIndex = 8;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Type";
			// 
			// rdoBtnChksum2
			// 
			this.rdoBtnChksum2.Location = new System.Drawing.Point(8, 40);
			this.rdoBtnChksum2.Name = "rdoBtnChksum2";
			this.rdoBtnChksum2.Size = new System.Drawing.Size(64, 16);
			this.rdoBtnChksum2.TabIndex = 1;
			this.rdoBtnChksum2.Text = "Type 2";
			// 
			// rdoBtnChksum1
			// 
			this.rdoBtnChksum1.Location = new System.Drawing.Point(8, 16);
			this.rdoBtnChksum1.Name = "rdoBtnChksum1";
			this.rdoBtnChksum1.Size = new System.Drawing.Size(64, 16);
			this.rdoBtnChksum1.TabIndex = 0;
			this.rdoBtnChksum1.Text = "Type 1";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(16, 88);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(64, 16);
			this.label3.TabIndex = 9;
			this.label3.Text = "Length";
			// 
			// txtLength
			// 
			this.txtLength.Location = new System.Drawing.Point(96, 88);
			this.txtLength.Name = "txtLength";
			this.txtLength.Size = new System.Drawing.Size(224, 20);
			this.txtLength.TabIndex = 10;
			this.txtLength.Text = "";
			// 
			// CheckSumUI
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(338, 312);
			this.Controls.Add(this.txtLength);
			this.Controls.Add(this.txtchksum);
			this.Controls.Add(this.txtInput);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "CheckSumUI";
			this.ShowInTaskbar = false;
			this.Text = "CheckSum";
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion


		private void btnOK_Click(object sender, System.EventArgs e)
		{
			Checksum	chk;
			string		istr;
			int			slen;
			uint		csum;
			bool		useBase10;
			int			numberLength;

			chk = new Checksum();
			// check the length
			if ( txtLength.Text.Length == 0 ) 
			{
				MessageBox.Show("Enter a valid length", "Checksum");
				txtLength.Focus();
				return;
			}
			slen = Convert.ToInt32(txtLength.Text);

			// see what base we are to use
			if ( rdobtn10.Checked ) 
			{
				useBase10 = true;
			}
			else 
			{
				useBase10 = false;
			}

			//
			// See what type of checksum is requested. 
			//
			if ( rdoBtnChksum1.Checked )
			{
				chk.ChecksumAlgorithm = Checksum.ChecksumType.Type1;
			}
			if ( rdoBtnChksum2.Checked )
			{
				chk.ChecksumAlgorithm = Checksum.ChecksumType.Type2;
			}

			istr = txtInput.Text;
			numberLength = Convert.ToInt32(txtLength.Text);
			chk.CalculateChecksum(istr);
			csum = chk.ChecksumNumber;
			txtchksum.Text = NumberDisplay.CreateNumberString(csum, numberLength, useBase10);
		}
	}
}
