using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Huanlin.Braille;
using Huanlin.Helpers;

namespace BrlReformat
{
    public partial class MainForm : Form
    {
		const int CellsPerLine = 32;	// 一行幾方

		private BrailleProcessor m_Processer;
        private string m_FileName;

        public MainForm()
        {
            InitializeComponent();
        }

		private void MainForm_Load(object sender, EventArgs e)
		{
            // 只需設定一次，且必須最早進行的初始化動作：建立點字轉換器物件，並指定點字對應表物件。            
            string filePath = StrHelper.AppendSlash(Application.StartupPath);

            ChineseWordConverter chtCvt = new ChineseWordConverter(filePath + "ChineseBrailleTable.xml");
            EnglishWordConverter engCvt = new EnglishWordConverter(filePath + "EnglishBrailleTable.xml");

            m_Processer = new BrailleProcessor();
            m_Processer.AddWordConverter(chtCvt);
            m_Processer.AddWordConverter(engCvt);
		}

		private void btnOpenFile_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			if (dlg.ShowDialog() != DialogResult.OK)
				return;

			m_FileName = dlg.FileName;

			StreamReader sr = new StreamReader(m_FileName, Encoding.Default);
			txtSrc.Text = sr.ReadToEnd();
			sr.Close();

		}

		private void btnFormat_Click(object sender, EventArgs e)
		{
			BrailleDocument brDoc = new BrailleDocument(m_Processer, 32);
			brDoc.Load(txtSrc.Text);

			txtFormatted.Text = brDoc.ToString();
		}

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtSrc.Clear();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.InitialDirectory = @"D:\Doc\淑萍的文件\轉好的文件\";
            dlg.FileName = m_FileName;
			if (dlg.ShowDialog() != DialogResult.OK)
				return;

			string filename = dlg.FileName;
            Encoding enc = Encoding.GetEncoding("big5");
            StreamWriter sw = new StreamWriter(filename, false, enc);
            sw.Write(txtFormatted.Text);
            sw.Close();
        }


	}
}