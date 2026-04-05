# Immutable refactoring benchmark - phase 3 clean worktree A/B

- 日期：2026-04-05
- baseline commit：`dcf71ef2d0aba531b6d724628234bec7d08168e1`
- candidate commit：`8251b2df5bc25d0b4bb67826e3745cbc64f59b2a`
- 方法：於兩個乾淨 worktree 中各自獨立建置並執行 `dotnet run --project src/EasyBrailleEditApp/BrailleToolkit.Benchmarks/BrailleToolkit.Benchmarks.csproj -c Release`

## 可比性確認

- `BrailleToolkit.Benchmarks` 專案本身在 baseline 與 candidate 間沒有變更。
- 本次 diff 主要集中在 `BrailleToolkit/Data` 的 Phase 3 重構檔案。
- 因此這次 A/B 可以直接用來觀察 Phase 3 對效能與配置量的影響。

## A/B 摘要

| Method | Baseline Mean | Candidate Mean | Mean Δ | Baseline Alloc | Candidate Alloc | Alloc Δ |
| ---- | ----: | ----: | ----: | ----: | ----: | ----: |
| 中文單行轉換 | 166.00 us | 68.70 us | -58.61% | 72.84 KB | 6.01 KB | -91.75% |
| 英文單行轉換 | 526.20 us | 514.98 us | -2.13% | 169.33 KB | 17.52 KB | -89.65% |
| 中英混合單行轉換 | 956.00 us | 400.45 us | -58.11% | 374.57 KB | 29.92 KB | -92.01% |
| 中文多行轉換 | 8,901.10 us | 3,420.40 us | -61.57% | 4,301.49 KB | 339.67 KB | -92.10% |
| 英文多行轉換 | 4,333.30 us | 3,734.43 us | -13.82% | 1,041.13 KB | 108.91 KB | -89.54% |
| 中英混合多行轉換 | 2,685.00 us | 2,088.58 us | -22.21% | 1,347.71 KB | 116.46 KB | -91.36% |
| 長中文字串轉換 | 6,332.10 us | 3,530.79 us | -44.24% | 4,303.15 KB | 341.32 KB | -92.07% |

## 解讀

- 本次 A/B 沒有任何案例出現平均時間回歸；七個 benchmark 全部改善。
- 最大改善集中在中文與混合內容：
  - 中文單行 `-58.61%`
  - 中文多行 `-61.57%`
  - 長中文字串 `-44.24%`
- 英文路徑也沒有退步，但改善幅度較小：
  - 英文單行 `-2.13%`
  - 英文多行 `-13.82%`
- `Allocated` 在所有案例都下降約 `89%` 到 `92%`，屬於非常一致且明顯的改善訊號。
- 這個結果與 Phase 3 的實作方向一致，推測主要收益來自：
  - 移除 `DataTable` / `DataRow` / `Select(...)` 的暫時配置與查詢成本
  - 改以 immutable entry + frozen index 直接查找

## 結論

- 以這次 clean worktree A/B 結果來看，Phase 3 不僅沒有明顯 regression，反而帶來了非常顯著的 allocation 改善，以及多數情境下明顯的執行時間改善。
- 若要為 Phase 3 下效能結論，目前可以合理寫成：
  - 功能完成
  - 未見效能回歸
  - allocation 顯著下降
  - 中文與混合轉換吞吐量明顯提升
