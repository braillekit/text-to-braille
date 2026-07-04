# Immutable refactoring benchmark - phase 4a clean worktree A/B

- 日期：2026-04-05
- baseline commit：`e3027e28556eb57570b85f34b739028fb38ab55e`
- candidate commit：`ea804795f6cf56bbe9a152ef272adab9130c51db`
- 比較方式：clean worktree A/B 重跑
- 執行命令：`dotnet run --project src/EasyBrailleEditApp/BrailleToolkit.Benchmarks/BrailleToolkit.Benchmarks.csproj -c Release`

## 量測前提

- 兩邊都在乾淨 worktree 中獨立建置與執行。
- 機器、SDK、Runtime、BenchmarkDotNet job 設定一致。
- `BrailleToolkit.Benchmarks` 專案本身在 baseline 與 candidate 間沒有變更。
- baseline 與 candidate 間的檔案差異只有：
  - [`BrailleCell.cs`](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit/BrailleCell.cs)
  - [`BrailleCellTest.cs`](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit.Tests/BrailleCellTest.cs)
  - [`phase4.md`](/d:/work/BrailleKit/text-to-braille/docs-dev/refactoring/immutable/phase4.md)
  - [`2026-04-05-phase4a-current.md`](/d:/work/BrailleKit/text-to-braille/docs-dev/refactoring/immutable/benchmark-result/2026-04-05-phase4a-current.md)

## 環境

- BenchmarkDotNet `0.14.0`
- OS: Windows 11 `10.0.26200.8117`
- CPU: 12th Gen Intel Core i7-12700H
- SDK: `.NET SDK 10.0.201`
- Runtime: `.NET 10.0.5`
- Job: `IterationCount=10`, `WarmupCount=3`, `RunStrategy=Throughput`

## Summary

| Method | Baseline Mean | Candidate Mean | Mean Δ | Baseline Alloc | Candidate Alloc | Alloc Δ |
| ---- | ----: | ----: | ----: | ----: | ----: | ----: |
| 中文單行轉換 | 50.24 us | 68.02 us | +35.39% | 6.01 KB | 5.82 KB | -3.16% |
| 英文單行轉換 | 465.56 us | 518.92 us | +11.46% | 17.52 KB | 16.09 KB | -8.16% |
| 中英混合單行轉換 | 285.31 us | 440.92 us | +54.55% | 29.92 KB | 28.73 KB | -3.98% |
| 中文多行轉換 | 2,425.17 us | 3,701.84 us | +52.64% | 339.66 KB | 327.83 KB | -3.48% |
| 英文多行轉換 | 2,873.45 us | 3,258.94 us | +13.42% | 108.91 KB | 100.21 KB | -7.99% |
| 中英混合多行轉換 | 1,644.28 us | 2,310.21 us | +40.50% | 116.45 KB | 111.28 KB | -4.44% |
| 長中文字串轉換 | 2,151.15 us | 3,417.10 us | +58.85% | 341.32 KB | 329.30 KB | -3.52% |

## 解讀

- 這次 A/B 沒有任何案例出現平均時間改善。
- 七個 benchmark 的 `Allocated` 全部下降，但下降幅度不大，約 `3%` 到 `8%`。
- 與 allocation 改善相比，`Mean` 的退步幅度明顯更大，尤其是中文與混合路徑。
- 目前最合理的結論是：
  - `BrailleCell` value type / record struct 化確實減少了部分配置
  - 但它同時在目前的資料流與使用模式下帶來了顯著的 throughput regression

## 代表數字

- 中文單行：`50.24 us -> 68.02 us`，`+35.39%`
- 中英混合單行：`285.31 us -> 440.92 us`，`+54.55%`
- 中文多行：`2,425.17 us -> 3,701.84 us`，`+52.64%`
- 中英混合多行：`1,644.28 us -> 2,310.21 us`，`+40.50%`
- 長中文字串：`2,151.15 us -> 3,417.10 us`，`+58.85%`

## 結論

- 以 clean worktree A/B benchmark 來看，Phase 4a 不能視為「安全前進」。
- 若只看 snapshot，容易誤判成穩定或略有改善；正式 A/B 顯示實際上存在明顯回歸。
- 建議先不要往 `4b` 擴大，應先分析 `BrailleCell` value type 化造成退步的原因。
