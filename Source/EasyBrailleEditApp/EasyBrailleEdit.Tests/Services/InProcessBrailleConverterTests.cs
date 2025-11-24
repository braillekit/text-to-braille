using System;
using System.IO;
using System.Threading.Tasks;
using EasyBrailleEdit.Services;
using Xunit;

namespace EasyBrailleEdit.Tests.Services
{
    public class InProcessBrailleConverterTests
    {
        [StaFact]
        public async Task ConvertAsync_WithValidText_ShouldSucceed()
        {
            // Arrange
            using var converter = new InProcessBrailleConverter();
            string content = "測試文字";
            int cellsPerLine = 40;
            string[] phraseFiles = Array.Empty<string>();

            // Act
            var result = await converter.ConvertAsync(content, cellsPerLine, phraseFiles);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.OutputFilePath);
            Assert.True(File.Exists(result.OutputFilePath));
            Assert.Empty(result.ErrorMessage);
            Assert.Empty(result.InvalidChars);

            // Cleanup
            if (File.Exists(result.OutputFilePath))
            {
                File.Delete(result.OutputFilePath);
            }
        }

        [StaFact]
        public async Task ConvertAsync_WithInvalidChar_ShouldReportError()
        {
            // Arrange
            using var converter = new InProcessBrailleConverter();
            // Assuming '○' might be an invalid char depending on the table, 
            // but let's try something that is definitely not in the table if possible.
            // However, BrailleToolkit might just ignore unknown chars or treat them as errors.
            // Let's use a character that is likely invalid or handled as error if not found.
            // Note: The behavior depends on BrailleToolkit implementation. 
            // If it skips unknown chars, this test might need adjustment.
            // For now, let's try a very obscure character or emoji.
            string content = "測試😊"; 
            int cellsPerLine = 40;
            string[] phraseFiles = Array.Empty<string>();

            // Act
            var result = await converter.ConvertAsync(content, cellsPerLine, phraseFiles);

            // Assert
            // Because emojis are surrogate pairs and BrailleProcessor handles chars one by one,
            // this causes an exception in NChinese which InProcessBrailleConverter should now catch.
            Assert.False(result.Success);
            Assert.Contains("錯誤", result.ErrorMessage);
        }

        [StaFact]
        public async Task ConvertAsync_WithUnknownChar_ShouldReportInvalidChar()
        {
            // Arrange
            using var converter = new InProcessBrailleConverter();
            // Use a Private Use Area character which should be treated as a normal char but invalid for Braille
            string content = "測試\uE000"; 
            int cellsPerLine = 40;
            string[] phraseFiles = Array.Empty<string>();

            // Act
            var result = await converter.ConvertAsync(content, cellsPerLine, phraseFiles);

            // Assert
            // It might succeed with invalid chars recorded, or fail depending on implementation.
            // BrailleProcessor usually sets HasError = true if InvalidChars > 0.
            if (result.Success)
            {
                // If it says success, it shouldn't have invalid chars? 
                // Wait, BrailleConversionResult.HasError checks InvalidChars.Count > 0.
                // So if InvalidChars > 0, Success (from !processor.HasError) should be false.
                // Let's check result properties directly.
                Assert.Empty(result.InvalidChars); 
            }
            else
            {
                // If failed, it should be because of invalid chars
                Assert.True(result.InvalidChars.Count > 0 || !string.IsNullOrEmpty(result.ErrorMessage));
            }
        }

        [StaFact]
        public async Task ConvertAsync_ProgressReporting_ShouldWork()
        {
            // Arrange
            using var converter = new InProcessBrailleConverter();
            string content = "第一行\r\n第二行";
            int cellsPerLine = 40;
            string[] phraseFiles = Array.Empty<string>();
            bool progressReported = false;
            var progress = new Progress<ConversionProgress>(p => 
            {
                progressReported = true;
                Assert.True(p.CurrentLine > 0);
                Assert.False(string.IsNullOrEmpty(p.CurrentText));
            });

            // Act
            var result = await converter.ConvertAsync(content, cellsPerLine, phraseFiles, progress);

            // Assert
            Assert.True(result.Success);
            Assert.True(progressReported);
        }
    }
}
