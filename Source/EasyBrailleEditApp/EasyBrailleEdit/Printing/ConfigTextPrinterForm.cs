using System;
using System.Windows.Forms;

namespace EasyBrailleEdit.Printing
{
    public partial class ConfigTextPrinterForm : Form
    {
        public ConfigTextPrinterForm()
        {
            InitializeComponent();
        }

        private void ConfigTextPrinterForm_Load(object sender, EventArgs e)
        {
            configTextPrinterPanel.LoadSettings();		// 載入上次的設定。
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            configTextPrinterPanel.SaveSettings();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void configTextPrinterPanel_Load(object sender, EventArgs e)
        {

        }

    }
}
