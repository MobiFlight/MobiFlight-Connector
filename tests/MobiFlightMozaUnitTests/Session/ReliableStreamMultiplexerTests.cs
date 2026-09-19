using System;
using MobiFlightMoza.Protocol;
using MobiFlightMoza.Session;
using MobiFlightMoza.Tests.Mocks;

namespace MobiFlightMoza.Session.Tests
{
    [TestClass]
    public class ReliableStreamMultiplexerTests
    {
        private static readonly DateTime Now = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static (ReliableStreamMultiplexer multiplexer, RecordingFrameSink sink) Create(params ushort[] acceptedPorts)
        {
            var sink = new RecordingFrameSink();
            var multiplexer = new ReliableStreamMultiplexer(sink, acceptedPorts);
            return (multiplexer, sink);
        }

        private static StreamRequest Syn1(ushort destinationPort = 9020, ushort isn = 0x1237, ushort announcedPort = 0x1234, byte version = 3)
            => new(destinationPort, StreamMessageType.Syn1, isn, announcedPort: announcedPort, version: version);

        #region SYN1 handling

        [TestMethod]
        public void HandleStreamMessage_Syn1OnAcceptedPort_SendsSyn2()
        {
            // Arrange
            var (multiplexer, sink) = Create(9020);
            var payload = ReliableStreamFrame.PackSyn(9020, StreamMessageType.Syn1, 0x1237, 0x1234, 3);

            // Act
            multiplexer.HandleStreamMessage(payload, isReply: false, Now);

            // Assert
            Assert.HasCount(1, sink.SentWires);
            CollectionAssert.AreEqual(
                ReliableStreamFrame.PackSyn(0x1234, StreamMessageType.Syn2, 0x1234, 0x1234, ReliableStreamFrame.NegotiatedVersion),
                sink.SentWires[0]);
        }

        [TestMethod]
        public void HandleStreamMessage_Syn1OnUnacceptedPort_IsIgnored()
        {
            // Arrange
            var (multiplexer, sink) = Create(9020); // 9050 not accepted
            var payload = ReliableStreamFrame.PackSyn(9050, StreamMessageType.Syn1, 1, 1, 3);

            // Act
            multiplexer.HandleStreamMessage(payload, isReply: false, Now);

            // Assert
            Assert.IsEmpty(sink.SentWires);
        }

        [TestMethod]
        public void AllocateLocalPort_CollidesWithReservedServicePort_SkipsToNextFreePort()
        {
            // Arrange
            var (multiplexer, sink) = Create(9020, 9050);
            // Announced port equals a reserved service port (9050) - must not reuse it.
            var payload = ReliableStreamFrame.PackSyn(9020, StreamMessageType.Syn1, 1, 9050, 3);

            // Act
            multiplexer.HandleStreamMessage(payload, isReply: false, Now);
            multiplexer.TryGetConnection(9020, out var connection);

            // Assert
            Assert.AreNotEqual((ushort)9050, connection.LocalPort);
        }

        #endregion

        #region Establishment

        [TestMethod]
        public void HandleStreamMessage_AckOfLocalPort_RaisesConnectionEstablished()
        {
            // Arrange
            var (multiplexer, _) = Create(9020);
            multiplexer.HandleStreamMessage(ReliableStreamFrame.PackSyn(9020, StreamMessageType.Syn1, 1, 0x1234, 3), false, Now);
            ushort? establishedOn = null;
            multiplexer.ConnectionEstablished += port => establishedOn = port;

            // Act
            multiplexer.HandleStreamMessage(ReliableStreamFrame.PackAck(0x1234, 0x1234), isReply: true, Now);

            // Assert
            Assert.AreEqual((ushort)9020, establishedOn);
        }

        #endregion

        #region Acknowledgement

        [TestMethod]
        public void HandleStreamMessage_Trans_SendsAckFlaggedAsReply()
        {
            // Arrange - the guide requires an ACK to use the tunnel's inner reply command
            // (0xFC), distinct from every request type (SYN1/SYN2/TRANS/FIN all share the
            // request command and carry their own Magic byte instead). A sink that forgets
            // to flag its ACK sends produces a wire the device can't recognize as an ACK at
            // all, so it never stops retransmitting.
            var (multiplexer, sink) = Create(9050);
            multiplexer.HandleStreamMessage(ReliableStreamFrame.PackSyn(9050, StreamMessageType.Syn1, 1, 0x2000, 3), false, Now);
            multiplexer.HandleStreamMessage(ReliableStreamFrame.PackAck(0x2000, 0x2000), true, Now);
            sink.SentWires.Clear();
            sink.SentIsReply.Clear();

            // Act - destination is the connection's local port (0x2000); ISN 2 is the next
            // one expected after the SYN1's ISN of 1.
            multiplexer.HandleStreamMessage(ReliableStreamFrame.PackTrans(0x2000, 2, [1, 2, 3]), isReply: false, Now);

            // Assert
            Assert.HasCount(1, sink.SentWires);
            Assert.IsTrue(sink.SentIsReply[0]);
        }

