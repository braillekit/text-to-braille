# Immutable refactoring - phase 4

## 狀態

Phase 4 目前：

- `4a` `BrailleCell` -> `readonly record struct`
- `4b` 已收尾到可交棒狀態：word-level builder / compatibility 策略已落地並完成效能收斂
- `4c` 已開始第一個實作切點：`BrailleWord` construction boundary 已整理成 model 內部入口
- `4c` 已進入 editor/document identity 熱點清理：剩餘 `ReferenceEquals` 相依已收斂到顯式 word/line identity

`4b` 的完成不代表 `BrailleWord` 已 fully immutable；它代表 `4c` 可以建立在目前這套 hybrid builder / compatibility 邊界上繼續推進。現在 `4c` 已先把 `BrailleWord` 的 construction/materialization 邊界往 model 內部收斂，但 `BrailleWord` 與 `4d` `BrailleLine` 的完整 immutable builder / result 分離仍未完成。

本階段以前一份 [`phase3.md`](./phase3.md) 為起點，先做最小可驗證的高風險 prototype。

## 這一批的重點

### `4c` 第一個切點

- 規劃文件見 [`docs-dev/planning/immutable-phase4c-word-construction-boundary.md`](/d:/work/BrailleKit/text-to-braille/docs-dev/planning/immutable-phase4c-word-construction-boundary.md)
- `BrailleWordBuilder` / `BrailleWordMaterialized` 不再透過外掛 compatibility helper materialize 舊 model
- `BrailleWord` 新增 internal construction boundary：
  - `CreateFromConstruction(...)`
  - `ApplyConstruction(...)`
  - `FromResult(IBrailleWordResult)`
  - `ApplyResult(IBrailleWordResult)`
- `BrailleWord.Copy()` / `Copy(BrailleWord)` 改走同一條 construction path，避免維護第二套欄位搬運邏輯
- `BrailleCellList` 新增 `Assign(ReadOnlySpan<BrailleCell>)`，讓 span-based materialization 可以直接落到既有 cell storage
- 驗證：
  - `dotnet test src/EasyBrailleEditApp/BrailleToolkit.Tests/BrailleToolkit.Tests.csproj`
  - `dotnet build src/EasyBrailleEditApp/BrailleToolkit.Benchmarks/BrailleToolkit.Benchmarks.csproj -c Release`

### `4c` 第二個切點

- `BrailleWordHelper` 新增 internal overload，可直接讀取 `IReadOnlyList<IBrailleWordResult>`：
  - `ToString(...)`
  - `ToTextString(...)`
  - `ToDotNumberString(...)`
  - `ToOriginalTextString(...)`
  - `GetCellCount(...)`
  - `ContainsTitleTag(...)`
- `BrailleFontConverter` 新增 internal `ToString(IBrailleWordResult)`，讓 font rendering 不必先 materialize 回 `BrailleWord`
- 這代表 helper / font rendering 這類 read-only downstream，已可直接消費 materialized result
- 補測試：
  - materialized result list 的 helper 驗證
  - `BrailleFontConverter.ToString(result)` 與 `result.ToBrailleWord()` 等價驗證

### `4c` 第三個切點

- 新增 `BrailleWordSequenceFormatter`
- `BrailleLine` 的下列 read-only formatting 已委派給 sequence formatter：
  - `ToBrailleCellHexString()`
  - `ToPositionNumberString()`
  - `ToHtmlString(...)`
- sequence formatter 同時提供：
  - `IReadOnlyList<BrailleWord>` 版本
  - `IReadOnlyList<IBrailleWordResult>` 版本
- 這讓 HTML export / line rendering 這類 line-level downstream，與 result renderer 共用同一條格式化邏輯
- 補測試：
  - result sequence 與既有 `BrailleLine` 在 hex / position / HTML rendering 上的等價驗證

### `4c` 第四個切點

- 新增 `IBrailleWordView`
- `BrailleWord` 實作 `IBrailleWordView`
- `IBrailleWordResult` 改成繼承 `IBrailleWordView`
- `BrailleWordHelper` / `BrailleFontConverter` / `BrailleWordSequenceFormatter` 的 internal read-only 邏輯已收斂到同一個 `IBrailleWordView` contract
- 這代表 `4c` 的 read-only downstream 邊界已從「BrailleWord/result 雙軌 overload」收斂成單一 word view
- 補測試：
  - mixed `BrailleWord + IBrailleWordResult` sequence 的 helper / formatter 等價驗證

