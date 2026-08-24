using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight;
using System.Collections.Generic;

namespace MobiFlightUnitTests.MobiFlight.Joysticks
{
    [TestClass()]
    public class JoystickManagerTests
    {
        [TestMethod()]
        public void ShouldConnectHidJoystick_WhenJoystickIsExcluded_ReturnsFalse()
        {
            var excludedJoysticks = new List<string>
            {
                "WingFlex EFIS"
            };

            var result = JoystickManager.ShouldConnectHidJoystick(
                "WingFlex EFIS",
                excludedJoysticks
            );

            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void ShouldConnectHidJoystick_WhenJoystickIsNotExcluded_ReturnsTrue()
        {
            var excludedJoysticks = new List<string>
            {
                "WingFlex EFIS"
            };

            var result = JoystickManager.ShouldConnectHidJoystick(
                "WingFlex FCU",
                excludedJoysticks
            );

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void ShouldConnectHidJoystick_WhenNoJoysticksAreExcluded_ReturnsTrue()
        {
            var excludedJoysticks = new List<string>();

            var result = JoystickManager.ShouldConnectHidJoystick(
                "WingFlex EFIS",
                excludedJoysticks
            );

            Assert.IsTrue(result);
        }
    }
}