using System;
using System.Drawing;
using System.Windows.Forms;
using BrailleToolkit;
using BrailleToolkit.Converters;
using BrailleToolkit.Helpers;
using BrailleToolkit.Tags;
using EasyBrailleEdit.Common;
using EasyBrailleEdit.DualEdit;
using EasyBrailleEdit.Forms;
using Huanlin.Common.Helpers;
using Huanlin.Windows.Forms;

namespace EasyBrailleEdit
{
    public partial class DualEditForm : Form, IBrailleGridForm
    {
        private BrailleGridController _controller;

        private DualEditFindForm m_FindForm;


        public BrailleDocument BrailleDoc
        {
            get
            {
                if (_controller != null && _controller.BrailleDoc != null)
                {
                    return _controller.BrailleDoc;
                }
                return null;
            }
        }

        string IBrailleGridForm.CurrentWordStatusText
        {
            get { return statusLabelCurrentWord.Text; }
            set { statusLabelCurrentWord.Text = value; }
        }

        string IBrailleGridForm.CurrentLineStatusText
        {
            get { return statusLabelCurrentLine.Text; }
            set { statusLabelCurrentLine.Text = value; }
        }


        string IBrailleGridForm.StatusText
        {
            get { return statMessage.Text; }
            set
            {
                statMessage.Text = value;
                statusStrip1.Refresh();
            }
        }
        string IBrailleGridForm.PageNumberText
        {
            get { return statPageInfo.Text; }
            set { statPageInfo.Text = value; }
        }

        public ViewMode GridViewMode
        {
            get => _controller.ViewMode;
            set
            {
                switch (value)
                {
                    case ViewMode.All:
                        miViewAll.Checked = true;
                        miViewBrailleOnly.Checked = false;
                        miViewTextZhuyin.Checked = false;
                        break;
                    case ViewMode.BrailleOnly:
                        miViewAll.Checked = false;
                        miViewBrailleOnly.Checked = true;
                        miViewTextZhuyin.Checked = false;
                        break;
                    case ViewMode.TextAndZhuyin:
                        miViewAll.Checked = false;
                        miViewBrailleOnly.Checked = false;
                        miViewTextZhuyin.Checked = true;
                        break;
                    default:
                        throw new Exception($"無效的 ViewMode: {value}");
                }
                _controller.ViewMode = value;
            }
        }

        /// <summary>
        /// 狀態列的進度表數值。
        /// </summary>
        public int StatusProgress
        {
            get { return statProgressBar.Value; }
            set { statProgressBar.Value = value; }
        }

        private DualEditForm()
        {
            InitializeComponent();
        }

        public DualEditForm(string brxFileName) : this()
        {
            _controller = new BrailleGridController(this, brGrid, brxFileName, forPageTitle: false);
        }

        public bool DebugMode { get; set; } = true;

        private void DualEditForm_Load(object sender, EventArgs e)
        {
            cboZoom.SelectedIndex = 2;  // 100%
            cboZoom.Width = 60;

            txtGotoPageNum.Width = 50;

            brGrid.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            brGrid.Selection.FocusRowEntered += GridSelection_FocusRowEntered;
            brGrid.Selection.FocusColumnEntered += GridSelection_FocusColumnEntered;
            brGrid.Selection.CellGotFocus += GridSelection_CellGotFocus;

            statusLabelCurrentWord.Text = String.Empty;
            statusLabelCurrentLine.Text = String.Empty;

            GridViewMode = ViewMode.All;

            // 若 FileName 不是 null，表示在建構元已經載入過檔案，相關的初始化動作就不要重覆做。
            if (String.IsNullOrEmpty(_controller.FileName))
            {
                _controller.InitializeGrid();
                _controller.FillGrid();
            }

            m_FindForm = new DualEditFindForm();
            m_FindForm.Owner = this;
            m_FindForm.DecidingStartPosition += FindForm_DecidingStartPosition;
            m_FindForm.TargetFound += FindForm_TargetFound;

            BringToFront();
            Activate();
        }

