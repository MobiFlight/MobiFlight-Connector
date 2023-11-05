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
    public class MobiFlightModuleInfoTests
    {
        [TestMethod()]
        public void HasMfFirmwareTest()
        {
            var info = new MobiFlightModuleInfo() { 
            };
            Assert.IsFalse(info.HasMfFirmware());

            info.Version = "1.0.0";
            info.Serial = "SN-123-456";

            Assert.IsTrue(info.HasMfFirmware());
        }

        [TestMethod()]
        public void FirmwareInstallPossibleTest()
        {
            BoardDefinitions.Load();
            var board = BoardDefinitions.GetBoardByMobiFlightType("MobiFlight Mega");
            var info = new MobiFlightModuleInfo();

            Assert.IsFalse(info.FirmwareInstallPossible());

            info.Board = board;
            Assert.IsTrue(info.FirmwareInstallPossible());
        }
    }
}