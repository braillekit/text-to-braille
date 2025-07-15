# Gemini 專案：EasyBrailleEditApp

此檔案提供 Gemini AI 助理理解和操作此專案所需的背景資訊。

## 專案概觀

`EasyBrailleEditApp` 是一個 C#/.NET 專案，用於編輯文字並將其轉換為點字。它是一個功能豐富的 Windows Forms 應用程式，主要的功能是讓使用者輸入一般文字之後，透過點字轉換功能將一般明眼人閱讀的文字轉換成視障者使用的點字。將一般文字轉換成點字時，中文字和全形標點符號是採用台灣的點字規則，英文字母和半形標點符號則是採用 UEB。轉換成點字之後，此應用程式提供了一個編輯器來讓使用者進一步對轉換後的點字文件進行編輯和排版，並提供列印功能，可以列印一般文字的部分，也能將點字的部分輸出至點字印表機。

目前所有 .csproj 都已經升級成 SDK-style 的專案格式。目標 .NET 框架為 .NET 9。

## 主要模組

- **`EasyBrailleEdit/`**: 主要的 Windows Forms 應用程式專案。這很可能是使用者操作的圖形介面。
- **`BrailleToolkit/`**: 核心類別庫，包含文字到點字轉換的主要邏輯、資料結構��方、行、文件）、格式化規則，以及不同點字標準（中文、英文 UEB、數學等）的資料表。
- **`BrailleToolkit.Tests/`**: `BrailleToolkit` 的單元測試專案，確保核心轉換和格式化邏輯的正確性。
- **`Txt2Brl/`**: 一個命令列工具，用於文字到點字的轉換，可能使用了 `BrailleToolkit` 函式庫。
- **`EasyBrailleEdit.Common/`**: 一個共用函式庫，用於存放共通常數、應用程式全域變數和設定。

## 如何建置

此專案是一個 Visual Studio 解決方案 (`.sln`)。要建置此專案，您可以使用以  方法：
1.  在 Visual Studio 中開啟 `EasyBrailleEditApp.sln` 並建置解決方案。
2.  從命令列使用 `msbuild`：`msbuild EasyBrailleEditApp.sln`。

## 如何測試

測試位於 `BrailleToolkit.Tests` 專案中。您可以使用以下方式執行它們：
1.  Visual Studio 中的「測試總管」。
2.  在 `BrailleToolkit.Tests/` 目錄中執行 `dotnet test` 命令。

## 點字處理流程與架構

此專案的點字處理核心是 `BrailleToolkit` 函式庫，其架構採用了策略模式（Strategy Pattern），使得處理不同語言（中文、英文）或內容（數學、表格）的規則得以分離，保持了程式碼的模組化與可擴充性。

### 核心元件

1.  **`BrailleProcessor` (主控中心/Context):**
    *   這是點字轉換的總指揮。它負責接收原始的明眼文字串，並協調各個轉換器（Converter）來完成工作。
    *   它管理一個 `ContextTagManager` 來追蹤目前的轉換情境（例如，是否在數學模式或表格模式中）。
    *   在完成初步轉換後，它會套用一系列後處理規則（例如數字規則、大寫規則、標點符號規則）來確保點字的正確性。

2.  **`IWordConverter` (轉換器介面/Strategy Interface):**
    *   定義了所有具體轉換器都必須實作的標準介面。其核心方法是 `Convert(Stack<char> chars, ContextTagManager context)`。
    *   這種設計讓 `BrailleProcessor` 無需知道每個轉換器的內部細節，只需呼叫其 `Convert` 方法即可。

3.  **具體的轉換器 (Concrete Strategies):**
    *   **`ChineseWordConverter`**: 負責處理中文字、注音及全形標點符號。它會使用 `ZhuyinReverseConverter` 取得中文字的注音碼，並利用智慧型詞彙分析來修正破音字，最後轉換成台灣點字規則的點字。
    *   **`EnglishUebConverter`**: 負責處理英文（UEB - Unified English Braille）。它採用「貪婪演算法」(Greedy Algorithm)，優先匹配最長的縮寫（Contractions），若無匹配，則退回逐字翻譯（Grade 1）。
    *   **`MathConverter`, `TableConverter`, `UrlConverter` 等**: 這些是針對特定情境的轉換器，只有在對應的標籤（如 `<math>`）被啟用時才會作用。
    *   **`ContextTagConverter`**: 專門用來解析情境控制標籤（如 `<math>`, `</math>`），並更新 `ContextTagManager` 的狀態。它具有最高的處理優先權。

