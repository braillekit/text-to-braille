using BrailleToolkit;
using EasyBrailleEdit.DualEdit;
using SourceGrid;
using Xunit;

namespace EasyBrailleEdit.Tests.DualEdit
{
    public class BrailleGridStateTests
    {
        [StaFact]
        public void GridState_ShouldRestoreActivePositionAndSelectionUsingSnapshotIdentity()
        {
            var sourceDoc = CreateDocument();
            var sourceGrid = CreateGrid(sourceDoc);
            var sourceMapper = new BrailleGridPositionMapper(sourceDoc, sourceGrid);

            var activePosition = new Position(2, 2);
            var selectionStart = new Position(1, 2);
            var selectionEnd = new Position(3, 3);

            var snapshotDoc = sourceDoc.DeepCopy();
            var activeBookmark = BrailleGridCellBookmark.Capture(sourceDoc, snapshotDoc, sourceGrid, sourceMapper, activePosition);
            var selectionStartBookmark = BrailleGridCellBookmark.Capture(sourceDoc, snapshotDoc, sourceGrid, sourceMapper, selectionStart);
            var selectionEndBookmark = BrailleGridCellBookmark.Capture(sourceDoc, snapshotDoc, sourceGrid, sourceMapper, selectionEnd);

            var snapshotGrid = CreateGrid(snapshotDoc);
            var snapshotMapper = new BrailleGridPositionMapper(snapshotDoc, snapshotGrid);

            Assert.True(activeBookmark.TryResolve(snapshotGrid, snapshotMapper, out Position resolvedActivePosition));
            Assert.Equal(activePosition.Row, resolvedActivePosition.Row);
            Assert.Equal(activePosition.Column, resolvedActivePosition.Column);

            Assert.True(selectionStartBookmark.TryResolve(snapshotGrid, snapshotMapper, out Position resolvedSelectionStart));
            Assert.Equal(1, resolvedSelectionStart.Row);
            Assert.Equal(2, resolvedSelectionStart.Column);

            Assert.True(selectionEndBookmark.TryResolve(snapshotGrid, snapshotMapper, out Position resolvedSelectionEnd));
            Assert.Equal(3, resolvedSelectionEnd.Row);
            Assert.Equal(3, resolvedSelectionEnd.Column);
        }

        private static BrailleDocument CreateDocument()
        {
            var doc = new BrailleDocument
            {
                CellsPerLine = 6
            };

            var line = new BrailleLine();
            line.AddWord(new BrailleWord("甲", "01"));
            line.AddWord(new BrailleWord("乙", "0102"));
            line.AddWord(new BrailleWord("丙", "04"));
            doc.AddLine(line);

            return doc;
        }

        private static Grid CreateGrid(BrailleDocument doc)
        {
            var grid = new Grid
            {
                FixedRows = 1,
                FixedColumns = 1
            };

            grid.Redim(doc.LineCount * 3 + grid.FixedRows, doc.CellsPerLine + grid.FixedColumns);
            grid[0, 0] = new SourceGrid.Cells.Header();

            for (int lineIdx = 0; lineIdx < doc.LineCount; lineIdx++)
            {
                int row = grid.FixedRows + (lineIdx * 3);
                int col = grid.FixedColumns;
                var line = doc.Lines[lineIdx];

                foreach (var word in line.Words)
                {
                    int span = Math.Max(1, word.CellCount);

                    grid[row, col] = new SourceGrid.Cells.Cell(word.Text);
                    grid[row, col].Tag = word;
                    grid[row, col].ColumnSpan = span;

                    grid[row + 1, col] = new SourceGrid.Cells.Cell(word.Text);
                    grid[row + 1, col].Tag = word;
                    grid[row + 1, col].ColumnSpan = span;

                    grid[row + 2, col] = new SourceGrid.Cells.Cell(word.PhoneticCode);
                    grid[row + 2, col].Tag = word;
                    grid[row + 2, col].ColumnSpan = span;

                    col += span;
                }
            }

            return grid;
        }
    }
}
