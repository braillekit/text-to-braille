# Txt2Brl 記憶體洩漏分析報告

## 1. 摘要

經過對 `Txt2Brl` 及核心類別庫 `BrailleToolkit` 的程式碼分析，發現了一個明顯的記憶體洩漏風險。該風險主要源於 `BrailleConverter` 類別與單例模式（Singleton）的 `BrailleProcessor` 之間的事件訂閱機制。

雖然 `Txt2Brl` 作為一個執行一次即結束的命令列工具，此洩漏在單次執行中影響有限，但若將相關程式碼移植至長期運行的應用程式（如 GUI 編輯器或服務）中，將會導致嚴重的記憶體洩漏。

## 2. 詳細分析

### 2.1 BrailleProcessor 的單例模式

`BrailleToolkit.BrailleProcessor` 類別被設計為單例模式：

```csharp
// BrailleProcessor.cs
private static BrailleProcessor? s_Processor;

public static BrailleProcessor GetInstance(ZhuyinReverseConverter? zhuyinConverter = null)
{
    if (s_Processor != null)
    {
        return s_Processor;
    }
    // ...
    s_Processor = new BrailleProcessor(zhuyinConverter);
    return s_Processor;
}
```

這意味著在應用程式的整個生命週期中，只有一個 `BrailleProcessor` 實例，且它不會被記憶體回收機制（GC）回收。

### 2.2 事件訂閱導致的強引用

在 `Txt2Brl.BrailleConverter` 的建構函式中，程式碼訂閱了 `BrailleProcessor` 的事件：

```csharp
// BrailleConverter.cs
public BrailleConverter()
{
    // ...
    Processor = BrailleProcessor.GetInstance(_zhuyinConverter);
    // ...
    Processor.ConvertionFailed += BrailleProcessor_ConvertionFailed;
    Processor.TextConverted += BrailleProcessor_TextConverted;
    // ...
}
```

### 2.3 缺失的取消訂閱（Unsubscribe）

`BrailleConverter` 類別沒有實作 `IDisposable` 介面，也沒有在任何地方（如 `FinalizeConversion` 方法）取消訂閱這些事件。

由於 `BrailleProcessor` 是靜態單例，它會透過事件委派（Delegate）持有所有訂閱者（即 `BrailleConverter` 實例）的強引用。因此，即使 `BrailleConverter` 的工作已經完成且不再被外部使用，GC 也無法回收它，因為 `BrailleProcessor` 仍然「抓著」它不放。

如果 `BrailleConverter` 被重複建立（例如在迴圈中處理多個檔案），記憶體中將會堆積大量的 `BrailleConverter` 實例，導致記憶體洩漏。

## 3. 其他潛在問題

### 3.1 靜態快取

程式碼中呼叫了 `ZhuyinQueryHelper.Initialize()`。若此類別內部維護了大量的靜態資料（如注音字根表），且沒有釋放機制，這也是記憶體佔用的一個來源。但在點字轉換的情境下，這通常是必要的快取，只要不無限增長，通常視為正常行為。

### 3.2 BrailleDocument 的資源釋放

`BrailleDocument` 類別雖然有 `Clear()` 方法，但沒有實作 `IDisposable`。它持有 `BrailleProcessor` 的參考（雖然是單例）。建議檢查 `BrailleDocument` 是否持有其他非託管資源或大型物件，並考慮實作 `IDisposable` 以便明確釋放。

## 4. 建議解決方案

### 4.1 實作 IDisposable 模式

建議讓 `BrailleConverter` 實作 `IDisposable` 介面，並在 `Dispose` 方法中取消訂閱事件：

```csharp
public class BrailleConverter : IDisposable
{
    // ...
    public void Dispose()
    {
        if (Processor != null)
        {
            Processor.ConvertionFailed -= BrailleProcessor_ConvertionFailed;
            Processor.TextConverted -= BrailleProcessor_TextConverted;
        }
    }
}
```

### 4.2 使用弱事件模式（Weak Event Pattern）

