using BrailleToolkit;
using Xunit;
using System;
using System.Collections.Generic;
using EasyBrailleEdit.Common.Utilities;

namespace BrailleToolkit.Tests
{
    [Collection("Singleton-Sensitive Tests")]
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

        [Fact]
        public void Should_UseValueEquality()
        {
            var left = BrailleCell.GetInstance(0x25);
            var right = new BrailleCell(0x25);

            Assert.Equal(left, right);
            Assert.True(left == right);
            Assert.Equal(left.GetHashCode(), right.GetHashCode());
        }

        [Fact]
        public void Should_TreatDefaultValueAsBlank()
        {
            BrailleCell cell = default;

            Assert.Equal(BrailleCell.Blank, cell);
            Assert.Equal((byte)0x00, cell.Value);
            Assert.Equal("00", cell.ToString());
        }

        [Fact]
        public void Should_RoundTripThroughDataContractJsonSerializer()
        {
            var original = BrailleCell.GetInstance("3A");

            string json = JsonHelper.Serialize(original);
            var deserialized = JsonHelper.Deserialize<BrailleCell>(json);

            Assert.Contains("\"Value\":58", json);
            Assert.Equal(original, deserialized);
        }
    }
}
