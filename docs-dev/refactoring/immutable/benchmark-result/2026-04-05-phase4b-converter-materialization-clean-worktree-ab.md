# Phase 4b converter materialization reduction clean worktree A/B benchmark

- 日期：2026-04-05
- baseline commit：`53a7c22123ae328165e529e5c582d56818a5104b`
- candidate commit：`322bc351f3f16360b6dedc494285cc17ebda86d9`
- benchmark command：
  - `dotnet run --project src/EasyBrailleEditApp/BrailleToolkit.Benchmarks/BrailleToolkit.Benchmarks.csproj -c Release -- --filter *BrailleConversionBenchmarks*`
- 方法：
  - 以兩個乾淨 detached worktree 分別 checkout baseline / candidate
  - 各自獨立 restore、build、執行完整 conversion benchmark suite
  - 量測完成後移除臨時 worktree

## 比較範圍

這次比較的是 `4b` 內部的單一步驟：

- baseline：converter 層仍大量使用 `BrailleWordBuilder.ToBrailleWord()` 來建立新的 `BrailleWord`
- candidate：append-only 的 converter 新 word 建立改回直接 `new BrailleWord(...)`

candidate 主要包含：

- [`WordConverter.cs`](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit/Converters/WordConverter.cs) 改回直接建立 `BrailleWord`
- [`EnglishWordConverter.cs`](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit/Converters/EnglishWordConverter.cs) 的 append-only 路徑改回直接 materialize
- [`UrlConverter.cs`](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit/Converters/UrlConverter.cs)
- [`EnglishUebConverter.cs`](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit/Converters/EnglishUebConverter.cs)
- [`ConextTagConverter.cs`](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit/Converters/ConextTagConverter.cs)
- [`TwChineseCharConverter.cs`](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit/Converters/TwChineseCharConverter.cs)

## A/B 摘要

| Method | Baseline Mean | Candidate Mean | Mean Δ | Baseline Alloc | Candidate Alloc | Alloc Δ |
| ---- | ----: | ----: | ----: | ----: | ----: | ----: |
| 中文單行轉換 | 64.14 us | 63.99 us | -0.23% | 6.32 KB | 5.82 KB | -7.91% |
| 英文單行轉換 | 517.64 us | 492.13 us | -4.93% | 23.87 KB | 16.12 KB | -32.47% |
| 中英混合單行轉換 | 373.81 us | 426.41 us | +14.07% | 32.91 KB | 28.47 KB | -13.49% |
| 中文多行轉換 | 3,156.22 us | 3,127.80 us | -0.90% | 360.95 KB | 327.83 KB | -9.18% |
| 英文多行轉換 | 3,108.76 us | 2,966.22 us | -4.59% | 147.48 KB | 100.35 KB | -31.96% |
| 中英混合多行轉換 | 1,965.24 us | 1,893.63 us | -3.64% | 132.43 KB | 109.40 KB | -17.39% |
| 長中文字串轉換 | 3,051.91 us | 2,951.22 us | -3.30% | 362.43 KB | 329.30 KB | -9.14% |

## 解讀

- 這一步把 `4b` 的 allocation 問題往正確方向拉回來了：
  - 7 個 benchmark 的 `Allocated` 全部下降
  - 降幅最大的是英文單行 `-32.47%`、英文多行 `-31.96%`
  - 中英混合多行也有 `-17.39%`
- throughput 也大致維持正向：
  - 7 個 benchmark 中有 6 個變快
  - 改善較明顯的是英文單行 `-4.93%`、英文多行 `-4.59%`、中英混合多行 `-3.64%`、長中文字串 `-3.30%`
  - 中文單行與中文多行接近持平
- 唯一明確變慢的是中英混合單行 `+14.07%`
  - 但同時 allocation 仍下降 `-13.49%`
  - 這條路徑可能仍受英文 / 中文 converter 切換與 rule interaction 影響，值得後續再拆解

## 結論

這次 clean worktree A/B 明確支持先前的 allocation 分析結論：

- converter 層 append-only 的新 word 建立，不適合一律走 `BrailleWordBuilder.ToBrailleWord()`
- 將這些路徑改回直接 `new BrailleWord(...)`，可以在不犧牲大部分 throughput 的情況下，顯著回收 allocation

目前比較合理的 `4b` 方向是：

- 保留 builder 給既有 `BrailleWord` mutation / prepend / replace 的橋接路徑
- converter 層 append-only new word 盡量直接 materialize
- 下一步若還要繼續壓 allocation，應優先看 mixed single-line 的退步來源，以及 remaining builder bridge 是否仍有不必要的中間配置
