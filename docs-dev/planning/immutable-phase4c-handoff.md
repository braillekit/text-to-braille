# Immutable Phase 4c Handoff

日期：2026-04-05

## 結論

`4c` 可以結案，並作為 `4d` 的直接起點。

這裡的「完成」定義是：

- `BrailleWord` 的 construction / materialization boundary 已收斂到 model 內部入口
- read-only downstream 已從 `BrailleWord` / `IBrailleWordResult` 雙軌收斂到 `IBrailleWordView`
- serializer / mutation / formatter / editor-document identity 幾個高風險熱點都已回到明確的 model boundary
- editor state 不再把 page title / undo-redo selection 這類狀態完全綁死在 raw row/col 或舊 reference equality 上

## 4c 最後完成的切點

### 1. page title editor state

- `DualEditTitleForm` 顯示 begin-line 資訊時，不再依賴 `TitleLine.Tag`
- `BraillePageTitle.TryResolveContentStartLineIndex(...)` 會：
  - 優先使用 `ContentStartLineIdentity`
  - 找不到時才 fallback 到既有 `ContentStartLineIndex`

這代表 page title UI 已不再把「對應正文哪一列」完全視為一個易失的 index-only 狀態。

### 2. undo/redo grid state

- `BrailleEditMemento` 現在會先 deep copy document，再以 snapshot document 內的 fresh line/word identity 建立 grid state
- `BrailleGridState` 不再只保存：
  - `ActivePosition`
  - `RangeRegion`
- 現在改為保存 model-bound bookmark：
  - `BrailleGridCellBookmark`
  - `BrailleGridRangeBookmark`

bookmark 內容包括：

- line identity
- word identity
- row offset
- merged-cell span 內的 column offset
- fallback row/column

restore 時會重新映射回目前 grid，而不是假設原本的 row/col 在新 document snapshot 中仍然有效。

### 3. merged-cell aware grid mapping

- `BrailleGridPositionMapper.TryGetBrailleWordAtGridPosition(...)` 已處理 SourceGrid merged cell
- 會往左找出真正的 word start column，而不是把 span 內任一欄位誤當成起點

這是 `4c` 很重要的收尾，因為 undo/redo selection bookmark 若踩到 span cell，中間欄位偏移會直接失真。

## 4c 已達成的整體狀態

下列邊界現在可視為已清掉，不需要在 `4d` 重開：

- `BrailleWord` construction / result boundary
- helper / formatter / font rendering 的 read-only consumption boundary
- YAML / JSON / deep copy 對 runtime identity 的影響
- page title begin-line anchor 與 formatter first-line identity 保留
- dual-edit editor 幾個最危險的 reference-equality / raw-position state 熱點

## 4d 下次建議起點

`4d` 的重點應回到：

- `BrailleLine` 的 mutable builder / immutable result 分離

建議第一刀不要直接碰整份 document 或 editor，而是先做 line-level read-only / construction boundary。

### 建議順序

1. 定義 `BrailleLine` 對應的 view / result contract
2. 盤點目前哪些 line-level downstream 還直接依賴 mutable `BrailleLine`
3. 先把 line construction boundary 拉出來，再決定 formatter / document 是否切到 line result

### 建議先看的熱點

- `BrailleLine.AssignWords(...)`
- `BrailleDocumentFormatter.FormatLine(...)`
- `BrailleDocument.InsertLine(...)` / `InsertLines(...)` / `RemoveLine(...)`
- dual-edit command 對 line mutation 的主路徑
- clipboard / memento / YAML / JSON 對 line identity 與 line snapshot 的互動

## 4d 不要倒退的原則

進 `4d` 時，請保留這些已驗證有效的 `4c` 前提：

1. 不要重新把 editor state 綁回 raw grid row/column 當成唯一真相來源。
2. formatter 若只是 reflow 第一列，應優先保留既有 line identity。
3. page title anchor 仍應以 line identity 為主，而不是回到 object reference 或單純 index。
4. 若 memento / clipboard / serialization 會產生新 line object，必須維持 fresh runtime identity 的語意。

## 驗證基準

目前已通過：

- `dotnet test src/EasyBrailleEditApp/BrailleToolkit.Tests/BrailleToolkit.Tests.csproj`
- `dotnet test src/EasyBrailleEditApp/EasyBrailleEdit.Tests/EasyBrailleEdit.Tests.csproj -p:GenerateRuntimeConfigurationFiles=true`
- `dotnet build src/EasyBrailleEditApp/EasyBrailleEditApp.sln -p:GenerateRuntimeConfigurationFiles=true`

## 建議下次開始時先讀的文件

1. [`docs-dev/refactoring/immutable/phase4.md`](/d:/work/BrailleKit/text-to-braille/docs-dev/refactoring/immutable/phase4.md)
2. [`docs-dev/planning/immutable-phase4c-word-construction-boundary.md`](/d:/work/BrailleKit/text-to-braille/docs-dev/planning/immutable-phase4c-word-construction-boundary.md)
3. 這份 handoff

讀完之後可以直接開始切 `4d`，不需要再回頭重做 `4c` 的 editor/document identity 原因分析。
