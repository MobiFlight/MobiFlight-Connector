using CommandMessenger;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MobiFlight.Tests
{
    [TestClass()]
    public class MobiFlightModuleTests
    {
        [TestMethod()]
        [Ignore]
        public void MobiFlightModuleTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void UpdateConfigTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void ConnectTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void ResetBoardTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void LoadConfigTest()
        {
            Assert.Fail();
            /*
            MobiFlight.Config.Config config = new Config.Config();
            MobiFlightModule module = new MobiFlightModule("COM1", BoardDefinitions.GetBoardByMobiFlightType("asrduino_mega"));
            module.Config = config;
            config.Items.Add(new MobiFlightOutput() { })
            module.LoadConfig();

            // do the check for two devices with the same name
            */
        }

        [TestMethod()]
        public void GenerateUniqueDeviceNameTest()
        {
            List<String> UsedKeys = new List<String>()
            {
            };

            // Test with no Used Keys
            Assert.AreEqual("TestDevice", MobiFlightModule.GenerateUniqueDeviceName(UsedKeys.ToArray(), "TestDevice"), "Name is not as expected.");

            UsedKeys.Add("TestDevice");
            Assert.AreEqual("TestDevice 1", MobiFlightModule.GenerateUniqueDeviceName(UsedKeys.ToArray(), "TestDevice"), "Name is not as expected.");

            UsedKeys.Add("TestDevice 2");
            Assert.AreEqual("TestDevice 1", MobiFlightModule.GenerateUniqueDeviceName(UsedKeys.ToArray(), "TestDevice"), "Name is not as expected.");

            UsedKeys.Add("TestDevice 1");
            Assert.AreEqual("TestDevice 3", MobiFlightModule.GenerateUniqueDeviceName(UsedKeys.ToArray(), "TestDevice"), "Name is not as expected.");

            Assert.AreEqual("TestDevice 1 1", MobiFlightModule.GenerateUniqueDeviceName(UsedKeys.ToArray(), "TestDevice 1"), "Name is not as expected.");
        }

        [TestMethod()]
        public void IsValidDeviceNameTest()
        {
            // Arrange & Act & Assert - Valid names
            Assert.IsTrue(MobiFlightModule.IsValidDeviceName("TestDevice"), "Valid device name should return true.");
            Assert.IsTrue(MobiFlightModule.IsValidDeviceName("Device123"), "Valid device name with numbers should return true.");
            Assert.IsTrue(MobiFlightModule.IsValidDeviceName("a"), "Single character device name should return true.");
            Assert.IsTrue(MobiFlightModule.IsValidDeviceName("1234567890123456"), "16-character device name should return true.");

            // Arrange & Act & Assert - Invalid characters
            Assert.IsFalse(MobiFlightModule.IsValidDeviceName("a/"), "Device name with '/' should return false.");
            Assert.IsFalse(MobiFlightModule.IsValidDeviceName("Test:Device"), "Device name with ':' should return false.");
            Assert.IsFalse(MobiFlightModule.IsValidDeviceName("Test.Device"), "Device name with '.' should return false.");
            Assert.IsFalse(MobiFlightModule.IsValidDeviceName("Test;Device"), "Device name with ';' should return false.");
            Assert.IsFalse(MobiFlightModule.IsValidDeviceName("Test,Device"), "Device name with ',' should return false.");
            Assert.IsFalse(MobiFlightModule.IsValidDeviceName("Test#Device"), "Device name with '#' should return false.");
            Assert.IsFalse(MobiFlightModule.IsValidDeviceName("Test|Device"), "Device name with '|' should return false.");

            // Arrange & Act & Assert - Too long
            Assert.IsFalse(MobiFlightModule.IsValidDeviceName("12345678901234567"), "Device name longer than 16 characters should return false.");
            Assert.IsFalse(MobiFlightModule.IsValidDeviceName("VeryLongDeviceName"), "Device name longer than 16 characters should return false.");
        }

        [TestMethod()]
        [Ignore]
        public void DisconnectTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void InitUploadAndReturnUploadPortTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void SetPinTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void SetDisplayTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void SetServoTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void SetStepperTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void ResetStepperTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void RetriggerTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void GetInfoTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void SaveNameTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void SaveConfigTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void GetConnectedDevicesTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void GetConnectedOutputDeviceTypesTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void GetConnectedInputDeviceTypesTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void GetConnectedInputDevicesTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void GenerateNewSerialTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void HasFirmwareFeatureTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void StopTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetFreePinsTest()
        {
            BoardDefinitions.LoadDefinitions();

            var board = BoardDefinitions.GetBoardByMobiFlightType("MobiFlight Mega");
            MobiFlightModule o = new MobiFlightModule("COM1", board);
            o.Config = new Firmware.Config();

            Assert.HasCount(board.Pins.Count(), o.GetFreePins(), "Number of free pins is wrong");
            o.Config.Items.Add(new Firmware.Button() { Name = "Test", Pin = "2" });
            o.Config.Items.Add(new Firmware.Button() { Name = "Test", Pin = "5" });

            Assert.HasCount(board.Pins.Count() - o.Config.Items.Count, o.GetFreePins(), "Number of free pins is wrong");
            Assert.IsFalse(o.GetFreePins().Exists(x => x.Pin == 2), "Used pin still available");
            Assert.IsFalse(o.GetFreePins().Exists(x => x.Pin == 5), "Used pin still available");
            Assert.IsTrue(o.GetFreePins().Exists(x => x.Pin == 52), "Free pin not available");

            (o.Config.Items[0] as Firmware.Button).Pin = "3";
            Assert.IsFalse(o.GetFreePins().Exists(x => x.Pin == 3), "Used pin still available");
            Assert.IsTrue(o.GetFreePins().Exists(x => x.Pin == 2), "Free pin not available");

            board = BoardDefinitions.GetBoardByMobiFlightType("MobiFlight Uno");
            o = new MobiFlightModule("COM1", board);
            o.Config = new Firmware.Config();
            Assert.IsTrue(o.GetFreePins().Exists(x => x.Pin == 13), "Free pin not available");
            Assert.IsFalse(o.GetFreePins().Exists(x => x.Pin == 52), "Invalid pin available");
        }

        [TestMethod()]
        public void MobiFlightModuleType()
        {
            BoardDefinitions.LoadDefinitions();
            var board = BoardDefinitions.GetBoardByMobiFlightType("MobiFlight Mega");

            MobiFlightModule o = new MobiFlightModule("COM1", board);

            // Default use case
            // Information based on board description
            // Arduino type returned
            Assert.AreEqual("Arduino Mega 2560", o.Type, "Wrong module type");

            // Setting state manually like if GetInfo() was called.
            o.Version = "1.0.0";
            o.Serial = "SN-123-123";
            // MobiFlight type returned
            Assert.AreEqual("MobiFlight Mega", o.Type, "Wrong module type");

            var portDetails = new PortDetails()
            {
                Board = board,
                HardwareId = "VID_1A86&PID_7523&REV_0264",
                Name = "COM1"
            };

            // Type if ambiguous matches
            var moduleInfo = new MobiFlightModuleInfo()
            {
                Port = portDetails.Name,
                Type = MobiFlightModule.TYPE_UNKNOWN,
                Name = MobiFlightModule.TYPE_UNKNOWN,
                Board = portDetails.Board,
                HardwareId = portDetails.HardwareId
            };

            o = new MobiFlightModule(moduleInfo);
            Assert.AreEqual(MobiFlightModule.TYPE_COMPATIBLE, o.Type, "Wrong module type");

            // Setting state manually like if GetInfo() was called.
            o.Version = "1.0.0";
            o.Serial = "SN-123-123";
            // MobiFlight type returned
            Assert.AreEqual("MobiFlight Mega", o.Type, "Wrong module type");
        }

        [TestMethod()]
        [Ignore]
        public void SetDisplayBrightnessTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void SetLcdDisplayTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void GetConnectedDevicesStatisticsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void getPwmPinsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void GetConnectedOutputDevicesTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void GetPinsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void FirmwareRequiresUpdateTest()
        {
            BoardDefinitions.LoadDefinitions();
            var board = BoardDefinitions.GetBoardByMobiFlightType("MobiFlight Mega");
            Assert.IsNotNull(board, "Board not found");

            var o = new MobiFlightModule("COM1", board);
            o.Version = "1.0.0";
            Assert.IsTrue(o.FirmwareRequiresUpdate(), "Firmware version requires update.");

            o.Version = "999.0.0";
            Assert.IsFalse(o.FirmwareRequiresUpdate(), "Firmware version does NOT require update.");

            // special case
            // Dev Build
            o.Version = "0.0.1";
            Assert.IsFalse(o.FirmwareRequiresUpdate(), "Firmware version does NOT require update. Dev Build 0.0.1");
        }

        [TestMethod()]
        public void Encoder_RaisesCorrectEncoderInputEvent()
        {
            BoardDefinitions.LoadDefinitions();

            var board = BoardDefinitions.GetBoardByMobiFlightType("MobiFlight Mega");

            var module = new MobiFlightModule("COM1", board)
            {
                Serial = "SN-123-123",
                Name = "TestBoard",
                // These two fields make it a MobiFlight Board
                // It will report the MobiFlight Type instead of the Arduino Type
                CoreVersion = "1.0.0",
                Version = "1.0.0"
            };

            InputEventArgs capturedArgs = null;
            module.OnInputDeviceAction += (sender, e) => capturedArgs = e;

            var command = new ReceivedCommand(new string[] {
                "6", // EncoderChange
                "Encoder",
                "0"
            });

            var methodInfo = typeof(MobiFlightModule).GetMethod("OnEncoderChange", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(methodInfo, "Expected private method OnEncoderChange to exist.");
            methodInfo.Invoke(module, new object[] { command });

            // Assert
            Assert.IsNotNull(capturedArgs, "OnInputDeviceAction was not raised.");
            Assert.AreEqual(DeviceType.Encoder, capturedArgs.InputType, "Encoder should be reported as Encoder type");
            Assert.AreEqual(DeviceType.Encoder, capturedArgs.Device.Type, "Encoder should be reported as Encoder events");
            Assert.AreEqual("Encoder", capturedArgs.Device.Name, "Encoder have name");
            Assert.AreEqual(0, capturedArgs.Value, "Encoder should report 0 (ON LEFT)");
            Assert.AreEqual("SN-123-123", capturedArgs.Controller.Serial, "Controller serial should match");
            Assert.AreEqual("TestBoard", capturedArgs.Controller.Name, "Controller name should match");
        }

        [TestMethod()]
        public void InputMultiplexerChange_RaisesCorrectButtonInputEvent()
        {
            BoardDefinitions.LoadDefinitions();

            var board = BoardDefinitions.GetBoardByMobiFlightType("MobiFlight Mega");

            var module = new MobiFlightModule("COM1", board) {
                Serial = "SN-123-123",
                Name = "TestBoard",
                // These two fields make it a MobiFlight Board
                // It will report the MobiFlight Type instead of the Arduino Type
                CoreVersion = "1.0.0",
                Version = "1.0.0"
            };

            InputEventArgs capturedArgs = null;
            module.OnInputDeviceAction += (sender, e) => capturedArgs = e;

            var command = new ReceivedCommand(new string[] {
                "30",
                "InputMultiplexer",
                "2",
                "0"
            });

            var methodInfo = typeof(MobiFlightModule).GetMethod("OnInputMultiplexerChange", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(methodInfo, "Expected private method OnInputMultiplexerChange to exist.");
            methodInfo.Invoke(module, new object[] { command });

            // Assert
            Assert.IsNotNull(capturedArgs, "OnInputDeviceAction was not raised.");
            Assert.AreEqual(DeviceType.Button, capturedArgs.InputType, "Input Multiplexer should be reported as Button type");
            Assert.AreEqual(DeviceType.Button, capturedArgs.Device.Type, "Input Multiplexer should be reported as Button events");
            Assert.AreEqual("InputMultiplexer:2", capturedArgs.Device.Name, "Input Multiplexer have name and sub-pin combined");
            Assert.AreEqual(0, capturedArgs.Value, "Input Multiplexer should report 0 (ON PRESS)");
            Assert.AreEqual("SN-123-123", capturedArgs.Controller.Serial, "Controller serial should match");
            Assert.AreEqual("TestBoard", capturedArgs.Controller.Name, "Controller name should match");
        }

        [TestMethod()]
        public void InputShiftRegisterChange_RaisesCorrectButtonInputEvent()
        {
            BoardDefinitions.LoadDefinitions();

            var board = BoardDefinitions.GetBoardByMobiFlightType("MobiFlight Mega");

            var module = new MobiFlightModule("COM1", board)
            {
                Serial = "SN-123-123",
                Name = "TestBoard",
                // These two fields make it a MobiFlight Board
                // It will report the MobiFlight Type instead of the Arduino Type
                CoreVersion = "1.0.0",
                Version = "1.0.0"
            };

            InputEventArgs capturedArgs = null;
            module.OnInputDeviceAction += (sender, e) => capturedArgs = e;

            var command = new ReceivedCommand(new string[] {
                "29",
                "InputShiftRegister",
                "2",
                "0"
            });

            var methodInfo = typeof(MobiFlightModule).GetMethod("OnInputShiftRegisterChange", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(methodInfo, "Expected private method OnInputShiftRegisterChange to exist.");
            methodInfo.Invoke(module, new object[] { command });

            // Assert
            Assert.IsNotNull(capturedArgs, "OnInputDeviceAction was not raised.");
            Assert.AreEqual(DeviceType.Button, capturedArgs.InputType, "Input Shift Register should be reported as Button type");
            Assert.AreEqual(DeviceType.Button, capturedArgs.Device.Type, "Input Shift Register should be reported as Button events");
            Assert.AreEqual("InputShiftRegister:2", capturedArgs.Device.Name, "Input Shift Register have name and sub-pin combined");
            Assert.AreEqual(0, capturedArgs.Value, "Input Shift Register should report 0 (ON PRESS)");
            Assert.AreEqual("SN-123-123", capturedArgs.Controller.Serial, "Controller serial should match");
            Assert.AreEqual("TestBoard", capturedArgs.Controller.Name, "Controller name should match");
        }

        [TestMethod()]
        public void Button_RaisesCorrectButtonInputEvent()
        {
            BoardDefinitions.LoadDefinitions();

            var board = BoardDefinitions.GetBoardByMobiFlightType("MobiFlight Mega");

            var module = new MobiFlightModule("COM1", board)
            {
                Serial = "SN-123-123",
                Name = "TestBoard",
                // These two fields make it a MobiFlight Board
                // It will report the MobiFlight Type instead of the Arduino Type
                CoreVersion = "1.0.0",
                Version = "1.0.0"
            };

            InputEventArgs capturedArgs = null;
            module.OnInputDeviceAction += (sender, e) => capturedArgs = e;

            var command = new ReceivedCommand(new string[] {
                "7", // ButtonChange
                "Button",
                "0"
            });

            var methodInfo = typeof(MobiFlightModule).GetMethod("OnButtonChange", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(methodInfo, "Expected private method OnButtonChange to exist.");
            methodInfo.Invoke(module, new object[] { command });

            // Assert
            Assert.IsNotNull(capturedArgs, "OnInputDeviceAction was not raised.");
            Assert.AreEqual(DeviceType.Button, capturedArgs.InputType, "Button should be reported as Button type");
            Assert.AreEqual(DeviceType.Button, capturedArgs.Device.Type, "Button should be reported as Button events");
            Assert.AreEqual("Button", capturedArgs.Device.Name, "Button have name");
            Assert.AreEqual(0, capturedArgs.Value, "Button should report 0 (ON PRESS)");
            Assert.AreEqual("SN-123-123", capturedArgs.Controller.Serial, "Controller serial should match");
            Assert.AreEqual("TestBoard", capturedArgs.Controller.Name, "Controller name should match");
        }

        [TestMethod()]
        public void AnalogInput_RaisesCorrectAnalogInputEvent()
        {
            BoardDefinitions.LoadDefinitions();

            var board = BoardDefinitions.GetBoardByMobiFlightType("MobiFlight Mega");

            var module = new MobiFlightModule("COM1", board)
            {
                Serial = "SN-123-123",
                Name = "TestBoard",
                // These two fields make it a MobiFlight Board
                // It will report the MobiFlight Type instead of the Arduino Type
                CoreVersion = "1.0.0",
                Version = "1.0.0"
            };

            InputEventArgs capturedArgs = null;
            module.OnInputDeviceAction += (sender, e) => capturedArgs = e;

            var command = new ReceivedCommand(new string[] {
                "28", // AnalogChange
                "AnalogInput",
                "768"
            });

            var methodInfo = typeof(MobiFlightModule).GetMethod("OnAnalogChange", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(methodInfo, "Expected private method OnAnalogChange to exist.");
            methodInfo.Invoke(module, new object[] { command });

            // Assert
            Assert.IsNotNull(capturedArgs, "OnInputDeviceAction was not raised.");
            Assert.AreEqual(DeviceType.AnalogInput, capturedArgs.InputType, "AnalogInput should be reported as Analog type");
            Assert.AreEqual(DeviceType.AnalogInput, capturedArgs.Device.Type, "AnalogInput should be reported as Analog events");
            Assert.AreEqual("AnalogInput", capturedArgs.Device.Name, "AnalogInput have name");
            Assert.AreEqual(768, capturedArgs.Value, "AnalogInput should report 768");
            Assert.AreEqual("SN-123-123", capturedArgs.Controller.Serial, "Controller serial should match");
            Assert.AreEqual("TestBoard", capturedArgs.Controller.Name, "Controller name should match");
        }
    }
}