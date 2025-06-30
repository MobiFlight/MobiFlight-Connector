using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace MobiFlight.Tests
{
    [TestClass()]
    public class JoystickDefinitionTests
    {
        [TestMethod()]
        public void MapDeviceNameToLabelTest_WhenDeviceNameExists()
        {
            var inputDefinition = new JoystickInput()
            {
                Id = 17,
                Type = JoystickDeviceType.Button,
                Label = "Mode - IAS"
            };

            var joystickDefinition = new JoystickDefinition
            {
                Inputs = new List<JoystickInput> { inputDefinition }
            };

            // Act
            var label = joystickDefinition.MapDeviceNameToLabel("Button 17");

            // Assert
            Assert.AreEqual("Mode - IAS", label);
        }

        [TestMethod()]
        public void MapDeviceNameToLabelTest_WhenDeviceNameNotExists()
        {
            var inputDefinition = new JoystickInput()
            {
                Id = 17,
                Type = JoystickDeviceType.Button,
                Label = "Mode - IAS"
            };

            var joystickDefinition = new JoystickDefinition
            {
                Inputs = new List<JoystickInput> { inputDefinition }
            };

            // Act
            var label = joystickDefinition.MapDeviceNameToLabel("Button 18");

            // Assert
            Assert.AreEqual("Button 18", label);
        }
    }
}