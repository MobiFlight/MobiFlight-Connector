using MobiFlightMoza.Protocol;
using MobiFlightMoza.Tests;

namespace MobiFlightMoza.Protocol.Tests
{
    [TestClass]
    public class SerialLinkFrameTests
    {
        #region Golden vectors from the protocol guide

        [TestMethod]
        public void EncodeRaw_RootHandshakeRequest_MatchesGuideExample()
        {
            // Arrange
            // Empty payload, Command=0x00, DevicePair=0x12 (guide §2.2).

            // Act
            var frame = SerialLinkFrame.EncodeRaw(0x00, 0x12, []);

            // Assert
            CollectionAssert.AreEqual(TestHex.Parse("7e 00 00 12 9d"), frame);
        }

        [TestMethod]
        public void EncodeRaw_RootHandshakeResponse_MatchesGuideExample()
        {
            // Act
            var frame = SerialLinkFrame.EncodeRaw(0x80, 0x21, []);

            // Assert
            CollectionAssert.AreEqual(TestHex.Parse("7e 00 80 21 2c"), frame);
        }

        [TestMethod]
        public void EncodeRaw_DeviceInitRequest_MatchesGuideExample()
        {
            // Arrange
            // The empty FCD Display init command: inner command 0x00, no inner payload,
            // wrapped in the 0x43 tunnel (guide §2.2).

            // Act
            var frame = MozaTunnel.Wrap(0x00, []);

            // Assert
            CollectionAssert.AreEqual(TestHex.Parse("7e 01 43 12 00 e1"), frame);
        }

        [TestMethod]
        public void EncodeRaw_DeviceInitResponse_MatchesGuideExample()
        {
            // Arrange
            // Device->PC reply: outer command 0xC3, DevicePair=0x21, inner command 0x00
            // with its reply bit set (0x80) and no inner payload (guide §2.2).

            // Act
            var frame = SerialLinkFrame.EncodeRaw(0xC3, 0x21, [0x80]);

            // Assert
            CollectionAssert.AreEqual(TestHex.Parse("7e 01 c3 21 80 f0"), frame);
        }

        [TestMethod]
        public void EncodeRaw_PayloadContainingSof_EscapesAndCountsChecksumTwice()
        {
            // Arrange
            // guide §3.2's escaping example: payload's last byte is semantic 0x7E.
            var payload = TestHex.Parse("7c 23 3c 01 7e");

            // Act
            var frame = SerialLinkFrame.EncodeRaw(0x43, 0x12, payload);

            // Assert
            CollectionAssert.AreEqual(TestHex.Parse("7e 05 43 12 7c 23 3c 01 7e 7e bd"), frame);
        }

        [TestMethod]
        public void EncodeRaw_Heartbeat_MatchesGuideExample()
        {
            // Arrange
            // Empty TRANS ApplicationChunk with v3's 4-byte zero CRC trailer, dest port
            // 0x1234, ISN 0x1237 (guide §4.3).
            var payload = TestHex.Parse("7c 12 34 01 37 12 00 00 00 00");

            // Act
            var frame = SerialLinkFrame.EncodeRaw(0x43, 0x12, payload);

            // Assert
            CollectionAssert.AreEqual(TestHex.Parse("7e 0a 43 12 7c 12 34 01 37 12 00 00 00 00 f6"), frame);
        }

        [TestMethod]
        public void EncodeRaw_Fin_MatchesGuideExample()
        {
            // Arrange
            // FIN (Magic=0x00) to dest port 0x1234, ISN 0x37 (guide §4.4).
            var payload = TestHex.Parse("7c 12 34 00 37 12");

            // Act
            var frame = SerialLinkFrame.EncodeRaw(0x43, 0x12, payload);

            // Assert
            CollectionAssert.AreEqual(TestHex.Parse("7e 06 43 12 7c 12 34 00 37 12 f1"), frame);
        }

        #endregion

        #region Validation

        [TestMethod]
        public void EncodeRaw_PayloadLongerThanProtocolMaximum_Throws()
        {
            // Arrange
            var payload = new byte[MozaConstants.MaxSerialPayloadLength + 1];

            // Act & Assert
            Assert.ThrowsExactly<System.ArgumentException>(() => SerialLinkFrame.EncodeRaw(0x43, 0x12, payload));
        }

        [TestMethod]
        public void EncodeRaw_NullPayload_TreatedAsEmpty()
        {
            // Act
            var frame = SerialLinkFrame.EncodeRaw(0x00, 0x12, null);

            // Assert
            CollectionAssert.AreEqual(TestHex.Parse("7e 00 00 12 9d"), frame);
        }

        #endregion
    }
}
