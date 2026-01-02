using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Joysticks.Winwing;

namespace MobiFlight.Joysticks.Tests
{
    [TestClass()]
    public class WinwingControllerFactoryTests
    {
        private const int WINWING_VENDOR_ID = 0x4098;
        private const int OTHER_VENDOR_ID = 0x1234;

        [TestMethod()]
        public void CanCreate_WithWinwingFCU_ReturnsTrue()
        {
            var result = WinwingControllerFactory.CanCreate(WINWING_VENDOR_ID, WinwingConstants.PRODUCT_ID_FCU_ONLY);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void CanCreate_WithWinwingCDU_ReturnsTrue()
        {
            var result = WinwingControllerFactory.CanCreate(WINWING_VENDOR_ID, WinwingConstants.PRODUCT_ID_MCDU_CPT);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void CanCreate_WithWinwingPAP3_ReturnsTrue()
        {
            var result = WinwingControllerFactory.CanCreate(WINWING_VENDOR_ID, WinwingConstants.PRODUCT_ID_PAP3_ONLY);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void CanCreate_WithWinwingAirbusThrottle_ReturnsTrue()
        {
            var result = WinwingControllerFactory.CanCreate(WINWING_VENDOR_ID, WinwingConstants.PRODUCT_ID_AIRBUS_THROTTLE_L);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void CanCreate_WithWinwingAirbusStick_ReturnsTrue()
        {
            var result = WinwingControllerFactory.CanCreate(WINWING_VENDOR_ID, WinwingConstants.PRODUCT_ID_AIRBUS_STICK_L);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void CanCreate_WithWinwingPDC3_ReturnsTrue()
        {
            var result = WinwingControllerFactory.CanCreate(WINWING_VENDOR_ID, WinwingConstants.PRODUCT_ID_3NPDCL);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void CanCreate_WithWinwingECAM_ReturnsTrue()
        {
            var result = WinwingControllerFactory.CanCreate(WINWING_VENDOR_ID, WinwingConstants.PRODUCT_ID_ECAM);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void CanCreate_WithWinwingAGP_ReturnsTrue()
        {
            var result = WinwingControllerFactory.CanCreate(WINWING_VENDOR_ID, WinwingConstants.PRODUCT_ID_AGP);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void CanCreate_WithWrongVendorId_ReturnsFalse()
        {
            var result = WinwingControllerFactory.CanCreate(OTHER_VENDOR_ID, WinwingConstants.PRODUCT_ID_FCU_ONLY);
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void CanCreate_WithUnknownProductId_ReturnsFalse()
        {
            var result = WinwingControllerFactory.CanCreate(WINWING_VENDOR_ID, 0x9999);
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void CanCreate_WithWrongVendorAndProductId_ReturnsFalse()
        {
            var result = WinwingControllerFactory.CanCreate(OTHER_VENDOR_ID, 0x9999);
            Assert.IsFalse(result);
        }
    }
}
