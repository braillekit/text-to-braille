using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyBrailleEdit.Services;
using Xunit;

namespace EasyBrailleEdit.Tests.Services
{
    /// <summary>
    /// 記憶體洩漏測試
    /// </summary>
    public class MemoryLeakTests
    {
        static MemoryLeakTests()
        {
            // 預熱：先執行一次轉換以確保靜態資源（如 ZhuyinReverseConversionProvider）已載入
            // 這能避免靜態資源的初始化被誤判為記憶體洩漏
            try 
            {
                using var warmupConverter = new InProcessBrailleConverter();
                warmupConverter.ConvertAsync("warmup", 10, Array.Empty<string>()).Wait();
            }
            catch
            {
                // 忽略預熱過程的錯誤
            }
        }

        /// <summary>
        /// 測試重複建立和釋放轉換器是否會造成記憶體洩漏
        /// </summary>
        [StaFact]
        public async Task RepeatedConverterCreation_ShouldNotLeakMemory()
        {
            // Arrange
            const int iterations = 100;
            string content = "測試文字內容";
            int cellsPerLine = 40;
            string[] phraseFiles = Array.Empty<string>();

            // 記錄初始記憶體
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            long initialMemory = GC.GetTotalMemory(true);

            // Act - 重複建立和釋放轉換器
            for (int i = 0; i < iterations; i++)
            {
                using var converter = new InProcessBrailleConverter();
                var result = await converter.ConvertAsync(content, cellsPerLine, phraseFiles);
                
                // 清理輸出檔案
                if (result.OutputFilePath != null && System.IO.File.Exists(result.OutputFilePath))
                {
                    System.IO.File.Delete(result.OutputFilePath);
                }
            }

            // 強制垃圾回收
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            long finalMemory = GC.GetTotalMemory(true);

            // Assert - 記憶體增長應該在合理範圍內（小於 20MB）
            long memoryGrowth = finalMemory - initialMemory;
            const long maxAcceptableGrowth = 20 * 1024 * 1024; // 20 MB
            
            Assert.True(memoryGrowth < maxAcceptableGrowth, 
                $"記憶體增長過大: {memoryGrowth / 1024 / 1024} MB (允許: {maxAcceptableGrowth / 1024 / 1024} MB)");
        }

        /// <summary>
        /// 測試單一轉換器重複使用是否會造成記憶體洩漏
        /// </summary>
        [StaFact]
        public async Task RepeatedConversion_WithSameConverter_ShouldNotLeakMemory()
        {
            // Arrange
            const int iterations = 50;
            string content = "測試文字內容";
            int cellsPerLine = 40;
            string[] phraseFiles = Array.Empty<string>();

            // 記錄初始記憶體
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            long initialMemory = GC.GetTotalMemory(true);

            // Act - 使用同一個轉換器重複轉換
            using var converter = new InProcessBrailleConverter();
            for (int i = 0; i < iterations; i++)
            {
                var result = await converter.ConvertAsync(content, cellsPerLine, phraseFiles);
                
                // 清理輸出檔案
                if (result.OutputFilePath != null && System.IO.File.Exists(result.OutputFilePath))
                {
                    System.IO.File.Delete(result.OutputFilePath);
                }
            }

            // 強制垃圾回收
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            long finalMemory = GC.GetTotalMemory(true);

            // Assert
            long memoryGrowth = finalMemory - initialMemory;
            const long maxAcceptableGrowth = 5 * 1024 * 1024; // 5 MB
            
            Assert.True(memoryGrowth < maxAcceptableGrowth, 
                $"記憶體增長過大: {memoryGrowth / 1024 / 1024} MB (允許: {maxAcceptableGrowth / 1024 / 1024} MB)");
        }

        /// <summary>
        /// 測試大型文件轉換後記憶體是否能被正確釋放
        /// </summary>
        [StaFact]
        public async Task LargeDocumentConversion_ShouldReleaseMemory()
        {
            // Arrange
            // 建立一個較大的文件（約 1000 行）
            var lines = new List<string>();
            for (int i = 0; i < 1000; i++)
            {
                lines.Add($"這是第 {i + 1} 行測試文字，包含中文和 English 混合內容。");
            }
            string content = string.Join("\r\n", lines);
            int cellsPerLine = 40;
            string[] phraseFiles = Array.Empty<string>();

            // 記錄初始記憶體
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            long initialMemory = GC.GetTotalMemory(true);

            // Act
            using (var converter = new InProcessBrailleConverter())
            {
                var result = await converter.ConvertAsync(content, cellsPerLine, phraseFiles);
                
                // 清理輸出檔案
                if (result.OutputFilePath != null && System.IO.File.Exists(result.OutputFilePath))
                {
                    System.IO.File.Delete(result.OutputFilePath);
                }
            }

            // 強制垃圾回收
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            long finalMemory = GC.GetTotalMemory(true);

            // Assert - 大型文件轉換會使用較多記憶體，允許較大的誤差（50MB）
            long memoryGrowth = finalMemory - initialMemory;
            const long maxAcceptableGrowth = 50 * 1024 * 1024; // 50 MB
            
            Assert.True(memoryGrowth < maxAcceptableGrowth, 
                $"記憶體未正確釋放: {memoryGrowth / 1024 / 1024} MB (允許: {maxAcceptableGrowth / 1024 / 1024} MB)");
        }

        /// <summary>
        /// 測試轉換器是否正確實作 IDisposable
        /// </summary>
        [StaFact]
        public void Converter_ShouldImplementIDisposable()
        {
            // Arrange & Act
            var converter = new InProcessBrailleConverter();

            // Assert
            Assert.IsAssignableFrom<IDisposable>(converter);
            
            // 確保可以呼叫 Dispose
            converter.Dispose();
            
            // 多次呼叫 Dispose 不應該拋出異常
            converter.Dispose();
        }

        /// <summary>
        /// 測試並行轉換是否會造成記憶體問題
        /// </summary>
        [StaFact]
        public async Task ParallelConversion_ShouldNotLeakMemory()
        {
            // Arrange
            const int parallelCount = 10;
            string content = "測試文字內容";
            int cellsPerLine = 40;
            string[] phraseFiles = Array.Empty<string>();

            // 記錄初始記憶體
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            long initialMemory = GC.GetTotalMemory(true);

            // Act - 循序建立多個轉換器（避免檔案衝突）
            // 注意：InProcessBrailleConverter 使用固定的輸出檔名，不適合並行測試
            // 這裡改為循序測試，但仍然測試多個實例的記憶體管理
            for (int i = 0; i < parallelCount; i++)
            {
                using var converter = new InProcessBrailleConverter();
                var result = await converter.ConvertAsync(content, cellsPerLine, phraseFiles);
                
                // 清理輸出檔案
                if (result.OutputFilePath != null && System.IO.File.Exists(result.OutputFilePath))
                {
                    System.IO.File.Delete(result.OutputFilePath);
                }
            }

            // 強制垃圾回收
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            long finalMemory = GC.GetTotalMemory(true);

            // Assert
            long memoryGrowth = finalMemory - initialMemory;
            const long maxAcceptableGrowth = 10 * 1024 * 1024; // 10 MB
            
            Assert.True(memoryGrowth < maxAcceptableGrowth, 
                $"記憶體增長過大: {memoryGrowth / 1024 / 1024} MB (允許: {maxAcceptableGrowth / 1024 / 1024} MB)");
        }
    }
}
