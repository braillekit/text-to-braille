# Deployment Documentation

本目錄包含 EasyBrailleEdit 應用程式部署相關的文件。

## 文件說明

### [deployment-procedure.md](deployment-procedure.md)

此文件說明 EasyBrailleEdit 應用程式的部署流程，涵蓋從版本準備到正式發布的所有步驟：

- Git tag 建立
- Release 建置
- 打包檔案準備
- 安裝程式製作
- 自動更新部署
- 發布檢查清單

**適用對象**：發布管理員、開發團隊

### [release-packaging.md](release-packaging.md)

此文件說明如何打包 EasyBrailleEdit 應用程式的安裝程式（setup program）：

- `Prepare-Release.ps1` 腳本使用說明
- 檔案篩選機制（排除 .pdb）
- 平台篩選機制（只保留 Windows）
- Inno Setup 安裝程式製作
- 目錄結構說明

**適用對象**：開發人員、技術維護人員

## 快速開始

如果您要發布新版本，請按以下順序閱讀：

1. 先閱讀 [**deployment-procedure.md**](deployment-procedure.md) 了解完整流程
2. 執行建置和打包時參考 [**release-packaging.md**](release-packaging.md) 的詳細說明

## 相關資源

- [Setup/README.md](../../../Setup/README.md) - Prepare-Release.ps1 腳本使用說明
- [Setup/Prepare-Release.ps1](../../../Setup/Prepare-Release.ps1) - 自動化打包腳本
- [development/auto-updater.md](../development/auto-updater.md) - 自動更新機制說明
