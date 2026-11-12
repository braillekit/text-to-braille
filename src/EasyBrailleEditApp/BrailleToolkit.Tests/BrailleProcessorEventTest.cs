using System.Collections.Generic;
using Xunit;

namespace BrailleToolkit.Tests
{
    [Collection("Singleton-Sensitive Tests")]
    public class BrailleProcessorEventTest
    {
        public BrailleProcessorEventTest()
        {
            Shared.SetupLogger();
        }

        [Fact]
        public void TextConverted_ShouldCreateNewEventArgsPerRaise()
        {
            var processor = BrailleProcessor.CreateInstance();
            var eventArgs = new List<TextConvertedEventArgs>();

            processor.TextConverted += (_, args) => eventArgs.Add(args);

            processor.ConvertLine("A夏");

            Assert.True(eventArgs.Count >= 2);
            Assert.NotSame(eventArgs[0], eventArgs[1]);
            Assert.Equal("A", eventArgs[0].Text);
            Assert.Equal("夏", eventArgs[1].Text);
        }

        [Fact]
        public void ConvertionFailed_ShouldCreateNewEventArgsPerRaise()
        {
            var processor = BrailleProcessor.CreateInstance();
            var eventArgs = new List<ConversionFailedEventArgs>();

            processor.ConvertionFailed += (_, args) => eventArgs.Add(args);

            processor.ConvertLine("ЖЖ");

            Assert.True(eventArgs.Count >= 2);
            Assert.NotSame(eventArgs[0], eventArgs[1]);
            Assert.Equal('Ж', eventArgs[0].InvalidChar.CharValue);
            Assert.Equal('Ж', eventArgs[1].InvalidChar.CharValue);
        }
    }
}
