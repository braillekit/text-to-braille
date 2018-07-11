using System;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using EasyBrailleEdit.Common;
using EasyBrailleEdit.License;
using Huanlin.Windows.Forms;

namespace EasyBrailleEdit
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            string filename = Assembly.GetExecutingAssembly().Location;
            string fileVer = " v" + FileVersionInfo.GetVersionInfo(filename).FileVersion;
            lblVesion.Text = "版本號碼： " + fileVer;
            linkLabel1.Text = Constant.FacebookPageUrl;

            UpdateUI();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try 
            {
                Process.Start(linkLabel1.Text);
            }
            catch 
            {
                Process process = new Process();
                process.StartInfo.FileName = "iexplore.exe";
                process.StartInfo.Arguments = linkLabel1.Text;
                process.StartInfo.UseShellExecute = true;
                process.Start();
            }
        }

        private void UpdateUI()
        {
            if (AppGlobals.UserLicense.IsValid)
            {
                lblCustomerName.Text = AppGlobals.UserLicense.CustomerName;
                btnRegister.Text = "重新註冊";
            }
            else
            {
                lblCustomerName.Text = "(未授權)";
                btnRegister.Text = "註冊";
            }
        }

        private async void btnRegister_Click(object sender, EventArgs e)
        {
            var userLic = LicenseService.EnterLicenseData();
            if (userLic != null)
            {                
                bool isLicensed = await LicenseService.ValidateUserLicenseAsync(userLic);
                if (isLicensed && AppGlobals.UserLicense.IsValid)
                {
                    MsgBoxHelper.ShowInfo("註冊成功!");
                }
                else
                {
                    MsgBoxHelper.ShowError("註冊失敗：序號無效！");
                }
            }
            UpdateUI();
        }
    }
}