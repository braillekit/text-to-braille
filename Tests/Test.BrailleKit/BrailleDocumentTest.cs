using BrailleToolkit;
using NChinese.Phonetic;
using NUnit.Framework;
using System.IO;
using System.Text;

namespace Test.BrailleToolkit
{
    /// <summary>
    ///This is a test class for BrailleToolkit.BrailleDocument and is intended
    ///to contain all BrailleToolkit.BrailleDocument Unit Tests
    ///</summary>
    [TestFixture]
	public class BrailleDocumentTest
	{
        [SetUp]
        public void SetUp()
        {
            Shared.SetupLogger();
        }

        [Test]
		public void Should_LoadFromFileAndConvert_Succeed()
		{
			BrailleProcessor processor = 
                BrailleProcessor.GetInstance(new ZhuyinReverseConverter(null));

            string filename = Shared.TestDataPath + "TestData_Braille.txt";
			BrailleDocument brDoc = new BrailleDocument(filename, processor, 32);

			brDoc.LoadAndConvert();
		}

        [Test]
        public void Should_ConvertFraction_Succeed()
        {
            BrailleProcessor processor =
                BrailleProcessor.GetInstance(new ZhuyinReverseConverter(null));

            var brDoc = new BrailleDocument(processor, 32);

            brDoc.Convert("<分數>1/2</分數>");
        }

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
            

            BrailleProcessor processor =
                BrailleProcessor.GetInstance(new ZhuyinReverseConverter(null));


            var brDoc = new BrailleDocument(processor, 32);
            using (var reader = new StringReader(lines))
            {
                brDoc.LoadAndConvert(reader);
            }

            Assert.IsTrue(brDoc.LineCount == 7);
            Assert.IsTrue(brDoc.Lines[0].Words[0].IsContextTag);
            Assert.IsTrue(brDoc.Lines[6].Words[0].IsContextTag);
        }
    }

}
