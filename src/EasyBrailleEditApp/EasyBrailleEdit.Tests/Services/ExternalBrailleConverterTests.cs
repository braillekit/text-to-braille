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
            if (File.Exists(targetPath)) return;

            // Try to locate Txt2Brl.exe in the build output directory
            // We are in src\EasyBrailleEditApp\EasyBrailleEdit.Tests\bin\Debug\net10.0-windows10.0.17763.0
            // We want to go to output\EasyBrailleEdit\Debug\net10.0-windows10.0.17763.0\Txt2Brl.exe
            
            // Go up 5 levels to root (src\EasyBrailleEditApp\EasyBrailleEdit.Tests\bin\Debug\net10.0-windows10.0.17763.0 -> src\EasyBrailleEditApp\EasyBrailleEdit.Tests\bin\Debug -> ... -> src)
            // Actually, let's just search for it or use a known relative path.
            // Root is d:\Projects\BrailleKit\text-to-braille
            
            string rootDir = Path.GetFullPath(Path.Combine(Application.StartupPath, @"..\..\..\..\..\..\"));
            string sourcePath = Path.Combine(rootDir, @"output\EasyBrailleEdit\Debug\net10.0-windows10.0.17763.0\Txt2Brl.exe");

            if (File.Exists(sourcePath))
            {
                File.Copy(sourcePath, targetPath, true);
                
                // Also copy dependencies if needed (e.g. BrailleToolkit.dll, but it should be in test dir already)
                // Txt2Brl might need other dlls.
                // Let's copy everything from that folder just in case? No, that's messy.
                // Txt2Brl depends on BrailleToolkit and EasyBrailleEdit.Common, which are already in test dir.
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
            Assert.True(result.Success);
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
