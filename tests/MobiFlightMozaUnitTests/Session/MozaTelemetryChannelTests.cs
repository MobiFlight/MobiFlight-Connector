using MobiFlightMoza.Protocol;
using MobiFlightMoza.Tests.Mocks;
namespace MobiFlightMoza.Session.Tests
{
    [TestClass]
    public class MozaTelemetryChannelTests
    {
        private static readonly DateTime Now = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static (MozaTelemetryChannel channel, ReliableStreamMultiplexer multiplexer, RecordingFrameSink sink) Create()
        {
            var sink = new RecordingFrameSink();
            var multiplexer = new ReliableStreamMultiplexer(sink, [MozaConstants.ServicePortTelemetry]);
            MultiplexerTestHelper.Establish(multiplexer, MozaConstants.ServicePortTelemetry, 0x3001, Now);
            sink.SentWires.Clear(); // drop the SYN2 handshake frame - tests only care about what the channel itself sends
            var channel = new MozaTelemetryChannel(multiplexer);
            return (channel, multiplexer, sink);
        }
        // The device's own preamble on this channel: one raw 0xFF byte, then the
        // NetworkPackage stream - not itself a NetworkPackage.
        private static byte[] WithLeadingByte(byte[] data) => [0xFF, .. data];
        [TestMethod]
        public void OnApplicationData_TokenPackage_EchoesTokenUnchanged()
        {
            // Arrange
            var (channel, multiplexer, sink) = Create();
            byte[] token = [0x61, 0x00, 0x00, 0x00];
            // Act
            channel.OnApplicationData(WithLeadingByte(NetworkPackage.Pack(0x06, token)));
            multiplexer.Tick(Now);
            // Assert
            Assert.HasCount(1, sink.SentWires);
            Assert.IsTrue(ReliableStreamFrame.TryParseRequest(sink.SentWires[0], out var request, out _));
            CollectionAssert.AreEqual(NetworkPackage.Pack(0x06, token), request.ApplicationData);
        }
        [TestMethod]
        public void OnApplicationData_LeadingByteArrivesSeparately_StillParsesCorrectly()
        {
            // Arrange - the Reliable Stream layer may hand application bytes over in
            // arbitrary chunks; the one-byte preamble isn't guaranteed to arrive together
            // with the first real NetworkPackage.
            var (channel, multiplexer, sink) = Create();
            byte[] token = [0x2a, 0x00, 0x00, 0x00];
            byte[] tokenPackage = NetworkPackage.Pack(0x06, token);
            // Act
            channel.OnApplicationData([0xFF]);
            channel.OnApplicationData(tokenPackage);
            multiplexer.Tick(Now);
            // Assert
            Assert.HasCount(1, sink.SentWires);
            Assert.IsTrue(ReliableStreamFrame.TryParseRequest(sink.SentWires[0], out var request, out _));
            CollectionAssert.AreEqual(tokenPackage, request.ApplicationData);
        }
        [TestMethod]
        public void OnApplicationData_NonTokenPackage_SendsNothing()
        {
            // Arrange
            var (channel, multiplexer, sink) = Create();
            // Act - NetworkPackage(0x03, clientVersion) - not a token, no response expected
            channel.OnApplicationData(WithLeadingByte(NetworkPackage.Pack(0x03, [1, 0, 0, 0])));
            multiplexer.Tick(Now);
            // Assert
            Assert.IsEmpty(sink.SentWires);
        }
    }
}
