# Phase 4b clean worktree A/B benchmark

- 日期：2026-04-05
- baseline commit：`ea804795f6cf56bbe9a152ef272adab9130c51db`
- candidate commit：`53a7c22123ae328165e529e5c582d56818a5104b`
- benchmark command：
  - `dotnet run --project src/EasyBrailleEditApp/BrailleToolkit.Benchmarks/BrailleToolkit.Benchmarks.csproj -c Release -- --filter *BrailleConversionBenchmarks*`
- 方法：
  - 以兩個乾淨 detached worktree 分別 checkout baseline / candidate
  - 各自獨立 restore、build、執行完整 conversion benchmark suite
  - 量測完成後移除臨時 worktree

## 比較範圍

這次比較的是：

- baseline：Phase 4 `4a` 完成點
- candidate：目前整批 `4b` production cut

candidate 內容包含：

- `BrailleWordBuilder` / `BrailleCellBuffer`
- `EnglishWordConverter` builder 路徑
- `EnglishBrailleRule` / `GeneralBrailleRule` 規則層橋接
- `BrailleProcessor` 分數前插 / `TableConverter` 單格替換橋接
- `WordConverter` / `UrlConverter` / `EnglishUebConverter` / `TwChineseCharConverter` / `ContextTagConverter` 的 builder materialization 擴散

## A/B 摘要

| Method | Baseline Mean | Candidate Mean | Mean Δ | Baseline Alloc | Candidate Alloc | Alloc Δ |
| ---- | ----: | ----: | ----: | ----: | ----: | ----: |
| 中文單行轉換 | 66.03 us | 46.10 us | -30.18% | 5.82 KB | 6.32 KB | +8.59% |
| 英文單行轉換 | 578.35 us | 348.34 us | -39.77% | 16.09 KB | 23.87 KB | +48.35% |
| 中英混合單行轉換 | 383.81 us | 293.50 us | -23.53% | 28.73 KB | 32.91 KB | +14.55% |
| 中文多行轉換 | 3,336.77 us | 2,144.99 us | -35.72% | 327.83 KB | 360.96 KB | +10.11% |
| 英文多行轉換 | 2,810.04 us | 2,124.65 us | -24.39% | 100.21 KB | 147.48 KB | +47.17% |
| 中英混合多行轉換 | 1,374.60 us | 1,476.20 us | +7.39% | 111.28 KB | 132.43 KB | +19.01% |
| 長中文字串轉換 | 2,149.56 us | 2,102.52 us | -2.19% | 329.30 KB | 362.43 KB | +10.06% |

## 解讀

- candidate 在 7 個 benchmark 中有 6 個 `Mean` 變快。
- 最大的 throughput 改善出現在：
  - 英文單行轉換：`-39.77%`
  - 中文多行轉換：`-35.72%`
  - 中文單行轉換：`-30.18%`
  - 英文多行轉換：`-24.39%`
  - 中英混合單行轉換：`-23.53%`
- 唯一明確回歸的是中英混合多行轉換：`+7.39%`。
- 長中文字串轉換幾乎持平，但仍有小幅改善：`-2.19%`。

allocation 方向則完全相反：

- 7 個 benchmark 的 `Allocated` 全部上升。
- 上升幅度最大的路徑是：
  - 英文單行轉換：`+48.35%`
  - 英文多行轉換：`+47.17%`
  - 中英混合多行轉換：`+19.01%`
  - 中英混合單行轉換：`+14.55%`

## 結論

這次 clean worktree A/B 顯示，`4b` 目前的 builder / compatibility bridge 已經有效扳回 `4a` 帶來的大部分 throughput regression，說明方向本身是可行的；但它同時也把 allocation 往反方向推高，代表目前仍有相當多 materialize / copy / `BrailleWord` 相容層成本尚未被消掉。

因此目前比較合理的結論是：

- `4b` 在 CPU throughput 上是正向的
- `4b` 在 allocation 上仍未達標
- 後續若要繼續往下做，優先目標應該是減少 builder 到既有 `BrailleWord` model 之間的中間配置，而不是回退 builder 路線本身
