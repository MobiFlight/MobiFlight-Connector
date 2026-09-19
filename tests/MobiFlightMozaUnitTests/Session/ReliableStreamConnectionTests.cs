using System;
using MobiFlightMoza.Protocol;
using MobiFlightMoza.Session;

namespace MobiFlightMoza.Session.Tests
{
    [TestClass]
    public class ReliableStreamConnectionTests
    {
        private static readonly DateTime Now = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static StreamRequest Syn1(ushort destinationPort = 9020, ushort isn = 0x1237, ushort announcedPort = 0x1234, byte version = 3)
            => new(destinationPort, StreamMessageType.Syn1, isn, announcedPort: announcedPort, version: version);

        #region FromSyn1

        [TestMethod]
        public void FromSyn1_GuideExampleValues_ComputesExpectedFields()
        {
            // Act
            var connection = ReliableStreamConnection.FromSyn1(9020, 0x1234, Syn1(), Now);

            // Assert
            Assert.AreEqual((ushort)9020, connection.ServicePort);
            Assert.AreEqual((ushort)0x1234, connection.LocalPort);
            Assert.AreEqual((ushort)0x1234, connection.PeerPort);
            Assert.AreEqual((ushort)0x1237, connection.PeerLastIsn);
            Assert.AreEqual((ushort)0x1235, connection.SendIsn); // local_port + 1
            Assert.IsFalse(connection.Established);
        }

        [TestMethod]
        public void FromSyn1_DeviceDeclaresHigherVersion_NegotiatesDownToV3()
        {
            // Act
            var connection = ReliableStreamConnection.FromSyn1(9020, 0x1234, Syn1(version: 3), Now);

            // Assert
            Assert.AreEqual(ReliableStreamFrame.NegotiatedVersion, connection.Version);
        }

        [TestMethod]
        public void FromSyn1_IncompleteSyn1_Throws()
        {
            // Arrange
            var notASyn1 = new StreamRequest(9020, StreamMessageType.Trans, 1);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => ReliableStreamConnection.FromSyn1(9020, 1, notASyn1, Now));
        }

        #endregion

        #region SYN2 / establishment

        [TestMethod]
        public void BeginSyn2Handshake_MatchesGuideExample()
        {
            // Arrange
            var connection = ReliableStreamConnection.FromSyn1(9020, 0x1234, Syn1(), Now);

            // Act
            var wire = connection.BeginSyn2Handshake(Now);

            // Assert
            CollectionAssert.AreEqual(
                ReliableStreamFrame.PackSyn(0x1234, StreamMessageType.Syn2, 0x1234, 0x1234, ReliableStreamFrame.NegotiatedVersion),
                wire);
        }

        [TestMethod]
        public void AcceptAck_AcknowledgedIsnEqualsLocalPort_Establishes()
        {
            // Arrange
            var connection = ReliableStreamConnection.FromSyn1(9020, 0x1234, Syn1(), Now);
            connection.BeginSyn2Handshake(Now);

            // Act
            var accepted = connection.AcceptAck(new StreamAck(0x1234, 0x1234), Now);

            // Assert
            Assert.IsTrue(accepted);
            Assert.IsTrue(connection.Established);
        }

        [TestMethod]
        public void AcceptAck_WrongDestinationPort_ReturnsFalseAndDoesNotEstablish()
        {
            // Arrange
            var connection = ReliableStreamConnection.FromSyn1(9020, 0x1234, Syn1(), Now);
            connection.BeginSyn2Handshake(Now);

            // Act
            var accepted = connection.AcceptAck(new StreamAck(0x9999, 0x1234), Now);

            // Assert
            Assert.IsFalse(accepted);
            Assert.IsFalse(connection.Established);
        }

