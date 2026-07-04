# Immutable Design branch 變更整理

> 建立日期：2026-04-07
> 比對範圍：`git diff main...HEAD`
> 分支：`immutable-design`
> `main` / merge-base：`ba9bc71`
> `immutable-design` HEAD：`9fd741a`
> 參考計畫：[`immutable-refactoring-plan.md`](./immutable-refactoring-plan.md)

## 總覽

此分支的目標是將 `BrailleToolkit` 的核心資料結構與查詢表朝 immutable / read-only / builder boundary 重構。和 `main` 相比，整體變更規模為 106 個檔案、約 8094 行新增與 895 行刪除，包含：

- 核心 model：`BrailleCell` 改成 value type；`BrailleWord` / `BrailleLine` 保留 mutable class，但新增 builder、materialized result、read-only view 與 construction boundary。
- 公開集合 API：`BrailleDocument.Lines`、`BrailleDocument.PageTitles`、`BrailleLine.Words` 從公開 `List<T>` 收斂成 `IReadOnlyList<T>`，mutation 改走明確方法。
- 查詢表：`XmlBrailleTable` 從 `DataTable` / `DataRow` / SQL-like filter 改成 `BrailleTableEntry` + `FrozenDictionary` 索引。
- 轉換流程：`BrailleProcessor` 的 line construction 階段改用 `BrailleLineBuilder`；後處理 rule 階段仍 materialize 回 `BrailleLine` 後修改。
- 編輯器相容層：新增 runtime identity，避免 immutable / copy / deep copy 後繼續依賴 `ReferenceEquals`。
- 序列化：JSON 改透過 backing field 維持 `DataContract` 相容；YAML 改用 DTO model round-trip。
- 驗證：新增 `BrailleToolkit.Benchmarks` 專案、benchmark 測資與多份 A/B 結果文件；補強 builder、identity、YAML、table lookup、event args 等測試。

## 知識來源索引

以下標註皆指向 [`immutable-design.md`](./immutable-design.md) 中與本分支變更相對應的解說段落。標註為「對照」者，表示該段落提供設計原則或風險說明，但本分支實作不一定完全採用文件範例中的做法。

