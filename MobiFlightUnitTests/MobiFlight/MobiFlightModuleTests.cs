using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }

        [TestMethod()]
        [Ignore]
        public void GenerateUniqueDeviceNameTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void IsValidDeviceNameTest()
        {
            Assert.Fail();
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
            BoardDefinitions.Load();

            var board = BoardDefinitions.GetBoardByMobiFlightType("MobiFlight Mega");
            MobiFlightModule o = new MobiFlightModule("COM1", board);
            o.Config = new Config.Config();

            Assert.AreEqual(board.Pins.Count(), o.GetFreePins().Count, "Number of free pins is wrong");
            o.Config.Items.Add(new Config.Button() { Name = "Test", Pin = "2" });
            o.Config.Items.Add(new Config.Button() { Name = "Test", Pin = "5" });

            Assert.AreEqual(board.Pins.Count() - o.Config.Items.Count, o.GetFreePins().Count, "Number of free pins is wrong");
            Assert.AreEqual(false, o.GetFreePins().Exists(x=>x.Pin==2), "Used pin still available");
            Assert.AreEqual(false, o.GetFreePins().Exists(x => x.Pin == 5), "Used pin still available");
            Assert.AreEqual(true, o.GetFreePins().Exists(x => x.Pin == 52), "Free pin not available");

            (o.Config.Items[0] as Config.Button).Pin = "3";
            Assert.AreEqual(false, o.GetFreePins().Exists(x => x.Pin == 3), "Used pin still available");
            Assert.AreEqual(true, o.GetFreePins().Exists(x => x.Pin == 2), "Free pin not available");

            board = BoardDefinitions.GetBoardByMobiFlightType("MobiFlight Uno");
            o = new MobiFlightModule("COM1", board);
            o.Config = new Config.Config();
            Assert.AreEqual(true, o.GetFreePins().Exists(x => x.Pin == 13), "Free pin not available");
            Assert.AreEqual(false, o.GetFreePins().Exists(x => x.Pin == 52), "Invalid pin available");
        }
    }
}