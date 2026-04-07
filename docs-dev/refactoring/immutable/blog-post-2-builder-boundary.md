# C# Immutable Refactoring 實戰（二）：不要把所有 class 都改成 record，先建立 Builder Boundary

> 狀態：部落格草稿
> 主題：`BrailleWord` / `BrailleLine` 的 immutable boundary、builder、read-only view、runtime identity，以及 C# 語法取捨
> 參考資料：
> - [`immutable-design-change-summary.md`](./immutable-design-change-summary.md)
> - [`phase4.md`](./phase4.md)
> - [`immutable-design.md`](./immutable-design.md)

上一篇談的是效能改善最明顯的一段：把 XML 點字表從 `DataTable` 改成 `BrailleTableEntry` 加 `FrozenDictionary`。

這一篇想談比較難的部分：如果一個專案裡已經有大量 mutable model，而且 UI editor、formatter、conversion rules 都在修改它，該怎麼導入 immutable design？

我最後採用的答案不是「全部改成 record」。

而是：

> 保留既有 mutable class，但把建構階段、唯讀檢視、完成後 snapshot、相容 mutation 邊界分清楚。

這就是這次重構裡的 builder boundary。

## 為什麼不能直接全部 immutable？

這個專案有幾個核心型別：

- `BrailleCell`：一個點字方。
- `BrailleWord`：一個邏輯詞，例如中文字、英文字、標點或 tag。
- `BrailleLine`：一行點字，由多個 `BrailleWord` 組成。
- `BrailleDocument`：整份點字文件。

`BrailleCell` 很適合改成 value type，因為它只包一個 byte，語意上也是 value object。

但 `BrailleWord` 和 `BrailleLine` 不一樣。它們在既有系統中有大量 reference semantics：

- 編輯器 grid 會記住某個 word 對應哪個 cell。
- page title 會指向某個 content begin line。
- formatter 會切行、補縮排、插入連字號。
- braille rules 會插入空方、替換數字符號、補英文大寫符號。
- undo/redo 會 deep copy 文件，再試著回復 selection。

如果直接把 `BrailleWord` / `BrailleLine` 改成 immutable record，表面上可能很漂亮，但實際上會同時打破太多行為。

所以我選擇分階段做：

```text
BrailleCell：真的改成 readonly record struct
BrailleWord：保留 mutable class，但加 read-only view + builder + result
BrailleLine：保留 mutable class，但加 read-only view + builder + result
BrailleDocument：公開集合改 read-only facade，mutation 改走明確 API
```

## 第一個取捨：`BrailleCell` 改成 `readonly record struct`

`BrailleCell` 原本是 `sealed class`，內部只有一個 byte value，並用 flyweight pool 預先建立 256 個實例。

這種型別很適合改成：

```csharp
public readonly record struct BrailleCell
{
    [DataMember]
    public byte Value { get; init; }
}
```

改完後的好處：

- `BrailleCell` 變成 value type。
- equality 由 record struct 自動提供 value equality。
- `default(BrailleCell)` 自然代表空方 `0x00`。
- 不再需要依賴 flyweight reference identity。

但這裡有一個很好的教訓：

> 改成 value type 不保證比較快。

在 Phase 4a，我只把 `BrailleCell` 改成 `readonly record struct` 後，clean worktree A/B 顯示 throughput 反而退步。allocation 有下降，但平均時間變差。後來透過 builder / buffer / materialization 路徑逐步收斂，才把整體 branch 的效能扳回來。

這也是為什麼我不建議把文章寫成「record struct 讓程式變快」。更好的說法是：

> `readonly record struct` 讓 `BrailleCell` 的語意更正確，但效能必須靠 benchmark 驗證，不能靠語感判斷。

## 第二個取捨：公開 API 改 read-only，但內部先保留 List

`BrailleLine.Words` 原本是公開 `List<BrailleWord>`。這代表任何呼叫端都可以直接：

```csharp
brLine.Words.Insert(index, word);
brLine.Words.RemoveRange(index, count);
brLine.Words.Add(word);
```

這對重構很不友善，因為 mutation surface 太大。你無法知道到底有哪些地方會直接修改集合。

所以我先把公開 API 收斂成：

```csharp
public IReadOnlyList<BrailleWord> Words
{
    get { return m_Words; }
    private set { m_Words = CopyWords(value); }
}
```

然後補上明確的 mutation API：

```csharp
public void AddWord(BrailleWord word)
public void AddWords(IEnumerable<BrailleWord> words)
public void Insert(int index, BrailleWord word)
public void InsertWords(int index, IEnumerable<BrailleWord> words)
public void RemoveAt(int index)
public void RemoveRange(int index, int count)
```

