using BrailleToolkit;
using NChinese.Phonetic;
using Xunit;
using System.IO;
using System.Text;
using EasyBrailleEdit.Common.Utilities;

namespace BrailleToolkit.Tests
{
    /// <summary>
    ///This is a test class for BrailleToolkit.BrailleDocument and is intended
    ///to contain all BrailleToolkit.BrailleDocument Unit Tests
    ///</summary>
    [Collection("Singleton-Sensitive Tests")]
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
            Assert.True(brDoc.PageTitles[0].ContentStartLineIndex == 3);
            Assert.True(brDoc.PageTitles[0].ContentStartLineRef.ToString() == "3");
            Assert.True(brDoc.IsBeginLineOfPageTitle(3));
            Assert.Same(brDoc.PageTitles[0], brDoc.FindPageTitleByBeginLine(brDoc.Lines[3]));
        }

        [Fact]
        public void UpdateTitlesLineIndex_ShouldTrackBeginLineByIdentityAfterInsert()
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

            var title = brDoc.PageTitles[0];
            var originalLine = brDoc.Lines[3];
            var originalIdentity = originalLine.Identity;

            var insertedLine = new BrailleLine();
            insertedLine.AddWord(new BrailleWord("新", "01"));
            brDoc.InsertLine(0, insertedLine);

            int changeCount = brDoc.UpdateTitlesLineIndex();

            Assert.Equal(1, changeCount);
            Assert.Equal(4, title.ContentStartLineIndex);
            Assert.Equal(originalIdentity, title.ContentStartLineIdentity);
            Assert.Same(brDoc.Lines[4], title.ContentStartLineRef);
            Assert.Same(title, brDoc.FindPageTitleByBeginLine(brDoc.Lines[4]));
            Assert.True(brDoc.IsBeginLineOfPageTitle(4));
        }

        [Fact]
        public void PageTitle_ShouldResolveBeginLineIndexByIdentityBeforeStoredIndexIsUpdated()
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

            var title = brDoc.PageTitles[0];

            var insertedLine = new BrailleLine();
            insertedLine.AddWord(new BrailleWord("新", "01"));
            brDoc.InsertLine(0, insertedLine);

            Assert.Equal(3, title.ContentStartLineIndex);
            Assert.True(title.TryResolveContentStartLineIndex(brDoc, out int resolvedIndex));
            Assert.Equal(4, resolvedIndex);
        }

        [Fact]
        public void FormatLine_ShouldPreserveFirstLineIdentityAndPageTitleAnchor()
        {
            var brDoc = new BrailleDocument();
            brDoc.CellsPerLine = 10;

            var line = new BrailleLine();
            for (int i = 0; i < 12; i++)
            {
                line.AddWord(new BrailleWord("a", "01"));
            }
            brDoc.AddLine(line);
            brDoc.AddPageTitleAt(new[] { new BrailleWord("標", "01") }, 0);

            var originalIdentity = line.Identity;
            var title = brDoc.PageTitles[0];

            int formattedLineCount = BrailleDocumentFormatter.FormatLine(brDoc, 0, new ContextTagManager());

            Assert.Equal(2, formattedLineCount);
            Assert.Equal(2, brDoc.LineCount);
            Assert.Equal(originalIdentity, brDoc.Lines[0].Identity);
            Assert.Equal(originalIdentity, title.ContentStartLineIdentity);
            Assert.True(brDoc.IsBeginLineOfPageTitle(0));
            Assert.Same(title, brDoc.FindPageTitleByBeginLine(brDoc.Lines[0]));
        }

        [Fact]
        public void DeepCopy_ShouldAssignFreshRuntimeIdentitiesToLinesAndWords()
        {
            var original = new BrailleDocument();
            var line = new BrailleLine();
            var word = new BrailleWord("A", "01");
            line.AddWord(word);
            original.AddLine(line);

            var copied = original.DeepCopy();

            Assert.Single(copied.Lines);
            Assert.NotEqual(original.Lines[0].Identity, copied.Lines[0].Identity);
            Assert.NotEqual(original.Lines[0].Words[0].Identity, copied.Lines[0].Words[0].Identity);
            Assert.True(copied.Lines[0].Identity > 0);
            Assert.True(copied.Lines[0].Words[0].Identity > 0);
        }

        [Fact]
        public void JsonRoundTrip_ShouldKeepPageTitleAnchorConsistentWithFreshLineIdentity()
        {
            var original = new BrailleDocument();
            original.CellsPerLine = 30;

            var line = new BrailleLine();
            line.AddWord(new BrailleWord("A", "01"));
            original.AddLine(line);
            original.AddPageTitleAt(new[] { new BrailleWord("標", "01") }, 0);

            string json = JsonHelper.Serialize(original);
            var copied = JsonHelper.Deserialize<BrailleDocument>(json);

            Assert.Single(copied.PageTitles);
            Assert.True(copied.Lines[0].Identity > 0);
            Assert.Equal(copied.Lines[0].Identity, copied.PageTitles[0].ContentStartLineIdentity);
            Assert.True(copied.IsBeginLineOfPageTitle(0));
            Assert.Same(copied.PageTitles[0], copied.FindPageTitleByBeginLine(copied.Lines[0]));
        }

        [Fact]
        public void LoadBrailleDocument_ThakurPoem_ShouldDeserializeCorrectly()
        {
            string filename = Shared.TestDataPath + "poem.brx";
            
            // 確保檔案存在
            Assert.True(File.Exists(filename), $"測試檔案 {filename} 不存在。");

            // 執行反序列化
            BrailleDocument brDoc = BrailleDocument.LoadBrailleFile(filename);

            // 驗證反序列化後的物件不為 null
            Assert.NotNull(brDoc);

            // 驗證 CellsPerLine 屬性
            Assert.Equal(40, brDoc.CellsPerLine);

            // 驗證 Lines 集合的數量
            Assert.Equal(9, brDoc.LineCount); 

            // 驗證第一行內容
            BrailleLine firstLine = brDoc.Lines[0];
            Assert.NotNull(firstLine);
            Assert.Equal(15, firstLine.Words.Count); // 根據提供的 brx 檔案內容判斷

            // 驗證第一個 BrailleWord 的內容 (例如: "夏天")
            BrailleWord firstWord = firstLine.Words[0];
            Assert.Equal("夏", firstWord.OriginalText);
            Assert.Equal("夏", firstWord.Text);
            Assert.Equal("ㄒㄧㄚˋ", firstWord.PhoneticCode);
            Assert.Equal(3, firstWord.CellList.Count); // 點字方的數量

            // 驗證第三行內容 (以 "秋天" 開始)
            BrailleLine thirdLine = brDoc.Lines[2];
            Assert.NotNull(thirdLine);
            Assert.Equal("秋天的黃葉，它們沒有什麼可唱，", thirdLine.ToOriginalTextString());
        }
    }

}
