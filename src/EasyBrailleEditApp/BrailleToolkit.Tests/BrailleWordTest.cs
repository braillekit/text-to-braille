using Xunit;

namespace BrailleToolkit.Tests
{
    [Collection("Singleton-Sensitive Tests")]
    public class BrailleWordTest
    {
        [Fact]
        public void CopyMethods_ShouldPreserveOriginalText()
        {
            var source = new BrailleWord("甲")
            {
                OriginalText = "<數學>"
            };

            var copied = source.Copy();

            Assert.Equal(source.Text, copied.Text);
            Assert.Equal(source.OriginalText, copied.OriginalText);

            var target = new BrailleWord("乙");
            target.Copy(source);

            Assert.Equal(source.Text, target.Text);
            Assert.Equal(source.OriginalText, target.OriginalText);
        }
    }
}
