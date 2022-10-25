using System;
using System.Net.Http.Handlers;
using System.Windows.Forms;
using Huanlin.Common.Http;

namespace EasyBrailleEdit
{
    public partial class UpdateProgressForm : Form
    {
        public UpdateProgressForm()
        {
            InitializeComponent();
        }

        private void UpdateProgressForm_Load(object sender, EventArgs e)
        {
            this.TopMost = true;

            txtMsg.Clear();
        }

        public void updator_FileUpdating(object sender, HttpUpdaterFileEventArgs args)
        {
            txtMsg.AppendText("正在更新 " + args.FileName);
            txtMsg.AppendText(String.Format(" (第 {0} 個，共 {1} 個) ....", args.Number, args.Total));
        }

        public void updator_FileUpdated(object sender, HttpUpdaterFileEventArgs args)
        {
            txtMsg.AppendText(" 完成!");
            txtMsg.AppendText(Environment.NewLine);
        }


        public void updator_DownloadProgressChanged(object sender, HttpProgressEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        public void AppendMessage(string msg)
        {
            txtMsg.AppendText(msg);
        }

    }
}
