using System.Collections.Generic;
using System.Text;
using BrailleToolkit;
using Huanlin.Common.Helpers;
using NChinese.Phonetic;
using NUnit.Framework;

namespace Test.BrailleToolkit
{
    /// <summary>
    ///This is a test class for BrailleToolkit.BrailleProcesser and is intended
    ///to contain all BrailleToolkit.BrailleProcesser Unit Tests
    ///</summary>
    [TestFixture]
    public class BrailleProcessorTest
    {
        [SetUp]
        public void SetUp()
        {
            Shared.SetupLogger();
        }

        /// <summary>
        ///A test for ConvertLine (string)
        ///</summary>
        [Test]
        public void ConvertLineTest()
        {
            BrailleProcessor target = BrailleProcessor.GetInstance();

            ConvertLineTestChinese(target);

            ConvertLineTestEnglish(target);
        }

        public void ConvertLineTestChinese(BrailleProcessor target)
        {
            string msg = "BrailleProcesser.ConvertLine 測試失敗: ";

            // 測試明眼字內含注音符號、冒號後面跟著"我"、以及引號、句號。
            string line = "ㄅˇ你說：我是誰？　我說：「我是神。」";
            string expected = "ㄅˇ你說： 我是誰？　我說：「我是神。」";
            BrailleLine brLine = target.ConvertLine(line);
            string actual = brLine.ToString();
            Assert.AreEqual(expected, actual, msg + line);

            // 測試破折號和刪節號。
            line = "第一種破折號：─，第二種破折號：－，連續破折號：──，－－。";
            expected = "第一種破折號：─，第二種破折號：－，連續破折號：──，－－。";
            brLine = target.ConvertLine(line);
            actual = brLine.ToString();
            Assert.AreEqual(expected, actual, msg + line);

            // 測試刪節號。
            line = "單：…，雙：……";
            expected = "單：…，雙：……";
            brLine = target.ConvertLine(line);
            actual = brLine.ToString();
            Assert.AreEqual(expected, actual, msg + line);

            // 測試連續多個全形空白：保留空白。
            line = "空　　　白　　　";
            expected = "空　　　白　　　";
            brLine = target.ConvertLine(line);
            actual = brLine.ToString();
            Assert.AreEqual(expected, actual, msg + line);
        }

        public void ConvertLineTestEnglish(BrailleProcessor target)
        {
            string msg = "BrailleProcesser.ConvertLine 測試失敗: ";

            // 測試一個大寫字母。
            string line = "Hello";
            string expected = "Hello";
            BrailleLine brLine = target.ConvertLine(line);
            string actual = brLine.ToString();
            Assert.AreEqual(expected, actual, msg + line);
            bool isOk = (brLine[0].Cells[0].Value == (byte)BrailleCellCode.Capital) &&
                (brLine[0].Cells[1].Value == 0x13);
            Assert.IsTrue(isOk, msg + line);

            // 測試兩個大寫字母。
            line = "ABC";
            expected = "ABC";
            brLine = target.ConvertLine(line);
            actual = brLine.ToString();
            Assert.AreEqual(expected, actual, msg + line);
            isOk = (brLine[0].Cells[0].Value == (byte)BrailleCellCode.Capital) &&
                (brLine[0].Cells[1].Value == (byte)BrailleCellCode.Capital) &&
                (brLine[0].Cells[2].Value == 0x01) &&   // 'A'
                (brLine[1].Cells[0].Value == 0x03);     // 'B'
            Assert.IsTrue(isOk, msg + line);

            // 測試數字。
            line = "123,56 2006-09-29";
            expected = "123,56 2006-09-29";
            brLine = target.ConvertLine(line);
            actual = brLine.ToString();
            isOk = (brLine[0].Cells[0].Value == (byte)BrailleCellCode.Digit) &&
                (brLine[4].Cells[0].Value != (byte)BrailleCellCode.Capital) &&	// 逗號視為數字的延續，不用額外加數字記號。
                (brLine[7].Cells[0].Value == (byte)BrailleCellCode.Digit) &&
                (brLine[12].Cells[0].Value != (byte)BrailleCellCode.Capital);	// 連字號視為數字的延續，不用額外加數字記號。
            Assert.IsTrue(isOk, msg + line);

            // 測試連續多個空白：保留空白。
            line = "a   b   ";
            expected = "a   b   ";
            brLine = target.ConvertLine(line);
            actual = brLine.ToString();
            Assert.AreEqual(expected, actual, msg + line);
        }


