using System;
using System.Windows.Forms;
using BrailleToolkit;
using Huanlin.Windows.Forms;

namespace EasyBrailleEdit.DualEdit
{
    internal partial class DualEditController
    {
        private void OnDataChanged()
        {
            DataChanged(this, EventArgs.Empty);
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

        #region Grid popup menu item 點擊事件對應的處理函式

        /// <summary>
        /// 修改儲存格。
        /// </summary>
        /// <param name="row">儲存格的列索引。</param>
        /// <param name="col">儲存格的行索引。</param>
        /// <returns>若有更動儲存格內容，則傳回 true，否則傳回 false。</returns>
        internal void EditWord(int row, int col)
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

            BrailleWord brWord = (BrailleWord)brGrid[row, col].Tag;

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

                    OnDataChanged();

                    GridCellChanged(row, col, brWord, cellChgType);
                }
            }
        }

        /// <summary>
        /// 新增點字。
        /// </summary>
        /// <param name="row">儲存格的列索引。</param>
        /// <param name="col">儲存格的行索引。</param>
        internal void InsertWord(int row, int col)
        {
            if (!CheckCellPosition(row, col))
                return;

            EditCellForm form = new EditCellForm();
            form.Mode = EditCellMode.Insert;
            if (form.ShowDialog() == DialogResult.OK)
            {
                int wordIdx = GetBrailleWordIndex(row, col);
                int lineIdx = GetBrailleLineIndex(row);
                BrailleLine brLine = m_BrDoc.Lines[lineIdx];

                // 在第 wordIdx 個字之前插入新點字。
                brLine.Words.Insert(wordIdx, form.BrailleWord);

                OnDataChanged();

                // Update UI
                ReformatRow(row);
                SourceGrid.Position pos = new SourceGrid.Position(row, col + 1);
                brGrid.Selection.Focus(pos, true);    // 修正選取的儲存格範圍。
            }
        }

        /// <summary>
        /// 在行尾附加點字。
        /// </summary>
        internal void AppendWord(int row, int col)
        {
            if (!CheckCellPosition(row, col))
                return;

            EditCellForm form = new EditCellForm();
            form.Mode = EditCellMode.Insert;
            if (form.ShowDialog() == DialogResult.OK)
            {
                int lineIdx = GetBrailleLineIndex(row);
                BrailleLine brLine = m_BrDoc.Lines[lineIdx];

                // 在第 wordIdx 個字之前插入新點字。
                brLine.Words.Add(form.BrailleWord);
                
                OnDataChanged();

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
        internal void InsertBlankCell(int row, int col, int count)
        {
            // 防錯：如果不是有效的儲存格位置就直接返回。
            if (!CheckCellPosition(row, col))
                return;

            int wordIdx = GetBrailleWordIndex(row, col);
            int lineIdx = GetBrailleLineIndex(row);
            BrailleLine brLine = m_BrDoc.Lines[lineIdx];
            while (count > 0)
            {
                brLine.Words.Insert(wordIdx, BrailleWord.NewBlank());
                count--;
            }
            OnDataChanged();

            // Update UI.
            ReformatRow(row);
            SourceGrid.Position pos = new SourceGrid.Position(row, col + 1);
            brGrid.Selection.Focus(pos, true);    // 修正選取的儲存格範圍。
        }

        /// <summary>
        /// 在指定的列之前插入一列。
        /// </summary>
        internal void InsertLine(int row, int col)
        {
            // 防錯：如果不是有效的儲存格位置就直接返回。
            if (!CheckCellPosition(row, col))
                return;

            // 建立一列新的點字列，其中預設包含一個空方。
            BrailleLine brLine = new BrailleLine();
            brLine.Words.Add(BrailleWord.NewBlank());

            row = GetBrailleRowIndex(row);
            int lineIdx = GetBrailleLineIndex(row);
            m_BrDoc.Lines.Insert(lineIdx, brLine);
            OnDataChanged();

            // 更新 UI。
            GridInsertRowAt(row);
            RefreshRowNumbers();
            FillRow(brLine, row, true);

            // 將焦點移至新插入的那一列的第一個儲存格。
            GridFocusCell(new SourceGrid.Position(row, brGrid.FixedColumns), true);
        }

        /// <summary>
        /// 刪除一個儲存格的點字。
        /// </summary>
        internal void DeleteCell(int row, int col)
        {
            // 防錯：如果不是有效的儲存格位置就直接返回。
            if (!CheckCellPosition(row, col))
                return;

            row = GetBrailleRowIndex(row);
            int lineIdx = GetBrailleLineIndex(row);
            int wordIdx = GetBrailleWordIndex(row, col);
            BrailleLine brLine = m_BrDoc.Lines[lineIdx];

            if (brLine.Words.Count == 1)    // 如果要刪除該列的最後一個字，就整列刪除。
            {
                DeleteLine(row, col, false);
                return;
            }

            brLine.Words.RemoveAt(wordIdx);
            OnDataChanged();

            // Update UI
            ReformatRow(row);
            brGrid.Selection.ResetSelection(false); // 修正選取的儲存格範圍。
            SourceGrid.Position pos = new SourceGrid.Position(row, col);
            brGrid.Selection.Focus(pos, true);
        }

        /// <summary>
        /// 倒退刪除一個儲存格的點字。若在列首的位置執行此動作，則會將該列附加至上一列。
        /// </summary>
        internal void BackspaceCell(int row, int col)
        {
            // 防錯：如果不是有效的儲存格位置就直接返回。
            if (!CheckCellPosition(row, col))
                return;

            row = GetBrailleRowIndex(row);  // 確保列索引為點字列。

            if (col <= brGrid.FixedColumns) // 在列首執行此動作?
            {
                JoinToPreviousRow(row);
            }
            else
            {
                DeleteCell(row, col - 1);
            }
        }

        /// <summary>
        /// 將指定的列附加至上一列。
        /// </summary>
        /// <param name="row"></param>
        private void JoinToPreviousRow(int row)
        {
            if (row < 0)    // 防錯
                return;

            row = GetBrailleRowIndex(row);  // 確保列索引為點字列。

            if (row <= brGrid.FixedRows)    // 第一列的列首，無需處理。
                return;

            int lineIdx = GetBrailleLineIndex(row);
            BrailleLine prevBrLine = m_BrDoc.Lines[lineIdx - 1];
            BrailleLine currBrLine = m_BrDoc.Lines[lineIdx];

            // 檢查上一列是否還有空間可以容納新點字
            int avail = m_BrDoc.CellsPerLine - prevBrLine.CellCount;
            if (avail < currBrLine.Words[0].Cells.Count)
            {
                // 上一列的空間不夠，就算接上去，還是會在斷行時再度折下來，因此不處理。
                return;
            }

            // 執行附加至上一列的動作。
            prevBrLine.Append(currBrLine);

            // 清除本列
            BrailleLine brLine = m_BrDoc.Lines[lineIdx];
            brLine.Clear();
            brLine = null;
            m_BrDoc.Lines.RemoveAt(lineIdx);

            OnDataChanged();

            // 更新 UI：移除本列
            brGrid.Rows.RemoveRange(row, 3);

            // 更新上一列
            ReformatRow(row - 1);

            RefreshRowNumbers();

            return;
        }

        /// <summary>
        /// 在指定處斷行。
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void BreakLine(int row, int col)
        {
            int wordIdx = GetBrailleWordIndex(row, col);
            if (wordIdx == 0)   // 若在第一個字元處斷行，其實就等於插入一列。
            {
                InsertLine(row, col);
                return;
            }

            int lineIdx = GetBrailleLineIndex(row);
            BrailleLine brLine = m_BrDoc.Lines[lineIdx];

            BrailleLine newLine = brLine.ShallowCopy(wordIdx, 255);	// 複製到新行。
            newLine.TrimEnd();	// 去尾空白。 
            m_BrDoc.Lines.Insert(lineIdx + 1, newLine);
            brLine.RemoveRange(wordIdx, 255);	// 從原始串列中刪除掉已經複製到新行的點字。

            OnDataChanged();

            // Update UI

            // 換上新列
            RecreateRow(row);
            FillRow(m_BrDoc[lineIdx], row, true);

            // 插入新列
            GridInsertRowAt(row + 3);
            FillRow(m_BrDoc[lineIdx + 1], row + 3, true);

            // 重新填列號
            RefreshRowNumbers();

            SourceGrid.Position pos = new SourceGrid.Position(row + 3, brGrid.FixedColumns);
            brGrid.Selection.Focus(pos, true);    // 修正選取的儲存格範圍。
        }

        internal void DeleteLine(int row, int col, bool needConfirm)
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
            BrailleLine brLine = m_BrDoc.Lines[lineIdx];
            brLine.Clear();
            brLine = null;
            m_BrDoc.Lines.RemoveAt(lineIdx);

            OnDataChanged();

            // 更新 UI。
            brGrid.Rows.RemoveRange(row, 3);

            RefreshRowNumbers();

            GridSelectRow(row, false);
            GridFocusCell(activePos, true);
        }


        /// <summary>
        /// 重新編排目前所在的（或選取的）段落。即段落重整。<br />
        /// 處理方式：<br />
        /// 把
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        internal void FormatParagraph(int row, int col)
        {

        }

        #endregion

    }
}
