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

        public BrailleEditMemento Undo(BrailleEditMemento currentState)
        {
            if (CanUndo())
            {
                var lastState = _undoStack.Pop();
                currentState.Operation = lastState.Operation;
                _redoStack.Push(currentState);

                OnUndoBufferChanged();
                return lastState;
            }
            else
                return null;
        }

        public BrailleEditMemento Redo(BrailleEditMemento currentState)
        {
            if (CanRedo())
            {
                var lastState = _redoStack.Pop();
                currentState.Operation = lastState.Operation;
                _undoStack.Push(currentState);

                OnUndoBufferChanged();
                return lastState;
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

                OnUndoBufferChanged();
            }
        }
        public bool CanUndo() => _undoStack.Count > 0;

        public bool CanRedo() => _redoStack.Count > 0;

        public void Reset()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }

        public List<string> GetUndoableOperations()
        {
            var result = new List<string>(_undoStack.Count);
            var undoList = _undoStack.ToList();
            foreach (var item in undoList)
            {
                result.Add(item.Operation);
            }
            return result;
        }

        public event EventHandler<EventArgs> UndoBufferChanged;

        private void OnUndoBufferChanged()
        {
            UndoBufferChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
