using BrailleToolkit.Tags;
using BrailleToolkit.Helpers;
using BrailleToolkit.Converters;
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

        [Fact]
        public void FromBrailleWord_ApplyTo_ShouldMutateExistingWordAndPreserveMetadata()
        {
            var source = new BrailleWord("A", "01")
            {
                Language = BrailleLanguage.English,
                DontBreakLineHere = true,
                ContextNames = "時間",
                NoDigitCell = true,
                NoSpace = true,
                NoCapitalRule = true,
                IsEngPhonetic = true
            };
            source.PhoneticCode = "ㄅ";
            source.IsPolyphonic = true;
            var originalCells = source.Cells;

            var builder = BrailleWordBuilder.FromBrailleWord(source);
            builder.PrependCell(BrailleCell.Capital);
            builder.ReplaceCell(1, BrailleCell.GetInstance("02"));

            builder.ApplyTo(source);

            Assert.Same(originalCells, source.Cells);
            Assert.Equal("A", source.Text);
            Assert.Equal("A", source.OriginalText);
            Assert.Equal(BrailleLanguage.English, source.Language);
            Assert.Equal("ㄅ", source.PhoneticCode);
            Assert.True(source.IsPolyphonic);
            Assert.True(source.DontBreakLineHere);
            Assert.Equal("時間", source.ContextNames);
            Assert.True(source.NoDigitCell);
            Assert.True(source.NoSpace);
            Assert.True(source.NoCapitalRule);
            Assert.True(source.IsEngPhonetic);
            Assert.Equal(2, source.CellCount);
            Assert.Equal(BrailleCell.Capital, source.Cells[0]);
            Assert.Equal(BrailleCell.GetInstance("02"), source.Cells[1]);
        }

        [Fact]
        public void BuildResult_ShouldApplyToExistingBrailleWordViaConstructionBoundary()
        {
            var builder = new BrailleWordBuilder("B")
            {
                OriginalText = "<原文>B",
                Language = BrailleLanguage.English,
                ContextNames = "數學",
                NoCapitalRule = true
            };
            builder.AppendHex("1B");

            IBrailleWordResult result = builder.Build();
            var target = new BrailleWord("X", "01");
            var originalCells = target.Cells;

            target.ApplyResult(result);

            Assert.Same(originalCells, target.Cells);
            Assert.Equal("B", target.Text);
            Assert.Equal("<原文>B", target.OriginalText);
            Assert.Equal(BrailleLanguage.English, target.Language);
            Assert.Equal("數學", target.ContextNames);
            Assert.True(target.NoCapitalRule);
            Assert.Single(target.Cells);
            Assert.Equal(BrailleCell.GetInstance("1B"), target.Cells[0]);
        }

        [Fact]
        public void ReadOnlyHelpers_ShouldConsumeMaterializedResultsWithoutBrailleWordMaterialization()
        {
            var titleBuilder = new BrailleWordBuilder(ContextTagNames.Title)
            {
                OriginalText = ContextTagNames.Title,
                IsContextTag = true
            };

            var wordBuilder = new BrailleWordBuilder("甲")
            {
                OriginalText = "原甲"
            };
            wordBuilder.AppendHex("01");
            wordBuilder.AppendHex("02");

            var derivedBuilder = new BrailleWordBuilder("顯示")
            {
                OriginalText = "<數學>",
                IsConvertedFromTag = true
            };
            derivedBuilder.AppendHex("03");

            IReadOnlyList<IBrailleWordResult> results =
            [
                titleBuilder.Build(),
                wordBuilder.Build(),
                derivedBuilder.Build()
            ];
            IReadOnlyList<IBrailleWordResult> wordOnlyResults =
            [
                wordBuilder.Build()
            ];

            Assert.True(BrailleWordHelper.ContainsTitleTag(results));
            Assert.Equal($"{ContextTagNames.Title}甲顯示", BrailleWordHelper.ToTextString(results));
            Assert.Equal($"{ContextTagNames.Title}原甲", BrailleWordHelper.ToOriginalTextString(results));
            Assert.Equal("(1 2)", BrailleWordHelper.ToDotNumberString(wordOnlyResults));
            Assert.Equal(3, results.GetCellCount());
        }

        [Fact]
        public void BrailleFontConverter_ShouldRenderMaterializedResultWithoutBrailleWordMaterialization()
        {
            var builder = new BrailleWordBuilder("A");
            builder.AppendHex("01");
            builder.AppendHex("02");

            IBrailleWordResult result = builder.Build();

            Assert.Equal(
                BrailleFontConverter.ToString(result.ToBrailleWord()),
                BrailleFontConverter.ToString(result));
        }

        [Fact]
        public void SequenceFormatter_ShouldRenderMaterializedResultsLikeBrailleLine()
        {
            var line = new BrailleLine();
            line.AddWord(new BrailleWord("A", "01"));
            line.AddWord(new BrailleWord("B", "1203"));

            var builder1 = new BrailleWordBuilder("A");
            builder1.AppendHex("01");

            var builder2 = new BrailleWordBuilder("B");
            builder2.AppendHex("1203");

            IReadOnlyList<IBrailleWordResult> results =
            [
                builder1.Build(),
                builder2.Build()
            ];

            Assert.Equal(line.ToBrailleCellHexString(), BrailleWordSequenceFormatter.ToBrailleCellHexString(results));
            Assert.Equal(line.ToPositionNumberString(), BrailleWordSequenceFormatter.ToPositionNumberString(results));
            Assert.Equal(
                line.ToHtmlString("  ", "column", "braille", "text"),
                BrailleWordSequenceFormatter.ToHtmlString(results, "  ", "column", "braille", "text"));
        }

        [Fact]
        public void ReadOnlyViewContract_ShouldSupportMixedBrailleWordAndResultSequences()
        {
            var directWord = new BrailleWord("A", "01");

            var builder = new BrailleWordBuilder("B")
            {
                OriginalText = "原B"
            };
            builder.AppendHex("1203");

            IReadOnlyList<IBrailleWordView> mixedWords =
            [
                directWord,
                builder.Build()
            ];
            var expectedLine = new BrailleLine();
            expectedLine.AddWord(directWord);
            expectedLine.AddWord(builder.Build().ToBrailleWord());

            Assert.Equal("AB", BrailleWordHelper.ToTextString(mixedWords));
            Assert.Equal("A原B", BrailleWordHelper.ToOriginalTextString(mixedWords));
            Assert.Equal("011203", BrailleWordSequenceFormatter.ToBrailleCellHexString(mixedWords));
            Assert.Equal(
                expectedLine.ToHtmlString("  ", "column", "braille", "text"),
                BrailleWordSequenceFormatter.ToHtmlString(mixedWords, "  ", "column", "braille", "text"));
        }
    }
}
