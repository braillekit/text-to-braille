using System;
using System.Windows.Forms;
using LicenseKeyBuilder;

namespace LicenseLayout
{
	/// <summary>
	/// Summary description for Random.
	/// </summary>
	public class RandomUI : System.Windows.Forms.Form
	{
		Randomm	rnd;		// the random number 

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtRanlen;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtRanNumber;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton rdobtn10;
		private System.Windows.Forms.RadioButton rdobtn16;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtRanNumStr;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public RandomUI()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// create the randomm class and set the radio button
			//
			rnd = new Randomm();
			rdobtn10.Checked = true;
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
			this.txtRanlen = new System.Windows.Forms.TextBox();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.txtRanNumber = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.rdobtn16 = new System.Windows.Forms.RadioButton();
			this.rdobtn10 = new System.Windows.Forms.RadioButton();
			this.label3 = new System.Windows.Forms.Label();
			this.txtRanNumStr = new System.Windows.Forms.TextBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Length";
			// 
			// txtRanlen
			// 
			this.txtRanlen.Location = new System.Drawing.Point(160, 24);
			this.txtRanlen.Name = "txtRanlen";
			this.txtRanlen.Size = new System.Drawing.Size(104, 20);
			this.txtRanlen.TabIndex = 1;
			this.txtRanlen.Text = "";
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(74, 272);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(48, 24);
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(170, 272);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(48, 24);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "Cancel";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 80);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(96, 16);
			this.label2.TabIndex = 4;
			this.label2.Text = "Random Number";
			// 
			// txtRanNumber
			// 
			this.txtRanNumber.Enabled = false;
			this.txtRanNumber.Location = new System.Drawing.Point(160, 72);
			this.txtRanNumber.Name = "txtRanNumber";
			this.txtRanNumber.Size = new System.Drawing.Size(104, 20);
			this.txtRanNumber.TabIndex = 5;
			this.txtRanNumber.Text = "";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.rdobtn16);
			this.groupBox1.Controls.Add(this.rdobtn10);
			this.groupBox1.Location = new System.Drawing.Point(24, 176);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(248, 80);
			this.groupBox1.TabIndex = 6;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Base";
			// 
			// rdobtn16
			// 
			this.rdobtn16.Location = new System.Drawing.Point(16, 40);
			this.rdobtn16.Name = "rdobtn16";
			this.rdobtn16.TabIndex = 1;
			this.rdobtn16.Text = "16";
			// 
			// rdobtn10
			// 
			this.rdobtn10.Location = new System.Drawing.Point(16, 16);
			this.rdobtn10.Name = "rdobtn10";
			this.rdobtn10.Size = new System.Drawing.Size(104, 16);
			this.rdobtn10.TabIndex = 0;
			this.rdobtn10.Text = "10";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(16, 120);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(128, 16);
			this.label3.TabIndex = 7;
			this.label3.Text = "Random Number String";
			// 
			// txtRanNumStr
			// 
			this.txtRanNumStr.Enabled = false;
			this.txtRanNumStr.Location = new System.Drawing.Point(160, 120);
			this.txtRanNumStr.Name = "txtRanNumStr";
			this.txtRanNumStr.Size = new System.Drawing.Size(104, 20);
			this.txtRanNumStr.TabIndex = 8;
			this.txtRanNumStr.Text = "";
			// 
			// RandomUI
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(292, 320);
			this.Controls.Add(this.txtRanNumStr);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.txtRanNumber);
			this.Controls.Add(this.txtRanlen);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "RandomUI";
			this.ShowInTaskbar = false;
			this.Text = "Random";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			int		rannum; 
			int		length;
			bool	useBase10;

			// determine the base that we want to work in. 
			if ( rdobtn10.Checked ) 
			{
				useBase10 = true;
			}
			else 
			{
				useBase10 = false;
			}
	
			if ( txtRanlen.Text.Length == 0 ) 
			{
				MessageBox.Show("Please Enter a valid Number", "Layout");
				txtRanlen.Focus();
				return;
			}

			try 
			{
				length = Convert.ToInt32(txtRanlen.Text);
				rnd.SetMaxLength(length);
			}
			catch
			{
				MessageBox.Show("Please Enter a valid Number", "Layout");
				txtRanlen.Focus();
				return;
			}

			// get the random number
			rannum = rnd.GetRandomNumber();

			if ( rdobtn10.Checked ) 
			{
				txtRanNumber.Text = rannum.ToString("D");
			}
			else 
			{
				txtRanNumber.Text = rannum.ToString("X");
			}
			txtRanNumStr.Text = NumberDisplay.CreateNumberString((uint)rannum, length, useBase10);
		}
	}
}
