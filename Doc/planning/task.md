# 雙模式點字轉換架構實施

## 目標

保留 Txt2Brl 專案作為外部工具選項，同時新增內建轉換服務。透過組態檔 `UseInProcessConversion` 控制使用哪種轉換模式。

## 策略優勢

- ✅ 保留穩定的外部工具作為退路
- ✅ 新增內建轉換提升效能
- ✅ 透過組態檔靈活切換
- ✅ 可以比較兩種模式的效能
- ✅ 降低部署風險

## 階段一：研究與分析

- [x] 了解 Txt2Brl 專案的功能範圍
- [x] 分析 Txt2Brl.exe 的啟動參數和介面
- [x] 檢視 EasyBrailleEdit 如何呼叫 Txt2Brl
- [x] 識別 Txt2Brl 專案的所有依賴項
- [x] 評估潛在的風險點

## 階段二：制定計畫

- [x] 建立 implementation_plan.md
- [x] 定義整合策略
- [x] 識別需要修改的檔案
- [x] 規劃測試策略
- [x] 取得使用者審核
- [x] 根據使用者回饋修改為雙模式架構

## 階段三：實作整合

- [x] 建立 Services 目錄
- [x] 定義 IBrailleConverter 介面
- [x] 定義 BrailleConversionResult 類別
- [x] 實作 InProcessBrailleConverter（內建模式）
- [x] 實作 ExternalBrailleConverter（外部工具模式）
- [x] 修改 AppConfig 新增 UseInProcessConversion
- [x] 建立 BrailleConverterFactory
- [x] 修改 MainForm 使用新架構

## 階段四：測試驗證

- [x] 單元測試（兩種模式）
  - [x] InProcessBrailleConverter 測試（4 個測試通過）
  - [x] ExternalBrailleConverter 測試（2 個測試通過）
- [x] 整合測試
  - [x] BrailleConverterFactory 測試（3 個測試通過）
- [x] 記憶體洩漏測試
  - [x] 重複建立轉換器測試（通過）
  - [x] 重複使用同一轉換器測試（通過）
  - [x] 大型文件轉換記憶體釋放測試（通過）
  - [x] IDisposable 實作測試（通過）
  - [x] 多實例記憶體管理測試（通過）
- [x] 使用者驗收測試

## 階段五：文件與部署

- [x] 更新使用者文件
- [x] 更新開發者文件
- [ ] 準備發布

## 風險與注意事項

- ⚠️ 確認內建模式沒有記憶體洩漏問題
- ⚠️ 確保兩種模式產生一致的結果
- ⚠️ 長期觀察效能表現

## 階段六：即時點字預覽功能

- [x] 研究與評估
  - [x] 評估 PreviewConversionForm 的可行性
  - [x] 比較 WinForms 控制項與 HTML 呈現方式
- [x] 設計與實作
  - [x] 設計預覽介面 (Text + Phonetic + Braille)
  - [x] 實作檔案儲存事件觸發機制
  - [x] 實作部分內容擷取邏輯 (前後 5 行)
  - [x] 實作預覽呈現邏輯
  - [x] 改進預覽排版：每個 BrailleWord 獨立儲存格
- [ ] 測試驗證
  - [ ] 驗證儲存觸發
  - [ ] 驗證預覽內容正確性
  - [ ] 驗證效能影響

## 階段七：自動更新預覽功能

- [x] 設計與實作
  - [x] 實作 Debounce 機制 (Timer)
  - [x] 整合至 MainForm
- [x] 測試驗證
  - [x] 驗證打字時不更新
  - [x] 驗證停止打字後自動更新
  - [x] 驗證未儲存檔案（新檔案）可正常預覽

