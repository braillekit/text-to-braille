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


	
	public partial class Form1 : Form
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

		public Form1()
		{
            InitializeComponent();
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
