using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Firmware;
using System.Collections.Generic;

namespace MobiFlight.Tests
{
    [TestClass]
    public class DeviceReferenceFactoryTests
    {
        #region Output

        [TestMethod]
        public void Create_ShouldSetPwmTrue_WhenOutputPinSupportsPwm()
        {
            // Arrange
            var board = new Board { Pins = [new MobiFlightPin { Pin = 3, isPWM = true }] };
            var output = new Output { Name = "Led", Pin = "3" };

            // Act
            var result = DeviceReferenceFactory.Create(output, board);

            // Assert
            Assert.HasCount(1, result);
            Assert.IsTrue(result[0].Capabilities.Pwm);
        }

        [TestMethod]
        public void Create_ShouldSetPwmFalse_WhenOutputPinDoesNotSupportPwm()
        {
            // Arrange
            var board = new Board { Pins = [new MobiFlightPin { Pin = 4, isPWM = false }] };
            var output = new Output { Name = "Led", Pin = "4" };

            // Act
            var result = DeviceReferenceFactory.Create(output, board);

            // Assert
            Assert.IsFalse(result[0].Capabilities.Pwm);
        }

        [TestMethod]
        public void Create_ShouldSetPwmFalse_WhenBoardIsNull()
        {
            // Arrange
            var output = new Output { Name = "Led", Pin = "3" };

            // Act
            var result = DeviceReferenceFactory.Create(output);

            // Assert
            Assert.IsFalse(result[0].Capabilities.Pwm);
        }

        #endregion

        #region LcdDisplay

        [TestMethod]
        public void Create_ShouldSetRowsAndColumns_WhenDeviceIsLcdDisplay()
        {
            // Arrange
            var lcd = new LcdDisplay { Name = "Lcd", Cols = 20, Lines = 4 };

            // Act
            var result = DeviceReferenceFactory.Create(lcd);

            // Assert
            Assert.HasCount(1, result);
            Assert.AreEqual(20, result[0].Capabilities.Columns);
            Assert.AreEqual(4, result[0].Capabilities.Rows);
        }

        #endregion

        #region LedModule

        [TestMethod]
        public void Create_ShouldCreateOneDevicePerModule_WhenDeviceIsLedModule()
        {
            // Arrange
            var led = new LedModule { Name = "Display", NumModules = "3" };

            // Act
            var result = DeviceReferenceFactory.Create(led);

            // Assert
            Assert.HasCount(3, result);
            Assert.AreEqual("Display:1", result[0].Name);
            Assert.AreEqual("Display:3", result[2].Name);
            Assert.AreEqual(DeviceType.LedModule, result[0].Type);
        }

        [TestMethod]
        [DataRow(LedModule.MODEL_TYPE_MAX72xx, 8)]
        [DataRow(LedModule.MODEL_TYPE_TM1637_4DIGIT, 4)]
        [DataRow(LedModule.MODEL_TYPE_TM1637_6DIGIT, 6)]
        public void Create_ShouldSetDigitsByModelType_WhenDeviceIsLedModule(string modelType, int expectedDigits)
        {
            // Arrange
            var led = new LedModule { Name = "Display", ModelType = modelType };

            // Act
            var result = DeviceReferenceFactory.Create(led);

            // Assert
            Assert.AreEqual(expectedDigits, result[0].Capabilities.Digits);
        }

        #endregion

        #region ShiftRegister

        [TestMethod]
        public void Create_ShouldCreateEightOutputsPerModuleWithoutPwm_WhenDeviceIsShiftRegister()
        {
            // Arrange
            var sr = new ShiftRegister { Name = "SR", NumModules = "2" };

            // Act
            var result = DeviceReferenceFactory.Create(sr);

            // Assert
            Assert.HasCount(16, result);
            Assert.AreEqual(DeviceType.Output, result[0].Type);
            Assert.IsFalse(result[0].Capabilities.Pwm);
        }

        #endregion

        #region Stepper

        [TestMethod]
        [DataRow("0", false)]
        [DataRow("5", true)]
        public void Create_ShouldSetAutoZero_WhenStepperHasHomeButton(string btnPin, bool expectedAutoZero)
        {
            // Arrange
            var stepper = new Stepper { Name = "Stepper", BtnPin = btnPin };

            // Act
            var result = DeviceReferenceFactory.Create(stepper);

            // Assert
            Assert.AreEqual(expectedAutoZero, result[0].Capabilities.AutoZero);
        }

        [TestMethod]
        public void Create_ShouldSetProfile_WhenStepperProfileIsKnown()
        {
            // Arrange
            var stepper = new Stepper { Name = "Stepper", Profile = 1 };

            // Act
            var result = DeviceReferenceFactory.Create(stepper);

            // Assert
            Assert.AreEqual(1, result[0].Capabilities.Profile.id);
        }

        #endregion

        #region CustomDevice

        [TestMethod]
        public void Create_ShouldHaveNoMessageTypes_WhenCustomDeviceTypeIsUnknown()
        {
            // Arrange
            var custom = new Firmware.CustomDevice { Name = "Custom", CustomType = "DoesNotExist" };

            // Act
            var result = DeviceReferenceFactory.Create(custom);

            // Assert
            Assert.HasCount(1, result);
            Assert.IsNull(result[0].Capabilities.MessageTypes);
        }

        #endregion

        #region Devices without capabilities

        [TestMethod]
        [DataRow(DeviceType.Button)]
        [DataRow(DeviceType.Servo)]
        public void Create_ShouldHaveNoCapabilities_WhenDeviceHasNone(DeviceType type)
        {
            // Arrange
            BaseDevice device = type == DeviceType.Button ? new Button { Name = "Dev" } : new Servo { Name = "Dev" };

            // Act
            var result = DeviceReferenceFactory.Create(device);

            // Assert
            Assert.HasCount(1, result);
            Assert.IsNull(result[0].Capabilities);
        }

        #endregion
    }
}