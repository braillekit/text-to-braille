# Benchmark 結果：Immutable Refactoring Phase 2 checkpoint (`2a/2e/2f/2g`)

- 日期：2026-04-05
- 性質：checkpoint benchmark
- commit SHA：`1abae1a4f7d5416a0f4c55c94e66c3c68705250e`
- 說明：以目前 Phase 2 第一批子項 (`2a/2e/2f/2g`) 完成後的 workspace 狀態執行一次 benchmark，作為進入 `2b/2c/2d` 前的檢查點。
- 指令：`dotnet run --project src/EasyBrailleEditApp/BrailleToolkit.Benchmarks/BrailleToolkit.Benchmarks.csproj -c Release`

## 環境

- OS: Windows 11 (10.0.26200.8117)
- CPU: 12th Gen Intel Core i7-12700H, 1 CPU, 20 logical and 14 physical cores
- .NET SDK: 10.0.201
- Runtime: .NET 10.0.5 (10.0.526.15411), X64 RyuJIT AVX2
- BenchmarkDotNet: v0.14.0
- Job: `IterationCount=10`, `RunStrategy=Throughput`, `WarmupCount=3`

## 結果

| Method | Mean | Error | StdDev | Allocated |
| ---- | ----: | ----: | ----: | ----: |
| 中文單行轉換 | 104.6 μs | 5.60 μs | 3.70 μs | 72.8 KB |
| 英文單行轉換 | 501.0 μs | 24.95 μs | 16.50 μs | 169.29 KB |
| 中英混合單行轉換 | 566.8 μs | 44.44 μs | 29.39 μs | 374.45 KB |
| 中文多行轉換 | 6,058.9 μs | 221.51 μs | 146.51 μs | 4301.09 KB |
| 英文多行轉換 | 2,997.9 μs | 156.45 μs | 103.48 μs | 1040.9 KB |
| 中英混合多行轉換 | 2,728.2 μs | 124.63 μs | 82.44 μs | 1347.01 KB |
| 長中文字串轉換 | 6,144.6 μs | 166.52 μs | 110.14 μs | 4303.1 KB |

## 分析

- 這次量測是單點 checkpoint，不是 clean worktree 的 commit-to-commit A/B 比較，因此不適合直接拿來下「Phase 2 是否回歸」的結論。
- 從這次報表來看，`Allocated` 沒有出現明顯異常尖峰；最敏感的 `2f` 事件參數改為每次建立新實例後，至少在整體 benchmark 層級沒有呈現出異常的配置量。
- 後續若要判斷 `2a/2e/2f/2g` 是否真的影響 Mean，仍應在 Phase 2 全部完成後做 clean worktree A/B 重跑。

## 原始報表

- BenchmarkDotNet GitHub report：`BenchmarkDotNet.Artifacts/results/BrailleToolkit.Benchmarks.BrailleConversionBenchmarks-report-github.md`
- BenchmarkDotNet CSV：`BenchmarkDotNet.Artifacts/results/BrailleToolkit.Benchmarks.BrailleConversionBenchmarks-report.csv`
- BenchmarkDotNet HTML：`BenchmarkDotNet.Artifacts/results/BrailleToolkit.Benchmarks.BrailleConversionBenchmarks-report.html`
