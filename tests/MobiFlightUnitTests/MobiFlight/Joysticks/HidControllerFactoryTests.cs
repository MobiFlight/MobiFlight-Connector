using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Joysticks.Cdu.Tests.Mocks;

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
            Assert.IsTrue(HidControllerFactory.CanCreate("Logitech Switch Panel"));
        }

        [TestMethod]
        public void Create_SwitchPanelDefinition_ReturnsSwitchPanel()
        {
            var definition = new JoystickDefinition
            {
                InstanceName = "Logitech Switch Panel"
            };

            var controller = HidControllerFactory.Create(definition, new FakeCduWebsocketHub());

            Assert.IsInstanceOfType(controller, typeof(Logitech.SwitchPanel));
            Assert.AreEqual("Logitech Switch Panel", controller.Name);
            Assert.AreEqual("JS-LOGITECH-SWITCH-PANEL-1234-ABCD-12345678", controller.Serial);
        }

        [TestMethod]
        public void CanCreate_MozaFcdDisplayInstanceName_ReturnsTrue()
        {
            Assert.IsTrue(HidControllerFactory.CanCreate("MOZA MA3F MFCD Display"));
        }

        [TestMethod]
        public void Create_MozaFcdDisplayDefinition_ReturnsMozaMcdu()
        {
            var definition = new JoystickDefinition
            {
                InstanceName = "MOZA MA3F MFCD Display"
            };

            var controller = HidControllerFactory.Create(definition, new FakeCduWebsocketHub());

            Assert.IsInstanceOfType(controller, typeof(Moza.MozaMcdu));
        }
    }
}
