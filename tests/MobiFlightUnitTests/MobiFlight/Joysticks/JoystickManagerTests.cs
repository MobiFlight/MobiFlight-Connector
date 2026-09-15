using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight;
using MobiFlight.Joysticks;
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

        [TestMethod()]
        public void TryExcludeJoystick_WhenJoystickIsExcluded_AddsItToExcludedJoysticks()
        {
            var manager = new JoystickManager();

            var settingsExcludedJoysticks = new List<string> { "EFIS Cube" };

            var result = manager.TryExcludeJoystick("EFIS Cube", settingsExcludedJoysticks);

            Assert.IsTrue(result);
            CollectionAssert.Contains(manager.GetExcludedJoystickNames(), "EFIS Cube");
        }

        [TestMethod()]
        public void TryExcludeJoystick_WhenJoystickIsNotExcluded_DoesNotAddIt()
        {
            var manager = new JoystickManager();

            var settingsExcludedJoysticks = new List<string> { "EFIS Cube" };

            var result = manager.TryExcludeJoystick("FCU Cube", settingsExcludedJoysticks);

            Assert.IsFalse(result);
            CollectionAssert.DoesNotContain(manager.GetExcludedJoystickNames(), "FCU Cube");
        }
    }
}