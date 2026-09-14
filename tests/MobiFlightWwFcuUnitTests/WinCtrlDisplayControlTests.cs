using MobiFlightWwFcu;
using System.Net;
using WebSocketSharp.Server;

namespace MobiFlightWwFcuUnitTests
{
    [TestClass]
    public class WinCtrlDisplayControlTests
    {
        private WebSocketServer server = null!;

        private class FakeWinCtrlMessageSender : IWinCtrlMessageSender
        {
            private bool connected;

            public bool IsConnected() => connected;
            public void Connect() => connected = true;
            public void Shutdown() => connected = false;

            public void SendDisplayCommands(IList<byte[]> commands) { }
            public void SendCduDisplayBytes(byte[] byteList) { }
            public void SendLightControlMessage(
                byte[] destination, byte type, byte value)
            { }
            public void SetBrightness(
                byte[] destinationAddress, byte type, byte brightness)
            { }
            public void SetVibration(
                byte[] destinationAddress, byte type, byte level)
            { }
            public void SetPulseLight(
                byte[] destinationAddress, bool isOn)
            { }
            public void SendHeartBeatMessage() { }
            public void SendRequestFirmwareMessage() { }
        }

        [TestInitialize]
        public void Setup()
        {
            server = new WebSocketServer(
                IPAddress.Loopback,
                8320
            );

            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            Directory.CreateDirectory(
                Path.Combine(
                    baseDirectory,
                    "Scripts",
                    "Winwing",
                    "Fonts",
                    "Default",
                    "MCDU"
                )
            );

            Directory.CreateDirectory(
                Path.Combine(
                    baseDirectory,
                    "Scripts",
                    "Winwing",
                    "Fonts",
                    "Default",
                    "PFP"
                )
            );
        }

        [TestMethod]
        public void CduConstructor_DoesNotRegisterWebSocketService()
        {
            // Act
            _ = new WinCtrlDisplayControl(
                WinCtrlConstants.PRODUCT_ID_MCDU_CPT,
                server
            );

            // Assert
            Assert.AreEqual(
                0,
                server.WebSocketServices.Count
            );
        }

        [TestMethod]
        public void IgnoredMcdu_Creation_DoesNotReserveCaptainWebSocket()
        {
            _ = new WinCtrlDisplayControl(
                WinCtrlConstants.PRODUCT_ID_MCDU_CPT,
                server
            );

            Assert.IsFalse(
                server.WebSocketServices.TryGetServiceHost(
                    "/winwing/cdu-captain",
                    out _
                )
            );
        }

        [TestMethod]
        public void IgnoredMcdu_DoesNotBlockEnabledPfp3nCaptainWebSocket()
        {
            // Ignored MCDU is constructed but never connected.
            _ = new WinCtrlDisplayControl(
                WinCtrlConstants.PRODUCT_ID_MCDU_CPT,
                server
            );

            Assert.IsFalse(
                server.WebSocketServices.TryGetServiceHost(
                    "/winwing/cdu-captain",
                    out _
                )
            );

            var enabledPfp3n = new WinCtrlDisplayControl(
                WinCtrlConstants.PRODUCT_ID_PFP3N_CPT,
                server,
                new FakeWinCtrlMessageSender()
            );

            enabledPfp3n.Connect();

            Assert.IsTrue(
                server.WebSocketServices.TryGetServiceHost(
                    "/winwing/cdu-captain",
                    out _
                )
            );

            enabledPfp3n.Shutdown();
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (server.IsListening)
            {
                server.Stop();
            }
        }
    }
}
