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
        [DataRow("2", 16)]
        [DataRow("", 8)]
        public void Create_ShouldCreateOneDeviceWithEightPinsPerModule_WhenDeviceIsShiftRegister(string numModules, int expectedPins)
        {
            // Arrange
            var sr = new ShiftRegister { Name = "SR", NumModules = numModules };

            // Act
            var result = DeviceReferenceFactory.Create(sr);

            // Assert
            Assert.HasCount(1, result);
            Assert.AreEqual("SR", result[0].Name);
            Assert.AreEqual(DeviceType.ShiftRegister, result[0].Type);
            Assert.AreEqual(expectedPins, result[0].Capabilities.Pins);
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
    

        #region Arcaze

        [TestMethod]
        public void CreateForArcaze_ShouldCreateBaseBoardOutputsOnly_WhenNoExtensionIsConfigured()
        {
            // Act
            var result = DeviceReferenceFactory.CreateForArcaze(DeviceReferenceFactory.ArcazeExtension.None, 1);

            // Assert
            Assert.HasCount(40, result);
            Assert.IsTrue(result.TrueForAll(device => device.Type == DeviceType.Output));
            Assert.AreEqual("A01", result[0].Name);
            Assert.AreEqual("A20", result[19].Name);
            Assert.AreEqual("B01", result[20].Name);
            Assert.AreEqual("B20", result[39].Name);
        }

        [TestMethod]
        [DataRow(1, 88, "E08")]
        [DataRow(2, 136, "H08")]
        [DataRow(4, 232, "N08")]
        public void CreateForArcaze_ShouldCreateOutputsOfConfiguredLedDriverModules(int numModules, int expectedCount, string expectedLastPin)
        {
            // Act
            var result = DeviceReferenceFactory.CreateForArcaze(DeviceReferenceFactory.ArcazeExtension.LedDriver, numModules);

            // Assert
            Assert.HasCount(expectedCount, result);
            Assert.IsTrue(result.TrueForAll(device => device.Type == DeviceType.Output));
            Assert.AreEqual(expectedLastPin, result[^1].Name);
        }

        [TestMethod]
        public void CreateForArcaze_ShouldCreatePortsWith20And8Pins_WhenLedDriverIsConfigured()
        {
            // Act
            var result = DeviceReferenceFactory.CreateForArcaze(DeviceReferenceFactory.ArcazeExtension.LedDriver, 1);
            var names = result.ConvertAll(device => device.Name);

            // Assert
            CollectionAssert.Contains(names, "C20");
            CollectionAssert.Contains(names, "D20");
            CollectionAssert.Contains(names, "E08");
            CollectionAssert.DoesNotContain(names, "E09");
            CollectionAssert.DoesNotContain(names, "F01");
        }

        [TestMethod]
        public void CreateForArcaze_ShouldCreateDisplaysForEachAddressAndConnector_WhenDisplayDriverIsConfigured()
        {
            // Act
            var result = DeviceReferenceFactory.CreateForArcaze(DeviceReferenceFactory.ArcazeExtension.DisplayDriver, 2);
            var displays = result.FindAll(device => device.Type == DeviceType.LedModule);

            // Assert
            Assert.HasCount(44, result);
            CollectionAssert.AreEqual(
                new List<string> { "0:1", "0:2", "1:1", "1:2" },
                displays.ConvertAll(display => display.Name));
            Assert.IsTrue(displays.TrueForAll(display => display.Capabilities.Digits == 8));
        }

        [TestMethod]
        public void CreateForArcaze_ShouldNotCreateDisplays_WhenLedDriverIsConfigured()
        {
            // Act
            var result = DeviceReferenceFactory.CreateForArcaze(DeviceReferenceFactory.ArcazeExtension.LedDriver, 1);

            // Assert
            Assert.IsFalse(result.Exists(device => device.Type == DeviceType.LedModule));
        }

        [TestMethod]
        public void CreateForArcaze_ShouldNotSupportPwm()
        {
            // Act
            var result = DeviceReferenceFactory.CreateForArcaze(DeviceReferenceFactory.ArcazeExtension.None, 1);

            // Assert
            Assert.IsTrue(result.TrueForAll(device => device.Capabilities.Pwm == false));
        }

        #endregion
    }
}