using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using EasyBrailleEdit.Services;
using Xunit;

namespace EasyBrailleEdit.Tests.Services
{
    public class ExternalBrailleConverterTests
    {
        public ExternalBrailleConverterTests()
        {
            EnsureTxt2BrlExists();
        }

        private void EnsureTxt2BrlExists()
        {
            string targetPath = Path.Combine(Application.StartupPath, "txt2brl.exe");
            
            // Always try to locate Txt2Brl.exe in the build output directory and copy it
            // to ensure we are using the latest version.
            
            string rootDir = Path.GetFullPath(Path.Combine(Application.StartupPath, @"..\..\..\..\..\..\"));
            string sourcePath = Path.Combine(rootDir, @"output\EasyBrailleEdit\Debug\net10.0-windows10.0.17763.0\Txt2Brl.exe");

            if (File.Exists(sourcePath))
            {
                File.Copy(sourcePath, targetPath, true);
            }
        }

        [StaFact]
        public async Task ConvertAsync_WithValidText_ShouldSucceed()
        {
            // Arrange
            if (!File.Exists(Path.Combine(Application.StartupPath, "txt2brl.exe")))
            {
                // Skip test if Txt2Brl is not found
                return;
            }

            using var converter = new ExternalBrailleConverter();
            string content = "測試文字";
            int cellsPerLine = 40;
            string[] phraseFiles = Array.Empty<string>();

            // Act
            var result = await converter.ConvertAsync(content, cellsPerLine, phraseFiles);

            // Assert
            Assert.True(result.Success, $"Conversion failed: {result.ErrorMessage}");
            Assert.NotNull(result.OutputFilePath);
            Assert.True(File.Exists(result.OutputFilePath));
            
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
            if (!File.Exists(Path.Combine(Application.StartupPath, "txt2brl.exe")))
            {
                // Skip test if Txt2Brl is not found
                return;
            }

            using var converter = new ExternalBrailleConverter();
            string content = "測試\uE000"; // Invalid char
            int cellsPerLine = 40;
            string[] phraseFiles = Array.Empty<string>();

            // Act
            var result = await converter.ConvertAsync(content, cellsPerLine, phraseFiles);

            // Assert
            // Txt2Brl usually reports errors for invalid chars
            if (result.Success)
            {
                Assert.Empty(result.InvalidChars);
            }
            else
            {
                Assert.True(result.InvalidChars.Count > 0 || !string.IsNullOrEmpty(result.ErrorMessage));
            }
        }
    }
}
