using System.Collections.Generic;
using System.Text;
using BrailleToolkit;
using Huanlin.Common.Helpers;
using NChinese.Phonetic;
using NUnit.Framework;

namespace Test.BrailleToolkit
{
    [TestFixture]
    public partial class BrailleProcessorTest
    {
        /// <summary>
        ///A test for BreakLine (BrailleLine, int)
        ///</summary>
        [TestCase(12, "一二三四：我", 2, "一二三四：", "我")] // 測試斷行：冒號+我。在冒號後面的空方之後斷行。
        [TestCase(10, "一二三四。", 2, "一二三", "四。")] // 測試斷行：斷在句點時，應把前一個字連同句號斷至下一行。
        [TestCase(12, "this is a loooooong word.", 3, "this is a", "loooooong")] // 測試斷行：斷在英文字中間要加上連字號。應該把最後一個字連同句號斷至下一行。
        [TestCase(8, "12345 6789", 2, "12345", "6789")]  // 測試斷行：連續的數字不可斷開。
        [TestCase(8, "abc 123,456", 2, "abc", "123,456")] // 測試斷行：斷在數字中間的逗號。故意斷在逗號處。
        public void Should_BreakLine_Succeed(int cellsPerLine, string input,
            int expectedLineCount, string expectedLine1, string expectedLine2)
        {
            BrailleProcessor target = BrailleProcessor.GetInstance();

            ContextTagManager context = new ContextTagManager();

            BrailleLine brLine = target.ConvertLine(input);	// 冒號後面會加一個空方

            var brLines = target.BreakLine(brLine, cellsPerLine, context);

            Assert.AreEqual(expectedLineCount, brLines.Count);

            string actual = brLines[0].ToString();
            Assert.AreEqual(expectedLine1, actual);

            if (expectedLineCount > 1)
            {
                string actual2 = brLines[1].ToString();
                Assert.AreEqual(expectedLine2, actual2);
            }
        }

        [TestCase("0123456789012345678901234567890123456<書名號>哈利波特</書名號>。")]
        public void Should_SpecificName_NotAtEndOfLine(string inputText)
        {
            BrailleProcessor processor =
                BrailleProcessor.GetInstance(new ZhuyinReverseConverter(null));

            BrailleLine brLine = processor.ConvertLine(inputText);

            int cellsPerLine = 40;
            var formattedLines = processor.FormatLine(brLine, cellsPerLine, new ContextTagManager());

            Assert.IsTrue(formattedLines.Count == 2 && formattedLines[0].CellCount == 38 && formattedLines[1].CellCount == 15);

            // 第二行應該會以書名號開始，因為書名號單獨出現在行尾時必須折到下一行。
            Assert.AreEqual("<書名號>", formattedLines[1].Words[0].Text);
            string expectedBeginCellsOfSecondLine = "(6 36)";
            Assert.AreEqual(expectedBeginCellsOfSecondLine, formattedLines[1].Words[1].ToPositionNumberString(true));
        }

        [TestCase("012345678901234567890123456782測試……的功能", 35, 15, "……")]
        [TestCase("0123456789012345678901234567823測試──的功能", 36, 14, "──")]
        [TestCase("0123456789012345678901234567823測試─的功能", 36, 14, "─")]
        public void Should_EmdashAndEllipsis_NotAtBeginOfLine(string inputText, 
            int expectedCellCountOfLine1, int expectedCellCountOfLine2,
            string textShouldNotBeginOfLine)
        {
            BrailleProcessor processor =
                BrailleProcessor.GetInstance(new ZhuyinReverseConverter(null));

            BrailleLine brLine = processor.ConvertLine(inputText);

            int cellsPerLine = 40;
            var formattedLines = processor.FormatLine(brLine, cellsPerLine, new ContextTagManager());

            Assert.IsTrue(formattedLines.Count == 2 && formattedLines[0].CellCount == expectedCellCountOfLine1 && formattedLines[1].CellCount == expectedCellCountOfLine2);

            // 第二行不應該以刪節號開頭。
            Assert.AreNotEqual(textShouldNotBeginOfLine, formattedLines[1].Words[0].Text);
        }

    }
}
