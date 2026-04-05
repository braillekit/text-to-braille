# Immutable refactoring - phase 1

## 變更檔案

| 檔案 | 變更 |
| ---- | ---- |
| [BrailleGlobals.cs](/src/EasyBrailleEditApp/BrailleToolkit/BrailleGlobals.cs) | 加 `readonly` |
| [ContextTagNames.cs](/src/EasyBrailleEditApp/BrailleToolkit/Tags/ContextTagNames.cs) | `HashSet` → `FrozenSet` |
| [SimpleTag.cs](/src/EasyBrailleEditApp/BrailleToolkit/Tags/SimpleTag.cs) | `Dictionary` → `FrozenDictionary` |
| [BrailleCharConverter.cs](/src/EasyBrailleEditApp/BrailleToolkit/Converters/BrailleCharConverter.cs) | `Dictionary` → `FrozenDictionary` |
| [BrailleFontConverter.cs](/src/EasyBrailleEditApp/BrailleToolkit/Converters/BrailleFontConverter.cs) | `Hashtable` → `FrozenDictionary` |
| [BrailleProcessor.cs](/src/EasyBrailleEditApp/BrailleToolkit/BrailleProcessor.cs) | `_autoReplacedText` → `FrozenDictionary`；`CharPosition` → `readonly record struct` |
| [ExternalBrailleConverter.cs](/src/EasyBrailleEditApp/EasyBrailleEdit/Services/ExternalBrailleConverter.cs) | 配合 `CharPosition` 建構子語法調整 |

## 效能測試結果

- 執行日期：2026-04-05
- Benchmark 指令：`dotnet run --project src/EasyBrailleEditApp/BrailleToolkit.Benchmarks/BrailleToolkit.Benchmarks.csproj -c Release`
- 第一次量測紀錄：[`2026-04-05-phase1.md`](./benchmark-result/2026-04-05-phase1.md)
- A/B 重跑結果：[`2026-04-05-phase1-ab-rerun.md`](./benchmark-result/2026-04-05-phase1-ab-rerun.md)

### 回歸驗證

- `BrailleToolkit.Tests`: 131/131 通過。
- `EasyBrailleEdit.Tests`: 完整執行時 25 個測試中有 3 個失敗，但失敗原因是測試共用固定暫存檔 `Temp\\cvt_out.tmp` 造成互相干擾。
- 上述 3 個失敗測試在單獨執行時皆通過：
  - `InProcessBrailleConverterTests.ConvertAsync_WithValidText_ShouldSucceed`
  - `InProcessBrailleConverterTests.ConvertAsync_ProgressReporting_ShouldWork`
  - `MemoryLeakTests.RepeatedConverterCreation_ShouldNotLeakMemory`

### 第一次量測摘要

| Method | Baseline | Phase 1 | 變化 | Allocated |
| ---- | ----: | ----: | ----: | ----: |
| 中文單行轉換 | 93.46 μs | 151.3 μs | +61.89% | 72.84 KB |
| 英文單行轉換 | 701.09 μs | 702.2 μs | +0.16% | 169.33 KB |
| 中英混合單行轉換 | 812.19 μs | 845.3 μs | +4.08% | 374.43 KB |
| 中文多行轉換 | 5,216.39 μs | 8,854.1 μs | +69.74% | 4301.5 KB |
| 英文多行轉換 | 4,146.09 μs | 4,208.9 μs | +1.51% | 1041.13 KB |
| 中英混合多行轉換 | 2,154.94 μs | 3,577.0 μs | +65.99% | 1346.77 KB |
| 長中文字串轉換 | 4,865.34 μs | 8,372.6 μs | +72.09% | 4303.18 KB |

### A/B 重跑驗證

- 之後使用 clean worktree 重新比較 `pre-Phase 1` commit `7459cde` 與 `Phase 1` commit `e1f86b4`。
- 重跑時兩邊都使用相同的 benchmark 專案、相同機器、相同 .NET SDK 與 BenchmarkDotNet 設定。
- 重跑結果沒有重現第一次量測看到的巨大回歸，反而是 Phase 1 略快或近乎持平。

| Method | Pre-Phase 1 | Phase 1 | 變化 | Allocated |
| ---- | ----: | ----: | ----: | ----: |
| 中文單行轉換 | 172.2 μs | 171.0 μs | -0.70% | 72.84 KB |
| 英文單行轉換 | 785.7 μs | 782.3 μs | -0.43% | 169.33 KB |
| 中英混合單行轉換 | 926.2 μs | 918.7 μs | -0.81% | 374.43 KB |
| 中文多行轉換 | 9,972.1 μs | 9,727.5 μs | -2.45% | 4301.5 KB |
| 英文多行轉換 | 4,763.8 μs | 4,698.9 μs | -1.36% | 1041.13 KB |
| 中英混合多行轉換 | 4,082.3 μs | 3,979.7 μs | -2.51% | 1346.76 KB |
| 長中文字串轉換 | 10,207.5 μs | 9,955.9 μs | -2.46% | 4303.14 KB |

### 修正後結論

- 目前應以 clean worktree 的 A/B 重跑結果為準；第一次量測的 baseline 很可能不是同條件可直接比較的數據。
- 依照 A/B 重跑結果，Phase 1 沒有證據顯示 `FrozenDictionary` / `FrozenSet` 導致中文路徑明顯退步。
- 所有案例的 managed allocation 幾乎不變，表示 Phase 1 的收益主要是不可變性與 API 安全性整理，而不是記憶體配置改善。
- 若要繼續做效能優化，下一步應把焦點放在真正的熱路徑，例如中文轉換與注音查詢流程，而不是先假設 Frozen collection 是瓶頸。
