using Xunit;

namespace BrailleToolkit.Tests
{
    [Collection("Singleton-Sensitive Tests")]
    public class BrailleLineBuilderTest
    {
        [Fact]
        public void BrailleLineMaterialized_ShouldExposeWordsAndTag()
        {
            var word1 = new BrailleWord("A");
            var word2 = new BrailleWord("B");
            var words = new[] { word1, word2 };

            var materialized = new BrailleLineMaterialized(words, "myTag");

            Assert.Equal(2, materialized.WordCount);
            Assert.Same(word1, materialized[0]);
            Assert.Same(word2, materialized[1]);
            Assert.Equal("myTag", materialized.Tag);
            Assert.Equal(0L, materialized.Identity);
        }

        [Fact]
        public void BrailleLineMaterialized_ToBrailleLine_ShouldProduceFreshIdentity()
        {
            var word = new BrailleWord("Test");
            var materialized = new BrailleLineMaterialized(new[] { word }, null);

            BrailleLine line = materialized.ToBrailleLine();

            Assert.True(line.Identity > 0);
            Assert.Equal(1, line.WordCount);
            Assert.Same(word, line.Words[0]);
        }

        [Fact]
        public void BrailleLine_FromResult_ShouldProduceNewIdentity()
        {
            var word = new BrailleWord("X");
            var result = new BrailleLineMaterialized(new[] { word }, "tag");

            BrailleLine line = BrailleLine.FromResult(result);

            Assert.True(line.Identity > 0);
            Assert.Equal(1, line.WordCount);
            Assert.Same(word, line.Words[0]);
            Assert.Equal("tag", line.Tag);
        }

        [Fact]
        public void BrailleLine_ApplyResult_ShouldPreserveIdentity()
        {
            var originalWord = new BrailleWord("Old");
            var line = new BrailleLine();
            line.AddWord(originalWord);
            long originalIdentity = line.Identity;

            var newWord = new BrailleWord("New");
            var result = new BrailleLineMaterialized(new[] { newWord }, "newTag");

            line.ApplyResult(result);

            Assert.Equal(originalIdentity, line.Identity);
            Assert.Equal(1, line.WordCount);
            Assert.Same(newWord, line.Words[0]);
            Assert.Equal("newTag", line.Tag);
        }

        [Fact]
        public void BrailleLineMaterialized_CellCount_ShouldSumWordCells()
        {
            var word1 = new BrailleWord("A");
            word1.Cells.Add(BrailleCell.GetInstance(0x01));
            word1.Cells.Add(BrailleCell.GetInstance(0x02));

            var word2 = new BrailleWord("B");
            word2.Cells.Add(BrailleCell.GetInstance(0x03));

            var materialized = new BrailleLineMaterialized(new[] { word1, word2 }, null);

            Assert.Equal(3, materialized.CellCount);
        }

        [Fact]
        public void Builder_AddWord_AndBuild_ShouldProduceCorrectResult()
        {
            var builder = new BrailleLineBuilder();
            var word1 = new BrailleWord("A");
            var word2 = new BrailleWord("B");

            builder.AddWord(word1);
            builder.AddWord(word2);
            builder.Tag = "test";

            IBrailleLineResult result = builder.Build();

            Assert.Equal(2, result.WordCount);
            Assert.Same(word1, result[0]);
            Assert.Same(word2, result[1]);
            Assert.Equal("test", result.Tag);
        }

        [Fact]
        public void Builder_InsertAndRemove_ShouldWork()
        {
            var builder = new BrailleLineBuilder();
            builder.AddWord(new BrailleWord("A"));
            builder.AddWord(new BrailleWord("C"));
            builder.Insert(1, new BrailleWord("B"));

            Assert.Equal(3, builder.WordCount);
            Assert.Equal("B", builder[1].Text);

            builder.RemoveAt(0);
            Assert.Equal(2, builder.WordCount);
            Assert.Equal("B", builder[0].Text);
        }

        [Fact]
        public void Builder_FromBrailleLine_ShouldCopyWordsAndTag()
        {
            var line = new BrailleLine();
            var word = new BrailleWord("X");
            line.AddWord(word);
            line.Tag = "orig";

            var builder = BrailleLineBuilder.FromBrailleLine(line);

            Assert.Equal(1, builder.WordCount);
            Assert.Same(word, builder[0]);
            Assert.Equal("orig", builder.Tag);
        }

        [Fact]
        public void Builder_ToBrailleLine_RoundTrip()
        {
            var builder = new BrailleLineBuilder();
            builder.AddWord(new BrailleWord("Hello"));
            builder.Tag = "hi";

            BrailleLine line = builder.ToBrailleLine();

            Assert.True(line.Identity > 0);
            Assert.Equal(1, line.WordCount);
            Assert.Equal("Hello", line.Words[0].Text);
            Assert.Equal("hi", line.Tag);
        }

        [Fact]
        public void Builder_ApplyTo_ShouldPreserveTargetIdentity()
        {
            var target = new BrailleLine();
            target.AddWord(new BrailleWord("Old"));
            long targetId = target.Identity;

            var builder = new BrailleLineBuilder();
            builder.AddWord(new BrailleWord("New"));
            builder.Tag = "updated";

            builder.ApplyTo(target);

            Assert.Equal(targetId, target.Identity);
            Assert.Equal(1, target.WordCount);
            Assert.Equal("New", target.Words[0].Text);
            Assert.Equal("updated", target.Tag);
        }
    }
}
