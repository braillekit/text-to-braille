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
        public void Should_FetchPageTitles_Succeed()
        {
            string text =
                "0\n" +
                "1\n" +
                "2\n" +
                "<標題>insert at 3</標題>\n" +
                "3\n" +
                "4\n";

            var processor =  BrailleProcessor.GetInstance();

            var brDoc = new BrailleDocument(processor);
            using (var reader = new StringReader(text))
            {
                brDoc.LoadAndConvert(reader);
            }

            Assert.That(brDoc.PageTitles.Count == 1, Is.True);
            Assert.That(brDoc.LineCount == 5, Is.True);
            Assert.That(brDoc.PageTitles[0].BeginLineIndex == 3, Is.True);
            Assert.That(brDoc.PageTitles[0].BeginLineRef.ToString() == "3", Is.True);
        }
    }

}
