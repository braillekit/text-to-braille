using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrailleToolkit;
using SourceGrid;
using SourceGrid.Selection;

namespace EasyBrailleEdit.DualEdit
{

    internal class BrailleGridState
    {
        public Position ActivePosition { get; set; }
        public RangeRegion SelectionRegion { get; set; }

        public BrailleGridState(SourceGrid.Grid grid)
        {
            ActivePosition = new Position(grid.Selection.ActivePosition.Row, grid.Selection.ActivePosition.Column);
            SelectionRegion = new RangeRegion(grid.Selection.GetSelectionRegion());
        }
    }

    internal class BrailleEditMemento
    {
        public string Operation { get; set; }
        public BrailleDocument BrailleDoc { get; }
        public bool IsDirty { get; }
        public BrailleGridState GridState { get; }

        /// <summary>
        /// 此建構子會把傳入的 BrailleDocument 複製成一份新的 instance，保存於內部。
        /// </summary>
        /// <param name="command">為了執行甚麼命令而建立此 memento 物件。</param>
        /// <param name="doc">BrailleDocument 物件。</param>
        /// <param name="isDirty">文件是否修改過，且尚未儲存。</param>
        /// <param name="gridState">網格狀態。</param>
        public BrailleEditMemento(string operation, BrailleDocument doc, bool isDirty, BrailleGridState gridState)
        {
            Operation = operation;
            BrailleDoc = doc.DeepCopy();
            IsDirty = isDirty;
            GridState = gridState;
        }

    }
}
