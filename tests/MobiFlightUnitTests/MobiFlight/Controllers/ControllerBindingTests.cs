using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base;
using MobiFlight.Controllers;
using System.Windows.Forms;

namespace MobiFlightUnitTests.Controllers
{
    [TestClass]
    public class ControllerBindingTests
    {
        [TestMethod]
        public void Equals_SameValues_ReturnsTrue()
        {
            // Arrange
            var controller1 = new Controller { Name = "Board1", Serial = "SN-111" };
            var controller2 = new Controller { Name = "Board1", Serial = "SN-111" };
            
            var binding1 = new ControllerBinding
            {
                OriginalController = controller1,
                BoundController = controller2,
                Status = ControllerBindingStatus.Match
            };
            
            var binding2 = new ControllerBinding
            {
                OriginalController = new Controller { Name = "Board1", Serial = "SN-111" },
                BoundController = new Controller { Name = "Board1", Serial = "SN-111" },
                Status = ControllerBindingStatus.Match
            };

            // Act
            bool result = binding1.Equals(binding2);

            // Assert
            Assert.IsTrue(result, "Bindings with same values should be equal");
            Assert.AreEqual(binding1.GetHashCode(), binding2.GetHashCode(), "Equal bindings should have same hash code");
        }

        [TestMethod]
        public void Equals_DifferentBoundController_ReturnsFalse()
        {
            // Arrange
            var binding1 = new ControllerBinding
            {
                OriginalController = new Controller { Name = "Board1", Serial = "SN-111" },
                BoundController = new Controller { Name = "Board1", Serial = "SN-111" },
                Status = ControllerBindingStatus.Match
            };
            
            var binding2 = new ControllerBinding
            {
                OriginalController = new Controller { Name = "Board1", Serial = "SN-111" },
                BoundController = new Controller { Name = "Board2", Serial = "SN-222" },
                Status = ControllerBindingStatus.Match
            };

            // Act
            bool result = binding1.Equals(binding2);

            // Assert
            Assert.IsFalse(result, "Bindings with different BoundController should not be equal");
            Assert.AreNotEqual(binding1.GetHashCode(), binding2.GetHashCode(), "Bindings with different values should have different hash codes");
        }

        [TestMethod]
        public void Equals_DifferentStatus_ReturnsFalse()
        {
            // Arrange
            var binding1 = new ControllerBinding
            {
                OriginalController = new Controller { Name = "Board1", Serial = "SN-111" },
                BoundController = new Controller { Name = "Board1", Serial = "SN-111" },
                Status = ControllerBindingStatus.Match
            };
            
            var binding2 = new ControllerBinding
            {
                OriginalController = new Controller { Name = "Board1", Serial = "SN-111" },
                BoundController = new Controller { Name = "Board1", Serial = "SN-111" },
                Status = ControllerBindingStatus.AutoBind
            };

            // Act
            bool result = binding1.Equals(binding2);

            // Assert
            Assert.IsFalse(result, "Bindings with different Status should not be equal");
            Assert.AreNotEqual(binding1.GetHashCode(), binding2.GetHashCode(), "Bindings with different values should have different hash codes");
        }

        [TestMethod]
        public void Equals_DifferentOriginalController_ReturnsFalse()
        {
            // Arrange
            var binding1 = new ControllerBinding
            {
                OriginalController = new Controller { Name = "Board1", Serial = "SN-111" },
                BoundController = new Controller { Name = "Board1", Serial = "SN-111" },
                Status = ControllerBindingStatus.Match
            };
            
            var binding2 = new ControllerBinding
            {
                OriginalController = new Controller { Name = "Board2", Serial = "SN-222" },
                BoundController = new Controller { Name = "Board1", Serial = "SN-111" },
                Status = ControllerBindingStatus.Match
            };

            // Act
            bool result = binding1.Equals(binding2);

            // Assert
            Assert.IsFalse(result, "Bindings with different OriginalController should not be equal");
            Assert.AreNotEqual(binding1.GetHashCode(), binding2.GetHashCode(), "Bindings with different values should have different hash codes");
        }

        [TestMethod]
        public void Equals_Null_ReturnsFalse()
        {
            // Arrange
            var binding = new ControllerBinding
            {
                OriginalController = new Controller { Name = "Board1", Serial = "SN-111" },
                BoundController = new Controller { Name = "Board1", Serial = "SN-111" },
                Status = ControllerBindingStatus.Match
            };

            // Act
            bool result = binding.Equals(null);

            // Assert
            Assert.IsFalse(result, "Binding should not equal null");
        }

        [TestMethod]
        public void Equals_DifferentType_ReturnsFalse()
        {
            // Arrange
            var binding = new ControllerBinding
            {
                OriginalController = new Controller { Name = "Board1", Serial = "SN-111" },
                BoundController = new Controller { Name = "Board1", Serial = "SN-111" },
                Status = ControllerBindingStatus.Match
            };

            // Act
            bool result = binding.Equals("not a controller binding");

            // Assert
            Assert.IsFalse(result, "Binding should not equal object of different type");
        }

        [TestMethod]
        public void Equals_NullControllers_HandlesGracefully()
        {
            // Arrange
            var binding1 = new ControllerBinding
            {
                OriginalController = null,
                BoundController = null,
                Status = ControllerBindingStatus.Missing
            };
            
            var binding2 = new ControllerBinding
            {
                OriginalController = null,
                BoundController = null,
                Status = ControllerBindingStatus.Missing
            };

            // Act & Assert - Should not throw NullReferenceException
            bool result = binding1.Equals(binding2);
            Assert.IsTrue(result, "Bindings with null controllers should be equal if status matches");
            Assert.AreEqual(binding1.GetHashCode(), binding2.GetHashCode(), "Bindings with different values should have different hash codes");
        }

        [TestMethod]
        public void Equals_AllStatuses_ComparesCorrectly()
        {
            // Test all enum values
            var statuses = new[]
            {
                ControllerBindingStatus.Match,
                ControllerBindingStatus.AutoBind,
                ControllerBindingStatus.RequiresManualBind,
                ControllerBindingStatus.Missing
            };

            foreach (var status in statuses)
            {
                var binding1 = new ControllerBinding
                {
                    OriginalController = new Controller { Name = "Board1", Serial = "SN-111" },
                    BoundController = new Controller { Name = "Board1", Serial = "SN-111" },
                    Status = status
                };
                
                var binding2 = new ControllerBinding
                {
                    OriginalController = new Controller { Name = "Board1", Serial = "SN-111" },
                    BoundController = new Controller { Name = "Board1", Serial = "SN-111" },
                    Status = status
                };

                Assert.IsTrue(binding1.Equals(binding2), 
                    $"Bindings with same status {status} should be equal");

                Assert.AreEqual(binding1.GetHashCode(), binding2.GetHashCode(), "Bindings with different values should have different hash codes");
            }
        }
    }
}