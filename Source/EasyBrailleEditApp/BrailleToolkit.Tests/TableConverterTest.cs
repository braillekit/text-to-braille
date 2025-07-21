using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrailleToolkit;
using Xunit;

namespace BrailleToolkit.Tests
{
    [Collection("Singleton-Sensitive Tests")]
    public class TableConverterTest
    {
        [Fact]
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


            var processor = BrailleProcessor.CreateInstance();


            var brDoc = new BrailleDocument(processor, 32);
            using (var reader = new StringReader(lines))
            {
                brDoc.LoadAndConvert(reader);
            }

            Assert.Equal(expectedBrailleCells.Length, brDoc.LineCount);

            for (int i = 0; i < brDoc.LineCount; i++)
            {
                Assert.Equal(expectedBrailleCells[i], brDoc.Lines[i].ToPositionNumberString());
            }
        }

    }
}