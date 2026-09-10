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
