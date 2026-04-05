# Immutable refactoring - phase 3

## 狀態

Phase 3 已完成：

- `3a` 定義 `BrailleTableEntry` immutable record struct
- `3b` `XmlBrailleTable` 內部以 immutable entry + `FrozenDictionary` 索引取代 `DataTable`
- `3c` 更新各個 concrete table 的查詢方法
- `3d` 移除 `BrailleToolkit/Data` 這條路徑對 `System.Data` / `DataTable` / `DataRow` 的依賴

本階段以 [`phase2.md`](./phase2.md) 為 baseline。

## 這一批的重點

### `3a`

- 新增 [`BrailleTableEntry.cs`](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit/Data/BrailleTableEntry.cs)
- 以 `readonly record struct` 表示單一 XML `<symbol>` 項目
- 目前保留的核心資料包含：
  - `Text`
  - `Dots` / `Code`
  - `Dots2` / `Code2`
  - `Type`
  - `Joined`
  - `Mono`
  - `Rule`
  - `Description`

### `3b`

- [`XmlBrailleTable.cs`](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit/Data/XmlBrailleTable.cs) 改為直接用 `XDocument` 解析 XML。
- `code` / `code2` 不再透過 `DataTable` 欄位後加工，而是在建立 `BrailleTableEntry` 時直接從 `dots` / `dots2` 轉換。
- 載入完成後建立三組 frozen index：
  - `text -> entry`
  - `(type, text) -> entry`
  - `type -> ordered entry[]`
- 保留大小寫敏感比對，與舊版 `DataTable.CaseSensitive = true` 的查找語意一致。
- 若 XML 中出現重複 `text` 或重複 `(type, text)`，載入時直接拋錯，避免悄悄覆蓋資料。

### `3c`

- 下列子類別已改用 base helper lookup，不再依賴 `Select(...)` 字串查詢：
  - [`EnglishBrailleTable.cs`](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit/Data/EnglishBrailleTable.cs)
  - [`EnglishUebBrailleTable.cs`](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit/Data/EnglishUebBrailleTable.cs)
  - [`TwChineseBrailleTable.cs`](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit/Data/TwChineseBrailleTable.cs)
  - [`UrlBrailleTable.cs`](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit/Data/UrlBrailleTable.cs)
- `TwChineseBrailleTable.GetAllPunctuations()` 透過 `type -> entry[]` 索引保留 XML 原始順序。
- 舊版為了 SQL-style filter 特別處理單引號 escape 的邏輯已不需要。

### 測試補強

- [`XmlBrailleTableTest.cs`](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit.Tests/XmlBrailleTableTest.cs) 新增：
  - immutable entry / `Entries` 驗證
  - `dots2 -> code2` 自動轉換驗證
  - concrete table lookup smoke test

## 主要變更檔案

| 檔案 | 變更 |
| ---- | ---- |
| [BrailleTableEntry.cs](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit/Data/BrailleTableEntry.cs) | 新增 immutable table entry 型別 |
| [XmlBrailleTable.cs](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit/Data/XmlBrailleTable.cs) | 改用 `XDocument` + frozen indexes 載入與查詢 |
| [EnglishBrailleTable.cs](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit/Data/EnglishBrailleTable.cs) | `FindLetter` / `FindDigit` 改走 immutable lookup |
| [EnglishUebBrailleTable.cs](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit/Data/EnglishUebBrailleTable.cs) | `FindLetter` 改走 immutable lookup |
| [TwChineseBrailleTable.cs](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit/Data/TwChineseBrailleTable.cs) | 各種 type/flag lookup 改走 immutable lookup |
| [UrlBrailleTable.cs](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit/Data/UrlBrailleTable.cs) | `FindLetter` / `FindDigit` 改走 immutable lookup |
| [XmlBrailleTableTest.cs](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit.Tests/XmlBrailleTableTest.cs) | 補 Phase 3 lookup 與 entry 測試 |

## 回歸驗證

- `dotnet test src/EasyBrailleEditApp/BrailleToolkit.Tests/BrailleToolkit.Tests.csproj`
- `dotnet test src/EasyBrailleEditApp/EasyBrailleEdit.Tests/EasyBrailleEdit.Tests.csproj`
- `dotnet build src/EasyBrailleEditApp/EasyBrailleEditApp.sln`

結果：

- `BrailleToolkit.Tests`: 136 / 136 通過
- `EasyBrailleEdit.Tests`: 25 / 25 通過
- Solution build：成功

## 效能測試

- 目前先保存一份 post-change benchmark 量測結果：
  - [`2026-04-05-phase3-current.md`](./benchmark-result/2026-04-05-phase3-current.md)
- 這份結果可當 Phase 3 完成時的量測快照，但不是 clean worktree A/B 基準。
- 若要正式判定 Phase 3 是否造成 regression，下一步仍建議比照 Phase 2 方式，再做一次 baseline vs candidate 的 clean worktree A/B benchmark。

## 後續建議

- 若 Phase 4 要繼續推 immutable model，這一層 data lookup 可以先視為穩定基礎。
- 若後續需要更完整的 table metadata 查詢，再考慮是否要把 XML 其他屬性抽成更明確的 typed fields，而不是回退成 dictionary / DataTable 模式。
