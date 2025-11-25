# UAT 測試準備與執行指引

## 測試環境已就緒 ✅

### 建置狀態

- ✅ Debug 版本建置成功（16.4 秒）
- ✅ 應用程式位置：`D:\Projects\BrailleKit\text-to-braille\Output\EasyBrailleEdit\Debug\net10.0-windows10.0.17763.0\`

### 組態檔資訊

- **檔案名稱**：`AppConfig.ini`
- **檔案位置**：應用程式執行目錄（首次執行時自動產生）
- **預設值**：`UseInProcessConversion = true`（內建模式）

---

## 開始測試

### 方式一：直接執行應用程式

```powershell
# 啟動應用程式
cd D:\Projects\BrailleKit\text-to-braille\Output\EasyBrailleEdit\Debug\net10.0-windows10.0.17763.0\
.\EasyBrailleEdit.exe
```

### 方式二：使用 dotnet run

```powershell
cd D:\Projects\BrailleKit\text-to-braille\Source\EasyBrailleEditApp
dotnet run --project EasyBrailleEdit\EasyBrailleEdit.csproj --no-build -c Debug
```

---

## 測試案例 1：內建模式基本轉換

### 步驟

1. **啟動應用程式**

2. **確認組態**：首次執行時會自動產生 `AppConfig.ini`，預設使用內建模式

3. **準備測試文字**：

   ```
   這是測試 Test 123
   包含中文、English 和數字
   ```

4. **執行轉換**：
   - 輸入測試文字
   - 點擊「轉換」按鈕
   - 等待轉換完成

5. **檢查結果**：
   - 點字編輯器應該正常開啟
   - 顯示轉換後的點字內容
   - 沒有錯誤訊息

---

## 測試案例 2：外部工具模式基本轉換

### 步驟

1. **修改組態檔**：
   - 關閉應用程式
   - 開啟 `AppConfig.ini`
   - 在 `[Braille]` 區段加入或修改：

     ```ini
     UseInProcessConversion=false
     ```

   - 儲存檔案

2. **重新啟動應用程式**

3. **執行轉換**：
   - 輸入**相同的**測試文字
   - 點擊「轉換」按鈕
   - 等待轉換完成

4. **檢查結果**：
   - 點字編輯器應該正常開啟
   - 顯示轉換後的點字內容
   - 結果應該與測試案例 1 相同

---

## 測試案例 3-7

請依照 `Doc\planning\uat\dual_mode_conversion.md` 中的測試案例繼續執行：

- ✅ 測試案例 3：結果一致性比較
- ✅ 測試案例 4：錯誤字元處理
- ✅ 測試案例 5：自訂詞庫測試
- ✅ 測試案例 6：大型文件轉換
- ✅ 測試案例 7：連續轉換穩定性

---

## 記錄測試結果

請在 `Doc\planning\uat\dual_mode_conversion.md` 檔案中勾選測試結果：

1. 找到對應的測試案例
2. 在「實際結果」部分勾選 `[x] 通過` 或 `[ ] 失敗`
3. 如果失敗，請描述問題

---

## 疑難排解

### 問題：找不到 Txt2Brl.exe

如果外部工具模式報錯找不到 `Txt2Brl.exe`，請檢查：

```powershell
# 檢查 Txt2Brl.exe 是否存在
ls D:\Projects\BrailleKit\text-to-braille\Output\EasyBrailleEdit\Debug\net10.0-windows10.0.17763.0\Txt2Brl.exe
```

如果不存在，需要確認建置時 PostBuild 事件是否正確執行。

### 問題：組態檔在哪裡？

首次執行應用程式後，`AppConfig.ini` 會自動產生在：

```
D:\Projects\BrailleKit\text-to-braille\Output\EasyBrailleEdit\Debug\net10.0-windows10.0.17763.0\AppConfig.ini
```

---

## 完成測試後

請將測試結果總結記錄在 `dual_mode_conversion.md` 的「測試結果摘要」區段。
