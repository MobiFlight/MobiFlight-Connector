using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base;

namespace MobiFlight.Base.Tests
{
    [TestClass()]
    public class ControllerTests
    {
        [TestMethod()]
        public void Constructor_Default_InitializesEmptyStrings()
        {
            // Arrange & Act
            var controller = new Controller();

            // Assert
            Assert.IsNotNull(controller);
            Assert.AreEqual("", controller.Name);
            Assert.AreEqual("", controller.Serial);
        }

        [TestMethod()]
        public void Constructor_WithParameters_InitializesCorrectly()
        {
            // Arrange & Act
            var controller = new Controller("TestBoard", "SN-123-456");

            // Assert
            Assert.AreEqual("TestBoard", controller.Name);
            Assert.AreEqual("SN-123-456", controller.Serial);
        }

        [TestMethod()]
        public void Constructor_WithNullParameters_InitializesEmptyStrings()
        {
            // Arrange & Act
            var controller = new Controller(null, null);

            // Assert
            Assert.AreEqual("", controller.Name);
            Assert.AreEqual("", controller.Serial);
        }

        [TestMethod()]
        public void FromModuleSerial_ValidModuleSerial_ParsesCorrectly()
        {
            // Arrange
            var moduleSerial = "ProtoBoard-v2/ SN-5FC-1CF";

            // Act
            var controller = Controller.FromModuleSerial(moduleSerial);

            // Assert
            Assert.IsNotNull(controller);
            Assert.AreEqual("ProtoBoard-v2", controller.Name);
            Assert.AreEqual("SN-5FC-1CF", controller.Serial);
        }

        [TestMethod()]
        public void FromModuleSerial_EmptyString_ReturnsEmptyController()
        {
            // Arrange
            var moduleSerial = "";

            // Act
            var controller = Controller.FromModuleSerial(moduleSerial);

            // Assert
            Assert.IsNotNull(controller);
            Assert.AreEqual("", controller.Name);
            Assert.AreEqual("", controller.Serial);
        }

        [TestMethod()]
        public void FromModuleSerial_NullString_ReturnsEmptyController()
        {
            // Arrange
            string moduleSerial = null;

            // Act
            var controller = Controller.FromModuleSerial(moduleSerial);

            // Assert
            Assert.IsNotNull(controller);
            Assert.AreEqual("", controller.Name);
            Assert.AreEqual("", controller.Serial);
        }

        [TestMethod()]
        public void FromModuleSerial_JoystickSerial_ParsesCorrectly()
        {
            // Arrange
            var moduleSerial = "WINWING Orion Joystick Base 2 + JGRIP-F16/ JS-3145ad90-c6f2-11ef-8001-444553540000";

            // Act
            var controller = Controller.FromModuleSerial(moduleSerial);

            // Assert
            Assert.IsNotNull(controller);
            Assert.AreEqual("WINWING Orion Joystick Base 2 + JGRIP-F16", controller.Name);
            Assert.AreEqual("JS-3145ad90-c6f2-11ef-8001-444553540000", controller.Serial);
        }

        [TestMethod()]
        public void ToModuleSerial_ValidController_FormatsCorrectly()
        {
            // Arrange
            var controller = new Controller("ProtoBoard-v2", "SN-5FC-1CF");

            // Act
            var moduleSerial = controller.ToModuleSerial();

            // Assert
            Assert.AreEqual("ProtoBoard-v2/ SN-5FC-1CF", moduleSerial);
        }

        [TestMethod()]
        public void ToModuleSerial_EmptyController_ReturnsEmptyString()
        {
            // Arrange
            var controller = new Controller("", "");

            // Act
            var moduleSerial = controller.ToModuleSerial();

            // Assert
            Assert.AreEqual("", moduleSerial);
        }

        [TestMethod()]
        public void ToModuleSerial_NameOnlyNoSerial_ReturnsNameOnly()
        {
            // Arrange
            var controller = new Controller("TestBoard", "");

            // Act
            var moduleSerial = controller.ToModuleSerial();

            // Assert
            Assert.AreEqual("TestBoard", moduleSerial);
        }

        [TestMethod()]
        public void RoundTrip_ModuleSerialToControllerAndBack_PreservesValue()
        {
            // Arrange
            var originalModuleSerial = "ProtoBoard-v2/ SN-5FC-1CF";

            // Act
            var controller = Controller.FromModuleSerial(originalModuleSerial);
            var resultModuleSerial = controller.ToModuleSerial();

            // Assert
            Assert.AreEqual(originalModuleSerial, resultModuleSerial);
        }

        [TestMethod()]
        public void Equals_SameValues_ReturnsTrue()
        {
            // Arrange
            var controller1 = new Controller("TestBoard", "SN-123");
            var controller2 = new Controller("TestBoard", "SN-123");

            // Act & Assert
            Assert.IsTrue(controller1.Equals(controller2));
        }

        [TestMethod()]
        public void Equals_DifferentName_ReturnsFalse()
        {
            // Arrange
            var controller1 = new Controller("TestBoard1", "SN-123");
            var controller2 = new Controller("TestBoard2", "SN-123");

            // Act & Assert
            Assert.IsFalse(controller1.Equals(controller2));
        }

        [TestMethod()]
        public void Equals_DifferentSerial_ReturnsFalse()
        {
            // Arrange
            var controller1 = new Controller("TestBoard", "SN-123");
            var controller2 = new Controller("TestBoard", "SN-456");

            // Act & Assert
            Assert.IsFalse(controller1.Equals(controller2));
        }

        [TestMethod()]
        public void Equals_Null_ReturnsFalse()
        {
            // Arrange
            var controller = new Controller("TestBoard", "SN-123");

            // Act & Assert
            Assert.IsFalse(controller.Equals(null));
        }

        [TestMethod()]
        public void Clone_CreatesIndependentCopy()
        {
            // Arrange
            var original = new Controller("TestBoard", "SN-123");

            // Act
            var clone = original.Clone();
            clone.Name = "ModifiedBoard";
            clone.Serial = "SN-999";

            // Assert
            Assert.AreEqual("TestBoard", original.Name);
            Assert.AreEqual("SN-123", original.Serial);
            Assert.AreEqual("ModifiedBoard", clone.Name);
            Assert.AreEqual("SN-999", clone.Serial);
        }

        [TestMethod()]
        public void GetHashCode_SameValues_ReturnsSameHashCode()
        {
            // Arrange
            var controller1 = new Controller("TestBoard", "SN-123");
            var controller2 = new Controller("TestBoard", "SN-123");

            // Act
            var hash1 = controller1.GetHashCode();
            var hash2 = controller2.GetHashCode();

            // Assert
            Assert.AreEqual(hash1, hash2);
        }
    }
}
