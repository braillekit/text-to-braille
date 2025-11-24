using BrailleToolkit;
using EasyBrailleEdit.DualEdit;
using Xunit;

namespace EasyBrailleEdit.Tests.DualEdit
{
    /// <summary>
    /// ClipboardHelper 的整合測試。
    /// 這些測試需要 Windows Forms 環境和 STA 執行緒。
    /// </summary>
    [Trait("Category", "Integration")]
    [Trait("Category", "UI")]
    [Trait("Category", "Clipboard")]
    public class ClipboardHelperTests
    {
        /// <summary>
        /// 在每個測試前清除剪貼簿，避免測試間互相干擾。
        /// </summary>
        public ClipboardHelperTests()
        {
            // 清除剪貼簿，確保測試獨立性
            try
            {
                System.Windows.Forms.Clipboard.Clear();
            }
            catch
            {
                // 在某些環境下可能無法存取剪貼簿，忽略錯誤
            }
        }

        [StaFact]
        public void SetAndGetWords_WithValidData_ShouldRoundTripCorrectly()
        {
            // Arrange
            var originalWords = new List<BrailleWord>
            {
                new BrailleWord("測試"),
                new BrailleWord("點字")
            };

            // Act
            ClipboardHelper.SetWords(originalWords);
            var retrievedWords = ClipboardHelper.GetWords();

            // Assert
            Assert.NotNull(retrievedWords);
            Assert.Equal(2, retrievedWords.Count);
            Assert.Equal("測試", retrievedWords[0].Text);
            Assert.Equal("點字", retrievedWords[1].Text);
        }

        [StaFact]
        public void SetAndGetWords_WithPhoneticCode_ShouldPreservePhoneticCode()
        {
            // Arrange - 這是最重要的測試，驗證 PhoneticCode 不會遺失
            var originalWords = new List<BrailleWord>
            {
                new BrailleWord("測") { PhoneticCode = "ㄘㄜˋ" },
                new BrailleWord("試") { PhoneticCode = "ㄕˋ" }
            };

            // Act
            ClipboardHelper.SetWords(originalWords);
            var retrievedWords = ClipboardHelper.GetWords();

            // Assert
            Assert.NotNull(retrievedWords);
            Assert.Equal(2, retrievedWords.Count);
            Assert.Equal("測", retrievedWords[0].Text);
            Assert.Equal("ㄘㄜˋ", retrievedWords[0].PhoneticCode);
            Assert.Equal("試", retrievedWords[1].Text);
            Assert.Equal("ㄕˋ", retrievedWords[1].PhoneticCode);
        }

        [StaFact]
        public void SetAndGetWords_WithEmptyList_ShouldRoundTripCorrectly()
        {
            // Arrange
            var originalWords = new List<BrailleWord>();

            // Act
            ClipboardHelper.SetWords(originalWords);
            var retrievedWords = ClipboardHelper.GetWords();

            // Assert
            Assert.NotNull(retrievedWords);
            Assert.Empty(retrievedWords);
        }

        [StaFact]
        public void GetWords_WhenClipboardIsEmpty_ShouldReturnNull()
        {
            // Arrange
            System.Windows.Forms.Clipboard.Clear();

            // Act
            var retrievedWords = ClipboardHelper.GetWords();

            // Assert
            Assert.Null(retrievedWords);
        }

        [StaFact]
        public void SetAndGetLines_WithValidData_ShouldRoundTripCorrectly()
        {
            // Arrange
            var line1 = new BrailleLine();
            line1.Words.Add(new BrailleWord("第一行"));
            
            var line2 = new BrailleLine();
            line2.Words.Add(new BrailleWord("第二行"));

            var originalLines = new List<BrailleLine> { line1, line2 };

            // Act
            ClipboardHelper.SetLines(originalLines);
            var retrievedLines = ClipboardHelper.GetLines();

            // Assert
            Assert.NotNull(retrievedLines);
            Assert.Equal(2, retrievedLines.Count);
            Assert.Equal("第一行", retrievedLines[0].Words[0].Text);
            Assert.Equal("第二行", retrievedLines[1].Words[0].Text);
        }

