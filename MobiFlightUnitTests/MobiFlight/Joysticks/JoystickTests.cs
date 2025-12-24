using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace MobiFlight.Tests
{
    [TestClass()]
    public class JoystickTests
    {
        [TestMethod()]
        public void GetAxisNameForUsage_ShouldReturnNamesForValidIds()
        {
            // Arrange - Valid axis usage IDs that map to JoystickState properties
            var validUsageIds = new List<int>
            {
                48, // X
                49, // Y
                50, // Z
                51, // RotationX
                52, // RotationY
                53, // RotationZ
                54, // Slider1
                55  // Slider2
            };

            // Act & Assert - Valid IDs should return axis names
            validUsageIds.ForEach(id =>
            {
                var axisName = Joystick.GetAxisNameForUsage(id);
                Assert.IsFalse(string.IsNullOrEmpty(axisName),
                    $"UsageMap should contain valid usage ID {id} and return a non-empty axis name.");
            });
        }

        [TestMethod()]
        public void GetAxisNameForUsage_ShouldThrowExceptionForInvalidIds()
        {
            // Arrange - Invalid usage IDs that don't map to any JoystickState property
            var invalidUsageIds = new List<int>
            {
                46, // Below valid range
                47, // Below valid range
                56, // Wheel - not supported by JoystickState
                57  // HatSwitch - not supported by JoystickState (should be POV)
            };

            invalidUsageIds.ForEach(id =>
            {
                Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Joystick.GetAxisNameForUsage(id));
            });
        }

        [TestMethod()]
        public void GetAxisNameForUsage_ValidIds_ReturnsExpectedNames()
        {
            // Verify exact mappings
            Assert.AreEqual("X", Joystick.GetAxisNameForUsage(48));
            Assert.AreEqual("Y", Joystick.GetAxisNameForUsage(49));
            Assert.AreEqual("Z", Joystick.GetAxisNameForUsage(50));
            Assert.AreEqual("RotationX", Joystick.GetAxisNameForUsage(51));
            Assert.AreEqual("RotationY", Joystick.GetAxisNameForUsage(52));
            Assert.AreEqual("RotationZ", Joystick.GetAxisNameForUsage(53));
            Assert.AreEqual("Slider1", Joystick.GetAxisNameForUsage(54));
            Assert.AreEqual("Slider2", Joystick.GetAxisNameForUsage(55));
        }
    }
}