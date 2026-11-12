using BrailleToolkit;
using SourceGrid;
using SourceGrid.Selection;

namespace EasyBrailleEdit.DualEdit
{
    internal sealed class BrailleGridCellBookmark
    {
        public long LineIdentity { get; }
        public long WordIdentity { get; }
        public int RowOffset { get; }
        public int ColumnOffset { get; }
        public int FallbackRow { get; }
        public int FallbackColumn { get; }

        private BrailleGridCellBookmark(
            long lineIdentity,
            long wordIdentity,
            int rowOffset,
            int columnOffset,
            int fallbackRow,
            int fallbackColumn)
        {
            LineIdentity = lineIdentity;
            WordIdentity = wordIdentity;
            RowOffset = rowOffset;
            ColumnOffset = columnOffset;
            FallbackRow = fallbackRow;
            FallbackColumn = fallbackColumn;
        }

        public static BrailleGridCellBookmark Capture(
            BrailleDocument sourceDoc,
            BrailleDocument snapshotDoc,
            SourceGrid.Grid grid,
            BrailleGridPositionMapper positionMapper,
            Position position)
        {
            if (position.Row < grid.FixedRows || position.Row >= grid.RowsCount
                || position.Column < grid.FixedColumns || position.Column >= grid.ColumnsCount)
            {
                return new BrailleGridCellBookmark(0, 0, 0, 0, position.Row, position.Column);
            }

            int brailleRow = positionMapper.GetBrailleRowIndex(position.Row);
            int rowOffset = position.Row - brailleRow;
            int lineIdx = positionMapper.GridRowToBrailleLineIndex(position.Row);
            if (lineIdx < 0 || lineIdx >= sourceDoc.Lines.Count || lineIdx >= snapshotDoc.Lines.Count)
            {
                return new BrailleGridCellBookmark(0, 0, rowOffset, 0, position.Row, position.Column);
            }

            long lineIdentity = snapshotDoc.Lines[lineIdx].Identity;
            if (!positionMapper.TryGetBrailleWordAtGridPosition(position.Row, position.Column, out var wordInSource, out int wordStartCol)
                || wordInSource == null)
            {
                return new BrailleGridCellBookmark(lineIdentity, 0, rowOffset, 0, position.Row, position.Column);
            }

            int wordIdx = sourceDoc.Lines[lineIdx].IndexOf(wordInSource);
            if (wordIdx < 0 || wordIdx >= snapshotDoc.Lines[lineIdx].Words.Count)
            {
                return new BrailleGridCellBookmark(lineIdentity, 0, rowOffset, 0, position.Row, position.Column);
            }

            long wordIdentity = snapshotDoc.Lines[lineIdx].Words[wordIdx].Identity;
            int columnOffset = position.Column - wordStartCol;
            return new BrailleGridCellBookmark(lineIdentity, wordIdentity, rowOffset, columnOffset, position.Row, position.Column);
        }

        public bool TryResolve(SourceGrid.Grid grid, BrailleGridPositionMapper positionMapper, out Position position)
        {
            if (LineIdentity > 0 && positionMapper.TryGetLineIndex(LineIdentity, out int lineIdx))
            {
                int row = positionMapper.LineIndexToGridBrailleRow(lineIdx) + RowOffset;
                int col = FallbackColumn;

                if (WordIdentity > 0 && positionMapper.TryGetWordIndex(LineIdentity, WordIdentity, out lineIdx, out int wordIdx))
                {
                    col = positionMapper.WordIndexToGridColumn(lineIdx, wordIdx) + ColumnOffset;
                }

                row = Clamp(row, grid.FixedRows, grid.RowsCount - 1);
                col = Clamp(col, grid.FixedColumns, grid.ColumnsCount - 1);
                position = new Position(row, col);
                return true;
            }

            if (grid.RowsCount <= grid.FixedRows || grid.ColumnsCount <= grid.FixedColumns)
            {
                position = new Position(grid.FixedRows, grid.FixedColumns);
                return false;
            }

            position = new Position(
                Clamp(FallbackRow, grid.FixedRows, grid.RowsCount - 1),
                Clamp(FallbackColumn, grid.FixedColumns, grid.ColumnsCount - 1));
            return true;
        }

