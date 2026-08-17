using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MobiFlight.Joysticks.Tests
{
    [TestClass()]
    public class HidControllerFactoryTests
    {
        [TestMethod()]
        public void CanCreateTest()
        {
            var canCreateFcuCube = HidControllerFactory.CanCreate("FCU Cube");
            Assert.IsTrue(canCreateFcuCube);
            var canCreateUnknown = HidControllerFactory.CanCreate("Unknown Device");
            Assert.IsFalse(canCreateUnknown);

            var canCreateWithWhitespace = HidControllerFactory.CanCreate("  FCU Cube  ");
            Assert.IsTrue(canCreateWithWhitespace);

            var canCreateEmptyString = HidControllerFactory.CanCreate("");
            Assert.IsFalse(canCreateEmptyString);

            var canCreateNullString = HidControllerFactory.CanCreate(null);
            Assert.IsFalse(canCreateNullString);
        }

        [TestMethod]
        public void CanCreate_Pz55VendorAndProductId_ReturnsTrue()
        {
            Assert.IsTrue(HidControllerFactory.CanCreate(
                Logitech.Pz55SwitchPanel.VendorId,
                Logitech.Pz55SwitchPanel.ProductId));
            Assert.IsFalse(HidControllerFactory.CanCreate(
                Logitech.Pz55SwitchPanel.VendorId,
                0xFFFF));
        }
    }
}
