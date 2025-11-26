# 應用程式組態檔說明

EasyBrailleEdit 的許多行為可以透過修改安裝目錄下的 `AppConfig.ini` 檔案來調整。此檔案是一個標準的 INI 格式文字檔，您可以使用記事本或其他純文字編輯器進行修改。

## [General] 一般設定

此區段包含應用程式的一般性設定。

| 參數名稱 | 預設值 | 說明 |
| :--- | :--- | :--- |
| **AutoUpdate** | `true` | 是否啟用自動更新檢查。 |
| **AutoUpdateFilesUrl** | (網址) | 自動更新檔案的下載網址。 |
| **UseFullWidthIme** | `false` | 是否預設使用全形輸入法。若設為 `true`，進入編輯區時會自動切換為全形模式；若設為 `false` 則為半形模式。 |
| **PhraseFiles** | `phrase.phf=0` | 指定自訂詞庫檔案。格式為 `檔名=優先權`。 |
| **AutoReplacedText** | (空) | 轉點字前的自動文字替換規則。格式範例：`{ABC=XYZ 《=<書名號>}`。 |

## [Braille] 點字轉換設定

此區段控制點字轉換的核心行為。

| 參數名稱 | 預設值 | 說明 |
| :--- | :--- | :--- |
| **UseInProcessConversion** | `true` | 是否使用內建轉換引擎。`true` 為內建模式，`false` 為外部工具模式。 |
| **EnableInstantPreview** | `true` | 是否預設啟用即時預覽面板。 |
| **AutoPreviewDelay** | `1500` | 即時預覽的延遲時間（毫秒）。 |
| **PreviewContextLines** | `3` | 即時預覽顯示游標前後的行數。 |
| **LinesPerPage** | `25` | 預設每頁列數。 |
| **CellsPerLine** | `40` | 預設每列方數。 |
| **AutoIndentNumberedLine** | `false` | 以 `#` 開頭的編號項目是否在折行時自動內縮。 |
| **ErrorProneWords** | `為` | 指定容易誤判的破音字，在雙視編輯時會以紅色標示。 |
| **UseUpperPositionForOrgPageNumber** | `true` | 原書頁碼數字是否使用上位點且不加數符。 |
| **NoSpaceAfterTheseCharacters** | (符號列表) | 指定哪些符號右側不自動加空方。 |
| **NoSpaceBeforeTheseCharacters** | (符號列表) | 指定哪些符號左側不自動加空方。 |
| **UseUpperPositionForNumbers** | `false` | 數字是否一律使用上位點。 |

## [BrailleEditor] 雙視編輯器設定

此區段設定雙視編輯視窗的行為。

| 參數名稱 | 預設值 | 說明 |
| :--- | :--- | :--- |
| **ShowUndoWindow** | `true` | 是否顯示復原/重做視窗。 |
| **MaxUndoLevel** | `10` | 最大復原次數。 |

## [Printing] 列印設定

此區段包含明眼字與點字列印的詳細參數。

| 參數名稱 | 預設值 | 說明 |
| :--- | :--- | :--- |
| **PrintPageFoot** | `true` | 是否列印頁尾（頁碼）。 |
| **PrintBrailleToBrailler** | `true` | 是否輸出至點字印表機。 |
| **PrintBrailleToFile** | `false` | 是否輸出至檔案。 |
| **PrintBrailleToFileName** | (空) | 輸出點字檔案的名稱。 |
| **PrintTextFontName** | `新細明體` | 明眼字列印字型。 |
| **PrintTextFontSize** | `12` | 明眼字列印字號。 |
| **PrintTextLineHeight** | `40.0975` | 明眼字列高（文字高度+列距）。 |
| **BrailleCellWidth** | `24` | 一方點字的列印寬度（影響明眼字字距）。 |

> **注意**：列印邊界設定（`PrintTextMargin...`）通常建議直接在列印對話窗中調整，程式會自動儲存設定。
