# 雙模式點字轉換架構實施計畫

## 策略變更說明

**原計畫：** 完全移除 Txt2Brl 專案，整合到主程式  
**新計畫：** 保留 Txt2Brl，新增內建轉換服務，提供兩種模式供選擇

---

## 背景與動機

### 為何採用雙模式架構

✅ **安全性更高**
- 內建模式有問題時可立即切換回外部工具
- 降低部署風險
- 給使用者安全的退路

✅ **保留現有價值**
- Txt2Brl 仍可作為獨立命令列工具使用
- 在腳本或批次處理中仍然有用
- 不會失去已驗證的穩定功能

✅ **漸進式遷移**
- 可以先預設使用內建功能
- 長期觀察記憶體表現
- 根據實際使用情況再決定未來方向

✅ **效能比較**
- 可以實際比較兩種方式的效能
- 幫助做出數據驅動的決策

---

## 使用者審核關鍵點

> [!IMPORTANT]
> **架構變更：** 新增雙模式轉換架構，而非直接移除 Txt2Brl。

> [!NOTE]
> **預設行為：** 預設使用內建轉換（`UseInProcessConversion = true`），可透過組態檔切換。

---

## 提議的修改

### 階段 1：定義轉換介面與結果類別

#### 新增檔案

##### [NEW] [IBrailleConverter.cs](file:///d:/Projects/BrailleKit/text-to-braille/Source/EasyBrailleEditApp/EasyBrailleEdit/Services/IBrailleConverter.cs)

```csharp
namespace EasyBrailleEdit.Services
{
    /// <summary>
    /// 點字轉換介面，支援不同的轉換實作
    /// </summary>
    public interface IBrailleConverter
    {
        /// <summary>
        /// 執行點字轉換
        /// </summary>
        /// <param name="content">要轉換的文字內容</param>
        /// <param name="cellsPerLine">每列最大方數</param>
        /// <param name="phraseFiles">使用者自訂詞庫檔案</param>
        /// <param name="progress">進度回報</param>
        /// <returns>轉換結果</returns>
        Task<BrailleConversionResult> ConvertAsync(
            string content, 
            int cellsPerLine,
            string[] phraseFiles,
            IProgress<ConversionProgress>? progress = null);
    }
}
```

##### [NEW] [BrailleConversionResult.cs](file:///d:/Projects/BrailleKit/text-to-braille/Source/EasyBrailleEditApp/EasyBrailleEdit/Services/BrailleConversionResult.cs)

```csharp
namespace EasyBrailleEdit.Services
{
    /// <summary>
    /// 點字轉換結果
    /// </summary>
    public class BrailleConversionResult
    {
        /// <summary>
        /// 是否成功（無錯誤）
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// 輸出的點字檔案路徑
        /// </summary>
        public string? OutputFilePath { get; set; }
        
        /// <summary>
        /// 是否有錯誤
        /// </summary>
        public bool HasError => !Success || InvalidChars.Count > 0;
        
        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;
        
        /// <summary>
        /// 無法轉換的字元清單
        /// </summary>
        public List<CharPosition> InvalidChars { get; set; } = new();
    }
    
    /// <summary>
    /// 轉換進度資訊
    /// </summary>
    public class ConversionProgress
    {
        public int CurrentLine { get; set; }
        public string CurrentText { get; set; } = string.Empty;
        public int PercentComplete { get; set; }
    }
}
```

---

### 階段 2：實作內建轉換服務

#### 新增檔案

##### [NEW] [InProcessBrailleConverter.cs](file:///d:/Projects/BrailleKit/text-to-braille/Source/EasyBrailleEditApp/EasyBrailleEdit/Services/InProcessBrailleConverter.cs)

**用途：** 在主程式內直接執行轉換，不啟動外部程序