如果無法確保 `Dispose` 一定會被呼叫（例如開發者忘記呼叫，或物件生命週期管理較為複雜），可以考慮在 `BrailleProcessor` 中使用弱事件模式來實作事件。這樣一來，`BrailleProcessor` 只會持有訂閱者的「弱引用」（Weak Reference），當訂閱者不再被其他物件引用時，GC 就可以正常回收它，而不會因為事件訂閱而被 `BrailleProcessor` 強制留住。

在 .NET 中，可以使用 `WeakEventManager` 來實作，或者自行封裝 `WeakReference`。以下是一個自行實作的簡單範例概念：

```csharp
public class BrailleProcessor
{
    // 使用 List<WeakReference> 來儲存訂閱者
    private List<WeakReference<EventHandler<ConversionFailedEventArgs>>> _conversionFailedSubs = new();

    public event EventHandler<ConversionFailedEventArgs> ConvertionFailed
    {
        add
        {
            _conversionFailedSubs.Add(new WeakReference<EventHandler<ConversionFailedEventArgs>>(value));
        }
        remove
        {
            // 移除邏輯需遍歷 List 找到對應的 WeakReference
            _conversionFailedSubs.RemoveAll(wr => 
                wr.TryGetTarget(out var handler) && handler == value);
        }
    }

    protected virtual void OnConvertionFailed(ConversionFailedEventArgs args)
    {
        // 觸發事件時，清理已回收的訂閱者
        for (int i = _conversionFailedSubs.Count - 1; i >= 0; i--)
        {
            if (_conversionFailedSubs[i].TryGetTarget(out var handler))
            {
                handler(this, args);
            }
            else
            {
                _conversionFailedSubs.RemoveAt(i); // 訂閱者已被回收，移除紀錄
            }
        }
    }
}
```

**優點：**

- 即使忘記取消訂閱，也不會造成記憶體洩漏。

**缺點：**

- 實作較為複雜，且每次觸發事件時需要額外的檢查與清理開銷。
- 在 .NET 4.5+ 可使用 `WeakEventManager` 簡化實作，但仍比標準事件重。

### 4.3 檢視單例模式的必要性

使用者提出了關於 `BrailleProcessor` 是否必須為單例（Singleton）的疑問。經過進一步的程式碼分析，我們有以下發現：

#### 4.3.1 現狀分析：為何目前需要單例？

目前的 `BrailleProcessor` 建構函式會建立多個轉換器，例如 `TwChineseCharConverter` 和 `EnglishWordConverter`。這些轉換器在建構時，會分別建立新的點字對照表實例：

```csharp
// TwChineseCharConverter.cs
public TwChineseCharConverter(BrailleProcessor processor)
{
    _brailleTable = TwChineseBrailleTable.CreateInstance(); // 建立新實例
    // ...
}
```

而 `TwChineseBrailleTable`（繼承自 `XmlBrailleTable`）在建立實例時，會從資源檔中讀取 XML 並解析成 `DataTable`。這是一個相對昂貴（Heavy）的操作。

因此，若目前的程式碼將 `BrailleProcessor` 改為非單例（Transient），每次建立 `BrailleProcessor` 都會重複進行昂貴的 XML 解析與記憶體配置，這將對效能造成顯著影響。從這個角度來看，目前的單例設計在效能上是有其必要性的。

#### 4.3.2 改進建議：如何移除單例？

雖然目前有其必要性，但我們可以透過重構來優化設計，進而安全地移除 `BrailleProcessor` 的單例模式，從根本上解決記憶體洩漏問題。

**步驟如下：**

1. **共用點字對照表實例**：
    `TwChineseBrailleTable` 和 `EnglishBrailleTable` 類別其實已經實作了單例模式（`GetInstance()` 方法）。我們應該修改轉換器，讓它們使用共用的對照表實例，而不是每次都建立新的。

    ```csharp
    // 修改後的 TwChineseCharConverter.cs
    public TwChineseCharConverter(BrailleProcessor processor)
    {
        _brailleTable = TwChineseBrailleTable.GetInstance(); // 使用共用實例
        // ...
    }
    ```

2. **將 BrailleProcessor 改為 Transient**：
    一旦轉換器不再負責載入昂貴的資源（改由單例的 Table 負責），`BrailleProcessor` 的建構成本將大幅降低（只剩下輕量級的物件配置）。此時，我們就可以安全地移除 `BrailleProcessor` 的單例模式，改為每次需要轉換時都建立一個新的 `BrailleProcessor`。

