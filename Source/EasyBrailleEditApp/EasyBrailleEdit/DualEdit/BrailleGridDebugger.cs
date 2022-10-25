using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrailleToolkit;
using Huanlin.Windows.Forms;

namespace EasyBrailleEdit.DualEdit
{
    internal class BrailleGridDebugger
    {
        private BrailleGridController _controller;
        private SourceGrid.Grid _grid;

        public BrailleGridDebugger(BrailleGridController controller)
        {
            _controller = controller;
            _grid = _controller.Grid;
        }

        public void ShouldEveryGridCellHasBrailleWord()
        {
            int row = _grid.FixedRows;            

            while (row < _grid.RowsCount)
            {
                int col = _grid.FixedColumns;
                while (col < _grid.ColumnsCount)
                {
                    var cell = _grid[row, col];
                    if (cell != null)
                    {
                        var brWord = cell.Tag as BrailleWord;
                        if (brWord == null)
                        {
                            MsgBoxHelper.ShowError($"儲存格 [{row},{col}] 的 BrailleWord 為 null!");
                        }
                    }
                    col++;
                }
                row++;
            }
        }
    }
}
