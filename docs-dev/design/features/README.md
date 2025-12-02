# 功能設計文件索引

本目錄包含「易點雙視」應用程式的各項功能設計文件。

## 目錄結構

### 📱 [ui/](ui/) - 使用者介面與互動功能

與使用者直接互動的介面功能設計。

- **[instant-preview-conversion.md](ui/instant-preview-conversion.md)** - 即時點字預覽功能
  - 即時顯示游標附近文字的點字轉換結果
  - 防抖動機制、範圍限制、錯誤回饋
  
- **[quick-preview.md](ui/quick-preview.md)** - 快速預覽功能
  - 預覽明眼字列印結果
  - 不顯示點字，僅檢視排版
  
- **[dual-edit-grid.md](ui/dual-edit-grid.md)** - 雙視編輯網格
  - 以網格同時顯示與編輯明眼字和點字
  - 支援增刪改、重新排版、驗證功能
  
- **[dual-edit-search.md](ui/dual-edit-search.md)** - 雙視編輯搜尋功能
  - FindForm 類別設計
  - 搜尋事件與屬性

### 📄 [document-processing/](document-processing/) - 文件處理與轉換

文件結構處理、標題、頁碼等轉換邏輯。

- **[document-title-processing.md](document-processing/document-title-processing.md)** - 文件標題處理
  - `<標題>` 標籤的使用與轉換
  - BraillePageTitle 類別設計
  
- **[page-number-display.md](document-processing/page-number-display.md)** - 頁碼顯示規則
  - `<P></P>` 標記的轉換方式
  - 原書頁次與點字書頁次的處理
  
- **[yaml-support-design.md](document-processing/yaml-support-design.md)** - YAML 支援設計
  - 點字文件的 YAML 前置資料 (front matter) 支援

### 🔤 [syntax/](syntax/) - 語法規則

各種標籤和特殊語法的說明。

- **[contaxt-tag-syntax.md](syntax/contaxt-tag-syntax.md)** - 情境標籤語法
- **[fraction-syntax.md](syntax/fraction-syntax.md)** - 分數語法
  - `<分數>I&N/D</分數>` 格式
  
- **[original-page-number-syntax.md](syntax/original-page-number-syntax.md)** - 原書頁碼語法
  - `<P>` 標籤處理演算法
  
- **[table-syntax.md](syntax/table-syntax.md)** - 表格語法
  - `<表格>` 標籤與表格字元處理
  
- **[text-tag-syntax.md](syntax/text-tag-syntax.md)** - 文字標籤
  - 如何新增文字標籤

### 📚 [reference/](reference/) - 參考資料與工具

技術參考資料、對應表、歷史文件。

- **[braille-font-table.md](reference/braille-font-table.md)** - 點字字型對應表
  - 點字碼與點字字型碼的完整對應
  - 注音符號、聲調、標點符號對照
  
- **[phrases.md](reference/phrases.md)** - 詞庫檔案設計
  - 系統內建詞庫與使用者自訂詞庫
  - 詞庫檔案格式與處理流程
  
## 相關文件

- [技術文件](../../technical/) - 技術架構與實作細節
- [使用者文件](../../../docs-user/) - 使用者操作手冊