### `4c` 第五個切點

- serializer 熱點：
  - `BrailleDocumentYamlSerializer` serialization 端改由 `IBrailleWordView` 讀取
  - deserialization 端改由 `BrailleWord.CreateFromConstruction(...)` 回建
- mutation 熱點：
  - `BrailleProcessor.ConvertFraction(...)` 的分母尾端符號追加，已改走 `BrailleWordBuilder.FromBrailleWord(...)` / `ApplyTo(...)`
  - 移除剩餘的手動 `lastBrWord.Cells.Add(...)`
- 這代表 `4c` 已不只收斂 read-only downstream，也開始把 production mutation / serialization call site 拉回 construction boundary
- 補測試：
  - YAML round-trip 補 `OriginalText` / `ContextNames` / `IsConvertedFromTag` 驗證
  - 既有 fraction 轉換測試持續通過

### `4c` 第六個切點

- `BrailleWord` / `BrailleLine` 新增執行期 `Identity`
- `BrailleLine.IndexOf(BrailleWord)` 改由 word identity 比對
- `BraillePageTitle` 新增 `ContentStartLineIdentity`
- `BrailleDocument` 下列查找／定位邏輯已改走 line identity：
  - `IsBeginLineOfPageTitle(...)`
  - `FindPageTitleByBeginLine(...)`
  - `IndexOfLine(...)`
- dual-edit editor 的 grid mapping / selection / edit command 幾個剩餘 `ReferenceEquals` 熱點已改用 identity
- 這代表 `4c` 已開始把 editor/document compatibility 從 CLR reference equality 拉回顯式 model identity
- 補測試：
  - builder `ApplyTo(...)` 後保留 `BrailleWord.Identity`
  - page title 在文件插入列後仍可透過 line identity 重新定位 begin line

### `4c` 第七個切點

- `BrailleDocumentFormatter.FormatLine(BrailleDocument, ...)` 在斷行時不再移除原始第一列再整批插入新 line
- 新語意改成：
  - 保留原始第一列物件與其 line identity
  - 將第一個 formatted line 的 words 回填到原始列
  - 只為後續新增的折行結果建立新 `BrailleLine`
- `BrailleLine` 新增內部 `AssignWords(...)`，用來支撐 formatter 這種 whole-object replace 相容邊界
- 這代表 editor commands 最常走到的 `ReformatRow(...)` 主路徑，現在已不會因 formatter 斷行而切斷第一列的 identity
- 補測試：
  - formatter 斷行後保留第一列 `Identity`
  - page title begin-line anchor 在 formatter 斷行後仍指向同一列

### `4c` 第八個切點

- `BrailleWord` / `BrailleLine` 新增 `OnDeserialized` runtime identity 補發邏輯
- `DataContractJsonSerializer` 反序列化後，若物件尚未帶有有效 `Identity`，會自動補發新的 runtime identity
- 這讓下列 editor command 的新物件來源，都能一致取得 fresh logical identity：
  - clipboard copy / cut / paste
  - line clone / selected-word clone
  - `BrailleDocument.DeepCopy()` 與 undo/redo memento
  - paragraph reshape 過程中經過 JSON deep copy 的文件狀態
- 補測試：
  - `BrailleDocument.DeepCopy()` 後 line / word identity 應為 fresh identity
  - document JSON round-trip 後 page title anchor 應與新文件內的 fresh line identity 保持一致

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

### `4b` 正式 clean worktree A/B benchmark

- 日期：2026-04-05
- baseline commit：`ea804795f6cf56bbe9a152ef272adab9130c51db`
- candidate commit：`53a7c22123ae328165e529e5c582d56818a5104b`
- 方法：於兩個乾淨 worktree 各自獨立建置與執行完整 conversion benchmark suite
- 詳細紀錄：
  - [`2026-04-05-phase4b-clean-worktree-ab.md`](./benchmark-result/2026-04-05-phase4b-clean-worktree-ab.md)

#### A/B 摘要

