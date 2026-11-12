# Immutable Phase 4b Handoff

日期：2026-04-05

## 結論

`4b` 已收尾到可交棒狀態，可以作為 `4c` 的起點。

這裡的「完成」定義是：

- word-level builder / compatibility 策略已落地
- converter 層 append-only new word 建立已收斂，不再一律走 `BrailleWordBuilder.ToBrailleWord()`
- 既有 word 的 prepend / replace / mutation 路徑仍保留 builder
- benchmark 已有足夠證據支持目前 `4b` 方向，不再有會阻擋 `4c` 的未解 regression

## 4b 已交付的範圍

- `BrailleWordBuilder` / `BrailleCellBuffer` 骨架與 `IBrailleWordResult` 相容層
- `BrailleWordBuilder.FromBrailleWord(...)` / `ApplyTo(...)`
- 規則層與既有 word mutation 路徑的 builder 橋接：
  - `EnglishBrailleRule`
  - `GeneralBrailleRule`
  - `BrailleProcessor`
  - `TableConverter`
- converter 層 append-only new word 的 materialization 收斂：
  - `WordConverter`
  - `EnglishWordConverter`
  - `UrlConverter`
  - `EnglishUebConverter`
  - `TwChineseCharConverter`
  - `ConextTagConverter`

## 效能結論

### 1. `4a -> 4b` 主線 A/B

見 [`2026-04-05-phase4b-clean-worktree-ab.md`](/d:/work/BrailleKit/text-to-braille/docs-dev/refactoring/immutable/benchmark-result/2026-04-05-phase4b-clean-worktree-ab.md)。

結論：

- `4b` 已扳回 `4a` 大部分 throughput regression
- 但 allocation 當時仍偏高，因此才有後續的 converter materialization 收斂

### 2. converter materialization 收斂 A/B

見 [`2026-04-05-phase4b-converter-materialization-clean-worktree-ab.md`](/d:/work/BrailleKit/text-to-braille/docs-dev/refactoring/immutable/benchmark-result/2026-04-05-phase4b-converter-materialization-clean-worktree-ab.md)。

重點：

- 7 個 benchmark 的 allocation 全部下降
- 英文單行 `-32.47%`
- 英文多行 `-31.96%`
- throughput 在 7 個 benchmark 中有 6 個改善

### 3. mixed single-line 的 blocking 疑慮已解除

在完整 suite 的那次 clean A/B 中，`中英混合單行轉換` 曾出現 `+14.07%`。

但後續 focused clean rerun 顯示這個回歸沒有重現：

| Method | Baseline Mean | Candidate Mean | Mean Δ | Baseline Alloc | Candidate Alloc | Alloc Δ |
| ---- | ----: | ----: | ----: | ----: | ----: | ----: |
| 中英混合單行轉換 | 417.8 us | 374.3 us | -10.41% | 32.91 KB | 28.47 KB | -13.49% |

這表示該筆 `+14.07%` 不適合作為 `4b` 未完成的阻擋條件；目前較合理的判讀是量測交互影響或波動，而不是穩定 regression。

## 交棒給 4c 的前提

`4c` 可以直接建立在以下假設上：

- builder 只該用在需要 prepend / replace / mutation 的路徑
- append-only new word 若最終仍要 materialize 成既有 `BrailleWord`，優先直接建立 `BrailleWord`
- `BrailleWord` 的 reference identity 仍是有效邊界，`4c` 不應假設可以一次把整條資料流改成 fully immutable model

## 4c 建議起點

優先順序建議如下：

1. 先把 `BrailleWord` 的 construction boundary 定義清楚，而不是直接全面 immutable 化
2. 保留 `4b` 已驗證有效的 hybrid 策略
3. 每做一個切點就保留可獨立量測的 benchmark 基準，避免把 `BrailleWord` 與 `BrailleLine` 的訊號混在一起

## 備註

- 這份 handoff 的目的，是讓 `4c` 可以在不重做 `4b` 原因分析的前提下接續進行
- `4b` 並不代表 word model 已完全 immutable 化；它代表的是「word-level builder / compatibility 策略已收斂到可作為下一階段基礎」
