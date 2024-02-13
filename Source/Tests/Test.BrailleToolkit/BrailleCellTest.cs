using BrailleToolkit;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.BrailleToolkit
{
    [TestFixture]
    public class BrailleCellTest
    {
        [SetUp]
        public void SetUp()
        {
            Shared.SetupLogger();
        }

        [TestCase(new int[] { 1 }, 0x01)]
        [TestCase(new int[] { 1, 2 }, 0x03)]
        [TestCase(new int[] { 1, 3, 6 }, 0x25)]
        [TestCase(new int[] { 2, 3, 4, 5 }, 0x1E)]
        [TestCase(new int[] { 1, 2, 3, 4, 5 }, 0x1F)]
        [TestCase(new int[] { 1, 2, 3, 4, 5, 6 }, 0x3F)]
        public void Should_CreateInstanceWithDotNumbers(int[] dotNumbers, byte expectedValue)
        {
            var brCell = BrailleCell.GetInstance(dotNumbers);
            Assert.That(brCell.Value, Is.EqualTo(expectedValue));
        }

        [TestCase(0x01, "1")]
        [TestCase(0x03, "12")]
        [TestCase(0x25, "136")]
        [TestCase(0x1E, "2345")]
        [TestCase(0x1F, "12345")]
        [TestCase(0x3F, "123456")]
        public void Should_GetDotNumberString(byte brailleValue, string expectedDots)
        {
            var brCell = BrailleCell.GetInstance(brailleValue);
            Assert.That(brCell.ToPositionNumberString(), Is.EqualTo(expectedDots));
        }

        [TestCase(0x01, new int[] { 1 })]
        [TestCase(0x03, new int[] { 1, 2 })]
        [TestCase(0x25, new int[] { 1, 3, 6 })]
        [TestCase(0x1E, new int[] { 2, 3, 4, 5 })]
        [TestCase(0x1F, new int[] { 1, 2, 3, 4, 5 })]
        [TestCase(0x3F, new int[] { 1, 2, 3, 4, 5, 6 })]
        public void Should_GetDotNumberArray(byte brailleValue, int[] expectedDots)
        {
            var brCell = BrailleCell.GetInstance(brailleValue);
            Assert.That(brCell.ToPositionNumberArray(), Is.EqualTo(expectedDots));
        }
    }
}
