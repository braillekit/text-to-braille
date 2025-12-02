# EasyBrailleEdit Auto-Update Mechanism

## 概要

EasyBrailleEdit 的自動更新功能是由 `EasyBrailleEdit.Common.Utilities.Http.HttpUpdater` 類別提供，支援基於 HTTP 的檔案更新，並具備失敗回復（rollback）機制。

## 更新檔案位置

目前使用 GitHub 作為更新檔案的託管平台：

**GitHub Repository:**

```url
https://github.com/huanlin/EasyBrailleEdit/tree/master/UpdateFiles
```

**原始檔案 URL（用於 HTTP 下載）:**

```url
https://raw.githubusercontent.com/huanlin/EasyBrailleEdit/master/UpdateFiles/
```

> **注意**: 如需變更檔案位置，只需修改 `AppConfig.ini` 中的 `[Internet]` 區段的 `AppUpdateFilesUrl` 參數（舊版為 `AppUpdateFilesUri`）。

### 範例配置

```ini
[Internet]
AppServerName=
AppUpdateFilesUrl=https://raw.githubusercontent.com/huanlin/EasyBrailleEdit/master/UpdateFiles/
```

> `AppServerName` 已廢棄，但部分程式碼可能仍保留參考。

## 必要檔案

伺服器端至少須包含以下兩個檔案，自動更新機制才能正常運作：

1. **Update.txt** - 更新清單檔案
2. **ChangeLog.txt** - 版本變更記錄

## 工作流程

### 1. 啟動時自動檢查

應用程式啟動時（`MainForm_Load`），會自動執行 `AutoUpdateAsync()` 方法：

```csharp
private async Task<bool> AutoUpdateAsync()
{
    if (!AppGlobals.Config.General.AutoUpdate)  // 檢查是否啟用自動更新
    {
        return false;
    }

    if (SysInfo.IsNetworkConnected())  // 檢查網路連線
    {
        return await DoUpdateAsync(true);
    }
    return false;
}
```

### 2. 更新程序

#### 2.1 初始化 HttpUpdater

```csharp
var updater = new HttpUpdater()
{
    ClientPath = Application.StartupPath,
    ServerUri = AppGlobals.Config.General.AutoUpdateFilesUrl,
    ChangeLogFileName = Constant.ChangeLogFileName
};
```

#### 2.2 獲取更新清單

```csharp
await updater.GetUpdateListAsync(Constant.DefaultAutoUpdateFileListName);
```

`GetUpdateListAsync` 方法會：

- 從伺服器下載 `Update.txt`
- 解析檔案清單並去除註解（以 `'` 開頭）
- 比對本地與遠端檔案版本
- 建立需要更新的檔案清單

#### 2.3 執行更新

如果有可用更新（`updater.HasUpdates()` 返回 `true`），則：

1. 顯示更新進度視窗 （`UpdateProgressForm`）
2. 註冊事件處理器：
   - `FileUpdating` - 檔案開始更新時觸發
   - `FileUpdated` - 檔案更新完成時觸發
   - `DownloadProgressChanged` - 下載進度變更時觸發

3. 執行更新：

   ```csharp
   await updater.UpdateAsync()
   ```

4. 更新完成後，重新啟動應用程式

### 3. 更新機制詳細流程

對於每個需要更新的檔案，`HttpUpdater.UpdateAsync()` 會：

1. **下載檔案** - 下載至暫存檔名（例如：`filename.ext.{ticks}.UpdTmp`）
2. **備份舊檔** - 將現有檔案重新命名為待刪除檔名（例如：`filename.ext.{ticks}.UpdTmp.ToDelete`）
3. **替換檔案** - 將暫存檔重新命名為目標檔案名稱
4. **記錄 Rollback 資訊** - 以便失敗時能夠回復

> **關鍵設計**: 使用 `File.Move` 而非 `File.Delete` 的原因是執行中的檔案可以被重新命名，但無法被刪除。這使得應用程式可以更新自己的執行檔。

### 4. Rollback 機制

如果任何檔案下載失敗，系統會：

1. 刪除已下載的新檔案
2. 將備份檔案還原回原始檔名
3. 拋出例外，通知使用者更新失敗

### 5. 清理暫存檔

下次執行更新時，`CleanUp()` 方法會刪除：

- 所有 `*.ToDelete` 檔案（舊版檔案）
- 所有 `*.UpdTmp` 檔案（未完成的暫存檔）

## Update.txt 檔案格式

### 基本格式

```text
檔案名稱       ; 版本號碼或特定參數
```

- 分號 `;` 前為檔案名稱（可包含路徑）
- 分號後為版本號碼或特定參數（見下方說明）
- 單引號 `'` 後為註解（會被忽略）

### 參數說明

| 參數 | 說明 | 範例 |
|------|------|------|
| `版本號碼` | 若遠端版本 > 本地版本則更新 | `EasyBrailleEdit.exe ; 5.0.1` |
| `?` | 僅當本地無此檔案時才更新 | `AppConfig.Default.ini ; ?` |
| `=版本號碼` | 遠端與本地版本不同就更新 | `plugin.dll ; =2.3.0` |
| `delete` | 刪除本地檔案 | `obsolete.dll ; delete` |

