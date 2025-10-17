using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MobiFlight.Tests
{
    [TestClass()]
    public class InputEventArgsTests
    {
        [TestMethod]
        public void GetEventActionLabel_ButtonPressEvent_ReturnsCorrectString()
        {
            // Arrange
            var inputEvent = new InputEventArgs
            {
                Type = DeviceType.Button,
                Value = (int)MobiFlightButton.InputEvent.PRESS
            };

            // Act
            var result = inputEvent.GetEventActionLabel();

            // Assert
            Assert.AreEqual("PRESS", result);
        }

        [TestMethod]
        public void GetEventActionLabel_ButtonReleaseEvent_ReturnsCorrectString()
        {
            // Arrange
            var inputEvent = new InputEventArgs
            {
                Type = DeviceType.Button,
                Value = (int)MobiFlightButton.InputEvent.RELEASE
            };

            // Act
            var result = inputEvent.GetEventActionLabel();

            // Assert
            Assert.AreEqual("RELEASE", result);
        }

        [TestMethod]
        public void GetEventActionLabel_ButtonLongReleaseEvent_ReturnsCorrectString()
        {
            // Arrange
            var inputEvent = new InputEventArgs
            {
                Type = DeviceType.Button,
                Value = (int)MobiFlightButton.InputEvent.LONG_RELEASE
            };

            // Act
            var result = inputEvent.GetEventActionLabel();

            // Assert
            Assert.AreEqual("LONG_RELEASE", result);
        }

        [TestMethod]
        public void GetEventActionLabel_ButtonInvalidEvent_ReturnsNA()
        {
            // Arrange
            var inputEvent = new InputEventArgs
            {
                Type = DeviceType.Button,
                Value = 999 // Invalid event
            };

            // Act
            var result = inputEvent.GetEventActionLabel();

            // Assert
            Assert.AreEqual("n/a", result);
        }

        [TestMethod]
        public void GetEventActionLabel_EncoderLeftEvent_ReturnsCorrectString()
        {
            // Arrange
            var inputEvent = new InputEventArgs
            {
                Type = DeviceType.Encoder,
                Value = (int)MobiFlightEncoder.InputEvent.LEFT
            };

            // Act
            var result = inputEvent.GetEventActionLabel();

            // Assert
            Assert.AreEqual("LEFT", result);
        }

        [TestMethod]
        public void GetEventActionLabel_EncoderRightEvent_ReturnsCorrectString()
        {
            // Arrange
            var inputEvent = new InputEventArgs
            {
                Type = DeviceType.Encoder,
                Value = (int)MobiFlightEncoder.InputEvent.RIGHT
            };

            // Act
            var result = inputEvent.GetEventActionLabel();

            // Assert
            Assert.AreEqual("RIGHT", result);
        }

        [TestMethod]
        public void GetEventActionLabel_EncoderLeftFastEvent_ReturnsCorrectString()
        {
            // Arrange
            var inputEvent = new InputEventArgs
            {
                Type = DeviceType.Encoder,
                Value = (int)MobiFlightEncoder.InputEvent.LEFT_FAST
            };

            // Act
            var result = inputEvent.GetEventActionLabel();

            // Assert
            Assert.AreEqual("LEFT_FAST", result);
        }

        [TestMethod]
        public void GetEventActionLabel_EncoderRightFastEvent_ReturnsCorrectString()
        {
            // Arrange
            var inputEvent = new InputEventArgs
            {
                Type = DeviceType.Encoder,
                Value = (int)MobiFlightEncoder.InputEvent.RIGHT_FAST
            };

            // Act
            var result = inputEvent.GetEventActionLabel();

            // Assert
            Assert.AreEqual("RIGHT_FAST", result);
        }

        [TestMethod]
        public void GetEventActionLabel_EncoderInvalidEvent_ReturnsNA()
        {
            // Arrange
            var inputEvent = new InputEventArgs
            {
                Type = DeviceType.Encoder,
                Value = 999 // Invalid event
            };

            // Act
            var result = inputEvent.GetEventActionLabel();

            // Assert
            Assert.AreEqual("n/a", result);
        }

        [TestMethod]
        public void GetEventActionLabel_InputShiftRegisterPressEvent_ReturnsCorrectString()
        {
            // Arrange
            var inputEvent = new InputEventArgs
            {
                Type = DeviceType.InputShiftRegister,
                Value = (int)MobiFlightInputShiftRegister.InputEvent.PRESS
            };

            // Act
            var result = inputEvent.GetEventActionLabel();

            // Assert
            Assert.AreEqual("PRESS", result);
        }

        [TestMethod]
        public void GetEventActionLabel_InputShiftRegisterReleaseEvent_ReturnsCorrectString()
        {
            // Arrange
            var inputEvent = new InputEventArgs
            {
                Type = DeviceType.InputShiftRegister,
                Value = (int)MobiFlightInputShiftRegister.InputEvent.RELEASE
            };

            // Act
            var result = inputEvent.GetEventActionLabel();

            // Assert
            Assert.AreEqual("RELEASE", result);
        }

        [TestMethod]
        public void GetEventActionLabel_InputShiftRegisterRepeatEvent_ReturnsCorrectString()
        {
            // Arrange
            var inputEvent = new InputEventArgs
            {
                Type = DeviceType.InputShiftRegister,
                Value = (int)MobiFlightInputShiftRegister.InputEvent.REPEAT
            };

            // Act
            var result = inputEvent.GetEventActionLabel();

            // Assert
            Assert.AreEqual("REPEAT", result);
        }

        [TestMethod]
        public void GetEventActionLabel_InputShiftRegisterInvalidEvent_ReturnsNA()
        {
            // Arrange
            var inputEvent = new InputEventArgs
            {
                Type = DeviceType.InputShiftRegister,
                Value = 999 // Invalid event
            };

            // Act
            var result = inputEvent.GetEventActionLabel();

            // Assert
            Assert.AreEqual("n/a", result);
        }

        [TestMethod]
        public void GetEventActionLabel_InputMultiplexerPressEvent_ReturnsCorrectString()
        {
            // Arrange
            var inputEvent = new InputEventArgs
            {
                Type = DeviceType.InputMultiplexer,
                Value = (int)MobiFlightButton.InputEvent.PRESS // InputMultiplexer uses Button events
            };

            // Act
            var result = inputEvent.GetEventActionLabel();

            // Assert
            Assert.AreEqual("PRESS", result);
        }

        [TestMethod]
        public void GetEventActionLabel_InputMultiplexerReleaseEvent_ReturnsCorrectString()
        {
            // Arrange
            var inputEvent = new InputEventArgs
            {
                Type = DeviceType.InputMultiplexer,
                Value = (int)MobiFlightButton.InputEvent.RELEASE // InputMultiplexer uses Button events
            };

            // Act
            var result = inputEvent.GetEventActionLabel();

            // Assert
            Assert.AreEqual("RELEASE", result);
        }

        [TestMethod]
        public void GetEventActionLabel_InputMultiplexerInvalidEvent_ReturnsNA()
        {
            // Arrange
            var inputEvent = new InputEventArgs
            {
                Type = DeviceType.InputMultiplexer,
                Value = 999 // Invalid event
            };

            // Act
            var result = inputEvent.GetEventActionLabel();

            // Assert
            Assert.AreEqual("n/a", result);
        }

        [TestMethod]
        public void GetEventActionLabel_AnalogInputEvent_ReturnsCorrectString()
        {
            // Arrange
            var inputEvent = new InputEventArgs
            {
                Type = DeviceType.AnalogInput,
                Value = 512 // Analog value
            };

            // Act
            var result = inputEvent.GetEventActionLabel();

            // Assert
            Assert.AreEqual("CHANGE => 512", result);
        }

        [TestMethod]
        public void GetEventActionLabel_AnalogInputZeroValue_ReturnsCorrectString()
        {
            // Arrange
            var inputEvent = new InputEventArgs
            {
                Type = DeviceType.AnalogInput,
                Value = 0
            };

            // Act
            var result = inputEvent.GetEventActionLabel();

            // Assert
            Assert.AreEqual("CHANGE => 0", result);
        }

        [TestMethod]
        public void GetEventActionLabel_AnalogInputMaxValue_ReturnsCorrectString()
        {
            // Arrange
            var inputEvent = new InputEventArgs
            {
                Type = DeviceType.AnalogInput,
                Value = 1023
            };

            // Act
            var result = inputEvent.GetEventActionLabel();

            // Assert
            Assert.AreEqual("CHANGE => 1023", result);
        }

        [TestMethod]
        public void GetEventActionLabel_UnsupportedDeviceType_ReturnsNA()
        {
            // Arrange
            var inputEvent = new InputEventArgs
            {
                Type = DeviceType.Output, // Not an input device
                Value = 1
            };

            // Act
            var result = inputEvent.GetEventActionLabel();

            // Assert
            Assert.AreEqual("n/a", result);
        }

        [TestMethod]
        public void GetEventActionLabel_NotSetDeviceType_ReturnsNA()
        {
            // Arrange
            var inputEvent = new InputEventArgs
            {
                Type = DeviceType.NotSet,
                Value = 1
            };

            // Act
            var result = inputEvent.GetEventActionLabel();

            // Assert
            Assert.AreEqual("n/a", result);
        }

        [TestMethod]
        public void GetMsgEventLabel_BasicEvent_ReturnsCorrectFormat()
        {
            // Arrange
            var inputEvent = new InputEventArgs
            {
                Name = "TestModule",
                DeviceLabel = "Button1",
                Type = DeviceType.Button,
                Value = (int)MobiFlightButton.InputEvent.PRESS
            };

            // Act
            var result = inputEvent.GetMsgEventLabel();

            // Assert
            Assert.AreEqual("TestModule => Button1 => PRESS", result);
        }

        [TestMethod]
        public void GetMsgEventLabel_EventWithExtPin_ReturnsCorrectFormat()
        {
            // Arrange
            var inputEvent = new InputEventArgs
            {
                Name = "TestModule",
                DeviceLabel = "InputShiftReg1",
                Type = DeviceType.InputShiftRegister,
                Value = (int)MobiFlightInputShiftRegister.InputEvent.PRESS,
                ExtPin = 5
            };

            // Act
            var result = inputEvent.GetMsgEventLabel();

            // Assert
            Assert.AreEqual("TestModule => InputShiftReg1:5 => PRESS", result);
        }

        [TestMethod]
        public void GetMsgEventLabel_EventWithoutExtPin_ReturnsCorrectFormat()
        {
            // Arrange
            var inputEvent = new InputEventArgs
            {
                Name = "EncoderModule",
                DeviceLabel = "Rotary1",
                Type = DeviceType.Encoder,
                Value = (int)MobiFlightEncoder.InputEvent.LEFT,
                ExtPin = null
            };

            // Act
            var result = inputEvent.GetMsgEventLabel();

            // Assert
            Assert.AreEqual("EncoderModule => Rotary1 => LEFT", result);
        }

        [TestMethod]
        public void GetMsgEventLabel_InputMultiplexerWithExtPin_ReturnsCorrectFormat()
        {
            // Arrange
            var inputEvent = new InputEventArgs
            {
                Name = "MuxModule",
                DeviceLabel = "Multiplexer1",
                Type = DeviceType.InputMultiplexer,
                Value = (int)MobiFlightButton.InputEvent.PRESS,
                ExtPin = 12
            };

            // Act
            var result = inputEvent.GetMsgEventLabel();

            // Assert
            Assert.AreEqual("MuxModule => Multiplexer1:12 => PRESS", result);
        }

        [TestMethod]
        public void GetMsgEventLabel_AnalogInputWithValue_ReturnsCorrectFormat()
        {
            // Arrange
            var inputEvent = new InputEventArgs
            {
                Name = "AnalogModule",
                DeviceLabel = "Potentiometer1",
                Type = DeviceType.AnalogInput,
                Value = 768
            };

            // Act
            var result = inputEvent.GetMsgEventLabel();

            // Assert
            Assert.AreEqual("AnalogModule => Potentiometer1 => CHANGE => 768", result);
        }

        [TestMethod]
        public void GetMsgEventLabel_NullOrEmptyValues_HandlesGracefully()
        {
            // Arrange
            var inputEvent = new InputEventArgs
            {
                Name = null,
                DeviceLabel = "",
                Type = DeviceType.Button,
                Value = (int)MobiFlightButton.InputEvent.PRESS
            };

            // Act
            var result = inputEvent.GetMsgEventLabel();

            // Assert
            Assert.AreEqual(" =>  => PRESS", result);
        }

        [TestMethod]
        public void GetMsgEventLabel_ExtPinZero_IncludesExtPin()
        {
            // Arrange
            var inputEvent = new InputEventArgs
            {
                Name = "TestModule",
                DeviceLabel = "ShiftReg1",
                Type = DeviceType.InputShiftRegister,
                Value = (int)MobiFlightInputShiftRegister.InputEvent.PRESS,
                ExtPin = 0 // Zero is a valid pin number
            };

            // Act
            var result = inputEvent.GetMsgEventLabel();

            // Assert
            Assert.AreEqual("TestModule => ShiftReg1:0 => PRESS", result);
        }
    }
}