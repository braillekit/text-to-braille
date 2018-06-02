using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyBrailleEdit.DualEdit
{
    internal class UndoRedoManager
    {
        private Stack<BrailleEditMemento> _undoStack = new Stack<BrailleEditMemento>();
        private Stack<BrailleEditMemento> _redoStack = new Stack<BrailleEditMemento>();

        public BrailleEditMemento GetUndoMemento()
        {
            if (_undoStack.Count > 0)
            {
                var m = _undoStack.Pop();
                _redoStack.Push(m);
                return m;
            }
            else
                return null;
        }

        public BrailleEditMemento GetRedoMemento()
        {
            if (_redoStack.Count > 0)
            {
                var m = _redoStack.Pop();
                _undoStack.Push(m);
                return m;
            }
            else
                return null;
        }
        public void SaveMementoForUndo(BrailleEditMemento memento)
        {
            if (memento != null)
            {
                _undoStack.Push(memento);
                _redoStack.Clear();
            }
        }
        public bool CanUndo()
        {
            return _undoStack.Count > 0;

        }
        public bool CanRedo()
        {
            return _redoStack.Count > 0;
        }

        public void Reset()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }
    }
}
