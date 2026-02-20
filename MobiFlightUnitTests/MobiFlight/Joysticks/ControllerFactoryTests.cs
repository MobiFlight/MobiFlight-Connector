using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Joysticks.VKB;
using SharpDX.DirectInput;

namespace MobiFlight.Joysticks.Tests
{
    [TestClass()]
    public class ControllerFactoryTests
    {
        private const int WINWING_VENDOR_ID = 0x4098;
        private const int OTHER_VENDOR_ID = 0x1234;

        [TestMethod()]
        public void CanCreate_WithOctavi_ReturnsTrue()
        {
            var deviceInstance = CreateDeviceInstance("Octavi");
            var result = ControllerFactory.CanCreate(deviceInstance, OTHER_VENDOR_ID, 0x0000);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void CanCreate_WithIFR1_ReturnsTrue()
        {
            var deviceInstance = CreateDeviceInstance("IFR1");
            var result = ControllerFactory.CanCreate(deviceInstance, OTHER_VENDOR_ID, 0x0000);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void CanCreate_WithAuthentiKit_ReturnsTrue()
        {
            var deviceInstance = CreateDeviceInstance("AuthentiKit");
            var result = ControllerFactory.CanCreate(deviceInstance, OTHER_VENDOR_ID, 0x0000);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void CanCreate_WithAuthentiKitWithWhitespace_ReturnsTrue()
        {
            var deviceInstance = CreateDeviceInstance("AuthentiKit ");
            var result = ControllerFactory.CanCreate(deviceInstance, OTHER_VENDOR_ID, 0x0000);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void CanCreate_WithVKBVendorId_ReturnsTrue()
        {
            var deviceInstance = CreateDeviceInstance("Some VKB Device");
            var result = ControllerFactory.CanCreate(deviceInstance, VKBDevice.VKB_VENDOR_ID, 0x0000);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void CanCreate_WithWinwingDevice_ReturnsTrue()
        {
            var deviceInstance = CreateDeviceInstance("Winwing FCU");
            var result = ControllerFactory.CanCreate(deviceInstance, WINWING_VENDOR_ID, Winwing.WinwingConstants.PRODUCT_ID_FCU_ONLY);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void CanCreate_WithUnknownDevice_ReturnsFalse()
        {
            var deviceInstance = CreateDeviceInstance("Unknown Device");
            var result = ControllerFactory.CanCreate(deviceInstance, OTHER_VENDOR_ID, 0x9999);
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void CanCreate_WithStandardJoystick_ReturnsFalse()
        {
            var deviceInstance = CreateDeviceInstance("Standard Joystick");
            var result = ControllerFactory.CanCreate(deviceInstance, OTHER_VENDOR_ID, 0x0000);
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void GetProductName_WithNonVKBDevice_ReturnsInstanceName()
        {
            var deviceInstance = CreateDeviceInstance("Test Device");
            // We can't easily create a real DirectInput joystick for testing, so we'll pass null
            // The method should handle this gracefully for non-VKB devices
            var result = ControllerFactory.GetProductName(deviceInstance, null, OTHER_VENDOR_ID);
            Assert.AreEqual("Test Device", result);
        }

        [TestMethod()]
        public void GetProductName_WithWhitespace_ReturnsTrimmedName()
        {
            var deviceInstance = CreateDeviceInstance("  Test Device  ");
            var result = ControllerFactory.GetProductName(deviceInstance, null, OTHER_VENDOR_ID);
            Assert.AreEqual("Test Device", result);
        }

        /// <summary>
        /// Helper method to create a mock DeviceInstance for testing.
        /// </summary>
        private DeviceInstance CreateDeviceInstance(string instanceName)
        {
            // DeviceInstance is a struct, so we can create it directly
            // We only need to set the InstanceName for our tests
            return new DeviceInstance
            {
                InstanceName = instanceName
            };
        }
    }
}
