# Immutable refactoring benchmark - phase 4a current snapshot

- 日期：2026-04-05
- workspace HEAD：`e3027e28556eb57570b85f34b739028fb38ab55e`
- 性質：Phase 4a 完成後的當前 workspace 量測快照
- 執行命令：`dotnet run --project src/EasyBrailleEditApp/BrailleToolkit.Benchmarks/BrailleToolkit.Benchmarks.csproj -c Release`

## 注意

- 這份結果是 post-change snapshot，不是 clean worktree A/B benchmark。
- workspace 含有尚未提交的 `BrailleCell` / test / 文件修改，因此這裡的 SHA 代表當前 HEAD，而不是「已提交的 phase4a commit」。
- 若要正式判斷 `4a` 是否相對於 Phase 3 回歸，仍應在乾淨 worktree 上做 commit-to-commit A/B。

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
| 中文單行轉換 | 67.53 us | 5.578 us | 3.689 us | 0.4272 | - | 5.82 KB |
| 英文單行轉換 | 522.34 us | 36.349 us | 24.043 us | 0.9766 | - | 16.09 KB |
| 中英混合單行轉換 | 380.82 us | 15.910 us | 10.524 us | 1.9531 | - | 28.73 KB |
| 中文多行轉換 | 3,101.44 us | 86.246 us | 57.046 us | 23.4375 | - | 327.83 KB |
| 英文多行轉換 | 3,008.05 us | 41.420 us | 27.397 us | 7.8125 | - | 100.21 KB |
| 中英混合多行轉換 | 1,868.38 us | 13.000 us | 8.599 us | 7.8125 | - | 111.28 KB |
| 長中文字串轉換 | 2,947.57 us | 24.314 us | 16.082 us | 23.4375 | 3.9063 | 329.3 KB |

## 與 Phase 3 current snapshot 的對照

基準：

- [`2026-04-05-phase3-current.md`](./2026-04-05-phase3-current.md)

| Method | Phase 3 Mean | Phase 4a Mean | Mean Δ | Phase 3 Alloc | Phase 4a Alloc | Alloc Δ |
| ---- | ----: | ----: | ----: | ----: | ----: | ----: |
| 中文單行轉換 | 64.95 us | 67.53 us | +3.97% | 6.01 KB | 5.82 KB | -3.16% |
| 英文單行轉換 | 498.97 us | 522.34 us | +4.68% | 17.52 KB | 16.09 KB | -8.16% |
| 中英混合單行轉換 | 417.29 us | 380.82 us | -8.74% | 29.92 KB | 28.73 KB | -3.98% |
| 中文多行轉換 | 3,110.24 us | 3,101.44 us | -0.28% | 339.67 KB | 327.83 KB | -3.49% |
| 英文多行轉換 | 3,058.34 us | 3,008.05 us | -1.64% | 108.91 KB | 100.21 KB | -7.99% |
| 中英混合多行轉換 | 2,118.93 us | 1,868.38 us | -11.82% | 116.46 KB | 111.28 KB | -4.45% |
| 長中文字串轉換 | 2,987.96 us | 2,947.57 us | -1.35% | 341.32 KB | 329.30 KB | -3.52% |

## 解讀

- 七個案例的 allocation 都比 Phase 3 current snapshot 更低。
- Mean 沒有出現一致性的退步方向，整體仍屬穩定。
- 單行中文 / 英文各有小幅上升，但落在單次 snapshot 合理波動範圍內；若後續要推進 `4b`，建議再做 clean worktree A/B 確認。

## 原始報表

- BenchmarkDotNet 匯出：
  - `BenchmarkDotNet.Artifacts/results/BrailleToolkit.Benchmarks.BrailleConversionBenchmarks-report.csv`
  - `BenchmarkDotNet.Artifacts/results/BrailleToolkit.Benchmarks.BrailleConversionBenchmarks-report-github.md`
  - `BenchmarkDotNet.Artifacts/results/BrailleToolkit.Benchmarks.BrailleConversionBenchmarks-report.html`
