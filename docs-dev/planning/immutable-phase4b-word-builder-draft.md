# Phase 4b Draft: `BrailleWordBuilder` 與 Materialized Result MVP

建立日期：2026-04-05

## 背景

`Phase 4a` 已確認：

- `BrailleCell -> readonly record struct` 可以降低 allocation
- 但在真實 conversion benchmark 上會造成明顯 throughput regression

後續 prototype 驗證則顯示：

- plain `readonly struct` 不能穩定修復 regression
- `BrailleWordBuilder + BrailleCellBuffer` 小型 prototype 對比目前 `BrailleWord + BrailleLine`，有約 `-35%` Mean 與 `-30%` allocation 的改善訊號

參考文件：

- [`docs-dev/refactoring/immutable/phase4.md`](../refactoring/immutable/phase4.md)
- [`docs-dev/refactoring/immutable/phase4-cause-analysis.md`](../refactoring/immutable/phase4-cause-analysis.md)
- [`docs-dev/refactoring/immutable/benchmark-result/2026-04-05-phase4-prototype-validation.md`](../refactoring/immutable/benchmark-result/2026-04-05-phase4-prototype-validation.md)

## 為什麼要重寫 `4b` 範圍

原始 immutable refactoring plan 將 `4b` 定義為：

- `BrailleCellList` -> `ImmutableArray<BrailleCell>` builder pattern

但從目前量測結果來看，真正需要優先處理的不是「把 `BrailleCellList` 的公開容器改成 immutable」本身，而是：

1. word 建構期間大量 `Append` / `Insert(0, ...)`
2. mutable `List<BrailleCell>` 的反覆搬移
3. `BrailleWord.Copy()` / `BrailleLine.GetBrailleCells()` 之類的 cell flatten 與複製

因此本草案建議把 `4b` 的 MVP 改寫成：

- **先引入 `BrailleWordBuilder` 與 `BrailleCellBuffer`**
- **建立最小可行的 materialized result 介面**
- **先在 conversion hot path 做 builder/result 分離**

也就是說，`4b` 不再理解成單一容器替換，而是「word-level construction model 改造」。

## 設計目標

### 目標

1. 降低 word 建構過程中的前插與複製成本。
2. 在不立即全面改寫 `BrailleWord` / `BrailleLine` 的前提下，建立可漸進導入的 builder/result 邊界。
3. 保留往後演進到 immutable result 的可能性。
4. 讓 benchmark 可以先驗證 builder/result 分離是否真的適合真實 conversion pipeline。

### 非目標

1. 這一版不直接替換所有 `BrailleWord` 呼叫點。
2. 這一版不處理 `BrailleLine.IndexOf(ReferenceEquals)` 與 `BraillePageTitle.ContentStartLineRef` 的 reference identity 問題。
3. 這一版不承諾立刻將結果物件序列化。
4. 這一版不把 `BrailleLine` 也同步改成 immutable result。

## MVP 邊界

`4b` MVP 只處理這個責任切分：

- **Builder**: 在 conversion / rule pipeline 中可變地建構 cell 與 metadata
- **Materialized Result**: 在 build 完成後提供只讀觀察面
- **Compatibility Bridge**: 必要時可轉回既有 `BrailleWord`

換句話說：

- builder 先服務 hot path
- result 先服務 read-only downstream
- 舊 `BrailleWord` 先保留，作為過渡期相容邊界

## 建議型別

### 1. `BrailleCellBuffer`

用途：

- 取代 builder 階段的 `List<BrailleCell>`
- 支援高頻率 `Append`
- 支援少量但昂貴的 `Prepend`
- 提供 `ReadOnlySpan<BrailleCell>` 供掃描與 materialize

建議特性：

```csharp
internal struct BrailleCellBuffer
{
    public int Count { get; }

    public void Clear();
    public void Append(BrailleCell cell);
    public void AppendRange(ReadOnlySpan<BrailleCell> cells);
    public void Prepend(BrailleCell cell);

    public ReadOnlySpan<BrailleCell> AsSpan();
    public BrailleCell[] ToArray();
}
```

設計備註：

- 先用自管 array + headroom 的 deque-like buffer。
- `Prepend` 不應退回 `List<T>.Insert(0, ...)`。
- 先不引入 `ArrayPool<T>`，避免第一版複雜度過高；等真實 pipeline benchmark 證明方向正確，再考慮 pool 化。

### 2. `BrailleWordBuilder`

用途：

- 在 conversion 與 rule pipeline 中取代目前「先 new `BrailleWord`，再直接改 `Cells`」的模式
- 將 metadata 與 cell construction 集中在單一可變物件

建議 API：

