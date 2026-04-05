# Immutable refactoring - phase 4

## 狀態

Phase 4 目前：

- `4a` `BrailleCell` -> `readonly record struct`
- `4b` 已開始進入第一個 production cut：`BrailleWordBuilder + BrailleCellBuffer` 骨架已落地，並先接到單一路徑的 `EnglishWordConverter` / 英文規則

`4b` `BrailleCellList`、`4c` `BrailleWord`、`4d` `BrailleLine` 的完整 immutable builder / result 分離尚未完成；目前仍是小步 prototype 與相容橋接階段。

本階段以前一份 [`phase3.md`](./phase3.md) 為起點，先做最小可驗證的高風險 prototype。

## 這一批的重點

### `4a`

- [`BrailleCell.cs`](/src/EasyBrailleEditApp/BrailleToolkit/BrailleCell.cs) 從 sealed class 改成 `readonly record struct`
- 保留既有 public API：
  - `GetInstance(BrailleCellCode)`
  - `GetInstance(int)`
  - `GetInstance(string)`
  - `GetInstance(int[])`
  - `GetInstanceFromPositionNumberString(string)`
  - `Blank` / `Capital`
- `Value` 改成 `[DataMember]` + `init` 屬性，讓 `DataContractJsonSerializer` / 既有 YAML converter 仍可用
- 保留 256-entry 靜態快取陣列，讓 `GetInstance(int)` / `GetInstance(BrailleCellCode)` 的入口與索引檢查語意不變
- `ToString()` 仍維持輸出十六進位碼，不採用 record 預設字串格式

### 語意上的差異

- `BrailleCell` 現在是 value type，因此 `default(BrailleCell)` 會自然表示空方（`Value = 0x00`）
- equality / hash code 改由 record struct 的 value equality 負責，語意仍然是以 `Value` 為準
- 原本 class 版本的 flyweight 共用實例觀念，現在只保留為相容 API 的快取入口；呼叫端不應再假設它有參考相等語意

## 測試補強

- [`BrailleCellTest.cs`](/src/EasyBrailleEditApp/BrailleToolkit.Tests/BrailleCellTest.cs) 新增：
  - value equality 驗證
  - `default(BrailleCell)` = `BrailleCell.Blank` 驗證
  - `DataContractJsonSerializer` round-trip 驗證

## 主要變更檔案

| 檔案 | 變更 |
| ---- | ---- |
| [BrailleCell.cs](/src/EasyBrailleEditApp/BrailleToolkit/BrailleCell.cs) | 改為 `readonly record struct`，保留相容 factory 與十六進位/點位轉換 API |
| [BrailleCellTest.cs](/src/EasyBrailleEditApp/BrailleToolkit.Tests/BrailleCellTest.cs) | 補 value equality、default blank、JSON round-trip 測試 |

## 回歸驗證

- `dotnet test src/EasyBrailleEditApp/BrailleToolkit.Tests/BrailleToolkit.Tests.csproj`
- `dotnet test src/EasyBrailleEditApp/EasyBrailleEdit.Tests/EasyBrailleEdit.Tests.csproj -p:GenerateRuntimeConfigurationFiles=true`
- `dotnet build src/EasyBrailleEditApp/EasyBrailleEditApp.sln -p:GenerateRuntimeConfigurationFiles=true`

結果：

- `BrailleToolkit.Tests`: 139 / 139 通過
- `EasyBrailleEdit.Tests`: 25 / 25 通過
- Solution build：成功

補充：

- 在目前環境中，`EasyBrailleEdit.Tests` 直接執行時會因 `Txt2Brl.deps.json` / `Txt2Brl.runtimeconfig.json` 未生成而失敗。
- 加上 `GenerateRuntimeConfigurationFiles=true` 後即可正常建置與執行，這次 Phase 4a 未另外修改 project 檔。

## 效能測試

### Post-change snapshot

- 日期：2026-04-05
- 詳細紀錄：
  - [`2026-04-05-phase4a-current.md`](./benchmark-result/2026-04-05-phase4a-current.md)

### 正式 clean worktree A/B benchmark

- 日期：2026-04-05
- baseline commit：`e3027e28556eb57570b85f34b739028fb38ab55e`
- candidate commit：`ea804795f6cf56bbe9a152ef272adab9130c51db`
- 方法：於兩個乾淨 worktree 各自獨立建置與執行 benchmark
- 詳細紀錄：
  - [`2026-04-05-phase4a-clean-worktree-ab.md`](./benchmark-result/2026-04-05-phase4a-clean-worktree-ab.md)

#### A/B 摘要

| Method | Baseline Mean | Candidate Mean | Mean Δ | Baseline Alloc | Candidate Alloc | Alloc Δ |
| ---- | ----: | ----: | ----: | ----: | ----: | ----: |
| 中文單行轉換 | 50.24 us | 68.02 us | +35.39% | 6.01 KB | 5.82 KB | -3.16% |
| 英文單行轉換 | 465.56 us | 518.92 us | +11.46% | 17.52 KB | 16.09 KB | -8.16% |
| 中英混合單行轉換 | 285.31 us | 440.92 us | +54.55% | 29.92 KB | 28.73 KB | -3.98% |
| 中文多行轉換 | 2,425.17 us | 3,701.84 us | +52.64% | 339.66 KB | 327.83 KB | -3.48% |
| 英文多行轉換 | 2,873.45 us | 3,258.94 us | +13.42% | 108.91 KB | 100.21 KB | -7.99% |
| 中英混合多行轉換 | 1,644.28 us | 2,310.21 us | +40.50% | 116.45 KB | 111.28 KB | -4.44% |
| 長中文字串轉換 | 2,151.15 us | 3,417.10 us | +58.85% | 341.32 KB | 329.30 KB | -3.52% |

