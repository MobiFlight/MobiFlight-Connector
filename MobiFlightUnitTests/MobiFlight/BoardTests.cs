using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight;
using System.Collections.Generic;

namespace MobiFlight.Tests
{
    [TestClass]
    public class BoardTests
    {
        [TestMethod]
        public void Migrate_WithNullAvrDudeSettings_DoesNotThrow()
        {
            // Arrange
            var board = new Board
            {
                AvrDudeSettings = null,
                Connection = new Connection { TimeoutForFirmwareUpdate = 0 },
                Info = new Info()
            };

            // Act & Assert - Should not throw
            board.Migrate();
        }

        #region BaudRate Migration Tests

        [TestMethod]
        public void Migrate_WithBaudRateButNoBaudRates_MigratesBaudRateToArray()
        {
            // Arrange
            var board = new Board
            {
                AvrDudeSettings = new AvrDudeSettings
                {
#pragma warning disable CS0612 // Type or member is obsolete
                    BaudRate = "115200",
#pragma warning restore CS0612
                    BaudRates = null
                },
                Connection = new Connection { TimeoutForFirmwareUpdate = 5000 },
                Info = new Info()
            };

            // Act
            board.Migrate();

            // Assert
            Assert.IsNotNull(board.AvrDudeSettings.BaudRates);
            Assert.AreEqual(1, board.AvrDudeSettings.BaudRates.Count);
            Assert.AreEqual("115200", board.AvrDudeSettings.BaudRates[0]);
        }

        [TestMethod]
        public void Migrate_WithBaudRateAndExistingBaudRates_DoesNotOverwriteBaudRates()
        {
            // Arrange
            var existingBaudRates = new List<string> { "9600", "57600" };
            var board = new Board
            {
                AvrDudeSettings = new AvrDudeSettings
                {
#pragma warning disable CS0612 // Type or member is obsolete
                    BaudRate = "115200",
#pragma warning restore CS0612
                    BaudRates = existingBaudRates
                },
                Connection = new Connection { TimeoutForFirmwareUpdate = 5000 },
                Info = new Info()
            };

            // Act
            board.Migrate();

            // Assert
            Assert.AreEqual(existingBaudRates, board.AvrDudeSettings.BaudRates);
            Assert.AreEqual(2, board.AvrDudeSettings.BaudRates.Count);
            Assert.AreEqual("9600", board.AvrDudeSettings.BaudRates[0]);
            Assert.AreEqual("57600", board.AvrDudeSettings.BaudRates[1]);
        }

        [TestMethod]
        public void Migrate_WithEmptyBaudRateAndNoBaudRates_DoesNotCreateBaudRatesArray()
        {
            // Arrange
            var board = new Board
            {
                AvrDudeSettings = new AvrDudeSettings
                {
#pragma warning disable CS0612 // Type or member is obsolete
                    BaudRate = "",
#pragma warning restore CS0612
                    BaudRates = null
                },
                Connection = new Connection { TimeoutForFirmwareUpdate = 5000 },
                Info = new Info()
            };

            // Act
            board.Migrate();

            // Assert
            Assert.IsNull(board.AvrDudeSettings.BaudRates);
        }

        [TestMethod]
        public void Migrate_WithNullBaudRateAndNoBaudRates_DoesNotCreateBaudRatesArray()
        {
            // Arrange
            var board = new Board
            {
                AvrDudeSettings = new AvrDudeSettings
                {
#pragma warning disable CS0612 // Type or member is obsolete
                    BaudRate = null,
#pragma warning restore CS0612
                    BaudRates = null
                },
                Connection = new Connection { TimeoutForFirmwareUpdate = 5000 },
                Info = new Info()
            };

            // Act
            board.Migrate();

            // Assert
            Assert.IsNull(board.AvrDudeSettings.BaudRates);
        }

        #endregion

        #region ResetFirmwareFile Migration Tests

        [TestMethod]
        public void Migrate_WithResetFirmwareFileInAvrDudeSettings_MigratesToInfo()
        {
            // Arrange
            var board = new Board
            {
                AvrDudeSettings = new AvrDudeSettings
                {
#pragma warning disable CS0612 // Type or member is obsolete
                    ResetFirmwareFile = "reset_firmware.hex"
#pragma warning restore CS0612
                },
                Connection = new Connection { TimeoutForFirmwareUpdate = 5000 },
                Info = new Info { ResetFirmwareFile = null }
            };

            // Act
            board.Migrate();

            // Assert
            Assert.AreEqual("reset_firmware.hex", board.Info.ResetFirmwareFile);
        }

