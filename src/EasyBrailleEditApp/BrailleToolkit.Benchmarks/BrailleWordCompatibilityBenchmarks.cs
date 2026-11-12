using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

namespace BrailleToolkit.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(RunStrategy.Throughput, warmupCount: 3, iterationCount: 10)]
public class BrailleWordCompatibilityBenchmarks
{
    private readonly BrailleCell _capitalCell = BrailleCell.Capital;

    private BrailleWord _sourceWord = null!;
    private BrailleWord _directTarget = null!;
    private BrailleWord _compatTarget = null!;
    private BrailleWord _builderTarget = null!;
    private BrailleCell[] _baseCells = null!;
    private BrailleCell[] _prefixedCells = null!;

    [Params("English", "Chinese")]
    public string Scenario { get; set; } = "English";

    [GlobalSetup]
    public void Setup()
    {
        if (Scenario == "Chinese")
        {
            _baseCells =
            [
                BrailleCell.GetInstance("0B"),
                BrailleCell.GetInstance("3A"),
                BrailleCell.GetInstance("10")
            ];
        }
        else
        {
            _baseCells =
            [
                BrailleCell.GetInstance("01"),
                BrailleCell.GetInstance("12")
            ];
        }

        _prefixedCells = new BrailleCell[_baseCells.Length + 1];
        _prefixedCells[0] = _capitalCell;
        Array.Copy(_baseCells, 0, _prefixedCells, 1, _baseCells.Length);

        _sourceWord = CreateBaseWord();
        _directTarget = CreateBaseWord();
        _compatTarget = CreateBaseWord();
        _builderTarget = CreateBaseWord();
    }

    [Benchmark(Baseline = true, Description = "Direct: new BrailleWord + fill list")]
    public int DirectNewBrailleWord()
    {
        var word = new BrailleWord(GetText())
        {
            OriginalText = GetOriginalText()
        };
        ApplyMetadata(word, includePrefix: true);
        PopulateCells(word.CellList.Items, _prefixedCells);
        return Consume(word);
    }

    [Benchmark(Description = "Compat: CreateBrailleWord(span)")]
    public int CompatibilityCreateBrailleWord()
    {
        BrailleWord word = BrailleWord.CreateFromConstruction(
            GetText(),
            GetOriginalText(),
            GetLanguage(),
            _prefixedCells,
            GetPhoneticCode(),
            Scenario == "Chinese",
            dontBreakLineHere: true,
            contextNames: "分析",
            contextTag: null,
            isContextTag: false,
            isConvertedFromTag: false,
            noDigitCell: Scenario == "English",
            noSpace: Scenario == "English",
            noCapitalRule: Scenario == "English",
            isEngPhonetic: false);

        return Consume(word);
    }

    [Benchmark(Description = "Builder: ToBrailleWord()")]
    public int BuilderToBrailleWord()
    {
        BrailleWord word = CreatePrefixedBuilder().ToBrailleWord();
        return Consume(word);
    }

    [Benchmark(Description = "Builder: Build()")]
    public int BuilderBuildOnly()
    {
        IBrailleWordResult result = CreatePrefixedBuilder().Build();
        return result.CellCount + result.Text.Length + result.ContextNames.Length;
    }

    [Benchmark(Description = "Builder: Build().ToBrailleWord()")]
    public int BuilderBuildThenToBrailleWord()
    {
        BrailleWord word = CreatePrefixedBuilder().Build().ToBrailleWord();
        return Consume(word);
    }

    [Benchmark(Description = "Direct: mutate existing word")]
    public int DirectApplyToExistingWord()
    {
        BrailleWord word = _directTarget;
        ApplyMetadata(word, includePrefix: true);
        PopulateCells(word.CellList.Items, _prefixedCells);
        return Consume(word);
    }