```csharp
namespace EasyBrailleEdit.Services
{
    /// <summary>
    /// 內建點字轉換服務（在主程式內執行）
    /// </summary>
    public class InProcessBrailleConverter : IBrailleConverter, IDisposable
    {
        private BrailleDocument? _doc;
        private BrailleProcessor? _processor;
        private ZhuyinReverseConverter? _zhuyinConverter;
        
        public async Task<BrailleConversionResult> ConvertAsync(
            string content, 
            int cellsPerLine,
            string[] phraseFiles,
            IProgress<ConversionProgress>? progress = null)
        {
            return await Task.Run(() => ConvertInternal(content, cellsPerLine, phraseFiles, progress));
        }
        
        private BrailleConversionResult ConvertInternal(
            string content, 
            int cellsPerLine,
            string[] phraseFiles,
            IProgress<ConversionProgress>? progress)
        {
            try
            {
                // 初始化轉換器
                var provider = new ZhuyinReverseConversionProvider();
                _zhuyinConverter = new ZhuyinReverseConverter(provider);
                _processor = BrailleProcessor.CreateInstance(_zhuyinConverter);
                _doc = new BrailleDocument(_processor);
                
                // 載入自訂詞庫
                LoadPhraseFiles(phraseFiles);
                
                // 設定進度回報
                if (progress != null)
                {
                    _processor.TextConverted += (s, e) => 
                    {
                        progress.Report(new ConversionProgress 
                        { 
                            CurrentLine = e.LineNumber,
                            CurrentText = e.Text 
                        });
                    };
                }
                
                // 執行轉換
                _doc.CellsPerLine = cellsPerLine;
                _doc.Convert(content);
                
                // 儲存到臨時檔案
                string outFileName = Path.Combine(
                    AppGlobals.TempPath, 
                    Constant.Files.CvtOutputTempFileName);
                    
                if (!_processor.HasError)
                {
                    _doc.SaveBrailleFile(outFileName);
                }
                
                // 建立結果
                var result = new BrailleConversionResult
                {
                    Success = !_processor.HasError,
                    OutputFilePath = outFileName,
                    ErrorMessage = _processor.ErrorMessage,
                    InvalidChars = new List<CharPosition>(_processor.InvalidChars)
                };
                
                return result;
            }
            finally
            {
                // 確保資源釋放
                Cleanup();
            }
        }
        
        private void LoadPhraseFiles(string[] phraseFiles)
        {
            var phtbl = ZhuyinPhraseTable.GetInstance();
            foreach (string fname in phraseFiles)
            {
                if (!string.IsNullOrEmpty(fname) && File.Exists(fname))
                {
                    phtbl.Load(fname);
                }
            }
        }
        
        private void Cleanup()
        {
            _doc?.Clear();
            _doc = null;
            _processor = null;
            _zhuyinConverter = null;
        }
        
        public void Dispose()
        {
            Cleanup();
        }
    }
}
```

---

### 階段 3：封裝現有外部工具呼叫

#### 新增檔案

##### [NEW] [ExternalBrailleConverter.cs](file:///d:/Projects/BrailleKit/text-to-braille/Source/EasyBrailleEditApp/EasyBrailleEdit/Services/ExternalBrailleConverter.cs)

**用途：** 封裝現有的 Txt2Brl.exe 呼叫邏輯

