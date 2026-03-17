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
    }
}