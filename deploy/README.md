# 發布打包說明

## 概要

`Prepare-Release.ps1` 是用於準備 EasyBrailleEdit 應用程式發布的自動化腳本。它會將 Release 建置輸出複製到 Inno Setup 安裝程式製作目錄，並自動進行檔案篩選和平台過濾。

## 使用方式

### 前置條件

1. 已完成 Release 建置：

   ```powershell
   dotnet build .\src\EasyBrailleEditApp\EasyBrailleEdit\EasyBrailleEdit.csproj -c Release
   ```

2. 確認建置輸出存在於：

   ```
   output\EasyBrailleEdit\Release\net10.0-windows10.0.17763.0\
   ```

### 執行腳本

從專案根目錄執行：

```powershell
.\deploy\Prepare-Release.ps1
```

若需查看詳細執行過程，可加上 `-Verbose` 參數：

```powershell
.\deploy\Prepare-Release.ps1 -Verbose
```

### 執行結果

腳本會將檔案複製到：

```text
deploy\InnoSetup\Files\app\
```

## 自動處理功能

### 檔案篩選

- ✅ **複製**：所有 `.exe`、`.dll`、`.json`、`.config`、`.ini`、`.txt`、`.md` 等檔案
- ❌ **排除**：`.pdb` 除錯符號檔案

### 平台篩選（runtimes 目錄）

- ✅ **保留**：`win`、`win-x64`、`win-x86`、`win-arm64`（Windows 平台）
- ❌ **排除**：`android-*`、`linux-*`、`osx-*`、`maccatalyst-*`、`unix`（非 Windows 平台）

### 手動維護檔案保留

腳本會保留以下手動維護的檔案（不會被刪除）：

- `LICENSE.md`
- `ReleaseNote.txt`

## 發布流程

完整的發布流程建議如下：

1. **更新版本號和文件**
   - 更新 `ChangeLog.md`
   - 準備 `deploy/InnoSetup/Files/ReleaseNote.txt`（發行說明）
   - 確認 `deploy/InnoSetup/Files/LICENSE.md` 為最新版本

2. **執行 Release 建置**

   ```powershell
   dotnet build .\src\EasyBrailleEditApp\EasyBrailleEdit\EasyBrailleEdit.csproj -c Release
   ```

3. **執行打包腳本**

   ```powershell
   .\deploy\Prepare-Release.ps1
   ```

4. **製作安裝程式**
   - 使用 Inno Setup 開啟 `deploy\InnoSetup\Setup.iss`
   - 編譯安裝程式
   - 產生的 `setup.exe` 會在 `deploy\InnoSetup\` 目錄

5. **測試安裝程式**
   - 在乾淨的測試環境安裝
   - 驗證應用程式功能

6. **發布**
   - 上傳安裝程式到發布平台
   - 更新線上使用手冊（docs-user）
   - 發布更新公告

## 疑難排解

### 找不到建置輸出目錄

**錯誤訊息**：

```text
找不到建置輸出目錄: output\EasyBrailleEdit\Release\net10.0-windows10.0.17763.0
請先執行 Release 建置。
```

**解決方法**：
執行 Release 建置：

```powershell
dotnet build .\src\EasyBrailleEditApp\EasyBrailleEdit\EasyBrailleEdit.csproj -c Release
```

### 找不到必要檔案

**錯誤訊息**：

```text
找不到必要檔案: EasyBrailleEdit.exe
請確認 Release 建置是否成功。
```

**解決方法**：
檢查建置是否成功，確認沒有編譯錯誤。

## 腳本輸出範例

成功執行時會顯示：

```text
========================================
EasyBrailleEdit 發布檔案準備工具
========================================

[檢查前置條件]
✓ 前置條件檢查通過

[清理目標目錄]
✓ 目標目錄已清理

[複製建置輸出]
✓ 已複製 64 個檔案（跳過 4 個）

[複製子目錄]

[複製 runtimes 目錄（僅 Windows 平台）]
✓ 已複製 4 個 Windows 平台（跳過 19 個其他平台）

[完成摘要]

來源目錄: output\EasyBrailleEdit\Release\net10.0-windows10.0.17763.0
目標目錄: deploy\InnoSetup\Files\app
檔案總數: 105
目錄總數: 26
總大小: 43.21 MB

✓ 發布檔案準備完成！
  下一步：使用 Inno Setup 開啟 Setup.iss 並編譯安裝程式。
```
