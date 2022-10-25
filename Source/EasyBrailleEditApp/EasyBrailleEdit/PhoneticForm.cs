using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace EasyBrailleEdit
{
	public partial class PhoneticForm : Form
	{
		public PhoneticForm()
		{
			InitializeComponent();
		}

		private void PhoneticForm_Load(object sender, EventArgs e)
		{

		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		public string Phonetic
		{
			get
			{
				if (chkAddBracket.Checked)
				{
					return "[" + txtPhonetic.Text + "]";
				}
				return txtPhonetic.Text;				
			}
		}

		private void phoneticBtn_Click(object sender, EventArgs e)
		{
			ToolStripButton btn = sender as ToolStripButton;
			if (btn == null)
				return;

			txtPhonetic.AppendText(btn.Text);
		}
	}
}