        [StaFact]
        public void SetAndGetLines_WithPhoneticCode_ShouldPreservePhoneticCode()
        {
            // Arrange - 驗證多層級的資料結構都能正確序列化
            var line1 = new BrailleLine();
            line1.Words.Add(new BrailleWord("點") { PhoneticCode = "ㄉㄧㄢˇ" });
            line1.Words.Add(new BrailleWord("字") { PhoneticCode = "ㄗˋ" });

            var originalLines = new List<BrailleLine> { line1 };

            // Act
            ClipboardHelper.SetLines(originalLines);
            var retrievedLines = ClipboardHelper.GetLines();

            // Assert
            Assert.NotNull(retrievedLines);
            Assert.Single(retrievedLines);
            Assert.Equal(2, retrievedLines[0].Words.Count);
            Assert.Equal("點", retrievedLines[0].Words[0].Text);
            Assert.Equal("ㄉㄧㄢˇ", retrievedLines[0].Words[0].PhoneticCode);
            Assert.Equal("字", retrievedLines[0].Words[1].Text);
            Assert.Equal("ㄗˋ", retrievedLines[0].Words[1].PhoneticCode);
        }

        [StaFact]
        public void GetLines_WhenClipboardIsEmpty_ShouldReturnNull()
        {
            // Arrange
            System.Windows.Forms.Clipboard.Clear();

            // Act
            var retrievedLines = ClipboardHelper.GetLines();

            // Assert
            Assert.Null(retrievedLines);
        }

        [StaFact]
        public void ClearData_WhenClipboardContainsWords_ShouldClearClipboard()
        {
            // Arrange
            var words = new List<BrailleWord> { new BrailleWord("測試") };
            ClipboardHelper.SetWords(words);

            // Act
            ClipboardHelper.ClearData();

            // Assert
            var retrievedWords = ClipboardHelper.GetWords();
            Assert.Null(retrievedWords);
        }

        [StaFact]
        public void ClearData_WhenClipboardContainsLines_ShouldClearClipboard()
        {
            // Arrange
            var line = new BrailleLine();
            line.Words.Add(new BrailleWord("測試"));
            var lines = new List<BrailleLine> { line };
            ClipboardHelper.SetLines(lines);

            // Act
            ClipboardHelper.ClearData();

            // Assert
            var retrievedLines = ClipboardHelper.GetLines();
            Assert.Null(retrievedLines);
        }

        [StaFact]
        public void SetWords_ShouldNotThrowNotSupportedException()
        {
            // Arrange - 這個測試確保我們修正的問題不會再次發生
            var words = new List<BrailleWord> 
            { 
                new BrailleWord("測試") { PhoneticCode = "ㄘㄜˋㄕˋ" } 
            };

            // Act & Assert - 不應該拋出 NotSupportedException
            var exception = Record.Exception(() =>
            {
                ClipboardHelper.SetWords(words);
                var retrieved = ClipboardHelper.GetWords();
            });

            Assert.Null(exception);
        }

        [StaFact]
        public void SetLines_ShouldNotThrowNotSupportedException()
        {
            // Arrange - 這個測試確保我們修正的問題不會再次發生
            var line = new BrailleLine();
            line.Words.Add(new BrailleWord("測試") { PhoneticCode = "ㄘㄜˋㄕˋ" });
            var lines = new List<BrailleLine> { line };

            // Act & Assert - 不應該拋出 NotSupportedException
            var exception = Record.Exception(() =>
            {
                ClipboardHelper.SetLines(lines);
                var retrieved = ClipboardHelper.GetLines();
            });

            Assert.Null(exception);
        }
    }
}
