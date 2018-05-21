using System;
using System.Windows.Forms;
using BrailleToolkit;
using Huanlin.Windows.Forms;
using SourceGrid;

namespace EasyBrailleEdit
{
    /// <summary>
    /// DualEditForm 局部類別: 編輯功能相關 methods。
    /// </summary>
    partial class DualEditForm : Form
    {
        #region 計算點字、列索引的相關函式

        /// <summary>
        /// 根據指定的 grid 列索引計算出該列的點字列的列索引。
        /// 由於每個點字在顯示時佔三列（點字、明眼字、注音碼），此方法可從 grid 的列索引推算點字列的列索引。
        /// 註：grid 的第 0 列是標題列。
        /// </summary>
        /// <param name="gridRowIndex"></param>
        /// <returns></returns>
        internal int GetBrailleRowIndex(int gridRowIndex)
        {
            if (gridRowIndex % 3 == 0)
            {
                gridRowIndex -= 2;
            }
            else if (gridRowIndex % 3 == 2)
            {
                gridRowIndex--;
            }
            return gridRowIndex;
        }

        /// <summary>
        /// 根據 grid 列索引計算出該列屬於點字文件的哪一列，即 BrailleDocument 的 Lines 集合索引。
        /// 註：grid 的第 0 列是標題列。
        /// </summary>
        /// <param name="gridRowIndex">Grid 列索引。</param>
        /// <returns></returns>
        internal int GetBrailleLineIndex(int gridRowIndex)
        {
            return (gridRowIndex - brGrid.FixedRows) / 3;
        }

        /// <summary>
        /// 根據 grid 行索引計算出該行屬於點字列的哪一個字，即 BrailleLine 的 Words 集合索引。
        /// </summary>
        /// <param name="gridRowIndex">Grid 列索引。</param>
        /// <param name="gridColumnIndex">Grid 行索引。</param>
        /// <returns></returns>
        internal int GetBrailleWordIndex(int gridRowIndex, int gridColumnIndex)
        {
            int wordIdx = 0;
            int i = brGrid.FixedColumns;

            // 由於每個點字可能有多方，即在 grid 中可能合併多行，因此必須考慮合併的情形。
            while (true)
            {
                i += brGrid[gridRowIndex, i].ColumnSpan;
                if (i > gridColumnIndex)
                    break;
                wordIdx++;
            }
            return wordIdx;
        }

        /// <summary>
        /// 根據傳入的點字文件列索引取得對應的 Grid 點字列索引。
        /// </summary>
        /// <param name="lineIdx"></param>
        /// <returns></returns>
        internal int GetGridRowIndex(int lineIdx)
        {
            return (lineIdx * 3) + brGrid.FixedRows;
        }

        /// <summary>
        /// 根據傳入的點字文件列索引和字索引，取得對應的 Grid 欄索引。
        /// </summary>
        /// <param name="lineIdx"></param>
        /// <param name="wordIdx"></param>
        /// <returns></returns>
        internal int GetGridColumnIndex(int lineIdx, int wordIdx)
        {
            int textRowIdx = GetGridRowIndex(lineIdx) + 1;	// 明眼字的列索引
            int col = brGrid.FixedColumns;

            //TODO: 不可以這樣算!! 因為每一個合併儲存格不一定只有一個字元，可能有多個。
            //正確算法，應該是用點字的方數來計算，只要有幾方，儲存格的索引就是第幾個。

            while (col < brGrid.ColumnsCount && wordIdx > 0)
            {
                col += brGrid[textRowIdx, col].ColumnSpan;
                wordIdx--;
            }
            return col;
        }

        #endregion

