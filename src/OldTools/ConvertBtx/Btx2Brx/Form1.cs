using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Huanlin.Common.Helpers;
using Huanlin.Helpers;
using OldBrailleDocument = Huanlin.Braille.BrailleDocument;

namespace Btx2Brx
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            DoConvert();
        }

        void DoConvert()
        {

            if (String.IsNullOrWhiteSpace(txtBtxPath.Text))
            {
                MessageBox.Show("請指定 .btx 檔案所在的資料夾！");
                return;
            }

            if (!Directory.Exists(txtBtxPath.Text))
            {
                MessageBox.Show("指定的資料夾路徑不存在!");
                return;
            }

            if (MessageBox.Show("如果欲轉換的 .brx 檔案已經存在，將會被新的轉換結果覆蓋，是否繼續？", "確認", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }

            var btxFiles = Directory.GetFiles(txtBtxPath.Text, "*.btx");
            int count = 0;
            foreach (var filename in btxFiles)
            {
                string dstFileName = Path.ChangeExtension(filename, ".brx");

                try
                {
                    var brDoc = OldBrailleDocument.a(filename); // 這個混淆過的方法其實是 Deserialize(string filename)
                    BrailleDocumentConverter.SaveAsBrx(brDoc, dstFileName);

                    txtLog.Text += $"已將 '{filename}' 轉換成 '{StrHelper.ExtractFileName(dstFileName)}'。\r\n";
                    count++;
                }
                catch (Exception ex)
                {
                    txtLog.Text += $"轉換 '{filename}' 時發生錯誤: {ex.Message}\r\n";
                }
            }

            txtLog.Text += $"\r\n總共成功轉換了 {count} 個檔案。";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            var dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtBtxPath.Text = dlg.SelectedPath;
            }
        }
    }


    public static class BrailleDocumentConverter
    {
        public static void SaveAsBrx(OldBrailleDocument brDoc, string filename)
        {
            //var newBrDoc = Mapper.Map<BrailleToolkit.BrailleDocument>(brDoc);
            var newBrDoc = new BrailleToolkit.BrailleDocument();

            newBrDoc.CellsPerLine = brDoc.CellsPerLine;

            foreach (var brLine in brDoc.Lines)
            {
                var newLine = CreateLineFromOldVersion(brLine);
                newBrDoc.AddLine(newLine);
            }


            // page titles
            foreach (var title in brDoc.PageTitles)
            {
                var newTitle = new BrailleToolkit.BraillePageTitle(newBrDoc, title.BeginLineIndex);
                newTitle.TitleLine = CreateLineFromOldVersion(title.TitleLine);
                newBrDoc.PageTitles.Add(newTitle);
            }         

            newBrDoc.SaveBrailleFile(filename);
        }

        public static BrailleToolkit.BrailleLine CreateLineFromOldVersion(Huanlin.Braille.BrailleLine brLine)
        {
            var newLine = new BrailleToolkit.BrailleLine();
            foreach (var brWord in brLine.Words)
            {
                var newWord = new BrailleToolkit.BrailleWord(brWord.Text);
                newWord.PhoneticCode = brWord.PhoneticCode;
                newWord.PhoneticCodes.AddRange(brWord.PhoneticCodes);
                newWord.NoDigitCell = brWord.NoDigitCell;
                newWord.Language = (BrailleToolkit.BrailleLanguage)(int)brWord.Language;
                newWord.DontBreakLineHere = brWord.DontBreakLineHere;
                newWord.ActivePhoneticIndex = brWord.ActivePhoneticIndex;
                foreach (var cell in brWord.Cells)
                {
                    var newCell = BrailleToolkit.BrailleCell.GetInstance(cell.Value);
                    newWord.Cells.Add(newCell);
                }
                newLine.Words.Add(newWord);
            }
            return newLine;
        }
    }

}
