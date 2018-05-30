using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrailleToolkit;
using BrailleToolkit.Helpers;
using Huanlin.Windows.Forms;
using SourceGrid;

namespace EasyBrailleEdit.DualEdit
{
    /// <summary>
    /// DualEditForm 局部類別: 編輯功能相關 methods。
    /// </summary>

    internal partial class BrailleGridController
    {

        /// <summary>
        /// 修改儲存格。
        /// </summary>
        /// <param name="grid">來源 grid。</param>
        /// <param name="row">儲存格的列索引。</param>
        /// <param name="col">儲存格的行索引。</param>
        private void EditWord(SourceGrid.Grid grid, int row, int col)
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
                var cellChgType = CellChangedType.None;
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
            row = _positionMapper.GetBrailleRowIndex(row);

            // Note: 不可重新轉點字，只重新斷行。

            // 判斷新設定的點字方數是否不同於儲存格原有的點字方數，若否，則必須重新斷字。
            if (_grid[row, col].ColumnSpan == brWord.Cells.Count)
            {
                UpdateCell(row, col, brWord);
            }
            else
            {
                ReformatRow(_grid, row);
            }
        }


        /// <summary>
        /// 新增點字。
        /// </summary>
        /// <param name="grid">來源 grid。</param>
        /// <param name="row">儲存格的列索引。</param>
        /// <param name="col">儲存格的行索引。</param>
        private void InsertWord(SourceGrid.Grid grid, int row, int col)
        {
            if (!CheckCellPosition(row, col))
                return;

            EditCellForm form = new EditCellForm();
            form.Mode = EditCellMode.Insert;
            if (form.ShowDialog() == DialogResult.OK)
            {
                int wordIdx = _positionMapper.CellPositionToWordIndex(row, col);
                int lineIdx = _positionMapper.GridRowToBrailleLineIndex(row);
                BrailleLine brLine = BrailleDoc.Lines[lineIdx];

                // 在第 wordIdx 個字之前插入新點字。
                brLine.Words.Insert(wordIdx, form.BrailleWord);
                IsDirty = true;

                // Update UI
                ReformatRow(grid, row);
                int focusCol = _positionMapper.WordIndexToGridColumn(lineIdx, wordIdx);
                GridFocusCell(row, focusCol);
            }
        }

