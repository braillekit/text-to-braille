using System;
using System.Drawing;
using System.Windows.Forms;
using BrailleToolkit;
using BrailleToolkit.Converters;
using EasyBrailleEdit.Common;
using EasyBrailleEdit.DualEdit;
using EasyBrailleEdit.Forms;
using Huanlin.Common.Helpers;
using Huanlin.Windows.Forms;

namespace EasyBrailleEdit
{
    public enum ViewMode { All, BrailleOnly, TextAndZhuyin };

	public partial class DualEditForm : Form
	{
		private const int FixedColumns = 1;
		private const int FixedRows = 1;
		private const float DefaultHeaderFontSize = 9.0f;
		private const float DefaultBrailleFontSize = 19.5f;
		private const float DefaultTextFontSize = 11.25f;
		private const float DefaultPhoneticFontSize = 8.0f;

		private bool m_IsInitialized = false;
        private string m_FileName;
		private bool m_IsDirty;   // 檔案內容是否被修改過

		private ViewMode m_ViewMode = ViewMode.All;

		private DualEditFindForm m_FindForm;

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
        private Font m_MingFontCJK;	// Grid cell 字型 for 中日韓明眼字
		private Font m_PhonFont;	// Grid cell 字型 for 注音符號
		private GridPopupMenuController m_MenuController;
		private CellClickEvent m_ClickController;

#endregion

		public DualEditForm(BrailleDocument brDoc)
			: base()
		{
			BrailleDoc = brDoc;

			InitializeComponent();
		}

		public DualEditForm(string brlFileName)
			: base()
		{
			InitializeComponent();

			InternalLoadFile(brlFileName);
		}

        #region 屬性

        public BrailleDocument BrailleDoc { get; private set; }

        public string FileName
		{
			get { return m_FileName; }
			set
			{
				// 如果是暫存的輸出檔名，則視為尚未存檔。
				if (value.IndexOf(Constant.Files.CvtOutputTempFileName, StringComparison.CurrentCultureIgnoreCase) >= 0)
				{
					m_IsDirty = true;
				}
				m_FileName = value;
				UpdateWindowCaption();
			}
		}

        public bool DebugMode { get; set; } = true;

        public StatusStrip StatusBar
		{
			get { return this.statusStrip1; }
		}

		public string StatusText
		{
			get { return statMessage.Text; }
			set
			{
				statMessage.Text = value;
				statusStrip1.Refresh();
			}
		}

		public string PageNumberText
		{
			get { return statPageInfo.Text; }
			set { statPageInfo.Text = value; }
		}

