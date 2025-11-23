# 易點雙視 文件中心

本目錄包含易點雙視（EasyBrailleEdit）專案的所有文件，包括使用者手冊、技術文件、設計文件和發行紀錄。

## 📚 文件導覽

### 🚀 快速開始

- **使用手冊**: [manual/使用手冊/](manual/使用手冊/)
- **安裝指南**: [manual/安裝手冊/](manual/安裝手冊/)
- **最新發行紀錄**: [v5.0.0-alpha.md](v5.0.0-alpha.md)
- **完整變更歷史**: [CHANGELOG.md](CHANGELOG.md)

### 📖 使用者文件

- [**manual/**](manual/) - 使用者手冊與安裝指南
  - [使用手冊/](manual/使用手冊/) - 使用手冊（繁體中文版）
  - [安裝手冊/](manual/安裝手冊/) - 安裝手冊（繁體中文版）
  - [user-guide/](manual/user-guide/) - 使用手冊（英文版）
  - [installation/](manual/installation/) - 安裝手冊（英文版）
  - [images/](manual/images/) - 手冊圖片資源
  - [Syntax.htm](manual/Syntax.htm) - 語法說明頁面

### 🏗️ 設計文件

- [**design/**](design/) - 系統設計與架構文件
  - [architecture/](design/architecture/) - 系統架構設計
    - [BrailleModel.uml](design/architecture/BrailleModel.uml) - 點字資料模型 UML 圖
    - [類別圖.png](design/architecture/類別圖.png) - 系統類別圖
    - [點字轉換活動圖.png](design/architecture/點字轉換活動圖.png) - 點字轉換流程圖
    - [點字轉換處理流程.doc](design/architecture/點字轉換處理流程.doc) - 轉換流程說明文件
  - [features/](design/features/) - 功能設計文件
    - 語法相關：情境標籤、表格、分數、原書頁碼、文字標籤等
    - 編輯功能：雙視編輯尋找、快速預覽等
    - 資料相關：點字字型對應表、點字資料字典、詞庫處理流程等
  - [YAML_Support_Design.md](design/YAML_Support_Design.md) - YAML 檔案格式支援設計

### 🔧 技術文件

- [**technical/**](technical/) - 開發者技術文件
  - [development/](technical/development/) - 開發相關文件
    - [auto-updater.md](technical/development/auto-updater.md) - 自動更新機制
    - [build-output-merge.md](technical/development/build-output-merge.md) - 建置輸出合併
    - [新符號加入時的修改步驟.md](technical/development/新符號加入時的修改步驟.md) - 新增符號指南
    - [Context Tags.docx](technical/development/Context%20Tags.docx) - 情境標籤技術文件
    - [SourceGrid 筆記.txt](technical/development/SourceGrid%20筆記.txt) - SourceGrid 元件使用筆記
  - [braille-rules/](technical/braille-rules/) - 點字規則參考
    - [國語點字自學手冊/](technical/braille-rules/國語點字自學手冊/) - 中文點字學習資源
    - [UEB/](technical/braille-rules/UEB/) - 英文 UEB 點字規則
    - [數學點字.htm](technical/braille-rules/數學點字.htm) - 數學點字規則（HTML）
    - [數學點字.pdf](technical/braille-rules/數學點字.pdf) - 數學點字規則（PDF）
    - [點字自學.doc](technical/braille-rules/點字自學.doc) - 點字自學文件
    - [20091204-高中數理符號表.doc](technical/braille-rules/20091204-高中數理符號表.doc) - 高中數理符號表
    - [中英文夾雜點寫範例.gif](technical/braille-rules/中英文夾雜點寫範例.gif) - 中英混合點寫範例
  - [printers/](technical/printers/) - 點字印表機相關文件
  - [testing/](technical/testing/) - 測試相關文件
  - [deployment.txt](technical/deployment.txt) - 部署程序

### 📦 發行紀錄

- [CHANGELOG.md](CHANGELOG.md) - 完整變更歷史
- [v5.0.0-alpha.md](v5.0.0-alpha.md) - v5.0.0 Alpha 版本發行紀錄

## 🎯 文件貢獻指南

### 命名規範

- **目錄名稱**: 優先使用小寫英文加連字號（kebab-case），例如 `user-guide`；重要的繁體中文目錄可保留中文名稱
- **檔案名稱**: 優先使用英文，重要的中文文件可保留中文檔名
- **Markdown 檔案**: 優先使用 `.md` 格式，提升版本控制友善度

### 文件格式

- **技術文件**: 優先使用 Markdown (`.md`)
- **使用者手冊**: 可使用 PDF、DOC 或 DOCX 以保留格式
- **設計圖表**: 支援 `.png`, `.gif`, `.svg`, `.uml` 等格式

## 📝 版本說明

文件結構支援多語言與版本管理：

- **繁體中文文件**: 使用中文目錄名稱（如 `使用手冊/`、`安裝手冊/`）
- **英文文件**: 使用英文目錄名稱（如 `user-guide/`、`installation/`）
- 未來可擴充 `current/` 與 `archive/` 子目錄進行版本區分

## 📧 聯絡資訊

如有任何問題或建議，請聯繫：huanlin.tsai@gmail.com
