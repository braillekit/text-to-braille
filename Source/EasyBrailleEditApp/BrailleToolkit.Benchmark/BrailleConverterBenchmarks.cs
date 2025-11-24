using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using EasyBrailleEdit.Services;

namespace BrailleToolkit.Benchmark
{
    /// <summary>
    /// 點字轉換器效能比較測試
    /// </summary>
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class BrailleConverterBenchmarks
    {
        private string _smallText = null!;
        private string _mediumText = null!;
        private string _largeText = null!;
        private readonly int _cellsPerLine = 40;
        private readonly string[] _phraseFiles = Array.Empty<string>();

        [GlobalSetup]
        public void Setup()
        {
            // 小型文件：10 行
            _smallText = string.Join("\r\n", Enumerable.Range(1, 10)
                .Select(i => $"這是第 {i} 行測試文字，包含中文和 English 混合內容。"));

            // 中型文件：100 行
            _mediumText = string.Join("\r\n", Enumerable.Range(1, 100)
                .Select(i => $"這是第 {i} 行測試文字，包含中文和 English 混合內容。"));

            // 大型文件：1000 行
            _largeText = string.Join("\r\n", Enumerable.Range(1, 1000)
                .Select(i => $"這是第 {i} 行測試文字，包含中文和 English 混合內容。"));
        }

        #region Small Text Benchmarks

        [Benchmark(Description = "InProcess - 小型文件 (10 行)")]
        public async Task<BrailleConversionResult> InProcess_SmallText()
        {
            using var converter = new InProcessBrailleConverter();
            var result = await converter.ConvertAsync(_smallText, _cellsPerLine, _phraseFiles);
            
            // 清理輸出檔案
            if (result.OutputFilePath != null && File.Exists(result.OutputFilePath))
            {
                File.Delete(result.OutputFilePath);
            }
            
            return result;
        }

        [Benchmark(Description = "External - 小型文件 (10 行)")]
        public async Task<BrailleConversionResult> External_SmallText()
        {
            using var converter = new ExternalBrailleConverter();
            var result = await converter.ConvertAsync(_smallText, _cellsPerLine, _phraseFiles);
            
            // 清理輸出檔案
            if (result.OutputFilePath != null && File.Exists(result.OutputFilePath))
            {
                File.Delete(result.OutputFilePath);
            }
            
            return result;
        }

        #endregion

        #region Medium Text Benchmarks

        [Benchmark(Description = "InProcess - 中型文件 (100 行)")]
        public async Task<BrailleConversionResult> InProcess_MediumText()
        {
            using var converter = new InProcessBrailleConverter();
            var result = await converter.ConvertAsync(_mediumText, _cellsPerLine, _phraseFiles);
            
            // 清理輸出檔案
            if (result.OutputFilePath != null && File.Exists(result.OutputFilePath))
            {
                File.Delete(result.OutputFilePath);
            }
            
            return result;
        }

        [Benchmark(Description = "External - 中型文件 (100 行)")]
        public async Task<BrailleConversionResult> External_MediumText()
        {
            using var converter = new ExternalBrailleConverter();
            var result = await converter.ConvertAsync(_mediumText, _cellsPerLine, _phraseFiles);
            
            // 清理輸出檔案
            if (result.OutputFilePath != null && File.Exists(result.OutputFilePath))
            {
                File.Delete(result.OutputFilePath);
            }
            
            return result;
        }

        #endregion

        #region Large Text Benchmarks

        [Benchmark(Description = "InProcess - 大型文件 (1000 行)")]
        public async Task<BrailleConversionResult> InProcess_LargeText()
        {
            using var converter = new InProcessBrailleConverter();
            var result = await converter.ConvertAsync(_largeText, _cellsPerLine, _phraseFiles);
            
            // 清理輸出檔案
            if (result.OutputFilePath != null && File.Exists(result.OutputFilePath))
            {
                File.Delete(result.OutputFilePath);
            }
            
            return result;
        }

        [Benchmark(Description = "External - 大型文件 (1000 行)")]
        public async Task<BrailleConversionResult> External_LargeText()
        {
            using var converter = new ExternalBrailleConverter();
            var result = await converter.ConvertAsync(_largeText, _cellsPerLine, _phraseFiles);
            
            // 清理輸出檔案
            if (result.OutputFilePath != null && File.Exists(result.OutputFilePath))
            {
                File.Delete(result.OutputFilePath);
            }
            
            return result;
        }

        #endregion
    }
}