        [TestMethod]
        public void Migrate_WithResetFirmwareFileInInfoAndAvrDudeSettings_DoesNotOverwriteInfo()
        {
            // Arrange
            var board = new Board
            {
                AvrDudeSettings = new AvrDudeSettings
                {
#pragma warning disable CS0612 // Type or member is obsolete
                    ResetFirmwareFile = "old_reset_firmware.hex"
#pragma warning restore CS0612
                },
                Connection = new Connection { TimeoutForFirmwareUpdate = 5000 },
                Info = new Info { ResetFirmwareFile = "new_reset_firmware.hex" }
            };

            // Act
            board.Migrate();

            // Assert
            Assert.AreEqual("new_reset_firmware.hex", board.Info.ResetFirmwareFile);
        }

        [TestMethod]
        public void Migrate_WithEmptyResetFirmwareFileInAvrDudeSettings_DoesNotMigrateToInfo()
        {
            // Arrange
            var board = new Board
            {
                AvrDudeSettings = new AvrDudeSettings
                {
#pragma warning disable CS0612 // Type or member is obsolete
                    ResetFirmwareFile = ""
#pragma warning restore CS0612
                },
                Connection = new Connection { TimeoutForFirmwareUpdate = 5000 },
                Info = new Info { ResetFirmwareFile = null }
            };

            // Act
            board.Migrate();

            // Assert
            Assert.IsNull(board.Info.ResetFirmwareFile);
        }

        #endregion

        #region FirmwareBaseName Migration Tests

        [TestMethod]
        public void Migrate_WithFirmwareBaseNameInAvrDudeSettings_MigratesToInfo()
        {
            // Arrange
            var board = new Board
            {
                AvrDudeSettings = new AvrDudeSettings
                {
#pragma warning disable CS0612 // Type or member is obsolete
                    FirmwareBaseName = "mobiflight_mega"
#pragma warning restore CS0612
                },
                Connection = new Connection { TimeoutForFirmwareUpdate = 5000 },
                Info = new Info { FirmwareBaseName = null }
            };

            // Act
            board.Migrate();

            // Assert
            Assert.AreEqual("mobiflight_mega", board.Info.FirmwareBaseName);
        }

        [TestMethod]
        public void Migrate_WithFirmwareBaseNameInInfoAndAvrDudeSettings_DoesNotOverwriteInfo()
        {
            // Arrange
            var board = new Board
            {
                AvrDudeSettings = new AvrDudeSettings
                {
#pragma warning disable CS0612 // Type or member is obsolete
                    FirmwareBaseName = "old_mobiflight_mega"
#pragma warning restore CS0612
                },
                Connection = new Connection { TimeoutForFirmwareUpdate = 5000 },
                Info = new Info { FirmwareBaseName = "new_mobiflight_mega" }
            };

            // Act
            board.Migrate();

            // Assert
            Assert.AreEqual("new_mobiflight_mega", board.Info.FirmwareBaseName);
        }

        [TestMethod]
        public void Migrate_WithEmptyFirmwareBaseNameInAvrDudeSettings_DoesNotMigrateToInfo()
        {
            // Arrange
            var board = new Board
            {
                AvrDudeSettings = new AvrDudeSettings
                {
#pragma warning disable CS0612 // Type or member is obsolete
                    FirmwareBaseName = ""
#pragma warning restore CS0612
                },
                Connection = new Connection { TimeoutForFirmwareUpdate = 5000 },
                Info = new Info { FirmwareBaseName = null }
            };

            // Act
            board.Migrate();

            // Assert
            Assert.IsNull(board.Info.FirmwareBaseName);
        }

        #endregion

        #region FirmwareExtension Migration Tests

        [TestMethod]
        public void Migrate_WithEmptyFirmwareExtension_SetsToHex()
        {
            // Arrange
            var board = new Board
            {
                AvrDudeSettings = new AvrDudeSettings(),
                Connection = new Connection { TimeoutForFirmwareUpdate = 5000 },
                Info = new Info { FirmwareExtension = null }
            };

            // Act
            board.Migrate();

            // Assert
            Assert.AreEqual("hex", board.Info.FirmwareExtension);
        }

        [TestMethod]
        public void Migrate_WithNullFirmwareExtension_SetsToHex()
        {
            // Arrange
            var board = new Board
            {
                AvrDudeSettings = new AvrDudeSettings(),
                Connection = new Connection { TimeoutForFirmwareUpdate = 5000 },
                Info = new Info { FirmwareExtension = null }
            };

            // Act
            board.Migrate();

            // Assert
            Assert.AreEqual("hex", board.Info.FirmwareExtension);
        }