### 處理流程

點字轉換的過程可以分為以下幾個主要步驟：

1.  **輸入與預處理**: `BrailleProcessor.ConvertLine` 方法接收一行文字。為了方便處理，文字會被反轉（reverse）並存入一個字元堆疊 (`Stack<char>`)。
2.  **核心轉換迴圈**: `BrailleProcessor` 逐一讀取堆疊中的字元，並在 `ConvertWord` 方法中依序嘗試使用不同的轉換器：
    *   首先，`ContextTagConverter` 檢查是否為情境標籤。
    *   接著，根據當前情境（如數學、表格）呼叫對應的轉換器。
    *   然後，依序嘗試 `ChineseWordConverter` 和 `EnglishUebConverter`。
    *   第一個成功轉換的轉換器會從堆疊中取出其處理過的字元，並傳回 `BrailleWord` 物件串列。
3.  **後處理與規則套用**: 當整行文字都被轉換成 `BrailleWord` 串列（存於 `BrailleLine` 物件）後，`BrailleProcessor` 會進行一系列的後處理，套用各種點字規則：
    *   處理私名號、書名號��
    *   套用中文標點符號規則。
    *   套用英文大寫、數字規則。
    *   在中英文之間補上必要的空格。
4.  **輸出**: 最終產出一個結構化的 `BrailleLine` 物件，其中包含了所有點字方 (cell) 的資訊，可用於後續的排版、編輯和列印。

### 資料模型

*   **`BrailleDocument`**: 代表整個點字文件。
*   **`BrailleLine`**: 代表一行點字。
*   **`BrailleWord`**: 代表一個邏輯單位，如一個中文字、一個英文字詞或一個標點符號。
*   **`BrailleCell`**: 代表一個單獨的六點點字方。

## 點字碼 (code) 與點位對應規則

在 `BrailleToolkit` 函式庫中，一個點字方 (Braille Cell) 的六個點位是透過一個位元組 (byte) 的位元遮罩 (bitmask) 來表示的。XML 資料檔 (`/Data/*.xml`) 中的 `code` 屬性值就是這個位元組的十六進位 (hex) 表示法。

對應規則如下，點 1 到點 6 分別對應到一個位元組的第 0 個位元到第 5 個位元：

| 點位 | 位元 (Bit) | 十進位值 | 十六進位值 |
| :--- | :--- | :--- | :--- |
| 點 1 | Bit 0 | 1 | `0x01` |
| 點 2 | Bit 1 | 2 | `0x02` |
| 點 3 | Bit 2 | 4 | `0x04` |
| 點 4 | Bit 3 | 8 | `0x08` |
| 點 5 | Bit 4 | 16 | `0x10` |
| 點 6 | Bit 5 | 32 | `0x20` |

### 範例

以英文字母 **W** 為例，其標準點位為 **2, 4, 5, 6**。

1.  **查表**: 在 `EnglishBrailleTable.xml` 中，`<symbol text="W" code="3A" type="Letter" />`。
2.  **轉換**: `code="3A"`，其十六進位值 `3A` 等於十進位的 `58`。
3.  **分解**: `58 = 32 + 16 + 8 + 2`。
4.  **對應**:
    *   `32` -> 點 6
    *   `16` -> 點 5
    *   `8` -> 點 4
    *   `2` -> 點 2
5.  **結論**: `code="3A"` 正確地表示了點位 2, 4, 5, 6。

### 備註

*   **多方點字**: 如果 `code` 屬性的長度超過 2 個字元（例如 `code="101010"`），代表此符號由多個點字方組成。每個兩位數的十六進位碼代表一個點字方。
*   **`dots` 屬性**: 部分 XML 項目使用 `dots` 屬性（如 `dots="126"`）來直接表示點位。在程式載入時，`XmlBrailleTable` 類別會呼叫 `BrailleCellHelper.PositionNumbersToHexString` 方法將 `dots` 屬性的值轉換成對應的 `code` 十六進位碼。
