using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

namespace BrailleToolkit.Benchmarks;

/// <summary>
/// 驗證 BrailleCell prototype 的儲存策略與型別表示方式。
/// 這組 benchmark 不直接跑正式轉點字流程，而是模擬目前熱路徑常見的 cell 操作：
/// append、prepend、scan、copy。
/// </summary>
[MemoryDiagnoser]
[SimpleJob(RunStrategy.Throughput, warmupCount: 3, iterationCount: 10)]
public class BrailleCellStoragePrototypeBenchmarks
{
    private const byte CapitalCellValue = 0x20;
    private const byte DigitCellValue = 0x3C;
    private const int RoundCount = 96;

    private WordPlan[] _plans = null!;

    private static string GetTestDataPath(string fileName)
    {
        string dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!;
        return Path.Combine(dir, "TestData", fileName);
    }

    [GlobalSetup]
    public void Setup()
    {
        var lines = File.ReadAllLines(GetTestDataPath("BenchmarkText_Chinese.txt"))
            .Concat(File.ReadAllLines(GetTestDataPath("BenchmarkText_English.txt")))
            .Concat(File.ReadAllLines(GetTestDataPath("BenchmarkText_Mixed.txt")));

        var plans = new List<WordPlan>();
        foreach (string line in lines)
        {
            foreach (char ch in line)
            {
                if (char.IsWhiteSpace(ch))
                {
                    continue;
                }
                plans.Add(CreatePlan(ch));
            }
        }
        _plans = plans.ToArray();
    }

    [Benchmark(Description = "Prototype: class flyweight + List")]
    public int ClassFlyweightList()
    {
        int checksum = 0;
        var digitCell = PrototypeClassCell.GetInstance(DigitCellValue);

        for (int round = 0; round < RoundCount; round++)
        {
            var words = new List<ClassListWord>(_plans.Length);
            foreach (WordPlan plan in _plans)
            {
                var word = new ClassListWord();
                PopulateClassWord(word, plan);
                words.Add(word);
            }

            for (int i = 0; i < words.Count; i++)
            {
                var word = words[i];
                if (!word.FirstEquals(digitCell))
                {
                    checksum += word.FirstValue;
                }
                if (_plans[i].ScanPrefixCells)
                {
                    checksum += word.CountValue(PrototypeClassCell.GetInstance(_plans[i].PrefixValue));
                }
            }

            var copies = new List<ClassListWord>(words.Count);
            foreach (ClassListWord word in words)
            {
                copies.Add(word.Clone());
            }
            checksum += copies.Count;
        }

        return checksum;
    }

    [Benchmark(Description = "Prototype: plain readonly struct + List")]
    public int PlainReadonlyStructList()
    {
        int checksum = 0;
        var digitCell = PrototypePlainCell.GetInstance(DigitCellValue);

        for (int round = 0; round < RoundCount; round++)
        {
            var words = new List<PlainListWord>(_plans.Length);
            foreach (WordPlan plan in _plans)
            {
                var word = new PlainListWord();
                PopulatePlainListWord(word, plan);
                words.Add(word);
            }

            for (int i = 0; i < words.Count; i++)
            {
                var word = words[i];
                if (!word.FirstEquals(digitCell))
                {
                    checksum += word.FirstValue;
                }
                if (_plans[i].ScanPrefixCells)
                {
                    checksum += word.CountValue(PrototypePlainCell.GetInstance(_plans[i].PrefixValue));
                }
            }

            var copies = new List<PlainListWord>(words.Count);
            foreach (PlainListWord word in words)
            {
                copies.Add(word.Clone());
            }
            checksum += copies.Count;
        }

        return checksum;
    }