        #endregion

        #region Outgoing data

        [TestMethod]
        public void TrySend_ChunksOver54Bytes_SendsOnlyFirstChunkOnTick()
        {
            // Arrange
            var (multiplexer, sink) = Create(9050);
            multiplexer.HandleStreamMessage(ReliableStreamFrame.PackSyn(9050, StreamMessageType.Syn1, 1, 0x2000, 3), false, Now);
            multiplexer.HandleStreamMessage(ReliableStreamFrame.PackAck(0x2000, 0x2000), true, Now);
            sink.SentWires.Clear();

            byte[] data = new byte[MozaConstants.MaxApplicationChunkLength + 10];

            // Act
            multiplexer.TrySend(9050, data);
            multiplexer.Tick(Now);

            // Assert
            Assert.HasCount(1, sink.SentWires); // second chunk waits for the first's ACK
        }

        [TestMethod]
        public void TrySend_UnknownServicePort_ReturnsFalse()
        {
            // Arrange
            var (multiplexer, _) = Create(9050);

            // Act
            var sent = multiplexer.TrySend(9050, [1]);

            // Assert
            Assert.IsFalse(sent);
        }

        #endregion

        #region Tick: retransmit and heartbeat

        [TestMethod]
        public void Tick_UnackedTrans_RetransmitsAfter600ms()
        {
            // Arrange
            var (multiplexer, sink) = Create(9050);
            multiplexer.HandleStreamMessage(ReliableStreamFrame.PackSyn(9050, StreamMessageType.Syn1, 1, 0x2000, 3), false, Now);
            multiplexer.HandleStreamMessage(ReliableStreamFrame.PackAck(0x2000, 0x2000), true, Now);
            multiplexer.TrySend(9050, [1, 2, 3]);
            multiplexer.Tick(Now);
            var firstWire = sink.SentWires[^1];

            // Act
            multiplexer.Tick(Now.AddMilliseconds(601));

            // Assert
            CollectionAssert.AreEqual(firstWire, sink.SentWires[^1]);
        }

        [TestMethod]
        public void Tick_IdleEstablishedConnection_SendsHeartbeatAfter3Seconds()
        {
            // Arrange
            var (multiplexer, sink) = Create(9050);
            multiplexer.HandleStreamMessage(ReliableStreamFrame.PackSyn(9050, StreamMessageType.Syn1, 1, 0x2000, 3), false, Now);
            multiplexer.HandleStreamMessage(ReliableStreamFrame.PackAck(0x2000, 0x2000), true, Now);
            sink.SentWires.Clear();

            // Act
            multiplexer.Tick(Now.AddSeconds(3));

            // Assert
            Assert.HasCount(1, sink.SentWires);
            ReliableStreamFrame.TryParseRequest(sink.SentWires[0], out var request, out _);
            Assert.AreEqual(StreamMessageType.Trans, request.MessageType);
            Assert.IsEmpty(request.ApplicationData);
        }

        #endregion

        #region Close

        [TestMethod]
        public void BeginClose_FinsEstablishedConnection()
        {
            // Arrange
            var (multiplexer, sink) = Create(9050);
            multiplexer.HandleStreamMessage(ReliableStreamFrame.PackSyn(9050, StreamMessageType.Syn1, 1, 0x2000, 3), false, Now);
            multiplexer.HandleStreamMessage(ReliableStreamFrame.PackAck(0x2000, 0x2000), true, Now);
            sink.SentWires.Clear();

            // Act
            multiplexer.BeginClose(Now);

            // Assert
            Assert.HasCount(1, sink.SentWires);
            ReliableStreamFrame.TryParseRequest(sink.SentWires[0], out var request, out _);
            Assert.AreEqual(StreamMessageType.Fin, request.MessageType);
        }

        [TestMethod]
        public void HandleStreamMessage_Syn1WhileClosing_IsIgnored()
        {
            // Arrange
            var (multiplexer, sink) = Create(9050);
            multiplexer.BeginClose(Now);
            sink.SentWires.Clear();

            // Act
            multiplexer.HandleStreamMessage(ReliableStreamFrame.PackSyn(9050, StreamMessageType.Syn1, 1, 0x2000, 3), false, Now);

            // Assert
            Assert.IsEmpty(sink.SentWires);
        }

        [TestMethod]
        public void IsCloseComplete_NoConnections_IsTrue()
        {
            // Arrange
            var (multiplexer, _) = Create(9050);

            // Act & Assert
            Assert.IsTrue(multiplexer.IsCloseComplete);
        }

        #endregion
    }
}
