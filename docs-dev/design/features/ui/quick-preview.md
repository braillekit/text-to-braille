# 快速預覽

## 用途

在編輯明眼字時，可利用快速預覽功能檢視轉換點字之後的列印結果，以便調整文字的編排段落。

- 此預覽的結果**只顯示明眼字**，不顯示點字。

## 處理步驟

1. 當使用者執行快速預覽，程式會先將明眼字轉成點字，然後直接開啟明眼字的預覽視窗。
   - 轉點字時不開啟挑選詞庫檔的對話窗，直接採用上次的設定。
   - 預覽時的列印設定也是採用上次儲存的設定，且固定為單面列印。
   - 只能預覽，不可列印。

## 例外情節

- 轉點字過程發生錯誤：停止預覽，回到明眼字編輯畫面並顯示錯誤的字元。

## 程式實作

### 主要元件

#### 1. MainForm.ConvertAndPrintPreview()

**位置**: MainForm.cs 的 ConvertAndPrintPreview() 方法。

**說明**: 轉點字並預覽明眼字列印結果（只能預覽，不真的印出）。

**流程**:

1. 檢查是否已設定預設印表機
2. 呼叫 `ConvertTextToBrailleAsync()` 將明眼字轉成點字
3. 若轉換失敗則返回（對應例外情節）
4. 建立 `DualPrintDialog` 並載入點字資料
5. 呼叫 `dlg.PreviewText()` 顯示預覽視窗

#### 2. DualPrintDialog.PreviewText()

**位置**: Printing/DualPrintDialog.cs 的 PreviewText 方法。

**說明**: 直接預覽明眼字，而不顯示列印對話窗。

**關鍵實作**:

```csharp
LoadSettings();  // 載入先前儲存的設定
m_DontSaveSettings = true;  // 視窗關閉時不要儲存設定
cboPrintTextManualDoubleSide.SelectedIndex = 0;  // 固定為單面列印
prn.PrintText(true);  // 參數 true 表示僅預覽，不列印
```

### UI 觸發點

- **選單**: 檔案 > 預覽列印 (`miFilePrintPreview`)
- **快速鍵**: `Ctrl+P`
- **程式碼位置**: MainForm.cs 的 `miFilePrintPreview_Click` 方法。

### 實作與設計對照

| 設計需求 | 實作狀態 | 實作位置 |
|---------|---------|---------|
| 轉點字後開啟預覽視窗 | ✅ | `MainForm.cs:611` |
| 不開啟挑選詞庫檔對話窗 | ✅ | `ConvertTextToBrailleAsync()` 直接使用現有設定 |
| 採用上次儲存的設定 | ✅ | `DualPrintDialog.PreviewText()` |
| 固定為單面列印 | ✅ | `DualPrintDialog.PreviewText()` |
| 只能預覽，不可列印 | ✅ | `DualPrintDialog.PreviewText()` |
| 錯誤處理 | ✅ | `MainForm.cs` |
