# 易點雙視 文件中心

本目錄包含易點雙視（EasyBrailleEdit）專案的所有文件，包括使用者手冊、技術文件、設計文件和發行紀錄。

## 📚 文件導覽

### 🚀 快速開始

- **安裝指南**: [manual/installation/current/](manual/installation/current/)
- **使用手冊**: [manual/user-guide/current/](manual/user-guide/current/)
- **最新發行紀錄**: [releases/v5.0.0-alpha.md](releases/v5.0.0-alpha.md)

### 📖 使用者文件

- [**manual/**](manual/) - 使用者手冊與安裝指南
  - [user-guide/](manual/user-guide/) - 使用手冊（含當前版本與歷史版本）
  - [installation/](manual/installation/) - 安裝手冊

### 🏗️ 設計文件

- [**design/**](design/) - 系統設計與架構文件
  - [architecture/](design/architecture/) - 系統架構設計
  - [features/](design/features/) - 功能設計文件
  - [YAML_Support_Design.md](design/YAML_Support_Design.md) - YAML 檔案格式支援設計

### 🔧 技術文件

- [**technical/**](technical/) - 開發者技術文件
  - [development/](technical/development/) - 開發相關文件
    - [auto-updater.md](technical/development/auto-updater.md) - 自動更新機制
    - [build-output-merge.md](technical/development/build-output-merge.md) - 建置輸出合併
    - [新符號加入時的修改步驟.md](technical/development/新符號加入時的修改步驟.md) - 新增符號指南
    - [examples/](technical/development/examples/) - 程式碼範例
  - [braille-rules/](technical/braille-rules/) - 點字規則參考
    - [chinese/](technical/braille-rules/chinese/) - 中文點字規則
    - [UEB/](technical/braille-rules/UEB/) - 英文 UEB 點字規則
  - [printers/](technical/printers/) - 點字印表機相關文件
  - [deployment.md](technical/deployment.md) - 部署程序
  - [testing/](technical/testing/) - 測試相關文件

### 📦 發行紀錄

- [**releases/**](releases/) - 版本發行紀錄與變更日誌
  - [CHANGELOG.md](releases/CHANGELOG.md) - 完整變更歷史
  - [v5.0.0-alpha.md](releases/v5.0.0-alpha.md) - 當前版本發行紀錄

## 🎯 文件貢獻指南

### 命名規範

- **目錄名稱**: 使用小寫英文加連字號（kebab-case），例如 `user-guide`
- **檔案名稱**: 優先使用英文，重要的中文文件可保留中文檔名
- **Markdown 檔案**: 優先使用 `.md` 格式，提升版本控制友善度

### 文件格式

- **技術文件**: 優先使用 Markdown (`.md`)
- **使用者手冊**: 可使用 PDF 或 DOCX 以保留格式
- **設計圖表**: 支援 `.png`, `.svg`, `.uml` 等格式

## 📝 版本說明

- **current/**: 目前版本的文件
- **archive/**: 歷史版本的文件（用於參考）

## 📧 聯絡資訊

如有任何問題或建議，請聯繫：huanlin.tsai@gmail.com