3. **自然解決記憶體洩漏**：
    當 `BrailleProcessor` 不再是全域單例後，它與 `BrailleConverter` 之間的事件訂閱關係將隨著物件生命週期結束而自然解除（因為兩者會一起被回收），從而無需實作複雜的弱事件模式或依賴 `Dispose`。

**結論：**
建議優先採用此重構方案。它不僅解決了記憶體洩漏，也讓物件職責更清晰（資源管理歸資源類別，邏輯處理歸處理器類別）。

#### 4.3.3 實作狀態更新 (2025-11-23)

已完成第一階段重構：

1. **共用實例**：已修改所有 Converter (`TwChineseCharConverter`, `EnglishWordConverter`, `UrlConverter`, `TableConverter`, `PhoneticConverter`, `MathConverter`, `CoordinateConverter`) 改用 `BrailleTable.GetInstance()` 取得共用的點字對照表單例。
2. **移除冗餘代碼**：已移除所有 `BrailleTable` 類別中不再使用的 `CreateInstance()` 方法，確保未來不會誤用。

此變更已大幅降低 `BrailleProcessor` 的初始化成本。下一步將評估移除 `BrailleProcessor` 的單例模式。

#### 4.4 BrailleDocument 記憶體優化 (2025-11-23)

針對 `BrailleDocument` 及其相關類別進行了分析與優化，主要解決了以下問題：

1. **BrailleCell 不可變性 (Immutability)**：
    - **問題**：`BrailleCell` 使用享元模式 (Flyweight Pattern) 共用實例，但其 `Value` 屬性原本擁有公開的 setter，存在被意外修改的風險，可能導致全域性的資料錯誤。
    - **解決**：將 `BrailleCell.Value` 的 setter 改為 `private`，確保共用實例不可變。

2. **BrailleWord 記憶體優化**：
    - **問題**：每個 `BrailleWord` 物件在建構時都會初始化 `m_PhoneticCodes` (List<string>)，但此欄位僅在處理多音字或需校正的中文字時才需要。對於大量的非中文內容或普通中文字，這造成了不必要的記憶體開銷。
    - **解決**：實作 `PhoneticCodes` 的延遲初始化 (Lazy Initialization)，僅在真正需要時才建立 List 物件。

**驗證結果**：

- 專案建置成功。
- 專案建置成功。
- `BrailleToolkit.Tests` 單元測試全數通過 (131/131)，確認無回歸錯誤。

#### 4.5 BrailleProcessor 單例模式移除 (2025-11-23)

移除了 `BrailleProcessor` 的單例模式 (Singleton Pattern)，徹底解決 `Txt2Brl` 的記憶體洩漏問題。

**問題回顧**：
在 `Txt2Brl` 中，`BrailleConverter` 會訂閱 `BrailleProcessor` 的事件。由於 `BrailleProcessor` 是單例，即使 `BrailleConverter` 執行完畢並應被回收，但單例 `BrailleProcessor` 仍持有對 `BrailleConverter` 的參考（透過事件訂閱），導致 `BrailleConverter` 無法被垃圾回收，造成記憶體洩漏。

**解決方案**：
1. 移除 `BrailleProcessor` 的單例實作：
    - 刪除 `s_Processor` 靜態欄位
    - 刪除 `GetInstance()` 方法
    - 將建構函式由 `private` 改為 `public`

2. 更新所有呼叫端，將 `GetInstance()` 改為 `CreateInstance()`，確保每次建立新的 `BrailleProcessor` 實例。

**影響範圍**：
- `BrailleConverter.cs` (Txt2Brl)
- `TextToBrailleConverter.cs`
- `EditCellForm.cs`
- `BrailleGridController_EditCommands.cs`
- `BrailleProcessorBenchmarks.cs`

**驗證結果**：
- 專案建置成功。
- `BrailleToolkit.Tests` 單元測試全數通過 (131/131)。
- 每個 `BrailleConverter` 現在擁有獨立的 `BrailleProcessor` 實例，可隨 `BrailleConverter` 一起被垃圾回收，記憶體洩漏問題已解決。
