# BrailleDocument 及相關類別記憶體分析報告

## 1. 摘要

針對 `BrailleDocument`、`BrailleLine`、`BrailleWord` 及 `BrailleCell` 進行了程式碼分析。目前未發現明顯的記憶體洩漏（Memory Leak），例如未取消訂閱的事件或未釋放的非託管資源。然而，發現了幾個記憶體優化機會以及一個嚴重的設計風險。

## 2. 詳細分析

### 2.1 BrailleCell 的設計風險 (嚴重)

`BrailleCell` 類別使用了 **Flyweight Pattern (享元模式)**，透過靜態陣列 `m_AllCells` 快取所有可能的點字方（0x00 ~ 0xFF）。`GetInstance` 方法會回傳這些共用的實例。

**問題：**
`Value` 屬性擁有 `public` 的 `set` 存取子：

```csharp
// BrailleCell.cs
public byte Value
{
    get { return m_Value; }
    set { m_Value = value; } // 危險！
}
```

**風險：**
由於 `BrailleCell` 實例是全域共用的，如果任何一段程式碼修改了某個 `BrailleCell` 物件的 `Value`，將會影響到整個應用程式中所有使用該點字方的地方。這違反了 Flyweight 模式的不可變（Immutable）原則。

**建議：**
移除 `Value` 的 `set` 存取子，或將其改為 `private`，確保 `BrailleCell` 為不可變物件。

### 2.2 BrailleWord 的記憶體優化

#### 2.2.1 注音碼列表 (PhoneticCodes)

```csharp
// BrailleWord.cs
private List<string> m_PhoneticCodes;

public BrailleWord(string text)
{
    // ...
    m_PhoneticCodes = new List<string>(); // 總是初始化
    // ...
}
```

**問題：**
每個 `BrailleWord` 建構時都會初始化 `m_PhoneticCodes` 列表。然而，只有中文字（且是多音字或需校正時）才需要此列表。對於大量的英文字母、數字、標點符號或非多音中文字，這是一個不必要的記憶體開銷（空的 `List<T>` 物件）。

**建議：**
改為 **Lazy Initialization (延遲初始化)**。只有在真正需要加入注音碼時才建立 List。

#### 2.2.2 原始文字 (OriginalText)

`BrailleWord` 同時儲存了 `Text` 和 `OriginalText`。對於絕大多數情況，這兩者是相同的。這導致了字串內容的重複儲存，增加了記憶體壓力。

**建議：**
如果 `OriginalText` 與 `Text` 相同，可以讓它們指向同一個字串實例（String Interning 或直接賦值），或者僅在兩者不同時才儲存 `OriginalText`。

### 2.3 BrailleDocument 的 DeepCopy 效能

```csharp
// BrailleDocument.cs
public BrailleDocument DeepCopy()
{
    string jsonStr = JsonHelper.Serialize(this);
    return JsonHelper.Deserialize<BrailleDocument>(jsonStr);
}
```

**問題：**
目前的 `DeepCopy` 實作依賴 JSON 序列化與反序列化。這涉及大量的字串配置、Reflection 操作以及 JSON 解析，效能較差且產生大量臨時物件（Garbage）。

**建議：**
實作手動的 Deep Copy 邏輯（類似 `BrailleLine.DeepCopy`），直接複製物件結構，以提升效能並減少記憶體波動。

### 2.4 IDisposable 實作

`BrailleDocument.cs` 中有 `TODO: Dispose pattern` 的註解。

**分析：**
目前 `BrailleDocument` 持有的資源（`List`、`BrailleProcessor`）皆為託管資源。除非 `BrailleProcessor` 本身持有非託管資源或需要取消訂閱全域事件，否則 `BrailleDocument` 目前不需要實作 `IDisposable`。

若未來將 `BrailleProcessor` 改為 Transient 並實作 `IDisposable`，則 `BrailleDocument` 也應跟進實作以釋放 `m_Processor`。

## 3. 總結與建議行動

1.  **修復 `BrailleCell` 不可變性**：立即將 `Value` setter 設為私有。（高優先級，防止潛在 Bug）
2.  **優化 `BrailleWord`**：對 `m_PhoneticCodes` 實施延遲初始化。（中優先級，減少記憶體佔用）
3.  **優化 `DeepCopy`**：重寫 `BrailleDocument.DeepCopy`。（低優先級，視效能需求而定）
