# ClipboardHelper 剪貼簿操作錯誤修正報告

## 問題描述

### 錯誤現象
使用者在執行剪貼簿貼上操作（Ctrl+V）時，應用程式引發 `System.NotSupportedException` 例外。

### 錯誤訊息
```
System.NotSupportedException
Message='System.Object' is not a concrete type, and could result in an unbounded deserialization attempt. 
Please use a concrete type or alternatively define a 'resolver' function that supports types that you are 
expecting to retrieve from the clipboard or use in drag-and-drop operations.
```

### 發生位置
- 檔案：`ClipboardHelper.cs`
- 方法：`GetWords()` 第 38 行
- 相關方法：`GetLines()` 第 52 行

---

## 根本原因分析

### .NET 安全性限制
在較新版本的 .NET（特別是 .NET 10）中，Windows Forms 的剪貼簿 API 引入了新的安全性限制：

1. **類型安全性要求**
   - `Clipboard.TryGetData<T>()` 的泛型參數 `T` 必須是具體類型
   - 不允許使用 `object` 或 `object?` 作為泛型參數
   - 這是為了防止無邊界的反序列化攻擊

2. **舊版 API 已過時**
   - `Clipboard.GetData()` 方法已被標記為過時
   - 雖然仍可使用，但編譯時會產生警告

### 有問題的程式碼
```csharp
// 錯誤：使用 object? 作為泛型參數
if (Clipboard.TryGetData(ClipboardObjectFormatForWords, out object? data))
{
    var s = data as string;
    if (s != null)
    {
        result = JsonHelper.Deserialize<List<BrailleWord>>(s);
    }
}
```

**問題點：**
- 使用 `object?` 作為 `TryGetData` 的泛型參數
- .NET 拒絕這種寫法，因為 `System.Object` 不是具體類型

---

## 解決方案

### 修正後的程式碼

#### GetWords() 方法
```csharp
public static List<BrailleWord>? GetWords()
{
    List<BrailleWord>? result = null;
    if (Clipboard.TryGetData(ClipboardObjectFormatForWords, out string? data))
    {
        if (data != null)
        {
            result = JsonHelper.Deserialize<List<BrailleWord>>(data);
        }
    }
    return result;
}
```

#### GetLines() 方法
```csharp
public static List<BrailleLine>? GetLines()
{
    List<BrailleLine>? result = null;
    if (Clipboard.TryGetData(ClipboardObjectFormatForLines, out string? data))
    {
        if (data != null)
        {
            result = JsonHelper.Deserialize<List<BrailleLine>>(data);
        }
    }
    return result;
}
```

### 修正重點

1. **指定具體類型**
   - 將 `out object? data` 改為 `out string? data`
   - 因為我們知道剪貼簿中儲存的是 JSON 字串

2. **簡化程式碼**
   - 移除不必要的 `as string` 轉型
   - 直接使用 `data` 變數，因為它已經是 `string?` 類型

3. **保持一致性**
   - `SetWords()` 和 `SetLines()` 都是使用 `Clipboard.SetData()` 儲存字串
   - `GetWords()` 和 `GetLines()` 應該也取回字串

---

## 技術說明

### 為何使用 string 而非其他類型？

在 `SetWords()` 和 `SetLines()` 方法中，我們可以看到：

```csharp
var s = JsonHelper.Serialize(brWords);
Clipboard.SetData(ClipboardObjectFormatForWords, s);
```

這表示：
1. 資料被序列化為 JSON 字串
2. 字串被儲存到剪貼簿
3. 因此，從剪貼簿取回的資料也應該是字串

### 為何不使用 Clipboard 內建的序列化？

程式碼註解中已說明：

```csharp
// 注意：這裡不使用 Clipboard 內建的序列化，是因為它會遺漏 BrailleWord 的 PhoneticCode 屬性
```

這就是為何我們需要：
1. 使用 `JsonHelper` 進行序列化/反序列化
2. 以字串格式儲存到剪貼簿
3. 從剪貼簿取回字串並反序列化

---

## 驗證結果

### 建置狀態
✅ **成功** - 無編譯錯誤或警告

### 預期行為
- ✅ 複製點字詞語（BrailleWord）到剪貼簿
- ✅ 從剪貼簿貼上點字詞語
- ✅ 複製點字行（BrailleLine）到剪貼簿
- ✅ 從剪貼簿貼上點字行
- ✅ 不會引發 `System.NotSupportedException`

---

## 相關檔案

- 📄 [`ClipboardHelper.cs`](../../../src/EasyBrailleEditApp/EasyBrailleEdit/DualEdit/ClipboardHelper.cs)
- 📄 [`BrailleGridController_EditCommands.cs`](../../../src/EasyBrailleEditApp/EasyBrailleEdit/DualEdit/BrailleGridController_EditCommands.cs) (呼叫 ClipboardHelper 的地方)

---

## 總結

這個問題的核心是 .NET 10 對剪貼簿 API 的安全性改進。修正方法很簡單：**使用具體的類型（string）而非泛型的 object**。這樣既符合新的安全性要求，又能正確處理剪貼簿資料。
