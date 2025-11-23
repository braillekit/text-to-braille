# Txt2Brl 與 EasyBrailleEdit 建置輸出合併

## 概要

為了方便進行整合測試，已在 `Txt2Brl.csproj` 中配置 MSBuild PostBuild 事件，每次建置 Txt2Brl 專案時，會自動將輸出檔案複製到 EasyBrailleEdit 的輸出目錄。

## 實作方式

在 `Txt2Brl.csproj` 中加入了以下 PostBuild Target：

```xml
<Target Name="CopyToEasyBrailleEditOutput" AfterTargets="Build">
  <PropertyGroup>
    <EasyBrailleEditOutputPath>$(MSBuildThisFileDirectory)..\..\..\Output\EasyBrailleEdit\$(Configuration)\$(Platform)\</EasyBrailleEditOutputPath>
  </PropertyGroup>
  
  <ItemGroup>
    <Txt2BrlOutputFiles Include="$(OutputPath)**\*.*" />
  </ItemGroup>
  
  <Message Text="Copying Txt2Brl output to: $(EasyBrailleEditOutputPath)" Importance="high" />
  
  <Copy SourceFiles="@(Txt2BrlOutputFiles)" 
        DestinationFolder="$(EasyBrailleEditOutputPath)%(RecursiveDir)" 
        SkipUnchangedFiles="true"
        OverwriteReadOnlyFiles="true" />
</Target>
```

## 複製的檔案

以下檔案會被自動複製：
- `Txt2Brl.exe` - 主要執行檔
- `Txt2Brl.dll` - 程式庫
- `Txt2Brl.pdb` - 偵錯符號檔
- `Txt2Brl.deps.json` - 相依性清單
- `Txt2Brl.runtimeconfig.json` - 執行階段配置
- `Txt2Brl.dll.config` - 應用程式配置
- `Txt2Brl.xml` - XML 文件
- `sample.txt` - 範例檔案
- 所有相依的 DLL 檔案

## 使用方式

### 建置單一專案
```powershell
dotnet build Source\EasyBrailleEditApp\Txt2Brl\Txt2Brl.csproj /p:Platform=x86
```

建置完成後，Txt2Brl 的輸出會自動複製到：
```
Output\EasyBrailleEdit\Debug\x86\
```

### 建置整個解決方案
```powershell
dotnet build Source\EasyBrailleEditApp\EasyBrailleEditApp.sln /p:Platform=x86
```

## 整合測試

建置完成後，可以在 EasyBrailleEdit 的輸出目錄中直接執行 Txt2Brl：

```powershell
cd Output\EasyBrailleEdit\Debug\x86
.\Txt2Brl.exe --help
```

## 優點

1. **自動化** - 每次建置自動複製，無需手動操作
2. **增量複製** - 只複製變更的檔案，提升建置速度
3. **整合測試** - 兩個應用程式的執行檔位於同一目錄，方便測試
4. **Version Control 友善** - 設定在專案檔中，不需要額外的腳本檔案

## 注意事項

- 如果只想建置 Txt2Brl 而不複製檔案，可以暫時註解掉 `<Target>` 區塊
- 複製的目標目錄會根據建置配置（Debug/Release）和平台（x86/x64）自動調整
