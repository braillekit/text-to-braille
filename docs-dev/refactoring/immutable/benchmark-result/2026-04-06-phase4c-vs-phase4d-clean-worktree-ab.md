# Immutable refactoring benchmark - phase 4c vs phase 4d clean worktree A/B

- 日期：2026-04-06
- baseline commit：`b344b74`
- candidate commit：`279355533346a66352230062c89e30a5637cacf1`
- benchmark command：
  - `dotnet run --project src/EasyBrailleEditApp/BrailleToolkit.Benchmarks/BrailleToolkit.Benchmarks.csproj -c Release -- --filter *BrailleConversionBenchmarks*`
- 方法：
  - 以兩個乾淨 detached worktree 分別 checkout `4c` 結束點與 `4d` 完成點
  - `BrailleToolkit.Benchmarks` 專案在兩個 commit 間沒有變更，因此這次比較可以視為純 production code diff
  - 各自獨立 restore、build、執行完整 conversion benchmark suite
  - 量測完成後移除臨時 worktree

## 比較範圍

這次比較的是 `4d` 本身的增量效果：

- baseline：`4c` handoff 完成點
- candidate：`4d` 完成點

candidate 內容主要是 line-level builder / result / view 邊界及其主要建構點遷移：

- `IBrailleLineView`
- `IBrailleLineResult` / `BrailleLineMaterialized`
- `BrailleLineBuilder`
- `BrailleLineHelper`
- `BrailleProcessor` line construction migration
- `BrailleDocumentFormatter` new-line construction migration
- `BrailleDocumentYamlSerializer` / `BraillePageTitle` / `BrailleLine` copy path migration

## A/B 摘要

| Method | Baseline Mean | Candidate Mean | Mean Δ | Baseline Alloc | Candidate Alloc | Alloc Δ |
| ---- | ----: | ----: | ----: | ----: | ----: | ----: |
| 中文單行轉換 | 62.98 us | 63.80 us | +1.30% | 5.86 KB | 6.09 KB | +3.92% |
| 英文單行轉換 | 563.42 us | 483.66 us | -14.16% | 16.60 KB | 18.25 KB | +9.94% |
| 中英混合單行轉換 | 415.30 us | 368.15 us | -11.35% | 28.74 KB | 30.16 KB | +4.94% |
| 中文多行轉換 | 3,064.60 us | 3,034.14 us | -0.99% | 329.98 KB | 339.05 KB | +2.75% |
| 英文多行轉換 | 3,451.26 us | 2,978.34 us | -13.70% | 103.30 KB | 115.28 KB | +11.60% |
| 中英混合多行轉換 | 2,107.84 us | 1,827.09 us | -13.32% | 110.74 KB | 116.83 KB | +5.50% |
| 長中文字串轉換 | 2,873.13 us | 2,507.18 us | -12.74% | 331.45 KB | 337.60 KB | +1.86% |

## 解讀

- `4d` 在 7 個 benchmark 中有 6 個 `Mean` 改善。
- 改善最明顯的是：
  - 英文單行轉換：`-14.16%`
  - 英文多行轉換：`-13.70%`
  - 中英混合多行轉換：`-13.32%`
  - 長中文字串轉換：`-12.74%`
  - 中英混合單行轉換：`-11.35%`
- 中文多行幾乎持平：`-0.99%`。
- 唯一輕微回歸的是中文單行：`+1.30%`，幅度很小，較接近量測波動等級。

allocation 方向則全部上升，但幅度遠小於先前 `4b` 初期曾觀察到的 allocation 回升：

- allocation 上升最大的是：
  - 英文多行轉換：`+11.60%`
  - 英文單行轉換：`+9.94%`
  - 中英混合多行轉換：`+5.50%`
- 中文單行、中文多行、長中文字串的 allocation 變動都在約 `+4%` 以內。

## 量測穩定度注意事項

- 長中文字串轉換在 candidate 這輪的 `Error` / `StdDev` 明顯高於其他案例，代表這一筆噪音較大。
- 因此長中文字串的 `-12.74%` 應視為正向訊號，但不宜單獨下過度強烈的結論。
- 其餘英文與混合內容路徑的改善趨勢較一致，可信度相對更高。

## 結論

- 若只看 `4d` 本身，結論可整理為：
  - line-level builder / result / view 邊界沒有引入明顯 throughput regression
  - 英文與混合內容路徑多數出現約 `11%` 到 `14%` 的改善
  - 中文路徑整體接近持平，其中中文單行有極小幅回歸
  - allocation 全面小幅上升，但幅度可控，明顯低於 `4b` 早期尚未收斂時的回升量
