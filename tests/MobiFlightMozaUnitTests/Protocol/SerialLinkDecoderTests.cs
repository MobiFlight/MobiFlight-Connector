using MobiFlightMoza.Protocol;
using MobiFlightMoza.Tests;

namespace MobiFlightMoza.Protocol.Tests
{
    [TestClass]
    public class SerialLinkDecoderTests
    {
        #region Whole frames

        [TestMethod]
        public void Feed_RootHandshakeResponse_YieldsOneMessage()
        {
            // Arrange
            var decoder = new SerialLinkDecoder();

            // Act
            var messages = decoder.Feed(TestHex.Parse("7e 00 80 21 2c"), 5);

            // Assert
            Assert.HasCount(1, messages);
            Assert.AreEqual(0x80, messages[0].Command);
            Assert.AreEqual(0x21, messages[0].DevicePair);
            Assert.IsEmpty(messages[0].Payload);
        }

        [TestMethod]
        public void Feed_TunnelFrame_PreservesCommandDevicePairAndPayload()
        {
            // Arrange
            var decoder = new SerialLinkDecoder();
            var raw = TestHex.Parse("7e 01 43 12 00 e1");

            // Act
            var messages = decoder.Feed(raw, raw.Length);

            // Assert
            Assert.HasCount(1, messages);
            Assert.AreEqual(0x43, messages[0].Command);
            Assert.AreEqual(0x12, messages[0].DevicePair);
            CollectionAssert.AreEqual((byte[])[0x00], messages[0].Payload);
        }

        [TestMethod]
        public void Feed_EscapedSofInPayload_DecodesSingleSemanticByte()
        {
            // Arrange
            var decoder = new SerialLinkDecoder();
            var raw = TestHex.Parse("7e 05 43 12 7c 23 3c 01 7e 7e bd");

            // Act
            var messages = decoder.Feed(raw, raw.Length);

            // Assert
            Assert.HasCount(1, messages);
            CollectionAssert.AreEqual(TestHex.Parse("7c 23 3c 01 7e"), messages[0].Payload);
        }

        [TestMethod]
        public void Feed_TwoFramesInOneRead_YieldsBoth()
        {
            // Arrange
            var decoder = new SerialLinkDecoder();
            var raw = TestHex.Parse("7e 00 80 21 2c 7e 01 c3 21 80 f0");

            // Act
            var messages = decoder.Feed(raw, raw.Length);

            // Assert
            Assert.HasCount(2, messages);
            Assert.AreEqual(0x80, messages[0].Command);
            Assert.AreEqual(0xC3, messages[1].Command);
        }

        #endregion

        #region Fragmented reads

        [TestMethod]
        public void Feed_ByteByByte_YieldsSameMessageAsOneRead()
        {
            // Arrange
            var decoder = new SerialLinkDecoder();
            var raw = TestHex.Parse("7e 01 43 12 00 e1");

            // Act
            SerialLinkMessage message = null;
            foreach (byte b in raw)
            {
                var chunk = decoder.Feed([b], 1);
                if (chunk.Count > 0) message = chunk[0];
            }

            // Assert
            Assert.IsNotNull(message);
            Assert.AreEqual(0x43, message.Command);
        }

        [TestMethod]
        public void Feed_SplitInsideEscapedSof_WaitsForSecondByte()
        {
            // Arrange
            var decoder = new SerialLinkDecoder();
            var raw = TestHex.Parse("7e 05 43 12 7c 23 3c 01 7e 7e bd");

            // Act
            var firstChunk = decoder.Feed(raw[..9], 9); // ends right after the first 0x7e of the escape pair
            var secondChunk = decoder.Feed(raw[9..], raw.Length - 9);

            // Assert
            Assert.IsEmpty(firstChunk);
            Assert.HasCount(1, secondChunk);
        }

        #endregion

        #region Recovery

        [TestMethod]
        public void Feed_GarbageBeforeSof_SkipsToSof()
        {
            // Arrange
            var decoder = new SerialLinkDecoder();
            byte[] raw = [0x01, 0x02, 0x03, .. TestHex.Parse("7e 00 80 21 2c")];

            // Act
            var messages = decoder.Feed(raw, raw.Length);

            // Assert
            Assert.HasCount(1, messages);
            Assert.AreEqual(0x80, messages[0].Command);
        }

        [TestMethod]
        public void Feed_BadChecksum_DropsFrameAndResyncsOnNextSof()
        {
            // Arrange
            var decoder = new SerialLinkDecoder();
            var corrupted = TestHex.Parse("7e 00 80 21 00"); // wrong checksum (should be 2c)
            var good = TestHex.Parse("7e 01 43 12 00 e1");
            byte[] raw = [.. corrupted, .. good];

            // Act
            var messages = decoder.Feed(raw, raw.Length);

            // Assert
            Assert.HasCount(1, messages);
            Assert.AreEqual(0x43, messages[0].Command);
        }

        [TestMethod]
        public void Feed_EmptyRead_YieldsNothing()
        {
            // Arrange
            var decoder = new SerialLinkDecoder();

            // Act
            var messages = decoder.Feed([], 0);

            // Assert
            Assert.IsEmpty(messages);
        }

        #endregion
    }
}
