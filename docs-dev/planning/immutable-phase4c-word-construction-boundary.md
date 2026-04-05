# Immutable Phase 4c - BrailleWord Construction Boundary First Cut

日期：2026-04-05

## 背景

`4b` 已完成目前定義下的 hybrid builder / compatibility 邊界：

- builder 保留在 prepend / replace / mutation 橋接
- append-only new word 仍優先直接建立既有 `BrailleWord`
- `IBrailleWordResult` / `BrailleWordMaterialized` 已存在，但還沒有成為明確的 `BrailleWord` construction boundary

因此 `4c` 的第一刀不應重開 `4b`，而是要把「builder/result 如何落到 `BrailleWord`」整理成單一邊界。

## 這一刀要解的問題

目前 builder 到舊 model 的 materialization 邏輯主要放在 `BrailleWordBuilderCompatibility`。

這樣的問題是：

- `BrailleWord` 自己不知道自己的 construction boundary 在哪裡
- `BrailleWord.Copy()` / `Copy(BrailleWord)` 仍維持另一套手工欄位搬運
- 後續若要讓更多 downstream 直接消費 `IBrailleWordResult`，落點仍然分散

## 第一個實作切點

把 `BrailleWord` 的建構與回填入口集中成 internal construction API：

1. `BrailleWord.CreateFromConstruction(...)`
2. `BrailleWord.ApplyConstruction(...)`
3. `BrailleWord.FromResult(IBrailleWordResult)`
4. `BrailleWord.ApplyResult(IBrailleWordResult)`

同時把 `BrailleWordBuilder` 與 `BrailleWordMaterialized` 改成使用這組 API，而不是外掛的 compatibility helper。

## 範圍

這一刀包含：

- 集中 `BrailleWord` materialization / apply 邏輯
- 讓 `BrailleWord.Copy()` / `Copy(BrailleWord)` 走同一條 construction path
- 補測試驗證 result -> existing word 的回填語意

這一刀不包含：

- 將公開 `BrailleWord` API 改成 immutable
- 將 converter append-only 路徑改成一律回傳 result
- 處理 `BrailleLine` / `BrailleDocument` 的 identity 相依
- 引入新的序列化格式

## 預期效果

- `BrailleWord` 自身成為 `4c` 的第一個 construction boundary
- 後續若要把部分 downstream 改吃 `IBrailleWordResult`，會有明確落點
- `Copy()` / compatibility materialization 不再有兩套欄位同步邏輯

## 下一步建議

完成這一刀後，`4c` 下一個切點可往下看兩個方向：

1. 讓更多 read-only downstream 先接受 `IBrailleWordResult`
2. 盤點仍直接操作 `BrailleWord.Cells` 的熱路徑，決定哪些需要停留在 compatibility 邊界、哪些可延後 materialize

## 後續進展：第二個切點

第二個切點已先往 read-only downstream 踏出一步：

- `BrailleWordHelper` 新增 internal overload，可直接處理 `IReadOnlyList<IBrailleWordResult>`
- `BrailleFontConverter` 新增 internal overload，可直接處理單一 `IBrailleWordResult`
- 這代表目前至少在 helper / debug / font rendering 這一類唯讀路徑上，materialized result 已可被直接消費，不必先轉回 `BrailleWord`

這一刀的意義是：

- 把 `IBrailleWordResult` 從「只能回轉舊 model 的過渡物件」推進成「可直接被部分 downstream 讀取的結果邊界」
- 後續若要挑 formatter / exporter / preview 路徑延後 materialize，已經有可重用的 helper 基礎

## 後續進展：第三個切點

第三個切點把 line-level rendering 也往 result 邊界推進：

- 新增 `BrailleWordSequenceFormatter`
- 既有 `BrailleLine.ToBrailleCellHexString()` / `ToPositionNumberString()` / `ToHtmlString()` 已改成委派給 sequence formatter
- sequence formatter 同時提供 `IReadOnlyList<BrailleWord>` 與 `IReadOnlyList<IBrailleWordResult>` 版本

這表示：

- HTML export 這條既有 read-only downstream，現在與 result renderer 共用同一組格式化邏輯
- 下一步若要讓 formatter / exporter 其中一段直接吃 result，不必再先重做 line-level rendering 基礎

## 後續進展：第四個切點

第四個切點把 `BrailleWord` 與 immutable result 收斂到同一個唯讀 contract：

- 新增 `IBrailleWordView`
- `BrailleWord` 實作 `IBrailleWordView`
- `IBrailleWordResult` 改成繼承 `IBrailleWordView`
- `BrailleWordHelper` / `BrailleFontConverter` / `BrailleWordSequenceFormatter` 的 internal read-only API 全部改以 `IBrailleWordView` 為共同下游介面

這一刀的意義是：

- `4c` 的 read-only boundary 不再需要同時維護「BrailleWord 版」與「result 版」兩套邏輯
- downstream 看見的是同一個 word view contract，而不是來源型別差異
- 之後要收尾 `4c`，就可以把重點放在剩餘仍強依賴 mutable object identity 或直接 mutation 的 call site，而不是繼續擴散平行 overload
