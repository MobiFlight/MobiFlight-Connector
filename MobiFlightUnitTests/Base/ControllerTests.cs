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