`BrailleDocument.Lines` 和 `BrailleDocument.PageTitles` 也做了類似處理，對外改成 `IReadOnlyList<T>`，內部仍保留 `List<T>`。

這不是完美 immutable collection。`IReadOnlyList<T>` 只是 read-only facade，不是真正不可變資料結構。但它是一個務實中間步驟：

- 呼叫端不能再隨手改集合。
- mutation 被集中到明確 API。
- 既有 formatter / editor / rule 不需要一次全部重寫。
- 序列化也比較容易維持相容。

## 第三個取捨：Builder 負責建構，Result 負責快照

接著是這次重構的核心：builder boundary。

`BrailleWord` 新增了這幾個內部角色：

```text
IBrailleWordView
IBrailleWordResult
BrailleWordBuilder
BrailleWordMaterialized
```

`IBrailleWordView` 是唯讀檢視，讓 helper / formatter / font converter 可以只讀資料，不需要知道背後是 `BrailleWord` 還是 materialized result。

概念像這樣：

```csharp
internal interface IBrailleWordView
{
    string Text { get; }
    string OriginalText { get; }
    BrailleLanguage Language { get; }
    int CellCount { get; }
    BrailleCell GetCell(int index);
}
```

`BrailleWordBuilder` 則處理建構階段的 mutation：

```csharp
var builder = new BrailleWordBuilder("A")
{
    OriginalText = "A",
    Language = BrailleLanguage.English
};

builder.PrependCell(BrailleCell.Capital);
builder.AppendHex("01");

BrailleWord word = builder.ToBrailleWord();
```

比較關鍵的是：builder 不只可以建立新 word，也可以套用到既有 word：

```csharp
var builder = BrailleWordBuilder.FromBrailleWord(existingWord);
builder.PrependCell(BrailleCell.Capital);
builder.ApplyTo(existingWord);
```

這對相容既有 rule 很重要。因為很多 rule 不是在建構新 word，而是在轉換後追加或替換 cell。例如：

- 英文大寫符號要 prepend capital sign。
- 數字模式要替換 digit cell。
- 分數轉換要在分子前、分母後追加符號。
- 表格線 context 要替換第一個 cell。

如果直接要求所有 rule 都回傳新 `BrailleWord`，改動會太大。透過 `FromBrailleWord(...).ApplyTo(...)`，可以把 cell mutation 收斂到 construction boundary，同時保留既有物件 identity。

## 第四個取捨：Line 也需要 builder，但不要急著 immutable

word-level 邊界建立後，我用同樣方式處理 `BrailleLine`：

```text
IBrailleLineView
IBrailleLineResult
BrailleLineBuilder
BrailleLineMaterialized
BrailleLineHelper
```

`BrailleProcessor` 原本在轉換時直接建立 `BrailleLine` 然後一直 `Words.AddRange(...)`。

重構後，initial conversion 階段先用 builder：

```text
文字輸入
-> BrailleLineBuilder 累積 BrailleWord
-> ToBrailleLine()
-> 後續 context tag / braille rules 修改 materialized BrailleLine
```

這個邊界很重要。它把流程分成兩段：

1. 建構階段：可用 builder 做 append / insert / trim。
2. 規則階段：先 materialize 回既有 `BrailleLine`，讓舊 rule 繼續運作。

也就是說，我沒有假裝整個世界都已經 immutable。這比較像是先架橋。

## 第五個取捨：ReferenceEquals 改成 runtime identity

把 model 開始往 builder / copy / materialized result 推之後，另一個老問題會浮出來：原本很多地方靠 reference identity。

例如 `BrailleLine.IndexOf(BrailleWord)` 以前不能用 value equality，因為兩個 word 內容可能一樣，但它們在文件中的位置不同。原本做法是類似 `ReferenceEquals`。

但一旦開始 deep copy、JSON round-trip、builder materialization，reference identity 很容易被切斷。

所以我新增了 runtime identity：

```csharp
public long Identity
{
    get { return m_Identity; }
}
```

由 `BrailleObjectIdentityGenerator` 產生：

```csharp
internal static class BrailleObjectIdentityGenerator
{
    private static long s_NextWordIdentity;
    private static long s_NextLineIdentity;

    public static long NextWordIdentity()
    {
        return Interlocked.Increment(ref s_NextWordIdentity);
    }

    public static long NextLineIdentity()
    {
        return Interlocked.Increment(ref s_NextLineIdentity);
    }
}
```

`BrailleWord` 與 `BrailleLine` 反序列化後，如果沒有有效 identity，就補發新的。

這讓 editor 可以用 explicit identity 還原狀態，而不是依賴 CLR object reference。

例如 undo/redo grid state 不再只保存 raw row / column，而是保存：

