using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrailleToolkit;
using NUnit.Framework;

namespace Test.BrailleToolkit
{
    [TestFixture]
    public class TableConverterTest
    {
        [Test]
        public void Should_ConvertTable_Succeed()
        {
            string lines =
                "<表格>\n" +
                "┌──┬──┐\n" +
                "│　　∣　　∣\n" +
                "├──┼──┤\n" +
                "│　　∣　　∣\n" +
                "└──┴──┘\n" +
                "</表格>\n";


            var processor = BrailleProcessor.GetInstance();


            var brDoc = new BrailleDocument(processor, 32);
            using (var reader = new StringReader(lines))
            {
                brDoc.LoadAndConvert(reader);
            }

            Assert.IsTrue(brDoc.LineCount == 7);
            Assert.IsTrue(brDoc.Lines[0].Words[0].IsContextTag);
            Assert.IsTrue(brDoc.Lines[6].Words[0].IsContextTag);

            // TODO: Assert 左邊直線與中間和右邊直線的點字是否正確。
        }

    }
}