        [TestCase("#1-2. 1", "#1-2. 1", "(3456 1)(36)(12)(256)()(3456 2)")] // 編號的數字使用上位點；非編號的數字使用下位點。
        public void Should_DigitNumbers_UseUpperPosition(string input, string expected, string expectedDots)
        {
            var processor = BrailleProcessor.GetInstance();

            var brLine = processor.ConvertLine(input);
            var actual = brLine.ToString();

            Assert.AreEqual(input, expected);

            var actualDots = brLine.ToPositionNumberString();
            Assert.AreEqual(expectedDots, actualDots);
        }

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


        /// <summary>
        ///A test for PreprocessTags (string)
        ///</summary>
        [Test]
        public void Should_ConvertPreprocessTags_Succeed()
        {
            BrailleProcessor target = BrailleProcessor.GetInstance();

            string line = "<大單元結束>測試</大單元結束>";
            string expected = new string('ˍ', 20) + "測試" + ' ';
            string actual = target.ReplaceSimpleTagsWithConvertableText(line);

            Assert.AreEqual(expected, actual);
        }

        [TestCase(
            "小明說：（今天）下雨。",
            "(15 246 4)(134 13456 2)(24 25 3)(25 25)(246)(13 1456 3)(124 2345 3)(135)()(15 23456 5)(1256 4)(36)")]
        public void Should_ConvertString_Succeed(string inputText, string expectedPositionNumbers)
        {
            BrailleProcessor processor =
                BrailleProcessor.GetInstance(new ZhuyinReverseConverter(null));

            BrailleLine brLine = processor.ConvertLine(inputText);

            var result = brLine.ToPositionNumberString();
            Assert.AreEqual(expectedPositionNumbers, result);
        }


        [TestCase("）。", "(135)(36)")]
        [TestCase("），", "(135)(23)")]
        [TestCase("）；", "(135)(56)")]
        [TestCase("）：", "(135)(25 25)")]
        [TestCase("）！", "(135)(123)")]
        [TestCase("）？", "(135)(135)")]
        [TestCase("）」", "(135)(36 23)")]
        public void Should_NoSpace_BetweenRightParenthesisAndPunctuation(string inputText, string expectedPositionNumbers)
        {
            BrailleProcessor processor =
                BrailleProcessor.GetInstance(new ZhuyinReverseConverter(null));

            BrailleLine brLine = processor.ConvertLine(inputText);

            var result = brLine.ToPositionNumberString();
            Assert.AreEqual(expectedPositionNumbers, result);
        }


        [TestCase("。我", "(36)()(25 4)")]
        [TestCase("。「", "(36)()(56 36)")]
        [TestCase("」我", "(36 23)()(25 4)")]
        [TestCase("」「", "(36 23)()(56 36)")]
        [TestCase("！我", "(123)()(25 4)")]
        [TestCase("！「", "(123)()(56 36)")]
        [TestCase("？我", "(135)()(25 4)")]
        [TestCase("？「", "(135)()(56 36)")]
        public void Should_HaveSpace_AfterSpecificPunctuations(string inputText, string expectedPositionNumbers)
        {
            BrailleProcessor processor =
                BrailleProcessor.GetInstance(new ZhuyinReverseConverter(null));

            BrailleLine brLine = processor.ConvertLine(inputText);

            var result = brLine.ToPositionNumberString();
            Assert.AreEqual(expectedPositionNumbers, result);
        }


        [TestCase("<私名號>台北</私名號>。", "(56 56)(124 2456 2)(135 356 4)(36)")]
        [TestCase("<書名號>魔戒</書名號>。", "(6 36)(134 126 2)(13 346 5)(36)")]
        public void Should_NoSpace_BetweenSpecificNameAndPunctuation(string inputText, string expectedPositionNumbers)
        {
            BrailleProcessor processor =
                BrailleProcessor.GetInstance(new ZhuyinReverseConverter(null));

            BrailleLine brLine = processor.ConvertLine(inputText);            

            var lines = processor.FormatLine(brLine, BrailleConst.DefaultCellsPerLine, new ContextTagManager());

            var result = lines[0].ToPositionNumberString();
            Assert.AreEqual(expectedPositionNumbers, result);
        }

        //[TestCase("<私名號>台北</私名號>我", "(56 56)(124 2456 2)(135 356 4)()(25 4)")]
        [TestCase("<書名號>魔戒</書名號>我", "(6 36)(134 126 2)(13 346 5)()(25 4)")]