        /// <summary>
        /// 修改儲存格。
        /// </summary>
        /// <param name="grid">來源 grid。</param>
        /// <param name="row">儲存格的列索引。</param>
        /// <param name="col">儲存格的行索引。</param>
        void EditCell(SourceGrid.Grid grid, int row, int col)
        {
            /* NOTE
             * 每當儲存格內容有變動時，需考慮以下情況： 
             * 
             * 1. 修改了明眼字。此情況的變化比較大，例如：把原本的英數字 "123"
             *    中間的 "2" 改成中文字。碰到這種情況，相鄰的 "3" 的點字也會受
             *    到影響，必須重新產生才行。但重新產生整份文件的點字又會造成其
             *    他已經修改過的部份得再修改一次，因此，碰到這種英數字改成中文
             *    字的情況，程式還是不自動修正相鄰的點字，而由使用者自行以修改
             *    點字（接著的第 3 種情況）的功能來修正此問題。
             * 2. 只修改點字的注音碼。此種情況可能使新的點字方數增加或減少，
             *    因此必須重新斷行，並將點字重新添入 Grid。若方數不變，或沒有
             *    超過每列最大方數，就不要重新斷行，以節省處理時間。
             * 3. 修改點字。這種情況只需比對新舊點字的方數，若有差異，則要重新
             *    斷行。
             * 
             * 第 1 種情況可能會包含後面兩種情況，第 2 種可能包含第 3 種情況，
             * 但是不會包含第 1 種情況。同理，第 3 種情況也不會包含第 1 或第 2
             * 種情況。
             */

            if (row < 0 || col < 0) // 防錯：如果不是有效的儲存格位置就直接返回。
                return;

            BrailleWord brWord = (BrailleWord)grid[row, col].Tag;

            EditCellForm form = new EditCellForm();
            form.Mode = EditCellMode.Edit;
            form.BrailleWord = brWord;
            if (form.ShowDialog() == DialogResult.OK)
            {
                // 判斷新的跟原本的點字，以得知是屬於哪一種修改情況。
                CellChangedType cellChgType = CellChangedType.None;
                if (!brWord.Text.Equals(form.BrailleWord.Text))
                {
                    cellChgType = CellChangedType.Text;
                }
                else if (!brWord.PhoneticCode.Equals(form.BrailleWord.PhoneticCode))
                {
                    cellChgType = CellChangedType.Phonetic;
                }
                else if (!brWord.CellList.Equals(form.BrailleWord.CellList))
                {
                    cellChgType = CellChangedType.Braille;
                }

                if (cellChgType != CellChangedType.None)
                {
                    brWord.Copy(form.BrailleWord);
                    GridCellChanged(row, col, brWord, cellChgType);
                    IsDirty = true;
                }
            }
        }

        /// <summary>
        /// 將指定的 BrailleWord 物件的資料填入指定的儲存格。
        /// </summary>
        /// <param name="row">指定之儲存格的列索引。</param>
        /// <param name="col">指定之儲存格的行索引。</param>
        /// <param name="brWord">欲填入儲存格的 BrailleWord 物件。</param>
        private void GridCellChanged(int row, int col, BrailleWord brWord, CellChangedType chgType)
        {
            if (chgType == CellChangedType.None)
                return;

            // 由於每個點字在顯示時佔三列（點字、明眼字、注音碼），因此先推算出點字的列索引。
            row = GetBrailleRowIndex(row);

            // Note: 不可重新轉點字，只重新斷行。

            // 判斷新設定的點字方數是否不同於儲存格原有的點字方數，若否，則必須重新斷字。
            if (brGrid[row, col].ColumnSpan == brWord.Cells.Count)
            {
                UpdateCell(row, col, brWord);
            }
            else
            {
                ReformatRow(row);
            }
        }

