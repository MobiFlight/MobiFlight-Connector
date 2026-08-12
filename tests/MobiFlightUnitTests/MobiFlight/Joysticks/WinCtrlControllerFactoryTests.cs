using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Joysticks.WinCtrl;
using MobiFlightWwFcu;

namespace MobiFlight.Joysticks.Tests
{
    [TestClass()]
    public class WinCtrlControllerFactoryTests
    {
        private const int WINCTRL_VENDOR_ID = 0x4098;
        private const int OTHER_VENDOR_ID = 0x1234;

        [TestMethod()]
        public void CanCreate_WithWinCtrlFCU_ReturnsTrue()
        {
            var result = WinCtrlControllerFactory.CanCreate(WINCTRL_VENDOR_ID, WinCtrlConstants.PRODUCT_ID_FCU_ONLY);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void CanCreate_WithWinCtrlCDU_ReturnsTrue()
        {
            var result = WinCtrlControllerFactory.CanCreate(WINCTRL_VENDOR_ID, WinCtrlConstants.PRODUCT_ID_MCDU_CPT);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void CanCreate_WithWinCtrlPAP3_ReturnsTrue()
        {
            var result = WinCtrlControllerFactory.CanCreate(WINCTRL_VENDOR_ID, WinCtrlConstants.PRODUCT_ID_PAP3);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void CanCreate_WithWinCtrlAirbusThrottle_ReturnsTrue()
        {
            var result = WinCtrlControllerFactory.CanCreate(WINCTRL_VENDOR_ID, WinCtrlConstants.PRODUCT_ID_AIRBUS_THROTTLE_L);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void CanCreate_WithWinCtrlAirbusStick_ReturnsTrue()
        {
            var result = WinCtrlControllerFactory.CanCreate(WINCTRL_VENDOR_ID, WinCtrlConstants.PRODUCT_ID_AIRBUS_STICK_L);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void CanCreate_WithWinCtrlPDC3_ReturnsTrue()
        {
            var result = WinCtrlControllerFactory.CanCreate(WINCTRL_VENDOR_ID, WinCtrlConstants.PRODUCT_ID_3NPDCL);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void CanCreate_WithWinCtrlECAM_ReturnsTrue()
        {
            var result = WinCtrlControllerFactory.CanCreate(WINCTRL_VENDOR_ID, WinCtrlConstants.PRODUCT_ID_ECAM);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void CanCreate_WithWinCtrlAGP_ReturnsTrue()
        {
            var result = WinCtrlControllerFactory.CanCreate(WINCTRL_VENDOR_ID, WinCtrlConstants.PRODUCT_ID_AGP);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void CanCreate_WithWinCtrlPTO2_ReturnsTrue()
        {
            var result = WinCtrlControllerFactory.CanCreate(WINCTRL_VENDOR_ID, WinCtrlConstants.PRODUCT_ID_PTO2);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void CanCreate_WithWrongVendorId_ReturnsFalse()
        {
            var result = WinCtrlControllerFactory.CanCreate(OTHER_VENDOR_ID, WinCtrlConstants.PRODUCT_ID_FCU_ONLY);
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void CanCreate_WithUnknownProductId_ReturnsFalse()
        {
            var result = WinCtrlControllerFactory.CanCreate(WINCTRL_VENDOR_ID, 0x9999);
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void CanCreate_WithWrongVendorAndProductId_ReturnsFalse()
        {
            var result = WinCtrlControllerFactory.CanCreate(OTHER_VENDOR_ID, 0x9999);
            Assert.IsFalse(result);
        }
    }
}