        [TestMethod]
        public void AcceptAck_BeforeEstablished_WrongIsn_DoesNotEstablish()
        {
            // Arrange
            var connection = ReliableStreamConnection.FromSyn1(9020, 0x1234, Syn1(), Now);
            connection.BeginSyn2Handshake(Now);

            // Act
            var accepted = connection.AcceptAck(new StreamAck(0x1234, 0x0001), Now);

            // Assert
            Assert.IsFalse(accepted);
            Assert.IsFalse(connection.Established);
        }

        #endregion

        #region AcceptRequest / cumulative ACK

        private static ReliableStreamConnection Established()
        {
            var connection = ReliableStreamConnection.FromSyn1(9020, 0x1234, Syn1(), Now);
            connection.BeginSyn2Handshake(Now);
            connection.AcceptAck(new StreamAck(0x1234, 0x1234), Now);
            return connection;
        }

        [TestMethod]
        public void AcceptRequest_NextInSequence_AdvancesAndBuffersData()
        {
            // Arrange
            var connection = Established();
            var request = new StreamRequest(0x1234, StreamMessageType.Trans, 0x1238, applicationData: [1, 2, 3]);

            // Act
            var acked = connection.AcceptRequest(request, Now);

            // Assert
            Assert.AreEqual((ushort)0x1238, acked);
            Assert.AreEqual((ushort)0x1238, connection.PeerLastIsn);
            CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, connection.ReadApplicationBytes());
        }

        [TestMethod]
        public void AcceptRequest_OutOfOrder_DoesNotAdvance_ReturnsCurrentCumulativeAck()
        {
            // Arrange
            var connection = Established(); // PeerLastIsn = 0x1237
            var skippedAhead = new StreamRequest(0x1234, StreamMessageType.Trans, 0x123A, applicationData: [9]);

            // Act
            var acked = connection.AcceptRequest(skippedAhead, Now);

            // Assert
            Assert.AreEqual((ushort)0x1237, acked); // unchanged - still the last one actually accepted
            Assert.AreEqual((ushort)0x1237, connection.PeerLastIsn);
        }

        [TestMethod]
        public void AcceptRequest_DuplicateTrans_NotDeliveredTwice()
        {
            // Arrange
            var connection = Established();
            var request = new StreamRequest(0x1234, StreamMessageType.Trans, 0x1238, applicationData: [1]);
            connection.AcceptRequest(request, Now);
            connection.ReadApplicationBytes(); // drain

            // Act
            connection.AcceptRequest(request, Now); // same ISN again
            var buffered = connection.ReadApplicationBytes();

            // Assert
            Assert.IsEmpty(buffered);
        }

        [TestMethod]
        public void AcceptRequest_Fin_SetsPeerClosed()
        {
            // Arrange
            var connection = Established();
            var fin = new StreamRequest(0x1234, StreamMessageType.Fin, 0x1238);

            // Act
            connection.AcceptRequest(fin, Now);

            // Assert
            Assert.IsTrue(connection.PeerClosed);
        }

        #endregion

        #region One outstanding TRANS at a time

        [TestMethod]
        public void TryBeginTransmission_WhilePending_ReturnsFalse()
        {
            // Arrange
            var connection = Established();
            connection.TryBeginTransmission([1], Now, out _);

            // Act
            var beganSecond = connection.TryBeginTransmission([2], Now, out _);

            // Assert
            Assert.IsFalse(beganSecond);
        }

        [TestMethod]
        public void TryBeginTransmission_NotEstablished_ReturnsFalse()
        {
            // Arrange
            var connection = ReliableStreamConnection.FromSyn1(9020, 0x1234, Syn1(), Now);

            // Act
            var began = connection.TryBeginTransmission([1], Now, out _);

            // Assert
            Assert.IsFalse(began);
        }

        #endregion

        #region Retransmit / heartbeat / timeout timing

        [TestMethod]
        public void TryGetRetransmit_Before600ms_ReturnsFalse()
        {
            // Arrange
            var connection = Established();
            connection.TryBeginTransmission([1], Now, out _);

            // Act
            var due = connection.TryGetRetransmit(Now.AddMilliseconds(599), out _);

            // Assert
            Assert.IsFalse(due);
        }

        [TestMethod]
        public void TryGetRetransmit_After600ms_ReturnsIdenticalWire()
        {
            // Arrange
            var connection = Established();
            connection.TryBeginTransmission([1, 2, 3], Now, out byte[] originalWire);

            // Act
            var due = connection.TryGetRetransmit(Now.AddMilliseconds(601), out byte[] retryWire);

            // Assert
            Assert.IsTrue(due);
            CollectionAssert.AreEqual(originalWire, retryWire);
        }

        [TestMethod]
        public void IsRetransmitExhausted_AfterMaxRetries_IsTrue()
        {
            // Arrange
            var connection = Established();
            connection.TryBeginTransmission([1], Now, out _);
            var now = Now;
            for (int i = 0; i < ReliableStreamConnection.MaxRetries; i++)
            {
                now = now.AddSeconds(1);
                connection.TryGetRetransmit(now, out _);
            }

            // Act & Assert
            Assert.IsTrue(connection.IsRetransmitExhausted);
            Assert.IsFalse(connection.TryGetRetransmit(now.AddSeconds(1), out _));
        }

        [TestMethod]
        public void NeedsHeartbeat_After3SecondsIdle_IsTrue()
        {
            // Arrange
            var connection = Established();

            // Act & Assert
            Assert.IsTrue(connection.NeedsHeartbeat(Now.AddSeconds(3)));
        }

        [TestMethod]
        public void NeedsHeartbeat_BeforeIdleThreshold_IsFalse()
        {
            // Arrange
            var connection = Established();

            // Act & Assert
            Assert.IsFalse(connection.NeedsHeartbeat(Now.AddSeconds(2)));
        }

        [TestMethod]
        public void NeedsHeartbeat_WithPendingTrans_IsFalse()
        {
            // Arrange
            var connection = Established();
            connection.TryBeginTransmission([1], Now, out _);

            // Act & Assert
            Assert.IsFalse(connection.NeedsHeartbeat(Now.AddSeconds(5)));
        }

        [TestMethod]
        public void IsTimedOut_After10Seconds_IsTrue()
        {
            // Arrange
            var connection = Established();

            // Act & Assert
            Assert.IsTrue(connection.IsTimedOut(Now.AddSeconds(10)));
        }

        [TestMethod]
        public void IsTimedOut_Before10Seconds_IsFalse()
        {
            // Arrange
            var connection = Established();

            // Act & Assert
            Assert.IsFalse(connection.IsTimedOut(Now.AddSeconds(9)));
        }

        #endregion

        #region Close sequencing

        [TestMethod]
        public void IsFullyClosed_LocalFinAckedThenPeerFin_IsTrue()
        {
            // Arrange
            var connection = Established();
            connection.TryBeginFin(Now, out byte[] finWire);
            // The FIN's ISN is whatever SendIsn was before TryBeginFin advanced it.
            ushort finIsn = (ushort)(connection.SendIsn - 1);
            connection.AcceptAck(new StreamAck(0x1234, finIsn), Now);

            // Act
            connection.AcceptRequest(new StreamRequest(0x1234, StreamMessageType.Fin, 0x1238), Now);

            // Assert
            Assert.IsTrue(connection.LocalFinAcked);
            Assert.IsTrue(connection.IsFullyClosed);
        }

        [TestMethod]
        public void IsFullyClosed_PeerFinThenLocalFinAcked_IsTrue()
        {
            // Arrange
            var connection = Established();
            connection.AcceptRequest(new StreamRequest(0x1234, StreamMessageType.Fin, 0x1238), Now);
            connection.TryBeginFin(Now, out _);
            ushort finIsn = (ushort)(connection.SendIsn - 1);

            // Act
            connection.AcceptAck(new StreamAck(0x1234, finIsn), Now);

            // Assert
            Assert.IsTrue(connection.IsFullyClosed);
        }

        #endregion
    }
}
