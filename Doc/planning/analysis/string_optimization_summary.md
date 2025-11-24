# 字串操作效率改善摘要

## 改善日期
2025-11-24

## 改善範圍
針對效能分析報告「2.1 字串操作效率問題（優先級：高）」的兩個問題進行改善。

---

## 改善 1: 優化字串反轉操作

### 問題描述
每次轉換一行文字都需要呼叫 `StrHelper.Reverse()` 來反轉整個字串，然後再建立 `Stack<char>`，造成：
- 建立臨時的反轉字串（O(n) 時間與空間）
- 然後又建立 Stack（再次 O(n)）

### 受影響的檔案
- `BrailleProcessor.cs`

### 修改前
```csharp
// SimpleConvertText 方法 (第 429-430 行)
Text = StrHelper.Reverse(Text);
Stack<char> charStack = new Stack<char>(Text);

// ConvertLine 方法 (第 514-515 行)
line = StrHelper.Reverse(line);
Stack<char> charStack = new Stack<char>(line);
```

### 修改後
```csharp
// SimpleConvertText 方法 (第 429-434 行)
// 直接從原字串倒序建立 Stack，避免 Reverse 操作建立臨時字串。
Stack<char> charStack = new Stack<char>(Text.Length);
for (int i = Text.Length - 1; i >= 0; i--)
{
    charStack.Push(Text[i]);
}

// ConvertLine 方法 (第 518-523 行)
// 直接從原字串倒序建立 Stack，避免 Reverse 操作建立臨時字串。
Stack<char> charStack = new Stack<char>(line.Length);
for (int idx = line.Length - 1; idx >= 0; idx--)
{
    charStack.Push(line[idx]);
}
```

### 效能影響
- **記憶體配置減少**：每行文字減少一次字串物件的建立
- **時間複雜度**：仍為 O(n)，但減少了字串建立的開銷
- **估計效能提升**：對於處理大量文字行的場景，記憶體使用量和 GC 壓力將明顯降低

---

## 改善 2: 簡化特殊字元檢查

### 問題描述
為了檢查 4 個字元是否為 "&gt;" 或 "&lt;"，每次都建立 `StringBuilder` 並進行字串比對，效率不佳。

### 受影響的檔案
- `EnglishWordConverter.cs`
- `UrlConverter.cs`

### 修改前
```csharp
// EnglishWordConverter.cs (第 188-193 行)
// UrlConverter.cs (第 123-128 行)
StringBuilder sb = new StringBuilder();
sb.Append(ch);
sb.Append(ch2);
sb.Append(ch3);
sb.Append(ch4);
if (sb.ToString().Equals("&gt;"))
{
    text = ">";
    isExtracted = true;
}
else if (sb.ToString().Equals("&lt;"))
{
    text = "<";
    isExtracted = true;
}
```

### 修改後
```csharp
// EnglishWordConverter.cs (第 187-196 行)
// UrlConverter.cs (第 122-131 行)
// 直接比對字元，不需要 StringBuilder。
// 特殊字元: "&gt;" 表示 '>' 和 "&lt;" 表示 '<'
if (ch == '&' && ch2 == 'g' && ch3 == 't' && ch4 == ';')
{
    text = ">";
    isExtracted = true;
}
else if (ch == '&' && ch2 == 'l' && ch3 == 't' && ch4 == ';')
{
    text = "<";
    isExtracted = true;
}
```

### 效能影響
- **記憶體配置減少**：每次檢查減少 1 個 StringBuilder 和 2 個字串物件的建立
- **時間複雜度**：從字串建立與比對改為直接字元比對，效率顯著提升
- **估計效能提升**：在處理含有 HTML 實體的文件時，效能提升將更明顯

### 程式碼清晰度
- 新增註解說明 "&gt;" 和 "&lt;" 的意義，符合使用者的建議
- 程式碼更直觀，一眼就能看出在比對什麼字元

---

## 驗證結果

### 建置狀態
✅ **成功** - 所有專案建置成功，無編譯錯誤

### 測試結果
✅ **所有測試通過**
- 總測試數：131
- 失敗：0
- 成功：131
- 已跳過：0

### 測試涵蓋範圍
測試包含：
- 中文點字轉換
- 英文 UEB 點字轉換
- 特殊字元處理（包括 "&gt;" 和 "&lt;"）
- 各種情境標籤處理

---

## 後續建議

已完成高優先級的字串操作效率改善，建議接下來處理：

### 🟡 中優先級
1. **減少 Stack.ToArray() 呼叫** - 在 `EnglishWordConverter.cs`、`UrlConverter.cs` 和 `TwChineseCharConverter.cs` 中優化情境標籤檢查
2. **考慮快取注音查詢結果** - 在 `TwChineseCharConverter.cs` 中對常見詞彙建立快速查找表

### 🟢 低優先級
3. **簡化 List 初始化** - 移除轉換器中不必要的 null 檢查

---

## 效能測量建議

為了量化這些改善的實際效益，建議：

1. 使用 `BenchmarkDotNet` 進行效能測試
2. 測試場景：
   - 短文本（100 字以內）
   - 中等文本（1000 字）
   - 長文本（10000 字以上）
   - 含大量 HTML 實體的文本

3. 測量指標：
   - 轉換時間
   - 記憶體配置量
   - GC 發生次數
