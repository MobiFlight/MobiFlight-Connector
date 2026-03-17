using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MobiFlight.Base.Tests
{
    [TestClass()]
    public class ControllerTests
    {
        [TestMethod()]
        public void Constructor_Default_InitializesNullStrings()
        {
            // Arrange & Act
            var controller = new Controller();

            // Assert
            Assert.IsNotNull(controller);
            Assert.IsNull(controller.Name);
            Assert.IsNull(controller.Serial);
        }

        [TestMethod()]
        public void Constructor_CopyConstructor_CopiesValues()
        {
            // Arrange
            var original = new Controller();
            original.Name = "TestBoard";
            original.Serial = "SN-123-456";

            // Act
            var copy = new Controller(original);

            // Assert
            Assert.AreEqual("TestBoard", copy.Name);
            Assert.AreEqual("SN-123-456", copy.Serial);
        }

        [TestMethod()]
        public void Constructor_CopyConstructorWithNull_InitializesEmptyStrings()
        {
            // Arrange & Act
            var controller = new Controller(null);

            // Assert
            Assert.IsNull(controller.Name);
            Assert.IsNull(controller.Serial);
        }

        [TestMethod()]
        public void Equals_SameValues_ReturnsTrue()
        {
            // Arrange
            var controller1 = new Controller();
            controller1.Name = "TestBoard";
            controller1.Serial = "SN-123";

            var controller2 = new Controller();
            controller2.Name = "TestBoard";
            controller2.Serial = "SN-123";

            // Act & Assert
            Assert.IsTrue(controller1.Equals(controller2));
        }

        [TestMethod()]
        public void Equals_DifferentName_ReturnsFalse()
        {
            // Arrange
            var controller1 = new Controller() { Name = "TestBoard1", Serial = "SN-123" };
            var controller2 = new Controller() { Name = "TestBoard2", Serial = "SN-123" };

            // Act & Assert
            Assert.IsFalse(controller1.Equals(controller2));
        }

        [TestMethod()]
        public void Equals_DifferentSerial_ReturnsFalse()
        {
            // Arrange
            var controller1 = new Controller() { Name = "TestBoard", Serial = "SN-123" };
            var controller2 = new Controller() { Name = "TestBoard", Serial = "SN-456" };

            // Act & Assert
            Assert.IsFalse(controller1.Equals(controller2));
        }

        [TestMethod()]
        public void Equals_Null_ReturnsFalse()
        {
            // Arrange
            var controller = new Controller() { Name = "TestBoard", Serial = "SN-123" };

            // Act & Assert
            Assert.IsFalse(controller.Equals(null));
        }

        [TestMethod()]
        public void Clone_CreatesIndependentCopy()
        {
            // Arrange
            var original = new Controller() { Name = "TestBoard", Serial = "SN-123" };

            // Act
            var clone = original.Clone() as Controller;
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
            var controller1 = new Controller() { Name = "TestBoard", Serial = "SN-123" };
            var controller2 = new Controller() { Name = "TestBoard", Serial = "SN-123" };

            // Act
            var hash1 = controller1.GetHashCode();
            var hash2 = controller2.GetHashCode();

            // Assert
            Assert.AreEqual(hash1, hash2);
        }
    }
}