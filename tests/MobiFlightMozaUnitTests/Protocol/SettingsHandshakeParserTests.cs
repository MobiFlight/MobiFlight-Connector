using System.Collections.Generic;
using MobiFlightMoza.Protocol;
using MobiFlightMoza.Tests;

namespace MobiFlightMoza.Protocol.Tests
{
    [TestClass]
    public class SettingsHandshakeParserTests
    {
        // Header + marker + McuIdLength=2 + SettingCount=3 + McuId(AA BB) + SupportedIds(01 04 0A).
        private static readonly byte[] FullPreamble = TestHex.Parse("07 01 00 00 00 00 02 03 aa bb 01 04 0a");

        [TestMethod]
        public void TryParse_CompletePreamble_ExtractsMcuIdAndSupportedIds()
        {
            // Arrange
            List<byte> buffer = [.. FullPreamble];

            // Act
            var parsed = SettingsHandshakeParser.TryParse(buffer, out byte[] mcuId, out var supportedIds, out int consumed);

            // Assert
            Assert.IsTrue(parsed);
            CollectionAssert.AreEqual(TestHex.Parse("aa bb"), mcuId);
            CollectionAssert.AreEqual(TestHex.Parse("01 04 0a"), (byte[])[.. supportedIds]);
            Assert.AreEqual(FullPreamble.Length, consumed);
        }

        [TestMethod]
        public void TryParse_SplitBeforeHeaderIsFullyKnown_ReturnsFalse()
        {
            // Arrange
            List<byte> buffer = [.. FullPreamble[..6]];

            // Act
            var parsed = SettingsHandshakeParser.TryParse(buffer, out _, out _, out int consumed);

            // Assert
            Assert.IsFalse(parsed);
            Assert.AreEqual(0, consumed);
        }

        [TestMethod]
        public void TryParse_SplitInsideVariableLengthFields_ReturnsFalseThenTrue()
        {
            // Arrange
            List<byte> buffer = [.. FullPreamble[..9]]; // header + marker + lengths + 1 of 2 McuId bytes

            // Act
            var firstAttempt = SettingsHandshakeParser.TryParse(buffer, out _, out _, out _);
            buffer.AddRange(FullPreamble[9..]);
            var secondAttempt = SettingsHandshakeParser.TryParse(buffer, out byte[] mcuId, out _, out _);

            // Assert
            Assert.IsFalse(firstAttempt);
            Assert.IsTrue(secondAttempt);
            CollectionAssert.AreEqual(TestHex.Parse("aa bb"), mcuId);
        }

        [TestMethod]
        public void TryParse_WrongHeader_Throws()
        {
            // Arrange
            List<byte> buffer = [.. TestHex.Parse("00 01 00 00 00 00 00 00")];

            // Act & Assert
            Assert.ThrowsExactly<System.InvalidOperationException>(() => SettingsHandshakeParser.TryParse(buffer, out _, out _, out _));
        }

        [TestMethod]
        public void TryParse_WrongHandshakeMarker_Throws()
        {
            // Arrange
            List<byte> buffer = [.. TestHex.Parse("07 01 00 00 00 01 00 00")];

            // Act & Assert
            Assert.ThrowsExactly<System.InvalidOperationException>(() => SettingsHandshakeParser.TryParse(buffer, out _, out _, out _));
        }
    }
}
