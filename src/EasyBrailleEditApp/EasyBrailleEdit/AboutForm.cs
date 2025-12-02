using System;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using EasyBrailleEdit.Common;
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
            linkLabel1.Text = Constant.ProjectUrl;

            UpdateUI();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c start {linkLabel1.Text}",
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void UpdateUI()
        {
            lblVersionLicense.Text = Constant.ProductVersionName;
        }

    }
}