```csharp
internal sealed class BrailleWordBuilder
{
    public BrailleWordBuilder(string text);

    public string Text { get; set; }
    public string OriginalText { get; set; }
    public BrailleLanguage Language { get; set; }

    public string? PhoneticCode { get; set; }
    public bool IsPolyphonic { get; set; }
    public bool DontBreakLineHere { get; set; }
    public string ContextNames { get; set; }
    public IContextTag? ContextTag { get; set; }
    public bool IsContextTag { get; set; }
    public bool IsConvertedFromTag { get; set; }
    public bool NoDigitCell { get; set; }
    public bool NoSpace { get; set; }
    public bool NoCapitalRule { get; set; }
    public bool IsEngPhonetic { get; set; }

    public int CellCount { get; }
    public ReadOnlySpan<BrailleCell> Cells { get; }

    public void ClearCells();
    public void AppendCell(BrailleCell cell);
    public void AppendCells(ReadOnlySpan<BrailleCell> cells);
    public void AppendHex(string? hex);
    public void AppendPositionNumbers(string positionNumbers);
    public void PrependCell(BrailleCell cell);

    public IBrailleWordResult Build();
    public BrailleWord ToBrailleWord(); // compatibility bridge
}
```

設計備註：

- `Cells` 只提供 `ReadOnlySpan<BrailleCell>`，避免 builder 外部再把可變內部暴露成 `List<T>`。
- `Build()` 回傳只讀結果；`ToBrailleWord()` 是過渡期用的 adapter，不是長期終點。
- 若 `IsContextTag = true`，`ClearCells()` 應與現況語意一致。

### 3. 最小可行的 materialized result：`IBrailleWordResult`

用途：

- 作為 build 完成後的只讀邊界
- 讓後續 rule / formatter / exporter 可以逐步改依賴 result 介面，而不是直接依賴 mutable `BrailleWord`

MVP 建議介面：

```csharp
internal interface IBrailleWordResult
{
    string Text { get; }
    string OriginalText { get; }
    BrailleLanguage Language { get; }

    int CellCount { get; }
    ReadOnlyMemory<BrailleCell> Cells { get; }

    string? PhoneticCode { get; }
    bool IsPolyphonic { get; }
    bool DontBreakLineHere { get; }
    string ContextNames { get; }
    IContextTag? ContextTag { get; }
    bool IsContextTag { get; }
    bool IsConvertedFromTag { get; }
    bool NoDigitCell { get; }
    bool NoSpace { get; }
    bool NoCapitalRule { get; }
    bool IsEngPhonetic { get; }

    BrailleWord ToBrailleWord();
}
```

### 為什麼 `Cells` 用 `ReadOnlyMemory<BrailleCell>`

原因：

1. `ReadOnlyMemory<T>` 適合放在 interface 上作為穩定結果面。
2. 呼叫端若需要高效掃描，可用 `Cells.Span`。
3. 相較於直接暴露 `ImmutableArray<T>`，這個抽象對第一版 MVP 更輕量，也比較不會過早綁死 materialization 方式。

換句話說，MVP 的重點是 **builder/result 分離**，不是先把底層容器型別鎖死。

### 4. 預設實作：`BrailleWordMaterialized`

建議的第一版結果物件：

```csharp
internal sealed class BrailleWordMaterialized : IBrailleWordResult
{
    public string Text { get; init; } = string.Empty;
    public string OriginalText { get; init; } = string.Empty;
    public BrailleLanguage Language { get; init; }
    public ReadOnlyMemory<BrailleCell> Cells { get; init; }

    public string? PhoneticCode { get; init; }
    public bool IsPolyphonic { get; init; }
    public bool DontBreakLineHere { get; init; }
    public string ContextNames { get; init; } = string.Empty;
    public IContextTag? ContextTag { get; init; }
    public bool IsContextTag { get; init; }
    public bool IsConvertedFromTag { get; init; }
    public bool NoDigitCell { get; init; }
    public bool NoSpace { get; init; }
    public bool NoCapitalRule { get; init; }
    public bool IsEngPhonetic { get; init; }

    public int CellCount => Cells.Length;

    public BrailleWord ToBrailleWord();
}
```

設計備註：

- 第一版先接受內部持有 `BrailleCell[]`。
- 之後若 benchmark 證明值得，再考慮改成 pooled owner、`ImmutableArray<BrailleCell>` 或其他更穩定儲存形式。

## 建議導入順序

### Step 1. 先引入新型別，不改舊 public model

新增：

- `BrailleCellBuffer`
- `BrailleWordBuilder`
- `IBrailleWordResult`
- `BrailleWordMaterialized`

但先不動：

- `BrailleWord`
- `BrailleCellList`
- `BrailleLine`

### Step 2. 在 benchmark 專案先做真實 conversion-path prototype

先把目前已在 `BrailleToolkit.Benchmarks` 中驗過的小型 prototype，推成更接近真實路徑的 bench：

- `EnglishWordConverter`
- `EnglishBrailleRule`
- `GeneralBrailleRule`
- `BrailleProcessor` 中分數 / 數符 / 大寫前插路徑

