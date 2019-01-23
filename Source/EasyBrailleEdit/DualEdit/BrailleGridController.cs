using BrailleToolkit;
using BrailleToolkit.Converters;
using EasyBrailleEdit.Common;
using Huanlin.Common.Helpers;
using Huanlin.Windows.Forms;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace EasyBrailleEdit.DualEdit
{
    internal partial class BrailleGridController
    {
        private const int FixedColumns = 1;
        private const int FixedRows = 1;
        private const float DefaultHeaderFontSize = 9.0f;
        private const float DefaultBrailleFontSize = 19.5f;
        private const float DefaultTextFontSize = 11.25f;
        private const float DefaultPhoneticFontSize = 8.0f;

        private IBrailleGridForm _form;
        private BrailleDocument _doc;
        private SourceGrid.Grid _grid;
        private BrailleGridPositionMapper _positionMapper;

        private string _fileName;
        private bool _isDirty;   // 檔案內容是否被修改過


        private BrailleGridDebugger _debugger;
        private ViewMode m_ViewMode = ViewMode.All;

        private SemaphoreSlim semaphore = new SemaphoreSlim(1, 1); // 用來避免多執行緒重入同一區塊或同時存取同一份資源

        #region 供 Grid 使用的物件

        private SourceGrid.Cells.Views.Header m_HeaderView;
        private SourceGrid.Cells.Views.Header m_HeaderView2;    // for 每一頁的起始列，用以辨別新頁的開始。
        private SourceGrid.Cells.Views.Cell m_BrView;   // Grid cell view for 點字
        private SourceGrid.Cells.Views.Cell m_MingView;     // Grid cell view for 明眼字（字型用 Arial Unicode）
        private SourceGrid.Cells.Views.Cell m_MingViewCJK;  // Grid cell view for 中日韓明眼字（字型用新細明體）
        private SourceGrid.Cells.Views.Cell m_PhonView; // Grid cell view for 注音符號
        private SourceGrid.Cells.Views.Cell m_PhonView2; // Grid cell view for 破音字的注音符號
        private SourceGrid.Cells.Views.Cell m_PhonView3; // Grid cell view for 容易判斷錯誤的破音字注音符號
        private Font m_MingFont;	// Grid cell 字型 for 一般明眼字
        private Font m_MingFontCJK; // Grid cell 字型 for 中日韓明眼字
        private Font m_PhonFont;    // Grid cell 字型 for 注音符號
        private GridPopupMenuController m_MenuController;
        private CellClickEvent m_ClickController;

        #endregion

        #region 屬性

        public BrailleDocument BrailleDoc
        {
            get => _doc;
            private set
            {
                if (_doc != value)
                {
                    if (_doc != null)
                    {
                        _doc.Clear();
                    }
                    _doc = value;
                    PositionMapper.BrailleDoc = _doc;

                    IsDirty = false;
                    OnBrailleDocPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 每當 BrailleDoc 屬性改變時要做的事。
        /// </summary>
        private void OnBrailleDocPropertyChanged()
        {
            InitializeGrid();
            FillGrid(_doc);

            // 注意：不可在這裡 reset undo buffer，因為執行 undo 時會改變 BrailleDoc 屬性。
        }

        public SourceGrid.Grid Grid { get => _grid; }

        public BrailleGridPositionMapper PositionMapper
        {
            get
            {
                if (_positionMapper == null)
                {
                    _positionMapper = new BrailleGridPositionMapper(_doc, _grid);
                }
                return _positionMapper;
            }
        }

        public bool IsUsedForPageTitle { get; }

        public string FileName
        {
            get { return _fileName; }
            set
            {
                // 如果是暫存的輸出檔名，則視為尚未存檔。
                if (value.IndexOf(Constant.Files.CvtOutputTempFileName, StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    _isDirty = true;
                }
                _fileName = value;
                UpdateWindowCaption();
            }
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;
                    UpdateWindowCaption();
                }
            }
        }

        /// <summary>
        /// 是否正忙著更新內部點字物件或更新 UI。
        /// 如果是的話，就不接受鍵盤操作，以免產生奇怪的狀況：BrailleLine 裡面找不到儲存格所關聯的 BrailleWord 物件。
        /// </summary>
        public bool IsBusy { get; set; }

        public GridPopupMenuController MenuController
        {
            get { return m_MenuController; }
        }


        public UndoRedoManager UndoRedo { get; }

        public BrailleGridDebugger Debugger
        {
            get
            {
                if (_debugger == null)
                {
                    _debugger = new BrailleGridDebugger(this);
                }
                return _debugger;
            }
        }

        public ViewMode ViewMode
        {
            get { return m_ViewMode; }
            set
            {
                if (m_ViewMode != value)
                {
                    m_ViewMode = value;
                    if (m_ViewMode == ViewMode.BrailleOnly)
                    {
                        ViewBrailleOnly();
                    }
                    else if (m_ViewMode == ViewMode.TextAndZhuyin)
                    {
                        ViewTextAndZhuyin();
                    }
                    else
                    {
                        ViewAll();
                    }
                }
            }
        }

        #endregion


        private BrailleGridController()
        {
            UndoRedo = new UndoRedoManager(AppGlobals.Config.BrailleEditor.MaxUndoLevel);
        }

        public BrailleGridController(IBrailleGridForm form, SourceGrid.Grid grid, BrailleDocument doc, bool forPageTitle)
            : this()
        {
            _form = form;
            _doc = doc;
            _grid = grid;
            IsUsedForPageTitle = forPageTitle;
        }

        public BrailleGridController(IBrailleGridForm form, SourceGrid.Grid grid, string brxFileName, bool forPageTitle)
            : this()
        {
            _form = form;
            _grid = grid;
            _doc = LoadFile(brxFileName);
            IsUsedForPageTitle = forPageTitle;
        }

        public void InitializeGrid()
        {
            _grid.BackColor = Color.DarkGray;

            // 確保既有的欄和列都被清除
            _grid.Rows.Clear();
            _grid.Columns.Clear();

            // 設定 grid 預設的欄寬與列高
            _grid.DefaultWidth = 30;
            _grid.DefaultHeight = 20;

            // 設定 grid 列數與行數。
            int maxCol = _doc.CellsPerLine;  // brDoc.LongestLine.Words.Count;
            _grid.Redim(_doc.GetVisibleLineCount() * 3 + FixedRows, maxCol + FixedColumns);

            // 設定欄寬最小限制，以免呼叫 AutoSizeView 時，欄寬被縮得太小。
            for (int i = 1; i < _grid.ColumnsCount; i++)
            {
                _grid.Columns[i].MinimalWidth = 24;
            }
            _grid.Columns[0].MinimalWidth = 40;    // 第 0 欄要顯示列號，需要寬些.

            // 標題欄
            if (m_HeaderView == null)
            {
                m_HeaderView = new SourceGrid.Cells.Views.Header();
                m_HeaderView.Font = new Font(_grid.Font, FontStyle.Regular);
            }

            if (m_HeaderView2 == null)
            {
                m_HeaderView2 = new SourceGrid.Cells.Views.RowHeader();
                DevAge.Drawing.VisualElements.RowHeader backHeader = new DevAge.Drawing.VisualElements.RowHeader();
                backHeader.BackColor = Color.Blue;
                m_HeaderView2.Background = backHeader;
                m_HeaderView2.Font = m_HeaderView.Font;
            }

            CreateFixedArea();

            // Font objects

            if (m_PhonFont == null)
            {
                m_PhonFont = new Font("PMingLiU", DefaultPhoneticFontSize, FontStyle.Regular, GraphicsUnit.Point, 1);
            }

            if (m_MingFont == null)
            {
                m_MingFont = new Font("Arial Unicode MS", DefaultTextFontSize, FontStyle.Regular, GraphicsUnit.Point, 0);
                // Note: 原本為新細明體，可是為了顯示英文音標等特殊符號，必須使用 Arial Unicode MS 字型。
            }
            if (m_MingFontCJK == null)
            {
                m_MingFontCJK = new Font("PMingLiU", DefaultTextFontSize, FontStyle.Regular, GraphicsUnit.Point, 1);
            }

            // view for 點字
            if (m_BrView == null)
            {
                m_BrView = new SourceGrid.Cells.Views.Cell();
                m_BrView.BackColor = Color.Snow;
                m_BrView.Font = new Font("SimBraille", DefaultBrailleFontSize);
                m_BrView.TrimmingMode = SourceGrid.TrimmingMode.None;
            }

            // view for 明眼字
            if (m_MingView == null)
            {
                m_MingView = new SourceGrid.Cells.Views.Cell();
                m_MingView.BackColor = Color.Snow;
                m_MingView.Font = m_MingFont;
                m_MingView.ElementText.Font = m_MingFont;
            }

            if (m_MingViewCJK == null)
            {
                m_MingViewCJK = new SourceGrid.Cells.Views.Cell();
                m_MingViewCJK.BackColor = Color.Snow;
                m_MingViewCJK.Font = m_MingFontCJK;
                m_MingViewCJK.ElementText.Font = m_MingFontCJK;
            }

            // view for 注音符號
            if (m_PhonView == null)
            {
                m_PhonView = new SourceGrid.Cells.Views.Cell();
                m_PhonView.BackColor = Color.YellowGreen;
                m_PhonView.Font = m_PhonFont;
                m_PhonView.TrimmingMode = SourceGrid.TrimmingMode.None;
            }

            // view for 破音字的注音符號
            if (m_PhonView2 == null)
            {
                m_PhonView2 = new SourceGrid.Cells.Views.Cell();
                m_PhonView2.BackColor = Color.Yellow;
                m_PhonView2.Font = m_PhonFont;
                m_PhonView2.TrimmingMode = SourceGrid.TrimmingMode.None;
            }
            // view for 容易判斷錯誤的破音字注音符號
            if (m_PhonView3 == null)
            {
                m_PhonView3 = new SourceGrid.Cells.Views.Cell();
                m_PhonView3.BackColor = Color.Red;
                m_PhonView3.Font = m_PhonFont;
                m_PhonView3.TrimmingMode = SourceGrid.TrimmingMode.None;
            }

            // 設置 controllers
            if (m_MenuController == null)
            {
                m_MenuController = new GridPopupMenuController();
                m_MenuController.PopupMenuClick += GridMenu_Click;
            }

            if (m_ClickController == null)
            {
                m_ClickController = new CellClickEvent();
            }
        }

        /// <summary>
        /// 建立固定儲存格的內容，包括：標題列、標題行。
        /// </summary>
        private void CreateFixedArea()
        {
            _grid.FixedColumns = FixedColumns;
            _grid.FixedRows = FixedRows;

            _grid[0, 0] = new SourceGrid.Cells.Header();
            _grid[0, 0].View = m_HeaderView;
            _grid[0, 0].Row.Height = 22;
            //_grid[0, 0].Column.AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize;

            // column headers
            int cnt = 0;
            for (int col = FixedColumns; col < _grid.ColumnsCount; col++)
            {
                cnt++;
                var hdr = new SourceGrid.Cells.ColumnHeader(cnt.ToString());
                //hdr.EnableResize = false;               
                _grid[0, col] = hdr;
                _grid[0, col].View = m_HeaderView;
            }

            // row headers
            cnt = 1;
            for (int row = FixedRows; row < _grid.RowsCount; row += 3)
            {
                SourceGrid.Cells.RowHeader hdr = new SourceGrid.Cells.RowHeader(cnt.ToString());
                _grid[row, 0] = hdr;
                _grid[row, 0].View = m_HeaderView;
                hdr.RowSpan = 3;    // 不可以在指定 hdr 物件之前設定 RowSpan, 否則會出錯!
                cnt++;
            }

            RefreshRowNumbers();
        }

        /// <summary>
        /// 重新填列號。
        /// </summary>
        private void RefreshRowNumbers()
        {
            int rowNum = 1;
            int lineIdx = 0;
            int linesPerPage = AppGlobals.Config.Braille.LinesPerPage;

            if (AppGlobals.Config.Printing.PrintPageFoot)
            {
                linesPerPage--; // 頁碼佔一列，所以每頁實際的點字列數少一列。
            }

            for (int row = 1; row < _grid.RowsCount; row += 3)
            {
                var cell = _grid[row, 0];

                var brLine = BrailleDoc.Lines[lineIdx];
                var pageTitle = BrailleDoc.FindPageTitleByBeginLine(brLine);
                if (pageTitle != null)
                {
                    cell.Value = $"{rowNum} 標";
                }
                else
                {
                    cell.Value = $"{rowNum}";
                }

                if ((rowNum - 1) % linesPerPage == 0)
                {
                    cell.View = m_HeaderView2;
                }
                else
                {
                    cell.View = m_HeaderView;
                }

                rowNum++;
                lineIdx++;
            }
        }

        private void UpdateWindowCaption()
        {
            var aForm = _form as Form;
            if (IsNoName())
            {
                aForm.Text = "雙視編輯 - 未命名 (" + StrHelper.ExtractFileName(_fileName) + ")";
            }
            else
            {
                aForm.Text = "雙視編輯 - " + StrHelper.ExtractFileName(_fileName);
            }

            if (IsDirty)
            {
                aForm.Text += "*";
            }
        }

        /// <summary>
        /// 判斷是否尚未命名.
        /// </summary>
        /// <returns></returns>
        private bool IsNoName()
        {
            if (String.IsNullOrEmpty(_fileName))
                return true;

            string fname = StrHelper.ExtractFileName(_fileName);
            if (fname.Equals(Constant.Files.CvtOutputTempFileName, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 縮放顯示比例。
        /// </summary>
        /// <param name="ratio"></param>
        public void Zoom(int ratio)
        {
            if (ratio > 200 || ratio < 30)
            {
                MsgBoxHelper.ShowInfo("指定的縮放比例太小或太大: " + ratio.ToString() + "%");
                return;
            }

            double r = ratio / 100.0;
            float size = 0.0f;

            _form.StatusText = "正在調整顯示比例...";
            CursorHelper.ShowWaitCursor();
            try
            {
                // 標題字型
                size = (float)(DefaultHeaderFontSize * r);
                m_HeaderView.Font = new Font(_grid.Font.FontFamily, size);
                //_grid[0, 1].View.Font = new Font(_grid[0, 1].View.Font.FontFamily, size);

                m_HeaderView2.Font = m_HeaderView.Font;

                // 點字字型
                size = (float)(DefaultBrailleFontSize * r);
                m_BrView.Font = new Font(m_BrView.Font.FontFamily, size);
                //_grid[1, 1].View.Font = new Font(_grid[1, 1].View.Font.FontFamily, size);

                // 明眼字字型
                size = (float)(DefaultTextFontSize * r);
                m_MingView.Font = new Font(m_MingFont.FontFamily, size, m_MingFont.Style, m_MingFont.Unit);
                m_MingViewCJK.Font = new Font("PMingLiU", size, m_MingFontCJK.Style, m_MingFontCJK.Unit, 1);
                //_grid[2, 1].View.Font = new Font(_grid[2, 1].View.Font.FontFamily, size);

                // 注音符號字型
                size = (float)(DefaultPhoneticFontSize * r);
                m_PhonView.Font = new Font(m_PhonView.Font.FontFamily, size, m_PhonView.Font.Style, m_PhonView.Font.Unit, m_PhonView.Font.GdiCharSet);
                m_PhonView2.Font = m_PhonView.Font;
                m_PhonView3.Font = m_PhonView.Font;
                //_grid[3, 1].View.Font = new Font(_grid[3, 1].View.Font.FontFamily, size);

                _grid.Columns.AutoSizeView();
            }
            finally
            {
                CursorHelper.RestoreCursor();
                _form.StatusText = "";
            }
        }

        public void FillGrid()
        {
            FillGrid(BrailleDoc);
        }

        /// <summary>
        /// 將 BrailleDocument 文件內容填入 grid。
        /// </summary>
        /// <param name="brDoc">BrailleDocument 文件。</param>
        public void FillGrid(BrailleDocument brDoc)
        {
            if (brDoc.Lines.Count < 1)
            {
                return;
            }

            int cnt = 0;
            _form.StatusText = "正在刷新視窗內容...";
            _form.StatusProgress = 0;
            _grid.UseWaitCursor = true;
            CursorHelper.ShowWaitCursor();
            _grid.SuspendLayout();
            var busyForm = new BusyForm();
            busyForm.Message = "正在刷新視窗內容...";
            busyForm.Show();
            Application.DoEvents();
            try
            {
                int row = FixedRows;
                foreach (BrailleLine brLine in brDoc.Lines)
                {
                    if (brLine.CellCount < 1)
                    {
                        continue; // 有可能是空的列，例如 <表格> 起始標籤就單獨佔據一列。
                    }
                    FillRow(brLine, row, false);    // 填一列，先不要調整列高。

                    // 把沒有資料的儲存格填入空白字元。
                    //for (int x = col; x < maxWordCount; x++)
                    //{
                    //    _grid[row, x] = new SourceGrid.Cells.Cell(" ");
                    //    _grid[row, x].View = brView;
                    //    _grid[row, x].Editor = txtEditor;
                    //    _grid[row + 1, x] = new SourceGrid.Cells.Cell(" ");
                    //    _grid[row + 1, x].View = mingView;
                    //    _grid[row + 1, x].Editor = txtEditor;
                    //    _grid[row + 2, x] = new SourceGrid.Cells.Cell(" ");
                    //    _grid[row + 2, x].View = phonView;
                    //    _grid[row + 2, x].Editor = txtEditor;
                    //}

                    row += 3;

                    cnt++;
                    _form.StatusProgress = cnt * 100 / brDoc.Lines.Count;
                }
            }
            finally
            {
                busyForm.Close();

                _form.StatusText = "重新調整儲存格大小...";
                ResizeCells();

                _grid.ResumeLayout();
                _grid.UseWaitCursor = false;
                _form.StatusText = String.Empty;
                _form.StatusProgress = 0;

                // 焦點移至第一列的第一個儲存格，並且清除既有的選取區域。
                GridFocusCell(_grid.FixedRows, _grid.FixedColumns);

                CursorHelper.RestoreCursor();
            }
        }

        /// <summary>
        /// 調整所有儲存格大小。
        /// </summary>
        private void ResizeCells()
        {
            //_grid.AutoSizeCells(); // 調整所有儲存格大小，速度非常慢!!
            _grid.Columns.AutoSizeView();      // 比 AutoSizeCells 快十倍以上!
            _grid.Columns.AutoSizeColumn(0);   // 重新調整第 0 欄，以確保顯示列號的儲存格夠大。
        }

        /// <summary>
        /// 把一列點字填入指定的 grid 列（影響三列）。
        /// </summary>
        /// <param name="brLine">點字串列。</param>
        /// <param name="row">欲填入 grid 中的哪一列。</param>
        /// <param name="autoSize">填完之後，是否要自動重新調整儲存格大小。</param>
        private void FillRow(BrailleLine brLine, int row, bool autoSize)
        {
            string brFontText;
            int col = _grid.FixedColumns;

            // 確保列索引是點字所在的列。
            row = PositionMapper.GetBrailleRowIndex(row);

            _grid.SuspendLayout();
            try
            {
                foreach (BrailleWord brWord in brLine.Words)
                {
                    // 處理點字
                    try
                    {
                        if (brWord.IsContextTag)
                        {
                            continue; // 略過 context tags.
                        }
                        else
                        {
                            brFontText = BrailleFontConverter.ToString(brWord);
                        }
                    }
                    catch (Exception e)
                    {
                        MsgBoxHelper.ShowError(e.Message + "\r\n" +
                            "列:" + row.ToString() + ", 行: " + col.ToString());
                        brFontText = "";
                    }

                    if (String.IsNullOrEmpty(brFontText))
                    {
                        throw new Exception($"無法轉換成對應的點字字型: {brWord.Text}。CellList 元素數量為 {brWord.CellCount}。");
                    }

                    _grid[row, col] = new SourceGrid.Cells.Cell(brFontText);

                    // 若需要 column span，則必須先清除欲合併的 cell，否則在設定 ColumnSpan 屬性時會拋錯。
                    //for (int i = 1; i < brFontText.Length; i++)
                    //{
                    //    _grid[row + 0, col + i] = null;
                    //    _grid[row + 1, col + i] = null;
                    //    _grid[row + 2, col + i] = null;
                    //}
                    _grid[row, col].ColumnSpan = brFontText.Length;
                    _grid[row, col].View = m_BrView;
                    _grid[row, col].Tag = brWord;
                    _grid[row, col].AddController(m_MenuController);
                    _grid[row, col].AddController(m_ClickController);

                    // 處理明眼字
                    _grid[row + 1, col] = new SourceGrid.Cells.Cell(brWord.Text);
                    _grid[row + 1, col].ColumnSpan = brFontText.Length;
                    _grid[row + 1, col].View = m_MingViewCJK;  // TODO: 確認音標字形可以正確顯示. 否則要分開判斷，音標符號改用 m_MingView
                    _grid[row + 1, col].Tag = brWord;
                    _grid[row + 1, col].AddController(m_MenuController);
                    _grid[row + 1, col].AddController(m_ClickController);

                    // 處理注音碼
                    _grid[row + 2, col] = new SourceGrid.Cells.Cell(brWord.PhoneticCode);
                    _grid[row + 2, col].ColumnSpan = brFontText.Length;
                    if (brWord.IsPolyphonic)
                    {
                        if (AppGlobals.Config.Braille.ErrorProneWords.IndexOf(brWord.Text) >= 0)
                        {
                            // 容易判斷錯誤的破音字用顯眼的紅色標示。
                            _grid[row + 2, col].View = m_PhonView3;
                        }
                        else
                        {
                            // 一般破音字用黃色標示。
                            _grid[row + 2, col].View = m_PhonView2;
                        }
                    }
                    else
                    {
                        _grid[row + 2, col].View = m_PhonView;
                    }
                    _grid[row + 2, col].Tag = brWord;

                    col += brFontText.Length;
                }
            }
            finally
            {
                _grid.Rows.AutoSizeRow(row);
                _grid.Rows.AutoSizeRow(row + 1);
                _grid.Rows.AutoSizeRow(row + 2);

                _grid.ResumeLayout();
            }
        }


        /// <summary>
        /// 檢查儲存格位置是否有效。
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private bool CheckCellPosition(int row, int col)
        {
            if (row < 0 || col < 0)
            {
                if (_grid.RowsCount > _grid.FixedRows && _grid.ColumnsCount > _grid.FixedColumns)
                {
                    // Grid 有資料時，才告訴 user 要點選儲存格。
                    MsgBoxHelper.ShowInfo("請先點選儲存格!");
                }
                return false;
            }
            return true;
        }


        /// <summary>
        /// 設定某個儲存格為 active cell。
        /// </summary>
        /// <param name="pos">儲存格位置。</param>
        /// <param name="resetSelection">是否清除選取範圍。</param>
        /// <returns></returns>
        public bool GridFocusCell(SourceGrid.Position pos, bool resetSelection)
        {
            if (pos.Row >= _grid.RowsCount)
                return false;
            if (pos.Column >= _grid.ColumnsCount)
                return false;
            _grid.Selection.SelectCell(pos, true);
            return _grid.Selection.Focus(pos, resetSelection);
        }

        public void GridFocusCell(int row, int col)
        {
            var position = new SourceGrid.Position(row, col);
            GridFocusCell(position, true);
        }

        /// <summary>
        /// 選取/取消選取指定的列
        /// </summary>
        /// <param name="row">列索引。</param>
        /// <param name="select">是否選取。</param>
        public void GridSelectRow(int row, bool select)
        {
            row = PositionMapper.GetBrailleRowIndex(row);
            var range = new SourceGrid.Range(row, _grid.FixedColumns, row + 2, _grid.ColumnsCount);
            _grid.Selection.SelectRange(range, select);
        }

        public void GridSelectLeftWord(int row, int col)
        {
            if (row < 0 || col < 0)
            {
                return;
            }

            var currentWord = PositionMapper.GetBrailleWordFromGridCell(row, col);

            if (currentWord == null)
            {
                throw new Exception($"無法取得儲存格位置 ({row}, {col}) 所對應的點字物件!");
            }

            col--;
            while (col >= 0)
            {
                var leftWord = PositionMapper.GetBrailleWordFromGridCell(row, col);
                if (!ReferenceEquals(currentWord, leftWord))
                {
                    GridFocusCell(row, col);
                    return;
                }

                col--;
            }

            /* 另一種方法：
            int wordIdx = PositionMapper.GetBrailleWordIndex(row, col);

            if (wordIdx < 1)
            {
                return;
            }

            int lineIdx = PositionMapper.GetBrailleLineIndex(row);            
            var brLine = BrailleDoc.Lines[lineIdx];

            // 往左找到第一個非 context tag 的字
            wordIdx--;
            while (wordIdx >= 0 && brLine[wordIdx].IsContextTag)
            {
                wordIdx--;
            }

            int newCol = GetGridColumnIndex(lineIdx, wordIdx);
            GridFocusCell(row, newCol);
            */
        }

        /// <summary>
        /// 更新指定的點字方格。
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="brWord"></param>
        private void UpdateCell(int row, int col, BrailleWord brWord)
        {
            // 處理點字
            string brFontText = null;
            try
            {
                if (brWord.IsContextTag)
                {
                    brFontText = " ";
                }
                else
                {
                    brFontText = BrailleFontConverter.ToString(brWord);
                }
            }
            catch (Exception e)
            {
                MsgBoxHelper.ShowError(e.Message + "\r\n" +
                    "列:" + row.ToString() + ", 行: " + col.ToString());
                brFontText = "";
            }

            row = PositionMapper.GetBrailleRowIndex(row);  // 確保列索引為點字列。

            //若每個 cell 一方點字，就用以下迴圈填入點字
            //for (int i = 0; i < brFontText.Length; i++)
            //{
            //    _grid[row, col+i].Value = brFontText[i];
            //}
            _grid[row, col].Value = brFontText;
            _grid[row, col].Tag = brWord;

            _grid[row + 1, col].Value = brWord.Text;
            _grid[row + 1, col].Tag = brWord;

            _grid[row + 2, col].Value = brWord.PhoneticCode;
            _grid[row + 2, col].Tag = brWord;
        }

        /// <summary>
        ///  重新編排 grid 中的某一列。此方法會自動視需要斷行，讓指定的列拆成兩列。
        /// </summary>
        /// <param name="row">Grid 列索引。</param>
        /// <returns>傳回重新編排後的列數。如果大於 1，則代表此列經過重新編排之後有發生斷行。</returns>
        private int ReformatRow(SourceGrid.Grid grid, int row)
        {
            row = PositionMapper.GetBrailleRowIndex(row);  // 修正列索引為點字列所在的索引。

            int lineIndex = PositionMapper.GridRowToBrailleLineIndex(row);
            int lineCnt = BrailleDocumentFormatter.FormatLine(BrailleDoc, lineIndex, null);

            // 換上新列
            RecreateRow(row);
            FillRow(BrailleDoc[lineIndex], row, true);

            // 處理斷行所產生的其他 lines
            for (int i = 1; i < lineCnt; i++)
            {
                // 插入新列
                lineIndex++;
                row += 3;
                GridInsertRowAt(row);
                FillRow(BrailleDoc[lineIndex], row, autoSize: true);
            }

            if (lineCnt > 1)
            {
                // 重新填列號
                RefreshRowNumbers();
            }
            Application.DoEvents();

            return lineCnt;
        }

        /// <summary>
        /// 在指定的列索引新增一列（實際上是三列）。
        /// </summary>
        /// <param name="row"></param>
        private void GridInsertRowAt(int row)
        {
            row = PositionMapper.GetBrailleRowIndex(row);  // 確保列索引為點字列所在的索引。
            _grid.Rows.InsertRange(row, 3);

            // 建立列標題儲存格。
            int rowNum = row / 3 + 1;
            SourceGrid.Cells.Header hdr = new SourceGrid.Cells.Header(rowNum.ToString());
            _grid[row, 0] = hdr;
            _grid[row, 0].View = m_HeaderView;
            hdr.RowSpan = 3;
        }

        /// <summary>
        /// 將指定的列刪除，然後重新增加（插入）一列。
        /// </summary>
        /// <param name="row">列索引。</param>
        private void RecreateRow(int row)
        {
            row = PositionMapper.GetBrailleRowIndex(row);  // 修正列索引為點字列所在的索引。

            _grid.Rows.RemoveRange(row, 3);

            GridInsertRowAt(row);

            if (ViewMode == ViewMode.BrailleOnly)
            {
                _grid.Rows.HideRow(row + 1);
                _grid.Rows.HideRow(row + 2);
            }
            else if (ViewMode == ViewMode.TextAndZhuyin)
            {
                _grid.Rows.HideRow(row);
            }
        }

        public void GridEnsureAtLeastOneRow()
        {
            if (_grid.RowsCount == _grid.FixedRows)
            {
                GridInsertRowAt(_grid.FixedRows);
            }
        }

    }
}
