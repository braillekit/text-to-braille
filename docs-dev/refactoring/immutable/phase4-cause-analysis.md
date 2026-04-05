# Phase 4a Regression Cause Analysis

## 問題摘要

Phase 4a 將 [`BrailleCell.cs`](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit/BrailleCell.cs) 從 singleton-style sealed class 改成 `readonly record struct` 之後：

- clean worktree A/B benchmark 顯示 7 個案例的 `Allocated` 全部下降
- 但 7 個案例的 `Mean` 全部上升
- 最大退步落在中文與中英混合路徑，約 `+35%` 到 `+59%`

正式 A/B 數字見：

- [`2026-04-05-phase4a-clean-worktree-ab.md`](./benchmark-result/2026-04-05-phase4a-clean-worktree-ab.md)

## 調查方式

這次不是只看 benchmark 結果，而是額外做了兩類驗證：

1. 讀實際熱路徑，確認 `BrailleCell` 在轉換流程中最常出現的操作型態
2. 做數個小型 micro test，分別檢查：
   - `Equals`
   - `GetInstance`
   - `List<T>` 走訪
   - `List<T>.Insert(0, ...)`

## 熱路徑觀察

`BrailleCell` 並不是被當成獨立值物件偶爾使用，而是深度嵌在 mutable list 流程中：

- [`BrailleCellList.cs`](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit/BrailleCellList.cs)
  - 內部就是 `List<BrailleCell>`
  - 大量使用 `Add`、`Insert`、indexer、`AddRange`
- [`BrailleWord.cs`](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit/BrailleWord.cs)
  - `Cells` 直接暴露 `List<BrailleCell>`
  - `Copy()`、`Copy(BrailleWord)`、`AddCells()` 都逐一搬運 cell
- [`EnglishBrailleRule.cs`](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit/Rules/EnglishBrailleRule.cs#L86)
  - 會逐一掃描 cell，並且在前端 `Insert(0, ...)`
- [`GeneralBrailleRule.cs`](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit/Rules/GeneralBrailleRule.cs#L371)
  - 會反覆做 `Cells[0]` 比較與前端插入數符
- [`BrailleProcessor.cs`](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit/BrailleProcessor.cs#L983)
  - 分數處理也會對 `Cells.Insert(0, ...)` / `Cells.Add(...)` 做多次操作

換句話說，現行架構的關鍵特徵不是 immutable value pipeline，而是：

- mutable `List<BrailleCell>`
- 大量 indexer 讀取
- 大量 foreach 複製
- 局部但反覆出現的前端插入

這種使用型態對 reference singleton class 比較友善，對 value type 較不友善。

## Micro test 結果

### 1. `Equals` 不是主因

用簡化版 `OldCell`（singleton class）、`PlainCell`（plain readonly struct）、`RecordCell`（readonly record struct）做大量 equality 測試時：

- `OldCell equality`: 約 `8 ms`
- `PlainCell equality`: 約 `7 ms`
- `RecordCell equality`: 約 `7 ms`

結論：

- `record struct` 的 value equality 本身沒有比舊 singleton class 更慢
- 因此 regression 不太像是 `Equals` 本身造成

### 2. `GetInstance` 不是主因

對大量 `GetInstance(...).Value` 存取做測試時：

- `OldCell GetInstance`: 約 `20 ms`
- `PlainCell GetInstance`: 約 `21 ms`
- `RecordCell GetInstance`: 約 `21 ms`

結論：

- 從靜態快取陣列取值本身差異很小
- regression 不太像是單純來自 factory 取值

### 3. `List<T>` 的值型別存取模式比較可疑

在 `List<T>` 走訪 / 前端插入的測試中，value type 版本出現比較一致的額外成本：

- `OldCell list iterate`: 約 `37 ms`
- `RecordCell list iterate`: 約 `40 ms`
- `OldCell insert-front`: 約 `39 ms`
- `RecordCell insert-front`: 約 `44 ms`

另外也確認了：

- `RecordCell` 的 size 是 `1 byte`

所以問題不是「struct 太大」，而是：

- 雖然單一 cell 很小
- 但放進 `List<T>` 之後，每次 `list[i]`、`foreach`、`Insert`、`AddRange` 都是在搬值
- 舊版 class 則主要是在搬 reference

## 最可能的原因

目前最合理的結論是：

1. `BrailleCell` 改成 value type 之後，的確減少了 heap allocation
2. 但現有轉換流程不是 immutable / builder-friendly 的資料流
3. 目前大量依賴 `List<BrailleCell>` 的 mutable 操作
4. 因此 throughput 的主要成本，從「配置與 reference 存取」轉成了「值複製與 list 內元素搬移」

也就是說，退步主因比較像是：

- `BrailleCell` 被放在 mutable `List<BrailleCell>` 裡反覆操作時的累積成本

而不是：

- `record struct` 產生的 equality
- `GetInstance` 本身
- struct size 過大

## 為什麼這會在中文 / 混合路徑更明顯

從 benchmark 結果來看，中文與混合內容退步最大，這和目前資料流相符：

- 中文路徑會產生大量 `BrailleWord`
- 後處理規則會頻繁調整相鄰 word / cell
- 混合內容會同時走中文、英文與數字符號規則，cell list 的讀寫更密集

因此在這些案例中，value type 化造成的 `List<BrailleCell>` 累積成本會被放大。

## 對後續 Phase 4 的意義

這次結果代表：

- `4a` 並不是單獨安全的「低階 value type 化」
- 如果直接往 `4b` 推 `BrailleCellList` / `BrailleWord` / `BrailleLine` immutable builder 化，風險很高

比較合理的下一步有兩種：

1. 先停止 `BrailleCell` value type 路線，回到 class/flyweight
2. 若仍想保留 value type 方向，必須連同資料流一起改
   - 例如 builder pattern
   - 避免在 `List<BrailleCell>` 上頻繁前插與複製
   - 重新設計 cell storage，而不是只把最底層型別改成 struct

## 結論

Phase 4a 的 regression 原因，最可能不是 `record struct` 語法糖本身，而是：

- 在現有 mutable list-heavy 架構下，把 `BrailleCell` 從 flyweight reference object 改成 value type，導致大量 `List<BrailleCell>` 讀寫與搬移成本累積，最終壓過 allocation 改善。
