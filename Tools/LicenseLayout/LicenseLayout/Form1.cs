using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace LicenseLayout
{
	/// <summary>
	/// Summary description for Form1.
	/// Written by Don Sweitzer (don_sweitzer@hotmail.com)
	/// 
	/// This code snippet demonsttes the use of a listview control and 
	/// the use of creating a dynamic dialog based on
	/// what was entered into a listview. 
	/// 
	/// This file is provided "as is" with no expressed or implied warranty.
	/// The author accepts no liability if it causes any damage to your
	/// computer. 
	///
	/// Expect bugs.
	/// 
	/// Copyright (c) 2004.

	/// </summary>


	
	public class Form1 : System.Windows.Forms.Form
	{
		/// <summary>
		/// Show the column headers that we want displayed in the listview
		/// </summary>
		string [] hstr = {"Field", "Type", "Length"};
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnRandom;
		private System.Windows.Forms.Button btnChksum;
		private System.Windows.Forms.Button btnFulltest;
		private System.Windows.Forms.Button btnCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
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
			this.btnOK = new System.Windows.Forms.Button();
			this.btnRandom = new System.Windows.Forms.Button();
			this.btnChksum = new System.Windows.Forms.Button();
			this.btnFulltest = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(45, 152);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(96, 24);
			this.btnOK.TabIndex = 7;
			this.btnOK.Text = "OK";
			// 
			// btnRandom
			// 
			this.btnRandom.Location = new System.Drawing.Point(109, 104);
			this.btnRandom.Name = "btnRandom";
			this.btnRandom.Size = new System.Drawing.Size(96, 24);
			this.btnRandom.TabIndex = 11;
			this.btnRandom.Text = "Random";
			this.btnRandom.Click += new System.EventHandler(this.btnRandom_Click);
			// 
			// btnChksum
			// 
			this.btnChksum.Location = new System.Drawing.Point(109, 16);
			this.btnChksum.Name = "btnChksum";
			this.btnChksum.Size = new System.Drawing.Size(96, 24);
			this.btnChksum.TabIndex = 12;
			this.btnChksum.Text = "ChkSum";
			this.btnChksum.Click += new System.EventHandler(this.btnChksum_Click);
			// 
			// btnFulltest
			// 
			this.btnFulltest.Location = new System.Drawing.Point(109, 56);
			this.btnFulltest.Name = "btnFulltest";
			this.btnFulltest.Size = new System.Drawing.Size(96, 24);
			this.btnFulltest.TabIndex = 14;
			this.btnFulltest.Text = "Full Test";
			this.btnFulltest.Click += new System.EventHandler(this.btnFulltest_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(173, 152);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(96, 24);
			this.btnCancel.TabIndex = 15;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// Form1
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(314, 200);
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

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}
	
		protected override void OnLoad(EventArgs e)
		{
			// TODO:  Add Form1.OnLoad implementation
			base.OnLoad (e);
		}
		/// <summary>
		/// btnOK_Click
		///		event occurs upen the btton pressed. 
		///		check the fields and add the entered data into the listview. 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
	
		private void btnRandom_Click(object sender, System.EventArgs e)
		{
			RandomUI rnd;

			rnd = new RandomUI();
			rnd.ShowDialog();
		}

		private void btnChksum_Click(object sender, System.EventArgs e)
		{
			CheckSumUI chk;

			chk = new CheckSumUI();
			chk.ShowDialog();
		}

		private void btnFulltest_Click(object sender, System.EventArgs e)
		{
			FullTest ftest;

			ftest = new FullTest();
			ftest.ShowDialog();
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			Application.Exit();
		}

	}
}
