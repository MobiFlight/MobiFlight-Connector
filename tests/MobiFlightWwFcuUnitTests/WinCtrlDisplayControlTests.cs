using MobiFlightWwFcu;
using System.Net;
using WebSocketSharp.Server;

namespace MobiFlightWwFcuUnitTests
{
    [TestClass]
    public class WinCtrlDisplayControlTests
    {
        private WebSocketServer server = null!;

        [TestInitialize]
        public void Setup()
        {
            server = new WebSocketServer(
                IPAddress.Loopback,
                8320
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
        public void RegisterCduWebSocketService_RegistersConfiguredPath()
        {
            // Arrange
            var displayControl = new WinCtrlDisplayControl(
                WinCtrlConstants.PRODUCT_ID_PFP3N_CPT,
                server
            );

            // Act
            displayControl.RegisterCduWebSocketService();

            // Assert
            Assert.IsTrue(
                server.WebSocketServices.TryGetServiceHost(
                    "/winwing/cdu-captain",
                    out _
                )
            );
        }

        [TestMethod]
        public void IgnoredMcdu_DoesNotBlockPfp3nWebSocketRegistration(){
            // Arrange
            _ = new WinCtrlDisplayControl(
                WinCtrlConstants.PRODUCT_ID_MCDU_CPT,
                server
            );

            Assert.AreEqual(
                0,
                server.WebSocketServices.Count
            );

            var pfp3n = new WinCtrlDisplayControl(
                WinCtrlConstants.PRODUCT_ID_PFP3N_CPT,
                server
            );

            Assert.AreEqual(
                0,
                server.WebSocketServices.Count
            );

            // Act
            pfp3n.RegisterCduWebSocketService();

            // Assert
            Assert.AreEqual(
                1,
                server.WebSocketServices.Count
            );

            Assert.IsTrue(
                server.WebSocketServices.TryGetServiceHost(
                    "/winwing/cdu-captain",
                    out _
                )
            );
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
