using System.Collections.Generic;
using BrailleToolkit;
using BrailleToolkit.Converters;
using BrailleToolkit.Tags;
using Xunit;

namespace BrailleToolkit.Tests
{
    /// <summary>
    ///This is a test class for BrailleToolkit.EnglishWordConverter and is intended
    ///to contain all BrailleToolkit.EnglishWordConverter Unit Tests
    ///</summary>
    [Collection("Singleton-Sensitive Tests")]
    public class EnglishWordConverterTest
    {
        public EnglishWordConverterTest()
        {
            Shared.SetupLogger();
        }

        /// <summary>
        ///A test for Convert (Stack&lt;char&gt;)
        ///</summary>
        [Fact]
        public void Should_ConvertCommonCharacters_Succeed()
        {
            var processor = BrailleProcessor.CreateInstance();
            var converter = new EnglishWordConverter(processor);

			ContextTagManager context = new ContextTagManager();

            // 測試刪節號。
            string text = "...";
            Stack<char> charStack = new Stack<char>(text);
            List<BrailleWord> expected = new List<BrailleWord>();
            BrailleWord brWord = new BrailleWord(text, "040404");
            expected.Add(brWord);
            List<BrailleWord>? actual = converter.Convert(charStack, context);
            
            Assert.Equal(expected, actual);
            charStack.Clear();

            // 測試左單引號。
            text = "‘";
            charStack = new Stack<char>(text);
            brWord = new BrailleWord(text, "2026");
            expected.Clear();
            expected.Add(brWord);
            actual = converter.Convert(charStack, context);
            Assert.Equal(expected, actual);
            charStack.Clear();

            // 測試左雙引號。
            text = "“";
            charStack = new Stack<char>(text);
            brWord = new BrailleWord(text, "26");
            expected.Clear();
            expected.Add(brWord);
            actual = converter.Convert(charStack, context);
            Assert.Equal(expected, actual);
            charStack.Clear();

            // 測試大寫字母（不用加大寫記號，因為延後到整行調整時才處理）。
            text = "A";
            charStack = new Stack<char>(text);
            brWord = new BrailleWord(text, "01");
            expected.Clear();
            expected.Add(brWord);
            actual = converter.Convert(charStack, context);
            Assert.Equal(expected, actual);
            charStack.Clear();  
          
            // 測試數字
            text = "6";
            charStack = new Stack<char>(text);
            brWord = new BrailleWord(text, "16");	// 下位點。
            expected.Clear();
            expected.Add(brWord);
            actual = converter.Convert(charStack, context);
            Assert.Equal(expected, actual);
            charStack.Clear();            
        }

        [Fact]
        public void ColonInTimeContext_ShouldPrepend456Cell()
        {
            var processor = BrailleProcessor.CreateInstance();
            var converter = new EnglishWordConverter(processor);

            string text = ":";
            var plainContext = new ContextTagManager();
            var timeContext = new ContextTagManager();
            timeContext.Parse(ContextTagNames.Time, out bool _);

            List<BrailleWord>? plainWords = converter.Convert(new Stack<char>(text), plainContext);
            List<BrailleWord>? timeWords = converter.Convert(new Stack<char>(text), timeContext);

            Assert.NotNull(plainWords);
            Assert.NotNull(timeWords);
            Assert.Single(plainWords!);
            Assert.Single(timeWords!);

            BrailleWord plainWord = plainWords[0];
            BrailleWord timeWord = timeWords[0];

            Assert.Equal(plainWord.CellCount + 1, timeWord.CellCount);
            Assert.Equal(BrailleCell.GetInstance(new int[] { 4, 5, 6 }), timeWord.Cells[0]);
            for (int i = 0; i < plainWord.CellCount; i++)
            {
                Assert.Equal(plainWord.Cells[i], timeWord.Cells[i + 1]);
            }
        }
    }
}
