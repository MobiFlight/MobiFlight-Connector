using MobiFlightMoza.Protocol;
using MobiFlightMoza.Tests;

namespace MobiFlightMoza.Protocol.Tests
{
    [TestClass]
    public class ReliableStreamFrameTests
    {
        #region Golden vectors (literal worked examples from the protocol documentation)

        [TestMethod]
        public void PackSyn_Syn1FromGuideExample_MatchesTunnelPayload()
        {
            // Arrange
            // "7e 0a c3 21 7c 23 3c 80 37 12 34 12 fc 03 62" - dest port 9020, ISN
            // 0x1237, announced port 0x1234, device declares version 3 (v4).

            // Act
            var payload = ReliableStreamFrame.PackSyn(9020, StreamMessageType.Syn1, 0x1237, 0x1234, 3);

            // Assert
            CollectionAssert.AreEqual(TestHex.Parse("23 3c 80 37 12 34 12 fc 03"), payload);
        }

        [TestMethod]
        public void PackSyn_Syn2FromGuideExample_MatchesTunnelPayload()
        {
            // Arrange
            // "7e 0a 43 12 7c 12 34 81 34 12 34 12 fd 02 b8" - SYN2's ISN and
            // AnnouncedPort fields both carry local_port - not something you'd guess
            // from the field names alone, so it's pinned here explicitly.

            // Act
            var payload = ReliableStreamFrame.PackSyn(0x1234, StreamMessageType.Syn2, 0x1234, 0x1234, 2);

            // Assert
            CollectionAssert.AreEqual(TestHex.Parse("12 34 81 34 12 34 12 fd 02"), payload);
        }

        [TestMethod]
        public void PackAck_CumulativeAckFromGuideExample_MatchesTunnelPayload()
        {
            // Arrange
            // "7e 05 43 12 fc 12 34 38 12 71" - ACK of ISN 0x1238 on dest port 0x1234.

            // Act
            var payload = ReliableStreamFrame.PackAck(0x1234, 0x1238);

            // Assert
            CollectionAssert.AreEqual(TestHex.Parse("12 34 38 12"), payload);
        }

        [TestMethod]
        public void PackTrans_HeartbeatFromGuideExample_MatchesTunnelPayload()
        {
            // Arrange
            // "7e 0a 43 12 7c 12 34 01 37 12 00 00 00 00 f6" - empty TRANS, v3's CRC
            // trailer for empty data is 00 00 00 00.

            // Act
            var payload = ReliableStreamFrame.PackTrans(0x1234, 0x1237, []);

            // Assert
            CollectionAssert.AreEqual(TestHex.Parse("12 34 01 37 12 00 00 00 00"), payload);
        }

        [TestMethod]
        public void PackFin_FromGuideExample_MatchesTunnelPayload()
        {
            // Arrange
            // "7e 06 43 12 7c 12 34 00 37 12 f1"

            // Act
            var payload = ReliableStreamFrame.PackFin(0x1234, 0x1237);

            // Assert
            CollectionAssert.AreEqual(TestHex.Parse("12 34 00 37 12"), payload);
        }

        [TestMethod]
        public void TryParseAck_SynAckCompletingHandshake_ParsesCorrectly()
        {
            // Arrange
            // Device's ACK completing a SYN2 handshake: "7e 05 c3 21 fc 12 34 34 12 fc" -
            // tunnel payload (after the inner command byte fc) is "12 34 34 12".
            var payload = TestHex.Parse("12 34 34 12");

            // Act
            var parsed = ReliableStreamFrame.TryParseAck(payload, out var ack);

            // Assert
            Assert.IsTrue(parsed);
            Assert.AreEqual((ushort)0x1234, ack.DestinationPort);
            Assert.AreEqual((ushort)0x1234, ack.AcknowledgedIsn);
        }

        [TestMethod]
        public void TryParseRequest_Syn1FromGuideExample_ParsesAllFields()
        {
            // Arrange
            var payload = TestHex.Parse("23 3c 80 37 12 34 12 fc 03");

            // Act
            var parsed = ReliableStreamFrame.TryParseRequest(payload, out var request, out _);

            // Assert
            Assert.IsTrue(parsed);
            Assert.AreEqual((ushort)9020, request.DestinationPort);
            Assert.AreEqual(StreamMessageType.Syn1, request.MessageType);
            Assert.AreEqual((ushort)0x1237, request.Isn);
            Assert.AreEqual((ushort)0x1234, request.AnnouncedPort);
            Assert.AreEqual((byte)3, request.Version);
        }

        #endregion

        #region Validation

        [TestMethod]
        public void TryParseRequest_SynWithBadInverseVersion_Fails()
        {
            // Arrange
            var payload = TestHex.Parse("23 3c 80 37 12 34 12 00 03"); // inverse should be fc

            // Act
            var parsed = ReliableStreamFrame.TryParseRequest(payload, out _, out string error);

            // Assert
            Assert.IsFalse(parsed);
            Assert.IsNotNull(error);
        }

        [TestMethod]
        public void TryParseRequest_TransWithBadCrc_Fails()
        {
            // Arrange
            var payload = TestHex.Parse("12 34 01 37 12 ff ff ff ff"); // wrong CRC for empty data

            // Act
            var parsed = ReliableStreamFrame.TryParseRequest(payload, out _, out string error);

            // Assert
            Assert.IsFalse(parsed);
            Assert.IsNotNull(error);
        }

        [TestMethod]
        public void PackTrans_RoundTripsThroughTryParseRequest()
        {
            // Arrange
            byte[] appData = [1, 2, 3, 4, 5];
            var payload = ReliableStreamFrame.PackTrans(0x1234, 0x0042, appData);

            // Act
            var parsed = ReliableStreamFrame.TryParseRequest(payload, out var request, out _);

            // Assert
            Assert.IsTrue(parsed);
            Assert.AreEqual(StreamMessageType.Trans, request.MessageType);
            CollectionAssert.AreEqual(appData, request.ApplicationData);
        }

        [TestMethod]
        public void PackTrans_ApplicationDataOver54Bytes_Throws()
        {
            // Arrange
            var data = new byte[MozaConstants.MaxApplicationChunkLength + 1];

            // Act & Assert
            Assert.ThrowsExactly<System.ArgumentException>(() => ReliableStreamFrame.PackTrans(0x1234, 1, data));
        }

        #endregion
    }
}
