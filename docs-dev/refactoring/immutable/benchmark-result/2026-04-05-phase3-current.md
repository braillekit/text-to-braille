# Immutable refactoring benchmark - phase 3 current snapshot

- 日期：2026-04-05
- commit SHA：`a8894fbcc9a2131f7856cf69828d8b3c26387dab`
- 執行命令：`dotnet run --project src/EasyBrailleEditApp/BrailleToolkit.Benchmarks/BrailleToolkit.Benchmarks.csproj -c Release`
- 性質：Phase 3 完成後的當前 workspace 量測快照

## 注意

- 這份結果是 post-change snapshot，不是 clean worktree A/B benchmark。
- 因此它適合做後續追蹤與存檔，不適合單獨當成「Phase 3 相對於 Phase 2 是否回歸」的正式結論。
- 若要做正式比較，仍應以相同 benchmark 專案、相同機器、相同 SDK/Runtime，在乾淨 worktree 上重跑 baseline 與 candidate。

## 環境

- BenchmarkDotNet `0.14.0`
- OS: Windows 11 `10.0.26200.8117`
- CPU: 12th Gen Intel Core i7-12700H
- SDK: `.NET SDK 10.0.201`
- Runtime: `.NET 10.0.5`
- Job: `IterationCount=10`, `WarmupCount=3`, `RunStrategy=Throughput`

## Summary

| Method | Mean | Error | StdDev | Gen0 | Gen1 | Allocated |
| ---- | ----: | ----: | ----: | ----: | ----: | ----: |
| 中文單行轉換 | 64.95 us | 1.413 us | 0.934 us | 0.4883 | - | 6.01 KB |
| 英文單行轉換 | 498.97 us | 7.119 us | 4.709 us | 0.9766 | - | 17.52 KB |
| 中英混合單行轉換 | 417.29 us | 6.935 us | 4.587 us | 2.4414 | - | 29.92 KB |
| 中文多行轉換 | 3,110.24 us | 36.287 us | 21.594 us | 27.3438 | - | 339.67 KB |
| 英文多行轉換 | 3,058.34 us | 54.728 us | 36.199 us | 7.8125 | - | 108.91 KB |
| 中英混合多行轉換 | 2,118.93 us | 73.334 us | 48.506 us | 7.8125 | - | 116.46 KB |
| 長中文字串轉換 | 2,987.96 us | 19.181 us | 11.414 us | 27.3438 | 3.9063 | 341.32 KB |

## 原始報表

- BenchmarkDotNet 匯出：
  - `BenchmarkDotNet.Artifacts/results/BrailleToolkit.Benchmarks.BrailleConversionBenchmarks-report.csv`
  - `BenchmarkDotNet.Artifacts/results/BrailleToolkit.Benchmarks.BrailleConversionBenchmarks-report-github.md`
  - `BenchmarkDotNet.Artifacts/results/BrailleToolkit.Benchmarks.BrailleConversionBenchmarks-report.html`