```csharp
namespace EasyBrailleEdit.Services
{
    /// <summary>
    /// 外部工具點字轉換服務（呼叫 Txt2Brl.exe）
    /// </summary>
    public class ExternalBrailleConverter : IBrailleConverter
    {
        private readonly FileRunner _fileRunner = new();
        
        public async Task<BrailleConversionResult> ConvertAsync(
            string content, 
            int cellsPerLine,
            string[] phraseFiles,
            IProgress<ConversionProgress>? progress = null)
        {
            // 準備輸入檔案
            string inFileName = Path.Combine(AppGlobals.TempPath, Constant.Files.CvtInputTempFileName);
            string outFileName = Path.Combine(AppGlobals.TempPath, Constant.Files.CvtOutputTempFileName);
            string phraseListFile = Path.Combine(AppGlobals.TempPath, Constant.Files.CvtInputPhraseListFileName);
            
            await File.WriteAllTextAsync(inFileName, content, Encoding.UTF8);
            await File.WriteAllLinesAsync(phraseListFile, phraseFiles, Encoding.UTF8);
            
            // 呼叫 Txt2Brl.exe
            await InvokeTxt2BrlAsync(inFileName, outFileName, cellsPerLine);
            
            // 讀取結果
            return ReadConversionResult(outFileName);
        }
        
        private async Task InvokeTxt2BrlAsync(string inFileName, string outFileName, int cellsPerLine)
        {
            StringBuilder arg = new StringBuilder();
            arg.Append($" -i \"{inFileName}\" -o \"{outFileName}\" ");
            arg.Append($"-c{cellsPerLine} ");
            
            _fileRunner.NeedWait = true;
            _fileRunner.ShowWindow = false;
            _fileRunner.UseShellExecute = false;
            _fileRunner.RedirectStandardOutput = true;
            
            string cmd = Path.Combine(Application.StartupPath, "txt2brl.exe");
            int exitCode = await _fileRunner.RunAsync(cmd, arg.ToString());
            
            if (exitCode != 0)
            {
                throw new Exception($"轉點字過程發生錯誤 (Exit Code: {exitCode})!");
            }
        }
        
        private BrailleConversionResult ReadConversionResult(string outFileName)
        {
            var result = new BrailleConversionResult
            {
                OutputFilePath = outFileName
            };
            
            // 讀取錯誤資訊（從臨時檔案）
            string resultFile = Path.Combine(AppGlobals.TempPath, Constant.Files.CvtResultFileName);
            string errorCharFile = Path.Combine(AppGlobals.TempPath, Constant.Files.CvtErrorCharFileName);
            
            if (File.Exists(resultFile))
            {
                var lines = File.ReadAllLines(resultFile);
                if (lines.Length > 0 && lines[0] == "1")
                {
                    result.Success = false;
                    result.ErrorMessage = lines.Length > 1 ? lines[1] : "";
                }
                else
                {
                    result.Success = true;
                }
            }
            
            if (File.Exists(errorCharFile))
            {
                result.InvalidChars = ReadInvalidChars(errorCharFile);
            }
            
            return result;
        }
        
        private List<CharPosition> ReadInvalidChars(string fileName)
        {
            var invalidChars = new List<CharPosition>();
            var lines = File.ReadAllLines(fileName);
            
            foreach (var line in lines)
            {
                var parts = line.Split(' ');
                if (parts.Length == 3)
                {
                    invalidChars.Add(new CharPosition
                    {
                        LineNumber = int.Parse(parts[0]),
                        CharIndex = int.Parse(parts[1]),
                        CharValue = parts[2][0]
                    });
                }
            }
            
            return invalidChars;
        }
    }
}
```

---

### 階段 4：新增組態選項

#### 修改檔案

##### [MODIFY] [AppConfig.cs](file:///d:/Projects/BrailleKit/text-to-braille/Source/EasyBrailleEditApp/EasyBrailleEdit.Common/AppConfig.cs)

在 `BrailleConfig` 類別中新增屬性：

```csharp
public class BrailleConfig
{
    // 現有屬性
    public int CellsPerLine { get; set; } = 40;
    
    // 新增：是否使用內建轉換（預設 true）
    public bool UseInProcessConversion { get; set; } = true;
}
```

---

### 階段 5：建立轉換器工廠

#### 新增檔案

##### [NEW] [BrailleConverterFactory.cs](file:///d:/Projects/BrailleKit/text-to-braille/Source/EasyBrailleEditApp/EasyBrailleEdit/Services/BrailleConverterFactory.cs)

```csharp
namespace EasyBrailleEdit.Services
{
    /// <summary>
    /// 點字轉換器工廠，根據組態建立適當的轉換器
    /// </summary>
    public static class BrailleConverterFactory
    {
        public static IBrailleConverter CreateConverter()
        {
            if (AppGlobals.Config.Braille.UseInProcessConversion)
            {
                return new InProcessBrailleConverter();
            }
            else
            {
                return new ExternalBrailleConverter();
            }
        }
    }
}
```

---

### 階段 6：修改 MainForm 使用工廠

#### 修改檔案

##### [MODIFY] [MainForm.cs](file:///d:/Projects/BrailleKit/text-to-braille/Source/EasyBrailleEditApp/EasyBrailleEdit/MainForm.cs)

**修改 `DoConvertAsync` 方法**（第 369-391 行）：