		/// <summary>
		/// 狀態列的進度表數值。
		/// </summary>
		public int StatusProgress
		{
			get { return statProgressBar.Value; }
			set
			{
				statProgressBar.Value = value;
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

		public bool IsDirty
		{
			get { return m_IsDirty; }
			set
			{
				if (m_IsDirty != value)
				{
					m_IsDirty = value;
					UpdateWindowCaption();
				}
			}
		}

		#endregion

		/// <summary>
		/// 判斷是否尚未命名.
		/// </summary>
		/// <returns></returns>
		private bool IsNoName()
		{
			if (String.IsNullOrEmpty(m_FileName))
				return true;

			string fname = StrHelper.ExtractFileName(m_FileName);
			if (fname.Equals(Constant.Files.CvtOutputTempFileName, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }

            return false;
		}

		private void UpdateWindowCaption()
		{
			if (IsNoName())
			{
                Text = "雙視編輯 - 未命名 (" + StrHelper.ExtractFileName(m_FileName) + ")" ;
			}
			else
			{
				Text = "雙視編輯 - " + StrHelper.ExtractFileName(m_FileName);
			}

			if (m_IsDirty)
			{
				Text = Text + "*";
			}
		}

		/// <summary>
		/// 縮放顯示比例。
		/// </summary>
		/// <param name="ratio"></param>
		private void Zoom(int ratio)
		{
			if (!m_IsInitialized)
				return;

			if (ratio > 200 || ratio < 30)
			{
				MsgBoxHelper.ShowInfo("指定的縮放比例太小或太大: " + ratio.ToString() + "%");
				return;
			}

			double r = ratio / 100.0;
			float size = 0.0f;

			StatusText = "正在調整顯示比例...";
			CursorHelper.ShowWaitCursor();
			try
			{
				// 標題字型
				size = (float)(DualEditForm.DefaultHeaderFontSize * r);
				m_HeaderView.Font = new Font(brGrid.Font.FontFamily, size);
				//brGrid[0, 1].View.Font = new Font(brGrid[0, 1].View.Font.FontFamily, size);

				m_HeaderView2.Font = m_HeaderView.Font;

				// 點字字型
				size = (float)(DualEditForm.DefaultBrailleFontSize * r);
				m_BrView.Font = new Font(m_BrView.Font.FontFamily, size);
				//brGrid[1, 1].View.Font = new Font(brGrid[1, 1].View.Font.FontFamily, size);

				// 明眼字字型
				size = (float)(DualEditForm.DefaultTextFontSize * r);
                m_MingView.Font = new Font(m_MingFont.FontFamily, size, m_MingFont.Style, m_MingFont.Unit);
                m_MingViewCJK.Font = new Font("PMingLiU", size, m_MingFontCJK.Style, m_MingFontCJK.Unit, 1);
				//brGrid[2, 1].View.Font = new Font(brGrid[2, 1].View.Font.FontFamily, size);

				// 注音符號字型
				size = (float)(DualEditForm.DefaultPhoneticFontSize * r);
				m_PhonView.Font = new Font(m_PhonView.Font.FontFamily, size, m_PhonView.Font.Style, m_PhonView.Font.Unit, m_PhonView.Font.GdiCharSet);
				m_PhonView2.Font = m_PhonView.Font;
				m_PhonView3.Font = m_PhonView.Font;
				//brGrid[3, 1].View.Font = new Font(brGrid[3, 1].View.Font.FontFamily, size);

				brGrid.Columns.AutoSizeView();
			}
			finally
			{
				CursorHelper.RestoreCursor();
				StatusText = "";
			}
		}

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

            m_ViewMode = ViewMode.All;
			miViewAll.Checked = true;

			// 若 FileName 不是 null，表示在建構元已經載入過檔案，相關的初始化動作就不要重覆做。
			if (String.IsNullOrEmpty(m_FileName))
			{
				InitializeGrid();
				FillGrid(BrailleDoc);
			}

			m_FindForm = new DualEditFindForm();
			m_FindForm.Owner = this;
			m_FindForm.DecidingStartPosition += FindForm_DecidingStartPosition;
			m_FindForm.TargetFound += FindForm_TargetFound;

			this.BringToFront();
			this.Activate();
		}

        private void GridSelection_FocusRowEntered(object sender, SourceGrid.RowEventArgs e)
        {
            var lineIdx = GetBrailleLineIndex(brGrid, e.Row);
            var brLine = BrailleDoc.Lines[lineIdx];
            statusLabelCurrentLine.Text = brLine.ToOriginalTextString(null);
        }
        private void GridSelection_FocusColumnEntered(object sender, SourceGrid.ColumnEventArgs e)
        {
            // not used yet.
        }

        private void GridSelection_CellGotFocus(SourceGrid.Selection.SelectionBase sender, SourceGrid.ChangeActivePositionEventArgs e)
        {
            var lineIdx = GetBrailleLineIndex(brGrid, e.NewFocusPosition.Row);
            var brWord = GetBrailleWordOfGridCell(brGrid, e.NewFocusPosition.Row, e.NewFocusPosition.Column);
            var brWordIdx = BrailleDoc.Lines[lineIdx].IndexOf(brWord);
            statusLabelCurrentWord.Text = $"{brWord.Text} (字索引:{brWordIdx}  列索引:{e.NewFocusPosition.Row}  行索引:{e.NewFocusPosition.Column})";
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
				args.LineIndex = GetBrailleLineIndex(brGrid, row);
				args.WordIndex = GetBrailleWordIndex(brGrid, row, col);
			}
		}

		void FindForm_TargetFound(object sender, DualEditFindForm.TargetFoundEventArgs args)
		{
			int row = GetGridRowIndex(args.LineIndex) + 1;
			int col = GetGridColumnIndex(args.LineIndex, args.WordIndex);
			SourceGrid.Position pos = new SourceGrid.Position(row, col);
			brGrid.Selection.Focus(pos, true);
			brGrid.Selection.SelectCell(pos, true);
		}

		private void InitializeGrid()
		{
			if (m_IsInitialized)
				return;

			// 設定 grid 預設的欄寬與列高
			brGrid.DefaultWidth = 30;
			brGrid.DefaultHeight = 20;

			// 設定 grid 列數與行數。
			int maxCol = BrailleDoc.CellsPerLine;  // brDoc.LongestLine.Words.Count;
			brGrid.Redim(BrailleDoc.GetVisibleLineCount() * 3 + FixedRows, maxCol + FixedColumns);

			// 設定欄寬最小限制，以免呼叫 AutoSizeView 時，欄寬被縮得太小。
			for (int i = 1; i < brGrid.ColumnsCount; i++)
			{
				brGrid.Columns[i].MinimalWidth = 24;
			}
			brGrid.Columns[0].MinimalWidth = 40;	// 第 0 欄要顯示列號，需要寬些.

			// 標題欄
			if (m_HeaderView == null)
			{
				m_HeaderView = new SourceGrid.Cells.Views.Header();
				m_HeaderView.Font = new Font(brGrid.Font, FontStyle.Regular);
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
                m_PhonFont = new Font("PMingLiU", DualEditForm.DefaultPhoneticFontSize, FontStyle.Regular, GraphicsUnit.Point, 1);
			}

			if (m_MingFont == null)
			{
                m_MingFont = new Font("Arial Unicode MS", DualEditForm.DefaultTextFontSize, FontStyle.Regular, GraphicsUnit.Point, 0);
				// Note: 原本為新細明體，可是為了顯示英文音標等特殊符號，必須使用 Arial Unicode MS 字型。
			}
            if (m_MingFontCJK == null)
            {
                m_MingFontCJK = new Font("PMingLiU", DualEditForm.DefaultTextFontSize, FontStyle.Regular, GraphicsUnit.Point, 1);
            }

			// view for 點字
			if (m_BrView == null)
			{
				m_BrView = new SourceGrid.Cells.Views.Cell();
				m_BrView.BackColor = Color.Snow;
				m_BrView.Font = new Font("SimBraille", DualEditForm.DefaultBrailleFontSize);
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
				m_ClickController = new CellClickEvent(this);
			}

			m_IsInitialized = true;
		}

		/// <summary>
		/// 建立固定儲存格的內容，包括：標題列、標題行。
		/// </summary>
		private void CreateFixedArea()
		{
			brGrid.FixedColumns = DualEditForm.FixedColumns;
			brGrid.FixedRows = DualEditForm.FixedRows;

			brGrid[0, 0] = new SourceGrid.Cells.Header();
			brGrid[0, 0].View = m_HeaderView;
			brGrid[0, 0].Row.Height = 22;
			//brGrid[0, 0].Column.AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize;

			// column headers
			int cnt = 0;
			for (int col = FixedColumns; col < brGrid.ColumnsCount; col++)
			{
				cnt++;
				SourceGrid.Cells.ColumnHeader hdr = new SourceGrid.Cells.ColumnHeader(cnt.ToString());
				//hdr.EnableResize = false;               
				brGrid[0, col] = hdr;
				brGrid[0, col].View = m_HeaderView;
			}

			// row headers
			cnt = 1;
			for (int row = FixedRows; row < brGrid.RowsCount; row += 3)
			{
				SourceGrid.Cells.RowHeader hdr = new SourceGrid.Cells.RowHeader(cnt.ToString());
				brGrid[row, 0] = hdr;
				brGrid[row, 0].View = m_HeaderView;
				hdr.RowSpan = 3;	// 不可以在指定 hdr 物件之前設定 RowSpan, 否則會出錯!
				cnt++;
			}

			RefreshRowNumbers();
		}

		/// <summary>
		/// 調整所有儲存格大小。
		/// </summary>
		private void ResizeCells()
		{
			//brGrid.AutoSizeCells(); // 調整所有儲存格大小，速度非常慢!!
			brGrid.Columns.AutoSizeView();      // 比 AutoSizeCells 快十倍以上!
			brGrid.Columns.AutoSizeColumn(0);   // 重新調整第 0 欄，以確保顯示列號的儲存格夠大。
		}

		/// <summary>
		///  重新編排 grid 中的某一列。此方法會自動視需要斷行，讓指定的列拆成兩列。
		/// </summary>
		/// <param name="row">Grid 列索引。</param>
        /// <returns>傳回重新編排後的列數。如果大於 1，則代表此列經過重新編排之後有發生斷行。</returns>
		private int ReformatRow(SourceGrid.Grid grid, int row)
		{
			row = GetBrailleRowIndex(row);  // 修正列索引為點字列所在的索引。

			int lineIndex = GetBrailleLineIndex(grid, row);
			int lineCnt = BrailleProcessor.GetInstance().FormatLine(BrailleDoc, lineIndex, null);
			if (lineCnt > 1)    // 有斷行?
			{
				// 換上新列
				RecreateRow(row);
				FillRow(BrailleDoc[lineIndex], row, true);

				// 插入新列
				GridInsertRowAt(row + 3);
				FillRow(BrailleDoc[lineIndex + 1], row + 3, true);

				// 重新填列號
				RefreshRowNumbers();
			}
			else
			{
				// 換上新列
				RecreateRow(row);
				FillRow(BrailleDoc[lineIndex], row, true);
			}
            return lineCnt;
		}

		/// <summary>
		/// 在指定的列索引新增一列（實際上是三列）。
		/// </summary>
		/// <param name="row"></param>
		private void GridInsertRowAt(int row)
		{
			row = GetBrailleRowIndex(row);  // 確保列索引為點字列所在的索引。
			brGrid.Rows.InsertRange(row, 3);

			// 建立列標題儲存格。
			int rowNum = row / 3 + 1;
			SourceGrid.Cells.Header hdr = new SourceGrid.Cells.Header(rowNum.ToString());
			brGrid[row, 0] = hdr;
			brGrid[row, 0].View = m_HeaderView;
			hdr.RowSpan = 3;
		}

		/// <summary>
		/// 將指定的列刪除，然後重新增加（插入）一列。
		/// </summary>
		/// <param name="row">列索引。</param>
		private void RecreateRow(int row)
		{
			row = GetBrailleRowIndex(row);  // 修正列索引為點字列所在的索引。

			brGrid.Rows.RemoveRange(row, 3);

			GridInsertRowAt(row);

			if (ViewMode == ViewMode.BrailleOnly)
			{
				brGrid.Rows.HideRow(row + 1);
				brGrid.Rows.HideRow(row + 2);
			}
			else if (ViewMode == ViewMode.TextAndZhuyin)
			{
				brGrid.Rows.HideRow(row);
			}
		}


		/// <summary>
		/// 將 BrailleDocument 文件內容填入 grid。
		/// </summary>
		/// <param name="brDoc">BrailleDocument 文件。</param>
		private void FillGrid(BrailleDocument brDoc)
		{
			if (brDoc.Lines.Count < 1)
			{
				return;
			}

			int cnt = 0;
			StatusText = "正在準備顯示資料...";
			StatusProgress = 0;
			CursorHelper.ShowWaitCursor();
			brGrid.SuspendLayout();
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
					//    brGrid[row, x] = new SourceGrid.Cells.Cell(" ");
					//    brGrid[row, x].View = brView;
					//    brGrid[row, x].Editor = txtEditor;
					//    brGrid[row + 1, x] = new SourceGrid.Cells.Cell(" ");
					//    brGrid[row + 1, x].View = mingView;
					//    brGrid[row + 1, x].Editor = txtEditor;
					//    brGrid[row + 2, x] = new SourceGrid.Cells.Cell(" ");
					//    brGrid[row + 2, x].View = phonView;
					//    brGrid[row + 2, x].Editor = txtEditor;
					//}

					row += 3;

					cnt++;
					StatusProgress = cnt * 100 / brDoc.Lines.Count;
				}
			}
			finally
			{
				StatusText = "重新調整儲存格大小...";
				ResizeCells();
				brGrid.ResumeLayout();
				StatusProgress = 0;
				CursorHelper.RestoreCursor();
			}
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
			int col = brGrid.FixedColumns;

