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
            string[] expectedBrailleCells =
            {
                "(2356)(2356)(2356)(2356)(2356)(2356)(2356)",
                "(123)()()(456)()()(456)",
                "(1235)(25)(25)(2456)(25)(25)(2456)",
                "(123)()()(456)()()(456)",
                "(1245)(1245)(1245)(1245)(1245)(1245)(1245)"
            };


            var processor = BrailleProcessor.GetInstance();


            var brDoc = new BrailleDocument(processor, 32);
            using (var reader = new StringReader(lines))
            {
                brDoc.LoadAndConvert(reader);
            }

            Assert.That(brDoc.LineCount, Is.EqualTo(expectedBrailleCells.Length));

            for (int i = 0; i < brDoc.LineCount; i++)
            {
                Assert.That(expectedBrailleCells[i], Is.EqualTo(brDoc.Lines[i].ToPositionNumberString()));
            }
        }

    }
}
