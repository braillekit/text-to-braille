using BenchmarkDotNet.Running;
using BrailleToolkit.Benchmarks;

BenchmarkSwitcher.FromAssembly(typeof(BrailleConversionBenchmarks).Assembly).Run(args);
