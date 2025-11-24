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
- [ ] 建立 Services 目錄
- [ ] 定義 IBrailleConverter 介面
- [ ] 定義 BrailleConversionResult 類別
- [ ] 實作 InProcessBrailleConverter（內建模式）
- [ ] 實作 ExternalBrailleConverter（外部工具模式）
- [ ] 修改 AppConfig 新增 UseInProcessConversion
- [ ] 建立 BrailleConverterFactory
- [ ] 修改 MainForm 使用新架構

## 階段四：測試驗證
- [ ] 單元測試（兩種模式）
- [ ] 整合測試
- [ ] 記憶體洩漏測試
- [ ] 效能比較測試
- [ ] 使用者驗收測試

## 階段五：文件與部署
- [ ] 更新使用者文件
- [ ] 更新開發者文件
- [ ] 準備發布

## 風險與注意事項
- ⚠️ 確認內建模式沒有記憶體洩漏問題
- ⚠️ 確保兩種模式產生一致的結果
- ⚠️ 長期觀察效能表現
