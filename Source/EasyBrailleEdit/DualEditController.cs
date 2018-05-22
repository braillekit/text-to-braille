using System;
using System.Drawing;
using System.Windows.Forms;
using BrailleToolkit;
using BrailleToolkit.Converters;
using EasyBrailleEdit.Common;
using Huanlin.Windows.Forms;
using DevAge.Drawing.VisualElements;

namespace EasyBrailleEdit
{
    /// <summary>
    /// 儲存格內容修改的情況。
    /// </summary>
    internal enum CellChangedType
	{
		None,       // 完全沒有變動。
		Text,       // 修改了明眼字。
		Phonetic,   // 只修改點字的注音碼。
		Braille     // 修改點字。
	};


	/// <summary>
	/// 雙視編輯工具類別。此類別將一些雙視編輯時會用到的共用函式抽離出來，以減少重覆的程式碼。
	/// 用到此類別的 forms: DualEditForm, DualEditTitleForm。
	/// </summary>
	internal partial class DualEditController
	{
		private const int FixedColumns = 1;
		private const int FixedRows = 1;
		private const float DefaultHeaderFontSize = 9.0f;
		private const float DefaultBrailleFontSize = 19.5f;
		private const float DefaultTextFontSize = 11.25f;
		private const float DefaultPhoneticFontSize = 8.0f;

		private BrailleDocument m_BrDoc;
		private SourceGrid.Grid brGrid;
		private bool m_IsInitialized = false;
		private ViewMode m_ViewMode;

		#region 供 Grid 使用的物件

		private SourceGrid.Cells.Views.Header m_HeaderView;
		private SourceGrid.Cells.Views.Header m_HeaderView2;    // for 每一頁的起始列，用以辨別新頁的開始。
		private SourceGrid.Cells.Views.Cell m_BrView;   // Grid cell view for 點字
        private SourceGrid.Cells.Views.Cell m_MingView; // Grid cell view for 明眼字（字型用 Arial Unicode）
        private SourceGrid.Cells.Views.Cell m_MingViewCJK;  // Grid cell view for 中日韓明眼字（字型用新細明體）
		private SourceGrid.Cells.Views.Cell m_PhonView; // Grid cell view for 注音符號
		private SourceGrid.Cells.Views.Cell m_PhonView2; // Grid cell view for 破音字的注音符號
		private SourceGrid.Cells.Views.Cell m_PhonView3; // Grid cell view for 容易判斷錯誤的破音字注音符號
		private Font m_PhonFont;  // Grid cell view for 注音符號
		private PopupMenuController m_MenuController;

		// 注意這裡拿掉了 ClickController

		#endregion

		public PopupMenuController MenuController
		{
			get { return m_MenuController; }
		}

		#region 事件

		public event EventHandler DataChanged;

		#endregion


		public DualEditController(BrailleDocument brDoc, SourceGrid.Grid grid) 
		{
			m_BrDoc = brDoc;
			brGrid = grid;
			m_ViewMode = ViewMode.All;
		}

		/// <summary>
		/// 初始化 Grid。
		/// </summary>
		/// <param name="popupMenuClickHandler">在 Grid 上點滑鼠右鍵出現 popup 選單時，使用者點擊其中某 menuitem 的事件處理常式。</param>
		public void InitializeGrid(SourceGrid.CellContextEventHandler popupMenuClickHandler)
		{
			if (m_IsInitialized)
				return;

			// 設定 grid 預設的欄寬與列高
			brGrid.DefaultWidth = 30;
			brGrid.DefaultHeight = 20;

			// 設定 grid 列數與行數。
			int maxCol = m_BrDoc.CellsPerLine;  // brDoc.LongestLine.Words.Count;
			brGrid.Redim(m_BrDoc.LineCount * 3 + FixedRows, maxCol + FixedColumns);

			// 設定欄寬最小限制，以免呼叫 AutoSizeView 時，欄寬被縮得太小。
			for (int i = 1; i < brGrid.ColumnsCount; i++)
			{
				brGrid.Columns[i].MinimalWidth = 24;
			}
			brGrid.Columns[0].MinimalWidth = 40;	// 第 0 欄要顯示列號，需要寬些.


			// 標題欄
			if (m_HeaderView == null)
			{
                m_HeaderView = new SourceGrid.Cells.Views.Header
                {
                    Font = new Font(brGrid.Font, FontStyle.Regular)
                };
            }

			if (m_HeaderView2 == null)
			{
                m_HeaderView2 = new SourceGrid.Cells.Views.RowHeader
                {
                    Background = new RowHeader
                    {
                        BackColor = Color.Blue
                    },
                    Font = m_HeaderView.Font
                };
            }

			CreateFixedArea();

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
                m_MingView = new SourceGrid.Cells.Views.Cell
                {
                    BackColor = Color.Snow,
                    Font = new Font("Arial Unicode MS", DefaultTextFontSize, FontStyle.Regular, GraphicsUnit.Point, 1)
                };
            }
            if (m_MingViewCJK == null)
            {
                m_MingViewCJK = new SourceGrid.Cells.Views.Cell();
                m_MingViewCJK.BackColor = Color.Snow;
                m_MingViewCJK.Font = new Font("PMingLiU", DefaultTextFontSize, FontStyle.Regular, GraphicsUnit.Point, 1);
                m_MingViewCJK.ElementText.Font = m_MingView.Font;
            }

