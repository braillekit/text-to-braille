using System;
using EasyBrailleEdit.Common;
using EasyBrailleEdit.Services;
using Xunit;

namespace EasyBrailleEdit.Tests.Services
{
    public class BrailleConverterFactoryTests
    {
        [Fact]
        public void CreateConverter_WhenUseInProcessConversionIsTrue_ShouldReturnInProcessConverter()
        {
            // Arrange
            AppGlobals.Config.Braille.UseInProcessConversion = true;

            // Act
            using var converter = BrailleConverterFactory.CreateConverter();

            // Assert
            Assert.IsType<InProcessBrailleConverter>(converter);
        }

        [Fact]
        public void CreateConverter_WhenUseInProcessConversionIsFalse_ShouldReturnExternalConverter()
        {
            // Arrange
            AppGlobals.Config.Braille.UseInProcessConversion = false;

            // Act
            using var converter = BrailleConverterFactory.CreateConverter();

            // Assert
            Assert.IsType<ExternalBrailleConverter>(converter);
        }

        [Fact]
        public void CreateConverter_ShouldReturnNonNullConverter()
        {
            // Act
            using var converter = BrailleConverterFactory.CreateConverter();

            // Assert
            Assert.NotNull(converter);
        }
    }
}