			// 確保列索引是點字所在的列。
			row = GetBrailleRowIndex(row);

			brGrid.SuspendLayout();
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

					brGrid[row, col] = new SourceGrid.Cells.Cell(brFontText);
					brGrid[row, col].ColumnSpan = brFontText.Length;
					brGrid[row, col].View = m_BrView;
					brGrid[row, col].Tag = brWord;
					brGrid[row, col].AddController(m_MenuController);
					brGrid[row, col].AddController(m_ClickController);

					// 處理明眼字
					brGrid[row + 1, col] = new SourceGrid.Cells.Cell(brWord.Text);
					brGrid[row + 1, col].ColumnSpan = brFontText.Length;
                    brGrid[row + 1, col].View = m_MingViewCJK;  // TODO: 確認音標字形可以正確顯示. 否則要分開判斷，音標符號改用 m_MingView
					brGrid[row + 1, col].Tag = brWord;
					brGrid[row + 1, col].AddController(m_MenuController);
					brGrid[row + 1, col].AddController(m_ClickController);

					// 處理注音碼
					brGrid[row + 2, col] = new SourceGrid.Cells.Cell(brWord.PhoneticCode);
					brGrid[row + 2, col].ColumnSpan = brFontText.Length;
					if (brWord.IsPolyphonic)
					{
						if (AppGlobals.Config.Braille.ErrorProneWords.IndexOf(brWord.Text) >= 0)
						{
							// 容易判斷錯誤的破音字用顯眼的紅色標示。
							brGrid[row + 2, col].View = m_PhonView3;
						}
						else
						{
							// 一般破音字用黃色標示。
							brGrid[row + 2, col].View = m_PhonView2;
						}
					}
					else
					{
						brGrid[row + 2, col].View = m_PhonView;
					}
					brGrid[row + 2, col].Tag = brWord;

