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

### Post-change snapshot

- 保存一份 Phase 3 完成當下的 benchmark 快照：
  - [`2026-04-05-phase3-current.md`](./benchmark-result/2026-04-05-phase3-current.md)
- 這份結果主要用於追蹤與存檔，不單獨作正式回歸證據。

### 正式 clean worktree A/B benchmark

- 日期：2026-04-05
- baseline commit：`dcf71ef2d0aba531b6d724628234bec7d08168e1`
- candidate commit：`8251b2df5bc25d0b4bb67826e3745cbc64f59b2a`
- 方法：於兩個乾淨 worktree 各自獨立建置與執行 benchmark
- 詳細紀錄：
  - [`2026-04-05-phase3-clean-worktree-ab.md`](./benchmark-result/2026-04-05-phase3-clean-worktree-ab.md)

#### A/B 摘要

| Method | Baseline Mean | Candidate Mean | Mean Δ | Baseline Alloc | Candidate Alloc | Alloc Δ |
| ---- | ----: | ----: | ----: | ----: | ----: | ----: |
| 中文單行轉換 | 166.00 us | 68.70 us | -58.61% | 72.84 KB | 6.01 KB | -91.75% |
| 英文單行轉換 | 526.20 us | 514.98 us | -2.13% | 169.33 KB | 17.52 KB | -89.65% |
| 中英混合單行轉換 | 956.00 us | 400.45 us | -58.11% | 374.57 KB | 29.92 KB | -92.01% |
| 中文多行轉換 | 8,901.10 us | 3,420.40 us | -61.57% | 4,301.49 KB | 339.67 KB | -92.10% |
| 英文多行轉換 | 4,333.30 us | 3,734.43 us | -13.82% | 1,041.13 KB | 108.91 KB | -89.54% |
| 中英混合多行轉換 | 2,685.00 us | 2,088.58 us | -22.21% | 1,347.71 KB | 116.46 KB | -91.36% |
| 長中文字串轉換 | 6,332.10 us | 3,530.79 us | -44.24% | 4,303.15 KB | 341.32 KB | -92.07% |

#### 解讀

- 這次 clean worktree A/B 沒有任何案例出現平均時間回歸。
- `Allocated` 在所有案例都下降約 `89%` 到 `92%`，是非常一致的改善。
- 改善最明顯的是中文與混合內容，符合 Phase 3 把 table lookup 從 `DataTable.Select(...)` 換成 frozen index 的預期。
- BrailleToolkit.Benchmarks 專案本身在兩個 commit 間沒有變更，所以這次 A/B 是可比的。

幾個跟執行速度有關的代表數字：

- 中文單行：166.00 us -> 68.70 us，變化：-58.61%
- 中文多行：8,901.10 us -> 3,420.40 us，-61.57%
- 長中文字串：6,332.10 us -> 3,530.79 us，-44.24%
- 英文單行：526.20 us -> 514.98 us，-2.13%
- 英文多行：4,333.30 us -> 3,734.43 us，-13.82%

總結：

- 以目前結果來看，Phase 3 可以視為：
  - 功能完成
  - 未見效能回歸
  - allocation 顯著下降
  - 多數 benchmark 吞吐量提升，尤其是中文相關路徑

## 後續建議

- 若 Phase 4 要繼續推 immutable model，這一層 data lookup 可以先視為穩定基礎。
- 若後續需要更完整的 table metadata 查詢，再考慮是否要把 XML 其他屬性抽成更明確的 typed fields，而不是回退成 dictionary / DataTable 模式。
- 後續若有再改動 benchmark 專案或測資，需重新建立新的 baseline，避免和本次 A/B 混用。