目的不是一次改完，而是確認新的 builder 真能覆蓋目前最痛的 prepend / copy 模式。

### Step 3. 在 conversion 內部逐步改用 builder

優先順序建議：

1. `EnglishWordConverter`
2. `EnglishBrailleRule`
3. `GeneralBrailleRule`
4. `TwChineseCharConverter`

這些路徑共同特徵是：

- 會直接操作 `Cells`
- 會有 prepend / overwrite / scan
- benchmark 影響明顯

### Step 4. 在邊界 materialize 成既有 `BrailleWord`

第一版不要求整個 downstream 都懂 `IBrailleWordResult`。

可接受的過渡作法：

- conversion / rules 內部使用 `BrailleWordBuilder`
- 在離開 hot path 時呼叫 `.ToBrailleWord()`
- 其他尚未遷移的 API 仍接收 `BrailleWord`

這樣可以把風險控制在：

- 先優化建構成本
- 不一次處理所有 reference identity 相依點

## 與現有 `BrailleWord` 的欄位對照

第一版 builder / result 建議保留以下 metadata，因為它們直接參與規則或輸出：

- `Text`
- `OriginalText`
- `Language`
- `PhoneticCode`
- `IsPolyphonic`
- `DontBreakLineHere`
- `ContextNames`
- `ContextTag`
- `IsContextTag`
- `IsConvertedFromTag`
- `NoDigitCell`
- `NoSpace`
- `NoCapitalRule`
- `IsEngPhonetic`

第一版先不處理：

- `PhoneticCodes` 的完整多值集合
- `ActivePhoneticIndex`

理由：

- 目前 hot path 與主要 benchmark 主要依賴單一作用中的 `PhoneticCode`
- 多值注音集合可在第二輪再決定要不要抽成獨立 metadata 物件

## 重要相容性決策

### 決策 1：過渡期保留 `BrailleWord`

原因：

- `BrailleLine.IndexOf` 明確依賴 `ReferenceEquals`
- `BraillePageTitle.ContentStartLineRef` 也依賴 object identity
- 序列化與 UI 編輯器目前都已知認得 `BrailleWord`

所以 `4b` MVP 不應直接刪除或取代 `BrailleWord`。

### 決策 2：先不要讓 builder/result 直接暴露 `List<BrailleCell>`

原因：

- 這會把目前 regression 的主要來源帶回新設計
- 一旦外部又拿到 `List<T>`，就很容易重新出現 `Insert(0, ...)`

### 決策 3：第一版 materialized result 不以序列化為優先

原因：

- `4b` 主要是 conversion hot path 改造
- 若第一版就同時要求 `DataContract` / YAML / JSON 全相容，風險會過大

序列化相容應在 builder/result 真正證明值得導入後，再規劃第二輪。

## 建議驗證方式

### 單元測試

至少補：

1. `BrailleWordBuilder.AppendCell` / `PrependCell` 順序正確
2. `AppendHex` / `AppendPositionNumbers` 與既有 `BrailleWord.AddCells*` 語意一致
3. `Build()` 後 result 不受 builder 後續修改影響
4. `ToBrailleWord()` 與既有 `BrailleWord` 的 `Text` / `Cells` / metadata 一致
5. `IsContextTag` 時會清掉 cells

### 效能測試

至少分兩層：

1. 小型 prototype benchmark
   - `BrailleWordBuilderPrototypeBenchmarks`
2. 真實 conversion benchmark
   - 先挑一條最小可替換的轉換路徑
   - 跑 clean worktree A/B

## 建議的第一個實作切點

若開始進入真正 `4b` 實作，最適合的第一個切點是：

- 新增 internal `BrailleCellBuffer`
- 新增 internal `BrailleWordBuilder`
- 在 benchmark 專案或單一 converter prototype 中先使用它
- 暫時仍 materialize 回 `BrailleWord`

理由：

- 可以先驗證 builder 本身是否值得
- 不會立刻被 `BrailleLine` / `BrailleDocument` 的相容性拖住
- 也能最大限度保留 rollback 空間

## 開放問題

1. `PhoneticCodes` 是否需要在第一版 result 中完整保留？
2. `ContextTag` 是否應繼續直接掛在 result 上，還是改成 metadata wrapper？
3. `BrailleWordMaterialized` 應該持有 `BrailleCell[]`，還是後續升級為 pooled owner？
4. 若 `BrailleLine` 未立即 builder 化，line-level flatten 是否還值得先做額外抽象？

## 暫定結論

`4b` 的最小可行實作方向應是：

- **引入 `BrailleWordBuilder`**
- **內部採用 `BrailleCellBuffer`**
- **`Build()` 回傳 `IBrailleWordResult`**
- **在邊界用 `ToBrailleWord()` 與現有系統接軌**

這樣的設計最符合目前 benchmark 與風險分析結果，也最適合作為後續 `4c` / `4d` 的前置步驟。
