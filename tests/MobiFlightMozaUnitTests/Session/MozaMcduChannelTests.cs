using MobiFlightMoza.Cdu;
using MobiFlightMoza.Protocol;
using MobiFlightMoza.Tests.Mocks;
namespace MobiFlightMoza.Session.Tests
{
    [TestClass]
    public class MozaMcduChannelTests
    {
        private static readonly DateTime Now = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static (MozaMcduChannel channel, ReliableStreamMultiplexer multiplexer, RecordingFrameSink sink) Create()
        {
            var sink = new RecordingFrameSink();
            var multiplexer = new ReliableStreamMultiplexer(sink, [MozaConstants.ServicePortMcduTcp]);
            MultiplexerTestHelper.Establish(multiplexer, MozaConstants.ServicePortMcduTcp, 0x3001, Now);
            sink.SentWires.Clear(); // drop the SYN2 handshake frame - tests only care about what the channel itself sends
            var channel = new MozaMcduChannel(multiplexer);
            return (channel, multiplexer, sink);
        }
        private static byte[] ClientCapabilityPackage()
            => NetworkPackage.Pack(0x33, [2, 0, 0, 0, 0]); // version=2, pageIndex=0
        [TestMethod]
        public void Start_QueuesInitConfig()
        {
            // Arrange
            var (channel, multiplexer, sink) = Create();
            // Act
            channel.Start();
            multiplexer.Tick(Now);
            // Assert
            Assert.HasCount(1, sink.SentWires);
            ReliableStreamFrame.TryParseRequest(sink.SentWires[0], out var request, out _);
            Assert.AreEqual((byte)0x32, request.ApplicationData[0]); // InitConfig PackageId
        }
        [TestMethod]
        public void OnApplicationData_ClientCapability_RaisesCapabilityReceived()
        {
            // Arrange
            var (channel, _, _) = Create();
            McduClientCapability? received = null;
            channel.CapabilityReceived += cap => received = cap;
            // Act
            channel.OnApplicationData(ClientCapabilityPackage());
            // Assert
            Assert.IsNotNull(received);
            Assert.AreEqual((byte)2, received.Value.Version);
        }
        [TestMethod]
        public void SubmitPage_BeforeCapability_IsHeldUntilCapabilityArrives()
        {
            // Arrange
            var (channel, multiplexer, sink) = Create();
            channel.SubmitPage(CduPage.CreateBlank());
            multiplexer.Tick(Now);
            int sentBeforeCapability = sink.SentWires.Count;
            // Act
            channel.OnApplicationData(ClientCapabilityPackage());
            multiplexer.Tick(Now);
            // Assert
            Assert.AreEqual(0, sentBeforeCapability);
            Assert.HasCount(1, sink.SentWires);
            ReliableStreamFrame.TryParseRequest(sink.SentWires[0], out var request, out _);
            Assert.AreEqual((byte)0x30, request.ApplicationData[0]); // Keyframe PackageId
        }
        [TestMethod]
        public void SubmitPage_AfterCapability_SendsImmediately()
        {
            // Arrange
            var (channel, multiplexer, sink) = Create();
            channel.OnApplicationData(ClientCapabilityPackage());
            multiplexer.Tick(Now); // nothing queued yet, but harmless
            // Act
            channel.SubmitPage(CduPage.CreateBlank());
            multiplexer.Tick(Now);
            // Assert
            Assert.HasCount(1, sink.SentWires);
        }
        [TestMethod]
        public void OnApplicationData_NonCapabilityPackage_IsIgnored()
        {
            // Arrange
            var (channel, _, _) = Create();
            McduClientCapability? received = null;
            channel.CapabilityReceived += cap => received = cap;
            // Act
            channel.OnApplicationData(NetworkPackage.Pack(0x0C, [1, 2, 3]));
            // Assert
            Assert.IsNull(received);
        }
    }
}