# Immutable refactoring - phase 1

## 變更檔案

| 檔案 | 變更 |
| ---- | ---- |
| [BrailleGlobals.cs](/src/EasyBrailleEditApp/BrailleToolkit/BrailleGlobals.cs) | 加 `readonly` |
| [ContextTagNames.cs](/src/EasyBrailleEditApp/BrailleToolkit/Tags/ContextTagNames.cs) | `HashSet` → `FrozenSet` |
| [SimpleTag.cs](/src/EasyBrailleEditApp/BrailleToolkit/Tags/SimpleTag.cs) | `Dictionary` → `FrozenDictionary` |
| [BrailleCharConverter.cs](/src/EasyBrailleEditApp/BrailleToolkit/Converters/BrailleCharConverter.cs) | `Dictionary` → `FrozenDictionary` |
| [BrailleFontConverter.cs](/src/EasyBrailleEditApp/BrailleToolkit/Converters/BrailleFontConverter.cs) | `Hashtable` → `FrozenDictionary` |
| [BrailleProcessor.cs](/src/EasyBrailleEditApp/BrailleToolkit/BrailleProcessor.cs) | `_autoReplacedText` → `FrozenDictionary`；`CharPosition` → `readonly record struct` |
| [ExternalBrailleConverter.cs](/src/EasyBrailleEditApp/EasyBrailleEdit/Services/ExternalBrailleConverter.cs) | 配合 `CharPosition` 建構子語法調整 |

## 效能測試結果

