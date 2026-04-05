# BrailleToolkit Immutable Design 重構計畫

> 建立日期：2026-04-05
> 目標：透過引入 C# immutable types 提升點字轉換核心程式庫的效能與安全性

---

## 專案環境

- **Target Framework**: .NET 10.0 (`LangVersion=latest`, `Nullable=enable`)
- **可用語言特性**: C# 13 records, `readonly record struct`, `FrozenDictionary`, `FrozenSet`, `ImmutableArray`, `init` properties, primary constructors
- **外部使用者**: EasyBrailleEdit (WinForms)、Txt2Brl (CLI)、BrailleToolkit.Tests、BrailleToolkit.Benchmarks
- **序列化機制**: System.Text.Json、YamlDotNet、legacy `[DataContract]/[DataMember]`

---

## 候選類型分析

### 1. `BrailleCell` (BrailleCell.cs)

- **現狀**: Sealed class，私有 `byte m_Value` 欄位。採用 flyweight 模式（256 個預分配靜態實例）。私有建構函式。自訂 `Equals`/`GetHashCode`。
- **適合重構的原因**: 已經是事實上的不可變型別，但使用 class（堆積配置）來包裝單一 byte，且語意上是 value equality。
- **建議做法**: 轉換為 `readonly record struct BrailleCell(byte Value)`。flyweight pool 可保留但變成選用（1-byte struct 的複製成本比間接查找更低）。
- **相依性**: 被幾乎所有型別引用。
- **風險**: **中等** — 型別無處不在，但不可變語意不變。需驗證序列化相容性。

### 2. `CharPosition` (BrailleProcessor.cs)

- **現狀**: Mutable struct，三個公開可讀寫的屬性（`CharValue`, `LineNumber`, `CharIndex`）。
- **適合重構的原因**: 建立後從未被修改，只用來記錄無效字元的位置。
- **建議做法**: 轉換為 `readonly record struct CharPosition(char CharValue, int LineNumber, int CharIndex)`。
- **風險**: **低**

### 3. `ConversionFailedEventArgs` / `TextConvertedEventArgs` (BrailleProcessor.cs)

- **現狀**: Sealed class，透過內部 `SetArgs`/`SetArgValues` 方法重複使用同一實例。
- **建議做法**: 屬性改為 `init`-only，每次觸發事件時建立新實例（成本低，且更符合慣例）。注意 `ConversionFailedEventArgs.Stop` 必須保持可變（事件處理器用來通知取消）。
- **風險**: **低**

### 4. `BrailleGlobals` (BrailleGlobals.cs)

- **現狀**: 靜態欄位 `ChinesePunctuations`（非 readonly）。
- **建議做法**: 加上 `readonly` 修飾詞。
- **風險**: **低**

### 5. `ContextTagNames.Collection` (Tags/ContextTagNames.cs)

- **現狀**: `public static HashSet<string>`（非 readonly，可變）。
- **建議做法**: 改為 `public static readonly FrozenSet<string>`，提供不可變性與最佳化的讀取效能。
- **風險**: **低**

### 6. `SimpleTag.Tags` (Tags/SimpleTag.cs)

- **現狀**: `public static Dictionary<string, string>`（可變）。
- **建議做法**: 改為 `public static readonly FrozenDictionary<string, string>`。
- **風險**: **低**

### 7. `BrailleCharConverter.m_CharTable` (Converters/BrailleCharConverter.cs)

- **現狀**: `static Dictionary<string, string>`，載入後不再修改。
- **建議做法**: 改為 `static readonly FrozenDictionary<string, string>`。
- **風險**: **低**

### 8. `BrailleFontConverter.m_FontTable` (Converters/BrailleFontConverter.cs)

- **現狀**: `static Hashtable`（legacy 非泛型集合），載入後不再修改。
- **建議做法**: 改為 `static readonly FrozenDictionary<string, string>`，同時去除 `Hashtable`。
- **風險**: **低**

### 9. `BrailleProcessor._autoReplacedText` (BrailleProcessor.cs)

- **現狀**: `Dictionary<string, string>`，建構時建立後不再修改。
- **建議做法**: 改為 `FrozenDictionary<string, string>`。
- **風險**: **低**

### 10. Data Tables (Data/XmlBrailleTable 及所有子類別)

- **現狀**: 所有點字對照表（TwChineseBrailleTable, EnglishBrailleTable, MathBrailleTable 等）都繼承 `XmlBrailleTable`，使用 `System.Data.DataTable` 作為儲存。載入後資料不再修改。
- **適合重構的原因**: `DataTable` 是舊式、重量級、非執行緒安全的容器。
- **建議做法**:
  1. 定義 `readonly record struct BrailleTableEntry(...)`
  2. 載入 XML 時建構 record struct
  3. 用 `FrozenDictionary` 取代 `DataTable` 的查詢
  4. 移除 `System.Data` 依賴
- **風險**: **中等** — 內部重構幅度大，但 API 邊界清晰。

### 11. Model 類型 (`BrailleCellList`, `BrailleWord`, `BrailleLine`, `BrailleDocument`)

- **現狀**: 這些是轉換管線中被大量修改的核心模型。
- **結論**: **目前不適合全面不可變化**。轉換管線依賴就地修改（in-place mutation）。
- **中期改善**: 公開 API 改用 `IReadOnlyList<T>`，內部保持 `List<T>` 以 `internal` 存取。
- **遠期目標**: 引入 builder pattern（如 `BrailleLineBuilder`），區分「建構中可變」與「完成後不可變」。
- **風險**: **高**（全面不可變化時）

### 12. 不需修改的類型