    [Benchmark(Description = "Prototype: readonly record struct + List")]
    public int ReadonlyRecordStructList()
    {
        int checksum = 0;
        var digitCell = PrototypeRecordCell.GetInstance(DigitCellValue);

        for (int round = 0; round < RoundCount; round++)
        {
            var words = new List<RecordListWord>(_plans.Length);
            foreach (WordPlan plan in _plans)
            {
                var word = new RecordListWord();
                PopulateRecordListWord(word, plan);
                words.Add(word);
            }

            for (int i = 0; i < words.Count; i++)
            {
                var word = words[i];
                if (!word.FirstEquals(digitCell))
                {
                    checksum += word.FirstValue;
                }
                if (_plans[i].ScanPrefixCells)
                {
                    checksum += word.CountValue(PrototypeRecordCell.GetInstance(_plans[i].PrefixValue));
                }
            }

            var copies = new List<RecordListWord>(words.Count);
            foreach (RecordListWord word in words)
            {
                copies.Add(word.Clone());
            }
            checksum += copies.Count;
        }

        return checksum;
    }

    [Benchmark(Description = "Prototype: plain readonly struct + deque buffer")]
    public int PlainReadonlyStructDequeBuffer()
    {
        int checksum = 0;
        var digitCell = PrototypePlainCell.GetInstance(DigitCellValue);

        for (int round = 0; round < RoundCount; round++)
        {
            var words = new List<PlainBufferWord>(_plans.Length);
            foreach (WordPlan plan in _plans)
            {
                var word = new PlainBufferWord();
                PopulatePlainBufferWord(word, plan);
                words.Add(word);
            }

            for (int i = 0; i < words.Count; i++)
            {
                var word = words[i];
                if (!word.FirstEquals(digitCell))
                {
                    checksum += word.FirstValue;
                }
                if (_plans[i].ScanPrefixCells)
                {
                    checksum += word.CountValue(PrototypePlainCell.GetInstance(_plans[i].PrefixValue));
                }
            }

            var copies = new List<PlainBufferWord>(words.Count);
            foreach (PlainBufferWord word in words)
            {
                copies.Add(word.Clone());
            }
            checksum += copies.Count;
        }

        return checksum;
    }

    private static WordPlan CreatePlan(char ch)
    {
        int seed = ch & 0x3F;

        byte baseCellCount;
        byte prefixCount = 0;
        byte prefixValue = CapitalCellValue;
        bool scanPrefixCells = false;

        if (char.IsDigit(ch))
        {
            baseCellCount = 1;
            prefixCount = 1;
            prefixValue = DigitCellValue;
        }
        else if (char.IsAsciiLetter(ch))
        {
            baseCellCount = 1;
            if (!char.IsLower(ch))
            {
                prefixCount = 1;
                prefixValue = CapitalCellValue;
                scanPrefixCells = true;
            }
        }
        else if (ch > 127)
        {
            baseCellCount = char.IsPunctuation(ch) ? (byte)1 : (byte)2;
        }
        else
        {
            baseCellCount = 1;
        }

        if (!scanPrefixCells && prefixCount == 0 && (seed % 11 == 0))
        {
            prefixCount = 1;
            prefixValue = CapitalCellValue;
            scanPrefixCells = true;
        }

        return new WordPlan(baseCellCount, prefixCount, prefixValue, scanPrefixCells, (byte)seed);
    }

    private static void PopulateClassWord(ClassListWord word, WordPlan plan)
    {
        for (int i = 0; i < plan.BaseCellCount; i++)
        {
            word.Append(PrototypeClassCell.GetInstance(GetCellValue(plan.Seed, i)));
        }
        for (int i = 0; i < plan.PrefixCount; i++)
        {
            word.Prepend(PrototypeClassCell.GetInstance(plan.PrefixValue));
        }
    }

    private static void PopulatePlainListWord(PlainListWord word, WordPlan plan)
    {
        for (int i = 0; i < plan.BaseCellCount; i++)
        {
            word.Append(PrototypePlainCell.GetInstance(GetCellValue(plan.Seed, i)));
        }
        for (int i = 0; i < plan.PrefixCount; i++)
        {
            word.Prepend(PrototypePlainCell.GetInstance(plan.PrefixValue));
        }
    }

