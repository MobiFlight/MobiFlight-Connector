using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight;
using MobiFlight.Joysticks;
using System.Collections.Generic;

namespace MobiFlightUnitTests.MobiFlight.Joysticks
{
    [TestClass()]
    public class JoystickManagerTests
    {
        /// <summary>Stands in for a joystick without requiring a DirectInput device.</summary>
        class NamedJoystick : Joystick
        {
            private readonly string name;

            public NamedJoystick(string name) : base(null, new JoystickDefinition { InstanceName = name })
            {
                this.name = name;
            }

            public override string Name => name;
        }

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
            var joystick = new NamedJoystick("EFIS Cube");
            var settingsExcludedJoysticks = new List<string> { "EFIS Cube" };

            var result = manager.TryExcludeJoystick(joystick, settingsExcludedJoysticks);

            Assert.IsTrue(result);
            CollectionAssert.Contains(manager.GetExcludedJoysticks(), joystick);
        }

        [TestMethod()]
        public void TryExcludeJoystick_WhenJoystickIsNotExcluded_DoesNotAddIt()
        {
            var manager = new JoystickManager();
            var joystick = new NamedJoystick("FCU Cube");
            var settingsExcludedJoysticks = new List<string> { "EFIS Cube" };

            var result = manager.TryExcludeJoystick(joystick, settingsExcludedJoysticks);

            Assert.IsFalse(result);
            CollectionAssert.DoesNotContain(manager.GetExcludedJoysticks(), joystick);
        }
    }
}