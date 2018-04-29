using System.Windows.Forms;
using BrailleToolkit;
using EasyBrailleEdit.Common;
using Huanlin.Windows.Forms;

namespace EasyBrailleEdit
{
    /// <summary>
    /// DualEditForm 局部類別: 檔案處理相關 methods。
    /// </summary>
    partial class DualEditForm: Form
    {
        #region 檔案處理函式

        private void DoOpenFile()
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
                    default:
                        break;
                }
            }

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Filter = Constant.Files.FileNameFilter;
            dlg.FilterIndex = Constant.Files.FileNameFilterIndex;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                InternalLoadFile(dlg.FileName);
            }
        }

        private void InternalLoadFile(string filename)
        {
            CursorHelper.ShowWaitCursor();
            try
            {
                StatusText = "正在載入資料...";

                BrailleDocument newBrDoc = BrailleDocument.LoadBrailleFile(filename);

                if (m_BrDoc != null)
                {
                    m_BrDoc.Clear();
                }
                m_BrDoc = newBrDoc;

                FileName = filename;
                IsDirty = false;

                // 2009-6-23: 防錯處理，有的檔案因為程式的 bug 而存入空的 BrailleLine，在此處自動濾掉.
                for (int i = m_BrDoc.LineCount - 1; i >= 0; i--)
                {
                    // 把空的 BrailleLine 移除.
                    if (m_BrDoc.Lines[i].CellCount < 1)
                    {
                        m_BrDoc.RemoveLine(i);
                        IsDirty = true;
                    }
                }

                StatusText = "正在準備顯示資料...";
                brGrid.Rows.Clear();
                brGrid.Columns.Clear();
                m_IsInitialized = false;

                InitializeGrid();

                FillGrid(m_BrDoc);

                // 焦點移至第一列的第一個儲存格。
                SourceGrid.Position pos = new SourceGrid.Position(brGrid.FixedRows, brGrid.FixedColumns);
                GridFocusCell(pos, true);
            }
            finally
            {
                CursorHelper.RestoreCursor();
                StatusText = "";
            }
        }

        /// <summary>
        /// 存檔。
        /// </summary>
        /// <returns>True=儲存成功；False=儲存失敗或取消存檔動作。</returns>
        private bool DoSaveFile()
        {
            if (IsNoName())
            {
                return DoSaveFileAs();
            }

            InternalSaveFile(m_FileName);
            return true;
        }

        /// <summary>
        /// 另存新檔。
        /// </summary>
        /// <returns>True=儲存成功；False=儲存失敗或取消存檔動作。</returns>
        private bool DoSaveFileAs()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = Constant.Files.DefaultBrailleFileExt;
            dlg.Filter = Constant.Files.SaveAsFileNameFilter;
            dlg.FilterIndex = Constant.Files.SaveAsFileNameFilterIndex;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                InternalSaveFile(dlg.FileName);
                return true;
            }
            return false;
        }

        public bool DoExportTextFile()
        {
            var dlg = new SaveFileDialog();
            dlg.DefaultExt = ".txt";
            dlg.Filter = "文字檔 (*.txt)|*.txt";
            dlg.FilterIndex = Constant.Files.SaveAsFileNameFilterIndex;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                m_BrDoc.SaveTextFile(dlg.FileName);
                return true;
            }
            return false;
        }

        private void InternalSaveFile(string filename)
        {
            m_BrDoc.SaveBrailleFile(filename);

            FileName = filename;
            IsDirty = false;
            StatusText = "檔案儲存成功。";
        }

        private void DoPrint()
        {
            if (m_BrDoc.LineCount < 1)
            {
                MsgBoxHelper.ShowInfo("沒有資料可供列印!");
                return;
            }

            m_BrDoc.UpdateTitlesLineIndex();

            DualPrintDialog prnDlg = new DualPrintDialog(m_BrDoc);
            prnDlg.ShowDialog();
        }

        #endregion

    }
}
