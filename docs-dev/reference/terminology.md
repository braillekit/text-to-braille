# 點字系統術語字典

本文件定義「易點雙視」系統中使用的點字相關術語，提供中英文對照和詳細說明。

## 基本概念

### 點字結構

| 中文 | 英文 | 說明 |
|------|------|------|
| 點 | Dot | 點字方上的凸點，每個點字方最多有 6 個點 |
| 點位 | Dot Position | 點的位置，編號為 1-6（左上到右下） |
| 方 | Cell | 一個完整的點字單位，包含 1-6 個點 |
| 點字碼 | Braille Code | 點字方的數位表示，用十六進位表示點位組合 |
| 空方 | Blank Cell | 沒有任何點的點字方，用於空格 |

### 文件結構

| 中文 | 英文 | 說明 |
|------|------|------|
| 明眼字 | Regular Text / Sighted Text | 一般人閱讀的文字（相對於點字） |
| 點字 | Braille | 視障者使用的觸覺文字系統 |
| 雙視文件 | Dual-view Document | 同時包含明眼字和點字的文件 |
| 點字行 | Braille Line | 一行點字，由多個點字詞組成 |
| 點字詞 | Braille Word | 一個邏輯單位，如一個中文字或一個英文字 |
| 點字文件 | Braille Document | 完整的點字文件，包含多行點字 |

### 排版相關

| 中文 | 英文 | 說明 |
|------|------|------|
| 方數 | Cell Count | 一個符號或詞語占用的點字方數量 |
| 每列方數 | Cells Per Line | 每一列點字的方數限制（通常為 32 或 40） |
| 斷行 | Line Break | 在適當位置將長文字分成多行 |
| 二方連書 | Must Joined | 不得分行點寫的兩個點字方 |

## 轉換相關

### 轉換元件

| 中文 | 英文 | 說明 |
|------|------|------|
| 轉換器 | Converter | 將特定類型文字轉換成點字的元件 |
| 點字處理器 | Braille Processor | 協調所有轉換器的核心元件 |
| 點字對照表 | Braille Table | 字元與點字碼的對應表（XML 格式） |
| 詞庫 | Phrase Dictionary | 儲存常用詞彙及其正確讀音的檔案 |

### 中文點字

| 中文 | 英文 | 說明 |
|------|------|------|
| 注音符號 | Phonetic Symbols | 中文發音的符號系統（ㄅㄆㄇㄈ...） |
| 聲母 | Initial Consonant | 中文音節的起始輔音（ㄅ、ㄆ、ㄇ...） |
| 韻母 | Final | 中文音節的韻尾（ㄚ、ㄛ、ㄜ...） |
| 介音 | Medial | 聲母和韻母之間的過渡音（ㄧ、ㄨ、ㄩ） |
| 結合韻 | Joined Vowel | 兩個或多個韻母的組合（如 ㄨㄛ） |
| 聲調 | Tone | 中文字的音調（一聲、二聲、三聲、四聲、輕聲） |
| 破音字 | Polyphonic Character | 有多種讀音的中文字（如「不」、「一」） |
| 特殊單音 | Special Monosyllable | 七個特殊的單音符號（ㄓㄔㄕㄖㄗㄘㄙ） |

### 英文點字

| 中文 | 英文 | 說明 |
|------|------|------|
| UEB | Unified English Braille | 統一英語點字，英文點字的國際標準 |
| 縮寫 | Contraction | 英文點字中的簡寫形式 |
| Grade 1 | Grade 1 | 逐字翻譯的英文點字（不使用縮寫） |
| Grade 2 | Grade 2 | 使用縮寫的英文點字 |
| 大寫符號 | Capital Sign | 表示下一個字母為大寫的點字符號 |
| 數字符號 | Number Sign | 表示後續為數字的點字符號 |

## 標籤與語法

### 情境標籤

| 中文 | 英文 | 說明 |
|------|------|------|
| 情境標籤 | Context Tag | 定義特殊轉換規則的標籤（如 `<math>`, `<table>`） |
| 標題標籤 | Title Tag | `<標題>...</標題>` |
| 數學標籤 | Math Tag | `<math>...</math>` |
| 表格標籤 | Table Tag | `<表格>...</表格>` |
| 分數標籤 | Fraction Tag | `<分數>...</分數>` |
| 頁碼標籤 | Page Number Tag | `<P>...</P>` |

### 特殊符號

| 中文 | 英文 | 說明 |
|------|------|------|
| 私名號 | Personal Name Mark | 標示人名或地名的符號 |
| 書名號 | Book Title Mark | 標示書名的符號 |
| 音界號 | Syllable Separator | 分隔音節的符號 |
| 破折號 | Dash | `──` |
| 刪節號 | Ellipsis | `......` |

## 點字規則

### 中文點字規則

| 中文 | 英文 | 說明 |
|------|------|------|
| 遇「我」字空一方 | Add Blank Before "I" | 冒號後接「我」字時，中間須空一方 |
| 遇前引號空一方 | Add Blank Before Left Quote | 後引號「」」後接前引號「「」時須空一方 |
| 句號後空方 | Add Blank After Period | 句號「。」後接文字須空一方（除非遇標點） |
| 括弧規則 | Bracket Rule | 右括弧後面一定要空方 |

### 英文點字規則

| 中文 | 英文 | 說明 |
|------|------|------|
| 連續大寫 | Consecutive Capitals | 連續大寫字母前加兩個大寫符號 |
| 數字與文字間空方 | Blank Between Number and Text | 數字和其他文字符號之間要空方 |
| 行尾連字符號 | End-of-line Hyphen | 英文單字在行尾被切斷時要加連字符號 |

## 系統架構

### 設計模式

| 中文 | 英文 | 說明 |
|------|------|------|
| 策略模式 | Strategy Pattern | 轉換系統採用的設計模式 |
| 轉換器介面 | Converter Interface | `IWordConverter`，定義轉換器的標準介面 |
| 後處理規則 | Post-processing Rules | 轉換完成後套用的點字規則 |
| 貪婪演算法 | Greedy Algorithm | 英文轉換器使用的匹配策略 |

### 資料模型

| 中文 | 英文 | 對應類別 |
|------|------|---------|
| 點字文件 | Braille Document | `BrailleDocument` |
| 點字行 | Braille Line | `BrailleLine` |
| 點字詞 | Braille Word | `BrailleWord` |
| 點字方 | Braille Cell | `BrailleCell` |
| 頁標題 | Page Title | `BraillePageTitle` |

## 檔案格式

| 中文 | 英文 | 副檔名 | 說明 |
|------|------|--------|------|
| 雙視檔案（JSON） | Dual-view File (JSON) | `.brlj` | 新版的雙視文件格式（JSON） |
| 雙視檔案（XML） | Dual-view File (XML) | `.brx` | 舊版的雙視文件格式（XML） |
| 點字對照表 | Braille Table | `.xml` | 字元與點字碼的對應表 |
| 詞庫檔案 | Phrase File | `.txt` | 儲存詞彙和讀音的純文字檔 |

## 相關文件

- [點字轉換處理流程](../design/architecture/conversion-process.md) - 了解轉換系統架構
- [點字字型對應表](../design/features/reference/braille-font-table.md) - 查看完整的點字碼對應
- [詞庫檔案設計](../design/features/reference/phrases.md) - 了解詞庫機制
