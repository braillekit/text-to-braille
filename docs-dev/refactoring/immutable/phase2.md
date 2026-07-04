# Immutable refactoring - phase 2

## 狀態

Phase 2 已完成：

- `2a` `BrailleWord.OriginalText` setter → `init`
- `2b` `BrailleDocument.Lines` → 公開 `IReadOnlyList<BrailleLine>`
- `2c` `BrailleDocument.PageTitles` → 公開 `IReadOnlyList<BraillePageTitle>`
- `2d` `BrailleLine.Words` → 公開 `IReadOnlyList<BrailleWord>`
- `2e` `ContextTagManager.Tags` → `IReadOnlyDictionary<string, IContextTag>`
- `2f` `ConversionFailedEventArgs` / `TextConvertedEventArgs` → `init-only`
- `2g` `GenericContextTag` 組態屬性 → `init-only`

## 這一批的重點

### `2a`

- `BrailleWord.OriginalText` 改為公開 `init`。
- 補強 `BrailleWord.Copy()` / `Copy(BrailleWord)`，確保複製時保留 `OriginalText`。

### `2b` / `2c` / `2d`

- `BrailleDocument.Lines`、`BrailleDocument.PageTitles`、`BrailleLine.Words` 對外全面收斂成唯讀集合。
- 內部仍維持 `List<T>` backing field：
  - `BrailleDocument.m_Lines`
  - `BrailleDocument.m_PageTitles`
  - `BrailleLine.m_Words`
- 為了讓既有編輯流程不需要回退成公開 mutable collection，補了明確的內部操作 API：
  - `BrailleLine.AddWord`
  - `BrailleLine.AddWords`
  - `BrailleLine.InsertWords`
  - `BrailleDocument.InsertLine`
  - `BrailleDocument.ClearPageTitles`
  - `BrailleDocument.IndexOfLine`
- 相關 formatter、rules、clipboard、preview 與 dual-edit call site 已一併調整成使用唯讀集合或新的 mutation 方法。

### 序列化相容處理

- JSON：
  - `BrailleLine.Words` 改由 backing field `m_Words` 以 `[DataMember(Name = "Words")]` 參與序列化。
  - `Words` / `Lines` / `PageTitles` 公開唯讀 façade 以 `[IgnoreDataMember]` 排除，避免 `DataContractJsonSerializer` 直接面對 `IReadOnlyList<T>`。
- YAML：
  - `BrailleDocumentYamlSerializer` 改為透過中介 DTO 進行 YAML serialize / deserialize。
  - 這樣可以保留 `.byml` 既有欄位名稱與向下相容行為，同時讓 runtime model 對外維持只讀 API。

### `2e`

- `ContextTagManager.Tags` 對外改為 `IReadOnlyDictionary<string, IContextTag>`。
- 內部仍維持原本字典結構，不改動 context 狀態管理流程。

### `2f`

- `ConversionFailedEventArgs` 與 `TextConvertedEventArgs` 的資料屬性改為 `init-only`。
- `BrailleProcessor` 改成每次觸發事件都建立新 event args 實例。
- `ConversionFailedEventArgs.Stop` 仍保留可變，維持事件處理器可中止轉換的既有行為。

### `2g`

- `GenericContextTag` 的組態屬性改為建構期設定。
- 衍生 tag 改成在 base constructor 傳入組態，減少建構後再修改狀態的空間。

### 附帶修正

- `InProcessBrailleConverter` 的輸出暫存檔名改為帶 GUID 的唯一檔名。
- 這不是 Phase 2 核心需求，但可以避免 full test run 時因固定暫存檔名而互相污染，讓驗證結果穩定。

## 主要變更檔案

| 檔案 | 變更 |
| ---- | ---- |
| [BrailleWord.cs](/src/EasyBrailleEditApp/BrailleToolkit/BrailleWord.cs) | `OriginalText` 改為 `init`；補強複製邏輯 |
| [BrailleLine.cs](/src/EasyBrailleEditApp/BrailleToolkit/BrailleLine.cs) | `Words` 對外改為 `IReadOnlyList`；新增 `m_Words` 與相關 mutation API；調整 JSON 序列化 |
| [BrailleDocument.cs](/src/EasyBrailleEditApp/BrailleToolkit/BrailleDocument.cs) | `Lines` / `PageTitles` 對外改為 `IReadOnlyList`；新增輔助 mutation API |
| [BraillePageTitle.cs](/src/EasyBrailleEditApp/BrailleToolkit/BraillePageTitle.cs) | 接口改為接受 `IReadOnlyList<BrailleWord>`；改用文件內部索引查找 |
| [BrailleDocumentYamlSerializer.cs](/src/EasyBrailleEditApp/BrailleToolkit/Helpers/BrailleDocumentYamlSerializer.cs) | 新增 YAML DTO 轉接層，維持 `.byml` round-trip |
| [BrailleWordHelper.cs](/src/EasyBrailleEditApp/BrailleToolkit/Helpers/BrailleWordHelper.cs) | helper 參數型別改為 `IReadOnlyList<BrailleWord>` |
| [BrailleProcessor.cs](/src/EasyBrailleEditApp/BrailleToolkit/BrailleProcessor.cs) | event args 改為 `init-only`；呼叫端改用 `AddWord` / `AddWords` |
| [ContextTagManager.cs](/src/EasyBrailleEditApp/BrailleToolkit/ContextTagManager.cs) | `Tags` 公開型別改為 `IReadOnlyDictionary<string, IContextTag>` |
| [GenericContextTag.cs](/src/EasyBrailleEditApp/BrailleToolkit/Tags/GenericContextTag.cs) | 組態屬性收斂為 `init` 導向 |
| [PreviewPanel.cs](/src/EasyBrailleEditApp/EasyBrailleEdit/Controls/PreviewPanel.cs) | `UpdatePreview` 參數改為 `IReadOnlyList<BrailleLine>?` |
| [ClipboardHelper.cs](/src/EasyBrailleEditApp/EasyBrailleEdit/DualEdit/ClipboardHelper.cs) | 接口改為接受唯讀集合並在序列化前複製 |
| [BrailleGridController_EditCommands.cs](/src/EasyBrailleEditApp/EasyBrailleEdit/DualEdit/BrailleGridController_EditCommands.cs) | 改用新的 line / document mutation API |
| [InProcessBrailleConverter.cs](/src/EasyBrailleEditApp/EasyBrailleEdit/Services/InProcessBrailleConverter.cs) | 輸出暫存檔改為唯一檔名 |
| [BrailleWordTest.cs](/src/EasyBrailleEditApp/BrailleToolkit.Tests/BrailleWordTest.cs) | 新增 `OriginalText` 複製測試 |
| [BrailleProcessorEventTest.cs](/src/EasyBrailleEditApp/BrailleToolkit.Tests/BrailleProcessorEventTest.cs) | 新增事件 args 每次都建立新實例的測試 |
| [YamlSerializationTests.cs](/src/EasyBrailleEditApp/BrailleToolkit.Tests/YamlSerializationTests.cs) | 調整測試資料建立方式以符合新 API |
| [ClipboardHelperTests.cs](/src/EasyBrailleEditApp/EasyBrailleEdit.Tests/DualEdit/ClipboardHelperTests.cs) | 調整測試資料建立方式以符合新 API |

