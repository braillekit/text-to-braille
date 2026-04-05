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
- 詳細結果：[`2026-04-05-phase1.md`](./benchmark-result/2026-04-05-phase1.md)

### 回歸驗證

- `BrailleToolkit.Tests`: 131/131 通過。
- `EasyBrailleEdit.Tests`: 完整執行時 25 個測試中有 3 個失敗，但失敗原因是測試共用固定暫存檔 `Temp\\cvt_out.tmp` 造成互相干擾。
- 上述 3 個失敗測試在單獨執行時皆通過：
  - `InProcessBrailleConverterTests.ConvertAsync_WithValidText_ShouldSucceed`
  - `InProcessBrailleConverterTests.ConvertAsync_ProgressReporting_ShouldWork`
  - `MemoryLeakTests.RepeatedConverterCreation_ShouldNotLeakMemory`

### Benchmark 摘要

| Method | Baseline | Phase 1 | 變化 | Allocated |
| ---- | ----: | ----: | ----: | ----: |
| 中文單行轉換 | 93.46 μs | 151.3 μs | +61.89% | 72.84 KB |
| 英文單行轉換 | 701.09 μs | 702.2 μs | +0.16% | 169.33 KB |
| 中英混合單行轉換 | 812.19 μs | 845.3 μs | +4.08% | 374.43 KB |
| 中文多行轉換 | 5,216.39 μs | 8,854.1 μs | +69.74% | 4301.5 KB |
| 英文多行轉換 | 4,146.09 μs | 4,208.9 μs | +1.51% | 1041.13 KB |
| 中英混合多行轉換 | 2,154.94 μs | 3,577.0 μs | +65.99% | 1346.77 KB |
| 長中文字串轉換 | 4,865.34 μs | 8,372.6 μs | +72.09% | 4303.18 KB |

### 初步結論

- Phase 1 沒有帶來可觀察到的配置量下降；所有 benchmark 的配置量都與 baseline 幾乎相同。
- 效能表現並非中性，中文相關情境與中英混合多行情境明顯變慢。
- 初步判斷，Phase 1 雖然屬於低風險的不可變性整理，但目前不應視為效能優化成果；進入 Phase 2 之前應先針對這一批變更做 profiler 分析，確認 `FrozenDictionary`/`FrozenSet` 的實際收益與熱點成本。
