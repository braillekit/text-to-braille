# Immutable refactoring - phase 2

## 範圍

本次先完成 Phase 2 中風險較低的第一批子項：

- `2a` `BrailleWord.OriginalText` setter → `init`
- `2e` `ContextTagManager.Tags` → `IReadOnlyDictionary<string, IContextTag>`
- `2f` `ConversionFailedEventArgs` / `TextConvertedEventArgs` → `init-only` 屬性
- `2g` `GenericContextTag` 組態屬性 → `init-only`

尚未處理的 Phase 2 子項：

- `2b` `BrailleDocument.Lines`
- `2c` `BrailleDocument.PageTitles`
- `2d` `BrailleLine.Words`

## 變更檔案

| 檔案 | 變更 |
| ---- | ---- |
| [BrailleWord.cs](/src/EasyBrailleEditApp/BrailleToolkit/BrailleWord.cs) | `OriginalText` 改為 `init`；補強 `Copy()` / `Copy(BrailleWord)` 保留 `OriginalText` |
| [ContextTagManager.cs](/src/EasyBrailleEditApp/BrailleToolkit/ContextTagManager.cs) | `Tags` 公開型別改為 `IReadOnlyDictionary<string, IContextTag>` |
| [BrailleProcessor.cs](/src/EasyBrailleEditApp/BrailleToolkit/BrailleProcessor.cs) | `ConversionFailedEventArgs` / `TextConvertedEventArgs` 改為 `init-only`；事件觸發時改為每次建立新實例 |
| [IContextTag.cs](/src/EasyBrailleEditApp/BrailleToolkit/Tags/IContextTag.cs) | `ConvertablePrefix` / `ConvertablePostfix` 改為 `init` |
| [GenericContextTag.cs](/src/EasyBrailleEditApp/BrailleToolkit/Tags/GenericContextTag.cs) | 組態屬性改為 `init` 導向：`ConvertablePrefix`、`ConvertablePostfix`、`TagName`、`Lifetime`、`IsSingleLine`、`RemoveTagOnConversion` |
| [OrgPageNumberContextTag.cs](/src/EasyBrailleEditApp/BrailleToolkit/Tags/OrgPageNumberContextTag.cs) | 改由 base constructor 設定 `singleLine` |
| [TableTopLineContextTag.cs](/src/EasyBrailleEditApp/BrailleToolkit/Tags/TableTopLineContextTag.cs) | 改由 base constructor 設定 `Lifetime` / `RemoveTagOnConversion` / `singleLine` |
| [TableBottomLineContextTag.cs](/src/EasyBrailleEditApp/BrailleToolkit/Tags/TableBottomLineContextTag.cs) | 改由 base constructor 設定 `Lifetime` / `RemoveTagOnConversion` / `singleLine` |
| [TableSingleLineContextTag.cs](/src/EasyBrailleEditApp/BrailleToolkit/Tags/TableSingleLineContextTag.cs) | 改由 base constructor 設定 `Lifetime` / `RemoveTagOnConversion` / `singleLine` |
| [BrailleWordTest.cs](/src/EasyBrailleEditApp/BrailleToolkit.Tests/BrailleWordTest.cs) | 新增 `OriginalText` 複製行為測試 |
| [BrailleProcessorEventTest.cs](/src/EasyBrailleEditApp/BrailleToolkit.Tests/BrailleProcessorEventTest.cs) | 新增事件 args 每次觸發都建立新實例的測試 |

## 實作摘要

### 2a - `BrailleWord.OriginalText`

- `OriginalText` 改為公開 `init`，保留既有建構子中的初始化行為。
- 由於 `BrailleWord.Copy()` 與 `Copy(BrailleWord)` 原本不會保留 `OriginalText`，這次一併修正，避免複製後遺失原始文字資訊。
- 這個補強對後續以 `OriginalText` 判斷 context tag 起訖、或匯出原文時較安全。

### 2e - `ContextTagManager.Tags`

- 對外公開面收斂為唯讀字典，避免呼叫端直接依賴可變 `Dictionary`。
- 內部仍維持原本的 `_tags` 實作，因此沒有改動既有解析與狀態管理流程。

### 2f - event args init-only 化

- `ConversionFailedEventArgs` 與 `TextConvertedEventArgs` 的資料屬性改為 `init-only`。
- 原本 `BrailleProcessor` 會重複使用同一個 event args 實例並以 `SetArgs` / `SetArgValues` 反覆覆寫內容；現在改成每次觸發事件都建立新的 event args 物件。
- `ConversionFailedEventArgs.Stop` 仍維持可變，讓事件處理器可以要求中止轉換。

### 2g - `GenericContextTag` 組態屬性

- `ConvertablePrefix` 與 `ConvertablePostfix` 改為 `init`。
- `GenericContextTag` 的組態相關屬性改為建構期設定，減少建構完成後再調整狀態的空間。
- 幾個衍生型別改為在 base constructor 就傳入 `Lifetime`、`RemoveTagOnConversion` 與 `singleLine`，讓建構流程更一致。

## 回歸驗證

- `BrailleToolkit.Tests`: 134/134 通過。
- 測試指令：`dotnet test src/EasyBrailleEditApp/BrailleToolkit.Tests/BrailleToolkit.Tests.csproj`

## 備註

- 本次尚未執行 benchmark。
- `phase2.md` 目前只記錄 `2a/2e/2f/2g`；後續若完成 `2b/2c/2d`，建議直接續寫到本文件並標明第二批完成範圍。