        /// <summary>
        /// 新增一串文字。
        /// </summary>
        /// <param name="grid">來源 grid。</param>
        /// <param name="row">儲存格的列索引。</param>
        /// <param name="col">儲存格的行索引。</param>
        private void InsertText(SourceGrid.Grid grid, int row, int col)
        {
            if (!CheckCellPosition(row, col))
                return;

            var form = new InsertTextForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                InsertBrailleWords(form.OutputLine.Words, grid, row, col);
            }
        }


        private void InsertBrailleLines(List<BrailleLine> brLines, SourceGrid.Grid grid, int row, int col)
        {
            row = _positionMapper.GetBrailleRowIndex(row);
            int lineIdx = _positionMapper.GridRowToBrailleLineIndex(row);
            BrailleLine brLine = BrailleDoc.Lines[lineIdx];

            int curRow = row;
            for (int i = 0; i < brLines.Count; i++)
            {
                BrailleDoc.Lines.Insert(lineIdx, brLines[i]);

                // Update UI
                GridInsertRowAt(curRow);
                FillRow(brLines[i], curRow, true);


                lineIdx++;
                curRow += 3;
            }
            IsDirty = true;

            RefreshRowNumbers();    // 重新填列號

            int endRow = row + brLines.Count * 3 - 1;
            var range = new Range(row, grid.FixedColumns, endRow, grid.ColumnsCount - grid.FixedColumns);
            grid.Selection.SelectRange(range, true);
        }

        private void InsertBrailleWords(List<BrailleWord> wordList, SourceGrid.Grid grid, int row, int col)
        {
            row = _positionMapper.GetBrailleRowIndex(row); // 確保列索引是點字所在的列。
            int wordIdx = _positionMapper.CellPositionToWordIndex(row, col);
            int lineIdx = _positionMapper.GridRowToBrailleLineIndex(row);
            BrailleLine brLine = BrailleDoc.Lines[lineIdx];

            // 如果插入的是頁標題，則直接加到頁標題集合裡。
            if (BrailleWordHelper.ContainsTitleTag(wordList))
            {
                var title = new BraillePageTitle(wordList, lineIdx);
                BrailleDoc.AddPageTitle(title);

                MsgBoxHelper.ShowInfo("已成功加入頁標題。");
                return;
            }

            // 在第 wordIdx 個字之前插入新點字。
            brLine.Words.InsertRange(wordIdx, wordList);
            IsDirty = true;

            // Update UI
            int formattedLineCount = ReformatRow(grid, row);

            // 經過上面的 reformat 處理，插入的文字有可能因為斷字或其他編排規則而被切到下一行。
            // 因此必須判斷插入文字的目標儲存格上面有沒有文字，若沒有，則表示文字被折到下一行了，應清除選取區域，並將游標定位至下一行的第一個字。
            if (formattedLineCount > 1 && grid[row, col] == null)
            {
                grid.Selection.ResetSelection(false);
                GridFocusCell(row + 3, grid.FixedColumns);
            }
            else
            {
                var range = new Range(row, col, row + 2, col + wordList.GetCellCount() - 1);
                grid.Selection.SelectRange(range, true);
            }
        }

        /// <summary>
        /// 在行尾附加點字。
        /// </summary>
        private void AppendWord(SourceGrid.Grid grid, int row, int col)
        {
            if (!CheckCellPosition(row, col))
                return;

            EditCellForm form = new EditCellForm();
            form.Mode = EditCellMode.Insert;
            if (form.ShowDialog() == DialogResult.OK)
            {
                int lineIdx = _positionMapper.GridRowToBrailleLineIndex(row);
                BrailleLine brLine = BrailleDoc.Lines[lineIdx];

                // 在第 wordIdx 個字之前插入新點字。
                brLine.Words.Add(form.BrailleWord);
                IsDirty = true;

                // Update UI
                ReformatRow(grid, row);
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

            int wordIdx = _positionMapper.CellPositionToWordIndex(row, col);
            int lineIdx = _positionMapper.GridRowToBrailleLineIndex(row);
            BrailleLine brLine = BrailleDoc.Lines[lineIdx];
            while (count > 0)
            {
                brLine.Words.Insert(wordIdx, BrailleWord.NewBlank());
                count--;
            }
            IsDirty = true;

            // Update UI.
            ReformatRow(grid, row);
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
            var brLine = new BrailleLine();
            brLine.Words.Add(BrailleWord.NewBlank());

            row = _positionMapper.GetBrailleRowIndex(row);
            int lineIdx = _positionMapper.GridRowToBrailleLineIndex(row);
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
        /// 在指定的列之後加入一列。
        /// </summary>
        private void AddLine(SourceGrid.Grid grid, int row, int col)
        {
            // 防錯：如果不是有效的儲存格位置就直接返回。
            if (!CheckCellPosition(row, col))
                return;

            // 建立一列新的點字列，其中預設包含一個空方。
            var brLine = new BrailleLine();
            brLine.Words.Add(BrailleWord.NewBlank());

            int lineIdx = _positionMapper.GridRowToBrailleLineIndex(row) + 1;
            BrailleDoc.Lines.Insert(lineIdx, brLine);
            IsDirty = true;

            // 更新 UI。
            row = _positionMapper.LineIndexToGridBrailleRow(lineIdx);
            GridInsertRowAt(row);
            RefreshRowNumbers();
            FillRow(brLine, row, true);

            // 將焦點移至新插入的那一列的第一個儲存格。
            GridFocusCell(new SourceGrid.Position(row, grid.FixedColumns), true);
        }


        /// <summary>
        /// 刪除一個儲存格的點字。
        /// </summary>
        private void DeleteWord(SourceGrid.Grid grid, int row, int col)
        {
            // 防錯：如果不是有效的儲存格位置就直接返回。
            if (!CheckCellPosition(row, col))
                return;

            int orgRow = row;
            row = _positionMapper.GetBrailleRowIndex(row);

            int lineIdx = _positionMapper.GridRowToBrailleLineIndex(row);
            BrailleLine brLine = BrailleDoc.Lines[lineIdx];

            if (grid[row, col] == null)
            {
                MsgBoxHelper.ShowError($"無效的行與列索引: 橫列={row}, 直行={col}。");
                return;
            }

            var wordToDelete = grid[row, col].Tag as BrailleWord;
            if (wordToDelete == null)
            {
                MsgBoxHelper.ShowError($"儲存格連結的點字物件為 null！橫列={row}, 直行={col}。");
                return;
            }
            int wordIdx = brLine.IndexOf(wordToDelete);
            if (wordIdx < 0)
            {
                MsgBoxHelper.ShowError($"目前的列找不到此物件: {wordToDelete.Text}。橫列={row}, 直行={col}。");
                return;
            }

            if (BrailleDoc.LineCount == 1 && brLine.CellCount == 1) // 整份文件只剩下最後一個字？
            {
                int firstWordIdx = brLine.GetFirstVisibleWordIndex();
                if (firstWordIdx < 0 || brLine[firstWordIdx].IsWhiteSpace) // 保留最後一個空方.
                    return;
            }

            // 取得目前要刪除的字的第一個 cell 的 column index。此操作必須在刪字之前做。
            col = _positionMapper.WordIndexToGridColumn(lineIdx, wordIdx);

            brLine.Words.RemoveAt(wordIdx);
            IsDirty = true;

            if (brLine.CellCount == 0)    // 如果整列都刪光了，就移除此列。
            {
                if (BrailleDoc.LineCount == 1)
                {
                    // 整份文件全刪光時，自動增加一個空方。
                    brLine.Words.Add(BrailleWord.NewBlank());
                    UpdateCell(row, col, brLine.Words[0]);
                    GridFocusCell(row, col);
                }
                else
                {
                    DoDeleteLine(grid, row, lineIdx);
                    GridFocusCell(row, col);
                }
                return;
            }

            // Update UI
            ReformatRow(grid, row);
            grid.Selection.ResetSelection(false); // 修正選取的儲存格範圍。

            // 如果被刪除的是目前所在列的最後一個字，則游標應移動至刪除該字之後的最後一個字的位置。
            if (col - FixedColumns >= brLine.CellCount)
            {
                col = brLine.CellCount - 1 + grid.FixedColumns;
            }
            GridFocusCell(orgRow, col);
        }

        /// <summary>
        /// 倒退刪除一個儲存格的點字。若在列首的位置執行此動作，則會將該列附加至上一列。
        /// </summary>
        private void BackspaceCell(SourceGrid.Grid grid, int row, int col)
        {
            // 防錯：如果不是有效的儲存格位置就直接返回。
            if (!CheckCellPosition(row, col))
                return;

            int orgRow = row;
            row = _positionMapper.GetBrailleRowIndex(row);  // 確保列索引為點字列。

            // 算法：找到目前位置所在的字索引，再取得該字的第一個 cell 的 column index，
            //      然後把 column index 減 1，便得到欲刪除的儲存格位置了。
            int lineIdx = _positionMapper.GridRowToBrailleLineIndex(row);
            int wordIdx = _positionMapper.CellPositionToWordIndex(row, col);

            if (lineIdx == 0 && wordIdx == BrailleDoc.Lines[lineIdx].GetFirstVisibleWordIndex())
            {
                return;     // 只剩下一個字，不用做任何處理。
            }

            int colToDelete = _positionMapper.WordIndexToGridColumn(lineIdx, wordIdx) - 1;
            bool isFirstColumn = false;
            if (colToDelete < grid.FixedColumns) // // 在列首執行此動作?
            {
                colToDelete = grid.FixedColumns;
                isFirstColumn = true;
            }

            if (isFirstColumn)
            {
                // 先計算新的游標位置
                int focusRow = orgRow - 3;
                lineIdx = _positionMapper.GridRowToBrailleLineIndex(focusRow);
                var brLine = BrailleDoc.Lines[lineIdx];
                int focusCol = brLine.CellCount + grid.FixedColumns;

                // 持續把下一列接上來，直到沒有發生斷行為止。
                int joinedToLineIdx;
                int joinedToWordIdx;
                bool isFocused = false;
                while (JoinToPreviousRow(row, out joinedToLineIdx, out joinedToWordIdx))
                {
                    if (!isFocused)
                    {
                        GridFocusCell(focusRow, focusCol);
                        isFocused = true;
                    }
                    lineIdx = _positionMapper.GridRowToBrailleLineIndex(row) + 1;
                    if (lineIdx >= BrailleDoc.LineCount)
                    {
                        break;
                    }
                    row = _positionMapper.LineIndexToGridBrailleRow(lineIdx);
                }
            }
            else
            {
                DeleteWord(grid, orgRow, colToDelete);
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

            row = _positionMapper.GetBrailleRowIndex(row);  // 確保列索引為點字列。
            int lineIdx = _positionMapper.GridRowToBrailleLineIndex(row) + 1; // 從下一列開始處理（把下一列接上去）

            while (lineIdx < BrailleDoc.LineCount)
            {
                var currLine = BrailleDoc[lineIdx];
                if (currLine.IsEmptyOrWhiteSpace() || currLine.IsBeginOfParagraph())
                {
                    break;
                }
                // 把下一列接上來。
                row = _positionMapper.LineIndexToGridBrailleRow(lineIdx);
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

            if (row < 0 || row > _grid.RowsCount)
                throw new ArgumentOutOfRangeException($"參數 {nameof(row)} 的數值超出合法範圍: {row}");

            row = _positionMapper.GetBrailleRowIndex(row);  // 確保列索引為點字列。

            if (row <= _grid.FixedRows)    // 第一列的列首，無需處理。
                return false;

            int lineIdx = _positionMapper.GridRowToBrailleLineIndex(row);
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
            _grid.Rows.RemoveRange(row, 3);

            // 更新上一列
            ReformatRow(_grid, row - 1);

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
            int lineIdx = _positionMapper.GridRowToBrailleLineIndex(row);
            BrailleLine brLine = BrailleDoc.Lines[lineIdx];

            int wordIdx = _positionMapper.CellPositionToWordIndex(row, col);
            if (wordIdx == brLine.GetFirstVisibleWordIndex())   // 若在第一個字元處斷行，其實就等於插入一列。
            {
                InsertLine(grid, row, col);
                return;
            }

            BrailleLine newLine = brLine.ShallowCopy(wordIdx, 255);	// 複製到新行。
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
            SourceGrid.Position activePos = grid.Selection.ActivePosition;
            GridSelectRow(row, true);

            if (needConfirm && MsgBoxHelper.ShowOkCancel("確定要刪除整列?") != DialogResult.OK)
            {
                GridSelectRow(row, false);
                GridFocusCell(activePos, true);
                return;
            }

            row = _positionMapper.GetBrailleRowIndex(row);  // 確保列索引為點字列。

            int lineIdx = _positionMapper.GridRowToBrailleLineIndex(row);
            BrailleLine brLine = BrailleDoc.Lines[lineIdx];
            brLine.Clear();
            brLine = null;
            BrailleDoc.Lines.RemoveAt(lineIdx);
            IsDirty = true;

            // 更新 UI。
            grid.Rows.RemoveRange(row, 3);

            RefreshRowNumbers();

            GridSelectRow(row, false);
            GridFocusCell(activePos, true);
        }

        private void DoDeleteLine(Grid grid, int row, int lineIdx)
        {
            row = _positionMapper.GetBrailleRowIndex(row);  // 確保列索引為點字列。

            var brLine = BrailleDoc.Lines[lineIdx];
            brLine.Clear();
            brLine = null;
            BrailleDoc.Lines.RemoveAt(lineIdx);
            IsDirty = true;

            // 更新 UI。
            grid.Rows.RemoveRange(row, 3);

            RefreshRowNumbers();

            GridSelectRow(row, false);
        }

        private bool GetSelectionRange(Grid grid,
            out int startRow, out int startCol, out int endRow, out int endCol)
        {
            var selectedCellsPositions = grid.Selection.GetSelectionRegion().GetCellsPositions();

            var sb = new StringBuilder();

            startRow = -1;
            endRow = -1;
            startCol = 0;
            endCol = 0;
            foreach (var pos in selectedCellsPositions)
            {
                if (startRow == -1)
                {
                    startRow = _positionMapper.GetBrailleRowIndex(pos.Row);
                    endRow = startRow;
                    startCol = pos.Column;
                    endCol = pos.Column;
                }
                else
                {
                    endRow = _positionMapper.GetBrailleRowIndex(pos.Row);

                    if (pos.Column - endCol > 1)
                    {
                        MsgBoxHelper.ShowError("不支援分段選取！請複製同一行中的連續文字。");
                        return false;
                    }
                    endCol = pos.Column;
                }
            }

            if (startRow != endRow) // 選取多行？
            {
                int totalColumns = grid.ColumnsCount - grid.FixedColumns;
                if ((endCol - startCol + 1) < totalColumns)
                {
                    int startLineIdx = _positionMapper.GridRowToBrailleLineIndex(startRow);
                    int endLineIdx = _positionMapper.GridRowToBrailleLineIndex(endRow);
                    var s = $"複製多行時，必須選取整行。\r\n您是否要複製第 {startLineIdx + 1} 至 {endLineIdx + 1} 行？";
                    if (MsgBoxHelper.ShowYesNo(s) != DialogResult.Yes)
                        return false;

                    // 修改選取範圍
                    startCol = grid.FixedColumns;
                    endCol = totalColumns;
                }

                grid.Selection.ResetSelection(false);
                grid.Selection.SelectRange(new Range(startRow, startCol, endRow + 2, endCol), true);
            }
            return true;
        }

        private List<BrailleLine> CloneSelectedBrailleWords(Grid grid, int startRow, int startCol, int endRow, int endCol)
        {
            var result = new List<BrailleLine>();

            if (startRow != endRow)
            {
                if ((endCol - startCol + 1) < (grid.ColumnsCount - grid.FixedColumns))
                {
                    throw new ArgumentException($"參數錯誤! 多行選取時，{nameof(startCol)} 和 {nameof(endCol)} 應為 1 和 40。");
                }
                int startLineIdx = _positionMapper.GridRowToBrailleLineIndex(startRow);
                int endLineIdx = _positionMapper.GridRowToBrailleLineIndex(endRow);


                for (int i = startLineIdx; i <= endLineIdx; i++)
                {
                    var aLine = BrailleDoc.Lines[i];
                    result.Add(aLine.DeepCopy(0, aLine.WordCount));
                    // TODO: 上面的複製操作，也許該略過 context tags？
                }
                return result;
            }

            // 選取同一列中的字。
            int row = startRow;
            int startWordIdx = _positionMapper.CellPositionToWordIndex(row, startCol);
            int endWordIdx = _positionMapper.CellPositionToWordIndex(row, endCol);

            // 建立欲複製的點字串列。注意：context tags 都會被忽略。

            int lineIdx = _positionMapper.GridRowToBrailleLineIndex(row);
            var brLine = BrailleDoc.Lines[lineIdx];
            var newBrLine = new BrailleLine();
            for (int i = startWordIdx; i <= endWordIdx; i++)
            {
                if (brLine[i].IsContextTag)
                    continue;
                var newWord = brLine[i].Copy();
                newBrLine.Words.Add(newWord);
            }
            result.Add(newBrLine);
            return result;
        }

        private void CopyToClipboard(Grid grid)
        {
            int startRow, startCol, endRow, endCol;

            if (!GetSelectionRange(grid, out startRow, out startCol, out endRow, out endCol))
                return;

            var brLines = CloneSelectedBrailleWords(grid, startRow, startCol, endRow, endCol);
            if (startRow == endRow)
            {
                ClipboardHelper.SetWords(brLines[0].Words);
            }
            else
            {

                ClipboardHelper.SetLines(brLines);
            }
        }

        private void PasteFromClipboard(Grid grid, int row, int col)
        {
            var brWords = ClipboardHelper.GetWords();
            var brLines = ClipboardHelper.GetLines();

            if (brWords != null)
            {
                InsertBrailleWords(brWords, grid, row, col);
                return;
            }
            if (brLines != null)
            {
                InsertBrailleLines(brLines, grid, row, col);
                return;
            }
            MsgBoxHelper.ShowInfo("剪貼簿裡面沒有資料！");
        }

    }
}
