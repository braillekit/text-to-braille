# 支援 YAML 檔案格式設計草案

> 由 Gemini CLI 生成。

## 1. 專案目標

本設計旨在為 `EasyBrailleEditApp` 引入一種新的檔案儲存格式：**YAML**。目前的 `.brx` 檔案格式基於 JSON，雖然能夠完整保存物件狀態，但其可讀性較差，且不易於使用一般文字編輯器進行檢視。新的 YAML 格式應在保持資料完整性（Full Object State Preservation）的前提下，顯著提升檔案的人眼可讀性 (Human-Readability)。

## 2. 格式選擇理由

在評估了 JSON、Markdown、XML、TOML 與 YAML 等格式後，選擇 YAML 作為新格式的主要理由如下：

1.  **完整性 (Data Integrity)**：YAML 是 JSON 的超集，能夠完美對應 `BrailleDocument` -> `BrailleLine` -> `BrailleWord` 的巢狀物件結構，確保所有屬性（如 `IsPolyphonic`, `PhoneticCode`, `CellList`）都能被精確保存與還原。
2.  **可讀性 (Readability)**：
    *   去除大量括號 (`{`, `}`, `[`, `]`) 與引號 (`"`)，畫面簡潔。
    *   使用縮排 (Indentation) 表示層級，符合人類直覺。
    *   支援多行字串，對於長文本的呈現優於 JSON。
3.  **編輯友善**：雖然不預期使用者手動修改複雜屬性，但 YAML 格式允許使用者在必要時使用記事本等工具查看內容，且易於進行版本控制 (Git diff)。

## 3. 技術選型

建議使用 **YamlDotNet** 函式庫進行實作。

*   **理由**：它是 .NET 生態系中最成熟、功能最完整的 YAML 處理函式庫，支援將 YAML 文件直接序列化/反序列化為 .NET 物件 (POCO)，且社群活躍度高。

## 4. 實作建議與重構需求

為了順利整合 YAML 序列化功能，現有的資料模型 (Data Models) 需要進行以下重構：

### 4.1 增加無參數建構函式 (Parameterless Constructors)

目前的序列化機制 (`DataContractJsonSerializer`) 可繞過建構函式直接建立物件，但現代序列化工具（包括 `System.Text.Json` 和 `YamlDotNet`）通常要求目標類別具備**無參數建構函式**。

**受影響類別：**
*   **`BrailleWord`**：目前僅有帶參數的建構函式。
    *   **行動**：必須新增一個無參數建構函式。可以是 `private` 或 `internal`，但需確保序列化庫能夠存取。
*   **`BraillePageTitle`**：已有 `private` 無參數建構函式。
    *   **行動**：確認 `YamlDotNet` 是否能存取 private 建構函式，或考慮將其改為 `internal`。

### 4.2 屬性標籤 (Attributes)

*   目前的 `[DataMember(Name="...")]` 是針對 `DataContractJsonSerializer` 設計的。
*   若使用 `YamlDotNet`，預設會使用屬性名稱作為 Key。若需自定義名稱，需使用 `[YamlMember(Alias = "...")]`。
*   **建議**：評估是否需要引入 `YamlDotNet` 的屬性依賴，或者直接使用預設命名規則（通常 PascalCase）。

## 5. 格式範例對照

### 目前格式 (JSON / .brx)

```json
{
  "CellsPerLine": 40,
  "Lines": [
    {
      "Words": [
        {
          "Text": "夏",
          "PhoneticCode": "ㄒㄧㄚˋ",
          "IsPolyphonic": true,
          "Cells": [ { "Value": 17 }, { "Value": 62 }, { "Value": 16 } ]
        }
      ]
    }
  ]
}
```

### 建議格式 (YAML)

```yaml
CellsPerLine: 40
Lines:
  - Words:
      - Text: 夏
        PhoneticCode: ㄒㄧㄚˋ
        IsPolyphonic: true
        Cells:
          - Value: 17
          - Value: 62
          - Value: 16
```

## 6. 風險與注意事項

1.  **檔案大小**：YAML 雖然減少了括號，但增加了縮排空白。對於大型文件，檔案大小可能會略有增加，但在現代儲存條件下通常可忽略。
2.  **建構函式相容性**：在為 `BrailleWord` 加入無參數建構函式時，需確保不會破壞現有的物件初始化邏輯，特別是 `CellList` 等集合屬性的初始化。
3.  **特殊字元處理**：YAML 對某些特殊字元（如 `:` 或 `-` 開頭的字串）敏感，序列化時需確保字串被正確引用 (Quoted)，以避免解析錯誤。
4.  **遷移策略**：
    *   建議初期採取並行策略：同時支援讀取 `.brx` (JSON) 和新格式 (`.byml`)。
    *   儲存時可提供選項，或預設改用 `.byml` 檔案格式。
