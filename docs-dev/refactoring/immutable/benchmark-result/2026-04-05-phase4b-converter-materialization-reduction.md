# Phase 4b converter new-word materialization reduction

日期：2026-04-05

## 目的

根據 [`2026-04-05-phase4b-compat-allocation-analysis.md`](./2026-04-05-phase4b-compat-allocation-analysis.md) 的結果，`ApplyToBrailleWord(...)` 本身不是 allocation 主因；真正較重的是 converter 層大量走 `BrailleWordBuilder.ToBrailleWord()` 的新 word materialization。

這一步的目標是：

- 保留 builder 給既有 `BrailleWord` mutation / prepend / replace 路徑
- 將 append-only 的 converter 新 word 建立改回直接 `new BrailleWord(...)`
- 觀察單行 conversion benchmark 的 throughput / allocation 是否回收

## 實作範圍

- [`WordConverter.cs`](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit/Converters/WordConverter.cs)
  - `ConvertToBrailleWord(string)` 改回直接 `new BrailleWord(text, brCode)`
- [`EnglishWordConverter.cs`](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit/Converters/EnglishWordConverter.cs)
  - 新 word 建立改成直接 materialize
  - 時間冒號前插改用既有 `BrailleWord.CellList.Insert(0, ...)`
- [`UrlConverter.cs`](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit/Converters/UrlConverter.cs)
  - append-only 路徑改回直接 `BrailleWord`
- [`EnglishUebConverter.cs`](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit/Converters/EnglishUebConverter.cs)
  - 內部對照表由 `Dictionary<string, BrailleWord>` 改成 `Dictionary<string, string>`
  - 比對成功後直接建立新 `BrailleWord`
- [`ConextTagConverter.cs`](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit/Converters/ConextTagConverter.cs)
  - context tag word 改回直接 `new BrailleWord(text)`
- [`TwChineseCharConverter.cs`](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit/Converters/TwChineseCharConverter.cs)
  - 注音、聲調、標點、其它符號與 phonetic cell list 路徑改成直接建立 `BrailleWord`

## 驗證

### 測試 / 建置

- `dotnet test src/EasyBrailleEditApp/BrailleToolkit.Tests/BrailleToolkit.Tests.csproj --no-restore -m:1`
- `dotnet build src/EasyBrailleEditApp/BrailleToolkit/BrailleToolkit.csproj --no-restore -m:1`

結果：

- `BrailleToolkit.Tests`: 143 / 143 通過
- `BrailleToolkit` build：成功

### Workspace snapshot benchmark

命令：

```powershell
dotnet run --project src/EasyBrailleEditApp/BrailleToolkit.Benchmarks/BrailleToolkit.Benchmarks.csproj -c Release -- --filter '*BrailleConversionBenchmarks.ConvertSingleChineseLine*' '*BrailleConversionBenchmarks.ConvertSingleEnglishLine*' '*BrailleConversionBenchmarks.ConvertSingleMixedLine*'
```

結果：

| Method | Mean | Allocated |
| ---- | ----: | ----: |
| 中文單行轉換 | 45.17 us | 5.82 KB |
| 英文單行轉換 | 332.52 us | 16.12 KB |
| 中英混合單行轉換 | 294.37 us | 28.47 KB |

## 與前一個 workspace snapshot 的對照

前一個 snapshot 來自 `4b` converter 擴散後、尚未減少 `ToBrailleWord()` 次數的 workspace 量測：

| Method | Previous Mean | Current Mean | Mean Δ | Previous Alloc | Current Alloc | Alloc Δ |
| ---- | ----: | ----: | ----: | ----: | ----: | ----: |
| 中文單行轉換 | 50.29 us | 45.17 us | -10.18% | 6.32 KB | 5.82 KB | -7.91% |
| 英文單行轉換 | 363.10 us | 332.52 us | -8.42% | 23.87 KB | 16.12 KB | -32.47% |
| 中英混合單行轉換 | 293.25 us | 294.37 us | +0.38% | 32.91 KB | 28.47 KB | -13.49% |

## 解讀

- converter 層的 append-only 新 word 建立改回直接 `BrailleWord` 後，allocation 有明顯回收：
  - 英文單行 `-32.47%`
  - 中英混合單行 `-13.49%`
  - 中文單行 `-7.91%`
- throughput 也同步改善或持平：
  - 中文單行 `-10.18%`
  - 英文單行 `-8.42%`
  - 中英混合單行大致持平（`+0.38%`，可視為量測波動）
- 這個結果支持先前的 allocation 分析結論：converter 層的新 word materialization 確實是目前 `4b` allocation 壓力的主要來源之一。
- builder 仍值得保留在既有 word 的 prepend / replace / compatibility bridge 路徑；但 append-only 的新 word 建立不宜一律走 `BrailleWordBuilder.ToBrailleWord()`。