### 範例

```text
EasyBrailleEdit.exe       ; 5.0.1   '主程式
Txt2Brl.exe               ; 5.0.1   '命令列工具
BrailleToolkit.dll        ; 5.0.0   '點字工具函式庫
EasyBrailleEdit.Common.dll; 5.0.0   '共用函式庫
AppConfig.Default.ini     ; ?       '僅當本地無此檔案時更新
Phrase.phf                ; ?       '僅當本地無此檔案時更新
obsolete.dll              ; delete  '刪除已廢棄的檔案
```

## 版本比對邏輯

`HttpUpdater.GetUpdateListAsync()` 使用 `System.Version` 類別進行版本比對：

```csharp
FileVersionInfo fileVerInfo = FileVersionInfo.GetVersionInfo(clientFileName);
Version verRemote = new Version(infoParam);
Version verLocal = new Version(fileVerInfo.FileVersion!.Split(' ')[0]);

if (verRemote > verLocal)
{
    item.Operation = UpdateAction.Overwrite;
}
```

> **注意**: 本地檔案若無版本資訊，系統會自動將其標記為需要更新。

## 使用方式

### 程式碼範例

在 `MainForm.cs` 的 `DoUpdateAsync` 方法中可見完整使用範例：

```csharp
private async Task<bool> DoUpdateAsync(bool autoMode)
{
    var updater = new HttpUpdater()
    {
        ClientPath = Application.StartupPath,
        ServerUri = AppGlobals.Config.General.AutoUpdateFilesUrl,
        ChangeLogFileName = Constant.ChangeLogFileName
    };

    await updater.GetUpdateListAsync(Constant.DefaultAutoUpdateFileListName);

    if (updater.HasUpdates())
    {
        // 詢問使用者是否更新
        // 顯示進度視窗
        // 註冊事件
        // 執行更新
        await updater.UpdateAsync();
        // 重新啟動應用程式
    }
}
```

### 手動觸發更新

使用者可透過選單「說明」→「檢查更新」手動觸發 `CheckUpdateAsync()` 方法。

## 技術細節

### 主要類別

- **HttpUpdater** (`EasyBrailleEdit.Common.Utilities.Http.HttpUpdater`)
  - 核心更新邏輯
  - 版本比對
  - 檔案下載與替換
  - Rollback 機制

- **UpdateProgressForm** (`EasyBrailleEdit.UpdateProgressForm`)
  - 顯示更新進度
  - 訂閱 HttpUpdater 的事件

- **MainForm.DoUpdateAsync** (`EasyBrailleEdit.MainForm`)
  - 協調更新流程
  - 處理使用者互動

### 事件

| 事件 | 用途 |
|------|------|
| `FileUpdating` | 檔案開始下載時觸發 |
| `FileUpdated` | 檔案下載完成時觸發 |
| `DownloadProgressChanged` | 下載進度變更時觸發（顯示進度條） |

### 檔案命名慣例

| 副檔名 | 用途 |
|--------|------|
| `.UpdTmp` | 下載中的暫存檔案 |
| `.ToDelete` | 待下次啟動時刪除的舊檔案 |

## 配置選項

在 `AppConfig.ini` 中可設定：

```ini
[Internet]
AppUpdateFilesUrl=https://raw.githubusercontent.com/huanlin/easy-braille-edit/master/UpdateFiles/

[General]
AutoUpdate=true  ; 是否啟用自動檢查更新（啟動時）
```

## 注意事項

1. **網路連線必要性** - 自動更新需要網際網路連線
2. **檔案版本資訊** - 所有 DLL 和 EXE 檔案應包含正確的 `AssemblyVersion` 和 `FileVersion`
3. **循環更新** - 若更新失敗並進行 rollback，暫存檔會在下次更新時清理
4. **執行中的檔案** - 系統可以更新正在執行的主程式（透過重新命名機制）
5. **重新啟動** - 更新完成後，應用程式會提示使用者重新啟動

## 除錯

若需要測試本地更新來源，可在 `DoUpdateAsync` 中暫時修改：

```csharp
// debug using local update feed.
updater.ServerUri = "https://raw.githubusercontent.com/huanlin/easy-braille-edit/test-auto-update-subfolder/UpdateFiles/";
```

## 相關檔案

- `src\EasyBrailleEditApp\EasyBrailleEdit.Common\Utilities\Http\HttpUpdater.cs` - 核心實作
- `src\EasyBrailleEditApp\EasyBrailleEdit\MainForm.cs` - `DoUpdateAsync`, `AutoUpdateAsync`, `CheckUpdateAsync`
- `src\EasyBrailleEditApp\EasyBrailleEdit\UpdateProgressForm.cs` - 進度視窗
- `UpdateFiles\Update.txt` - 伺服器端更新清單（GitHub）
- `UpdateFiles\ChangeLog.txt` - 版本變更記錄（GitHub）