        /// <summary>
        /// 新增點字。
        /// </summary>
        /// <param name="grid">來源 grid。</param>
        /// <param name="row">儲存格的列索引。</param>
        /// <param name="col">儲存格的行索引。</param>
        private void InsertCell(SourceGrid.Grid grid, int row, int col)
        {
            if (!CheckCellPosition(row, col))
                return;

            EditCellForm form = new EditCellForm();
            form.Mode = EditCellMode.Insert;
            if (form.ShowDialog() == DialogResult.OK)
            {
                int wordIdx = GetBrailleWordIndex(row, col);
                int lineIdx = GetBrailleLineIndex(row);
                BrailleLine brLine = BrailleDoc.Lines[lineIdx];

                // 在第 wordIdx 個字之前插入新點字。
                brLine.Words.Insert(wordIdx, form.BrailleWord);
                IsDirty = true;

                // Update UI
                ReformatRow(row);
                int focusCol = GetGridColumnIndex(lineIdx, wordIdx);
                GridFocusCell(row, focusCol);
            }
        }

        /// <summary>
        /// 在行尾附加點字。
        /// </summary>
        private void AppendCell(SourceGrid.Grid grid, int row, int col)
        {
            if (!CheckCellPosition(row, col))
                return;

            EditCellForm form = new EditCellForm();
            form.Mode = EditCellMode.Insert;
            if (form.ShowDialog() == DialogResult.OK)
            {
                int lineIdx = GetBrailleLineIndex(row);
                BrailleLine brLine = BrailleDoc.Lines[lineIdx];

                // 在第 wordIdx 個字之前插入新點字。
                brLine.Words.Add(form.BrailleWord);
                IsDirty = true;

                // Update UI
                ReformatRow(row);
            }
        }

        /// <summary>
        /// 插入一個空方。
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="count">插入幾個空方。</param>
        private void InsertBlankCell(SourceGrid.Grid grid, int row, int col, int count)
        {
            // 防錯：如果不是有效的儲存格位置就直接返回。
            if (!CheckCellPosition(row, col))
                return;

            int wordIdx = GetBrailleWordIndex(row, col);
            int lineIdx = GetBrailleLineIndex(row);
            BrailleLine brLine = BrailleDoc.Lines[lineIdx];
            while (count > 0)
            {
                brLine.Words.Insert(wordIdx, BrailleWord.NewBlank());
                count--;
            }
            IsDirty = true;

            // Update UI.
            ReformatRow(row);
            SourceGrid.Position pos = new SourceGrid.Position(row, col + 1);
            grid.Selection.Focus(pos, true);    // 修正選取的儲存格範圍。
        }

        /// <summary>
        /// 在指定的列之前插入一列。
        /// </summary>
        private void InsertLine(SourceGrid.Grid grid, int row, int col)
        {
            // 防錯：如果不是有效的儲存格位置就直接返回。
            if (!CheckCellPosition(row, col))
                return;

            // 建立一列新的點字列，其中預設包含一個空方。
            BrailleLine brLine = new BrailleLine();
            brLine.Words.Add(BrailleWord.NewBlank());

            row = GetBrailleRowIndex(row);
            int lineIdx = GetBrailleLineIndex(row);
            BrailleDoc.Lines.Insert(lineIdx, brLine);
            IsDirty = true;

            // 更新 UI。
            GridInsertRowAt(row);
            RefreshRowNumbers();
            FillRow(brLine, row, true);

            // 將焦點移至新插入的那一列的第一個儲存格。
            GridFocusCell(new SourceGrid.Position(row, grid.FixedColumns), true);
        }

        /// <summary>
        /// 刪除一個儲存格的點字。
        /// </summary>
        private void DeleteCell(SourceGrid.Grid grid, int row, int col)
        {
            // 防錯：如果不是有效的儲存格位置就直接返回。
            if (!CheckCellPosition(row, col))
                return;

            row = GetBrailleRowIndex(row);
            int lineIdx = GetBrailleLineIndex(row);
            int wordIdx = GetBrailleWordIndex(row, col);
            BrailleLine brLine = BrailleDoc.Lines[lineIdx];

            if (brLine.Words.Count == 1)    // 如果要刪除該列的最後一個字，就整列刪除。
            {
                DeleteLine(grid, row, col, false);
                return;
            }

            brLine.Words.RemoveAt(wordIdx);
            IsDirty = true;

            // Update UI
            ReformatRow(row);
            grid.Selection.ResetSelection(false); // 修正選取的儲存格範圍。
            SourceGrid.Position pos = new SourceGrid.Position(row, col);
            grid.Selection.Focus(pos, true);
        }