        private void GridSelection_FocusRowEntered(object sender, SourceGrid.RowEventArgs e)
        {
            _controller.GridSelection_FocusRowEntered(e);
        }
        private void GridSelection_FocusColumnEntered(object sender, SourceGrid.ColumnEventArgs e)
        {
            // not used yet.
        }

        private void GridSelection_CellGotFocus(SourceGrid.Selection.SelectionBase sender, SourceGrid.ChangeActivePositionEventArgs e)
        {
            _controller.GridSelection_CellGotFocus(e);
        }

        void FindForm_DecidingStartPosition(object sender, DualEditFindForm.DecideStartPositionEventArgs args)
        {
            // 協助尋找視窗決定要從哪一個位置開始尋找（從目前作用中的儲存格開始）
            int row = brGrid.Selection.ActivePosition.Row;
            int col = brGrid.Selection.ActivePosition.Column;
            if (row < brGrid.FixedRows || col < brGrid.FixedColumns)
            {
                args.LineIndex = 0;
                args.WordIndex = 0;
            }
            else
            {
                args.LineIndex = _controller.PositionMapper.GridRowToBrailleLineIndex(row);
                args.WordIndex = _controller.PositionMapper.CellPositionToWordIndex(row, col);
            }
        }

        void FindForm_TargetFound(object sender, DualEditFindForm.TargetFoundEventArgs args)
        {
            int row = _controller.PositionMapper.LineIndexToGridBrailleRow(args.LineIndex) + 1;
            int col = _controller.PositionMapper.WordIndexToGridColumn(args.LineIndex, args.WordIndex);
            SourceGrid.Position pos = new SourceGrid.Position(row, col);
            brGrid.Selection.Focus(pos, true);
            brGrid.Selection.SelectCell(pos, true);
        }


        /// <summary>
        /// 當注音碼的 ComboBox 下拉時觸發此事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PhoneticComboBox_DropDown(object sender, EventArgs e)
        {
            //DevAgeComboBox cbo = (DevAgeComboBox)sender;
            //SourceGrid.Grid grid = (SourceGrid.Grid)cbo.Parent;
            //SourceGrid.Cells.ICell cell = (SourceGrid.Cells.ICell) grid.GetCell(grid.Selection.ActivePosition);
            //BrailleWord brWord = (BrailleWord)cell.Tag;

            //cbo.Items.Clear();
            //foreach (string s in brWord.CandidatePhoneticCodes)
            //{
            //    cbo.Items.Add(s);
            //}


            //if (brWord.CandidatePhoneticCodes.Count > 1)    // 破音字？
            //{

            //}
            //else
            //{
            //    cbo.Items.Add(cbo.Text);
            //}

        }


        private void OpenFile()
        {
            _controller.DoOpenFile();
        }

        private void miFileOpen_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void miFileSaveAs_Click(object sender, EventArgs e)
        {
            _controller.DoSaveFileAs();
        }

        private void miFileSave_Click(object sender, EventArgs e)
        {
            _controller.DoSaveFile();
        }

        private void miFilePrint_Click(object sender, EventArgs e)
        {
            _controller.DoPrint();
        }