    [Benchmark(Description = "Compat: ApplyToBrailleWord(span)")]
    public int CompatibilityApplyToExistingWord()
    {
        BrailleWord word = _compatTarget;
        word.ApplyConstruction(
            GetText(),
            GetOriginalText(),
            GetLanguage(),
            _prefixedCells,
            GetPhoneticCode(),
            Scenario == "Chinese",
            dontBreakLineHere: true,
            contextNames: "分析",
            contextTag: null,
            isContextTag: false,
            isConvertedFromTag: false,
            noDigitCell: Scenario == "English",
            noSpace: Scenario == "English",
            noCapitalRule: Scenario == "English",
            isEngPhonetic: false);

        return Consume(word);
    }

    [Benchmark(Description = "Builder: FromBrailleWord()")]
    public int BuilderFromBrailleWordOnly()
    {
        BrailleWordBuilder builder = BrailleWordBuilder.FromBrailleWord(_sourceWord);
        return builder.CellCount + builder.Text.Length + builder.ContextNames.Length;
    }

    [Benchmark(Description = "Builder: FromBrailleWord() + ApplyTo()")]
    public int BuilderFromBrailleWordApplyToExistingWord()
    {
        BrailleWordBuilder builder = BrailleWordBuilder.FromBrailleWord(_sourceWord);
        builder.PrependCell(_capitalCell);
        builder.DontBreakLineHere = true;
        builder.ContextNames = "分析";
        builder.ApplyTo(_builderTarget);
        return Consume(_builderTarget);
    }

    private BrailleWord CreateBaseWord()
    {
        var word = new BrailleWord(GetText())
        {
            OriginalText = GetOriginalText()
        };
        ApplyMetadata(word, includePrefix: false);
        PopulateCells(word.CellList.Items, _baseCells);
        return word;
    }

    private BrailleWordBuilder CreatePrefixedBuilder()
    {
        var builder = new BrailleWordBuilder(GetText())
        {
            OriginalText = GetOriginalText(),
            Language = GetLanguage(),
            PhoneticCode = GetPhoneticCode(),
            IsPolyphonic = Scenario == "Chinese",
            DontBreakLineHere = true,
            ContextNames = "分析",
            NoDigitCell = Scenario == "English",
            NoSpace = Scenario == "English",
            NoCapitalRule = Scenario == "English"
        };

        builder.PrependCell(_capitalCell);
        builder.AppendCells(_baseCells);
        return builder;
    }

    private void ApplyMetadata(BrailleWord word, bool includePrefix)
    {
        word.Language = GetLanguage();
        word.PhoneticCode = GetPhoneticCode() ?? String.Empty;
        word.IsPolyphonic = Scenario == "Chinese";
        word.DontBreakLineHere = true;
        word.ContextNames = "分析";
        word.NoDigitCell = Scenario == "English";
        word.NoSpace = Scenario == "English";
        word.NoCapitalRule = Scenario == "English";

        if (!includePrefix && word.Cells.Count > _baseCells.Length)
        {
            word.Cells.RemoveAt(0);
        }
    }

    private static void PopulateCells(List<BrailleCell> items, ReadOnlySpan<BrailleCell> cells)
    {
        items.Clear();
        if (items.Capacity < cells.Length)
        {
            items.Capacity = cells.Length;
        }

        for (int i = 0; i < cells.Length; i++)
        {
            items.Add(cells[i]);
        }
    }

    private int Consume(BrailleWord word)
    {
        return word.CellCount + word.Text.Length + word.ContextNames.Length + word.Cells[0].Value;
    }

    private BrailleLanguage GetLanguage()
    {
        return Scenario == "Chinese" ? BrailleLanguage.Chinese : BrailleLanguage.English;
    }

    private string? GetPhoneticCode()
    {
        return Scenario == "Chinese" ? "ㄅ" : null;
    }

    private string GetText()
    {
        return Scenario == "Chinese" ? "我" : "A";
    }

    private string GetOriginalText()
    {
        return Scenario == "Chinese" ? "<原文>我" : "<orig>A";
    }
}
