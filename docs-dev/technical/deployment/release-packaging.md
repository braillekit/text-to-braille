# EasyBrailleEdit 發布打包流程

本文件說明如何準備和打包 EasyBrailleEdit 新版本的發布檔案。

## 概要

發布流程包含以下主要步驟：

1. 準備發行文件（Release Notes、授權條款）
2. 執行 Release 建置
3. 使用自動化腳本準備打包檔案
4. 使用 Inno Setup 製作安裝程式
5. 測試安裝程式
6. 發布到目標平台

## 前置條件

### 必要工具

- **.NET 10 SDK** - 用於建置專案
- **PowerShell 5.1+** - 用於執行打包腳本
- **Inno Setup** - 用於製作 Windows 安裝程式

### 必要文件

確認以下文件已準備好：

| 檔案 | 位置 | 說明 |
|------|------|------|
| `ReleaseNote.txt` | `Setup/InnoSetup/Files/` | 本次發行的更新說明 |
| `LICENSE.md` | `Setup/InnoSetup/Files/` | 軟體授權條款 |
| `ChangeLog.md` | `Source/EasyBrailleEditApp/` | 完整的變更記錄 |

> **注意**：已不再打包 PDF 使用手冊，改為提供線上使用手冊（位於 `docs-user/`）。

## 發布流程

### 步驟 1：更新版本資訊

確認專案的版本號已正確設定（使用 MinVer 自動版本控制）。

### 步驟 2：執行 Release 建置

在專案根目錄執行：

```powershell
dotnet build .\Source\EasyBrailleEditApp\EasyBrailleEdit\EasyBrailleEdit.csproj -c Release
```

**建置輸出位置**：

```text
Output\EasyBrailleEdit\Release\net10.0-windows10.0.17763.0\
```

**預期結果**：

- `EasyBrailleEdit.exe` - 主程式
- `Txt2Brl.exe` - 命令列工具
- 所有相依的 DLL 檔案
- 設定檔和資源檔

### 步驟 3：準備打包檔案

執行自動化打包腳本：

```powershell
.\Setup\Prepare-Release.ps1
```

#### 腳本功能詳細說明

`Prepare-Release.ps1` 腳本提供以下自動化功能：

1. **前置條件檢查**
   - 驗證建置輸出目錄 `Output\EasyBrailleEdit\Release\net10.0-windows10.0.17763.0` 存在
   - 確認必要檔案（`EasyBrailleEdit.exe`、`Txt2Brl.exe`）存在

2. **目標目錄清理**
   - 清空 `Setup/InnoSetup/Files/app` 目錄
   - 保留手動維護的檔案（`LICENSE.md`、`ReleaseNote.txt`）

3. **智慧型檔案複製**
   - 複製所有可執行檔（`.exe`）和函式庫（`.dll`）
   - 複製設定檔（`.config`、`.json`、`.ini`）和資源檔
   - **自動排除** `.pdb` 除錯符號檔案
   - 複製多語言資源目錄（cs、de、es、fr、it、ja、ko、pl、pt-BR、ru、tr、zh-Hans、zh-Hant）

4. **runtimes 平台篩選**
   - **只保留** Windows 平台：`win`、`win-x64`、`win-x86`、`win-arm64`
   - **排除** Android（`android-*`）、Linux（`linux-*`）、macOS（`osx-*`、`maccatalyst-*`）等其他平台
   - 節省約 **70%** 的 runtimes 空間，大幅減少安裝程式大小

5. **執行摘要**
   - 顯示複製的檔案數量和總大小
   - 提供下一步操作提示

#### 預期輸出

```text
來源目錄: Output\EasyBrailleEdit\Release\net10.0-windows10.0.17763.0
目標目錄: Setup\InnoSetup\Files\app
檔案總數: 105
目錄總數: 26
總大小: 43.21 MB
```

#### 關鍵檔案確認

執行腳本後，以下檔案狀態應符合預期：

| 檔案/目錄 | 狀態 | 說明 |
|----------|------|------|
| `EasyBrailleEdit.exe` | ✅ 應存在 | 主程式 |
| `Txt2Brl.exe` | ✅ 應存在 | 命令列工具 |
| `BrailleToolkit.dll` | ✅ 應存在 | 核心函式庫 |
| `*.pdb` | ❌ 應排除 | 除錯符號檔（0 個） |
| `runtimes/win-x64/` | ✅ 應存在 | Windows x64 平台 |
| `runtimes/linux-*/` | ❌ 應排除 | Linux 平台 |
| `runtimes/android-*/` | ❌ 應排除 | Android 平台 |