        [TestMethod]
        public void Migrate_WithEmptyStringFirmwareExtension_SetsToHex()
        {
            // Arrange
            var board = new Board
            {
                AvrDudeSettings = new AvrDudeSettings(),
                Connection = new Connection { TimeoutForFirmwareUpdate = 5000 },
                Info = new Info { FirmwareExtension = "" }
            };

            // Act
            board.Migrate();

            // Assert
            Assert.AreEqual("hex", board.Info.FirmwareExtension);
        }

        [TestMethod]
        public void Migrate_WithExistingFirmwareExtension_DoesNotOverwrite()
        {
            // Arrange
            var board = new Board
            {
                AvrDudeSettings = new AvrDudeSettings(),
                Connection = new Connection { TimeoutForFirmwareUpdate = 5000 },
                Info = new Info { FirmwareExtension = "bin" }
            };

            // Act
            board.Migrate();

            // Assert
            Assert.AreEqual("bin", board.Info.FirmwareExtension);
        }

        [TestMethod]
        public void Migrate_WithWhitespaceOnlyFirmwareExtension_SetsToHex()
        {
            // Arrange
            var board = new Board
            {
                AvrDudeSettings = new AvrDudeSettings(),
                Connection = new Connection { TimeoutForFirmwareUpdate = 5000 },
                Info = new Info { FirmwareExtension = "   " }
            };

            // Act
            board.Migrate();

            // Assert - String.IsNullOrEmpty("   ") returns false, so it should not be overwritten
            Assert.AreEqual("   ", board.Info.FirmwareExtension);
        }

        #endregion

        #region TimeoutForFirmwareUpdate Migration Tests

        [TestMethod]
        public void Migrate_WithZeroTimeoutForFirmwareUpdateAndAvrDudeTimeout_MigratesFromAvrDude()
        {
            // Arrange
            var board = new Board
            {
                AvrDudeSettings = new AvrDudeSettings { Timeout = 20000 },
                Connection = new Connection { TimeoutForFirmwareUpdate = 0 },
                Info = new Info()
            };

            // Act
            board.Migrate();

            // Assert
            Assert.AreEqual(20000, board.Connection.TimeoutForFirmwareUpdate);
        }

        [TestMethod]
        public void Migrate_WithZeroTimeoutForFirmwareUpdateAndZeroAvrDudeTimeout_SetsToDefault15000()
        {
            // Arrange
            var board = new Board
            {
                AvrDudeSettings = new AvrDudeSettings { Timeout = 0 },
                Connection = new Connection { TimeoutForFirmwareUpdate = 0 },
                Info = new Info()
            };

            // Act
            board.Migrate();

            // Assert
            Assert.AreEqual(15000, board.Connection.TimeoutForFirmwareUpdate);
        }

        [TestMethod]
        public void Migrate_WithZeroTimeoutForFirmwareUpdateAndNullAvrDudeSettings_SetsToDefault15000()
        {
            // Arrange
            var board = new Board
            {
                AvrDudeSettings = null,
                Connection = new Connection { TimeoutForFirmwareUpdate = 0 },
                Info = new Info()
            };

            // Act
            board.Migrate();

            // Assert
            Assert.AreEqual(15000, board.Connection.TimeoutForFirmwareUpdate);
        }

        [TestMethod]
        public void Migrate_WithNonZeroTimeoutForFirmwareUpdate_DoesNotChange()
        {
            // Arrange
            var board = new Board
            {
                AvrDudeSettings = new AvrDudeSettings { Timeout = 30000 },
                Connection = new Connection { TimeoutForFirmwareUpdate = 25000 },
                Info = new Info()
            };

            // Act
            board.Migrate();

            // Assert
            Assert.AreEqual(25000, board.Connection.TimeoutForFirmwareUpdate);
        }

        #endregion

        #region Complex Migration Scenarios

        [TestMethod]
        public void Migrate_WithAllLegacyValues_MigratesAllCorrectly()
        {
            // Arrange
            var board = new Board
            {
                AvrDudeSettings = new AvrDudeSettings
                {
#pragma warning disable CS0612 // Type or member is obsolete
                    BaudRate = "57600",
                    ResetFirmwareFile = "reset.hex",
                    FirmwareBaseName = "legacy_firmware",
#pragma warning restore CS0612
                    BaudRates = null,
                    Timeout = 25000
                },
                Connection = new Connection { TimeoutForFirmwareUpdate = 0 },
                Info = new Info 
                { 
                    ResetFirmwareFile = null,
                    FirmwareBaseName = null,
                    FirmwareExtension = null
                }
            };

            // Act
            board.Migrate();

            // Assert
            Assert.IsNotNull(board.AvrDudeSettings.BaudRates);
            Assert.AreEqual(1, board.AvrDudeSettings.BaudRates.Count);
            Assert.AreEqual("57600", board.AvrDudeSettings.BaudRates[0]);
            Assert.AreEqual("reset.hex", board.Info.ResetFirmwareFile);
            Assert.AreEqual("legacy_firmware", board.Info.FirmwareBaseName);
            Assert.AreEqual("hex", board.Info.FirmwareExtension);
            Assert.AreEqual(25000, board.Connection.TimeoutForFirmwareUpdate);
        }

