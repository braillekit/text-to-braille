# C# Immutable Refactoring 實戰（一）：從 DataTable 到 FrozenDictionary，點字轉換快了 28% 到 65%

> 狀態：部落格草稿
> 主題：效能結果、benchmark 方法、`DataTable` 到 immutable entry + frozen index 的重構
> 參考資料：
> - [`immutable-design-change-summary.md`](./immutable-design-change-summary.md)
> - [`phase3.md`](./phase3.md)
> - [`2026-04-06-branch-start-vs-phase4d-clean-worktree-ab.md`](./benchmark-result/2026-04-06-branch-start-vs-phase4d-clean-worktree-ab.md)

這次我在一個文字轉點字的 C# 專案裡，做了一輪偏向 immutable design 的核心重構。

這個專案的核心工作是把一般文字轉成點字。中文字和全形標點符號使用台灣點字規則，英文和半形標點符號使用 UEB。轉換流程中有大量查表、字詞建構、後處理規則與文件排版，因此它很適合拿來觀察一件事：

> 把核心資料結構朝不可變設計收斂，真的會變快嗎？

答案不是「只要 immutable 就會變快」。

比較精準的說法是：這次真正帶來巨大改善的，不是把所有 class 都改成 record，而是把「載入後不再修改」的資料表，從 `DataTable` 換成 immutable entry 加上 frozen index。後續的 builder boundary 則讓這個方向能繼續推下去，而不會被既有 mutable 編輯流程拖垮。

## 先看結果

我用 clean worktree A/B benchmark 比較整個 `immutable-design` branch 從起點到 Phase 4d 完成後的累積結果。

baseline 是 branch 起點，candidate 是完成資料表重構、word-level builder、line-level builder、read-only view 與 identity 相容層之後的版本。

| 案例 | Baseline Mean | Candidate Mean | Mean 改善 | Baseline Alloc | Candidate Alloc | Alloc 改善 |
| ---- | ----: | ----: | ----: | ----: | ----: | ----: |
| 中文單行轉換 | 148.40 us | 61.30 us | -58.69% | 72.84 KB | 6.03 KB | -91.72% |
| 英文單行轉換 | 680.40 us | 484.69 us | -28.76% | 169.33 KB | 18.25 KB | -89.22% |
| 中英混合單行轉換 | 828.70 us | 374.29 us | -54.83% | 374.43 KB | 30.17 KB | -91.94% |
| 中文多行轉換 | 8,614.60 us | 3,053.40 us | -64.56% | 4,301.49 KB | 339.05 KB | -92.12% |
| 英文多行轉換 | 4,782.20 us | 3,443.42 us | -28.00% | 1,041.13 KB | 115.28 KB | -88.93% |
| 中英混合多行轉換 | 3,580.20 us | 1,830.19 us | -48.88% | 1,346.76 KB | 116.83 KB | -91.33% |
| 長中文字串轉換 | 8,368.70 us | 2,916.53 us | -65.15% | 4,303.15 KB | 337.60 KB | -92.15% |

7 個 benchmark 的平均時間全部改善，配置量也全部下降。配置量下降幅度大約落在 89% 到 92%。

這個數字很漂亮，但必須補一句限制：這份比較是整個 branch 的累積效果，不是單一 commit 或單一 phase 的效果。它包含資料表重構，也包含後續 builder / view boundary 的調整。

## Benchmark 怎麼做

這種重構如果只看感覺，很容易被錯誤結論帶走。

我採用的方式是：

1. 用兩個乾淨 detached worktree 分別 checkout baseline 與 candidate。
2. 兩邊各自 restore、build、跑 BenchmarkDotNet。
3. baseline 當時還沒有 benchmark 專案，所以只補入相同 benchmark harness。
4. benchmark harness 不修改 baseline production code，只是讓兩邊可以用同一個入口量測。

執行命令大致如下：

```powershell
dotnet run --project src/EasyBrailleEditApp/BrailleToolkit.Benchmarks/BrailleToolkit.Benchmarks.csproj -c Release -- --filter *BrailleConversionBenchmarks*
```

這點很重要。因為在重構過程中，我曾經遇過「第一次量測看起來退步很多，clean worktree A/B 重跑後沒有重現」的情況。如果沒有把 benchmark harness、測資、SDK、建置狀態與 worktree 都控制住，很容易把量測噪音誤判成架構結論。

## 原本的瓶頸：DataTable 查表

點字轉換核心需要查多種 XML 對照表，例如中文注音、聲調、標點、英文縮寫、URL 字元等等。

原本的 `XmlBrailleTable` 使用 `System.Data.DataTable` 來存 XML 載入後的資料，查詢時常見模式像這樣：

```csharp
string filter = "type='Phonetic' and text='" + text + "'";
DataRow[] rows = m_Table.Select(filter);
if (rows.Length > 0)
    return rows[0]["code"].ToString();
return null;
```

這有幾個問題：

