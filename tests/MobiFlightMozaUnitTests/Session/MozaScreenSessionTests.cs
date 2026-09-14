using MobiFlightMoza.Protocol;
using MobiFlightMoza.Tests.Mocks;
using System.Diagnostics.CodeAnalysis;
namespace MobiFlightMoza.Session.Tests
{
    [TestClass]
    public class MozaScreenSessionTests
    {
        private static readonly DateTime Now = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        // McuIdLength=0, SettingCount=0 - the shortest valid preamble.
        private static readonly byte[] MinimalPreamble = [0x07, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00];
        private const ushort SettingsLocalPort = 0x2001;
        private static void Feed(MozaScreenSession session, byte[] wire, DateTime? now = null)
            => session.OnBytesReceived(wire, wire.Length, now ?? Now);
        // Everything the session sends through the multiplexer is a full wire frame (SerialLink
        // framing + the 0x43 tunnel), unlike the two fixed handshake frames it sends directly -
        // this undoes both layers to recover the Reliable Stream request/ack underneath, the
        // same way a real device's receiver would.
        private static bool TryDecodeStreamRequest(byte[] wireFrame, [NotNullWhen(true)] out StreamRequest? request)
        {
            request = null;
            var decoder = new SerialLinkDecoder();
            var messages = decoder.Feed(wireFrame, wireFrame.Length);
            if (messages.Count != 1) return false;
            if (!MozaTunnel.TryUnwrap(messages[0], out var tunnel)) return false;
            if (tunnel.IsReply) return false;
            return ReliableStreamFrame.TryParseRequest(tunnel.InnerPayload, out request, out _);
        }
        private static (MozaScreenSession session, RecordingFrameSink sink) StartAndInit()
        {
            var sink = new RecordingFrameSink();
            var session = new MozaScreenSession(sink);
            session.Start();
            Feed(session, MozaConstants.RootHandshakeResponse);
            Feed(session, MozaConstants.DeviceInitResponse);
            return (session, sink);
        }
        private static void EstablishSettingsConnection(MozaScreenSession session)
        {
            Feed(session, ScriptedMozaDevice.Syn1(MozaConstants.ServicePortSettings, 1, SettingsLocalPort, 3));
            Feed(session, ScriptedMozaDevice.Ack(SettingsLocalPort, SettingsLocalPort));
        }
        // Repeatedly ticks and ACKs whatever TRANS was just sent on the settings
        // connection, until the send queue is empty - only one TRANS may be outstanding
        // per connection at a time, so nothing later can go out until each one is ACKed.
        private static void DrainSettingsQueue(MozaScreenSession session, RecordingFrameSink sink, DateTime now)
        {
            for (int i = 0; i < 10; i++)
            {
                int before = sink.SentWires.Count;
                session.Tick(now);
                if (sink.SentWires.Count == before) return;
                if (!TryDecodeStreamRequest(sink.SentWires[^1], out var request)) return;
                if (request.MessageType != StreamMessageType.Trans) return;
                Feed(session, ScriptedMozaDevice.Ack(SettingsLocalPort, request.Isn), now);
            }
        }
        #region Handshake
        [TestMethod]
        public void Start_SendsRootHandshakeRequest()
        {
            // Arrange
            var sink = new RecordingFrameSink();
            var session = new MozaScreenSession(sink);
            // Act
            session.Start();
            // Assert
            Assert.HasCount(1, sink.SentWires);
            CollectionAssert.AreEqual(MozaConstants.RootHandshakeRequest, sink.SentWires[0]);
        }
        [TestMethod]
        public void OnBytesReceived_WrongRootHandshakeReply_Faults()
        {
            // Arrange
            var sink = new RecordingFrameSink();
            var session = new MozaScreenSession(sink);
            session.Start();
            // Act
            byte[] wrong = SerialLinkFrame.EncodeRaw(0x99, MozaConstants.DevicePairFromDevice, []);
            Feed(session, wrong);
            // Assert
            Assert.AreEqual(MozaSessionState.Faulted, session.State);
        }
        [TestMethod]
        public void OnBytesReceived_CorrectHandshakeReply_SendsDeviceInit()
        {
            // Arrange
            var sink = new RecordingFrameSink();
            var session = new MozaScreenSession(sink);
            session.Start();
            // Act
            Feed(session, MozaConstants.RootHandshakeResponse);
            // Assert
            Assert.HasCount(2, sink.SentWires); // root handshake request + device init request
            CollectionAssert.AreEqual(MozaConstants.DeviceInitRequest, sink.SentWires[1]);
        }
        [TestMethod]
        public void OnBytesReceived_DeviceInitResponse_ReachesRunning()
        {
            // Arrange
            var (session, _) = StartAndInit();
            // Assert
            Assert.AreEqual(MozaSessionState.Running, session.State);
        }
        [TestMethod]
        public void OnBytesReceived_DeviceSyn1InsteadOfInitReply_AlsoReachesRunning()
        {
            // Arrange - the guide allows the device to skip a separate init reply and go
            // straight to opening a service connection.
            var sink = new RecordingFrameSink();
            var session = new MozaScreenSession(sink);
            session.Start();
            Feed(session, MozaConstants.RootHandshakeResponse);
            // Act
            Feed(session, ScriptedMozaDevice.Syn1(MozaConstants.ServicePortSettings, 1, SettingsLocalPort, 3));
            // Assert
            Assert.AreEqual(MozaSessionState.Running, session.State);
        }
        #endregion
        #region Settings bring-up
        [TestMethod]
        public void FullScriptedBringUp_ResolvesCabinPositionExactlyOnce()
        {
            // Arrange
            var (session, _) = StartAndInit();
            int resolvedCount = 0;
            byte? resolvedValue = null;
            session.CabinPositionResolved += value => { resolvedCount++; resolvedValue = value; };
            EstablishSettingsConnection(session);
            Feed(session, ScriptedMozaDevice.Trans(SettingsLocalPort, 2, MinimalPreamble));
            // Act
            Feed(session, ScriptedMozaDevice.Trans(SettingsLocalPort, 3, MozaSettingsFrame.PackSettingFrame(0x13, [1])));
            // A second, unrelated setting must not fire CabinPositionResolved again.
            Feed(session, ScriptedMozaDevice.Trans(SettingsLocalPort, 4, MozaSettingsFrame.PackSettingFrame(0x18, [0])));
            // Assert
            Assert.AreEqual(1, resolvedCount);
            Assert.AreEqual((byte)1, resolvedValue);
        }
        #endregion
        #region Shutdown
        [TestMethod]
        public void BeginShutdown_WithCachedDisplayMode_QueuesRestoreWrite()
        {
            // Arrange
            var (session, sink) = StartAndInit();
            EstablishSettingsConnection(session);
            Feed(session, ScriptedMozaDevice.Trans(SettingsLocalPort, 2, MinimalPreamble));
            DrainSettingsQueue(session, sink, Now); // flush + ACK the time-sync and version sends
            Feed(session, ScriptedMozaDevice.Trans(SettingsLocalPort, 3, MozaSettingsFrame.PackSettingFrame(0x18, [5])));
            // Closes the collection window and caches DisplayMode=5. The 4-second jump is
            // also past the 3-second heartbeat threshold, so this legitimately sends one -
            // draining (not a plain Tick) so that heartbeat gets ACKed too, instead of
            // blocking the restore write below the same way an un-ACKed send always would.
            DrainSettingsQueue(session, sink, Now.AddSeconds(4.0));
            sink.SentWires.Clear();
            // Act
            session.BeginShutdown(Now.AddSeconds(4.0));
            session.Tick(Now.AddSeconds(4.0)); // sends the queued restore write (nothing else is pending)
            // Assert
            Assert.HasCount(1, sink.SentWires);
            Assert.IsTrue(TryDecodeStreamRequest(sink.SentWires[0], out var request));
            byte[] body = request.ApplicationData;
            int settingId = body[9] | (body[10] << 8) | (body[11] << 16) | (body[12] << 24);
            Assert.AreEqual(0x18, settingId);
            Assert.AreEqual((byte)5, body[13]);
        }
        [TestMethod]
        public void BeginShutdown_NoCachedDisplayMode_DoesNotWriteIt()
        {
            // Arrange - settings channel never reached Ready, so nothing was cached.
            var (session, sink) = StartAndInit();
            EstablishSettingsConnection(session);
            sink.SentWires.Clear();
            // Act
            session.BeginShutdown(Now);
            session.Tick(Now);
            // Assert - a FIN goes out to close the connection, but never a settings write.
            bool foundFin = false;
            foreach (byte[] wire in sink.SentWires)
            {
                if (!TryDecodeStreamRequest(wire, out var request)) continue;
                Assert.AreNotEqual(StreamMessageType.Trans, request.MessageType);
                if (request.MessageType == StreamMessageType.Fin) foundFin = true;
            }
            Assert.IsTrue(foundFin);
        }
        [TestMethod]
        public void IsShutdownComplete_BeforeShutdownRequested_IsFalse()
        {
            // Arrange
            var (session, _) = StartAndInit();
            // Act & Assert - not complete merely because there's nothing to close; it has
            // to actually be asked for first.
            Assert.IsFalse(session.IsShutdownComplete);
        }
        [TestMethod]
        public void IsShutdownComplete_NoConnectionsEverOpened_BecomesTrueAfterOneTick()
        {
            // Arrange
            var (session, _) = StartAndInit();
            // Act
            session.BeginShutdown(Now);
            session.Tick(Now);
            // Assert
            Assert.IsTrue(session.IsShutdownComplete);
        }
        #endregion
    }
}