        private void cboZoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboZoom.SelectedIndex < 0)
                return;
            string s = cboZoom.Items[cboZoom.SelectedIndex].ToString();
            s = s.Substring(0, s.Length - 1);
            _controller.Zoom(Convert.ToInt32(s));
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Tag == null)
                return;

            string s = e.ClickedItem.Tag.ToString();

            switch (s)
            {
                case "Open":
                    OpenFile();
                    break;
                case "Save":
                    _controller.DoSaveFile();
                    break;
                case "Print":
                    _controller.DoPrint();
                    break;
                default:
                    break;
            }
        }


        private void miViewMode_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = sender as ToolStripMenuItem;
            switch (mi.Tag.ToString())
            {
                case "All":
                    _controller.ViewMode = ViewMode.All;
                    break;
                case "BrailleOnly":
                    _controller.ViewMode = ViewMode.BrailleOnly;
                    break;
                case "TextAndZhuyin":
                    _controller.ViewMode = ViewMode.TextAndZhuyin;
                    break;
            }
        }

        private void DualEditForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            _controller.Form_KeyPress(e);
        }

        private void DualEditForm_KeyDown(object sender, KeyEventArgs e)
        {
            _controller.Form_KeyDown(e);
        }

        private void EditDocProperties()
        {
            var form = new BrailleDocPropertiesForm();
            form.CellsPerLine = BrailleDoc.CellsPerLine;
            form.StartPageNumber = BrailleDoc.StartPageNumber;
            if (form.ShowDialog() == DialogResult.OK)
            {
                BrailleDoc.StartPageNumber = form.StartPageNumber;
                _controller.IsDirty = true;
            }
        }

        private void EditPageTitles()
        {
            var form = new DualEditTitleForm(BrailleDoc);

            if (form.ShowDialog() == DialogResult.OK)
            {
                BrailleDoc.PageTitles.Clear();
                BrailleDoc.PageTitles = form.Titles;
            }
        }

        private void FetchPageTitles()
        {
            int addedCount = BrailleDoc.FetchPageTitles();
            if (addedCount > 0)
            {
                _controller.InitializeGrid();
                _controller.FillGrid();

                MsgBoxHelper.ShowInfo($"已加入 {addedCount} 個頁標題。");
            }
            else
            {
                MsgBoxHelper.ShowInfo("目前的文件中沒有發現任何新的頁標題。");
            }
        }

        /// <summary>
        /// 到指定的列。
        /// <param name="lineNumber">列號</param>
        /// </summary>
        private void GotoLine(int lineNum)
        {
            if (lineNum > BrailleDoc.LineCount)
            {
                lineNum = BrailleDoc.LineCount;
            }
            SourceGrid.Position pos = new SourceGrid.Position((lineNum - 1) * 3 + 1, 1);
            brGrid.ShowCell(pos, false);
        }

        /// <summary>
        /// 到指定的頁。
        /// <param name="pageNum">頁號</param>
        /// </summary>
        private void GotoPage(int pageNum)
        {
            if (pageNum < 1)
            {
                pageNum = 1;
            }
            int lineNum = (pageNum - 1) * (AppGlobals.Config.Braille.LinesPerPage - 1) + 1;

            GotoLine(lineNum);
        }

        private void Goto()
        {
            DualEditGotoForm fm = new DualEditGotoForm();
            if (fm.ShowDialog() == DialogResult.OK)
            {
                if (fm.IsGotoLine)
                {
                    GotoLine(fm.Position);
                }
                else
                {
                    GotoPage(fm.Position);
                }
            }
        }

        private void Find()
        {
            m_FindForm.Document = BrailleDoc;

            if (m_FindForm.Visible)
            {
                m_FindForm.BringToFront();
            }
            else
            {
                m_FindForm.Show();
            }
        }

        private void FindNext()
        {
            if (!m_FindForm.FindNext())
            {
                MsgBoxHelper.ShowInfo("已搜尋至文件結尾。");
            }
        }

        private void miEdit_Click(object sender, EventArgs e)
        {
            string s = (string)(sender as ToolStripMenuItem).Tag;
            switch (s)
            {
                case "DocProperties":
                    EditDocProperties();
                    break;
                case "PageTitles":
                    EditPageTitles();
                    break;
                case "FetchPageTitles":
                    FetchPageTitles();
                    break;
                case "Goto":
                    Goto();
                    break;
                case "Find":
                    Find();
                    break;
                case "FindNext":
                    FindNext();
                    break;
            }
        }

        private void btnGotoPage_Click(object sender, EventArgs e)
        {
            try
            {
                int pageNum = Convert.ToInt32(txtGotoPageNum.Text);
                GotoPage(pageNum);
            }
            catch
            {
                MsgBoxHelper.ShowError("頁號必需輸入整數!");
            }
        }

        private void DualEditForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (m_FindForm != null)
            {
                m_FindForm.DecidingStartPosition -= new DualEditFindForm.DecideStartPositionEvent(FindForm_DecidingStartPosition);
                m_FindForm.TargetFound -= new DualEditFindForm.TargetFoundEvent(FindForm_TargetFound);
                m_FindForm.Dispose();
            }
        }

        private void DualEditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = false;

            if (_controller.IsDirty)   //  || this.IsNoName()  沒取檔名時不用問
            {
                string s = "點字資料尚未儲存，是否儲存？" + Environment.NewLine + Environment.NewLine +
                    "[是]　 = 儲存並關閉此視窗。" + Environment.NewLine +
                    "[否]　 = 不儲存並關閉此視窗。" + Environment.NewLine +
                    "[取消] = 取消關閉視窗的動作，繼續編輯點字。";

                DialogResult result = MsgBoxHelper.ShowYesNoCancel(s);
                if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                if (result == DialogResult.Yes)
                {
                    if (!_controller.DoSaveFile())
                    {
                        e.Cancel = true;
                    }
                }
            }
        }

        private void miFileExportTxt_Click(object sender, EventArgs e)
        {
            _controller.DoExportTextFile();
        }

        private void miFileExportBrl_Click(object sender, EventArgs e)
        {
            _controller.DoExportBrailleFile();
        }

        private void miViewClick(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = sender as ToolStripMenuItem;
            switch (mi.Tag.ToString())
            {
                case "Refresh":
                    _controller.RefreshView();
                    break;
                case "Braille":
                    _controller.ViewBraille();
                    break;
                case "Text":
                    _controller.ViewText();
                    break;
            }
        }


        private void miToolsClick(object sender, EventArgs e)
        {
            string s = (string)(sender as ToolStripMenuItem).Tag;
            switch (s)
            {
                case "RemoveSharp":
                    int removedCount = BrailleDocumentHelper.RemoveSharpSymbolFromPageNumbers(BrailleDoc);
                    if (removedCount > 0)
                    {
                        MsgBoxHelper.ShowInfo($"完成。總共移除了 {removedCount} 個 # 號。請核對無誤後再存檔。");
                        _controller.IsDirty = true;
                        _controller.RefreshView();
                    }
                    else
                    {
                        MsgBoxHelper.ShowInfo($"沒有發現任何帶有 # 號的原書頁碼。");
                    }
                    break;
                case "RemoveUselessWords":
                    int emptyLinesCount;
                    int emptyTagsCount;
                    bool doRemove = false;
                    BrailleDocumentHelper.RemoveUselessWords(BrailleDoc, doRemove, out emptyLinesCount, out emptyTagsCount);
                    if (emptyLinesCount > 0 || emptyTagsCount > 0)
                    {
                        string msg = $"發現 {emptyLinesCount} 個空行，{emptyTagsCount} 個空標籤。要刪除它們嗎？";
                        if (MsgBoxHelper.ShowYesNo(msg) == DialogResult.Yes)
                        {
                            doRemove = true;
                            BrailleDocumentHelper.RemoveUselessWords(BrailleDoc, doRemove, out emptyLinesCount, out emptyTagsCount);
                            MsgBoxHelper.ShowInfo($"完成。總共移除了 {emptyLinesCount} 個空行，{emptyTagsCount} 個空標籤。請核對無誤後再存檔。");
                            _controller.IsDirty = true;
                            _controller.RefreshView();
                        }
                    }
                    else
                    {
                        MsgBoxHelper.ShowInfo("沒有發現任何多餘的空行或空標籤。");
                    }
                    break;
            }
        }
    }

}