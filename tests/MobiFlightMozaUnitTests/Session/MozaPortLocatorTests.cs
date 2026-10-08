namespace MobiFlightMoza.Session.Tests
{
    [TestClass]
    public class MozaPortLocatorTests
    {
        [TestMethod]
        public void TryParsePnpEntity_TypicalUsbCaption_ParsesVendorProductAndPort()
        {
            // Arrange
            string hardwareId = "USB\\VID_1B4F&PID_9206&REV_0100&MI_00";
            string caption = "USB Serial Device (COM22)";
            // Act
            bool parsed = MozaPortLocator.TryParsePnpEntity(hardwareId, caption, out var info);
            // Assert
            Assert.IsTrue(parsed);
            Assert.AreEqual(0x1B4F, info.VendorId);
            Assert.AreEqual(0x9206, info.ProductId);
            Assert.AreEqual("COM22", info.PortName);
        }

        [TestMethod]
        public void TryParsePnpEntity_NoVidPidInHardwareId_ReturnsFalse()
        {
            // Arrange
            string hardwareId = "ROOT\\SOMETHING\\0000";
            string caption = "Some Device (COM4)";
            // Act
            bool parsed = MozaPortLocator.TryParsePnpEntity(hardwareId, caption, out _);
            // Assert
            Assert.IsFalse(parsed);
        }

        [TestMethod]
        public void TryParsePnpEntity_CaptionWithoutComPort_ReturnsFalse()
        {
            // Arrange - Issue 1778 shape: a device renamed in Device Manager, dropping the port.
            string hardwareId = "USB\\VID_1B4F&PID_9206&MI_00";
            string caption = "My Renamed Device";
            // Act
            bool parsed = MozaPortLocator.TryParsePnpEntity(hardwareId, caption, out _);
            // Assert
            Assert.IsFalse(parsed);
        }

        [TestMethod]
        public void TryParsePnpEntity_EmptyHardwareIdOrCaption_ReturnsFalse()
        {
            Assert.IsFalse(MozaPortLocator.TryParsePnpEntity("", "USB Serial Device (COM4)", out _));
            Assert.IsFalse(MozaPortLocator.TryParsePnpEntity("USB\\VID_1B4F&PID_9206", "", out _));
        }
    }
}
