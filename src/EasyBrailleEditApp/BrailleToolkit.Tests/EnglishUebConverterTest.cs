using System.Collections.Generic;
using System.Linq;
using BrailleToolkit.Converters;
using Xunit;

namespace BrailleToolkit.Tests
{
    [Collection("Singleton-Sensitive Tests")]
    public class EnglishUebConverterTest
    {
        private EnglishUebConverter _converter;

        public EnglishUebConverterTest()
        {
            Shared.SetupLogger();
            _converter = new EnglishUebConverter();
        }

        /// <summary>
        /// Helper function to simulate the full conversion pipeline.
        /// It repeatedly calls the converter until the input stack is empty.
        /// </summary>
        private List<BrailleWord> ConvertText(string text)
        {
            var charStack = new Stack<char>(text.ToCharArray().Reverse());
            var resultWords = new List<BrailleWord>();
            var context = new ContextTagManager();

            while (charStack.Count > 0)
            {
                var convertedPart = _converter.Convert(charStack, context);
                if (convertedPart != null && convertedPart.Count > 0)
                {
                    resultWords.AddRange(convertedPart);
                }
                else
                {
                    // If converter returns null or empty, it means it cannot handle the character.
                    // For a robust test, we should probably stop, but here we'll just pop to avoid infinite loops.
                    if (charStack.Count > 0)
                    {
                        charStack.Pop();
                    }
                }
            }
            return resultWords;
        }

        [Fact]
        public void Should_ConvertGrade1Word_Correctly()
        {
            var result = ConvertText("cat");
            Assert.Equal(3, result.Count);
            Assert.Equal("c", result[0].Text);
            Assert.Equal("a", result[1].Text);
            Assert.Equal("t", result[2].Text);
            Assert.Equal("09", result[0].ToHexSting());
            Assert.Equal("01", result[1].ToHexSting());
            Assert.Equal("1E", result[2].ToHexSting());
        }

        [Fact]
        public void Should_ConvertWordSign_AsSingleWord()
        {
            var result = ConvertText("knowledge");
            Assert.Single(result);
            Assert.Equal("knowledge", result[0].Text);
            Assert.Equal("05", result[0].ToHexSting());
        }

        [Fact]
        public void Should_ConvertGroupSign_AsSingleWord()
        {
            var result = ConvertText("and");
            Assert.Single(result);
            Assert.Equal("and", result[0].Text);
            Assert.Equal("2F", result[0].ToHexSting());
        }

        [Fact]
        public void Should_ConvertMixedSentence_Correctly()
        {
            var result = ConvertText("you like it");
            Assert.Equal(5, result.Count); // Should be 5 words including spaces

            Assert.Equal("you", result[0].Text);
            Assert.Equal("3D", result[0].ToHexSting());

            Assert.True(result[1].IsWhiteSpace);

            Assert.Equal("like", result[2].Text);
            Assert.Equal("07", result[2].ToHexSting());

            Assert.True(result[3].IsWhiteSpace);

            Assert.Equal("it", result[4].Text);
            Assert.Equal("2D", result[4].ToHexSting());
        }

        // This test is expected to FAIL with the current simple greedy algorithm.
        // It demonstrates the need for more advanced rules.
        [Fact]
        public void Should_PrioritizeLongerContraction_GreedyTest()
        {
            // "about" is a short-form word, "ab" is its representation.
            // The converter should match "about" not "ab".
            var result = ConvertText("about");
            Assert.Single(result);
            Assert.Equal("about", result[0].Text);
            Assert.Equal("0103", result[0].ToHexSting());
        }
    }
}