        public void Should_HaveSpace_BetweenSpecificNameAndAlphabet(string inputText, string expectedPositionNumbers)
        {
            BrailleProcessor processor =
                BrailleProcessor.GetInstance(new ZhuyinReverseConverter(null));

            BrailleLine brLine = processor.ConvertLine(inputText);

            var lines = processor.FormatLine(brLine, BrailleConst.DefaultCellsPerLine, new ContextTagManager());

            var result = lines[0].ToPositionNumberString();
            Assert.AreEqual(expectedPositionNumbers, result);
        }

        [TestCase("<分數>1/2</分數>。", "(1456 2)(34)(23 3456)(36)")]
        public void Should_ConvertFraction_Succeed(string inputText, string expectedPositionNumbers)
        {
            BrailleProcessor processor =
                BrailleProcessor.GetInstance(new ZhuyinReverseConverter(null));

            BrailleLine brLine = processor.ConvertLine(inputText);

            var result = brLine.ToPositionNumberString();
            Assert.AreEqual(expectedPositionNumbers, result);
        }

        [TestCase("<點譯者註>台北</點譯者註>。", "(246)(6 3)(124 2456 2)(135 356 4)(135)(36)")]
        [TestCase("<點譯者註>abc</點譯者註>", "(246)(6 3)(1)(12)(14)(135)")]
        [TestCase("<點譯者註>123</點譯者註>", "(246)(6 3)(3456 2)(23)(25)(135)")]
        [TestCase("<點譯者註>：測試？</點譯者註>", "(246)(6 3)(25 25)(245 2346 5)(24 156 5)(135)(135)")]
        public void Should_NoExtraSpace_InsideBrailleTranslatorNote(string inputText, string expectedPositionNumbers)
        {
            BrailleProcessor processor =
                BrailleProcessor.GetInstance(new ZhuyinReverseConverter(null));

            BrailleLine brLine = processor.ConvertLine(inputText);

            processor.FormatLine(brLine, BrailleConst.DefaultCellsPerLine, new ContextTagManager());

            var result = brLine.ToPositionNumberString();
            Assert.AreEqual(expectedPositionNumbers, result);
        }

        [Test]
        public void Should_OrgPageNumber_UseUpperPosition_And_NoDigitSymbol()
        {
            BrailleProcessor processor =
                BrailleProcessor.GetInstance(new ZhuyinReverseConverter(null));

            string inputText = "<P>14</P>";
            string expectedPositionNumbers = new StringBuilder().Insert(0, "(36)", 36) + "(1)(145)"; // 36 個底線，後面跟著 "14" 的點字（沒有數符）。
            BrailleLine brLine = processor.ConvertLine(inputText);

            processor.FormatLine(brLine, BrailleConst.DefaultCellsPerLine, new ContextTagManager());

            var result = brLine.ToPositionNumberString();
            Assert.AreEqual(expectedPositionNumbers, result);
        }

        [Test]
        public void Should_OrgPageNumber_Accept_RomanNumber()
        {
            BrailleProcessor processor =
                BrailleProcessor.GetInstance(new ZhuyinReverseConverter(null));

            string inputText = "<P>xiv</P>"; // page 14
            string expectedPositionNumbers = new StringBuilder().Insert(0, "(36)", 36) + "(1346)(24)(1236)"; // 36 個底線，後面跟著 "xiv" 的點字。
            BrailleLine brLine = processor.ConvertLine(inputText);

            processor.FormatLine(brLine, BrailleConst.DefaultCellsPerLine, new ContextTagManager());

            var result = brLine.ToPositionNumberString();
            Assert.AreEqual(expectedPositionNumbers, result);
        }

        [TestCase("<選項>ㄅ.</選項>", "(135)(6)")]
        public void Should_BopomofoAndDotInChoiceTag_NoSpaceAndUse6ForDot(string inputText, string expectedPositionNumbers)
        {
            BrailleProcessor processor =
                BrailleProcessor.GetInstance(new ZhuyinReverseConverter(null));

            BrailleLine brLine = processor.ConvertLine(inputText);

            processor.FormatLine(brLine, BrailleConst.DefaultCellsPerLine, new ContextTagManager());

            var result = brLine.ToPositionNumberString();
            Assert.AreEqual(expectedPositionNumbers, result);
        }
    }
}
