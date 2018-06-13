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
        [TestCase("<數學>（1cm寬）</數學>", "(12356)(3456 2)(14)(134)()(123 12456 3)(23456)")]
        public void Should_NoExtraSpace_NextToParenthesis(string inputText, string expectedPositionNumbers)
        {
            var processor = BrailleProcessor.GetInstance();
            var brLine = processor.ConvertLine(inputText);
            var actual = brLine.ToPositionNumberString();

            Assert.AreEqual(expectedPositionNumbers, actual);
        }
    }
}
