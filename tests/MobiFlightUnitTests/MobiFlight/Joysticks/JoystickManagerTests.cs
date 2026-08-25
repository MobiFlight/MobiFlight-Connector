using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight;
using System.Collections.Generic;

namespace MobiFlightUnitTests.MobiFlight.Joysticks
{
    [TestClass()]
    public class JoystickManagerTests
    {
        [TestMethod()]
        public void IsExcludedJoystick_WhenJoystickIsExcluded_ReturnsTrue()
        {
            var excludedJoysticks = new List<string>
              {
                "WingFlex EFIS"

              };

            var result = JoystickManager.IsExcludedJoystick(
                "WingFlex EFIS",
                excludedJoysticks
            );

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void IsExcludedJoystick_WhenJoystickIsNotExcluded_ReturnsFalse()
        {
            var excludedJoysticks = new List<string>
             {
               "WingFlex EFIS"
             };

            var result = JoystickManager.IsExcludedJoystick(
                "WingFlex FCU",
                excludedJoysticks
            );

            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void IsExcludedJoystick_WhenNoJoysticksAreExcluded_ReturnsFalse()
        {
            var excludedJoysticks = new List<string>();

            var result = JoystickManager.IsExcludedJoystick(
                "WingFlex EFIS",
                excludedJoysticks
            );

            Assert.IsFalse(result);
        }
    }
}