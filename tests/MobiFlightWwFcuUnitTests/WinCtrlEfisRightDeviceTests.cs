using MobiFlightWwFcu;
using MobiFlightWwFcuUnitTests.Mocks;

namespace MobiFlightWwFcuUnitTests
{
    [TestClass]
    public class WinCtrlEfisRightControllerTests
    {
        private MockWinCtrlMessageSender mockMessageSender = null!;
        private WinCtrlEfisController device = null!;

        // The EFIS-Right destination (0x0E, 0xBF) is the prefix on every command frame.
        // RefreshCommand is always identical (17 bytes, header only).
        private static readonly byte[] RefreshCommand = new byte[]
        {
            0x0E, 0xBF, 0x00, 0x00, 0x03, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        [TestInitialize]
        public void Setup()
        {
            mockMessageSender = new MockWinCtrlMessageSender();
            // Use the "Right" EFIS variant for all tests (DEST_EFISR = 0x0E 0xBF).
            device = new WinCtrlEfisController(mockMessageSender, "Right");
        }

        [TestCleanup]
        public void Cleanup()
        {
            device?.Stop();
        }

        #region Annunciator Light Tests

        [TestMethod]
        public void SetDisplay_AnnunciatorLight_WithOne_ShouldTurnOnAllLights_Right()
        {
            device.SetDisplay("LCD Test On/Off", "1");
            Assert.HasCount(1, mockMessageSender.DisplayCommandsSent);

            // 18-byte LCD-test frame; byte 17 carries the AllOn mode (0x43).
            List<byte[]> expectedCommands = new List<byte[]>()
            {
                new byte[] { 0x0E, 0xBF, 0x00, 0x00, 0x04, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x43 },
                RefreshCommand
            };

            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[0].Commands, expectedCommands);
        }

        [TestMethod]
        public void SetDisplay_AnnunciatorLight_WithZero_ShouldResendCurrentBuffer_Right()
        {
            device.SetDisplay("LCD Test On/Off", "0");
            Assert.HasCount(1, mockMessageSender.DisplayCommandsSent);

            // The "off" branch resends the current SetValuesCommand (post-init "1013 QNH").
            List<byte[]> expectedCommands = new List<byte[]>()
            {
                new byte[] { 0x0E, 0xBF, 0x00, 0x00, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x60, 0x7D, 0x60, 0x7A, 0x02 },
                RefreshCommand
            };

            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[0].Commands, expectedCommands);
        }

        #endregion

        #region Helpers

        private void CompareDisplayCommands(List<byte[]> sentCommands, List<byte[]> expectedCommands)
        {
            Assert.HasCount(expectedCommands.Count, sentCommands);

            for (int i = 0; i < expectedCommands.Count; i++)
            {
                CollectionAssert.AreEqual(expectedCommands[i], sentCommands[i]);
            }
        }

        #endregion
    }
}
