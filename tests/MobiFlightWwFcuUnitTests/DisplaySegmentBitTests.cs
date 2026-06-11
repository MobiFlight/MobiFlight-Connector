using MobiFlightWwFcu;

namespace MobiFlightWwFcuUnitTests
{
    [TestClass]
    public class DisplaySegmentBitTests
    {
        #region Bit constructor

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(7)]
        public void BitCtor_PositionInRange_Constructs(int bitPosition)
        {
            var bit = new Bit(5, bitPosition);
            Assert.AreEqual(5, bit.ByteNumber);
            Assert.AreEqual(bitPosition, bit.BitPosition);
            Assert.IsFalse(bit.Value);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(8)]
        [DataRow(-100)]
        [DataRow(100)]
        public void BitCtor_PositionOutOfRange_ThrowsArgumentOutOfRangeException(int bitPosition)
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => new Bit(0, bitPosition));
        }

        [TestMethod]
        public void BitCtor_WithExplicitValueTrue_StoresValue()
        {
            var bit = new Bit(3, 4, true);
            Assert.IsTrue(bit.Value);
        }

        [TestMethod]
        public void BitValue_IsMutable()
        {
            var bit = new Bit(0, 0, false);
            bit.Value = true;
            Assert.IsTrue(bit.Value);
        }

        #endregion

        #region DisplaySegment constructors

        [TestMethod]
        public void DisplaySegmentCtor_WithBitArrayAndFlag_StoresBoth()
        {
            var bits = new[] { new Bit(0, 0), new Bit(0, 1) };
            var segment = new DisplaySegment(bits, isSevenSegment: true);
            Assert.AreSame(bits, segment.Bits);
            Assert.IsTrue(segment.IsSevenSegment);
        }

        [TestMethod]
        public void DisplaySegmentCtor_SingleBit_WrapsAsIndicator()
        {
            var bit = new Bit(2, 3);
            var segment = new DisplaySegment(bit);
            Assert.HasCount(1, segment.Bits);
            Assert.AreSame(bit, segment.Bits[0]);
            Assert.IsFalse(segment.IsSevenSegment);
        }

        [TestMethod]
        public void DisplaySegmentCtor_SevenSegmentReverseTrue_BitsAreInDescendingByteOrder()
        {
            // topByte=24, bitNumber=3, isReverse=true (default):
            // bits at (topByte - i*4) for i = 0..6 → 24,20,16,12,8,4,0
            var segment = new DisplaySegment(topByte: 24, bitNumber: 3, initChar: '*');

            Assert.IsTrue(segment.IsSevenSegment);
            Assert.HasCount(7, segment.Bits);
            int[] expectedByteNumbers = { 24, 20, 16, 12, 8, 4, 0 };
            for (int i = 0; i < 7; i++)
            {
                Assert.AreEqual(expectedByteNumbers[i], segment.Bits[i].ByteNumber, $"Bits[{i}].ByteNumber");
                Assert.AreEqual(3, segment.Bits[i].BitPosition, $"Bits[{i}].BitPosition");
            }
        }

        [TestMethod]
        public void DisplaySegmentCtor_SevenSegmentReverseFalse_BitsAreInAscendingByteOrder()
        {
            // topByte=24, bitNumber=3, isReverse=false:
            // bits at (topByte - i*4) for i = 6..0 → 0,4,8,12,16,20,24
            var segment = new DisplaySegment(topByte: 24, bitNumber: 3, initChar: '*', isReverse: false);

            Assert.IsTrue(segment.IsSevenSegment);
            Assert.HasCount(7, segment.Bits);
            int[] expectedByteNumbers = { 0, 4, 8, 12, 16, 20, 24 };
            for (int i = 0; i < 7; i++)
            {
                Assert.AreEqual(expectedByteNumbers[i], segment.Bits[i].ByteNumber, $"Bits[{i}].ByteNumber");
                Assert.AreEqual(3, segment.Bits[i].BitPosition, $"Bits[{i}].BitPosition");
            }
        }

        [TestMethod]
        public void DisplaySegmentCtor_SevenSegmentInitCharStar_AllBitsFalse()
        {
            // '*' maps to all-false in CharacterDict
            var segment = new DisplaySegment(topByte: 24, bitNumber: 0, initChar: '*');
            foreach (var bit in segment.Bits)
            {
                Assert.IsFalse(bit.Value);
            }
        }

        #endregion

        #region SetCharacter on 7-segment

        [TestMethod]
        public void SetCharacter_Digit0_LightsExpectedSegments()
        {
            // CharacterDict['0'] = { true, true, true, true, true, true, false }
            //   top, top-right, bottom-right, bottom, bottom-left, top-left, middle
            var segment = new DisplaySegment(topByte: 24, bitNumber: 0, initChar: '*');
            segment.SetCharacter('0');

            bool[] expected = { true, true, true, true, true, true, false };
            for (int i = 0; i < 7; i++)
            {
                Assert.AreEqual(expected[i], segment.Bits[i].Value, $"Bits[{i}].Value");
            }
        }

        [TestMethod]
        public void SetCharacter_Digit1_LightsOnlyTopRightAndBottomRight()
        {
            // CharacterDict['1'] = { false, true, true, false, false, false, false }
            var segment = new DisplaySegment(topByte: 24, bitNumber: 0, initChar: '*');
            segment.SetCharacter('1');

            bool[] expected = { false, true, true, false, false, false, false };
            for (int i = 0; i < 7; i++)
            {
                Assert.AreEqual(expected[i], segment.Bits[i].Value, $"Bits[{i}].Value");
            }
        }

        [TestMethod]
        public void SetCharacter_Digit8_AllSegmentsLit()
        {
            // CharacterDict['8'] = { true, true, true, true, true, true, true }
            var segment = new DisplaySegment(topByte: 24, bitNumber: 0, initChar: '*');
            segment.SetCharacter('8');

            foreach (var bit in segment.Bits)
            {
                Assert.IsTrue(bit.Value);
            }
        }

        [TestMethod]
        public void SetCharacter_Dash_LightsOnlyMiddle()
        {
            // CharacterDict['-'] = { false, false, false, false, false, false, true }
            var segment = new DisplaySegment(topByte: 24, bitNumber: 0, initChar: '*');
            segment.SetCharacter('-');

            for (int i = 0; i < 6; i++)
            {
                Assert.IsFalse(segment.Bits[i].Value, $"Bits[{i}].Value");
            }
            Assert.IsTrue(segment.Bits[6].Value);
        }

        [TestMethod]
        public void SetCharacter_UnknownChar_LeavesBitsUnchanged()
        {
            // Initialize with '8' (all true), then attempt unknown char.
            var segment = new DisplaySegment(topByte: 24, bitNumber: 0, initChar: '8');
            segment.SetCharacter('z'); // not in CharacterDict

            foreach (var bit in segment.Bits)
            {
                Assert.IsTrue(bit.Value);
            }
        }

        #endregion

        #region SetCharacter on non-7-segment

        [TestMethod]
        public void SetCharacter_OnNonSevenSegment_IsNoOp()
        {
            var bits = new[]
            {
                new Bit(0, 0, false),
                new Bit(0, 1, true)
            };
            var segment = new DisplaySegment(bits, isSevenSegment: false);

            segment.SetCharacter('8');

            Assert.IsFalse(segment.Bits[0].Value);
            Assert.IsTrue(segment.Bits[1].Value);
        }

        #endregion

        #region SetValue

        [TestMethod]
        public void SetValue_IndicatorTrue_SetsAllBitsTrue()
        {
            var bits = new[]
            {
                new Bit(0, 0, false),
                new Bit(1, 0, false),
                new Bit(2, 0, false)
            };
            var segment = new DisplaySegment(bits, isSevenSegment: false);

            segment.SetValue(true);

            foreach (var bit in segment.Bits)
            {
                Assert.IsTrue(bit.Value);
            }
        }

        [TestMethod]
        public void SetValue_IndicatorFalse_SetsAllBitsFalse()
        {
            var bits = new[]
            {
                new Bit(0, 0, true),
                new Bit(1, 0, true)
            };
            var segment = new DisplaySegment(bits, isSevenSegment: false);

            segment.SetValue(false);

            foreach (var bit in segment.Bits)
            {
                Assert.IsFalse(bit.Value);
            }
        }

        [TestMethod]
        public void SetValue_OnSevenSegment_AlsoSetsAllBits()
        {
            // SetValue is not gated on IsSevenSegment, so it works there too.
            var segment = new DisplaySegment(topByte: 24, bitNumber: 0, initChar: '*');
            segment.SetValue(true);

            foreach (var bit in segment.Bits)
            {
                Assert.IsTrue(bit.Value);
            }
        }

        #endregion
    }
}