			// view for 注音符號
			if (m_PhonFont == null)
			{
                m_PhonFont = new Font("PMingLiU", DefaultPhoneticFontSize, FontStyle.Regular, GraphicsUnit.Point, 1);
			}

			if (m_PhonView == null)
			{
                m_PhonView = new SourceGrid.Cells.Views.Cell
                {
                    BackColor = Color.YellowGreen,
                    Font = m_PhonFont,
                    TrimmingMode = SourceGrid.TrimmingMode.None
                };
            }

			// view for 破音字的注音符號
			if (m_PhonView2 == null)
			{
                m_PhonView2 = new SourceGrid.Cells.Views.Cell
                {
                    BackColor = Color.Yellow,
                    Font = m_PhonFont,
                    TrimmingMode = SourceGrid.TrimmingMode.None
                };
            }
			// view for 容易判斷錯誤的破音字注音符號
			if (m_PhonView3 == null)
			{
                m_PhonView3 = new SourceGrid.Cells.Views.Cell
                {
                    BackColor = Color.Red,
                    Font = m_PhonFont,
                    TrimmingMode = SourceGrid.TrimmingMode.None
                };
            }

			// 設置 controllers
			if (m_MenuController == null && popupMenuClickHandler != null)
			{
				m_MenuController = new PopupMenuController();
				m_MenuController.PopupMenuClick += popupMenuClickHandler;
			}

			m_IsInitialized = true;
		}


		/// <summary>
		/// 建立固定儲存格的內容，包括：標題列、標題行。
		/// </summary>
		private void CreateFixedArea()
		{
			brGrid.FixedColumns = FixedColumns;
			brGrid.FixedRows = FixedRows;

			brGrid[0, 0] = new SourceGrid.Cells.Header();
			brGrid[0, 0].View = m_HeaderView;
			brGrid[0, 0].Row.Height = 22;
			//brGrid[0, 0].Column.AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize;

			// column headers
			int cnt = 0;
			for (int col = FixedColumns; col < brGrid.ColumnsCount; col++)
			{
				cnt++;
				var hdr = new SourceGrid.Cells.ColumnHeader(cnt.ToString());
				//hdr.EnableResize = false;               
				brGrid[0, col] = hdr;
				brGrid[0, col].View = m_HeaderView;
			}

			// row headers
			cnt = 1;
			for (int row = FixedRows; row < brGrid.RowsCount; row += 3)
			{
				var hdr = new SourceGrid.Cells.RowHeader(cnt.ToString());
				brGrid[row, 0] = hdr;
				brGrid[row, 0].View = m_HeaderView;
				hdr.RowSpan = 3;	// 不可以在指定 hdr 物件之前設定 RowSpan, 否則會出錯!
				cnt++;
			}

			RefreshRowNumbers();
		}


		/// <summary>
		/// 重新填列號。
		/// </summary>
		/// <param name="brGrid"></param>
		public void RefreshRowNumbers() 
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

