using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

namespace BrailleToolkit.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(RunStrategy.Throughput, warmupCount: 3, iterationCount: 10)]
public class BrailleWordBuilderPrototypeBenchmarks
{
    private const byte CapitalCellValue = 0x20;
    private const byte DigitCellValue = 0x3C;
    private const int RoundCount = 48;

    private LinePlan[] _linePlans = null!;

    private static string GetTestDataPath(string fileName)
    {
        string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        return Path.Combine(dir, "TestData", fileName);
    }

    [GlobalSetup]
    public void Setup()
    {
        var linePlans = new List<LinePlan>();
        LoadPlans(linePlans, "BenchmarkText_Chinese.txt");
        LoadPlans(linePlans, "BenchmarkText_English.txt");
        LoadPlans(linePlans, "BenchmarkText_Mixed.txt");
        _linePlans = linePlans.ToArray();
    }

    [Benchmark(Description = "Prototype: current BrailleWord + BrailleLine")]
    public int CurrentBrailleWordAndLinePipeline()
    {
        int checksum = 0;
        var digitCell = BrailleCell.GetInstance(DigitCellValue);

        for (int round = 0; round < RoundCount; round++)
        {
            foreach (LinePlan linePlan in _linePlans)
            {
                var line = new BrailleLine();
                for (int i = 0; i < linePlan.Words.Length; i++)
                {
                    WordPlan plan = linePlan.Words[i];
                    var word = new BrailleWord(plan.Text);
                    PopulateCurrentWord(word, plan);
                    line.AddWord(word);
                }

                checksum += ConsumeCurrentLine(line, linePlan, digitCell);
            }
        }

        return checksum;
    }

    [Benchmark(Description = "Prototype: BrailleWordBuilder + BrailleCellBuffer")]
    public int BrailleWordBuilderCellBufferPipeline()
    {
        int checksum = 0;
        var digitCell = BrailleCell.GetInstance(DigitCellValue);

        for (int round = 0; round < RoundCount; round++)
        {
            foreach (LinePlan linePlan in _linePlans)
            {
                var words = new List<PrototypeBrailleWord>(linePlan.Words.Length);

                for (int i = 0; i < linePlan.Words.Length; i++)
                {
                    WordPlan plan = linePlan.Words[i];
                    var builder = new PrototypeBrailleWordBuilder(plan.Text);
                    PopulatePrototypeWord(builder, plan);
                    words.Add(builder.Build());
                }

                checksum += ConsumePrototypeWords(words, linePlan, digitCell);

                var flattenedCells = new List<BrailleCell>(linePlan.TotalCellCount);
                for (int i = 0; i < words.Count; i++)
                {
                    ReadOnlySpan<BrailleCell> cells = words[i].Cells;
                    for (int j = 0; j < cells.Length; j++)
                    {
                        flattenedCells.Add(cells[j]);
                    }
                }

                checksum += ConsumeFlattenedCells(flattenedCells);
            }
        }

        return checksum;
    }

    [Benchmark(Description = "Prototype: BrailleWordBuilder + BrailleCellBuffer + line buffer")]
    public int BrailleWordBuilderAndLineBufferPipeline()
    {
        int checksum = 0;
        var digitCell = BrailleCell.GetInstance(DigitCellValue);

        for (int round = 0; round < RoundCount; round++)
        {
            foreach (LinePlan linePlan in _linePlans)
            {
                var words = new List<PrototypeBrailleWord>(linePlan.Words.Length);

                for (int i = 0; i < linePlan.Words.Length; i++)
                {
                    WordPlan plan = linePlan.Words[i];
                    var builder = new PrototypeBrailleWordBuilder(plan.Text);
                    PopulatePrototypeWord(builder, plan);
                    words.Add(builder.Build());
                }

                checksum += ConsumePrototypeWords(words, linePlan, digitCell);

                var lineBuffer = new BrailleCellBuffer(Math.Max(linePlan.TotalCellCount + 4, 8), 0);
                for (int i = 0; i < words.Count; i++)
                {
                    lineBuffer.AppendRange(words[i].Cells);
                }

                checksum += ConsumeFlattenedCells(lineBuffer.AsSpan());
            }
        }

        return checksum;
    }

