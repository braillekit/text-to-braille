using System;
using System.Drawing.Printing;
using System.Windows.Forms;
using EasyBrailleEdit.Common;

namespace EasyBrailleEdit.Printing
{
    public partial class ConfigTextPrinterPanel : UserControl
    {
        private string m_PaperSourceName;
        private string m_PaperName;
        private Margins m_OddPageMargins;
        private Margins m_EvenPageMargins;
        private string m_TextFontName;
        private double m_TextFontSize;

        public ConfigTextPrinterPanel()
        {
            InitializeComponent();
        }

        public void LoadSettings()
        {
            var cfg = AppGlobals.Config.Printing;

            m_PaperSourceName = cfg.PrintTextPaperSourceName;
            m_PaperName = cfg.PrintTextPaperName;
            m_TextFontName = cfg.PrintTextFontName;
            m_TextFontSize = cfg.PrintTextFontSize;
            m_OddPageMargins = new Margins(cfg.PrintTextMarginLeft, cfg.PrintTextMarginRight, cfg.PrintTextMarginTop, cfg.PrintTextMarginBottom);
            m_EvenPageMargins = new Margins(cfg.PrintTextMarginLeft2, cfg.PrintTextMarginRight2, cfg.PrintTextMarginTop2, cfg.PrintTextMarginBottom2);

            cboPrinters.Items.Clear();
            foreach (string s in PrinterSettings.InstalledPrinters)
            {
                cboPrinters.Items.Add(s);
            }
            if (!String.IsNullOrEmpty(cfg.DefaultTextPrinter))
            {
                cboPrinters.SelectedIndex = cboPrinters.Items.IndexOf(cfg.DefaultTextPrinter);
            }
        }

        public void SaveSettings()
        {
            var cfg = AppGlobals.Config.Printing;

            cfg.DefaultTextPrinter = cboPrinters.Text;
            cfg.PrintTextPaperSourceName = m_PaperSourceName;
            cfg.PrintTextPaperName = m_PaperName;
            cfg.PrintTextMarginLeft = m_OddPageMargins.Left;
            cfg.PrintTextMarginTop = m_OddPageMargins.Top;
            cfg.PrintTextMarginRight = m_OddPageMargins.Right;
            cfg.PrintTextMarginBottom = m_OddPageMargins.Bottom;
            cfg.PrintTextMarginLeft2 = m_EvenPageMargins.Left;
            cfg.PrintTextMarginTop2 = m_EvenPageMargins.Top;
            cfg.PrintTextMarginRight2 = m_EvenPageMargins.Right;
            cfg.PrintTextMarginBottom2 = m_EvenPageMargins.Bottom;
            cfg.PrintTextFontName = m_TextFontName;
            cfg.PrintTextFontSize = m_TextFontSize;

            AppGlobals.Config.Save();
        }
        

        private void ConfigTextPrinterPanel_Load(object sender, EventArgs e)
        {
            LoadSettings();		// 載入上次的設定。
        }

        private void btnPageSetup_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(cboPrinters.Text))
            {
                MessageBox.Show("請先選擇印表機!");
                return;
            }
            TextPageSetupDialog dlg = new TextPageSetupDialog(cboPrinters.Text)
            {
                PaperSourceName = m_PaperSourceName,
                PaperName = m_PaperName,
                FontName = m_TextFontName,
                FontSize = m_TextFontSize,
                OddPageMargins = m_OddPageMargins,
                EvenPageMargins = m_EvenPageMargins
            };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                m_PaperSourceName = dlg.PaperSourceName;
                m_PaperName = dlg.PaperName;
                m_OddPageMargins = dlg.OddPageMargins;
                m_EvenPageMargins = dlg.EvenPageMargins;
                m_TextFontName = dlg.FontName;
                m_TextFontSize = dlg.FontSize;
            }			
        }
    }
}
