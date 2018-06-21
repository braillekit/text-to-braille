using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrailleToolkit;
using NUnit.Framework;

namespace Test.BrailleToolkit
{
    [TestFixture()]
    public class MathConverterTest
    {
        [TestCase("<數學>15？…</數學>", "(3456 2)(26)(456 236)()(5 5 5)")]
        public void Should_AddSpace_BetweenQuestionMarkAndEllipsis(string inputText, string expectedPositionNumbers)
        {
            var processor = BrailleProcessor.GetInstance();
            var brLine = processor.ConvertLine(inputText);
            var actual = brLine.ToPositionNumberString();

            Assert.AreEqual(expectedPositionNumbers, actual);
        }


        [TestCase("<數學>（1cm寬）</數學>", "(12356)(2)(14)(134)()(123 12456 3)(23456)")]
        [TestCase("<數學>（）×5 =</數學>", "(12356)()(23456)(4 16)(26)()(46 13)")]
        [TestCase("<數學>（√）</數學>", "(12356)(26 345)(23456)")]
        public void Should_NoSpace_NextToParenthesis(string inputText, string expectedPositionNumbers)
        {
            var processor = BrailleProcessor.GetInstance();
            var brLine = processor.ConvertLine(inputText);
            var actual = brLine.ToPositionNumberString();

            Assert.AreEqual(expectedPositionNumbers, actual);
        }

        [TestCase("<數學>【1】</數學>", "(12356)(2)(23456)")]
        public void Should_NoSpace_InCircledNumbers(string inputText, string expectedPositionNumbers)
        {
            var processor = BrailleProcessor.GetInstance();
            var brLine = processor.ConvertLine(inputText);
            var actual = brLine.ToPositionNumberString();

            Assert.AreEqual(expectedPositionNumbers, actual);
        }


        [TestCase("<數學>1+2+…+10</數學>", "(3456 2)(346)(23)(346)(5 5 5)(346)(2)(356)")]
        public void Should_NoSpace_NextToEllipsis(string inputText, string expectedPositionNumbers)
        {
            var processor = BrailleProcessor.GetInstance();
            var brLine = processor.ConvertLine(inputText);
            var actual = brLine.ToPositionNumberString();

            Assert.AreEqual(expectedPositionNumbers, actual);
        }

        [TestCase("<數學>（1+2)</數學>", "(12356)(2)(346)(23)(23456)")]
        public void Should_NoDigitSymbol_AfterLeftParenthesis(string inputText, string expectedPositionNumbers)
        {
            var processor = BrailleProcessor.GetInstance();
            var brLine = processor.ConvertLine(inputText);
            var actual = brLine.ToPositionNumberString();

            Assert.AreEqual(expectedPositionNumbers, actual);
        }


        [TestCase("<數學>（1+2)+4 = 10</數學>", "(12356)(2)(346)(23)(23456)(346)(256)()(46 13)()(3456 2)(356)")]
        public void Should_NoDigitSymbol_AfterRightParenthesis(string inputText, string expectedPositionNumbers)
        {
            var processor = BrailleProcessor.GetInstance();
            var brLine = processor.ConvertLine(inputText);
            var actual = brLine.ToPositionNumberString();

            Assert.AreEqual(expectedPositionNumbers, actual);
        }

    }
}