        /// <summary>
        /// 倒退刪除一個儲存格的點字。若在列首的位置執行此動作，則會將該列附加至上一列。
        /// </summary>
        private void BackspaceCell(SourceGrid.Grid grid, int row, int col)
        {
            // 防錯：如果不是有效的儲存格位置就直接返回。
            if (!CheckCellPosition(row, col))
                return;

            row = GetBrailleRowIndex(row);  // 確保列索引為點字列。
            int joinedToLineIdx;
            int joinedToWordIdx;
            bool isFocused = false;

            if (col <= grid.FixedColumns) // 在列首執行此動作?
            {
                // 若有發生斷行，則需要繼續把下一列接上來。
                while (JoinToPreviousRow(row, out joinedToLineIdx, out joinedToWordIdx))
                {
                    if (!isFocused)
                    {
                        int focusRow = GetGridRowIndex(joinedToLineIdx);
                        int focusCol = GetGridColumnIndex(joinedToLineIdx, joinedToWordIdx);
                        GridFocusCell(focusRow, focusCol);
                        isFocused = true;
                    }
                    int lineIdx = GetBrailleLineIndex(row) + 1;
                    if (lineIdx >= BrailleDoc.LineCount)
                    {
                        break;
                    }
                    row = GetGridRowIndex(lineIdx);
                }
            }
            else
            {
                int lineIdx = GetBrailleLineIndex(row);
                int wordIdx = GetBrailleWordIndex(row, col);
                int colToDelete = GetGridColumnIndex(lineIdx, wordIdx - 1);
                DeleteCell(grid, row, colToDelete);
            }
        }

        private void FormatParagraph(SourceGrid.Grid grid, int row, int col)
        {
            // 防錯：如果不是有效的儲存格位置就直接返回。
            if (!CheckCellPosition(row, col))
                return;

            if (MsgBoxHelper.ShowOkCancel("段落重整會將目前所在的段落重新編排，確定要執行此操作嗎？") != DialogResult.OK)
            {
                return;
            }

            row = GetBrailleRowIndex(row);  // 確保列索引為點字列。
            int lineIdx = GetBrailleLineIndex(row) + 1; // 從下一列開始處理（把下一列接上去）

            while (lineIdx < BrailleDoc.LineCount)
            {
                var currLine = BrailleDoc[lineIdx];
                if (currLine.IsEmptyOrWhiteSpace() || currLine.IsBeginOfParagraph())
                {
                    break;
                }
                // 把下一列接上來。
                row = GetGridRowIndex(lineIdx);
                if (JoinToPreviousRow(row, out _, out _))
                {
                    // 當有發生兩列銜接的情形時，列索引不應遞增。
                    continue;
                }
                lineIdx++;
            }
        }

