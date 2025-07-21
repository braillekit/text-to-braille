using BrailleToolkit;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleToolkit.Tests
{
    public class BrailleCellTest
    {
        public BrailleCellTest()
        {
            Shared.SetupLogger();
        }

        public static IEnumerable<object[]> Should_CreateInstanceWithDotNumbers_Data =>
            new List<object[]>
            {
                new object[] { new int[] { 1 }, (byte)0x01 },
                new object[] { new int[] { 1, 2 }, (byte)0x03 },
                new object[] { new int[] { 1, 3, 6 }, (byte)0x25 },
                new object[] { new int[] { 2, 3, 4, 5 }, (byte)0x1E },
                new object[] { new int[] { 1, 2, 3, 4, 5 }, (byte)0x1F },
                new object[] { new int[] { 1, 2, 3, 4, 5, 6 }, (byte)0x3F },
            };

        [Theory]
        [MemberData(nameof(Should_CreateInstanceWithDotNumbers_Data))]
        public void Should_CreateInstanceWithDotNumbers(int[] dotNumbers, byte expectedValue)
        {
            var brCell = BrailleCell.GetInstance(dotNumbers);
            Assert.Equal(expectedValue, brCell.Value);
        }

        [Theory]
        [InlineData(0x01, "1")]
        [InlineData(0x03, "12")]
        [InlineData(0x25, "136")]
        [InlineData(0x1E, "2345")]
        [InlineData(0x1F, "12345")]
        [InlineData(0x3F, "123456")]
        public void Should_GetDotNumberString(byte brailleValue, string expectedDots)
        {
            var brCell = BrailleCell.GetInstance(brailleValue);
            Assert.Equal(expectedDots, brCell.ToPositionNumberString());
        }

        public static IEnumerable<object[]> Should_GetDotNumberArray_Data =>
            new List<object[]>
            {
                new object[] { (byte)0x01, new int[] { 1 } },
                new object[] { (byte)0x03, new int[] { 1, 2 } },
                new object[] { (byte)0x25, new int[] { 1, 3, 6 } },
                new object[] { (byte)0x1E, new int[] { 2, 3, 4, 5 } },
                new object[] { (byte)0x1F, new int[] { 1, 2, 3, 4, 5 } },
                new object[] { (byte)0x3F, new int[] { 1, 2, 3, 4, 5, 6 } },
            };

        [Theory]
        [MemberData(nameof(Should_GetDotNumberArray_Data))]
        public void Should_GetDotNumberArray(byte brailleValue, int[] expectedDots)
        {
            var brCell = BrailleCell.GetInstance(brailleValue);
            Assert.Equal(expectedDots, brCell.ToPositionNumberArray());
        }
    }
}