    private static void PopulateRecordListWord(RecordListWord word, WordPlan plan)
    {
        for (int i = 0; i < plan.BaseCellCount; i++)
        {
            word.Append(PrototypeRecordCell.GetInstance(GetCellValue(plan.Seed, i)));
        }
        for (int i = 0; i < plan.PrefixCount; i++)
        {
            word.Prepend(PrototypeRecordCell.GetInstance(plan.PrefixValue));
        }
    }

    private static void PopulatePlainBufferWord(PlainBufferWord word, WordPlan plan)
    {
        for (int i = 0; i < plan.BaseCellCount; i++)
        {
            word.Append(PrototypePlainCell.GetInstance(GetCellValue(plan.Seed, i)));
        }
        for (int i = 0; i < plan.PrefixCount; i++)
        {
            word.Prepend(PrototypePlainCell.GetInstance(plan.PrefixValue));
        }
    }

    private static byte GetCellValue(byte seed, int offset)
    {
        return (byte)((seed + (offset * 7) + 1) & 0x3F);
    }

    private readonly record struct WordPlan(byte BaseCellCount, byte PrefixCount, byte PrefixValue, bool ScanPrefixCells, byte Seed);

    private sealed class PrototypeClassCell
    {
        private static readonly PrototypeClassCell[] s_AllCells = CreateAllCells();

        private PrototypeClassCell(byte value)
        {
            Value = value;
        }

        public byte Value { get; }

        public static PrototypeClassCell GetInstance(byte value)
        {
            return s_AllCells[value];
        }

        public override bool Equals(object? obj)
        {
            if (base.Equals(obj))
            {
                return true;
            }
            return obj is PrototypeClassCell other && Value == other.Value;
        }

        public override int GetHashCode()
        {
            return Value;
        }

