using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

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
            o.Config = new Config.Config();

            Assert.HasCount(board.Pins.Count(), o.GetFreePins(), "Number of free pins is wrong");
            o.Config.Items.Add(new Config.Button() { Name = "Test", Pin = "2" });
            o.Config.Items.Add(new Config.Button() { Name = "Test", Pin = "5" });

            Assert.HasCount(board.Pins.Count() - o.Config.Items.Count, o.GetFreePins(), "Number of free pins is wrong");
            Assert.IsFalse(o.GetFreePins().Exists(x => x.Pin == 2), "Used pin still available");
            Assert.IsFalse(o.GetFreePins().Exists(x => x.Pin == 5), "Used pin still available");
            Assert.IsTrue(o.GetFreePins().Exists(x => x.Pin == 52), "Free pin not available");

            (o.Config.Items[0] as Config.Button).Pin = "3";
            Assert.IsFalse(o.GetFreePins().Exists(x => x.Pin == 3), "Used pin still available");
            Assert.IsTrue(o.GetFreePins().Exists(x => x.Pin == 2), "Free pin not available");

            board = BoardDefinitions.GetBoardByMobiFlightType("MobiFlight Uno");
            o = new MobiFlightModule("COM1", board);
            o.Config = new Config.Config();
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
    }
}