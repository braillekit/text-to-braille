using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

namespace BrailleToolkit.Benchmarks;

/// <summary>
/// 點字轉換效能測試。
/// 測試 TextToBrailleConverter 對不同類型文字的轉換效率。
/// </summary>
[MemoryDiagnoser]
[SimpleJob(RunStrategy.Throughput, warmupCount: 3, iterationCount: 10)]
public class BrailleConversionBenchmarks
{
    private TextToBrailleConverter _converter = null!;
    private string[] _chineseLines = null!;
    private string[] _englishLines = null!;
    private string[] _mixedLines = null!;

    private static string GetTestDataPath(string fileName)
    {
        string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        return Path.Combine(dir, "TestData", fileName);
    }

    [GlobalSetup]
    public void Setup()
    {
        _converter = new TextToBrailleConverter();

        _chineseLines = File.ReadAllLines(GetTestDataPath("BenchmarkText_Chinese.txt"))
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToArray();

        _englishLines = File.ReadAllLines(GetTestDataPath("BenchmarkText_English.txt"))
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToArray();

        _mixedLines = File.ReadAllLines(GetTestDataPath("BenchmarkText_Mixed.txt"))
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToArray();
    }

    [Benchmark(Description = "中文單行轉換")]
    public BrailleLine? ConvertSingleChineseLine()
    {
        return _converter.Convert(_chineseLines[0]);
    }

    [Benchmark(Description = "英文單行轉換")]
    public BrailleLine? ConvertSingleEnglishLine()
    {
        return _converter.Convert(_englishLines[0]);
    }

    [Benchmark(Description = "中英混合單行轉換")]
    public BrailleLine? ConvertSingleMixedLine()
    {
        return _converter.Convert(_mixedLines[0]);
    }

    [Benchmark(Description = "中文多行轉換")]
    public void ConvertAllChineseLines()
    {
        foreach (string line in _chineseLines)
        {
            _converter.Convert(line);
        }
    }

    [Benchmark(Description = "英文多行轉換")]
    public void ConvertAllEnglishLines()
    {
        foreach (string line in _englishLines)
        {
            _converter.Convert(line);
        }
    }

    [Benchmark(Description = "中英混合多行轉換")]
    public void ConvertAllMixedLines()
    {
        foreach (string line in _mixedLines)
        {
            _converter.Convert(line);
        }
    }

    [Benchmark(Description = "長中文字串轉換")]
    public BrailleLine? ConvertLongChineseString()
    {
        // 將所有中文行串接成一個長字串
        return _converter.Convert(string.Join("", _chineseLines));
    }
}
