using System.Collections.Generic;
using System.Linq;
using BrailleToolkit.Converters;
using NUnit.Framework;

namespace BrailleToolkit.Tests
{
    [TestFixture]
    public class EnglishUebConverterTest
    {
        private EnglishUebConverter _converter;

        [SetUp]
        public void SetUp()
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
                    charStack.Pop(); 
                }
            }
            return resultWords;
        }

        [Test]
        public void Should_ConvertGrade1Word_Correctly()
        {
            var result = ConvertText("cat");
            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result[0].Text, Is.EqualTo("c"));
            Assert.That(result[1].Text, Is.EqualTo("a"));
            Assert.That(result[2].Text, Is.EqualTo("t"));
            Assert.That(result[0].ToHexSting(), Is.EqualTo("09"));
            Assert.That(result[1].ToHexSting(), Is.EqualTo("01"));
            Assert.That(result[2].ToHexSting(), Is.EqualTo("1E"));
        }

        [Test]
        public void Should_ConvertWordSign_AsSingleWord()
        {
            var result = ConvertText("knowledge");
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Text, Is.EqualTo("knowledge"));
            Assert.That(result[0].ToHexSting(), Is.EqualTo("05"));
        }

        [Test]
        public void Should_ConvertGroupSign_AsSingleWord()
        {
            var result = ConvertText("and");
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Text, Is.EqualTo("and"));
            Assert.That(result[0].ToHexSting(), Is.EqualTo("2F"));
        }

        [Test]
        public void Should_ConvertMixedSentence_Correctly()
        {
            var result = ConvertText("you like it");
            Assert.That(result.Count, Is.EqualTo(5), "Should be 5 words including spaces");

            Assert.That(result[0].Text, Is.EqualTo("you"));
            Assert.That(result[0].ToHexSting(), Is.EqualTo("3D"));

            Assert.That(result[1].IsWhiteSpace, Is.True);

            Assert.That(result[2].Text, Is.EqualTo("like"));
            Assert.That(result[2].ToHexSting(), Is.EqualTo("07"));

            Assert.That(result[3].IsWhiteSpace, Is.True);

            Assert.That(result[4].Text, Is.EqualTo("it"));
            Assert.That(result[4].ToHexSting(), Is.EqualTo("2D"));
        }

        // This test is expected to FAIL with the current simple greedy algorithm.
        // It demonstrates the need for more advanced rules.
        [Test]
        public void Should_PrioritizeLongerContraction_GreedyTest()
        {
            // "about" is a short-form word, "ab" is its representation.
            // The converter should match "about" not "ab".
            var result = ConvertText("about");
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Text, Is.EqualTo("about"));
            Assert.That(result[0].ToHexSting(), Is.EqualTo("0103"));
        }
    }
}