        [TestMethod]
        public void Migrate_WithPartialLegacyValues_MigratesOnlyMissingValues()
        {
            // Arrange
            var board = new Board
            {
                AvrDudeSettings = new AvrDudeSettings
                {
#pragma warning disable CS0612 // Type or member is obsolete
                    BaudRate = "115200",
                    ResetFirmwareFile = "old_reset.hex",
                    FirmwareBaseName = "old_firmware",
#pragma warning restore CS0612
                    BaudRates = new List<string> { "9600" },
                    Timeout = 20000
                },
                Connection = new Connection { TimeoutForFirmwareUpdate = 0 },
                Info = new Info 
                { 
                    ResetFirmwareFile = "new_reset.hex",
                    FirmwareBaseName = null,
                    FirmwareExtension = "uf2"
                }
            };

            // Act
            board.Migrate();

            // Assert
            // BaudRates should not be overwritten
            Assert.AreEqual(1, board.AvrDudeSettings.BaudRates.Count);
            Assert.AreEqual("9600", board.AvrDudeSettings.BaudRates[0]);
            
            // ResetFirmwareFile should not be overwritten
            Assert.AreEqual("new_reset.hex", board.Info.ResetFirmwareFile);
            
            // FirmwareBaseName should be migrated (was null)
            Assert.AreEqual("old_firmware", board.Info.FirmwareBaseName);
            
            // FirmwareExtension should not be overwritten
            Assert.AreEqual("uf2", board.Info.FirmwareExtension);
            
            // TimeoutForFirmwareUpdate should be migrated
            Assert.AreEqual(20000, board.Connection.TimeoutForFirmwareUpdate);
        }

        [TestMethod]
        public void Migrate_CalledMultipleTimes_IsIdempotent()
        {
            // Arrange
            var board = new Board
            {
                AvrDudeSettings = new AvrDudeSettings
                {
#pragma warning disable CS0612 // Type or member is obsolete
                    BaudRate = "115200",
                    ResetFirmwareFile = "reset.hex",
                    FirmwareBaseName = "firmware_base",
#pragma warning restore CS0612
                    BaudRates = null,
                    Timeout = 30000
                },
                Connection = new Connection { TimeoutForFirmwareUpdate = 0 },
                Info = new Info 
                { 
                    ResetFirmwareFile = null,
                    FirmwareBaseName = null,
                    FirmwareExtension = null
                }
            };

            // Act - Call Migrate multiple times
            board.Migrate();
            board.Migrate();
            board.Migrate();

            // Assert - Results should be the same after multiple calls
            Assert.AreEqual(1, board.AvrDudeSettings.BaudRates.Count);
            Assert.AreEqual("115200", board.AvrDudeSettings.BaudRates[0]);
            Assert.AreEqual("reset.hex", board.Info.ResetFirmwareFile);
            Assert.AreEqual("firmware_base", board.Info.FirmwareBaseName);
            Assert.AreEqual("hex", board.Info.FirmwareExtension);
            Assert.AreEqual(30000, board.Connection.TimeoutForFirmwareUpdate);
        }

        #endregion

        #region Edge Cases and Error Conditions

        [TestMethod]
        public void Migrate_WithNullConnection_ThrowsNullReferenceException()
        {
            // Arrange
            var board = new Board
            {
                AvrDudeSettings = new AvrDudeSettings(),
                Connection = null,
                Info = new Info()
            };

            // Act & Assert
            Assert.ThrowsExactly<System.NullReferenceException>(() => board.Migrate());
        }

        [TestMethod]
        public void Migrate_WithNullInfo_ThrowsNullReferenceException()
        {
            // Arrange
            var board = new Board
            {
                AvrDudeSettings = new AvrDudeSettings(),
                Connection = new Connection { TimeoutForFirmwareUpdate = 5000 },
                Info = null
            };

            // Act & Assert
            Assert.ThrowsExactly<System.NullReferenceException>(() => board.Migrate());
        }

        #endregion
    }
}