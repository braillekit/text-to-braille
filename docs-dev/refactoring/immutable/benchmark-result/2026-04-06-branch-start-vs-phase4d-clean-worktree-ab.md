# Immutable refactoring benchmark - branch start vs phase 4d

- 日期：2026-04-06
- baseline commit：`ba9bc71d6b33b00dbe04de957c5acc2dc07e576d`
- candidate commit：`279355533346a66352230062c89e30a5637cacf1`
- benchmark command：
  - `dotnet run --project src/EasyBrailleEditApp/BrailleToolkit.Benchmarks/BrailleToolkit.Benchmarks.csproj -c Release -- --filter *BrailleConversionBenchmarks*`
- 方法：
  - 以兩個乾淨 detached worktree 分別 checkout baseline / candidate
  - branch 起點尚未建立 `BrailleToolkit.Benchmarks` 專案，因此在 baseline worktree 僅補入目前 branch 使用的 benchmark harness（`BrailleToolkit.Benchmarks.csproj`、`Program.cs`、`BrailleConversionBenchmarks.cs`、`TestData/*.txt`）
  - benchmark harness 不修改 baseline production code，只負責提供相同量測入口
  - 各自獨立 restore、build、執行完整 conversion benchmark suite
  - 量測完成後移除臨時 worktree

## 比較範圍

這次比較的是整個 `immutable-design` branch 從起點到 `4d` 完成後的累積效應：

- baseline：branch 起點，尚未進入 Phase 4 immutable refactoring
- candidate：`4d` 完成後，word-level / line-level builder、result、view 邊界均已建立

因此這份結果不代表 `4d` 單一切點的增量，而是：

- Phase 3 的資料表與索引重構
- Phase 4 `4a`、`4b`、`4c`、`4d`

疊加後的總體 benchmark 對照。

## A/B 摘要

| Method | Baseline Mean | Candidate Mean | Mean Δ | Baseline Alloc | Candidate Alloc | Alloc Δ |
| ---- | ----: | ----: | ----: | ----: | ----: | ----: |
| 中文單行轉換 | 148.40 us | 61.30 us | -58.69% | 72.84 KB | 6.03 KB | -91.72% |
| 英文單行轉換 | 680.40 us | 484.69 us | -28.76% | 169.33 KB | 18.25 KB | -89.22% |
| 中英混合單行轉換 | 828.70 us | 374.29 us | -54.83% | 374.43 KB | 30.17 KB | -91.94% |
| 中文多行轉換 | 8,614.60 us | 3,053.40 us | -64.56% | 4,301.49 KB | 339.05 KB | -92.12% |
| 英文多行轉換 | 4,782.20 us | 3,443.42 us | -28.00% | 1,041.13 KB | 115.28 KB | -88.93% |
| 中英混合多行轉換 | 3,580.20 us | 1,830.19 us | -48.88% | 1,346.76 KB | 116.83 KB | -91.33% |
| 長中文字串轉換 | 8,368.70 us | 2,916.53 us | -65.15% | 4,303.15 KB | 337.60 KB | -92.15% |

## 解讀

- 7 個 benchmark 的 `Mean` 全部改善，沒有任何 throughput regression。
- 7 個 benchmark 的 `Allocated` 全部大幅下降，下降幅度約 `88.93%` 到 `92.15%`。
- throughput 改善最大的路徑是：
  - 長中文字串轉換：`-65.15%`
  - 中文多行轉換：`-64.56%`
  - 中文單行轉換：`-58.69%`
  - 中英混合單行轉換：`-54.83%`
- 相對改善幅度較小，但仍明顯變快的路徑是：
  - 英文單行轉換：`-28.76%`
  - 英文多行轉換：`-28.00%`

## 結論

- 若把 branch 起點視為整個 immutable refactoring 的 baseline，則目前到 `4d` 為止的累積結果是明確正向。
- `4a` 曾出現的 throughput regression，已被後續 `4b`、`4c`、`4d` 方向整體扳回，而且最終 allocation 也遠低於 branch 起點。
- 但這份結果不能直接用來宣稱 `4d` 單獨帶來多少改善；若要回答 `4d` 本身的效應，仍需額外比較 `4c` 結束點與 `4d` 完成點。
