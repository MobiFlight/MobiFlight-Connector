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
            BoardDefinitions.LoadDefinitions();
            var board = BoardDefinitions.GetBoardByMobiFlightType("MobiFlight Mega");
            var info = new MobiFlightModuleInfo();

            Assert.IsFalse(info.FirmwareInstallPossible());

            info.Board = board;
            Assert.IsTrue(info.FirmwareInstallPossible());
        }
     
        [TestMethod()]
        public void FirmwareRequiresUpdate_IgnoresDevAndPrBuilds()
        {
           var info = new MobiFlightModuleInfo
            {
                Board = new Board
                {
                    Info = new Info
                    {
                        LatestFirmwareVersion = "11.1.0"
                    }
                }
            };

            info.Version = "0.0.1";
            Assert.IsFalse(info.FirmwareRequiresUpdate());

            info.Version = "0.0.333";
            Assert.IsFalse(info.FirmwareRequiresUpdate());
        }

        [TestMethod()]
        public void FirmwareRequiresUpdate_TrueForOldVersion()
        {
            var info = new MobiFlightModuleInfo
            {
                Board = new Board
                {
                    Info = new Info
                    {
                        LatestFirmwareVersion = "11.1.0"
                    }
                }
            };

            info.Version = "1.0.0";

            Assert.IsTrue(info.FirmwareRequiresUpdate());
        }
        [TestMethod]
        public void FirmwareRequiresUpdate_FalseForLatestVersion()
        {
            var info = new MobiFlightModuleInfo
            {
                Board = new Board
                {
                    Info = new Info
                    {
                        LatestFirmwareVersion = "11.1.0"
                    }
                },
                Version = "11.1.0"
            };

            Assert.IsFalse(info.FirmwareRequiresUpdate());
        }

        [TestMethod]
        public void FirmwareRequiresUpdate_FalseForNewerVersion()
        {
            var info = new MobiFlightModuleInfo
            {
                Board = new Board
                {
                    Info = new Info
                    {
                        LatestFirmwareVersion = "11.1.0"
                    }
                },
                Version = "12.0.0"
            };

            Assert.IsFalse(info.FirmwareRequiresUpdate());
        }

        [TestMethod]
        public void FirmwareRequiresUpdate_FalseForInvalidVersion()
        {
            var info = new MobiFlightModuleInfo
            {
                Board = new Board
                {
                    Info = new Info
                    {
                        LatestFirmwareVersion = "11.1.0"
                    }
                },
                Version = null
            };

            Assert.IsFalse(info.FirmwareRequiresUpdate());
        }

        [TestMethod]
        public void FirmwareRequiresUpdate_FalseForMissingVersion()
        {
            var info = new MobiFlightModuleInfo
            {
                Board = new Board
                {
                    Info = new Info
                    {
                        LatestFirmwareVersion = "11.1.0"
                    }
                }
            };

            Assert.IsFalse(info.FirmwareRequiresUpdate());
        }

    }
}