- `BrailleCellCode` (enum) — 已不可變
- `BrailleConst` (static class, 全部 const) — 已不可變
- `ContextLifetime` (enum) — 已不可變
- 所有 Helper/Utility 靜態類別 — 無狀態
- 所有 Rule 靜態類別 — 無狀態

---

## 重構階段

### Phase 1: 零風險快速改善（無破壞性變更）

| # | 變更項目 | 風險 |
|---|---------|------|
| 1a | `BrailleGlobals.ChinesePunctuations` → 加上 `readonly` | 低 |
| 1b | `ContextTagNames.Collection` → `FrozenSet<string>` | 低 |
| 1c | `SimpleTag.Tags` → `FrozenDictionary<string, string>` | 低 |
| 1d | `BrailleCharConverter.m_CharTable` → `FrozenDictionary<string, string>` | 低 |
| 1e | `BrailleFontConverter.m_FontTable` → `FrozenDictionary<string, string>`（取代 Hashtable）| 低 |
| 1f | `BrailleProcessor._autoReplacedText` → `FrozenDictionary<string, string>` | 低 |
| 1g | `CharPosition` → `readonly record struct` | 低 |

### Phase 2: Model 類型收緊（init-only、IReadOnly 公開介面）

| # | 變更項目 | 風險 |
|---|---------|------|
| 2a | `BrailleWord.OriginalText` setter → `init` | 低 |
| 2b | `BrailleDocument.Lines` → 公開 `IReadOnlyList<BrailleLine>` | 低 |
| 2c | `BrailleDocument.PageTitles` → 公開 `IReadOnlyList<BraillePageTitle>` | 低 |
| 2d | `BrailleLine.Words` → 公開 `IReadOnlyList<BrailleWord>`（內部保持 `List`）| 中等 |
| 2e | `ContextTagManager.Tags` → 回傳 `IReadOnlyDictionary<string, IContextTag>` | 低 |
| 2f | `ConversionFailedEventArgs` / `TextConvertedEventArgs` → init-only 屬性 | 低 |
| 2g | `GenericContextTag` 組態屬性 → `init`-only | 低 |

### Phase 3: Data Table 不可變化（高效能影響）

| # | 變更項目 | 風險 |
|---|---------|------|
| 3a | 定義 `readonly record struct BrailleTableEntry(...)` | 低 |
| 3b | `XmlBrailleTable` 內部以 `FrozenDictionary` 取代 `DataTable` | 中等 |
| 3c | 更新所有子類別的查詢方法 | 中等 |
| 3d | 移除 `System.Data` 依賴 | 中等 |

### Phase 4: 深度 Model 不可變化（遠期，最高風險）

| # | 變更項目 | 風險 |
|---|---------|------|
| 4a | `BrailleCell` → `readonly record struct` | 中等 |
| 4b | `BrailleCellList` → `ImmutableArray<BrailleCell>` builder pattern | 高 |
| 4c | `BrailleWord` → 分離為 mutable builder + immutable result | 高 |
| 4d | `BrailleLine` → 分離為 mutable builder + immutable result | 高 |

---

## 關鍵架構決策

1. **FrozenDictionary/FrozenSet**: .NET 8+ 提供（`System.Collections.Frozen`），.NET 10 完全可用，無需額外套件。

2. **序列化相容性**: `[DataContract]/[DataMember]` 和 System.Text.Json 並用。`readonly record struct` 可與 System.Text.Json 搭配。需驗證 `[DataContract]` 對 init-only 屬性的支援。

3. **Builder Pattern（Phase 4）**: 需引入 builder 型別（如 `BrailleLineBuilder`），轉換管線使用 builder，最後呼叫 `.Build()` 回傳不可變版本。這是重大架構變更，應先 prototype 並 benchmark。

4. **Internal vs Public 可變性（Phase 2）**: 實用的中間步驟 — 內部保持 `List<T>`（透過 `internal` 方法存取），公開面改用 `IReadOnlyList<T>`。

5. **BrailleCell flyweight**: 若轉為 `readonly record struct`，flyweight pool 變為不必要（1-byte struct 的複製比間接查找更便宜）。但保留 `GetInstance` API 以維持向後相容。

---

## 注意事項與潛在挑戰

1. **YamlDotNet 序列化**: 對 `readonly record struct` 或 `ImmutableArray<T>` 可能需要自訂 type converter。

2. **BrailleLine.IndexOf 使用 ReferenceEquals**: 刻意使用參考相等（第 442 行），若 `BrailleWord` 改為 struct/record struct 會失效。這是 Phase 4 延後的原因之一。

3. **BraillePageTitle.ContentStartLineRef**: 使用參考相等追蹤文件中哪個 `BrailleLine` 物件對應頁面標題。此模式與 value type 不相容。

4. **Event args 重複使用模式**: `SetArgs`/`SetArgValues` 每次迭代重複使用同一實例。改為 init-only 後需每次建立新實例，但成本低廉且更符合慣例。

5. **DataTable 移除（Phase 3）**: `XmlBrailleTable` 使用 DataTable 查詢（`Select`, `Rows.Find`, `PrimaryKey`）。替換為 FrozenDictionary 需轉譯每種查詢模式，子類別有不同的過濾模式（依 `type` 欄位），需要次要索引或多鍵字典。

---

## 每個 Phase 完成後的驗證步驟

1. 執行所有單元測試（`dotnet test`），確認無回歸。
2. 執行 benchmark（`dotnet run --project BrailleToolkit.Benchmarks -c Release`），比較效能數據。
3. 將 benchmark 結果保存至 `docs-dev/refactoring/immutable/benchmark-result/`，以日期和 phase 命名。
