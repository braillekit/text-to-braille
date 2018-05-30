using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EasyBrailleEdit.Common;
using Huanlin.Windows.Forms;

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
            Debugger.ShouldEveryGridCellHasBrailleWord();
        }

        public void GridSelection_FocusRowEntered(SourceGrid.RowEventArgs e)
        {
            var lineIdx = _positionMapper.GridRowToBrailleLineIndex(e.Row);
            var brLine = BrailleDoc.Lines[lineIdx];
            _form.CurrentLineStatusText = brLine.ToOriginalTextString(null);
        }

        public void GridSelection_CellGotFocus(SourceGrid.ChangeActivePositionEventArgs e)
        {
            var lineIdx = _positionMapper.GridRowToBrailleLineIndex(e.NewFocusPosition.Row);
            var brWord = _positionMapper.GetBrailleWordFromGridCell(e.NewFocusPosition.Row, e.NewFocusPosition.Column);
            var brWordIdx = BrailleDoc.Lines[lineIdx].IndexOf(brWord);

            if (brWordIdx < 0)
            {
                MsgBoxHelper.ShowError("程式錯誤! 找不到目前選取之儲存格所對應的點字物件。請通知程式開發人員，謝謝。");
                return;
            }

            _form.CurrentWordStatusText = $"{brWord.Text} (文件索引:第 {lineIdx} 行，第 {brWordIdx} 字。儲存格索引:橫列={e.NewFocusPosition.Row}，直欄={e.NewFocusPosition.Column})";

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
            int row = _grid.Selection.ActivePosition.Row;
            int col = _grid.Selection.ActivePosition.Column;

            if (e.Modifiers == Keys.Control)
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
                    case Keys.C:
                        CopyToClipboard(_grid);
                        break;
                    case Keys.V:
                        PasteFromClipboard(_grid, row, col);
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
                }
            }
            else
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
