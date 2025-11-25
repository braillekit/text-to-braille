# 即時點字預覽功能實作計畫

## 目標
在使用者儲存檔案時，即時預覽游標所在位置前後 5 行的點字轉換結果。預覽內容需包含：原始文字、注音（若有）、點字。

## 評估結果
- **可行性**：`PreviewConversionForm` 作為容器完全可行。
- **呈現方式**：建議使用 `WebBrowser` 控制項搭配 HTML/CSS 呈現。
  - **優點**：
    - 易於實作三行對照（文字、注音、點字）的排版（使用 HTML Table）。
    - 內建捲軸與縮放支援。
    - 無需引入額外第三方元件（如 WebView2）。
  - **缺點**：基於 IE 核心，但對於簡單的靜態內容預覽已足夠。

## 使用者審查事項
- [ ] 確認是否同意使用 `WebBrowser` 控制項（基於 IE 核心）而非 `WebView2`（需額外安裝 Runtime 和 NuGet 套件）。
- [ ] 確認「前後 5 行」的定義是以「游標所在行」為中心，向上 5 行、向下 5 行，共約 11 行。

## 擬定變更

### EasyBrailleEditApp

#### [NEW] [PreviewConversionForm.cs](file:///d:/Projects/BrailleKit/text-to-braille/Source/EasyBrailleEditApp/EasyBrailleEdit/PreviewConversionForm.cs)
- 加入 `WebBrowser` 控制項。
- 實作 `UpdatePreview(List<BrailleLine> lines)` 方法：
  - 生成 HTML 字串。
  - HTML 結構：使用 `<table border="1">` 或 CSS Grid。
  - 每一格包含三層：
    1. `<div class="text">` (原始文字)
    2. `<div class="phonetic">` (注音)
    3. `<div class="braille">` (點字，使用字型或圖片)

#### [MODIFY] [MainForm.cs](file:///d:/Projects/BrailleKit/text-to-braille/Source/EasyBrailleEditApp/EasyBrailleEdit/MainForm.cs)
- 修改 `SaveFile()` 方法：
  - 在儲存成功後，檢查 `m_PreviewConversionForm` 是否開啟。
  - 若開啟，則觸發預覽更新。
- 實作 `GetPreviewContent()` 方法：
  - 取得 `m_TextArea.CurrentLine`。
  - 計算 StartLine 與 EndLine (CurrentLine +/- 5)。
  - 擷取該範圍的文字。
- 實作 `UpdatePreviewConversion()` 方法：
  - 呼叫 `BrailleConverter` 進行部分轉換。
  - 將轉換結果傳遞給 `PreviewConversionForm`。

## 驗證計畫

### 手動驗證
1. **啟用預覽**：
   - 點擊「啟用即時預覽」按鈕，確認視窗跳出。
2. **觸發預覽**：
   - 開啟一個既有檔案或儲存新檔案。
   - 移動游標到不同位置。
   - 按下 Ctrl+S 存檔。
   - 確認預覽視窗內容更新，且顯示游標附近的內容。
3. **內容驗證**：
   - 輸入中文「測試」，確認預覽顯示：
     - 文字：「測試」
     - 注音：「ㄘㄜˋ ㄕˋ」
     - 點字：對應的點字碼
4. **邊界測試**：
   - 在文件第一行存檔。
   - 在文件最後一行存檔。
   - 確認程式不會崩潰且範圍正確。
