﻿using System.Collections.Generic;
using BrailleToolkit;
using NChinese.Phonetic;
using NUnit.Framework;

namespace Test.BrailleToolkit
{
    /// <summary>
    ///This is a test class for BrailleToolkit.BrailleProcesser and is intended
    ///to contain all BrailleToolkit.BrailleProcesser Unit Tests
    ///</summary>
    [TestFixture]
    public class BrailleProcesserTest
    {
        [SetUp]
        public void SetUp()
        {
            Shared.SetupLogger();
        }

        public BrailleProcesserTest()
        {
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

            // 測試編號。
            line = "#1-2. 1";
            expected = "1-2. 1";
            brLine = target.ConvertLine(line);
            actual = brLine.ToString();
            isOk = (actual == expected) &&
                (brLine[0].Cells[0].Value == (byte)BrailleCellCode.Digit) &&
                (brLine[0].Cells[1].Value == 0x01) &&	// 數字 1 的上位點。
                (brLine[1].Cells[0].Value == 0x24) &&	// '-'
                (brLine[2].Cells[0].Value == 0x03) &&	// '2'
                (brLine[3].Cells[0].Value == 0x32) &&	// '.'
                (brLine[4].Cells[0].Value == 0x00) &&	// ' '
                (brLine[5].Cells[0].Value == (byte)BrailleCellCode.Digit) &&
                (brLine[5].Cells[1].Value == 0x02); 	// 數字 1 的下位點。
            Assert.IsTrue(isOk, msg + line);

            // 測試連續多個空白：保留空白。
            line = "a   b   ";
            expected = "a   b   ";
            brLine = target.ConvertLine(line);
            actual = brLine.ToString();
            Assert.AreEqual(expected, actual, msg + line);
        }


        /// <summary>
        ///A test for BreakLine (BrailleLine, int)
        ///</summary>
        [Test]
        public void Should_BreakLine_Succeed()
        {
            string msg = "BrailleProcesser.BreakLine 測試失敗!";

            BrailleProcessor target = BrailleProcessor.GetInstance();

            ContextTagManager context = new ContextTagManager();

            // 測試斷行：冒號+我。
            string line = "一二三四：我";
            BrailleLine brLine = target.ConvertLine(line);	// 冒號後面會加一個空方

            int cellsPerLine = 12;	// 故意在空方之後斷行。

            string expected = "一二三四：";
            List<BrailleLine> brLines = target.BreakLine(brLine, cellsPerLine, context);
            string actual = brLines[0].ToString();
            Assert.AreEqual(expected, actual, msg);
            brLines.Clear();

            // 測試斷行：斷在句點。
            line = "一二三四。";
            brLine = target.ConvertLine(line);

            cellsPerLine = 10;	// 故意斷在句號處。

            string expected1 = "一二三";	// 應該把最後一個字連同句號斷至下一行。
            string expected2 = "四。";
            brLines = target.BreakLine(brLine, cellsPerLine, context);
            bool isOk = (brLines.Count == 2 &&
                brLines[0].ToString() == expected1 && brLines[1].ToString() == expected2);
            Assert.IsTrue(isOk, msg);

            // 測試斷行：斷在英文字中間要加上連字號。
            line = "this is a loooooong word.";
            brLine = target.ConvertLine(line);

            cellsPerLine = 12;	// 故意斷在句號處。

            expected1 = "this is a";		// 應該把最後一個字連同句號斷至下一行。
            expected2 = "loooooong";
            brLines = target.BreakLine(brLine, cellsPerLine, context);
            isOk = (brLines.Count == 3 &&
                brLines[0].ToString() == expected1 && brLines[1].ToString() == expected2);
            Assert.IsTrue(isOk, msg);

            // 測試斷行：連續的數字不可斷開。
            line = "12345 6789";
            brLine = target.ConvertLine(line);

            cellsPerLine = 8;

            brLines = target.BreakLine(brLine, cellsPerLine, context);
            isOk = (brLines.Count == 2 && 
                brLines[0].ToString() == "12345" &&
                brLines[1].ToString() == "6789" &&
                brLines[0][0].Cells[0].Value == (byte)BrailleCellCode.Digit &&
                brLines[1][0].Cells[0].Value == (byte)BrailleCellCode.Digit);
            Assert.IsTrue(isOk, msg);

            // 測試斷行：斷在數字中間的逗號。
            line = "abc 123,456";
            brLine = target.ConvertLine(line);

            cellsPerLine = 8;	// 故意斷在逗號處。

            expected1 = "abc";
            expected2 = "123,456";
            brLines = target.BreakLine(brLine, cellsPerLine, context);
            isOk = (brLines.Count == 2 &&
                brLines[0].ToString() == expected1 && 
                brLines[1].ToString() == expected2 &&
                brLines[1][0].Cells[0].Value == (byte)BrailleCellCode.Digit);
            Assert.IsTrue(isOk, msg);
        }


        /// <summary>
        ///A test for PreprocessTags (string)
        ///</summary>
        [Test]
        public void Should_ConvertPreprocessTags_Succeed()
        {
            BrailleProcessor target = BrailleProcessor.GetInstance();

            string line = "<書名號>哈利波特</書名號>, <書名號>地下室手記</書名號>";
            string expected = "﹏﹏哈利波特 , ﹏﹏地下室手記 ";
            string actual = target.PreprocessTagsForLine(line);

            Assert.AreEqual(expected, actual, "BrailleProcesser.PreprocessTags 測試失敗!");
        }

        [TestCase(
            "小明說：（今天）下雨。",
            "(15 246 4)(134 13456 2)(24 25 3)(25 25)(246)(13 1456 3)(124 2345 3)(135)()(15 23456 5)(1256 4)(36)")]
        public void Should_ConvertString_Succeed(string inputText, string expectedDotNumbers)
        {
            BrailleProcessor processor =
                BrailleProcessor.GetInstance(new ZhuyinReverseConverter(null));

            BrailleLine brLine = processor.ConvertLine(inputText);

            var result = brLine.ToPositionNumberString();
            Assert.AreEqual(result, expectedDotNumbers);
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
            Assert.AreEqual(result, expectedPositionNumbers);
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
            Assert.AreEqual(result, expectedPositionNumbers);
        }


        [TestCase("<私名號>台北</私名號>。", "(56 56)(124 2456 2)(135 356 4)(36)")]
        public void Should_NoSpace_BetweenSpecificNameAndPunctuation(string inputText, string expectedPositionNumbers)
        {
            /*
             *
            測試私名號、書名號。
            line = "<私名號>蔡煥麟</私名號>，<書名號>倚天屠龍記</書名號>";
            expected = "╴╴蔡煥麟 ，﹏﹏倚天屠龍記 ";
            brLine = target.ConvertLine(line);
            actual = brLine.ToString();
            Assert.AreEqual(expected, actual, msg + line);
             */

            BrailleProcessor processor =
                BrailleProcessor.GetInstance(new ZhuyinReverseConverter(null));

            BrailleLine brLine = processor.ConvertLine(inputText);

            var result = brLine.ToPositionNumberString();
            Assert.AreEqual(result, expectedPositionNumbers);
        }

        [TestCase("<分數>1/2</分數>。", "(1456 2)(34)(23 3456)(36)")]
        public void Should_ConvertFraction_Succeed(string inputText, string expectedPositionNumbers)
        {
            BrailleProcessor processor =
                BrailleProcessor.GetInstance(new ZhuyinReverseConverter(null));

            BrailleLine brLine = processor.ConvertLine(inputText);
            var result = brLine.ToPositionNumberString();
            Assert.AreEqual(result, expectedPositionNumbers);
        }
    }


}
