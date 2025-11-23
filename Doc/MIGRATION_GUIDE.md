# Doc 目錄重組遷移指南

> 本文件記錄了 Doc 目錄重組的變更，幫助開發者快速找到檔案的新位置。

## 新舊目錄對應表

### 頂層變更

| 舊位置 | 新位置 | 說明 |
|---|---|---|
| `Doc/ChangeLog-before-3.2.txt` | `Doc/CHANGELOG.md` | 整合至 CHANGELOG |
| `Doc/ChangeLog-before-v5.txt` | `Doc/CHANGELOG.md` | 整合至 CHANGELOG |
| `Doc/ReleaseNote.txt` | `Doc/v5.0.0-alpha.md` | 轉換為 Markdown |
| `Doc/Design/` | `Doc/design/` | 目錄名稱改為小寫 |
| `Doc/Manual/` | `Doc/manual/` | 目錄名稱改為小寫 |
| `Doc/範例程式/` | `Doc/technical/development/examples/` | 移至技術文件 |

### 技術文件重組

| 舊位置 | 新位置 |
|---|---|
| `Doc/技術文件/程式設計相關/` | `Doc/technical/development/` |
| `Doc/技術文件/點字規則/` | `Doc/technical/braille-rules/` |
| `Doc/技術文件/Braille Printers/` | `Doc/technical/printers/` |
| `Doc/技術文件/分析設計/` | `Doc/design/architecture/` 和 `Doc/design/features/` |
| `Doc/技術文件/部署程序.txt` | `Doc/technical/deployment.md` |
| `Doc/技術文件/轉點字的測試案例.docx` | `Doc/technical/testing/` |

### 設計文件分類

| 舊位置 | 新位置 | 類別 |
|---|---|---|
| `分析設計/BrailleModel.uml` | `design/architecture/` | 架構 |
| `分析設計/類別圖.png` | `design/architecture/` | 架構 |
| `分析設計/點字轉換活動圖.png` | `design/architecture/` | 架構 |
| `分析設計/點字轉換處理流程.doc` | `design/architecture/` | 架構 |
| `分析設計/語法-*.docx` | `design/features/` | 功能設計 |
| `分析設計/其他 .txt, .doc 檔案` | `design/features/` | 功能設計 |

### 使用者手冊版本化

| 舊位置 | 新位置 | 說明 |
|---|---|---|
| `Manual/使用手冊/使用手冊.docx` | `manual/user-guide/current/` | 當前版本 |
| `Manual/使用手冊/使用手冊.pdf` | `manual/user-guide/current/` | 當前版本 |
| `Manual/使用手冊/使用手冊 v2.7.*` | `manual/user-guide/archive/` | 歷史版本 |
| `Manual/使用手冊/使用手冊 v3.18.pdf` | `manual/user-guide/archive/` | 歷史版本 |
| `Manual/安裝手冊/*.htm, *.gif` | `manual/installation/current/` | 當前版本 |
| `Manual/安裝手冊/安裝手冊 v2.7.*` | `manual/installation/archive/` | 歷史版本 |

## 新目錄結構

```
Doc/
├── README.md                      # 文件導覽索引 (新增)
├── releases/                      # 發行紀錄 (新)
│   ├── CHANGELOG.md              # 完整變更歷史 (整合)
│   └── v5.0.0-alpha.md           # 當前版本發行紀錄
├── manual/                        # 使用者手冊
│   ├── user-guide/
│   │   ├── current/
│   │   └── archive/
│   └── installation/
│       ├── current/
│       └── archive/
├── design/                        # 設計文件
│   ├── architecture/             # 架構設計
│   ├── features/                 # 功能設計
│   └── YAML_Support_Design.md
├── technical/                     # 技術文件
│   ├── development/              # 開發相關
│   │   ├── auto-updater.md
│   │   ├── build-output-merge.md
│   │   ├── 新符號加入時的修改步驟.md
│   │   ├── SourceGrid 筆記.txt
│   │   └── examples/             # 程式碼範例
│   │       └── examples.md
│   ├── braille-rules/            # 點字規則
│   │   ├── chinese/
│   │   └── UEB/
│   ├── printers/                 # 印表機相關
│   ├── testing/                  # 測試相關
│   └── deployment.md             # 部署程序
├── Design/                        # (舊目錄，待清理)
├── Manual/                        # (舊目錄，待清理)
└── 技術文件/                      # (舊目錄，待清理)
```

## 下一步清理工作

> [!CAUTION]
> 以下清理工作需謹慎執行，建議先確認所有檔案都已正確複製到新位置。

在確認所有檔案都已正確移動後，可以刪除以下舊目錄：

```bash
# 確認新結構無誤後再執行
Remove-Item "Doc\Design" -Recurse -Force
Remove-Item "Doc\Manual" -Recurse -Force
Remove-Item "Doc\技術文件" -Recurse -Force
Remove-Item "Doc\範例程式" -Recurse -Force
Remove-Item "Doc\ChangeLog-before-3.2.txt"
Remove-Item "Doc\ChangeLog-before-v5.txt"
Remove-Item "Doc\ReleaseNote.txt"
```

## 優勢

新的目錄結構帶來以下優勢：

1. **更清晰的分類**: 區分使用者文件、技術文件、設計文件
2. **版本管理**: 手冊有明確的當前版本和歷史版本分離
3. **國際化友善**: 使用英文目錄名稱，便於協作
4. **可維護性**: 統一使用 Markdown 格式，便於版本控制
5. **導覽性**: 新增 README.md 提供清晰的文件地圖
