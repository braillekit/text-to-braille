# EasyBrailleEdit.Tests

## 專案簡介

`EasyBrailleEdit.Tests` 是 **EasyBrailleEdit** 應用程式的整合測試專案，專門用於測試需要 Windows Forms 環境的 UI 功能。

## 目標框架

- **TargetFramework**: `net10.0-windows10.0.17763.0`
- **UseWindowsForms**: `true`

此專案需要 Windows 環境才能執行。

## 與 BrailleToolkit.Tests 的差異

| 特性 | BrailleToolkit.Tests | EasyBrailleEdit.Tests |
|------|---------------------|----------------------|
| 目標框架 | `net10.0` | `net10.0-windows10.0.17763.0` |
| 測試範圍 | 核心點字轉換邏輯 | Windows Forms UI 功能 |
| 執行環境 | 跨平台 | 僅限 Windows |
| 測試類型 | 單元測試 | 整合測試 |

## 測試類別

### DualEdit

#### ClipboardHelperTests
測試剪貼簿操作功能，確保：
- ✅ 點字資料的複製與貼上
- ✅ `PhoneticCode` 屬性不會在序列化過程中遺失
- ✅ 防止 `NotSupportedException` 回歸

**測試標記：**
- `[Trait("Category", "Integration")]`
- `[Trait("Category", "UI")]`
- `[Trait("Category", "Clipboard")]`

**測試方法：**
- `SetAndGetWords_WithValidData_ShouldRoundTripCorrectly()`
- `SetAndGetWords_WithPhoneticCode_ShouldPreservePhoneticCode()`
- `SetAndGetWords_WithEmptyList_ShouldRoundTripCorrectly()`
- `GetWords_WhenClipboardIsEmpty_ShouldReturnNull()`
- `SetAndGetLines_WithValidData_ShouldRoundTripCorrectly()`
- `SetAndGetLines_WithPhoneticCode_ShouldPreservePhoneticCode()`
- `GetLines_WhenClipboardIsEmpty_ShouldReturnNull()`
- `ClearData_WhenClipboardContainsWords_ShouldClearClipboard()`
- `ClearData_WhenClipboardContainsLines_ShouldClearClipboard()`
- `SetWords_ShouldNotThrowNotSupportedException()`
- `SetLines_ShouldNotThrowNotSupportedException()`

## 執行測試

### 使用 Visual Studio
1. 開啟 Test Explorer
2. 執行所有測試或選擇特定測試

### 使用命令列
```powershell
# 執行所有測試
dotnet test EasyBrailleEdit.Tests/EasyBrailleEdit.Tests.csproj

# 只執行剪貼簿測試
dotnet test EasyBrailleEdit.Tests/EasyBrailleEdit.Tests.csproj --filter Category=Clipboard

# 執行時顯示詳細資訊
dotnet test EasyBrailleEdit.Tests/EasyBrailleEdit.Tests.csproj --logger "console;verbosity=detailed"
```

### 在 CI/CD 中執行

這些測試需要 Windows 環境和 UI 存取權限。在 CI/CD 管道中：

```yaml
# 僅在 Windows Agent 上執行
- task: DotNetCoreCLI@2
  displayName: 'Run UI Tests'
  condition: eq(variables['Agent.OS'], 'Windows_NT')
  inputs:
    command: 'test'
    projects: '**/EasyBrailleEdit.Tests.csproj'
```

## 測試原則

### STA 執行緒
所有測試方法都使用 `[STAFact]` 屬性，因為 Windows Forms API (包括剪貼簿) 需要 STA (Single-Threaded Apartment) 執行緒。

### 測試隔離
每個測試在執行前都會清除剪貼簿，確保測試之間不會互相干擾。

### 防禦性程式設計
測試會捕捉並忽略某些環境下無法存取剪貼簿的錯誤，例如在無頭 (headless) 環境中。

## 未來擴充

此專案可以用來測試其他需要 Windows Forms 環境的功能：
- 表單行為測試
- 控制項互動測試
- 檔案對話框測試
- 列印功能測試

## 依賴套件

- **xUnit** - 測試框架
- **Microsoft.NET.Test.Sdk** - 測試 SDK

## 相關專案

- [`EasyBrailleEdit`](../EasyBrailleEdit/) - 主應用程式
- [`BrailleToolkit`](../BrailleToolkit/) - 核心點字轉換邏輯
- [`BrailleToolkit.Tests`](../BrailleToolkit.Tests/) - 核心邏輯單元測試