```text
LineIdentity
WordIdentity
RowOffset
ColumnOffset
FallbackRow
FallbackColumn
```

restore 時再透過目前 document 的 line/word identity 找回 grid position。

這個改動對 immutable boundary 很關鍵。因為 immutable 或 copy-heavy 設計會自然產生更多「新物件」，如果系統還假設「同一個物件參考才是同一個 domain entity」，後面會很痛。

## 第六個取捨：序列化要有轉接層

`Words` / `Lines` / `PageTitles` 改成 `IReadOnlyList<T>` 後，序列化也要小心。

JSON / DataContract 這邊，我保留 backing field：

```csharp
[DataMember(Name = "Words")]
private List<BrailleWord> m_Words;

[IgnoreDataMember]
public IReadOnlyList<BrailleWord> Words
{
    get { return m_Words; }
    private set { m_Words = CopyWords(value); }
}
```

YAML 則改用 DTO model：

```text
BrailleDocumentYamlModel
BrailleLineYamlModel
BrailleWordYamlModel
BraillePageTitleYamlModel
```

serialize 時從 read-only view 讀取，deserialize 時透過 builder / construction boundary 建回 model。

這樣做的好處是：

- runtime model 可以收斂公開 mutable API。
- YAML 格式可以維持穩定。
- 反序列化流程可以集中補 identity 與 metadata。

## 這次用到的 C# 語法與 .NET 型別

這次重構用到的語法和型別，可以整理成這張表：

| 技術 | 用在哪裡 | 取捨 |
| ---- | ---- | ---- |
| `readonly record struct` | `BrailleCell`、`BrailleTableEntry` | 適合小型 value object，但效能要量測 |
| `init` accessor | `OriginalText`、event args、context tag 組態 | 建立後不給外部任意修改 |
| `IReadOnlyList<T>` | `Words`、`Lines`、`PageTitles` | 收斂公開 mutation surface，但不等於真正 immutable |
| `IReadOnlyDictionary<TKey,TValue>` | `ContextTagManager.Tags` | 對外只讀，內部仍可管理狀態 |
| `FrozenDictionary` / `FrozenSet` | table lookup、static tag lookup | 適合建構一次、查詢很多次 |
| `ReadOnlySpan<T>` / `ReadOnlyMemory<T>` | cell materialization | 減少 cell 序列複製與中介物件 |
| Builder pattern | `BrailleWordBuilder`、`BrailleLineBuilder` | 建構階段可變，完成後 materialize |
| `InternalsVisibleTo` | tests / benchmarks | 讓 internal builder/result 可被驗證 |
| `OnDeserialized` | runtime identity 補發 | 維持 serialization round-trip 後的 editor 相容性 |

## 效能結果要怎麼解讀？

如果只看 Phase 4d 本身，也就是從 word-level boundary 到 line-level boundary 之後的增量，clean worktree A/B 結果是：

- 7 個 benchmark 中有 6 個 `Mean` 改善。
- 英文與混合內容多數改善約 11% 到 14%。
- 中文多行幾乎持平。
- 中文單行有 +1.30% 小幅回歸，比較接近量測波動。
- allocation 全部小幅上升，但幅度可控。

這代表 line-level builder boundary 沒有造成明顯 throughput regression。

但如果看整個 branch 從起點到 Phase 4d 的累積效果，結果是：

- 7 個 conversion benchmark 全部變快。
- Mean 改善約 28% 到 65%。
- allocation 下降約 89% 到 92%。

所以比較公平的說法是：

> Data table frozen index 是 allocation 與中文路徑改善的主因；builder boundary 則讓後續 model immutable 化可以持續推進，而且沒有留下 branch-level regression。

## 我會怎麼總結這次重構

這次重構後，我對 immutable design 的看法更保守，也更清楚：

1. 小型 value object 可以考慮 `readonly record struct`，但不要假設它一定更快。
2. `IReadOnlyList<T>` 是好的 API 收斂手段，但不是 immutable collection。
3. 對大量查詢、建好後不修改的資料，`FrozenDictionary` 很值得考慮。
4. 對既有 mutable model，不要急著全改 record，先建立 builder / result / view boundary。
5. 如果原系統依賴 `ReferenceEquals`，導入 immutable / copy / builder 前，要先設計 domain identity。
6. 序列化格式和 runtime model 可以分開，用 DTO 轉接是很務實的選擇。
7. benchmark 必須跟著每個高風險切點走，尤其是 value type 與 builder 這種看似「應該比較快」的改動。

最後，我覺得這次最重要的工程結論是：

> immutable design 的重點不是消滅所有 mutation，而是把 mutation 關在你能理解、能測試、能量測的邊界裡。

這次的 `BrailleWordBuilder` / `BrailleLineBuilder` 就是那條邊界。