| 編號 | 來源段落 | 對應觀念 |
| ---- | ---- | ---- |
| K1 | [4.1 為什麼需要不可變設計？](./immutable-design.md#L13) / [不可變設計的好處](./immutable-design.md#L38) | 降低隱藏副作用、提升可預測性與共享安全性。 |
| K2 | [4.2 struct vs. class：選擇正確的型別](./immutable-design.md#L52) / [實值型別 vs. 參考型別](./immutable-design.md#L58) | `struct` / `class` 的 value semantics 與 reference semantics 差異。 |
| K3 | [何時使用 struct？](./immutable-design.md#L98) / [何時使用 class？](./immutable-design.md#L134) | 小型 immutable value object 適合 struct；較大或需 reference semantics 的型別適合 class。 |
| K4 | [常見錯誤：可變的 struct](./immutable-design.md#L143) / [readonly struct](./immutable-design.md#L199) | 避免 mutable struct；用 `readonly struct` 表達並強制 immutable struct。 |
| K5 | [防禦性複製：隱藏的效能陷阱](./immutable-design.md#L246) / [效能影響](./immutable-design.md#L406) | value type / readonly context 可能帶來 defensive copy，需 benchmark 驗證。 |
| K6 | [Record 的值相等性](./immutable-design.md#L660) | record 由編譯器產生 value equality。 |
| K7 | [record class vs. record struct](./immutable-design.md#L703) / [何時使用 record struct？](./immutable-design.md#L779) | `readonly record struct` 適合小型 immutable value object / lookup key。 |
| K8 | [init 存取子：建立後不可變](./immutable-design.md#L827) / [Init 存取子語法](./immutable-design.md#L833) | 屬性只能在初始化時設定，兼顧 object initializer 與不可變性。 |
| K9 | [with 運算式：非破壞性修改](./immutable-design.md#L902) | 不修改原物件，而是建立修改後的新版本。 |
| K10 | [不可變的領域模型](./immutable-design.md#L1156) | 用 immutable model 降低副作用、支援歷史追蹤與 undo/redo 類場景。 |
| K11 | [相等性比較](./immutable-design.md#L1231) / [參考相等 vs. 值相等](./immutable-design.md#L1235) | 不可變 value object 通常應以內容值比較，而非參考位址。 |
| K12 | [不可變集合](./immutable-design.md#L1367) / [可變集合的問題](./immutable-design.md#L1373) | `IReadOnlyList<T>` 只是唯讀 facade，不等於真正 immutable collection。 |
| K13 | [不可變集合的效能考量](./immutable-design.md#L1416) | 大量建構時使用 builder，最後再轉成 immutable snapshot，可降低中介物件成本。 |

## 主要型別變更

### `BrailleCell`

知識來源：K2、K3、K4、K5、K6、K7、K11。

- `BrailleCell` 從 `sealed class` 改成 `readonly record struct`。
- `Value` 改為 `[DataMember] public byte Value { get; init; }`。
- 保留相容 API：`GetInstance(...)`、`Blank`、`Capital`、`ToHexString()`、`ToPositionNumberString()`、`ToPositionNumberArray()`。
- 保留 256-entry 靜態快取陣列，但快取現在只是 factory 相容入口，不再代表可依賴 reference identity。
- 新語意：`default(BrailleCell)` 等同空方 `Value = 0x00`；equality 改由 record struct value equality 處理。

### `BrailleTableEntry` / `XmlBrailleTable`

知識來源：K6、K7、K12、K13。其中 `FrozenDictionary` 不是 `immutable-design.md` 的範例型別，但此變更和 K12 / K13 的「建立後穩定查詢」與「建構完成後凍結」原則對應。

- 新增 `readonly record struct BrailleTableEntry(...)` 表示 XML `<symbol>` 的單一 immutable entry。
- `XmlBrailleTable` 不再使用 `DataTable`，改用 `XDocument` 解析 XML，載入時直接把 `dots` / `dots2` 轉換為 `Code` / `Code2`。
- 載入後建立三組 frozen index：
  - `text -> BrailleTableEntry`
  - `(type, text) -> BrailleTableEntry`
  - `type -> BrailleTableEntry[]`
- 新增 `Entries` 唯讀檢視，以及 `FindEntry(...)` / `FindEntriesByType(...)` lookup helper。
- concrete table (`EnglishBrailleTable`、`EnglishUebBrailleTable`、`TwChineseBrailleTable`、`UrlBrailleTable`) 改走 typed lookup，不再組 `DataTable.Select(...)` filter 字串。

### `BrailleWord`

知識來源：K1、K3、K8、K10、K11、K12。注意：`BrailleWord` 仍保留 mutable class，是依 K3 的 class 適用情境與現有 reference semantics 需求做出的折衷。

- `BrailleWord` 仍是 `sealed class`，但新增 `IBrailleWordView` 唯讀檢視介面。
- 新增 runtime `Identity`，由 `BrailleObjectIdentityGenerator.NextWordIdentity()` 產生；反序列化後若缺 identity 會補發。
- `OriginalText` 對外改成 `init`，內部透過 `SetOriginalText(...)` 支援 construction boundary。
- 新增 internal construction boundary：
  - `FromResult(IBrailleWordResult)`
  - `ApplyResult(IBrailleWordResult)`
  - `CreateFromConstruction(...)`
  - `ApplyConstruction(...)`
- `Copy()` / `Copy(BrailleWord)` 改走同一套 construction path，避免欄位搬運邏輯分散。

### `BrailleWordBuilder` / `IBrailleWordResult`

知識來源：K9、K10、K12、K13。builder / materialized result 的分層對應 K13 的 builder 模式，但此分支使用專案內部型別，不是直接改用 `System.Collections.Immutable`。

- 新增 `BrailleWordBuilder` 作為 word-level mutable builder。
- 新增 `IBrailleWordResult : IBrailleWordView` 與 `BrailleWordMaterialized`，代表 builder 完成後的唯讀結果。
- builder 支援 cell mutation：`AppendCell`、`AppendCells`、`AppendHex`、`AppendPositionNumbers`、`PrependCell`、`ReplaceCell`、`ClearCells`。
- builder 支援 materialization：`Build()`、`ToBrailleWord()`、`ApplyTo(BrailleWord)`。
- `BrailleCellBuffer` 作為 internal deque-like buffer，支援 prepend / append，減少在建構階段直接操作 `List<BrailleCell>`。
- `BrailleCellList.Assign(ReadOnlySpan<BrailleCell>)` 新增為 span-based materialization helper。

### `BrailleLine`

知識來源：K3、K10、K11、K12、K13。注意：`Words` 改成 `IReadOnlyList<T>` 只符合「公開唯讀 facade」的中間步驟；K12 也提醒這不等於真正 immutable collection。

- `BrailleLine` 仍是 mutable class，但新增 `IBrailleLineView` 唯讀檢視介面。
- `Words` 從公開 `List<BrailleWord>` 改成 `IReadOnlyList<BrailleWord>`，內部由 `[DataMember(Name = "Words")] private List<BrailleWord> m_Words` 儲存。
- 新增 runtime `Identity`，由 `BrailleObjectIdentityGenerator.NextLineIdentity()` 產生；反序列化後若缺 identity 會補發。
- 新增明確 mutation API：`AddWord`、`AddWords`、`Insert`、`InsertWords`、`RemoveAt`、`RemoveRange`、`AssignWords`、`ApplyResult`。
- `IndexOf(BrailleWord)` 改用 `BrailleWord.Identity` 比對，取代 reference equality。
- read-only query / formatting 部分逐步委派到 `BrailleLineHelper` 與 `BrailleWordSequenceFormatter`。

### `BrailleLineBuilder` / `IBrailleLineResult`

知識來源：K9、K10、K12、K13。此處的 builder / materialized result 也是「建構階段可變、完成後提供 snapshot」的實作。

- 新增 `IBrailleLineResult : IBrailleLineView` 與 `BrailleLineMaterialized`，代表 line builder 完成後的 immutable snapshot。
- 新增 `BrailleLineBuilder`，支援與 `BrailleLine` 類似的 mutation API：`AddWord`、`AddWords`、`Insert`、`InsertWords`、`RemoveAt`、`RemoveRange`、`TrimStart`、`TrimEnd`、`Trim`、`RemoveContextTags`。
- materialization API：`Build()`、`ToBrailleLine()`、`ApplyTo(BrailleLine)`。
- `BrailleProcessor` 的 initial conversion 階段與 `BrailleDocumentFormatter.BreakLine(...)` 的新行建構改走 builder；後續 rule / editor mutation 仍落在 materialized `BrailleLine`。

### `BrailleDocument` / `BraillePageTitle`

知識來源：K1、K3、K10、K11、K12。runtime identity 是針對此專案仍保留 mutable class / editor reference workflow 的相容設計，對應 K11 的「參考相等 vs. 值相等」問題意識。

- `BrailleDocument.Lines` 與 `BrailleDocument.PageTitles` 對外改成 `IReadOnlyList<T>`，內部仍由 `m_Lines` / `m_PageTitles` 管理。
- 新增 document mutation API：`InsertLine`、`InsertLines`、`RemoveLine`、`ClearPageTitles`、`IndexOfLine(long lineIdentity)`。
- `BraillePageTitle` 新增 `ContentStartLineIdentity`，標題錨點由原本依賴 `ContentStartLineRef` / stored index，改成優先用 line identity 解析。
- 新增 `TryResolveContentStartLineIndex(...)`，支援插入列或 formatter 斷行後先由 identity 找回 begin line，再 fallback 到既有 index。
- `BrailleDocumentFormatter.FormatLine(...)` 斷行時保留原始第一列物件與 identity，只回填第一個 formatted line 的內容，後續新增列才插入新 line。

### context tag 與 static lookup

知識來源：K1、K8、K12。static lookup 改成 frozen collection 對應「建立後不修改、可安全共享」的原則；tag 組態屬性改 `init` 對應 K8。

- `ContextTagNames.Collection`：`HashSet<string>` -> `readonly FrozenSet<string>`。
- `SimpleTag.Tags`：`Dictionary<string, string>` -> `readonly FrozenDictionary<string, string>`。
- `GenericContextTag` / `IContextTag` 的組態屬性改為 `init` / `protected init`，衍生 tag 改由 base constructor 傳入 `lifeTime`、`removeTagOnConversion`、`singleLine` 等組態。
- `ContextTagManager.Tags` 對外改為 `IReadOnlyDictionary<string, IContextTag>`。

## 序列化與相容性

知識來源：K8、K10、K12。DTO 與 construction boundary 是為了保留既有序列化格式，同時避免公開 mutable collection。

- JSON / DataContract：
  - `BrailleLine.Words` 改由 backing field `m_Words` 以 `[DataMember(Name = "Words")]` 序列化。
  - `Words` / `Lines` / `PageTitles` 等公開唯讀 facade 以 `[IgnoreDataMember]` 排除。
  - `BrailleWord` / `BrailleLine` 在 `OnDeserialized` 後補發 runtime identity。
- YAML：
  - `BrailleDocumentYamlSerializer` 改用 private YAML DTO：`BrailleDocumentYamlModel`、`BrailleLineYamlModel`、`BrailleWordYamlModel`、`BraillePageTitleYamlModel`。
  - serialize 時從 read-only view 讀取；deserialize 時走 `BrailleLineBuilder` / `BrailleWord.CreateFromConstruction(...)`。
- Clipboard / editor：
  - `ClipboardHelper` 接受 `IReadOnlyList<T>`，序列化前複製成 `List<T>`，維持既有 clipboard JSON 格式。
  - undo/redo memento 不再只保存 grid raw position，改保存 line/word identity bookmark。

## converter / rule / UI 邊界調整

知識來源：K1、K9、K10、K11、K13。converter/rule/UI 調整主要是把建構期 mutation 收斂到 builder / construction boundary，並把 editor 狀態從 reference equality 轉成 explicit identity。

- converter 類別把 append-only 建構路徑改回直接 `new BrailleWord(text, code)`，避免不必要的 builder materialization。
- 必須修改既有 word cell 的地方改走 `BrailleWordBuilder.FromBrailleWord(...).ApplyTo(...)`，例如分數符號、英文大寫符號、數字符號、表格線符號等。
- braille rules 不再直接 `brLine.Words.Insert(...)` / `RemoveRange(...)`，改用 `BrailleLine.Insert(...)` / `RemoveRange(...)`。
- `PreviewPanel.UpdatePreview(...)` 改吃 `IReadOnlyList<BrailleLine>?`。
- `BrailleGridPositionMapper` 新增 identity lookup 與 merged-cell aware 的 `TryGetBrailleWordAtGridPosition(...)`。
- `BrailleEditMemento` / `BrailleGridState` 改以 model identity 還原 active cell 與 selection。
- `InProcessBrailleConverter` 輸出暫存檔名加入 GUID，避免測試或並行轉換共用固定檔名。

## 逐檔案變更清單

### 專案根目錄與 agent 文件

| 檔案 | 變更摘要 |
| ---- | ---- |
| `.claude/settings.json` | 新增 Claude 權限設定與額外目錄設定。 |
| `AGENT_CONTEXT.md -> AGENTS.md` | 以 rename 方式改為標準 agent 指示檔名，內容包含專案概觀、建置測試方式、點字架構與重構指示。 |
| `immutable-design.md` | 新增 immutable design 初始討論與背景紀錄。 |

### planning 文件

| 檔案 | 變更摘要 |
| ---- | ---- |
| `docs-dev/planning/immutable-phase4b-handoff.md` | 新增 Phase 4b 交棒摘要。 |
| `docs-dev/planning/immutable-phase4b-word-builder-draft.md` | 新增 word builder 設計草案。 |
| `docs-dev/planning/immutable-phase4c-handoff.md` | 新增 Phase 4c 交棒摘要。 |
| `docs-dev/planning/immutable-phase4c-word-construction-boundary.md` | 新增 word construction boundary 實作規劃。 |

### refactoring 文件與 benchmark 結果

| 檔案 | 變更摘要 |
| ---- | ---- |
| `docs-dev/refactoring/immutable/immutable-refactoring-plan.md` | 新增完整 immutable refactoring 原始計畫。 |
| `docs-dev/refactoring/immutable/phase1.md` | 新增 Phase 1 結案文件，記錄 Frozen collection / event-independent changes。 |
| `docs-dev/refactoring/immutable/phase2.md` | 新增 Phase 2 結案文件，記錄 read-only public API 與 YAML 相容處理。 |
| `docs-dev/refactoring/immutable/phase3.md` | 新增 Phase 3 結案文件，記錄 DataTable -> immutable entry / frozen index。 |
| `docs-dev/refactoring/immutable/phase4-cause-analysis.md` | 新增 Phase 4 performance cause analysis。 |
| `docs-dev/refactoring/immutable/phase4.md` | 新增 Phase 4 詳細進度與 benchmark 解讀。 |
| `docs-dev/refactoring/immutable/benchmark-result/2026-04-05-baseline.md` | 新增 baseline benchmark 紀錄。 |
| `docs-dev/refactoring/immutable/benchmark-result/2026-04-05-phase1.md` | 新增 Phase 1 初次 benchmark 紀錄。 |
| `docs-dev/refactoring/immutable/benchmark-result/2026-04-05-phase1-ab-rerun.md` | 新增 Phase 1 clean A/B 重跑紀錄。 |
| `docs-dev/refactoring/immutable/benchmark-result/2026-04-05-phase2-checkpoint-2a-2e-2f-2g.md` | 新增 Phase 2 checkpoint benchmark。 |
| `docs-dev/refactoring/immutable/benchmark-result/2026-04-05-phase2-clean-worktree-ab.md` | 新增 Phase 2 clean worktree A/B benchmark。 |
| `docs-dev/refactoring/immutable/benchmark-result/2026-04-05-phase3-current.md` | 新增 Phase 3 current snapshot benchmark。 |
| `docs-dev/refactoring/immutable/benchmark-result/2026-04-05-phase3-clean-worktree-ab.md` | 新增 Phase 3 clean A/B benchmark。 |
| `docs-dev/refactoring/immutable/benchmark-result/2026-04-05-phase4-prototype-validation.md` | 新增 Phase 4 prototype validation benchmark。 |
| `docs-dev/refactoring/immutable/benchmark-result/2026-04-05-phase4a-current.md` | 新增 Phase 4a current snapshot benchmark。 |
| `docs-dev/refactoring/immutable/benchmark-result/2026-04-05-phase4a-clean-worktree-ab.md` | 新增 Phase 4a clean A/B benchmark。 |
| `docs-dev/refactoring/immutable/benchmark-result/2026-04-05-phase4b-clean-worktree-ab.md` | 新增 Phase 4b clean A/B benchmark。 |
| `docs-dev/refactoring/immutable/benchmark-result/2026-04-05-phase4b-compat-allocation-analysis.md` | 新增 Phase 4b compatibility allocation analysis。 |
| `docs-dev/refactoring/immutable/benchmark-result/2026-04-05-phase4b-converter-materialization-reduction.md` | 新增 converter materialization reduction benchmark 紀錄。 |
| `docs-dev/refactoring/immutable/benchmark-result/2026-04-05-phase4b-converter-materialization-clean-worktree-ab.md` | 新增 converter materialization clean A/B benchmark。 |
| `docs-dev/refactoring/immutable/benchmark-result/2026-04-06-branch-start-vs-phase4d-clean-worktree-ab.md` | 新增 branch start vs Phase 4d clean A/B benchmark。 |
| `docs-dev/refactoring/immutable/benchmark-result/2026-04-06-phase4c-vs-phase4d-clean-worktree-ab.md` | 新增 Phase 4c vs Phase 4d clean A/B benchmark。 |

### benchmark 專案

本節知識來源：K5、K13。benchmark 專案用來驗證 value type / builder / immutable boundary 的效能假設與風險。

| 檔案 | 變更摘要 |
| ---- | ---- |
| `src/EasyBrailleEditApp/BrailleToolkit.Benchmarks/BrailleToolkit.Benchmarks.csproj` | 新增 BenchmarkDotNet 專案。 |
| `src/EasyBrailleEditApp/BrailleToolkit.Benchmarks/Program.cs` | 新增 `BenchmarkSwitcher` 入口。 |
| `src/EasyBrailleEditApp/BrailleToolkit.Benchmarks/BrailleConversionBenchmarks.cs` | 新增正式點字轉換 benchmark。 |
| `src/EasyBrailleEditApp/BrailleToolkit.Benchmarks/BrailleCellStoragePrototypeBenchmarks.cs` | 新增 BrailleCell 儲存策略 prototype benchmark。 |
| `src/EasyBrailleEditApp/BrailleToolkit.Benchmarks/BrailleWordBuilderPrototypeBenchmarks.cs` | 新增 word builder prototype benchmark。 |
| `src/EasyBrailleEditApp/BrailleToolkit.Benchmarks/BrailleWordCompatibilityBenchmarks.cs` | 新增 word construction / compatibility benchmark。 |
| `src/EasyBrailleEditApp/BrailleToolkit.Benchmarks/TestData/BenchmarkText_Chinese.txt` | 新增中文 benchmark 測資。 |
| `src/EasyBrailleEditApp/BrailleToolkit.Benchmarks/TestData/BenchmarkText_English.txt` | 新增英文 benchmark 測資。 |
| `src/EasyBrailleEditApp/BrailleToolkit.Benchmarks/TestData/BenchmarkText_Mixed.txt` | 新增中英混合 benchmark 測資。 |

### BrailleToolkit core model

本節知識來源：K1、K2、K3、K4、K5、K6、K7、K8、K9、K10、K11、K12、K13。核心 model 是本分支主要落點，涵蓋 value type、record struct、init、read-only facade、builder 與 identity 相容層。

| 檔案 | 變更摘要 |
| ---- | ---- |
| `src/EasyBrailleEditApp/BrailleToolkit/BrailleCell.cs` | `sealed class` -> `readonly record struct`；`Value` 改 `init`；保留 factory 與格式化 API。 |
| `src/EasyBrailleEditApp/BrailleToolkit/BrailleCellBuffer.cs` | 新增 builder 階段使用的 internal cell buffer，支援 prepend / append / span materialization。 |
| `src/EasyBrailleEditApp/BrailleToolkit/BrailleCellList.cs` | 新增 `Assign(ReadOnlySpan<BrailleCell>)`，支援從 builder / result 快速回填既有 cell list。 |
| `src/EasyBrailleEditApp/BrailleToolkit/BrailleWord.cs` | 實作 `IBrailleWordView`；新增 `Identity`；`OriginalText` 改 `init`；新增 construction boundary 與 result apply path。 |
| `src/EasyBrailleEditApp/BrailleToolkit/BrailleWordBuilder.cs` | 新增 `BrailleWordBuilder`、`IBrailleWordResult`、`BrailleWordMaterialized`。 |
| `src/EasyBrailleEditApp/BrailleToolkit/IBrailleWordView.cs` | 新增 word-level read-only view contract。 |
| `src/EasyBrailleEditApp/BrailleToolkit/BrailleLine.cs` | 實作 `IBrailleLineView`；`Words` 改 `IReadOnlyList`；新增 `Identity`、mutation API、construction boundary。 |
| `src/EasyBrailleEditApp/BrailleToolkit/BrailleLineBuilder.cs` | 新增 `BrailleLineBuilder`、`IBrailleLineResult`、`BrailleLineMaterialized`。 |
| `src/EasyBrailleEditApp/BrailleToolkit/IBrailleLineView.cs` | 新增 line-level read-only view contract。 |
| `src/EasyBrailleEditApp/BrailleToolkit/BrailleObjectIdentityGenerator.cs` | 新增 word / line runtime identity generator。 |
| `src/EasyBrailleEditApp/BrailleToolkit/BrailleDocument.cs` | `Lines` / `PageTitles` 改 `IReadOnlyList`；新增明確 mutation API；page title lookup 改用 line identity。 |
| `src/EasyBrailleEditApp/BrailleToolkit/BraillePageTitle.cs` | 新增 `ContentStartLineIdentity` 與 `TryResolveContentStartLineIndex(...)`；title line 建構改用 builder。 |
| `src/EasyBrailleEditApp/BrailleToolkit/BrailleDocumentFormatter.cs` | 斷行時保留第一列 identity；新行建構改走 `BrailleLineBuilder`。 |
| `src/EasyBrailleEditApp/BrailleToolkit/BrailleGlobals.cs` | `ChinesePunctuations` 加上 `readonly`。 |
| `src/EasyBrailleEditApp/BrailleToolkit/BrailleProcessor.cs` | `CharPosition` 改 `readonly record struct`；event args 改 init-only；`_autoReplacedText` 改 `FrozenDictionary`；line construction 改走 builder。 |
| `src/EasyBrailleEditApp/BrailleToolkit/ContextTagManager.cs` | `Tags` 對外改為 `IReadOnlyDictionary<string, IContextTag>`。 |
| `src/EasyBrailleEditApp/BrailleToolkit/Properties/AssemblyInfo.cs` | 新增 `InternalsVisibleTo("BrailleToolkit.Tests")` 與 `InternalsVisibleTo("BrailleToolkit.Benchmarks")`。 |

### BrailleToolkit data table

本節知識來源：K6、K7、K12、K13。`BrailleTableEntry` 對應 record struct / value equality；table lookup 對應建立後穩定查詢與 builder/freeze 原則。

| 檔案 | 變更摘要 |
| ---- | ---- |
| `src/EasyBrailleEditApp/BrailleToolkit/Data/BrailleTableEntry.cs` | 新增 immutable table entry `readonly record struct`。 |
| `src/EasyBrailleEditApp/BrailleToolkit/Data/XmlBrailleTable.cs` | `DataTable` -> `XDocument` + `BrailleTableEntry[]` + frozen indexes；新增 `Entries` 與 lookup helper。 |
| `src/EasyBrailleEditApp/BrailleToolkit/Data/EnglishBrailleTable.cs` | `FindLetter` / `FindDigit` 改用 `FindEntry(...)`。 |
| `src/EasyBrailleEditApp/BrailleToolkit/Data/EnglishUebBrailleTable.cs` | `FindLetter` 改用 `FindEntry(...)`。 |
| `src/EasyBrailleEditApp/BrailleToolkit/Data/TwChineseBrailleTable.cs` | phonetic / joined / mono / tone / punctuation lookup 改用 typed immutable entry lookup。 |
| `src/EasyBrailleEditApp/BrailleToolkit/Data/UrlBrailleTable.cs` | `FindLetter` / `FindDigit` 改用 `FindEntry(...)`。 |

### BrailleToolkit helpers

本節知識來源：K8、K10、K12、K13。helper 層主要支援 read-only view、DTO round-trip 與 builder/result 共同格式化。

| 檔案 | 變更摘要 |
| ---- | ---- |
| `src/EasyBrailleEditApp/BrailleToolkit/Helpers/BrailleDocumentYamlSerializer.cs` | YAML serialize / deserialize 改用 DTO model；deserialization 走 builder / construction boundary。 |
| `src/EasyBrailleEditApp/BrailleToolkit/Helpers/BrailleLineHelper.cs` | 新增 line view query helper，供 `BrailleLine` / materialized line 共用。 |
| `src/EasyBrailleEditApp/BrailleToolkit/Helpers/BrailleWordHelper.cs` | helper 參數改支援 `IReadOnlyList` / `IBrailleWordView`；抽出 debug/original text/cell formatting helper。 |
| `src/EasyBrailleEditApp/BrailleToolkit/Helpers/BrailleWordSequenceFormatter.cs` | 新增 word sequence formatter，支援 hex、position number、HTML rendering。 |

### BrailleToolkit converters

本節知識來源：K1、K9、K13。converter 調整對應「建構階段可變、完成後 materialize」與避免不必要 materialization 的效能考量。

| 檔案 | 變更摘要 |
| ---- | ---- |
| `src/EasyBrailleEditApp/BrailleToolkit/Converters/BrailleCharConverter.cs` | `Dictionary` -> `FrozenDictionary`；載入方法回傳 frozen table。 |
| `src/EasyBrailleEditApp/BrailleToolkit/Converters/BrailleFontConverter.cs` | `Hashtable` -> `FrozenDictionary`；新增 `IBrailleWordView` rendering overload；span-based cell rendering。 |
| `src/EasyBrailleEditApp/BrailleToolkit/Converters/EnglishUebConverter.cs` | 內部 contraction table 從 `BrailleWord` 值改存 hex code string，匹配時再建立 `BrailleWord`，降低 materialization 成本。 |
| `src/EasyBrailleEditApp/BrailleToolkit/Converters/EnglishWordConverter.cs` | append-only 建構改用 `new BrailleWord(text, code)`；time context colon 前綴 cell 改走 `CellList.Insert`。 |
| `src/EasyBrailleEditApp/BrailleToolkit/Converters/TableConverter.cs` | 修改既有 word cell 時改用 `BrailleWordBuilder.FromBrailleWord(...).ApplyTo(...)`。 |
| `src/EasyBrailleEditApp/BrailleToolkit/Converters/TwChineseCharConverter.cs` | append-only 建構改用 `new BrailleWord(...)`；中文字 phonetic path 直接初始化 metadata。 |
| `src/EasyBrailleEditApp/BrailleToolkit/Converters/UrlConverter.cs` | append-only 建構改用 `new BrailleWord(text, code)`。 |
| `src/EasyBrailleEditApp/BrailleToolkit/Converters/WordConverter.cs` | append-only 建構改用 `new BrailleWord(text, code)`。 |

### BrailleToolkit rules

本節知識來源：K9、K13。rule 層仍是 mutation boundary，但改由明確 API 或 builder 進行局部非破壞式更新。

| 檔案 | 變更摘要 |
| ---- | ---- |
| `src/EasyBrailleEditApp/BrailleToolkit/Rules/ChineseBrailleRule.cs` | `brLine.Words.Insert/RemoveRange` 改用 `BrailleLine` 明確 mutation API。 |
| `src/EasyBrailleEditApp/BrailleToolkit/Rules/EnglishBrailleRule.cs` | 大寫符號 / 編號數字 cell 修改改走 builder；line mutation 改走 `BrailleLine` API。 |
| `src/EasyBrailleEditApp/BrailleToolkit/Rules/GeneralBrailleRule.cs` | space insertion 與 digit cell prepend 改用 `BrailleLine` API / `BrailleWordBuilder`。 |

### BrailleToolkit tags

本節知識來源：K1、K8、K12。tag 組態改 init-only；static collection 改為建立後不修改的 lookup。

| 檔案 | 變更摘要 |
| ---- | ---- |
| `src/EasyBrailleEditApp/BrailleToolkit/Tags/ContextTagNames.cs` | `Collection` 改成 `readonly FrozenSet<string>`。 |
| `src/EasyBrailleEditApp/BrailleToolkit/Tags/SimpleTag.cs` | `Tags` 改成 `readonly FrozenDictionary<string, string>`。 |
| `src/EasyBrailleEditApp/BrailleToolkit/Tags/IContextTag.cs` | `ConvertablePrefix` / `ConvertablePostfix` 改為 init-only contract。 |
| `src/EasyBrailleEditApp/BrailleToolkit/Tags/GenericContextTag.cs` | 組態屬性改為 `init` / `protected init`；constructor 負責設定 tag 組態。 |
| `src/EasyBrailleEditApp/BrailleToolkit/Tags/OrgPageNumberContextTag.cs` | `IsSingleLine` 改由 base constructor 參數設定。 |
| `src/EasyBrailleEditApp/BrailleToolkit/Tags/TableTopLineContextTag.cs` | transient / remove-on-conversion / single-line 組態改由 base constructor 設定。 |
| `src/EasyBrailleEditApp/BrailleToolkit/Tags/TableBottomLineContextTag.cs` | transient / remove-on-conversion / single-line 組態改由 base constructor 設定。 |
| `src/EasyBrailleEditApp/BrailleToolkit/Tags/TableSingleLineContextTag.cs` | transient / remove-on-conversion / single-line 組態改由 base constructor 設定。 |

### EasyBrailleEdit app

本節知識來源：K10、K11、K12。app/editor 變更主要是配合 read-only collection 與 explicit identity，支援 undo/redo 與 grid selection 還原。

| 檔案 | 變更摘要 |
| ---- | ---- |
| `src/EasyBrailleEditApp/EasyBrailleEdit/Controls/PreviewPanel.cs` | `UpdatePreview` 參數改為 `IReadOnlyList<BrailleLine>?`。 |
| `src/EasyBrailleEditApp/EasyBrailleEdit/DualEdit/BrailleEditMemento.cs` | 新增 `BrailleGridCellBookmark` / `BrailleGridRangeBookmark`；undo/redo state 由 raw grid position 改成 identity-based bookmark。 |
| `src/EasyBrailleEditApp/EasyBrailleEdit/DualEdit/BrailleGridController.cs` | word comparison 由 `ReferenceEquals` 改用 `BrailleWord.Identity`。 |
| `src/EasyBrailleEditApp/EasyBrailleEdit/DualEdit/BrailleGridController_EditCommands.cs` | 改用 `BrailleDocument` / `BrailleLine` mutation API；memento restore 改用 identity-based `GridState.Restore(...)`。 |
| `src/EasyBrailleEditApp/EasyBrailleEdit/DualEdit/BrailleGridPositionMapper.cs` | 新增 line/word identity lookup；新增 merged-cell aware word position lookup；grid column lookup 改用 identity。 |
| `src/EasyBrailleEditApp/EasyBrailleEdit/DualEdit/ClipboardHelper.cs` | `SetWords` / `SetLines` 接受 `IReadOnlyList<T>`，序列化前複製成 `List<T>`。 |
| `src/EasyBrailleEditApp/EasyBrailleEdit/DualEditForm.cs` | page title 操作改用 `ClearPageTitles()` / `AddPageTitle(...)`。 |
| `src/EasyBrailleEditApp/EasyBrailleEdit/DualEditTitleForm.cs` | title begin-line 顯示改用 `TryResolveContentStartLineIndex(...)`，不再依賴 `TitleLine.Tag`。 |
| `src/EasyBrailleEditApp/EasyBrailleEdit/Services/ExternalBrailleConverter.cs` | 配合 `CharPosition` record struct constructor 調整 invalid char 建立方式。 |
| `src/EasyBrailleEditApp/EasyBrailleEdit/Services/InProcessBrailleConverter.cs` | 輸出暫存檔名加入 GUID，避免固定檔名衝突。 |

### solution

| 檔案 | 變更摘要 |
| ---- | ---- |
| `src/EasyBrailleEditApp/EasyBrailleEditApp.sln` | 加入 `BrailleToolkit.Benchmarks` 專案與組態。 |

### BrailleToolkit tests

本節知識來源：K5。測試補強用來驗證 immutable / identity / serialization / builder 變更沒有破壞既有語意；benchmark 則覆蓋效能風險。

| 檔案 | 變更摘要 |
| ---- | ---- |
| `src/EasyBrailleEditApp/BrailleToolkit.Tests/BrailleCellTest.cs` | 新增 value equality、default blank、JSON round-trip 測試。 |
| `src/EasyBrailleEditApp/BrailleToolkit.Tests/BrailleDocumentTest.cs` | 新增 page title identity anchor、formatter identity preservation、deep copy fresh identity、JSON round-trip anchor 測試。 |
| `src/EasyBrailleEditApp/BrailleToolkit.Tests/BrailleLineBuilderTest.cs` | 新增 line builder / materialized result / identity preservation 測試。 |
| `src/EasyBrailleEditApp/BrailleToolkit.Tests/BrailleProcessorEventTest.cs` | 新增 event args 每次 raise 都建立新實例的測試。 |
| `src/EasyBrailleEditApp/BrailleToolkit.Tests/BrailleWordBuilderTest.cs` | 新增 word builder、materialized result、read-only helper、formatter、identity preservation 測試。 |
| `src/EasyBrailleEditApp/BrailleToolkit.Tests/BrailleWordTest.cs` | 新增 `OriginalText` 與 metadata copy 測試。 |
| `src/EasyBrailleEditApp/BrailleToolkit.Tests/EnglishWordConverterTest.cs` | 新增 time context colon prepend 456 cell 測試。 |
| `src/EasyBrailleEditApp/BrailleToolkit.Tests/XmlBrailleTableTest.cs` | 新增 immutable entries、typed lookup、concrete table smoke test。 |
| `src/EasyBrailleEditApp/BrailleToolkit.Tests/YamlSerializationTests.cs` | 調整測試建構方式並補 `OriginalText` / `ContextNames` / `IsConvertedFromTag` round-trip 驗證。 |

### EasyBrailleEdit tests

本節知識來源：K10、K11。測試補強主要驗證 editor 狀態可透過 model identity 還原，而不是依賴 reference equality。

| 檔案 | 變更摘要 |
| ---- | ---- |
| `src/EasyBrailleEditApp/EasyBrailleEdit.Tests/DualEdit/BrailleGridStateTests.cs` | 新增 identity-based grid bookmark / restore 測試。 |
| `src/EasyBrailleEditApp/EasyBrailleEdit.Tests/DualEdit/ClipboardHelperTests.cs` | 改用 `AddWord(...)` 建立測試資料，配合 `Words` read-only API。 |

## 驗證紀錄

此文件是根據目前工作區執行的 `git diff main...HEAD`、`git diff --stat main...HEAD`、相關核心檔案 diff，以及既有 phase 文件整理而成。既有 phase 文件記錄過的主要驗證包含：

- `dotnet build src/EasyBrailleEditApp/EasyBrailleEditApp.sln`
- `dotnet test src/EasyBrailleEditApp/BrailleToolkit.Tests/BrailleToolkit.Tests.csproj`
- `dotnet test src/EasyBrailleEditApp/EasyBrailleEdit.Tests/EasyBrailleEdit.Tests.csproj`
- `dotnet run --project src/EasyBrailleEditApp/BrailleToolkit.Benchmarks/BrailleToolkit.Benchmarks.csproj -c Release`

本文件本身新增後尚未重新執行 build / test，因為這次只新增 Markdown 整理文件。
