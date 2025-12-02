# EasyBrailleEdit 部署流程

本文件說明 EasyBrailleEdit 應用程式的完整部署流程，從版本準備到發布的所有步驟。

## 部署步驟

### 步驟 1：準備版本資訊

確保以下事項已完成：

- [ ] `ChangeLog.md` 已編寫完成
- [ ] 所有變更已 commit
- [ ] 變更已合併至 `main` branch

### 步驟 2：建立 Git Tag

在 `main` branch 建立版本 tag 並推送至 Git server。

**指令範例**：

```bash
git tag 4.1.0
git push origin 4.1.0
```

> [!IMPORTANT]
> **Tag 命名規則**：Tag 名稱只能是版本號碼（例如 `4.1.0`）。
> 
> **不可使用** `v` 前綴（例如 `v4.1.0`），否則會導致建置系統忽略該 tag。

### 步驟 3：執行 Release 建置

建置 Release 版本：

```powershell
dotnet build .\src\EasyBrailleEditApp\EasyBrailleEdit\EasyBrailleEdit.csproj -c Release
```

**建置輸出位置**：

```text
output\EasyBrailleEdit\Release\<.NET-target>\
```

**驗證步驟**：

1. 檢查主程式執行檔和 DLL 檔案的版本是否與步驟 2 的 tag 一致
2. 確認 `Txt2Brl.exe` 及其依賴項已自動複製到輸出目錄

> [!NOTE]
> `AppConfig.ini` 會在程式首次執行時自動產生，部署時不需包含此檔案，除非需要預設特定設定。

### 步驟 4：準備打包檔案

使用自動化腳本準備發布檔案：

```powershell
.\deploy\Prepare-Release.ps1
```

此腳本會：

- 複製所有必要檔案到 `deploy/InnoSetup/Files/app/`
- 自動排除除錯符號檔案（`.pdb`）
- 只保留 Windows 平台的 runtimes
- 保留手動維護的文件（LICENSE.md、ReleaseNote.txt）

> **詳細說明**：參閱 [release-packaging.md](release-packaging.md)

### 步驟 5：製作安裝程式（選擇性）

如果需要打包安裝程式：

1. 使用 Inno Setup Compiler 開啟 `deploy\InnoSetup\Setup.iss`
2. 編譯安裝程式（`Ctrl+F9`）
3. 產生的 `setup.exe` 會在 `deploy\InnoSetup\` 目錄

### 步驟 6：部署自動更新檔案

將應用程式檔案手動複製到 GitHub 更新專案：

**目標位置**：[text-to-braille-updates](https://github.com/braillekit/text-to-braille-updates/tree/main/Files)

**檔案選擇**：

- 從步驟 3 的建置輸出中選擇必要的執行檔和 DLL
- 不包含 `.pdb` 除錯符號檔案
- 不包含 `AppConfig.ini`（除非需要預設設定）

### 步驟 7：更新版本資訊檔案

修改更新專案中的 `_update.txt` 檔案：

1. 確認版本號與步驟 2 的 tag 一致
2. 更新檔案清單
3. 設定更新說明

### 步驟 8：本機測試自動更新

在本機執行應用程式並測試自動更新功能：

1. 執行舊版本的應用程式
2. 觸發自動更新檢查
3. 確認能正確下載和安裝新版本
4. 驗證更新後的應用程式功能正常

### 步驟 9：正式發布

確認所有測試通過後：

1. **提交更新檔案**：將 `_update.txt` 和應用程式檔案提交到更新專案
2. **發布安裝程式**（如有）：
   - 上傳 `setup.exe` 到 GitHub Releases
   - 或發布到其他下載平台
3. **更新文件**：
   - 發布線上使用手冊
   - 更新下載頁面連結
4. **發布公告**：
   - 撰寫發布公告
   - 包含主要新功能和修正說明

## 部署檢查清單

在正式發布前，確認以下事項：

### 版本準備

- [ ] ChangeLog.md 已更新
- [ ] Git tag 已建立並推送
- [ ] Tag 命名符合規則（無 `v` 前綴）

### 建置驗證

- [ ] Release 建置成功
- [ ] 執行檔版本號正確
- [ ] Txt2Brl.exe 已包含

### 檔案準備

- [ ] 執行 Prepare-Release.ps1 成功
- [ ] 確認打包目錄包含所有必要檔案
- [ ] 確認沒有 .pdb 檔案被打包

### 安裝程式（如有）

- [ ] Inno Setup 編譯成功
- [ ] 安裝程式測試通過

### 自動更新

- [ ] _update.txt 版本號正確
- [ ] 更新檔案已上傳
- [ ] 本機測試自動更新成功

### 發布確認

- [ ] 線上使用手冊已更新
- [ ] 下載連結正確
- [ ] 發布公告已準備

## 常見問題

### Q: 為什麼 tag 不能使用 `v` 前綴？

A: 建置系統（Flubu 或 MinVer）會忽略帶有 `v` 前綴的 tag，導致版本號無法正確識別。

### Q: 如何確認版本號是否正確？

A: 在建置輸出目錄中，右鍵點擊 `EasyBrailleEdit.exe`，選擇「內容」→「詳細資料」，檢查「產品版本」是否與 tag 一致。

### Q: 自動更新測試失敗怎麼辦？

A: 常見原因：

- `_update.txt` 版本號格式不正確
- 檔案路徑或 URL 錯誤
- 檔案權限問題
- 防火牆或防毒軟體阻擋

## 相關文件

- [發布打包詳細說明](release-packaging.md)
- [自動更新機制說明](../development/auto-updater.md)
- [Prepare-Release.ps1 使用說明](../../../Setup/README.md)

## 版本歷史

| 日期 | 版本 | 更新內容 |
|------|------|----------|
| 2025-12-01 | 2.0 | 轉換為 Markdown 格式，整合自動化打包流程 |
| - | 1.0 | 初版（deployment.txt） |
