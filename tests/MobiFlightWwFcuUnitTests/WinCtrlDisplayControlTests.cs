using MobiFlightWwFcu;
using System.Net;
using WebSocketSharp.Server;

namespace MobiFlightWwFcuUnitTests
{
    [TestClass]
    public class WinCtrlDisplayControlTests
    {
        private WebSocketServer server = null!;
        private string mcduFontDirectory = null!;
        private string pfpFontDirectory = null!;
        private bool mcduFontDirectoryCreatedByTest;
        private bool pfpFontDirectoryCreatedByTest;

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

            mcduFontDirectory = Path.Combine(
                baseDirectory,
                "Scripts",
                "Winwing",
                "Fonts",
                "Default",
                "MCDU"
                
            );

            pfpFontDirectory = Path.Combine(
                baseDirectory,
                "Scripts",
                "Winwing",
                "Fonts",
                "Default",
                "PFP"
            );

            mcduFontDirectoryCreatedByTest = !Directory.Exists(mcduFontDirectory);
            pfpFontDirectoryCreatedByTest = !Directory.Exists(pfpFontDirectory);

            Directory.CreateDirectory(mcduFontDirectory);
            Directory.CreateDirectory(pfpFontDirectory);
        }

        [TestMethod]
        public void CreatingMcdInstance_DoesNotCreateCaptainWebsocketSubscription()
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
        public void CreatingAndConnectingMcdInstance_CreatesCaptainWebsocketSubscription()
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

            if (mcduFontDirectoryCreatedByTest && Directory.Exists(mcduFontDirectory)) {
                Directory.Delete(mcduFontDirectory, true);
            }

            if (pfpFontDirectoryCreatedByTest && Directory.Exists(pfpFontDirectory))
            {
                Directory.Delete(pfpFontDirectory, true);
            }
        }
    }
}
