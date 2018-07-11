using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Configuration;
using System.Security.Cryptography;
using Huanlin.Common.Helpers;
using Huanlin.Windows.Forms;
using Huanlin.Http;
using Huanlin.Collections;
using Huanlin.Common.Cryptography;

namespace EasyBrailleEdit
{
    public partial class RegForm : Form
    {
        public string LicenseKey { get; set; }
        public string CustomerName { get; set; }

        public RegForm()
        {
            InitializeComponent();
        }

        private void RegForm_Load(object sender, EventArgs e)
        {
            
        }

        private void btnReg_Click(object sender, EventArgs e)
        {
            txtCustomerName.Text = txtCustomerName.Text.Trim();

            if (String.IsNullOrEmpty(txtCustomerName.Text))
            {
                MsgBoxHelper.ShowInfo("請輸入公司或使用者姓名!");
                txtCustomerName.Focus();
                return;
            }

            string licenseKey = txtLicenseKey.Text.Trim().Replace("-", "");
            if (String.IsNullOrEmpty(licenseKey))
            {
                MsgBoxHelper.ShowInfo("請輸入序號!");
            }
            LicenseKey = licenseKey;
            CustomerName = txtCustomerName.Text;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}