```csharp
// 修改前
private async Task<string> DoConvertAsync(string content)
{
    // ... 建立臨時檔案
    await InvokeTxt2BrlAsync(...);
    return outFileName;
}

// 修改後
private async Task<BrailleConversionResult> DoConvertAsync(string content)
{
    using var converter = BrailleConverterFactory.CreateConverter();
    
    string[] phraseFiles = m_ConvertDialog.SelectedPhraseFileNames;
    
    var progress = new Progress<ConversionProgress>(p => 
    {
        // 可以在這裡更新 UI 進度（未來擴充）
    });
    
    return await converter.ConvertAsync(
        content, 
        AppGlobals.Config.Braille.CellsPerLine,
        phraseFiles,
        progress);
}
```

**修改 `HandleConvertionError` 方法**（第 556-664 行）：

```csharp
// 修改前
private bool HandleConvertionError()
{
    // 從檔案讀取錯誤...
    bool hasError = GetCvtErrors(ref errMsg, ref invalidChars);
    // ...
}

// 修改後
private bool HandleConversionResult(BrailleConversionResult result)
{
    if (result.HasError)
    {
        if (result.InvalidChars.Count > 0)
        {
            foreach (var charPos in result.InvalidChars.Take(100))
            {
                m_InvalidCharForm.Add(charPos);
            }
            ShowInvlaidCharForm(result.InvalidChars.Count);
        }
        else if (!string.IsNullOrEmpty(result.ErrorMessage))
        {
            txtErrors.Text = result.ErrorMessage;
            txtErrors.Visible = true;
            MsgBoxHelper.ShowError("轉換過程中發生錯誤!");
        }
        return false;
    }
    return true;
}
```

**移除方法**：
- `InvokeTxt2BrlAsync` - 邏輯已封裝在 `ExternalBrailleConverter`
- `GetCvtErrors` - 不再需要，結果直接從轉換器取得

**更新呼叫點**（第 406-478 行等）：

```csharp
// ConvertAndShowEditor 方法
private async void ConvertAndShowEditor()
{
    // ... 前置準備 ...
    
    // 執行轉換
    var result = await DoConvertAsync(content);
    
    Enabled = true;
    
    if (!HandleConversionResult(result))
    {
        return;
    }
    
    OpenBrailleFileInEditor(result.OutputFilePath!);
}
```

---

## 驗證計畫

### 測試策略

#### 1. 單元測試

##### 測試檔案：`EasyBrailleEdit.Tests/Services/InProcessBrailleConverterTests.cs`

```csharp
[StaFact]
public async Task ConvertAsync_WithValidText_ShouldSucceed()
{
    // Arrange
    using var converter = new InProcessBrailleConverter();
    
    // Act
    var result = await converter.ConvertAsync("測試文字", 40, Array.Empty<string>());
    
    // Assert
    Assert.True(result.Success);
    Assert.NotNull(result.OutputFilePath);
    Assert.True(File.Exists(result.OutputFilePath));
}

[StaFact]
public async Task ConvertAsync_AfterDispose_ShouldNotLeakMemory()
{
    // 記憶體洩漏測試
}
```

#### 2. 整合測試

**測試雙模式切換：**

```csharp
[Theory]
[InlineData(true)]  // 內建模式
[InlineData(false)] // 外部工具模式
public async Task ConvertAsync_BothModes_ShouldProduceSameResult(bool useInProcess)
{
    // Arrange
    AppGlobals.Config.Braille.UseInProcessConversion = useInProcess;
    var converter = BrailleConverterFactory.CreateConverter();
    
    // Act
    var result = await converter.ConvertAsync("測試", 40, Array.Empty<string>());
    
    // Assert
    Assert.True(result.Success);
}
```

#### 3. 效能比較測試

建立效能基準測試：

```csharp
[Fact]
public async Task PerformanceComparison()
{
    string largeText = GenerateLargeText(10000); // 10000 字
    
    // 測試內建模式
    var sw1 = Stopwatch.StartNew();
    var result1 = await new InProcessBrailleConverter()
        .ConvertAsync(largeText, 40, Array.Empty<string>());
    sw1.Stop();
    
    // 測試外部工具模式
    var sw2 = Stopwatch.StartNew();
    var result2 = await new ExternalBrailleConverter()
        .ConvertAsync(largeText, 40, Array.Empty<string>());
    sw2.Stop();
    
    // 記錄結果
    _output.WriteLine($"內建模式: {sw1.ElapsedMilliseconds}ms");
    _output.WriteLine($"外部工具: {sw2.ElapsedMilliseconds}ms");
}
```

