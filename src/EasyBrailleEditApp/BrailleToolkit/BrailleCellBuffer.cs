using System;

namespace BrailleToolkit
{
    /// <summary>
    /// 供 builder 階段使用的點字方緩衝區。
    /// </summary>
    internal struct BrailleCellBuffer
    {
        private BrailleCell[]? _items;
        private int _start;
        private int _count;
        private readonly int _headroom;

        public BrailleCellBuffer(int capacity = 8, int headroom = 4)
        {
            int actualCapacity = Math.Max(capacity, 8);
            _items = new BrailleCell[actualCapacity];
            _headroom = Math.Max(headroom, 0);
            _start = Math.Min(_headroom, actualCapacity - 1);
            _count = 0;
        }

        public int Count
        {
            get { return _count; }
        }

        public void Clear()
        {
            EnsureInitialized();
            _count = 0;
            _start = Math.Min(_headroom, _items!.Length - 1);
        }

        public void Append(BrailleCell cell)
        {
            EnsureInitialized();
            EnsureTailRoom(1);
            _items![_start + _count] = cell;
            _count++;
        }

        public void AppendRange(ReadOnlySpan<BrailleCell> cells)
        {
            if (cells.Length < 1)
                return;

            EnsureInitialized();
            EnsureTailRoom(cells.Length);
            cells.CopyTo(_items!.AsSpan(_start + _count));
            _count += cells.Length;
        }

        public void Prepend(BrailleCell cell)
        {
            EnsureInitialized();
            EnsureHeadRoom();
            _start--;
            _items![_start] = cell;
            _count++;
        }

        public ReadOnlySpan<BrailleCell> AsSpan()
        {
            if (_items == null || _count < 1)
                return ReadOnlySpan<BrailleCell>.Empty;
            return _items.AsSpan(_start, _count);
        }

        public BrailleCell[] ToArray()
        {
            return AsSpan().ToArray();
        }

        private void EnsureInitialized()
        {
            if (_items != null)
                return;

            int actualCapacity = Math.Max((_headroom + 1) * 2, 8);
            _items = new BrailleCell[actualCapacity];
            _start = Math.Min(_headroom, actualCapacity - 1);
            _count = 0;
        }

        private void EnsureHeadRoom()
        {
            if (_start == 0)
            {
                Grow();
            }
        }

        private void EnsureTailRoom(int additionalCount)
        {
            while ((_start + _count + additionalCount) > _items!.Length)
            {
                Grow();
            }
        }

        private void Grow()
        {
            int newCapacity = (_items == null) ? 8 : _items.Length * 2;
            if (newCapacity < 8)
            {
                newCapacity = 8;
            }

            int newStart = Math.Max(newCapacity / 4, _headroom);
            var newItems = new BrailleCell[newCapacity];
            AsSpan().CopyTo(newItems.AsSpan(newStart));
            _items = newItems;
            _start = newStart;
        }
    }
}