		public void FillGrid()
		{
			if (m_BrDoc.LineCount < 1)
			{
				return;
			}

			int cnt = 0;
			//StatusText = "正在準備顯示資料...";
			//StatusProgress = 0;
			brGrid.SuspendLayout();
			CursorHelper.ShowWaitCursor();
			try
			{
				int row = FixedRows;
				foreach (BrailleLine brLine in m_BrDoc.Lines)
				{
					FillRow(brLine, row, false);    // 填一列，先不要調整列高。

					row += 3;

					cnt++;
					//StatusProgress = cnt * 100 / brDoc.Lines.Count;
				}
			}
			finally
			{
				//StatusText = "重新調整儲存格大小...";
				ResizeCells();
				brGrid.ResumeLayout();
				//StatusProgress = 0;
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

					brGrid[row, col] = new SourceGrid.Cells.Cell(brFontText);
					brGrid[row, col].ColumnSpan = brFontText.Length;
					brGrid[row, col].View = m_BrView;
					brGrid[row, col].Tag = brWord;
					brGrid[row, col].AddController(m_MenuController);
					//brGrid[row, col].AddController(m_ClickController);

					// 處理明眼字
					brGrid[row + 1, col] = new SourceGrid.Cells.Cell(brWord.Text);
					brGrid[row + 1, col].ColumnSpan = brFontText.Length;
                    brGrid[row + 1, col].View = m_MingViewCJK;  // TODO: 確認音標字形可以正確顯示. 否則要分開判斷，音標符號改用 m_MingView
					brGrid[row + 1, col].Tag = brWord;
					brGrid[row + 1, col].AddController(m_MenuController);
					//brGrid[row + 1, col].AddController(m_ClickController);

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
		/// 調整所有儲存格大小。
		/// </summary>
		private void ResizeCells()
		{
			//brGrid.AutoSizeCells(); // 調整所有儲存格大小。
			// 奇怪! 這裡呼叫 AutoSizeView 會導致欄寬縮太小，有些標題文字看不到! 在 DuelEditForm 卻不會!
			//brGrid.Columns.AutoSizeView();      // 比 AutoSizeCells 快很多!!
			brGrid.Columns.AutoSizeColumn(0);   // 重新調整第 0 欄，以確保顯示列號的儲存格夠大。
		}


		/// <summary>
		/// 更新指定的點字方格。
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <param name="brWord"></param>
		internal void UpdateCell(int row, int col, BrailleWord brWord)
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
		///  更新 grid 中的某一列。此方法會自動視需要斷行，讓指定的列拆成兩列。
		/// </summary>
		/// <param name="row">Grid 列索引。</param>
		internal void ReformatRow(int row)
		{
			row = GetBrailleRowIndex(row);  // 修正列索引為點字列所在的索引。

			int lineIndex = GetBrailleLineIndex(row);
			int lineCnt = BrailleProcessor.GetInstance().FormatLine(m_BrDoc, lineIndex, null);
			if (lineCnt > 1)    // 有斷行?
			{
				// 換上新列
				RecreateRow(row);
				FillRow(m_BrDoc[lineIndex], row, true);

				// 插入新列
				GridInsertRowAt(row + 3);
				FillRow(m_BrDoc.Lines[lineIndex + 1], row + 3, true);

				// 重新填列號
				RefreshRowNumbers();
			}
			else
			{
				// 換上新列
				RecreateRow(row);
				FillRow(m_BrDoc[lineIndex], row, true);
			}
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

			if (m_ViewMode == ViewMode.BrailleOnly)
			{
				brGrid.Rows.HideRow(row + 1);
				brGrid.Rows.HideRow(row + 2);
			}
			else if (m_ViewMode == ViewMode.TextAndZhuyin)
			{
				brGrid.Rows.HideRow(row);
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


		#region 計算點字、列索引的相關函式

		/// <summary>
		/// 根據指定的 grid 列索引計算出該列的點字列的列索引。
		/// 由於每個點字在顯示時佔三列（點字、明眼字、注音碼），此方法可從 grid 的列索引推算點字列的列索引。
		/// 註：grid 的第 0 列是標題列。
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		internal int GetBrailleRowIndex(int row)
		{
			if (row % 3 == 0)
			{
				row = row - 2;
			}
			else if (row % 3 == 2)
			{
				row--;
			}
			return row;
		}

		/// <summary>
		/// 根據 grid 列索引計算出該列屬於點字文件的哪一列，即 BrailleDocument 的 Lines 集合索引。
		/// 註：grid 的第 0 列是標題列。
		/// </summary>
		/// <param name="row">Grid 列索引。</param>
		/// <returns></returns>
		internal int GetBrailleLineIndex(int row)
		{
			return (row - brGrid.FixedRows) / 3;
		}

		/// <summary>
		/// 根據 grid 行索引計算出該行屬於點字列的哪一個字，即 BrailleLine 的 Words 集合索引。
		/// </summary>
		/// <param name="row">Grid 列索引。</param>
		/// <param name="col">Grid 行索引。</param>
		/// <returns></returns>
		internal int GetBrailleWordIndex(int row, int col)
		{
			int wordIdx = 0;
			int i = brGrid.FixedColumns;

			// 由於每個點字可能有多方，即在 grid 中可能合併多行，因此必須考慮合併的情形。
			while (true)
			{
				i += brGrid[row, i].ColumnSpan;
				if (i > col)
					break;
				wordIdx++;
			}
			return wordIdx;
		}

		#endregion


	} // of DualEditUIHelper


	/// <summary>
	/// 當使用者在 Grid 儲存格上點右鍵時顯示的 popup menu 類別。
	/// </summary>
	public class PopupMenuController : SourceGrid.Cells.Controllers.ControllerBase
	{
		private ContextMenu m_Menu = new ContextMenu();
		private SourceGrid.CellContext m_CellContext;

        private event SourceGrid.CellContextEventHandler m_PopupMenuClick = null;

		public PopupMenuController()
		{
			string[] menuItemDefs = 
            {
                "修改這個字(&E)...;Edit;" + ((int)Shortcut.F4).ToString(),
                "-;",
                "插入一串文字(&I)...;InsertText;" + ((int)Shortcut.CtrlIns).ToString(),
                "插入一個字(&W)...;InsertWord;" + ((int)Shortcut.CtrlI).ToString(),
                "插入於行尾(&A)...;Append",
                "插入空方(&B);InsertBlank",
                "在上方插入一行(&L);InsertLine;" + ((int)Shortcut.CtrlShiftI).ToString(),
                "在下方插入一行(&M);AddLine;" + ((int)Shortcut.CtrlShiftA).ToString(),
                "-;",
                "刪除(&D);Delete;" + ((int)Shortcut.CtrlDel).ToString(),
                "倒退刪除(&K);Backspace",
                "刪除整行(&R);DeleteLine;" + ((int)Shortcut.CtrlE).ToString(),
                "段落重整(&F);FormatParagraph;" + ((int)Shortcut.CtrlShiftF).ToString()
            };

			MenuItem mi;
			char[] sep = { ';' };
			EventHandler clickHandler = new EventHandler(GridPopupMenuItem_Click);

			foreach (string s in menuItemDefs)
			{
				string[] def = s.Split(sep);
				mi = new MenuItem(def[0]);
				mi.Tag = def[1];
				if (!mi.Text.Equals("-"))
				{
					mi.Click += clickHandler;
				}
				if (def.Length > 2)
				{
					mi.Shortcut = (Shortcut)Convert.ToInt32(def[2]);
				}
				m_Menu.MenuItems.Add(mi);
			}
		}

		/// <summary>
		/// 根據 tag 字串值尋找對應的選單項目，並令其隱藏或顯示。
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="hide">是否要隱藏</param>
		public void HideMenuItem(string tag, bool hide)
		{
			foreach (MenuItem item in m_Menu.MenuItems)
			{
				if (tag.Equals((string)item.Tag, StringComparison.CurrentCultureIgnoreCase))
				{
					item.Visible = !hide;
				}
			}
		}

		public override void OnMouseUp(SourceGrid.CellContext sender, MouseEventArgs e)
		{
			base.OnMouseUp(sender, e);

			if (e.Button == MouseButtons.Right)
			{
				m_CellContext = sender;
				m_Menu.Show(sender.Grid, new Point(e.X, e.Y));
			}
		}

		private void GridPopupMenuItem_Click(object sender, EventArgs e)
		{
			MenuItem mi = (MenuItem)sender;
			if (mi != null)
			{
				SourceGrid.CellContextEventArgs args = new SourceGrid.CellContextEventArgs(m_CellContext);
				Command = mi.Tag.ToString();
				OnPopupMenuClick(args);
			}
		}

		protected void OnPopupMenuClick(SourceGrid.CellContextEventArgs args)
		{
            m_PopupMenuClick?.Invoke(this, args);
        }

		public event SourceGrid.CellContextEventHandler PopupMenuClick
		{
			add
			{
                m_PopupMenuClick += value;
			}
			remove
			{
                m_PopupMenuClick -= value;
			}
		}

        public string Command { get; private set; }
    }

}
