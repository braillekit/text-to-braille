# Phase 4b compatibility allocation analysis

- 日期：2026-04-05
- benchmark：
  - [`BrailleWordCompatibilityBenchmarks.cs`](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit.Benchmarks/BrailleWordCompatibilityBenchmarks.cs)
- command：
  - `dotnet run --project src/EasyBrailleEditApp/BrailleToolkit.Benchmarks/BrailleToolkit.Benchmarks.csproj -c Release -- --filter *BrailleWordCompatibilityBenchmarks*`

## 目的

這組 micro-benchmark 不量正式轉點字流程，而是只拆 builder 與既有 `BrailleWord` 相容層本身的 allocation：

- 新建 `BrailleWord`
- `CreateBrailleWord(span)`
- `Builder.ToBrailleWord()`
- `Builder.Build()`
- `Builder.Build().ToBrailleWord()`
- `ApplyToBrailleWord(span)` 套回既有 word
- `FromBrailleWord()` / `FromBrailleWord() + ApplyTo()`

測試同時跑了兩種代表情境：

- `English`：2-cell 英文詞
- `Chinese`：3-cell 中文詞

## 主要結果

### English

| Method | Mean | Allocated |
| ---- | ----: | ----: |
| Direct: new BrailleWord + fill list | 26.11 ns | 88 B |
| Compat: CreateBrailleWord(span) | 62.08 ns | 176 B |
| Builder: ToBrailleWord() | 104.20 ns | 344 B |
| Builder: Build() | 57.87 ns | 288 B |
| Builder: Build().ToBrailleWord() | 123.50 ns | 464 B |
| Direct: mutate existing word | 10.67 ns | 0 B |
| Compat: ApplyToBrailleWord(span) | 28.63 ns | 0 B |
| Builder: FromBrailleWord() | 33.47 ns | 128 B |
| Builder: FromBrailleWord() + ApplyTo() | 63.68 ns | 168 B |

### Chinese

| Method | Mean | Allocated |
| ---- | ----: | ----: |
| Direct: new BrailleWord + fill list | 25.98 ns | 88 B |
| Compat: CreateBrailleWord(span) | 65.40 ns | 176 B |
| Builder: ToBrailleWord() | 104.33 ns | 344 B |
| Builder: Build() | 86.59 ns | 288 B |
| Builder: Build().ToBrailleWord() | 146.72 ns | 464 B |
| Direct: mutate existing word | 12.83 ns | 0 B |
| Compat: ApplyToBrailleWord(span) | 29.75 ns | 0 B |
| Builder: FromBrailleWord() | 33.42 ns | 128 B |
| Builder: FromBrailleWord() + ApplyTo() | 86.05 ns | 168 B |

## 拆解

### 1. `ApplyToBrailleWord(span)` 本身不是 allocation 主因

- `Direct: mutate existing word` = `0 B`
- `Compat: ApplyToBrailleWord(span)` = `0 B`

代表目前 `ApplyToBrailleWord(span)` 雖然有 CPU 成本，但在重用既有 `BrailleWord.Cells` list instance 的前提下，本身沒有新增 managed allocation。

### 2. 既有 word 的 builder bridge 有固定額外成本

- `Builder: FromBrailleWord()` = `128 B`
- `Builder: FromBrailleWord() + ApplyTo()` = `168 B`

這代表舊 word 走 builder bridge 時，主要 allocation 來自：

- `BrailleWordBuilder` 物件本身
- `BrailleCellBuffer` 內部陣列
- 從既有 `List<BrailleCell>` 複製到 buffer 的橋接過程

而不是 `ApplyTo(...)` 套回 target 的那一段。

### 3. 新建 word 路徑才是 allocation 放大的主要來源

- direct new word = `88 B`
- `CreateBrailleWord(span)` = `176 B`
- `Builder.ToBrailleWord()` = `344 B`

可直接看出兩層堆疊：

- `CreateBrailleWord(span)` 比 direct path 多 `88 B`
- `Builder.ToBrailleWord()` 比 `CreateBrailleWord(span)` 再多 `168 B`

也就是說，converter 目前大量把新字詞改成：

`BrailleWordBuilder -> ToBrailleWord()`

時，額外 allocation 主要是：

1. builder / buffer 自身的固定配置成本
2. 最後 materialize 成既有 `BrailleWord` object graph 的固定成本

這也解釋了為什麼 `4b` clean worktree A/B 中：

- 英文 throughput 已明顯回升
- 但英文單行 / 英文多行 allocation 仍大幅上升

因為英文 converter 會高頻率地「新建很多 BrailleWord」，而不是只在少數規則上 mutate 既有 word。

### 4. `Build()` 路徑有明確的雙重 materialization 成本

- `Builder.Build()` = `288 B`
- `Builder.Build().ToBrailleWord()` = `464 B`

這證明：

- `Build()` 會先配置一份 cell array 與 materialized result
- 再 `ToBrailleWord()` 時，又再配置一份既有 `BrailleWord` 相容 object graph

因此 `Build().ToBrailleWord()` 應持續避免出現在正式熱路徑。

## 結論

目前 builder 到既有 `BrailleWord` 相容層的 allocation 原因，可以拆成兩類：

1. 新建 word 的 compatibility materialization  
`BrailleWordBuilder.ToBrailleWord()` 每次都要建立既有 `BrailleWord` / `BrailleCellList` / `List<BrailleCell>` 結構，這是目前最大的 allocation 來源。

2. 舊 word 的 builder bridge 固定成本  
`FromBrailleWord()` 會額外配置 builder 與 buffer，但 `ApplyTo(...)` 本身已經是零配置。

因此下一步最值得做的，不是再優化 `ApplyToBrailleWord(span)`，而是：

- 優先減少 converter 層「每個新 word 都先建 builder，再 materialize 成舊 model」的次數
- 或讓 builder/result 能在更多下游流程中被直接消費，延後甚至避免 `ToBrailleWord()`
- 若仍需相容舊 model，則優先想辦法降低 `CreateBrailleWord(span)` / `ToBrailleWord()` 的固定 object graph 配置成本
