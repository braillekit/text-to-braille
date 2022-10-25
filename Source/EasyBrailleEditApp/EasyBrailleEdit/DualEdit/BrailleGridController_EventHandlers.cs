using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EasyBrailleEdit.Common;
using Huanlin.Windows.Forms;
using Serilog;

namespace EasyBrailleEdit.DualEdit
{
    internal partial class BrailleGridController
    {
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
                    BreakLine(grid, row, col);
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
                case DualEditCommand.Names.InsertTable:
                    InsertTable(grid, row, col);
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
                case DualEditCommand.Names.SelectAll:
                    SelectAll(grid);
                    break;
                case DualEditCommand.Names.CopyToClipboard:
                    CopyToClipboard(grid);
                    break;
                case DualEditCommand.Names.CutToClipboard:
                    CutToClipboard(grid);
                    break;
                case DualEditCommand.Names.PasteFromClipboard:
                    PasteFromClipboard(grid, row, col);
                    break;
                case DualEditCommand.Names.PasteToEndOfLine:
                    PasteToEndOfLine(grid, row, col);
                    break;
                case DualEditCommand.Names.RemoveDigitSymbol:
                    RemoveDigitSymbol();
                    break;
            }
            Debugger.ShouldEveryGridCellHasBrailleWord();
        }


        public void Grid_MouseDoubleClick(SourceGrid.Grid grid, MouseEventArgs e)
        {
            if (grid == null || grid.MouseCellPosition.IsEmpty()) //grid.Selection.ActivePosition.IsEmpty())
                return;

            //EditWord(grid, grid.Selection.ActivePosition.Row, grid.Selection.ActivePosition.Column);
            EditWord(grid, grid.MouseCellPosition.Row, grid.MouseCellPosition.Column);
        }

        public void GridSelection_FocusRowEntered(SourceGrid.RowEventArgs e)
        {
            var lineIdx = _positionMapper.GridRowToBrailleLineIndex(e.Row);
            var brLine = BrailleDoc.Lines[lineIdx];
            _form.CurrentLineStatusText = brLine.ToOriginalTextString();

            var pageTitle = BrailleDoc.FindPageTitleByBeginLine(brLine);
            if (pageTitle != null)
            {
                _form.CurrentPageTitleStatusText = $"頁標：{pageTitle.TitleLine.ToString()}";
            }
            else
            {
                _form.CurrentPageTitleStatusText = String.Empty;
            }
        }

        public void GridSelection_CellGotFocus(SourceGrid.ChangeActivePositionEventArgs e)
        {
            var lineIdx = _positionMapper.GridRowToBrailleLineIndex(e.NewFocusPosition.Row);
            var brWord = _positionMapper.GetBrailleWordFromGridCell(e.NewFocusPosition.Row, e.NewFocusPosition.Column);

            if (brWord == null)
            {
                Log.Error($"目前點選的儲存格沒有關聯的 BrailleWord 物件! 檔名:{FileName}, Row={e.NewFocusPosition.Row}, Col={e.NewFocusPosition.Column}, lineIdx={lineIdx}, 該列內容: '{BrailleDoc.Lines[lineIdx].ToOriginalTextString()}'");
                e.Cancel = true;
                return;
            }

            var brWordIdx = BrailleDoc.Lines[lineIdx].IndexOf(brWord);

            if (brWordIdx < 0)
            {
                Log.Error($"找不到目前點選的儲存格所關聯的 BrailleWord 物件的索引位置! 檔名:{FileName}, Row={e.NewFocusPosition.Row}, Col={e.NewFocusPosition.Column}, lineIdx={lineIdx}, BrailleWord 內容: {brWord.ToString()}, 該列內容: '{BrailleDoc.Lines[lineIdx].ToOriginalTextString()}'");
                e.Cancel = true;
                return;
            }

            _form.CurrentWordStatusText = $"{brWord.Text}  |  共 {brWord.CellCount} 方點字  |  文件索引:第 {lineIdx} 行，第 {brWordIdx} 字  |  儲存格索引:橫列={e.NewFocusPosition.Row}，直欄={e.NewFocusPosition.Column})";

            // 顯示目前焦點所在的儲存格屬於第幾頁。
            int linesPerPage = AppGlobals.Config.Braille.LinesPerPage;
            bool needPageFoot = AppGlobals.Config.Printing.PrintPageFoot;
            int currPage = AppGlobals.CalcCurrentPage(lineIdx, linesPerPage, needPageFoot) + 1;
            int totalPages = AppGlobals.CalcTotalPages(BrailleDoc.Lines.Count, linesPerPage, needPageFoot);
            _form.PageNumberText = currPage.ToString() + "/" + totalPages.ToString();
        }

        public void Form_KeyPress(KeyPressEventArgs e)
        {
            int row = _grid.Selection.ActivePosition.Row;
            int col = _grid.Selection.ActivePosition.Column;

            switch (e.KeyChar)
            {
                case ' ':   // 空白鍵：插入一個空方。
                    InsertBlankCell(_grid, row, col, 1);
                    e.Handled = true;
                    break;
                case '\r':
                    BreakLine(_grid, row, col);
                    e.Handled = true;
                    break;
            }
        }

        public void Form_KeyDown(KeyEventArgs e)
        {
            if (IsBusy)
            {
                // 如果正忙著處理先前的編輯操作，就暫不接受鍵盤輸入，以免產生奇怪的狀況，
                // 例如按住 Ctrl+Del 不放，會出現 BrailleLine 裡面找不到儲存格所關聯的 BrailleWord 物件。
                return;
            }

            int row = _grid.Selection.ActivePosition.Row;
            int col = _grid.Selection.ActivePosition.Column;

            if (row < 0 || col < 0)
            {
                return;
            }

            IsBusy = true;
            try
            {
                if (e.Modifiers == Keys.None)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.F4:
                            EditWord(_grid, row, col);
                            e.Handled = true;
                            break;
                        case Keys.Back:     // 倒退刪除
                            BackspaceCell(_grid, row, col);
                            e.Handled = true;
                            break;
                        case Keys.Left:
                            GridSelectLeftWord(row, col);
                            e.Handled = true;
                            break;
                    }
                }
                else if (e.Modifiers == Keys.Control)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.I:        // Ctrl+I: 新增點字。
                            InsertWord(_grid, row, col);
                            e.Handled = true;
                            break;
                        case Keys.Insert:    // Ctrl+Ins: 新增一串文字。
                            InsertText(_grid, row, col);
                            e.Handled = true;
                            break;
                        case Keys.Delete:   // Ctrl+Delete: 刪除一格點字。
                            DeleteWord(_grid, row, col);
                            e.Handled = true;
                            break;
                        case Keys.E:        // Ctrl+E: 刪除一列。
                            DeleteLine(_grid, row, col, true);
                            e.Handled = true;
                            break;
                    }
                }
                else if (e.Modifiers == (Keys.Control | Keys.Shift))
                {
                    switch (e.KeyCode)
                    {
                        case Keys.A:
                            AddLine(_grid, row, col);
                            e.Handled = true;
                            break;
                        case Keys.I:
                            InsertLine(_grid, row, col);
                            e.Handled = true;
                            break;
                        case Keys.F:    // 段落重整
                            FormatParagraph(_grid, row, col);
                            e.Handled = true;
                            break;
                        case Keys.T:
                            InsertTable(_grid, row, col);
                            e.Handled = true;
                            break;
                        case Keys.V:
                            PasteToEndOfLine(_grid, row, col);
                            break;
                    }
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

    public class CellClickEvent : SourceGrid.Cells.Controllers.ControllerBase
    {
        public override void OnClick(SourceGrid.CellContext sender, EventArgs e)
        {
            base.OnClick(sender, e);

            /* 以下程式碼已經搬移至  GridSelection_CellGotFocus
             * 
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
            */
        }
    }

}
