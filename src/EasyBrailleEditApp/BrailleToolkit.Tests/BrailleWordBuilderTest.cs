using BrailleToolkit.Tags;
using Xunit;

namespace BrailleToolkit.Tests
{
    [Collection("Singleton-Sensitive Tests")]
    public class BrailleWordBuilderTest
    {
        [Fact]
        public void ToBrailleWord_ShouldPreserveCellsAndMetadata()
        {
            var builder = new BrailleWordBuilder("A")
            {
                OriginalText = "<原文>A",
                Language = BrailleLanguage.English,
                PhoneticCode = "ㄅ",
                IsPolyphonic = true,
                DontBreakLineHere = true,
                ContextNames = "時間",
                IsConvertedFromTag = true,
                NoDigitCell = true,
                NoSpace = true,
                NoCapitalRule = true,
                IsEngPhonetic = true
            };

            builder.AppendHex("01");
            builder.PrependCell(BrailleCell.GetInstance(new int[] { 4, 5, 6 }));

            IBrailleWordResult result = builder.Build();
            BrailleWord word = result.ToBrailleWord();

            Assert.Equal("A", word.Text);
            Assert.Equal("<原文>A", word.OriginalText);
            Assert.Equal(BrailleLanguage.English, word.Language);
            Assert.Equal("ㄅ", word.PhoneticCode);
            Assert.True(word.IsPolyphonic);
            Assert.True(word.DontBreakLineHere);
            Assert.Equal("時間", word.ContextNames);
            Assert.True(word.IsConvertedFromTag);
            Assert.True(word.NoDigitCell);
            Assert.True(word.NoSpace);
            Assert.True(word.NoCapitalRule);
            Assert.True(word.IsEngPhonetic);
            Assert.Equal(2, word.CellCount);
            Assert.Equal(BrailleCell.GetInstance(new int[] { 4, 5, 6 }), word.Cells[0]);
            Assert.Equal(BrailleCell.GetInstance("01"), word.Cells[1]);
        }

        [Fact]
        public void IsContextTag_ShouldClearCellsWhenMaterialized()
        {
            var builder = new BrailleWordBuilder("#");
            builder.AppendHex("01");
            builder.IsContextTag = true;

            BrailleWord word = builder.ToBrailleWord();

            Assert.True(word.IsContextTag);
            Assert.Equal(0, word.CellCount);
        }
    }
}