## 回歸驗證

- `dotnet build src/EasyBrailleEditApp/EasyBrailleEditApp.sln`
- `dotnet test src/EasyBrailleEditApp/BrailleToolkit.Tests/BrailleToolkit.Tests.csproj`
- `dotnet test src/EasyBrailleEditApp/EasyBrailleEdit.Tests/EasyBrailleEdit.Tests.csproj`

結果：

- `BrailleToolkit.Tests`: 134 / 134 通過
- `EasyBrailleEdit.Tests`: 25 / 25 通過
- Solution build：成功

## 效能測試

### Checkpoint benchmark

- 第一批 `2a/2e/2f/2g` 的 checkpoint benchmark 仍保留於：
  - [`2026-04-05-phase2-checkpoint-2a-2e-2f-2g.md`](./benchmark-result/2026-04-05-phase2-checkpoint-2a-2e-2f-2g.md)
- 這份 checkpoint 只用來記錄進入 `2b/2c/2d` 前的狀態，不作正式回歸結論依據。

### 正式 clean worktree A/B benchmark

- 日期：2026-04-05
- baseline commit：`1abae1a4f7d5416a0f4c55c94e66c3c68705250e`
- candidate commit：`dcf71ef2d0aba531b6d724628234bec7d08168e1`
- 方法：從目前 workspace 變更建立暫時 benchmark snapshot，於兩個乾淨 worktree 各自獨立建置與執行
- 詳細紀錄：
  - [`2026-04-05-phase2-clean-worktree-ab.md`](./benchmark-result/2026-04-05-phase2-clean-worktree-ab.md)

#### A/B 摘要

| Method | Baseline Mean | Candidate Mean | Mean Δ | Baseline Alloc | Candidate Alloc | Alloc Δ |
| ---- | ----: | ----: | ----: | ----: | ----: | ----: |
| 中文單行轉換 | 153.8 us | 151.8 us | -1.30% | 72.8 KB | 72.84 KB | +0.05% |
| 英文單行轉換 | 708.5 us | 721.9 us | +1.89% | 169.29 KB | 169.33 KB | +0.02% |
| 中英混合單行轉換 | 849.8 us | 856.8 us | +0.82% | 374.45 KB | 374.57 KB | +0.03% |
| 中文多行轉換 | 8,820.3 us | 8,925.2 us | +1.19% | 4301.1 KB | 4301.48 KB | +0.01% |
| 英文多行轉換 | 3,963.5 us | 4,308.3 us | +8.70% | 1040.89 KB | 1041.13 KB | +0.02% |
| 中英混合多行轉換 | 3,788.8 us | 3,639.7 us | -3.94% | 1347.01 KB | 1347.72 KB | +0.05% |
| 長中文字串轉換 | 8,863.7 us | 8,384.1 us | -5.41% | 4303.11 KB | 4303.14 KB | +0.00% |

#### 解讀

- `Allocated` 幾乎持平，所有案例都落在 `0.05%` 以內，代表 `2b/2c/2d` 對配置量沒有明顯回歸。
- `Mean` 呈現混合變動，沒有一致性退步趨勢。
- 最大退步出現在「英文多行轉換」`+8.70%`，但 allocation 幾乎不變，較像需要後續持續觀察的單項波動，不足以單憑這次報表判定 Phase 2 整體回歸。
- 以本次 clean worktree A/B 結果來看，Phase 2 可以先視為「功能完成且未見明顯 allocation regression」。

## 後續建議

- 若下一步要進入 Phase 3，建議直接以本文件作為 Phase 2 結案基準。
- 若後續在 `BrailleDocument` / `BrailleLine` 附近繼續重構，需保留目前這層 JSON/YAML 相容做法，避免再引入序列化回歸。