> **詳細說明**：參閱 [Setup/README.md](file:///d:/Projects/BrailleKit/text-to-braille/Setup/README.md)

### 步驟 4：製作安裝程式

1. **開啟 Inno Setup Compiler**

2. **載入腳本**
   - 開啟檔案：`Setup\InnoSetup\Setup.iss`

3. **編譯安裝程式**
   - 點選「Compile」或按 `Ctrl+F9`
   - 編譯完成後會產生 `setup.exe`

4. **安裝程式位置**

   ```text
   Setup\InnoSetup\setup.exe
   ```

### 步驟 5：測試安裝程式

在乾淨的測試環境中：

1. **執行安裝程式**

   ```powershell
   .\Setup\InnoSetup\setup.exe
   ```

2. **驗證安裝**
   - ✅ 安裝程式正常啟動
   - ✅ 檔案正確安裝到目標目錄
   - ✅ 開始功能表捷徑建立成功
   - ✅ 桌面捷徑建立成功（如有選擇）

3. **功能測試**
   - ✅ EasyBrailleEdit.exe 正常啟動
   - ✅ 基本功能正常運作（開啟檔案、轉換、儲存）
   - ✅ Txt2Brl.exe 命令列工具可執行
   - ✅ 點字字型正確安裝

4. **解除安裝測試**
   - ✅ 解除安裝程式正常運作
   - ✅ 檔案正確移除

### 步驟 6：發布

1. **上傳安裝程式**
   - 將 `setup.exe` 上傳到發布平台（GitHub Releases、網站等）

2. **更新文件**
   - 發布線上使用手冊（docs-user）
   - 更新下載頁面連結

3. **發布公告**
   - 撰寫發布公告
   - 包含主要新功能和修正說明

## 發布檢查清單

在正式發布前，請確認以下事項：

### 文件準備

- [ ] `ReleaseNote.txt` 已撰寫完成
- [ ] `LICENSE.md` 為最新版本
- [ ] `ChangeLog.md` 已更新
- [ ] 線上使用手冊（docs-user）已更新

### 建置與打包

- [ ] Release 建置成功，無錯誤
- [ ] 執行 `Prepare-Release.ps1` 成功
- [ ] 確認打包目錄包含所有必要檔案
- [ ] 確認沒有 `.pdb` 檔案被打包
- [ ] 確認 runtimes 只包含 Windows 平台

### 安裝程式

- [ ] Inno Setup 編譯成功
- [ ] 安裝程式檔案大小合理（約 40-50 MB）
- [ ] 在測試環境安裝成功
- [ ] 主程式正常啟動和運作
- [ ] 命令列工具正常運作
- [ ] 解除安裝正常運作

### 發布準備

- [ ] 版本號正確
- [ ] 發布公告已準備
- [ ] 下載連結已準備

## 常見問題

### Q: 為什麼不打包 PDF 使用手冊？

A: 自 v5.0.0 起，改為提供線上使用手冊（位於 `docs-user/`），可以持續更新且更易維護。

### Q: 為什麼 runtimes 目錄這麼大？

A: 原始建置輸出包含所有平台（Android、Linux、macOS 等）的 runtimes。`Prepare-Release.ps1` 腳本會自動過濾，只保留 Windows 平台的部分，大幅減少安裝程式大小。

### Q: 可以手動複製檔案而不用腳本嗎？

A: 可以，但不建議。手動複製容易遺漏檔案或包含不必要的檔案（如 `.pdb`）。使用腳本可確保一致性和正確性。

### Q: 如何驗證打包的檔案是否完整？

A: 執行腳本後，檢查以下關鍵檔案是否存在：

```powershell
# 在 Setup\InnoSetup\Files\x86 目錄下
Test-Path EasyBrailleEdit.exe  # 應為 True
Test-Path Txt2Brl.exe          # 應為 True
Test-Path BrailleToolkit.dll   # 應為 True

# 確認沒有 .pdb 檔案
Get-ChildItem -Filter *.pdb -Recurse  # 應無結果
```

## 技術說明

### 自動化腳本設計決策

採用 **PowerShell 獨立腳本** 方案，手動執行而非自動觸發：

| 特點 | 說明 |
|------|------|
| **執行方式** | 手動執行 `.\Setup\Prepare-Release.ps1` |
| **優點** | 完全掌控執行時機、易於除錯、不干擾正常開發流程 |
| **檔案篩選** | 排除 `.pdb` 除錯符號檔案 |
| **平台篩選** | 只保留 Windows 平台的 runtimes |
| **文件處理** | 移除使用手冊 PDF（改用線上文件），保留 LICENSE.md 和 ReleaseNote.txt |

### 最終目錄結構

打包完成後的目錄結構如下：

```text
Setup/
├── Prepare-Release.ps1        # 自動化打包腳本
├── README.md                   # 腳本使用說明
└── InnoSetup/
    ├── Setup.iss               # Inno Setup 腳本
    ├── setup.exe               # 編譯後的安裝程式
    ├── Files/
    │   ├── LICENSE.md          # 授權條款（手動維護）
    │   ├── ReleaseNote.txt     # 發行說明（手動維護）
    │   └── app/                # 應用程式檔案（腳本自動產生）
    │       ├── EasyBrailleEdit.exe
    │       ├── Txt2Brl.exe
    │       ├── *.dll
    │       ├── *.json
    │       ├── *.config
    │       ├── cs/             # 語言資源目錄
    │       ├── de/
    │       ├── ...             # 其他語言資源
    │       └── runtimes/       # Windows 平台原生函式庫
    │           ├── win/
    │           ├── win-x64/
    │           ├── win-x86/
    │           └── win-arm64/
    └── Fonts/
        └── simbrl.ttf          # 點字字型
```

## 參考資料

- [部署流程完整說明](file:///d:/Projects/BrailleKit/text-to-braille/docs-dev/technical/deployment/deployment-procedure.md)
- [Prepare-Release.ps1 使用說明](file:///d:/Projects/BrailleKit/text-to-braille/Setup/README.md)
- [建置輸出合併說明](file:///d:/Projects/BrailleKit/text-to-braille/docs-dev/technical/development/build-output-merge.md)
- [Inno Setup 官方文件](https://jrsoftware.org/isinfo.php)

## 版本歷史

| 日期 | 版本 | 更新內容 |
|------|------|----------|
| 2025-12-01 | 1.0 | 初版，新增自動化打包腳本說明 |
