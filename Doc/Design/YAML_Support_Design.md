# 支援 YAML 檔案格式設計草案

> 由 Gemini CLI 生成。

## 1. 專案目標

本設計旨在為 `EasyBrailleEditApp` 引入一種新的檔案儲存格式：**YAML**。目前的 `.brx` 檔案格式基於 JSON，雖然能夠完整保存物件狀態，但其可讀性較差，且不易於使用一般文字編輯器進行檢視。新的 YAML 格式應在保持資料完整性（Full Object State Preservation）的前提下，顯著提升檔案的人眼可讀性 (Human-Readability)。

## 2. 格式選擇理由

在評估了 JSON、Markdown、XML、TOML 與 YAML 等格式後，選擇 YAML 作為新格式的主要理由如下：

1. **完整性 (Data Integrity)**：YAML 是 JSON 的超集，能夠完美對應 `BrailleDocument` -> `BrailleLine` -> `BrailleWord` 的巢狀物件結構，確保所有屬性（如 `IsPolyphonic`, `PhoneticCode`, `CellList`）都能被精確保存與還原。
2. **可讀性 (Readability)**：
    * 去除大量括號 (`{`, `}`, `[`, `]`) 與引號 (`"`)，畫面簡潔。
    * 使用縮排 (Indentation) 表示層級，符合人類直覺。
    * 支援多行字串，對於長文本的呈現優於 JSON。
3. **編輯友善**：雖然不預期使用者手動修改複雜屬性，但 YAML 格式允許使用者在必要時使用記事本等工具查看內容，且易於進行版本控制 (Git diff)。

## 3. 技術選型

建議使用 **YamlDotNet** 函式庫進行實作。

* **理由**：它是 .NET 生態系中最成熟、功能最完整的 YAML 處理函式庫，支援將 YAML 文件直接序列化/反序列化為 .NET 物件 (POCO)，且社群活躍度高。

## 4. 實作建議與重構需求

為了順利整合 YAML 序列化功能，現有的資料模型 (Data Models) 需要進行以下重構：

### 4.1 增加無參數建構函式 (Parameterless Constructors) (已完成)

目前的序列化機制 (`DataContractJsonSerializer`) 可繞過建構函式直接建立物件，但現代序列化工具（包括 `System.Text.Json` 和 `YamlDotNet`）通常要求目標類別具備**無參數建構函式**。此項重構已完成。

**受影響類別及行動：**

* **`BrailleWord`**：已新增一個 `public` 無參數建構函式，確保序列化庫能夠存取。
* **`BraillePageTitle`**：已將 `private` 無參數建構函式改為 `public`，確保序列化庫能夠存取。（雖然 `YamlDotNet` 應該可以存取 `private` 無參數建構函式）

### 4.2 屬性標籤 (Attributes)

* 目前的 `[DataMember(Name="...")]` 是針對 `DataContractJsonSerializer` 設計的。
* 若使用 `YamlDotNet`，預設會使用屬性名稱作為 Key。若需自定義名稱，需使用 `[YamlMember(Alias = "...")]`。
* **建議**：評估是否需要引入 `YamlDotNet` 的屬性依賴，或者直接使用預設命名規則（通常 PascalCase）。

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

1. **檔案大小**：YAML 雖然減少了括號，但增加了縮排空白。對於大型文件，檔案大小可能會略有增加，但在現代儲存條件下通常可忽略。
2. **建構函式相容性**：在為 `BrailleWord` 加入無參數建構函式時，需確保不會破壞現有的物件初始化邏輯，特別是 `CellList` 等集合屬性的初始化。
3. **特殊字元處理**：YAML 對某些特殊字元（如 `:` 或 `-` 開頭的字串）敏感，序列化時需確保字串被正確引用 (Quoted)，以避免解析錯誤。
4. **遷移策略**：
    * 建議初期採取並行策略：同時支援讀取 `.brx` (JSON) 和新格式 (`.byml`)。
    * 儲存時可提供選項，或預設改用 `.byml` 檔案格式。

## 7. 實作紀錄 (2025/11/20)

根據上述設計，已於 2025/11/20 完成核心函式庫 (`BrailleToolkit`) 的實作。詳細技術決策如下：

### 7.1 套件與依賴
*   **YamlDotNet**: 版本 `16.3.0`。
*   已將套件安裝至 `BrailleToolkit` 與 `BrailleToolkit.Tests` 專案。

### 7.2 關鍵技術解決方案

#### 7.2.1 處理 Flyweight 模式 (`BrailleCell`)
*   **問題**: `BrailleCell` 類別使用了享元模式 (Flyweight Pattern)，建構函式為 `private`，且物件實體應透過 `BrailleCell.GetInstance(byte value)` 取得，這導致預設的 YAML 反序列化器無法直接建立物件。
*   **解法**: 實作了自定義的 `IYamlTypeConverter`，即 `BrailleCellYamlTypeConverter`。
    *   **序列化**: 將 `BrailleCell` 簡化輸出為 `{ Value: 123 }` 的形式，甚至可以更簡潔。目前的實作保留了物件結構以便擴充，但確保輸出的是 `Value` 值。
    *   **反序列化**: 讀取 `Value` 後，呼叫 `BrailleCell.GetInstance` 來取得正確的參照，確保記憶體與邏輯的一致性。

#### 7.2.2 資料模型調整 (Data Model Refactoring)
為了支援反序列化時的屬性注入，對核心類別進行了微調，同時盡量保持封裝性：
*   **`BrailleDocument.Lines`**: 加入 `private set` (原本僅有 get)，允許反序列化器填入集合。
*   **`BrailleLine.Words`**: 將 setter 改為 `private` (原本是 public，但在反序列化時 private 即可滿足需求且更安全)。
*   **`BrailleWord.ActivePhoneticIndex`**: 加上 `[YamlIgnore]` 屬性。
    *   **原因**: 此屬性的值依賴 `PhoneticCodes` 列表的內容。在反序列化過程中，無法保證屬性的賦值順序，且此狀態屬於執行期間的動態選擇，不一定要持久化。

#### 7.2.3 Bug 修正 (`Equals` 方法)
在撰寫單元測試的過程中，發現並修正了以下類別的 `Equals(object obj)` 實作瑕疵：
*   **受影響類別**: `BrailleWord`, `BrailleCell`, `BrailleCellList`。
*   **問題**: 原本的實作直接進行強制轉型 `(Type)obj`，當傳入不同型別的物件時會拋出 `InvalidCastException`。
*   **修正**: 改為標準的 `obj as Type` 或 `obj is Type` 檢查，若型別不符則回傳 `false`。

#### 7.2.4 序列化設定
*   **DisableAliases**: 在 `SerializerBuilder` 中啟用了 `.DisableAliases()`。
    *   **原因**: 預設情況下，YamlDotNet 會偵測重複引用的物件並使用錨點 (Anchors, 如 `&o0`, `*o0`) 來縮減檔案大小。但在我們的案例中 (如多個相同的點字方)，這會降低檔案的人眼可讀性。禁用後可確保每個物件都完整展開，雖然檔案稍大，但更符合「可讀性」的設計目標。

### 7.3 測試驗證
*   已建立 `YamlSerializationTests` 單元測試。
*   測試涵蓋了 `BrailleCell`、`BrailleWord`、`BrailleLine` 到完整 `BrailleDocument` 的 Round-Trip (序列化 -> 反序列化 -> 比對) 測試。
*   確保中文字、注音、多音字標記以及點字方陣列都能正確還原。