    private static void LoadPlans(List<LinePlan> linePlans, string fileName)
    {
        string[] lines = File.ReadAllLines(GetTestDataPath(fileName));
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var words = new List<WordPlan>(line.Length);
            int totalCellCount = 0;
            for (int j = 0; j < line.Length; j++)
            {
                WordPlan word = CreatePlan(line[j]);
                words.Add(word);
                totalCellCount += word.TotalCellCount;
            }

            linePlans.Add(new LinePlan(words.ToArray(), totalCellCount));
        }
    }

    private static WordPlan CreatePlan(char ch)
    {
        int seed = ch & 0x3F;
        byte baseCellCount;
        byte prefixCount = 0;
        byte prefixValue = CapitalCellValue;
        bool scanPrefixCells = false;

        if (char.IsWhiteSpace(ch))
        {
            baseCellCount = 1;
        }
        else if (char.IsDigit(ch))
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

        if (!scanPrefixCells && prefixCount == 0 && seed % 11 == 0)
        {
            prefixCount = 1;
            prefixValue = CapitalCellValue;
            scanPrefixCells = true;
        }

        return new WordPlan(ch.ToString(), baseCellCount, prefixCount, prefixValue, scanPrefixCells, (byte)seed);
    }

    private static void PopulateCurrentWord(BrailleWord word, WordPlan plan)
    {
        for (int i = 0; i < plan.BaseCellCount; i++)
        {
            word.Cells.Add(BrailleCell.GetInstance(GetCellValue(plan.Seed, i)));
        }

        for (int i = 0; i < plan.PrefixCount; i++)
        {
            word.Cells.Insert(0, BrailleCell.GetInstance(plan.PrefixValue));
        }
    }

    private static void PopulatePrototypeWord(PrototypeBrailleWordBuilder builder, WordPlan plan)
    {
        for (int i = 0; i < plan.BaseCellCount; i++)
        {
            builder.Append(BrailleCell.GetInstance(GetCellValue(plan.Seed, i)));
        }

        for (int i = 0; i < plan.PrefixCount; i++)
        {
            builder.Prepend(BrailleCell.GetInstance(plan.PrefixValue));
        }
    }

    private static int ConsumeCurrentLine(BrailleLine line, LinePlan linePlan, BrailleCell digitCell)
    {
        int checksum = 0;

        for (int i = 0; i < line.WordCount; i++)
        {
            BrailleWord word = line[i];
            if (word.CellCount == 0)
            {
                continue;
            }

            BrailleCell firstCell = word.Cells[0];
            if (!firstCell.Equals(digitCell))
            {
                checksum += firstCell.Value;
            }

            if (linePlan.Words[i].ScanPrefixCells)
            {
                checksum += CountCells(word.Cells, linePlan.Words[i].PrefixValue);
            }
        }

        checksum += line.CellCount;
        checksum += ConsumeFlattenedCells(line.GetBrailleCells());
        return checksum;
    }

    private static int ConsumePrototypeWords(List<PrototypeBrailleWord> words, LinePlan linePlan, BrailleCell digitCell)
    {
        int checksum = 0;
        int totalCellCount = 0;

        for (int i = 0; i < words.Count; i++)
        {
            PrototypeBrailleWord word = words[i];
            ReadOnlySpan<BrailleCell> cells = word.Cells;
            if (cells.Length == 0)
            {
                continue;
            }

            BrailleCell firstCell = cells[0];
            if (!firstCell.Equals(digitCell))
            {
                checksum += firstCell.Value;
            }

            if (linePlan.Words[i].ScanPrefixCells)
            {
                checksum += CountCells(cells, linePlan.Words[i].PrefixValue);
            }

            totalCellCount += cells.Length;
        }

        checksum += totalCellCount;
        return checksum;
    }

    private static int ConsumeFlattenedCells(List<BrailleCell> cells)
    {
        if (cells.Count == 0)
        {
            return 0;
        }

        return cells.Count + cells[0].Value + cells[cells.Count - 1].Value;
    }

    private static int ConsumeFlattenedCells(ReadOnlySpan<BrailleCell> cells)
    {
        if (cells.Length == 0)
        {
            return 0;
        }

        return cells.Length + cells[0].Value + cells[cells.Length - 1].Value;
    }

    private static int CountCells(List<BrailleCell> cells, byte value)
    {
        int count = 0;
        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i].Value == value)
            {
                count++;
            }
        }
        return count;
    }

    private static int CountCells(ReadOnlySpan<BrailleCell> cells, byte value)
    {
        int count = 0;
        for (int i = 0; i < cells.Length; i++)
        {
            if (cells[i].Value == value)
            {
                count++;
            }
        }
        return count;
    }

    private static byte GetCellValue(byte seed, int offset)
    {
        return (byte)((seed + (offset * 7) + 1) & 0x3F);
    }

    private readonly record struct LinePlan(WordPlan[] Words, int TotalCellCount);

    private readonly record struct WordPlan(string Text, byte BaseCellCount, byte PrefixCount, byte PrefixValue, bool ScanPrefixCells, byte Seed)
    {
        public int TotalCellCount => BaseCellCount + PrefixCount;
    }

    private sealed class PrototypeBrailleWordBuilder
    {
        private readonly string _text;
        private BrailleCellBuffer _cells;

        public PrototypeBrailleWordBuilder(string text)
        {
            _text = text;
            _cells = new BrailleCellBuffer(8, 4);
        }

        public void Append(BrailleCell cell)
        {
            _cells.Append(cell);
        }

        public void Prepend(BrailleCell cell)
        {
            _cells.Prepend(cell);
        }

        public PrototypeBrailleWord Build()
        {
            return new PrototypeBrailleWord(_text, _cells.ToArray());
        }
    }

    private sealed class PrototypeBrailleWord
    {
        private readonly BrailleCell[] _cells;

        public PrototypeBrailleWord(string text, BrailleCell[] cells)
        {
            Text = text;
            _cells = cells;
        }

        public string Text { get; }

        public ReadOnlySpan<BrailleCell> Cells => _cells;
    }

    private struct BrailleCellBuffer
    {
        private BrailleCell[] _items;
        private int _start;
        private int _count;

        public BrailleCellBuffer(int capacity, int headroom)
        {
            _items = new BrailleCell[Math.Max(capacity, 8)];
            _start = Math.Max(headroom, 0);
            _count = 0;
        }

        public void Append(BrailleCell cell)
        {
            EnsureTailRoom();
            _items[_start + _count] = cell;
            _count++;
        }

        public void AppendRange(ReadOnlySpan<BrailleCell> cells)
        {
            EnsureTailRoom(cells.Length);
            cells.CopyTo(_items.AsSpan(_start + _count));
            _count += cells.Length;
        }

        public void Prepend(BrailleCell cell)
        {
            EnsureHeadRoom();
            _start--;
            _items[_start] = cell;
            _count++;
        }

        public ReadOnlySpan<BrailleCell> AsSpan()
        {
            return _items.AsSpan(_start, _count);
        }

        public BrailleCell[] ToArray()
        {
            var result = new BrailleCell[_count];
            AsSpan().CopyTo(result);
            return result;
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

        private void EnsureTailRoom(int additionalCount)
        {
            while (_start + _count + additionalCount > _items.Length)
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
            var newItems = new BrailleCell[newCapacity];
            AsSpan().CopyTo(newItems.AsSpan(newStart));
            _items = newItems;
            _start = newStart;
        }
    }
}
