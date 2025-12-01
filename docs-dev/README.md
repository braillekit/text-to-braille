# 開發與設計文件

本目錄包含易點雙視（EasyBrailleEdit）專案的開發與設計文件。

## 開發工具

- [Visual Studio 2022 Community Edition](https://visualstudio.microsoft.com/zh-hant/vs/community/) - 整合開發環境
- [Antigravity](https://antigravity.google/) - Google 的 AI 程式編輯器
- [Inno Setup](https://jrsoftware.org/isinfo.php) - 安裝程式建置工具
- [Hugo](https://gohugo.io/) - 文件網站建置工具
- [Git](https://git-scm.com/) - 版本控制工具
- [GitHub](https://github.com/) - 程式碼托管平台

## 📚 文件導覽

### 📖 使用者文件

使用者文件位於 [docs-user](../docs-user) 目錄下，使用 Hugo 生成靜態網站。

- **格式**：Markdown
- **位置**：`docs-user/content/`
- **發布**：透過 GitHub Pages 或其他靜態網站託管服務

### 🏗️ 設計文件

位於 [**design/**](design/) 目錄：

- [**architecture/**](design/architecture/) - 系統架構設計
  - 包含 UML 圖、類別圖、流程圖等
- [**features/**](design/features/) - 功能設計文件
  - 包含語法、編輯功能、資料處理等設計說明
- [YAML_Support_Design.md](design/YAML_Support_Design.md) - YAML 檔案格式支援設計

### 🔧 技術文件

位於 [**technical/**](technical/) 目錄：

- [**deployment/**](technical/deployment/) - 部署與發布
  - [deployment-procedure.md](technical/deployment/deployment-procedure.md) - 部署流程完整說明
  - [release-packaging.md](technical/deployment/release-packaging.md) - 發布打包技術文件
- [**development/**](technical/development/) - 開發相關文件
  - [auto-updater.md](technical/development/auto-updater.md) - 自動更新機制
  - [build-output-merge.md](technical/development/build-output-merge.md) - 建置輸出合併
  - [context-tags.md](technical/development/context-tags.md) - 情境標籤技術文件
- [**braille-rules/**](technical/braille-rules/) - 點字規則參考
  - 包含中文、英文 UEB、數學點字等規則文件
- [**printers/**](technical/printers/) - 點字印表機相關文件
- [**testing/**](technical/testing/) - 測試相關文件

### 📅 專案規劃

位於 [**planning/**](planning/) 目錄：

- 包含專案規劃、任務清單等文件

## 🎯 文件貢獻指南

### 命名規範

- **目錄名稱**: 優先使用小寫英文加連字號（kebab-case），例如 `user-guide`
- **檔案名稱**: 優先使用英文，重要的中文文件可保留中文檔名
- **Markdown 檔案**: 優先使用 `.md` 格式

### 文件格式

- **所有文件**: 優先使用 Markdown (`.md`) 以利版本控制與閱讀
- **設計圖表**: 支援 `.png`, `.gif`, `.svg`, `.uml` (Mermaid) 等格式
- **Legacy 文件**: 保留的舊版文件可能為 PDF 或 DOC 格式，但新文件應避免使用

## 📝 版本說明

- **線上文件**: 使用者手冊採用線上版本，隨軟體版本持續更新
- **歷史文件**: 舊版 PDF/DOC 手冊移至 `archive/` 或保留作為參考
