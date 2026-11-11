using MobiFlightMoza.Protocol;
using MobiFlightMoza.Tests.Mocks;
using System.Collections.Generic;
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
        #region Acknowledgement
        [TestMethod]
        public void OnBytesReceived_DeviceTrans_SendsAckWithTunnelReplyBitSet()
        {
            // Arrange - a sink that forgets to flag its ACK send produces a wire the device
            // can't tell apart from a fresh request (both share inner command 0x7C), so it
            // never recognizes the ACK and keeps retransmitting forever.
            var (session, sink) = StartAndInit();
            EstablishSettingsConnection(session);
            sink.SentWires.Clear();
            // Act
            Feed(session, ScriptedMozaDevice.Trans(SettingsLocalPort, 2, MinimalPreamble));
            // Assert
            bool foundReplyAck = false;
            var decoder = new SerialLinkDecoder();
            foreach (byte[] wire in sink.SentWires)
            {
                var messages = decoder.Feed(wire, wire.Length);
                if (messages.Count != 1) continue;
                if (!MozaTunnel.TryUnwrap(messages[0], out var tunnel)) continue;
                if (!tunnel.IsReply) continue;
                if (!ReliableStreamFrame.TryParseAck(tunnel.InnerPayload, out _)) continue;
                foundReplyAck = true;
            }
            Assert.IsTrue(foundReplyAck);
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
        #region MCDU mode entry
        [TestMethod]
        public void SettingsReadyAndCapabilityReceived_WritesDisplayMode0ThenDisplayMode1()
        {
            // Arrange
            const ushort McduLocalPort = 0x3001;
            var (session, sink) = StartAndInit();
            EstablishSettingsConnection(session);
            Feed(session, ScriptedMozaDevice.Trans(SettingsLocalPort, 2, MinimalPreamble));
            DrainSettingsQueue(session, sink, Now);
            DrainSettingsQueue(session, sink, Now.AddSeconds(4.0)); // closes the collection window - SettingsReady, no capability yet
            sink.SentWires.Clear();
            Feed(session, ScriptedMozaDevice.Syn1(MozaConstants.ServicePortMcduTcp, 1, McduLocalPort, 3), Now.AddSeconds(4.0));
            Feed(session, ScriptedMozaDevice.Ack(McduLocalPort, McduLocalPort), Now.AddSeconds(4.0));
            sink.SentWires.Clear(); // drop the SYN2 handshake and InitConfig - only care about what follows Capability
            // Act
            byte[] capability = NetworkPackage.Pack(0x33, [2, 0, 0, 0, 0]); // version=2, pageIndex=0
            Feed(session, ScriptedMozaDevice.Trans(McduLocalPort, 2, capability), Now.AddSeconds(4.0));
            // McduChannel.Start() (queuing InitConfig) fires on this same call, so the MCDU
            // connection also has a pending send now - drain generically (ACKing whichever
            // port each TRANS actually targets) rather than assuming settings-only traffic.
            for (int i = 0; i < 10; i++)
            {
                int before = sink.SentWires.Count;
                session.Tick(Now.AddSeconds(4.0));
                if (sink.SentWires.Count == before) break;
                for (int j = before; j < sink.SentWires.Count; j++)
                {
                    if (!TryDecodeStreamRequest(sink.SentWires[j], out var pending)) continue;
                    if (pending.MessageType != StreamMessageType.Trans) continue;
                    Feed(session, ScriptedMozaDevice.Ack(pending.DestinationPort, pending.Isn), Now.AddSeconds(4.0));
                }
            }
            // Assert - two settings TRANS go out on the settings connection: displayMode=0, then displayMode=1.
            var settingsWrites = new List<StreamRequest>();
            foreach (byte[] wire in sink.SentWires)
            {
                if (!TryDecodeStreamRequest(wire, out var request)) continue;
                if (request.DestinationPort != SettingsLocalPort || request.MessageType != StreamMessageType.Trans) continue;
                settingsWrites.Add(request);
            }
            Assert.HasCount(2, settingsWrites);
            static byte DisplayModeValue(StreamRequest request)
            {
                byte[] body = request.ApplicationData;
                int settingId = body[9] | (body[10] << 8) | (body[11] << 16) | (body[12] << 24);
                Assert.AreEqual(0x18, settingId);
                return body[13];
            }
            Assert.AreEqual((byte)0, DisplayModeValue(settingsWrites[0]));
            Assert.AreEqual((byte)1, DisplayModeValue(settingsWrites[1]));
        }
        #endregion
        #region Panel commands and screen ready
        private const ushort McduLocalPortForReady = 0x3001;
        // Runs the bring-up until the displayMode 0/1 writes have gone out (settings ready and
        // capability received) and returns the time used, so a test can feed the device's echo.
        private static DateTime BringUpUntilDisplayModeWritten(MozaScreenSession session, RecordingFrameSink sink)
        {
            DateTime now = Now.AddSeconds(4.0);
            EstablishSettingsConnection(session);
            Feed(session, ScriptedMozaDevice.Trans(SettingsLocalPort, 2, MinimalPreamble));
            DrainSettingsQueue(session, sink, Now);
            DrainSettingsQueue(session, sink, now);
            Feed(session, ScriptedMozaDevice.Syn1(MozaConstants.ServicePortMcduTcp, 1, McduLocalPortForReady, 3), now);
            Feed(session, ScriptedMozaDevice.Ack(McduLocalPortForReady, McduLocalPortForReady), now);
            Feed(session, ScriptedMozaDevice.Trans(McduLocalPortForReady, 2, NetworkPackage.Pack(0x33, [2, 0, 0, 0, 0])), now);
            return now;
        }
        private static int CountPanelFrames(RecordingFrameSink sink, byte[] payload)
        {
            byte[] expected = MozaPanelCommand.Encode(payload);
            int count = 0;
            foreach (byte[] wire in sink.SentWires)
            {
                if (wire.AsSpan().SequenceEqual(expected)) count++;
            }
            return count;
        }
        [TestMethod]
        public void SendPanelCommand_BeforeDeviceInit_IsSentOnceRunning()
        {
            // Arrange
            var sink = new RecordingFrameSink();
            var session = new MozaScreenSession(sink);
            session.Start();
            byte[] ledOn = MozaPanelCommand.Led((byte)MozaBoeingCduLed.Exec, true);
            // Act
            session.SendPanelCommand(ledOn);
            int sentBeforeInit = CountPanelFrames(sink, ledOn);
            Feed(session, MozaConstants.RootHandshakeResponse);
            Feed(session, MozaConstants.DeviceInitResponse);
            // Assert
            Assert.AreEqual(0, sentBeforeInit);
            Assert.AreEqual(1, CountPanelFrames(sink, ledOn));
        }
        [TestMethod]
        public void SendPanelCommand_SameTargetBeforeDeviceInit_OnlyLatestValueIsSent()
        {
            // Arrange
            var sink = new RecordingFrameSink();
            var session = new MozaScreenSession(sink);
            session.Start();
            byte[] first = MozaPanelCommand.Brightness(MozaLightGroup.KeyBacklight, (int)MozaBrightness.Off);
            byte[] latest = MozaPanelCommand.Brightness(MozaLightGroup.KeyBacklight, (int)MozaBrightness.Full);
            // Act
            session.SendPanelCommand(first);
            session.SendPanelCommand(latest);
            Feed(session, MozaConstants.RootHandshakeResponse);
            Feed(session, MozaConstants.DeviceInitResponse);
            // Assert
            Assert.AreEqual(0, CountPanelFrames(sink, first));
            Assert.AreEqual(1, CountPanelFrames(sink, latest));
        }
        [TestMethod]
        public void SendPanelCommand_WhileRunning_IsSentImmediately()
        {
            // Arrange
            var (session, sink) = StartAndInit();
            byte[] ledOn = MozaPanelCommand.Led((byte)MozaMcduLed.Fail, true);
            // Act
            session.SendPanelCommand(ledOn);
            // Assert
            Assert.AreEqual(1, CountPanelFrames(sink, ledOn));
        }
        [TestMethod]
        public void DisplayModeEcho_RaisesScreenReadyExactlyOnce()
        {
            // Arrange
            var (session, sink) = StartAndInit();
            int readyCount = 0;
            session.ScreenReady += () => readyCount++;
            DateTime now = BringUpUntilDisplayModeWritten(session, sink);
            bool readyBeforeEcho = session.IsScreenReady;
            // Act - the device echoes the forced transition 0 then 1.
            Feed(session, ScriptedMozaDevice.Trans(SettingsLocalPort, 3, MozaSettingsFrame.PackSettingFrame(0x18, [0])), now);
            Feed(session, ScriptedMozaDevice.Trans(SettingsLocalPort, 4, MozaSettingsFrame.PackSettingFrame(0x18, [1])), now);
            session.Tick(now.AddSeconds(5.0));
            // Assert
            Assert.IsFalse(readyBeforeEcho);
            Assert.IsTrue(session.IsScreenReady);
            Assert.AreEqual(1, readyCount);
        }
        [TestMethod]
        public void NoDisplayModeEcho_RaisesScreenReadyAfterTimeout()
        {
            // Arrange
            var (session, sink) = StartAndInit();
            int readyCount = 0;
            session.ScreenReady += () => readyCount++;
            DateTime now = BringUpUntilDisplayModeWritten(session, sink);
            // Act
            session.Tick(now.AddSeconds(1.0));
            int readyAfterOneSecond = readyCount;
            session.Tick(now.AddSeconds(2.5));
            // Assert
            Assert.AreEqual(0, readyAfterOneSecond);
            Assert.AreEqual(1, readyCount);
        }
        #endregion
        #region Shutdown
        [TestMethod]
        public void BeginShutdown_SendsFinImmediatelyWithoutAnySettingsWrite()
        {
            // Arrange - displayMode reset no longer happens at shutdown (moved to the
            // startup 0-then-1 write in TryEnterMcduMode), so BeginShutdown's only job now
            // is to start the guide's documented FIN close, with nothing to wait on first.
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