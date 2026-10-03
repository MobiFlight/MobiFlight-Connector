using MobiFlightWwFcu;
using System.Net;
using WebSocketSharp.Server;

namespace MobiFlightWwFcuUnitTests
{
    [TestClass]
    public class WinCtrlDisplayControlTests
    {
        private WebSocketServer server = null!;
        private static string mcduFontDirectory = null!;
        private static string pfpFontDirectory = null!;
        private static bool mcduFontDirectoryCreatedByTest;
        private static bool pfpFontDirectoryCreatedByTest;

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

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            var defaultFontDirectory = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Scripts",
                "Winwing",
                "Fonts",
                "Default"
            );

            mcduFontDirectory = Path.Combine(
                defaultFontDirectory,
                "MCDU"
            );

            pfpFontDirectory = Path.Combine(
                defaultFontDirectory,
                "PFP"
            );

            if (!Directory.Exists(mcduFontDirectory))
            {
                Directory.CreateDirectory(mcduFontDirectory);
                mcduFontDirectoryCreatedByTest = true;
            }

            if (!Directory.Exists(pfpFontDirectory))
            {
                Directory.CreateDirectory(pfpFontDirectory);
                pfpFontDirectoryCreatedByTest = true;
            }
        }


        [TestInitialize]
        public void Setup()
        {
            server = new WebSocketServer(
                IPAddress.Loopback,
                8320
            );
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
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            if (mcduFontDirectoryCreatedByTest && Directory.Exists(mcduFontDirectory))
            {
                Directory.Delete(mcduFontDirectory, true);
            }

            if (pfpFontDirectoryCreatedByTest && Directory.Exists(pfpFontDirectory))
            {
                Directory.Delete(pfpFontDirectory, true);
            }
        }
    }
}