#### 4. 記憶體測試

**測試案例：** 連續轉換 10 次，檢查記憶體

```csharp
[StaFact]
public async Task MemoryLeakTest_MultipleConversions()
{
    long initialMemory = GC.GetTotalMemory(true);
    
    for (int i = 0; i < 10; i++)
    {
        using var converter = new InProcessBrailleConverter();
        await converter.ConvertAsync("測試文字", 40, Array.Empty<string>());
    }
    
    GC.Collect();
    GC.WaitForPendingFinalizers();
    GC.Collect();
    
    long finalMemory = GC.GetTotalMemory(true);
    long growth = finalMemory - initialMemory;
    
    Assert.True(growth < 10 * 1024 * 1024); // 增長應小於 10MB
}
```

---

## 實施檢查清單

### 準備階段
- [ ] 確認所有現有測試通過
- [ ] 記錄當前轉換效能基準（使用外部工具）

### 階段 1：定義介面
- [ ] 建立 `Services` 目錄
- [ ] 建立 `IBrailleConverter.cs`
- [ ] 建立 `BrailleConversionResult.cs`

### 階段 2：實作內建轉換
- [ ] 建立 `InProcessBrailleConverter.cs`
- [ ] 實作 `IDisposable`
- [ ] 測試基本轉換功能

### 階段 3：封裝外部工具
- [ ] 建立 `ExternalBrailleConverter.cs`
- [ ] 重構現有外部工具呼叫邏輯
- [ ] 測試外部工具模式

### 階段 4：組態與工廠
- [ ] 修改 `AppConfig.cs` 新增 `UseInProcessConversion`
- [ ] 建立 `BrailleConverterFactory.cs`
- [ ] 測試工廠建立正確的轉換器

### 階段 5：整合到 MainForm
- [ ] 修改 `DoConvertAsync`
- [ ] 修改 `HandleConversionResult`
- [ ] 移除舊的 `GetCvtErrors` 方法
- [ ] 更新所有呼叫點

### 階段 6：測試驗證
- [ ] 建立單元測試
- [ ] 執行整合測試
- [ ] 執行記憶體洩漏測試
- [ ] 執行效能比較測試
- [ ] 手動驗收測試（兩種模式）

### 階段 7：文件更新
- [ ] 更新使用者文件說明組態選項
- [ ] 更新開發者文件

---

## 風險評估與緩解

### 風險 1：兩種模式行為不一致
**機率：** 低  
**影響：** 中  
**緩解措施：**
- 建立比較測試確保結果一致
- 使用相同的核心轉換邏輯（BrailleProcessor）

### 風險 2：內建模式記憶體洩漏
**機率：** 低（已優化）  
**影響：** 高  
**緩解措施：**
- 實作 `IDisposable` 確保資源釋放
- 詳細的記憶體測試
- 保留外部工具作為退路

### 風險 3：組態選項被忽略
**機率：** 低  
**影響：** 低  
**緩解措施：**
- 明確的預設值（true = 內建）
- 在 UI 中提供切換選項（未來）

---

## 長期觀察指標

部署後應觀察：

1. **記憶體使用**
   - 內建模式的記憶體穩定性
   - 是否有記憶體洩漏回報

2. **效能表現**
   - 兩種模式的轉換速度比較
   - 使用者反饋

3. **錯誤率**
   - 兩種模式的錯誤發生率
   - 是否有模式特定的問題

4. **使用者偏好**
   - 有多少使用者切換到外部工具模式
   - 切換的原因

**決策點：** 6 個月後根據數據決定：
- 若內建模式穩定，可考慮將其設為唯一模式
- 若有問題，繼續保留雙模式或恢復只用外部工具

---

## 預估工作量

- **介面與結果類別：** 1 小時
- **內建轉換服務：** 3-4 小時
- **外部工具封裝：** 2 小時
- **組態與工廠：** 1 小時
- **MainForm 整合：** 2 小時
- **測試：** 3-4 小時
- **文件：** 1 小時

**總計：** 約 1.5 個工作天
