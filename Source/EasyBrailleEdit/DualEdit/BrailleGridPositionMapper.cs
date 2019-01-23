using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrailleToolkit;
using Huanlin.Windows.Forms;

namespace EasyBrailleEdit.DualEdit
{
    /// <summary>
    /// 此類別提供 _document 物件中的元素與 Grid 儲存格之間的位置相互對應。
    /// </summary>
    internal class BrailleGridPositionMapper
    {
        private BrailleDocument _doc;
        private SourceGrid.Grid _grid;

        public BrailleGridPositionMapper(BrailleDocument doc, SourceGrid.Grid grid)
        {
            _doc = doc;
            _grid = grid;
        }

        public BrailleDocument BrailleDoc
        {
            get => _doc;
            set
            {
                if (_doc != value)
                {
                    _doc = value;
                }
            }
        }

        #region 計算點字、列索引的相關函式

        /// <summary>
        /// 根據指定的 grid 列索引計算出該列的點字列的列索引。
        /// 由於每個點字在顯示時佔三列（點字、明眼字、注音碼），此方法可從 grid 的列索引推算點字列的列索引。
        /// 註：grid 的第 0 列是標題列。
        /// </summary>
        /// <param name="gridRowIndex"></param>
        /// <returns></returns>
        public int GetBrailleRowIndex(int gridRowIndex)
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
        /// 根據 grid 列索引計算出該列屬於點字文件的哪一列，即 _document 的 Lines 集合索引。
        /// 註：grid 的第 0 列是標題列。
        /// </summary>
        /// <param name="gridRowIndex">Grid 列索引。</param>
        /// <returns></returns>
        public int GridRowToBrailleLineIndex(int gridRowIndex)
        {
            return (gridRowIndex - _grid.FixedRows) / 3;
        }

        public BrailleWord GetBrailleWordFromGridCell(int row, int col)
        {
            if (_grid[row, col] == null)
            {
                return null;
            }

            return _grid[row, col].Tag as BrailleWord;
        }

        /// <summary>
        /// 根據 grid 儲存格位置來計算它對應的點字列的哪一個字，即 BrailleLine 的 Words 集合索引。
        /// </summary>
        /// <param name="gridRowIndex">Grid 列索引。</param>
        /// <param name="gridColumnIndex">Grid 行索引。</param>
        /// <returns></returns>
        public int CellPositionToWordIndex(int row, int col)
        {
            if (_grid[row, col] == null)
            {
                throw new Exception($"無效的行與列索引: 橫列={row}, 直行={col}。");
            }

            var wordInQuestion = GetBrailleWordFromGridCell(row, col);
            if (wordInQuestion == null)
            {
                throw new Exception($"找不到點字物件: 橫列={row}, 直行={col}。");
            }

            int lineIdx = GridRowToBrailleLineIndex(row);
            BrailleLine brLine = _doc.Lines[lineIdx];

            int wordIndex = brLine.IndexOf(wordInQuestion);
            if (wordIndex < 0)
            {
                throw new Exception($"Grid 儲存格有點字物件 '{wordInQuestion.ToString()}'，但是在 BraillDocument 中找不到此點字物件!（可能是多執行緒重入所致）");
            }

            return wordIndex;
        }

        /// <summary>
        /// 根據傳入的點字文件列索引取得對應的 Grid 點字列索引。
        /// </summary>
        /// <param name="lineIdx"></param>
        /// <returns></returns>
        public int LineIndexToGridBrailleRow(int lineIdx)
        {
            return (lineIdx * 3) + _grid.FixedRows;
        }

        /// <summary>
        /// 根據傳入的點字文件列索引取得對應的 Grid 明眼字的列索引。
        /// </summary>
        /// <param name="lineIdx"></param>
        /// <returns></returns>
        public int LineIndexToGridTextRow(int lineIdx)
        {
            return (lineIdx * 3) + _grid.FixedRows + 1;
        }

        /// <summary>
        /// 根據傳入的點字文件列索引和字索引，取得對應的 Grid 欄索引。
        /// </summary>
        /// <param name="lineIdx"></param>
        /// <param name="wordIdx"></param>
        /// <returns></returns>
        public int WordIndexToGridColumn(int lineIdx, int wordIdx)
        {
            int textRowIdx = LineIndexToGridTextRow(lineIdx);	// 明眼字的列索引
            int col = _grid.FixedColumns;

            var brWord = _doc.Lines[lineIdx].Words[wordIdx];
            while (col < _grid.ColumnsCount)
            {
                if (_grid[textRowIdx, col] == null)
                {
                    throw new InvalidOperationException($"執行 WordIndexToGridColumn({lineIdx}, {wordIdx})時發現 Grid[{textRowIdx},{col}] 為空!");
                }
                var wordInCell = _grid[textRowIdx, col].Tag as BrailleWord;
                if (ReferenceEquals(wordInCell, brWord))
                {
                    return col;
                }
                col++;
            }

            throw new Exception("GetGridColumnIndex 找不到列索引和字索引。");
        }

        #endregion


    }
}