        /// <summary>
        /// 將指定的列附加至上一列。
        /// </summary>
        /// <param name="row"></param>
        /// <returns>傳回新的列數。如果大於 1，代表有發生斷行。</returns>
        private bool JoinToPreviousRow(int row, out int joinedToLineIndex, out int joinedToWordIdx)
        {
            joinedToLineIndex = -1;
            joinedToWordIdx = -1;

            if (row < 0 || row > brGrid.RowsCount)
                throw new ArgumentOutOfRangeException($"參數 {nameof(row)} 的數值超出合法範圍: {row}");

            row = GetBrailleRowIndex(row);  // 確保列索引為點字列。

            if (row <= brGrid.FixedRows)    // 第一列的列首，無需處理。
                return false;

            int lineIdx = GetBrailleLineIndex(row);
            int prevLineIdx = lineIdx - 1;

            BrailleLine currBrLine = BrailleDoc.Lines[lineIdx];
            BrailleLine prevBrLine = BrailleDoc.Lines[prevLineIdx];

            // 檢查上一列是否還有空間可以容納當前列的第一個字
            int avail = BrailleDoc.CellsPerLine - prevBrLine.CellCount;
            if (avail < currBrLine.Words[0].Cells.Count)
            {
                // 上一列的空間不夠，就算接上去，還是會在斷行時再度折下來，因此不處理。
                return false;
            }

            // 記住銜接至上一行的那個字的位置，以便呼叫端把游標移動至該儲存格。
            joinedToLineIndex = prevLineIdx;
            joinedToWordIdx = prevBrLine.WordCount;

            // 執行附加至上一列的動作。
            prevBrLine.Append(currBrLine);

            // 清除本列
            BrailleLine brLine = BrailleDoc.Lines[lineIdx];
            brLine.Clear();
            brLine = null;
            BrailleDoc.Lines.RemoveAt(lineIdx);

            IsDirty = true;

            // 更新 UI：移除本列
            brGrid.Rows.RemoveRange(row, 3);

            // 更新上一列
            ReformatRow(row - 1);

            return true;
        }

        /// <summary>
        /// 在指定處斷行。
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void BreakLine(SourceGrid.Grid grid, int row, int col)
        {
            int wordIdx = GetBrailleWordIndex(row, col);
            if (wordIdx == 0)   // 若在第一個字元處斷行，其實就等於插入一列。
            {
                InsertLine(grid, row, col);
                return;
            }

            int lineIdx = GetBrailleLineIndex(row);
            BrailleLine brLine = BrailleDoc.Lines[lineIdx];

            BrailleLine newLine = brLine.Copy(wordIdx, 255);	// 複製到新行。
            newLine.TrimEnd();	// 去尾空白。 
            BrailleDoc.Lines.Insert(lineIdx + 1, newLine);
            brLine.RemoveRange(wordIdx, 255);	// 從原始串列中刪除掉已經複製到新行的點字。

            IsDirty = true;

            // Update UI

            // 換上新列
            RecreateRow(row);
            FillRow(BrailleDoc[lineIdx], row, true);

            // 插入新列
            GridInsertRowAt(row + 3);
            FillRow(BrailleDoc[lineIdx + 1], row + 3, true);

            // 重新填列號
            RefreshRowNumbers();

            SourceGrid.Position pos = new SourceGrid.Position(row + 3, grid.FixedColumns);
            grid.Selection.Focus(pos, true);    // 修正選取的儲存格範圍。
        }

        private void DeleteLine(Grid grid, int row, int col, bool needConfirm)
        {
            // 防錯：如果不是有效的儲存格位置就直接返回。
            if (!CheckCellPosition(row, col))
                return;

            // 選取欲刪除的列，讓使用者容易知道。
            SourceGrid.Position activePos = brGrid.Selection.ActivePosition;
            GridSelectRow(row, true);

            if (needConfirm && MsgBoxHelper.ShowOkCancel("確定要刪除整列?") != DialogResult.OK)
            {
                GridSelectRow(row, false);
                GridFocusCell(activePos, true);
                return;
            }

            row = GetBrailleRowIndex(row);  // 確保列索引為點字列。

            int lineIdx = GetBrailleLineIndex(row);
            BrailleLine brLine = BrailleDoc.Lines[lineIdx];
            brLine.Clear();
            brLine = null;
            BrailleDoc.Lines.RemoveAt(lineIdx);
            IsDirty = true;

            // 更新 UI。
            brGrid.Rows.RemoveRange(row, 3);

            RefreshRowNumbers();

            GridSelectRow(row, false);
            GridFocusCell(activePos, true);
        }
    }
}
