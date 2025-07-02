
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;
using BrailleToolkitBenchmark;
using BenchmarkDotNet.Loggers;

BenchmarkRunner.Run<BrailleProcessorBenchmarks>();

/*
var bm = new BrailleProcessorBenchmarks();
var s = bm.ConvertLineTestChinese();
Console.WriteLine(s);
*/