        private static PrototypeClassCell[] CreateAllCells()
        {
            var cells = new PrototypeClassCell[256];
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i] = new PrototypeClassCell((byte)i);
            }
            return cells;
        }
    }

    private readonly struct PrototypePlainCell : IEquatable<PrototypePlainCell>
    {
        private static readonly PrototypePlainCell[] s_AllCells = CreateAllCells();

        public PrototypePlainCell(byte value)
        {
            Value = value;
        }

        public byte Value { get; }

        public static PrototypePlainCell GetInstance(byte value)
        {
            return s_AllCells[value];
        }

        public bool Equals(PrototypePlainCell other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object? obj)
        {
            return obj is PrototypePlainCell other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value;
        }

        private static PrototypePlainCell[] CreateAllCells()
        {
            var cells = new PrototypePlainCell[256];
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i] = new PrototypePlainCell((byte)i);
            }
            return cells;
        }
    }

    private readonly record struct PrototypeRecordCell(byte Value)
    {
        private static readonly PrototypeRecordCell[] s_AllCells = CreateAllCells();

        public static PrototypeRecordCell GetInstance(byte value)
        {
            return s_AllCells[value];
        }

        private static PrototypeRecordCell[] CreateAllCells()
        {
            var cells = new PrototypeRecordCell[256];
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i] = new PrototypeRecordCell((byte)i);
            }
            return cells;
        }
    }

    private sealed class ClassListWord
    {
        private readonly List<PrototypeClassCell> _cells = new(4);

        public byte FirstValue => _cells[0].Value;

        public void Append(PrototypeClassCell cell) => _cells.Add(cell);

        public void Prepend(PrototypeClassCell cell) => _cells.Insert(0, cell);

        public bool FirstEquals(PrototypeClassCell cell) => _cells[0].Equals(cell);

        public int CountValue(PrototypeClassCell cell)
        {
            int count = 0;
            foreach (PrototypeClassCell item in _cells)
            {
                if (item.Equals(cell))
                {
                    count++;
                }
            }
            return count;
        }

        public ClassListWord Clone()
        {
            var clone = new ClassListWord();
            foreach (PrototypeClassCell cell in _cells)
            {
                clone.Append(cell);
            }
            return clone;
        }
    }

    private sealed class PlainListWord
    {
        private readonly List<PrototypePlainCell> _cells = new(4);

        public byte FirstValue => _cells[0].Value;

        public void Append(PrototypePlainCell cell) => _cells.Add(cell);

        public void Prepend(PrototypePlainCell cell) => _cells.Insert(0, cell);

        public bool FirstEquals(PrototypePlainCell cell) => _cells[0].Equals(cell);

        public int CountValue(PrototypePlainCell cell)
        {
            int count = 0;
            foreach (PrototypePlainCell item in _cells)
            {
                if (item.Equals(cell))
                {
                    count++;
                }
            }
            return count;
        }

        public PlainListWord Clone()
        {
            var clone = new PlainListWord();
            foreach (PrototypePlainCell cell in _cells)
            {
                clone.Append(cell);
            }
            return clone;
        }
    }

    private sealed class RecordListWord
    {
        private readonly List<PrototypeRecordCell> _cells = new(4);

        public byte FirstValue => _cells[0].Value;

        public void Append(PrototypeRecordCell cell) => _cells.Add(cell);

        public void Prepend(PrototypeRecordCell cell) => _cells.Insert(0, cell);

        public bool FirstEquals(PrototypeRecordCell cell) => _cells[0].Equals(cell);

        public int CountValue(PrototypeRecordCell cell)
        {
            int count = 0;
            foreach (PrototypeRecordCell item in _cells)
            {
                if (item.Equals(cell))
                {
                    count++;
                }
            }
            return count;
        }

        public RecordListWord Clone()
        {
            var clone = new RecordListWord();
            foreach (PrototypeRecordCell cell in _cells)
            {
                clone.Append(cell);
            }
            return clone;
        }
    }

    private sealed class PlainBufferWord
    {
        private PrototypePlainCellBuffer _cells = new(8, 4);

        public byte FirstValue => _cells.First.Value;

        public void Append(PrototypePlainCell cell) => _cells.Append(cell);

        public void Prepend(PrototypePlainCell cell) => _cells.Prepend(cell);

        public bool FirstEquals(PrototypePlainCell cell) => _cells.First.Equals(cell);

        public int CountValue(PrototypePlainCell cell)
        {
            int count = 0;
            foreach (PrototypePlainCell item in _cells.AsSpan())
            {
                if (item.Equals(cell))
                {
                    count++;
                }
            }
            return count;
        }

        public PlainBufferWord Clone()
        {
            var clone = new PlainBufferWord();
            clone._cells = _cells.Clone();
            return clone;
        }
    }

    private struct PrototypePlainCellBuffer
    {
        private PrototypePlainCell[] _items;
        private int _start;
        private int _count;

        public PrototypePlainCellBuffer(int capacity, int headroom)
        {
            _items = new PrototypePlainCell[capacity];
            _start = headroom;
            _count = 0;
        }

        public PrototypePlainCell First => _items[_start];

        public void Append(PrototypePlainCell cell)
        {
            EnsureTailRoom();
            _items[_start + _count] = cell;
            _count++;
        }

        public void Prepend(PrototypePlainCell cell)
        {
            EnsureHeadRoom();
            _start--;
            _items[_start] = cell;
            _count++;
        }

        public ReadOnlySpan<PrototypePlainCell> AsSpan()
        {
            return _items.AsSpan(_start, _count);
        }

        public PrototypePlainCellBuffer Clone()
        {
            var clone = new PrototypePlainCellBuffer(Math.Max(_count + 8, 8), 4);
            foreach (PrototypePlainCell cell in AsSpan())
            {
                clone.Append(cell);
            }
            return clone;
        }

        private void EnsureHeadRoom()
        {
            if (_start == 0)
            {
                Grow();
            }
        }

        private void EnsureTailRoom()
        {
            if (_start + _count >= _items.Length)
            {
                Grow();
            }
        }

        private void Grow()
        {
            int newCapacity = _items.Length * 2;
            if (newCapacity < 8)
            {
                newCapacity = 8;
            }

            int newStart = newCapacity / 4;
            var newItems = new PrototypePlainCell[newCapacity];
            AsSpan().CopyTo(newItems.AsSpan(newStart));
            _items = newItems;
            _start = newStart;
        }
    }
}