					col += brFontText.Length;
				}
			}
			finally
			{
				brGrid.Rows.AutoSizeRow(row);
				brGrid.Rows.AutoSizeRow(row + 1);
				brGrid.Rows.AutoSizeRow(row + 2);

				brGrid.ResumeLayout();
			}
		}

		/// <summary>
		/// Grid popup menu 點擊事件處裡常式。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void GridMenu_Click(object sender, SourceGrid.CellContextEventArgs e)
		{
			GridPopupMenuController menuCtrl = (GridPopupMenuController)sender;
			SourceGrid.CellContext cell = e.CellContext;
			SourceGrid.Grid grid = (SourceGrid.Grid)cell.Grid;
			int row = cell.Position.Row;
			int col = cell.Position.Column;

			switch (menuCtrl.Command)
			{
                case DualEditCommand.Names.BreakLine:
                    BreakLine(brGrid, row, col);
                    break;
                case DualEditCommand.Names.EditWord:
					EditWord(grid, row, col);
					break;
				case DualEditCommand.Names.InsertBlank:  // 插入空方                        
					InsertBlankCell(grid, row, col, 1);
					break;
				case DualEditCommand.Names.AppendWord:  // 在列尾插入空方
					AppendWord(grid, row, col);
					break;
                case DualEditCommand.Names.InsertWord:
                    InsertWord(grid, row, col);
                    break;
                case DualEditCommand.Names.InsertLine:  // 插入一列
					InsertLine(grid, row, col);
					break;
                case DualEditCommand.Names.AddLine:     // 在下方插入一列
                    AddLine(grid, row, col);
                    break;
                case DualEditCommand.Names.InsertText:
                    InsertText(grid, row, col);
                    break;
				case DualEditCommand.Names.DeleteWord:
					DeleteWord(grid, row, col);
					break;
				case DualEditCommand.Names.BackDeleteWord:
					BackspaceCell(grid, row, col);
					break;
				case DualEditCommand.Names.DeleteLine:
					DeleteLine(grid, row, col, true);
					break;
                case DualEditCommand.Names.FormatParagraph:
                    FormatParagraph(grid, row, col);
                    break;
                case DualEditCommand.Names.CopyToClipboard:
                    CopyToClipboard(grid);
                    break;
                case DualEditCommand.Names.PasteFromClipboard:
                    PasteFromClipboard(grid, row, col);
                    break;
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
				if (brGrid.RowsCount > brGrid.FixedRows && brGrid.ColumnsCount > brGrid.FixedColumns)
				{
					// Grid 有資料時，才告訴 user 要點選儲存格。
					MessageBox.Show("請先點選儲存格!");
				}
				return false;
			}
			return true;
		}

		/// <summary>
		/// 重新填列號。
		/// </summary>
		private void RefreshRowNumbers()
		{
			int rowNum = 1;
			int linesPerPage = AppGlobals.Config.Braille.LinesPerPage;

			if (AppGlobals.Config.Printing.PrintPageFoot)
			{
				linesPerPage--; // 頁碼佔一列，所以每頁實際的點字列數少一列。
			}

			for (int row = 1; row < brGrid.RowsCount; row += 3)
			{
				if ((rowNum - 1) % linesPerPage == 0)
				{
					brGrid[row, 0].View = m_HeaderView2;
					brGrid[row, 0].Value = rowNum;
				}
				else
				{
					brGrid[row, 0].View = m_HeaderView;
					brGrid[row, 0].Value = rowNum;
				}
				rowNum++;
			}
		}

		/// <summary>
		/// 選取/取消選取指定的列
		/// </summary>
		/// <param name="row">列索引。</param>
		/// <param name="select">是否選取。</param>
		private void GridSelectRow(int row, bool select)
		{
			row = GetBrailleRowIndex(row);
			SourceGrid.Range range = new SourceGrid.Range(row, brGrid.FixedColumns, row + 2, brGrid.ColumnsCount);
			brGrid.Selection.SelectRange(range, select);
		}

		/// <summary>
		/// 設定某個儲存格為 active cell。
		/// </summary>
		/// <param name="pos">儲存格位置。</param>
		/// <param name="resetSelection">是否清除選取範圍。</param>
		/// <returns></returns>
		private bool GridFocusCell(SourceGrid.Position pos, bool resetSelection)
		{
			if (pos.Row >= brGrid.RowsCount)
				return false;
			if (pos.Column >= brGrid.ColumnsCount)
				return false;
			brGrid.Selection.SelectCell(pos, true);
			return brGrid.Selection.Focus(pos, resetSelection);
		}

        private void GridFocusCell(int row, int col)
        {
            var position = new SourceGrid.Position(row, col);
            GridFocusCell(position, true);
        }

        private void GridSelectLeftWord(int row, int col)
        {
            var currentWord = GetBrailleWordOfGridCell(brGrid, row, col);

            if (currentWord == null)
            {
                throw new Exception($"無法取得儲存格位置 ({row}, {col}) 所對應的點字物件!");
            }

            col--;
            while (col >= 0)
            {
                var leftWord = GetBrailleWordOfGridCell(brGrid, row, col);
                if (!ReferenceEquals(currentWord, leftWord))
                {
                    GridFocusCell(row, col);
                    return;
                }

                col--;
            }

            /* 另一種方法：
            int wordIdx = GetBrailleWordIndex(brGrid, row, col);

            if (wordIdx < 1)
            {
                return;
            }

            int lineIdx = GetBrailleLineIndex(brGrid, row);            
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

			row = GetBrailleRowIndex(row);  // 確保列索引為點字列。

			//若每個 cell 一方點字，就用以下迴圈填入點字
			//for (int i = 0; i < brFontText.Length; i++)
			//{
			//    brGrid[row, col+i].Value = brFontText[i];
			//}
			brGrid[row, col].Value = brFontText;
			brGrid[row, col].Tag = brWord;

			brGrid[row + 1, col].Value = brWord.Text;
			brGrid[row + 1, col].Tag = brWord;

			brGrid[row + 2, col].Value = brWord.PhoneticCode;
			brGrid[row + 2, col].Tag = brWord;
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

		private void miFileOpen_Click(object sender, EventArgs e)
		{
			DoOpenFile();
		}

		private void miFileSaveAs_Click(object sender, EventArgs e)
		{
			DoSaveFileAs();
		}

		private void miFileSave_Click(object sender, EventArgs e)
		{
			DoSaveFile();
		}

		private void miFilePrint_Click(object sender, EventArgs e)
		{
			DoPrint();
		}


		private void cboZoom_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cboZoom.SelectedIndex < 0)
				return;
			string s = cboZoom.Items[cboZoom.SelectedIndex].ToString();
			s = s.Substring(0, s.Length - 1);
			Zoom(Convert.ToInt32(s));
		}

		private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Tag == null)
				return;

			string s = e.ClickedItem.Tag.ToString();

			switch (s)
			{
				case "Open":
					DoOpenFile();
					break;
				case "Save":
					DoSaveFile();
					break;
				case "Print":
					DoPrint();
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// 檢視模式：只顯示點字。
		/// </summary>
		private void ViewBrailleOnly()
		{
			MsgBoxHelper.ShowWarning("注意! 此為測試功能，若發現任何問題，請切回預設模式:\n" +
				"從主選單點選「檢視 > 模式 > 顯示全部」。");

			int row;

			brGrid.SuspendLayout();
			CursorHelper.ShowWaitCursor();
			try
			{
				for (row = 1; row < brGrid.RowsCount; row += 3)
				{
					brGrid.Rows.HideRow(row + 1);
					brGrid.Rows.HideRow(row + 2);
				}
				miViewBrailleOnly.Checked = true;
				miViewTextZhuyin.Checked = false;
				miViewAll.Checked = false;
			}
			finally
			{
				brGrid.ResumeLayout();
				ResizeCells();
				CursorHelper.RestoreCursor();
			}
		}

		/// <summary>
		/// 檢視模式：顯示明眼字及注音。
		/// </summary>
		private void ViewTextAndZhuyin()
		{
			int row;

			brGrid.SuspendLayout();
			CursorHelper.ShowWaitCursor();
			try
			{
				for (row = 1; row < brGrid.RowsCount; row += 3)
				{
					brGrid.Rows.HideRow(row);
				}
				miViewTextZhuyin.Checked = true;
				miViewBrailleOnly.Checked = false;
				miViewAll.Checked = false;
			}
			finally
			{
				brGrid.ResumeLayout();
				ResizeCells();
				CursorHelper.RestoreCursor();
			}
		}

		/// <summary>
		/// 檢視模式：顯示全部。
		/// </summary>
		private void ViewAll()
		{
			int row;

			brGrid.SuspendLayout();
			CursorHelper.ShowWaitCursor();
			try
			{
				for (row = 1; row < brGrid.RowsCount; row += 3)
				{
					brGrid.Rows.ShowRow(row);
					brGrid.Rows.ShowRow(row + 1);
					brGrid.Rows.ShowRow(row + 2);
					brGrid.Rows.AutoSizeRow(row);
					brGrid.Rows.AutoSizeRow(row + 1);
					brGrid.Rows.AutoSizeRow(row + 2);
				}
				miViewAll.Checked = true;
				miViewBrailleOnly.Checked = false;
				miViewTextZhuyin.Checked = false;
			}
			finally
			{
				brGrid.ResumeLayout();
				ResizeCells();
				CursorHelper.RestoreCursor();
			}
		}

		private void miViewMode_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem mi = sender as ToolStripMenuItem;
			switch (mi.Tag.ToString())
			{
				case "All":
					ViewMode = ViewMode.All;
					break;
				case "BrailleOnly":
					ViewMode = ViewMode.BrailleOnly;
					break;
				case "TextAndZhuyin":
					ViewMode = ViewMode.TextAndZhuyin;
					break;
			}
		}

		private void DualEditForm_KeyPress(object sender, KeyPressEventArgs e)
		{
            int row = brGrid.Selection.ActivePosition.Row;
            int col = brGrid.Selection.ActivePosition.Column;

            switch (e.KeyChar)
			{
				case ' ':   // 空白鍵：插入一個空方。
					InsertBlankCell(brGrid, row, col, 1);
					e.Handled = true;
					break;
				case '\r':
					BreakLine(brGrid, row, col);
					e.Handled = true;
					break;
			}
		}

		private void DualEditForm_KeyDown(object sender, KeyEventArgs e)
		{
			int row = brGrid.Selection.ActivePosition.Row;
			int col = brGrid.Selection.ActivePosition.Column;

			if (e.Modifiers == Keys.Control)
			{
				switch (e.KeyCode)
				{
					case Keys.I:        // Ctrl+I: 新增點字。
						InsertWord(brGrid, row, col);
						e.Handled = true;
						break;
					case Keys.Insert:    // Ctrl+Ins: 新增一串文字。
                        InsertText(brGrid, row, col);
						e.Handled = true;
						break;
					case Keys.Delete:   // Ctrl+Delete: 刪除一格點字。
						DeleteWord(brGrid, row, col);
						e.Handled = true;
						break;
					case Keys.E:        // Ctrl+E: 刪除一列。
						DeleteLine(brGrid, row, col, true);
						e.Handled = true;
						break;
                    case Keys.C:
                        CopyToClipboard(brGrid);
                        break;
                    case Keys.V:
                        PasteFromClipboard(brGrid, row, col);
                        break;
				}
			}
            else if (e.Modifiers == (Keys.Control | Keys.Shift))
            {
                switch (e.KeyCode)
                {
                    case Keys.A:
                        AddLine(brGrid, row, col);
                        e.Handled = true;
                        break;
                    case Keys.I:
                        InsertLine(brGrid, row, col);
                        e.Handled = true;
                        break;
                    case Keys.F:    // 段落重整
                        FormatParagraph(brGrid, row, col);
                        e.Handled = true;
                        break;
                }
            }
            else {
                switch (e.KeyCode)
                {
                    case Keys.F4:
                        EditWord(brGrid, row, col);
                        e.Handled = true;
                        break;
                    case Keys.Back:     // 倒退刪除
                        BackspaceCell(brGrid, row, col);
                        e.Handled = true;
                        break;
                    case Keys.Left:
                        GridSelectLeftWord(row, col);
                        e.Handled = true;
                        break;
                }
            }
		}


		private void EditPageTitles()
		{
			DualEditTitleForm fm = new DualEditTitleForm(BrailleDoc);
			fm.CellsPerLine = BrailleDoc.CellsPerLine;
			if (fm.ShowDialog() == DialogResult.OK)
			{
				BrailleDoc.PageTitles.Clear();
				BrailleDoc.PageTitles = fm.Titles;
			}
		}

		private void FetchPageTitles()
		{
			BrailleDoc.FetchPageTitles();
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
			int lineNum = (pageNum - 1) * (AppGlobals.Config.Braille.LinesPerPage - 1)+ 1;

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
				default:
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

            if (IsDirty)   //  || this.IsNoName()  沒取檔名時不用問
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
					if (!DoSaveFile())
					{
						e.Cancel = true;
					}
				}
			}
		}

        private void miFileExportTxt_Click(object sender, EventArgs e)
        {
            DoExportTextFile();
        }

        private void miFileExportBrl_Click(object sender, EventArgs e)
        {
            DoExportBrailleFile();
        }

        private void miViewClick(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = sender as ToolStripMenuItem;
            switch (mi.Tag.ToString())
            {
                case "Braille":
                    ViewBraille();
                    break;
                case "Text":
                    ViewText();
                    break;
            }
        }

        private void ViewBraille()
        {

        }

        private void ViewText()
        {
            var form = new ViewTextForm();
            form.Text = "檢視明眼字";
            form.Content = BrailleDoc.GetAllText();
            form.ShowDialog();
        }
    }


    public class CellClickEvent : SourceGrid.Cells.Controllers.ControllerBase
	{
		private DualEditForm m_Form;

		public CellClickEvent(DualEditForm form)
		{
			m_Form = form;
		}

		public override void OnClick(SourceGrid.CellContext sender, EventArgs e)
		{
			base.OnClick(sender, e);

			// 顯示目前焦點所在的儲存格屬於第幾頁。
			SourceGrid.Grid grid = (SourceGrid.Grid)sender.Grid;
			int row = sender.Position.Row;

			if (row < 1)
				return;

			int lineIdx = m_Form.GetBrailleLineIndex(grid, row);
			int linesPerPage = AppGlobals.Config.Braille.LinesPerPage;
			bool needPageFoot = AppGlobals.Config.Printing.PrintPageFoot;
			int currPage = AppGlobals.CalcCurrentPage(lineIdx, linesPerPage, needPageFoot) + 1;
			int totalPages = AppGlobals.CalcTotalPages(m_Form.BrailleDoc.Lines.Count, linesPerPage, needPageFoot);
			m_Form.PageNumberText = currPage.ToString() + "/" + totalPages.ToString();


			//if (m_Form.DebugMode)
			//{
			//    SourceGrid.Grid grid = (SourceGrid.Grid)sender.Grid;
			//    int row = sender.Position.Row;
			//    int col = sender.Position.Column;

			//    BrailleWord brWord = (BrailleWord)grid[row, col].Tag;
			//    string brScreenText = brWord.CellList.ToString();
			//    string brPrinterText = BrailleGlobals.FontConvert.ToString(brWord);

			//    m_Form.StatusBar.Items[0].Text = brWord.Text +
			//        "(" + brScreenText + ") " + // 顯示時的點字 16 進位字串
			//        "[" + brPrinterText + "]";  // 列印時的點字 16 進位字串
			//}
		}
	}

}