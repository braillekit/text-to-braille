# Immutable Design

## Step 1

我要對此專案進行效能優化。我的計畫是加入 C# 的 immutable types。在此之前，我要先了解此專案是否已經有效能測試專案，因為有效能測試數據，才能比較修改前後的效能差異。效能測試主要是針對點字轉換的效率。

### Claude response

此專案目前沒有效能測試（benchmark）專案。

現有的測試專案：

- BrailleToolkit.Tests — 單元測試
- EasyBrailleEdit.Tests — 單元測試

這些都是功能性的單元測試，沒有引用 BenchmarkDotNet 或任何效能測試框架。

不過，專案中有一份先前的效能分析文件 converter_memory_performance_analysis.md，裡面以靜態分析方式指出了幾項效能瓶頸（字串操作效率、Hashtable vs Dictionary 等），但並非透過實際 benchmark 測量的數據。

建議下一步： 在開始引入 immutable types 之前，先建立一個 BenchmarkDotNet 效能測試專案，針對 BrailleProcessor 的點字轉換流程撰寫 benchmark，取得修改前的基準數據。這樣才能量化 immutable types 帶來的效能變化。需要我來建立這個 benchmark 專案嗎？