| Method | Baseline Mean | Candidate Mean | Mean Δ | Baseline Alloc | Candidate Alloc | Alloc Δ |
| ---- | ----: | ----: | ----: | ----: | ----: | ----: |
| 中文單行轉換 | 66.03 us | 46.10 us | -30.18% | 5.82 KB | 6.32 KB | +8.59% |
| 英文單行轉換 | 578.35 us | 348.34 us | -39.77% | 16.09 KB | 23.87 KB | +48.35% |
| 中英混合單行轉換 | 383.81 us | 293.50 us | -23.53% | 28.73 KB | 32.91 KB | +14.55% |
| 中文多行轉換 | 3,336.77 us | 2,144.99 us | -35.72% | 327.83 KB | 360.96 KB | +10.11% |
| 英文多行轉換 | 2,810.04 us | 2,124.65 us | -24.39% | 100.21 KB | 147.48 KB | +47.17% |
| 中英混合多行轉換 | 1,374.60 us | 1,476.20 us | +7.39% | 111.28 KB | 132.43 KB | +19.01% |
| 長中文字串轉換 | 2,149.56 us | 2,102.52 us | -2.19% | 329.30 KB | 362.43 KB | +10.06% |

#### 解讀

- `4b` candidate 在 7 個 benchmark 中有 6 個 `Mean` 變快，代表 builder / compatibility bridge 已有效扳回 `4a` 的大部分 throughput regression。
- 改善最明顯的是英文單行 `-39.77%`、中文多行 `-35.72%`、中文單行 `-30.18%`。
- 唯一明確回歸的是中英混合多行 `+7.39%`。
- 但 `Allocated` 在 7 個 benchmark 全部上升，尤其英文單行 `+48.35%`、英文多行 `+47.17%`。
- 目前可把 `4b` 理解成：
  - CPU throughput 方向正確
  - allocation 尚未收斂
  - 下一階段應優先減少 builder 與既有 `BrailleWord` 相容層之間的 materialize / copy 成本

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
- `4b` 收尾 / 交棒摘要見 [`docs-dev/planning/immutable-phase4b-handoff.md`](/d:/work/BrailleKit/text-to-braille/docs-dev/planning/immutable-phase4b-handoff.md)。
- converter 新 word materialization 收斂紀錄見 [`2026-04-05-phase4b-converter-materialization-reduction.md`](./benchmark-result/2026-04-05-phase4b-converter-materialization-reduction.md)：
  - append-only converter 路徑已改回直接 `new BrailleWord(...)`
  - 最新 workspace snapshot benchmark：
    - 中文單行約 `45.17 us`、`5.82 KB`
    - 英文單行約 `332.52 us`、`16.12 KB`
    - 中英混合單行約 `294.37 us`、`28.47 KB`
  - 對應的正式 clean worktree A/B 見 [`2026-04-05-phase4b-converter-materialization-clean-worktree-ab.md`](./benchmark-result/2026-04-05-phase4b-converter-materialization-clean-worktree-ab.md)：
    - baseline：`53a7c22`
    - candidate：`322bc351`
    - allocation 在 7 個 benchmark 全部下降
    - throughput 在 7 個 benchmark 中有 6 個改善
    - 後續 focused clean rerun 顯示中英混合單行也由 `417.8 us` 降到 `374.3 us`，因此原先 `+14.07%` 不視為 blocking regression
- `4b` 目前已落地的相容橋接包括：
  - `BrailleWordBuilder.FromBrailleWord(...)` / `ApplyTo(...)`
  - 既有 word mutation / prepend / replace 的 builder 路徑
  - `EnglishBrailleRule` / `GeneralBrailleRule` / `BrailleProcessor` 分數前插 / `TableConverter` 單格替換路徑
  - `ApplyTo(...)` 已改成重用既有 `BrailleWord.Cells` list instance，不再每次重建 `List<BrailleCell>`
  - builder 與既有 `BrailleWord` 相容層的 allocation micro-benchmark 分析見 [`2026-04-05-phase4b-compat-allocation-analysis.md`](./benchmark-result/2026-04-05-phase4b-compat-allocation-analysis.md)
- `4b` 之後會碰到 `BrailleCellList` / `BrailleWord` / `BrailleLine` 的資料流與建構模式，風險會明顯高於 `4a`。
- 若後續還要擴大 value type / immutable model 的範圍，應特別注意 reference identity 仍被使用的 `BrailleWord` / `BrailleLine` 路徑。

## 4b 收尾結論

- `4b` 已完成目前定義下的交付範圍，可以作為 `4c` 的起點。
- 已驗證可保留的規則是：
  - builder 保留在 prepend / replace / mutation 橋接
  - append-only new word 優先直接 materialize 成既有 `BrailleWord`
- 若接著推 `4c`，建議不要重開 `4b` 的方向討論，而是直接以 `BrailleWord` construction boundary 為下一個切點。
