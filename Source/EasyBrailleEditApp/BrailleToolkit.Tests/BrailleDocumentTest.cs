using BrailleToolkit;
using NChinese.Phonetic;
using Xunit;
using System.IO;
using System.Text;

namespace BrailleToolkit.Tests
{
    /// <summary>
    ///This is a test class for BrailleToolkit.BrailleDocument and is intended
    ///to contain all BrailleToolkit.BrailleDocument Unit Tests
    ///</summary>
    public class BrailleDocumentTest
	{
        private BrailleProcessor _processor = BrailleProcessor.CreateInstance(new ZhuyinReverseConverter(null));

        public BrailleDocumentTest()
        {
            Shared.SetupLogger();
        }

        [Fact]
		public void Should_LoadFromFileAndConvert_Succeed()
		{
            string filename = Shared.TestDataPath + "TestData_Braille.txt";
			BrailleDocument brDoc = new BrailleDocument(filename, _processor, 32);

			brDoc.LoadAndConvert();
		}

        [Fact]
        public void Should_ConvertFraction_Succeed()
        {
            var brDoc = new BrailleDocument(_processor, 32);

            brDoc.Convert("<分數>1/2</分數>");
        }

        [Fact]
        public void Should_FetchPageTitles_Succeed()
        {
            string text =
                "0\n" +
                "1\n" +
                "2\n" +
                "<標題>insert at 3</標題>\n" +
                "3\n" +
                "4\n";
            
            var brDoc = new BrailleDocument(_processor);
            using (var reader = new StringReader(text))
            {
                brDoc.LoadAndConvert(reader);
            }

            Assert.True(brDoc.PageTitles.Count == 1);
            Assert.True(brDoc.LineCount == 5);
            Assert.True(brDoc.PageTitles[0].BeginLineIndex == 3);
            Assert.True(brDoc.PageTitles[0].BeginLineRef.ToString() == "3");
        }
    }

}