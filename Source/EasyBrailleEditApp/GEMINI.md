# Gemini 專案：EasyBrailleEditApp

此檔案提供 Gemini AI 助理理解和操作此專案所需的背景資訊。

## 專案概觀

`EasyBrailleEditApp` 是一個 C#/.NET 專案，用於編輯文字並將其轉換為點字。它是一個功能豐富的 Windows Forms 應用程式，主要的功能是讓使用者輸入一般文字之後，透過點字轉換功能將一般明眼人閱讀的文字轉換成視障者使用的點字。將一般文字轉換成點字時，中文字和全形標點符號是採用台灣的點字規則，英文字母和半形標點符號則是採用 UEB。轉換成點字之後，此應用程式提供了一個編輯器來讓使用者進一步對轉換後的點字文件進行編輯和排版，並提供列印功能，可以列印一般文字的部分，也能將點字的部分輸出至點字印表機。

目前所有 .csproj 都已經升級成 SDK-style 的專案格式。目標 .NET 框架為 .NET 9。

## 主要模組

- **`EasyBrailleEdit/`**: 主要的 Windows Forms 應用程式專案。這很可能是使用者操作的圖形介面。
- **`BrailleToolkit/`**: 核心類別庫，包含文字到點字轉換的主要邏輯、資料結構（方、行、文件）、格式化規則，以及不同點字標準（中文、英文 UEB、數學等）的資料表。
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
