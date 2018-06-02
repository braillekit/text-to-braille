using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrailleToolkit;
using EasyBrailleEdit.Common;
using Huanlin.Windows.Forms;

namespace EasyBrailleEdit.DualEdit
{
    /// <summary>
    /// 局部類別: 檔案處理相關 methods。
    /// </summary>

    internal partial class BrailleGridController
    {
        public void DoOpenFile()
        {
            if (IsDirty)
            {
                switch (MsgBoxHelper.ShowYesNoCancel("目前的檔案尚未儲存，是否儲存？"))
                {
                    case DialogResult.Yes:
                        DoSaveFile();
                        break;
                    case DialogResult.Cancel:
                        return;
                }
            }

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Filter = Constant.Files.MainFileNameFilter;
            dlg.FilterIndex = 1;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                LoadFile(dlg.FileName);
            }
        }

        public BrailleDocument LoadFile(string filename)
        {
            CursorHelper.ShowWaitCursor();
            try
            {
                _form.StatusText = "正在載入資料...";

                BrailleDoc = BrailleDocument.LoadBrailleFile(filename);

                FileName = filename;

                UndoRedo.Reset();

                return BrailleDoc;
            }
            finally
            {
                CursorHelper.RestoreCursor();
                _form.StatusText = String.Empty;
            }
        }

        /// <summary>
        /// 存檔。
        /// </summary>
        /// <returns>True=儲存成功；False=儲存失敗或取消存檔動作。</returns>
        public bool DoSaveFile()
        {
            if (IsNoName())
            {
                return DoSaveFileAs();
            }

            InternalSaveFile(_fileName);
            return true;
        }

        /// <summary>
        /// 另存新檔。
        /// </summary>
        /// <returns>True=儲存成功；False=儲存失敗或取消存檔動作。</returns>
        public bool DoSaveFileAs()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = Constant.Files.DefaultMainBrailleFileExt;
            dlg.Filter = Constant.Files.SaveAsFileNameFilter;
            dlg.FilterIndex = 1;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                InternalSaveFile(dlg.FileName);
                return true;
            }
            return false;
        }

        public bool DoExportTextFile()
        {
            var dlg = new SaveFileDialog
            {
                DefaultExt = ".txt",
                Filter = "文字檔 (*.txt)|*.txt",
                FilterIndex = 1
            };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                BrailleDoc.SaveTextFile(dlg.FileName);
                return true;
            }
            return false;
        }

        public void DoExportBrailleFile()
        {
            var form = new ExportBrailleForm(BrailleDoc);
            form.ShowDialog();
        }

        private void InternalSaveFile(string filename)
        {
            BrailleDoc.SaveBrailleFile(filename);

            FileName = filename;
            IsDirty = false;

            _form.StatusText = "檔案儲存成功。";
        }

        public void DoPrint()
        {
            if (BrailleDoc.LineCount < 1)
            {
                MsgBoxHelper.ShowInfo("沒有資料可供列印!");
                return;
            }

            BrailleDoc.UpdateTitlesLineIndex();

            var prnDlg = new DualPrintDialog(BrailleDoc);
            prnDlg.ShowDialog();
        }
    }
}
