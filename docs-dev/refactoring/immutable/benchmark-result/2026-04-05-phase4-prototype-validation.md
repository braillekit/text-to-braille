# Phase 4 Prototype Validation

日期：2026-04-05

## 目的

在保留 [`phase4-cause-analysis.md`](../phase4-cause-analysis.md) 既有結論的前提下，進一步驗證兩件事：

1. 把 `BrailleCell` 從 `readonly record struct` 改成 plain `readonly struct`，能不能直接消除真實轉換流程的 regression。
2. 如果只靠型別切換不夠，改變 cell storage / builder 流程是否可能帶來改善訊號。

## 實驗 1：真實 pipeline，`record struct` vs plain `readonly struct`

### 方法

- 建立兩個 clean worktree，基準都來自 commit `2a20e527a0975ff544f4662eff7142054b811ff1`
- baseline：維持目前 `BrailleCell = readonly record struct`
- candidate：只在暫時 worktree 內把 [`BrailleCell.cs`](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit/BrailleCell.cs) 改成 plain `readonly struct : IEquatable<BrailleCell>`
- 其餘程式碼、benchmark 專案、測試資料都不變
- 命令：
  - `dotnet run --project src/EasyBrailleEditApp/BrailleToolkit.Benchmarks/BrailleToolkit.Benchmarks.csproj -c Release`

### 結果摘要

| Method | Record Mean | Plain Mean | Mean Δ | Record Alloc | Plain Alloc | Alloc Δ |
| ---- | ----: | ----: | ----: | ----: | ----: | ----: |
| 中文單行轉換 | 67.76 us | 68.77 us | +1.49% | 5.82 KB | 5.82 KB | 0.00% |
| 英文單行轉換 | 509.94 us | 529.43 us | +3.82% | 16.09 KB | 16.09 KB | 0.00% |
| 中英混合單行轉換 | 438.68 us | 415.23 us | -5.35% | 28.73 KB | 28.73 KB | 0.00% |
| 中文多行轉換 | 3,369.05 us | 3,424.78 us | +1.65% | 327.83 KB | 327.83 KB | 0.00% |
| 英文多行轉換 | 3,156.25 us | 3,579.06 us | +13.40% | 100.21 KB | 100.21 KB | 0.00% |
| 中英混合多行轉換 | 1,991.48 us | 2,006.35 us | +0.75% | 111.28 KB | 111.28 KB | 0.00% |
| 長中文字串轉換 | 3,222.80 us | 3,149.30 us | -2.28% | 329.30 KB | 329.30 KB | 0.00% |

### 解讀

- plain `readonly struct` 沒有一致性地修復 regression。
- 七個真實 benchmark 裡：
  - 2 個案例變快：中英混合單行、長中文字串
  - 5 個案例變慢：中文單行、英文單行、中文多行、英文多行、中英混合多行
- allocation 完全相同，代表 plain struct 並沒有在目前資料流下帶來額外的配置優勢。
- 這次驗證支持了前一份原因分析的判斷：
  - 問題主因不是 `record` 語法糖本身
  - 單純把 `record struct` 換成 plain `readonly struct`，不足以避免真實 pipeline 的 throughput regression

## 實驗 2：Synthetic storage / builder prototype

### 方法

以 [`BrailleCellStoragePrototypeBenchmarks.cs`](/d:/work/BrailleKit/text-to-braille/src/EasyBrailleEditApp/BrailleToolkit.Benchmarks/BrailleCellStoragePrototypeBenchmarks.cs) 模擬目前熱路徑常見的 cell 操作：

- append
- prepend
- scan
- copy

比較四種 prototype：

1. class flyweight + `List<T>`
2. plain `readonly struct` + `List<T>`
3. `readonly record struct` + `List<T>`
4. plain `readonly struct` + deque-style buffer

命令：

- `dotnet run --project src/EasyBrailleEditApp/BrailleToolkit.Benchmarks/BrailleToolkit.Benchmarks.csproj -c Release -- --filter *BrailleCellStoragePrototypeBenchmarks*`

### 結果摘要

| Method | Mean | Mean vs class | Allocated | Alloc vs class |
| ---- | ----: | ----: | ----: | ----: |
| class flyweight + List | 5.680 ms | baseline | 15.87 MB | baseline |
| plain readonly struct + List | 4.760 ms | -16.20% | 12.70 MB | -19.97% |
| readonly record struct + List | 4.757 ms | -16.25% | 12.70 MB | -19.97% |
| plain readonly struct + deque buffer | 4.512 ms | -20.56% | 12.17 MB | -23.31% |

### 解讀

- 在這個 synthetic benchmark 中，plain struct 與 record struct 幾乎沒有差異，差距約 `-0.06%`，可視為同級。
- deque-style buffer 比 `List<T>` 版本再快約 `5.21%`，allocation 也再下降一些。
- 這表示如果我們連同 storage / builder 流程一起改，確實有機會得到正面訊號。
- 但這組 benchmark 不能直接推論到正式轉點字流程，因為它只模擬 cell 操作型態，沒有包含完整中文 / 英文 / 後處理規則。

## 綜合結論

這輪 prototype 驗證把方向縮小得更清楚：

1. plain `readonly struct` 不是 `4a` regression 的直接修復手段。
2. `record struct` 與 plain `readonly struct` 的差異，在 synthetic 與真實驗證裡都沒有大到足以解釋整體 regression。
3. 若要延續 immutable / value type 方向，較有希望的是：
   - 改 cell storage
   - 引入 builder / buffer
   - 減少 `List<BrailleCell>` 前插、複製與搬移
4. 因此下一步若要做 prototype，應優先測：
   - `BrailleWordBuilder`
   - `BrailleCellBuffer`
   - `CollectionsMarshal.AsSpan(...)` 或連續 buffer 掃描

## 暫定結論

- `BrailleCell = record struct` 改成 plain struct：不足以解決 regression。
- `cell storage / builder` 路線：有繼續做原型的價值。
