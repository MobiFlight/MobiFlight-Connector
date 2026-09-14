using MobiFlightMoza.Protocol;
using MobiFlightMoza.Tests.Mocks;
namespace MobiFlightMoza.Session.Tests
{
    [TestClass]
    public class MozaSettingsChannelTests
    {
        private static readonly DateTime Now = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        // McuIdLength=0, SettingCount=0 - the shortest valid preamble.
        private static readonly byte[] MinimalPreamble = [0x07, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00];
        private static (MozaSettingsChannel channel, ReliableStreamMultiplexer multiplexer, RecordingFrameSink sink) Create()
        {
            var sink = new RecordingFrameSink();
            var multiplexer = new ReliableStreamMultiplexer(sink, [MozaConstants.ServicePortSettings]);
            MultiplexerTestHelper.Establish(multiplexer, MozaConstants.ServicePortSettings, 0x2001, Now);
            sink.SentWires.Clear(); // drop the SYN2 handshake frame - tests only care about what the channel itself sends
            var channel = new MozaSettingsChannel(multiplexer);
            return (channel, multiplexer, sink);
        }
        [TestMethod]
        public void OnApplicationData_Preamble_QueuesTimeSyncAndVersion()
        {
            // Arrange
            var (channel, multiplexer, sink) = Create();
            // Act
            channel.OnApplicationData(MinimalPreamble, Now);
            multiplexer.Tick(Now); // dequeues and sends the first queued frame
            // Assert
            Assert.HasCount(1, sink.SentWires);
            ReliableStreamFrame.TryParseRequest(sink.SentWires[0], out var request, out _);
            Assert.AreEqual(StreamMessageType.Trans, request.MessageType);
            // The settings-frame body starts after FF(1)+Size(4)+CRC32(4); its first 4
            // bytes are the setting id. Time sync (0x02) is queued before version (0x07).
            byte[] body = request.ApplicationData;
            int settingId = body[9] | (body[10] << 8) | (body[11] << 16) | (body[12] << 24);
            Assert.AreEqual(0x02, settingId);
        }
        [TestMethod]
        public void OnApplicationData_SettingFrame_CachesValue()
        {
            // Arrange
            var (channel, _, _) = Create();
            channel.OnApplicationData(MinimalPreamble, Now);
            byte[] frame = MozaSettingsFrame.PackSettingFrame(0x13, [1]);
            // Act
            channel.OnApplicationData(frame, Now);
            // Assert
            CollectionAssert.AreEqual(new byte[] { 1 }, channel.Settings[0x13]);
        }
        [TestMethod]
        public void CabinPosition_ExposesFirstByteOfSetting0x13()
        {
            // Arrange
            var (channel, _, _) = Create();
            channel.OnApplicationData(MinimalPreamble, Now);
            // Act
            channel.OnApplicationData(MozaSettingsFrame.PackSettingFrame(0x13, [1]), Now);
            // Assert
            Assert.AreEqual((byte)1, channel.CabinPosition);
        }
        [TestMethod]
        public void DisplayMode_ExposesFirstByteOfSetting0x18()
        {
            // Arrange
            var (channel, _, _) = Create();
            channel.OnApplicationData(MinimalPreamble, Now);
            // Act
            channel.OnApplicationData(MozaSettingsFrame.PackSettingFrame(0x18, [5]), Now);
            // Assert
            Assert.AreEqual((byte)5, channel.DisplayMode);
        }
        [TestMethod]
        public void SettingEchoed_FiresForEachIncomingFrame()
        {
            // Arrange
            var (channel, _, _) = Create();
            channel.OnApplicationData(MinimalPreamble, Now);
            int fired = 0;
            channel.SettingEchoed += (_, _) => fired++;
            // Act
            channel.OnApplicationData(MozaSettingsFrame.PackSettingFrame(0x18, [1]), Now);
            // Assert
            Assert.AreEqual(1, fired);
        }
        [TestMethod]
        public void Tick_BeforeCollectionWindow_DoesNotFireReady()
        {
            // Arrange
            var (channel, _, _) = Create();
            bool ready = false;
            channel.Ready += () => ready = true;
            channel.OnApplicationData(MinimalPreamble, Now);
            // Act
            channel.Tick(Now.AddSeconds(3.9));
            // Assert
            Assert.IsFalse(ready);
        }
        [TestMethod]
        public void Tick_AfterCollectionWindow_FiresReadyExactlyOnce()
        {
            // Arrange
            var (channel, _, _) = Create();
            int readyCount = 0;
            channel.Ready += () => readyCount++;
            channel.OnApplicationData(MinimalPreamble, Now);
            // Act
            channel.Tick(Now.AddSeconds(4.0));
            channel.Tick(Now.AddSeconds(5.0));
            // Assert
            Assert.AreEqual(1, readyCount);
        }
        [TestMethod]
        public void RequestSetting_QueuesAPackedSettingFrame()
        {
            // Arrange
            var (channel, multiplexer, sink) = Create();
            channel.OnApplicationData(MinimalPreamble, Now);
            multiplexer.Tick(Now); // drain the time-sync send queued by the preamble
            // Act
            channel.RequestSetting(0x18, [1], Now);
            // Assert - queued, not yet sent (one outstanding TRANS at a time on this connection)
            Assert.HasCount(1, sink.SentWires);
        }
    }
}