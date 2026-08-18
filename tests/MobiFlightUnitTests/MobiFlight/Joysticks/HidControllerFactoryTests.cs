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
        public void CanCreate_SwitchPanelInstanceName_ReturnsTrue()
        {
            Assert.IsTrue(HidControllerFactory.CanCreate("Logitech/Saitek Switch Panel"));
        }

        [TestMethod]
        public void Create_SwitchPanelDefinition_ReturnsSwitchPanel()
        {
            var definition = new JoystickDefinition
            {
                InstanceName = "Logitech/Saitek Switch Panel"
            };

            var controller = HidControllerFactory.Create(definition);

            Assert.IsInstanceOfType(controller, typeof(Logitech.SwitchPanel));
        }
    }
}