- `DataTable` 本身是重量級容器。
- `Select(...)` 需要解析 filter 字串。
- 文字查詢還要處理 escaping，例如單引號。
- 每次查詢都在比較通用的資料結構上操作，而不是針對 domain lookup 建立索引。

但這些 XML 對照表有一個非常重要的特性：

> 載入完成後，它們幾乎不再修改，只會被大量查詢。

這正是 immutable data + optimized lookup 很適合發揮的地方。

## 重構後：BrailleTableEntry + FrozenDictionary

我先新增一個 immutable entry 型別：

```csharp
public readonly record struct BrailleTableEntry(
    string Text,
    string Dots,
    string Code,
    string? Type = null,
    string? Dots2 = null,
    string? Code2 = null,
    bool Joined = false,
    bool Mono = false,
    string? Rule = null,
    string? Description = null);
```

接著把 XML 載入改成用 `XDocument` 解析，直接在建立 `BrailleTableEntry` 時把 `dots` / `dots2` 轉成 `Code` / `Code2`。

載入完成後，建立三組 frozen index：

```text
text -> entry
(type, text) -> entry
type -> ordered entry[]
```

實作上使用的是：

```csharp
FrozenDictionary<string, BrailleTableEntry>
FrozenDictionary<BrailleTableLookupKey, BrailleTableEntry>
FrozenDictionary<string, BrailleTableEntry[]>
```

查詢就變成 domain-specific lookup：

```csharp
BrailleTableEntry? entry = FindEntry(text, "Phonetic");
return entry?.Code;
```

這個改動的語意也比舊版清楚。以前 XML 中如果出現重複 key，可能會被 DataTable 行為或查詢順序掩蓋。現在載入時若出現重複 `text` 或重複 `(type, text)`，直接拋錯，讓資料問題早點暴露。

## Phase 3 單獨效果

資料表重構完成後，我也做過 Phase 3 的 clean worktree A/B。這次比較更能看出 `DataTable` 到 frozen index 的直接影響。

幾個代表數字：

| 案例 | Before | After | Mean 改善 | Allocation 改善 |
| ---- | ----: | ----: | ----: | ----: |
| 中文單行 | 166.00 us | 68.70 us | -58.61% | -91.75% |
| 中英混合單行 | 956.00 us | 400.45 us | -58.11% | -92.01% |
| 中文多行 | 8,901.10 us | 3,420.40 us | -61.57% | -92.10% |
| 長中文字串 | 6,332.10 us | 3,530.79 us | -44.24% | -92.07% |
| 英文單行 | 526.20 us | 514.98 us | -2.13% | -89.65% |

中文與混合內容改善特別明顯，這符合預期：中文路徑更依賴注音與標點表查詢，`DataTable.Select(...)` 的成本被替換掉後，效益自然更大。

## 這裡用到的 C# 與 .NET 型別

這一段重構用到的重點其實不多，但都很精準：

| 技術 | 用途 |
| ---- | ---- |
| `readonly record struct` | 表示小型 immutable value object，例如 `BrailleTableEntry` |
| `XDocument` | 取代 `DataSet.ReadXml(...)` / `DataTable` 載入流程 |
| `FrozenDictionary<TKey,TValue>` | 建立後不再修改、大量查詢的索引 |
| `IReadOnlyList<T>` | 對外暴露 immutable entry 陣列的唯讀檢視 |
| nullable reference types | 對 XML optional attribute，例如 `Type`、`Dots2`、`Code2` 做明確建模 |

其中 `FrozenDictionary` 是這次非常合適的選擇。它不是一般意義上的 persistent immutable collection，也不是每次修改都回傳新版本的 collection。它適合「先建好，之後大量查詢」的場景。

這剛好就是點字表。

## 不要把結論寫成「immutable 一定比較快」

這次我最想保留的結論反而是這句：

> immutable design 變快，不是因為「不可變」本身有魔法，而是因為它迫使我們把資料生命週期講清楚。

資料表的生命週期很簡單：

```text
啟動或首次使用時載入
建立 lookup index
之後只查詢，不修改
```

一旦把生命週期講清楚，就很自然會發現 `DataTable` 不是最合適的資料結構。`BrailleTableEntry[]` 加 frozen index 才是。

這也是這次 allocation 大幅下降的主要原因。

## 這篇的結論

第一階段最值得帶走的經驗是：

- 對載入後不再修改的資料，先考慮 immutable entry 與專用索引。
- `DataTable` 很方便，但在 hot path lookup 中可能非常昂貴。
- `FrozenDictionary` 適合用於「建構一次，查詢很多次」的資料。
- benchmark 要用 clean worktree A/B，不要只相信單次 snapshot。
- 這次整體 branch 到 Phase 4d 為止，7 個 conversion benchmark 全部變快，allocation 全部下降約 89% 到 92%。

下一篇我會談比較麻煩、也比較有趣的部分：`BrailleWord` / `BrailleLine` 這類既有 mutable model 要怎麼逐步導入 immutable boundary，而不是一口氣全部改成 record。
