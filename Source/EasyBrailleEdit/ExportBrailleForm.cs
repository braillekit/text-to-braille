using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrailleToolkit;
using EasyBrailleEdit.Common;
using Huanlin.Common.Helpers;
using Huanlin.Windows.Forms;

namespace EasyBrailleEdit
{
    public partial class ExportBrailleForm : Form
    {
        public BrailleDocument BrailleDoc { get; }

        public ExportBrailleForm(BrailleDocument brDoc)
        {
            InitializeComponent();

            BrailleDoc = brDoc;

            if (BrailleDoc.StartPageNumber < 1) // 舊版的雙視檔案沒有這個屬性，故手動初始化為 1。
            {
                BrailleDoc.StartPageNumber = 1;
            }
        }

        private void ExportBrailleForm_Load(object sender, EventArgs e)
        {
            lblLinesPerPage.Text = AppGlobals.Config.Braille.LinesPerPage.ToString();
            lblCellsPerLine.Text = BrailleDoc.CellsPerLine.ToString();

            var cfg = AppGlobals.Config.Printing;
            chkPrintPageFoot.Checked = cfg.PrintPageFoot;
            chkChangeStartPageNum.Checked = true;
            txtStartPageNumber.Text = BrailleDoc.StartPageNumber.ToString();

            txtBrailleFileName.TextBox.CharacterCasing = CharacterCasing.Lower;
            txtBrailleFileName.Button.Click += SelectBrailleFileNameButton_Click;
        }

        private void SelectBrailleFileNameButton_Click(object sender, EventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                DefaultExt = ".brl",
                CheckPathExists = true,
                Filter = "點字檔 (*.brl)|*.brl",
                FilterIndex = 1,
                Title = "指定要輸出的檔案名稱"
            };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtBrailleFileName.TextBox.Text = dlg.FileName;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DoExportBrailleFile();
        }

        private void DoExportBrailleFile()
        {
            int startPageNum = 1;
            if (chkChangeStartPageNum.Checked)
            {
                startPageNum = StrHelper.ToInteger(txtStartPageNumber.Text, 0);
                if (startPageNum < 1)
                {
                    MsgBoxHelper.ShowError("起始頁碼必須是大於 0 的數字!");
                    return;
                }
            }

            string filename = txtBrailleFileName.TextBox.Text;
            if (String.IsNullOrWhiteSpace(filename))
            {
                MsgBoxHelper.ShowError("尚未指定欲輸出的檔案名稱!");
                return;
            }

            var exporter = new BrailleDataExporter(
                BrailleDoc,
                AppGlobals.Config.Braille.LinesPerPage,
                startPageNum,
                chkPrintPageFoot.Checked);

            int endPageNum = exporter.SaveBrailleFile(filename);

            MsgBoxHelper.ShowInfo($"匯出檔案已完成。這次輸出的點字頁碼範圍是 {startPageNum}～{endPageNum}。");
        }

    }
}
