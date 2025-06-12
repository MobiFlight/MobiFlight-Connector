using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MobiFlight.InputConfig.Tests
{
    [TestClass()]
    public class InputDeviceConfigFactoryTests
    {
        [TestMethod()]
        public void CreateFromTypeTest_Button()
        {
            // Arrange
            var type = MobiFlightButton.TYPE;
            // Act
            var result = InputDeviceConfigFactory.CreateFromType(type);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(MobiFlightButton));
        }

        [TestMethod()]
        public void CreateFromTypeTest_Encoder()
        {
            // Arrange
            var type = MobiFlightEncoder.TYPE;
            // Act
            var result = InputDeviceConfigFactory.CreateFromType(type);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(MobiFlightEncoder));
        }

        [TestMethod()]
        public void CreateFromTypeTest_AnalogInput()
        {
            // Arrange
            var type = MobiFlightAnalogInput.TYPE;
            // Act
            var result = InputDeviceConfigFactory.CreateFromType(type);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(MobiFlightAnalogInput));
        }

        [TestMethod()]
        public void CreateFromTypeTest_AnalogInput_Old()
        {
            // Arrange
            var type = MobiFlightAnalogInput.TYPE_OLD;
            // Act
            var result = InputDeviceConfigFactory.CreateFromType(type);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(MobiFlightAnalogInput));
        }

        [TestMethod()]
        public void CreateFromTypeTest_InputShiftRegister()
        {
            // Arrange
            var type = MobiFlightInputShiftRegister.TYPE;
            // Act
            var result = InputDeviceConfigFactory.CreateFromType(type);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(MobiFlightInputShiftRegister));
        }

        [TestMethod()]
        public void CreateFromTypeTest_InputMultiplexer()
        {
            // Arrange
            var type = MobiFlightInputMultiplexer.TYPE;
            // Act
            var result = InputDeviceConfigFactory.CreateFromType(type);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(MobiFlightInputMultiplexer));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateFromTypeTest_UnknownType()
        {
            // Arrange
            var type = "UnknownType";
            // Act
            InputDeviceConfigFactory.CreateFromType(type);
            // Assert is handled by ExpectedException
        }
    }
}