#### 解讀

- 這次 clean worktree A/B 顯示 `4a` 在七個 benchmark 案例都出現平均時間回歸。
- allocation 同時在七個案例全部下降，下降幅度約 `3%` 到 `8%`。
- 回歸幅度最大的路徑是：
  - 長中文字串轉換：`+58.85%`
  - 中英混合單行轉換：`+54.55%`
  - 中文多行轉換：`+52.64%`
  - 中英混合多行轉換：`+40.50%`
- baseline 與 candidate 間 `BrailleToolkit.Benchmarks` 專案本身沒有變更；實際程式碼差異只有 [`BrailleCell.cs`](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit/BrailleCell.cs) 與測試 / 文件，因此這次 A/B 對 `4a` 具有直接參考價值。

### 與 Phase 3 current snapshot 的對照

注意：

- 這裡比較的是 [`2026-04-05-phase3-current.md`](./benchmark-result/2026-04-05-phase3-current.md) 與本次 `phase4a-current`。
- 兩者都是同機器、同 benchmark 專案、同命令的 workspace snapshot。
- 這不是 clean worktree A/B benchmark，所以只適合做趨勢觀察，不適合單獨下正式 regression 結論。
- 這份 snapshot 的方向與後續 clean worktree A/B 不一致，因此正式結論應以 A/B 為準。

| Method | Phase 3 Mean | Phase 4a Mean | Mean Δ | Phase 3 Alloc | Phase 4a Alloc | Alloc Δ |
| ---- | ----: | ----: | ----: | ----: | ----: | ----: |
| 中文單行轉換 | 64.95 us | 67.53 us | +3.97% | 6.01 KB | 5.82 KB | -3.16% |
| 英文單行轉換 | 498.97 us | 522.34 us | +4.68% | 17.52 KB | 16.09 KB | -8.16% |
| 中英混合單行轉換 | 417.29 us | 380.82 us | -8.74% | 29.92 KB | 28.73 KB | -3.98% |
| 中文多行轉換 | 3,110.24 us | 3,101.44 us | -0.28% | 339.67 KB | 327.83 KB | -3.49% |
| 英文多行轉換 | 3,058.34 us | 3,008.05 us | -1.64% | 108.91 KB | 100.21 KB | -7.99% |
| 中英混合多行轉換 | 2,118.93 us | 1,868.38 us | -11.82% | 116.46 KB | 111.28 KB | -4.45% |
| 長中文字串轉換 | 2,987.96 us | 2,947.57 us | -1.35% | 341.32 KB | 329.30 KB | -3.52% |

### 解讀

- 這一輪 snapshot 沒有看到 allocation 回升，七個案例全部下降。
- Mean 整體維持在 Phase 3 的量級，沒有出現明顯的大幅退步。
- 中文/英文單行各自有約 `4%` 左右的波動，但同時 allocation 下降；以單次 snapshot 來看，較像量測波動而不是明確 regression。
- 混合內容與多行情境有數個案例反而更快，特別是中英混合多行 `-11.82%`。

目前可先把 `4a` 視為：

- 功能完成
- 序列化與既有 API 相容性仍在
- allocation 小幅改善
- 但 clean worktree A/B 已確認有明顯執行時間回歸，暫時不適合直接把這個方向擴大到 `4b`

## 後續建議

- `4b` 建議先暫停，優先釐清 `BrailleCell` 改成 value type 後，為何會在轉換熱路徑上造成明顯 throughput regression。
- 原因分析紀錄見 [`phase4-cause-analysis.md`](./phase4-cause-analysis.md)。
- 若後續仍要推 immutable / value type 方向，建議先做更小範圍的 profiler / micro-benchmark，確認退步是否來自複製成本、JIT 行為、`List<BrailleCell>` 使用模式，或 record struct 產生的額外工作。
- prototype 驗證紀錄見 [`2026-04-05-phase4-prototype-validation.md`](./benchmark-result/2026-04-05-phase4-prototype-validation.md)：
  - 真實 pipeline A/B 顯示 plain `readonly struct` 不能穩定修復 regression
  - synthetic storage benchmark 顯示 builder / buffer 路線仍值得繼續驗證
  - `BrailleWordBuilder + BrailleCellBuffer` 小型 prototype 已額外量到約 `-35%` Mean、`-30%` allocation 的正面訊號
- `4b` 設計草案見 [`docs-dev/planning/immutable-phase4b-word-builder-draft.md`](/d:/work/BrailleKit/text-to-braille/docs-dev/planning/immutable-phase4b-word-builder-draft.md)。
- `4b` 目前已落地的相容橋接包括：
  - `BrailleWordBuilder.FromBrailleWord(...)` / `ApplyTo(...)`
  - `EnglishWordConverter` / `UrlConverter` / `EnglishUebConverter` / `TwChineseCharConverter` / `ContextTagConverter` 的 builder materialization 路徑
  - `EnglishBrailleRule` / `GeneralBrailleRule` / `BrailleProcessor` 分數前插 / `TableConverter` 單格替換路徑
  - `ApplyTo(...)` 已改成重用既有 `BrailleWord.Cells` list instance，不再每次重建 `List<BrailleCell>`
  - 最新 workspace snapshot benchmark：
    - 中文單行約 `50.29 us`、`6.32 KB`
    - 英文單行約 `363.10 us`、`23.87 KB`
    - 中英混合單行約 `293.25 us`、`32.91 KB`
- `4b` 之後會碰到 `BrailleCellList` / `BrailleWord` / `BrailleLine` 的資料流與建構模式，風險會明顯高於 `4a`。
- 若後續還要擴大 value type / immutable model 的範圍，應特別注意 reference identity 仍被使用的 `BrailleWord` / `BrailleLine` 路徑。
