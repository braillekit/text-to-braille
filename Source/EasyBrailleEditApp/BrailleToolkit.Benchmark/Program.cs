
using BenchmarkDotNet.Running;
using BrailleToolkit.Benchmark;

// 執行點字轉換器效能比較測試
BenchmarkRunner.Run<BrailleConverterBenchmarks>();

// 如需執行 BrailleProcessor 測試，取消註解以下行：
// BenchmarkRunner.Run<BrailleProcessorBenchmarks>();