        private static int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
    }

    internal sealed class BrailleGridRangeBookmark
    {
        public BrailleGridCellBookmark Start { get; }
        public BrailleGridCellBookmark End { get; }

        public BrailleGridRangeBookmark(BrailleGridCellBookmark start, BrailleGridCellBookmark end)
        {
            Start = start;
            End = end;
        }

        public bool TryResolve(SourceGrid.Grid grid, BrailleGridPositionMapper positionMapper, out SourceGrid.Range range)
        {
            bool hasStart = Start.TryResolve(grid, positionMapper, out Position start);
            bool hasEnd = End.TryResolve(grid, positionMapper, out Position end);
            if (!hasStart || !hasEnd)
            {
                range = default;
                return false;
            }

            range = new SourceGrid.Range(start, end);
            return true;
        }
    }

    internal sealed class BrailleGridState
    {
        public BrailleGridCellBookmark ActiveCell { get; }
        public IReadOnlyList<BrailleGridRangeBookmark> SelectionRanges { get; }

        public BrailleGridState(BrailleDocument sourceDoc, BrailleDocument snapshotDoc, SourceGrid.Grid grid, BrailleGridPositionMapper positionMapper)
        {
            ActiveCell = BrailleGridCellBookmark.Capture(sourceDoc, snapshotDoc, grid, positionMapper, grid.Selection.ActivePosition);

            var selectionRanges = new List<BrailleGridRangeBookmark>();
            foreach (var range in grid.Selection.GetSelectionRegion())
            {
                selectionRanges.Add(new BrailleGridRangeBookmark(
                    BrailleGridCellBookmark.Capture(sourceDoc, snapshotDoc, grid, positionMapper, range.Start),
                    BrailleGridCellBookmark.Capture(sourceDoc, snapshotDoc, grid, positionMapper, range.End)));
            }
            SelectionRanges = selectionRanges;
        }

        public void Restore(SourceGrid.Grid grid, BrailleGridPositionMapper positionMapper)
        {
            if (ActiveCell.TryResolve(grid, positionMapper, out Position activePosition))
            {
                grid.Selection.SelectCell(activePosition, true);
                grid.Selection.Focus(activePosition, true);
            }

            grid.Selection.ResetSelection(false);
            foreach (var rangeBookmark in SelectionRanges)
            {
                if (rangeBookmark.TryResolve(grid, positionMapper, out SourceGrid.Range range))
                {
                    grid.Selection.SelectRange(range, true);
                }
            }

            if (ActiveCell.TryResolve(grid, positionMapper, out activePosition))
            {
                grid.Selection.SelectCell(activePosition, true);
                grid.Selection.Focus(activePosition, false);
            }
        }
    }

    internal sealed class BrailleEditMemento
    {
        public string? Operation { get; set; }
        public BrailleDocument BrailleDoc { get; }
        public bool IsDirty { get; }
        public BrailleGridState GridState { get; }

        /// <summary>
        /// 此建構子會把傳入的 BrailleDocument 複製成一份新的 instance，保存於內部，
        /// 並以複製後的文件 identity 建立可還原的 grid state。
        /// </summary>
        /// <param name="operation">為了執行甚麼命令而建立此 memento 物件。</param>
        /// <param name="doc">BrailleDocument 物件。</param>
        /// <param name="isDirty">文件是否修改過，且尚未儲存。</param>
        /// <param name="grid">網格控制項。</param>
        /// <param name="positionMapper">位置對應器。</param>
        public BrailleEditMemento(string? operation, BrailleDocument doc, bool isDirty, SourceGrid.Grid grid, BrailleGridPositionMapper positionMapper)
        {
            Operation = operation;
            BrailleDoc = doc.DeepCopy();
            IsDirty = isDirty;
            GridState = new BrailleGridState(doc, BrailleDoc, grid, positionMapper);
        }
    }
}
