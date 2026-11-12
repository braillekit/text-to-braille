# Benchmark 結果：Immutable Refactoring Phase 2 clean worktree A/B

- 日期：2026-04-05
- 性質：正式 clean worktree A/B benchmark
- baseline commit SHA：`1abae1a4f7d5416a0f4c55c94e66c3c68705250e`
- candidate commit SHA：`dcf71ef2d0aba531b6d724628234bec7d08168e1`
- 說明：
  - baseline 使用 Phase 2 第一批完成後的既有 commit。
  - candidate 使用目前 workspace 的 Phase 2 全量變更建立暫時 detached commit 快照。
  - 兩邊都在獨立的乾淨 worktree 中各自建置與執行 benchmark。
- 指令：`dotnet run --project src/EasyBrailleEditApp/BrailleToolkit.Benchmarks/BrailleToolkit.Benchmarks.csproj -c Release`

## 環境

- OS: Windows 11 (10.0.26200.8117)
- CPU: 12th Gen Intel Core i7-12700H, 1 CPU, 20 logical and 14 physical cores
- .NET SDK: 10.0.201
- Runtime: .NET 10.0.5 (10.0.526.15411), X64 RyuJIT AVX2
- BenchmarkDotNet: v0.14.0
- Job: `IterationCount=10`, `RunStrategy=Throughput`, `WarmupCount=3`

## A/B 比較

| Method | Baseline Mean | Candidate Mean | Mean Δ | Baseline Alloc | Candidate Alloc | Alloc Δ |
| ---- | ----: | ----: | ----: | ----: | ----: | ----: |
| 中文單行轉換 | 153.8 us | 151.8 us | -1.30% | 72.8 KB | 72.84 KB | +0.05% |
| 英文單行轉換 | 708.5 us | 721.9 us | +1.89% | 169.29 KB | 169.33 KB | +0.02% |
| 中英混合單行轉換 | 849.8 us | 856.8 us | +0.82% | 374.45 KB | 374.57 KB | +0.03% |
| 中文多行轉換 | 8,820.3 us | 8,925.2 us | +1.19% | 4301.1 KB | 4301.48 KB | +0.01% |
| 英文多行轉換 | 3,963.5 us | 4,308.3 us | +8.70% | 1040.89 KB | 1041.13 KB | +0.02% |
| 中英混合多行轉換 | 3,788.8 us | 3,639.7 us | -3.94% | 1347.01 KB | 1347.72 KB | +0.05% |
| 長中文字串轉換 | 8,863.7 us | 8,384.1 us | -5.41% | 4303.11 KB | 4303.14 KB | +0.00% |

## Baseline 原始摘要

| Method | Mean | Error | StdDev | Allocated |
| ---- | ----: | ----: | ----: | ----: |
| 中文單行轉換 | 153.8 us | 4.82 us | 3.19 us | 72.8 KB |
| 英文單行轉換 | 708.5 us | 10.41 us | 6.19 us | 169.29 KB |
| 中英混合單行轉換 | 849.8 us | 17.34 us | 11.47 us | 374.45 KB |
| 中文多行轉換 | 8,820.3 us | 330.55 us | 218.64 us | 4301.1 KB |
| 英文多行轉換 | 3,963.5 us | 902.05 us | 596.65 us | 1040.89 KB |
| 中英混合多行轉換 | 3,788.8 us | 151.67 us | 100.32 us | 1347.01 KB |
| 長中文字串轉換 | 8,863.7 us | 327.82 us | 195.08 us | 4303.11 KB |

## Candidate 原始摘要

| Method | Mean | Error | StdDev | Allocated |
| ---- | ----: | ----: | ----: | ----: |
| 中文單行轉換 | 151.8 us | 2.34 us | 1.55 us | 72.84 KB |
| 英文單行轉換 | 721.9 us | 29.13 us | 19.27 us | 169.33 KB |
| 中英混合單行轉換 | 856.8 us | 15.44 us | 10.21 us | 374.57 KB |
| 中文多行轉換 | 8,925.2 us | 157.04 us | 103.87 us | 4301.48 KB |
| 英文多行轉換 | 4,308.3 us | 131.81 us | 87.18 us | 1041.13 KB |
| 中英混合多行轉換 | 3,639.7 us | 147.64 us | 97.65 us | 1347.72 KB |
| 長中文字串轉換 | 8,384.1 us | 122.42 us | 72.85 us | 4303.14 KB |

## 解讀

- 這次是 Phase 2 完成後的正式 clean worktree A/B，比 checkpoint benchmark 更適合拿來當結論依據。
- `Allocated` 幾乎沒有變化，代表 `IReadOnlyList` 公開 API + 序列化相容層並未引入明顯額外配置。
- `Mean` 的變化方向不一致：
  - 有些案例小幅變慢，如英文單行、多行。
  - 有些案例小幅變快，如中文單行、中英混合多行、長中文字串。
- 最大波動是英文多行 `+8.70%`，但對應 allocation 幾乎不變，因此較可能是量測波動、路徑細節差異或其他非配置因素；目前應列為「觀察項」，不宜直接下 Phase 2 有明顯回歸的結論。

## 結論

- Phase 2 完成後，功能與測試均已通過。
- 以 clean worktree A/B benchmark 來看，未觀察到明顯 allocation regression。
- 若後續進入 Phase 3，可將本次結果視為 Phase 2 的正式效能基準。
