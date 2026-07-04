# Benchmark 結果：Immutable Refactoring Phase 1 A/B 重跑

- 日期：2026-04-05
- 比較方式：clean worktree A/B 重跑
- `pre-Phase 1` commit：`7459cde`
- `Phase 1` commit：`e1f86b4`
- 指令：`dotnet run --project src/EasyBrailleEditApp/BrailleToolkit.Benchmarks/BrailleToolkit.Benchmarks.csproj -c Release`

## 環境

- OS: Windows 11 (10.0.26200.8117)
- CPU: 12th Gen Intel Core i7-12700H, 1 CPU, 20 logical and 14 physical cores
- .NET SDK: 10.0.201
- Runtime: .NET 10.0.5 (10.0.526.15411), X64 RyuJIT AVX2
- BenchmarkDotNet: v0.14.0

## 說明

- 這次重跑的目的，是驗證第一次量測中出現的巨大回歸是否真的是 Phase 1 造成。
- 兩個 commit 都在乾淨 worktree 中獨立建置與執行，避免工作目錄狀態或後續文件修改影響結果。

## 結果

| Method | Pre-Phase 1 Mean | Phase 1 Mean | 差異 | 變化比例 | Pre-Phase 1 Allocated | Phase 1 Allocated |
| ---- | ----: | ----: | ----: | ----: | ----: | ----: |
| 中文單行轉換 | 172.2 μs | 171.0 μs | -1.2 μs | -0.70% | 72.84 KB | 72.84 KB |
| 英文單行轉換 | 785.7 μs | 782.3 μs | -3.4 μs | -0.43% | 169.33 KB | 169.33 KB |
| 中英混合單行轉換 | 926.2 μs | 918.7 μs | -7.5 μs | -0.81% | 374.43 KB | 374.43 KB |
| 中文多行轉換 | 9,972.1 μs | 9,727.5 μs | -244.6 μs | -2.45% | 4301.49 KB | 4301.51 KB |
| 英文多行轉換 | 4,763.8 μs | 4,698.9 μs | -64.9 μs | -1.36% | 1041.13 KB | 1041.13 KB |
| 中英混合多行轉換 | 4,082.3 μs | 3,979.7 μs | -102.6 μs | -2.51% | 1346.76 KB | 1346.76 KB |
| 長中文字串轉換 | 10,207.5 μs | 9,955.9 μs | -251.6 μs | -2.46% | 4303.14 KB | 4303.14 KB |

## 分析

- A/B 重跑沒有重現第一次量測看到的 `+60% ~ +70%` 回歸。
- 中文路徑與混合多行路徑在這次重跑中反而略快，變化約在 0.4% 到 2.5% 之間。
- 所有案例的 managed allocation 幾乎不變，表示 Phase 1 不是以降低配置量為主要收益。
- 綜合 benchmark 與程式路徑檢查，目前沒有證據顯示 `FrozenDictionary` / `FrozenSet` 是中文轉換熱路徑的效能瓶頸。

## 結論

目前應將這次 A/B 重跑視為較可靠的結果。Phase 1 沒有被證明造成中文轉換效能回歸；第一次量測比較可能是 baseline 不可直接比對，或當時量測條件不同所造成的偏差。
