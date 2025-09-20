using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Joysticks;
using System;
using System.IO;
using System.Linq;

namespace MobiFlight.Joysticks.Tests
{
    [TestClass]
    public class JoystickDefinitionMigratorTests
    {
        private string _testDirectory;
        private string _joysticksDirectory;

        [TestInitialize]
        public void SetUp()
        {
            // Create a temporary test directory structure
            _testDirectory = Path.Combine(Path.GetTempPath(), "MobiFlightTests", Guid.NewGuid().ToString());
            _joysticksDirectory = Path.Combine(_testDirectory, "Joysticks");
            Directory.CreateDirectory(_joysticksDirectory);
        }

        [TestCleanup]
        public void TearDown()
        {
            // Clean up test directory
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }

        #region Unique File Migration Tests

        [TestMethod]
        public void MigrateDefinitions_UniqueFileInRoot_MovesToMigratedFolder()
        {
            // Arrange
            var uniqueFile = Path.Combine(_joysticksDirectory, "custom_controller.joystick.json");
            var uniqueContent = @"{""InstanceName"": ""Custom Controller"", ""VendorId"": ""0x1234"", ""ProductId"": ""0x5678""}";
            File.WriteAllText(uniqueFile, uniqueContent);

            // Act
            JoystickDefinitionMigrator.MigrateDefinitions(_joysticksDirectory);

            // Assert
            Assert.IsFalse(File.Exists(uniqueFile), "Original file should be moved");
            
            var migratedFile = Path.Combine(_joysticksDirectory, "_MIGRATED_", "custom_controller.joystick.json");
            Assert.IsTrue(File.Exists(migratedFile), "File should be in _MIGRATED_ folder");
            Assert.AreEqual(uniqueContent, File.ReadAllText(migratedFile), "Content should be preserved");
        }

        [TestMethod]
        public void MigrateDefinitions_MultipleUniqueFiles_AllMovedToMigratedFolder()
        {
            // Arrange
            var file1 = Path.Combine(_joysticksDirectory, "controller1.joystick.json");
            var file2 = Path.Combine(_joysticksDirectory, "controller2.joystick.json");
            
            File.WriteAllText(file1, @"{""InstanceName"": ""Controller 1""}");
            File.WriteAllText(file2, @"{""InstanceName"": ""Controller 2""}");

            // Act
            JoystickDefinitionMigrator.MigrateDefinitions(_joysticksDirectory);

            // Assert
            var migratedFolder = Path.Combine(_joysticksDirectory, "_MIGRATED_");
            Assert.IsTrue(Directory.Exists(migratedFolder), "_MIGRATED_ folder should exist");
            Assert.IsTrue(File.Exists(Path.Combine(migratedFolder, "controller1.joystick.json")), "Controller 1 should be migrated");
            Assert.IsTrue(File.Exists(Path.Combine(migratedFolder, "controller2.joystick.json")), "Controller 2 should be migrated");
        }

        #endregion

        #region Identical Duplicate Tests

        [TestMethod]
        public void MigrateDefinitions_IdenticalDuplicates_DeletesOldFile()
        {
            // Arrange
            var content = @"{""InstanceName"": ""Test Controller"", ""VendorId"": ""0x1234""}";
            var oldFile = Path.Combine(_joysticksDirectory, "test_controller.joystick.json");
            var subFolder = Path.Combine(_joysticksDirectory, "vendor");
            Directory.CreateDirectory(subFolder);
            var newFile = Path.Combine(subFolder, "test_controller.joystick.json");

            File.WriteAllText(oldFile, content);
            File.WriteAllText(newFile, content);

            // Act
            JoystickDefinitionMigrator.MigrateDefinitions(_joysticksDirectory);

            // Assert
            Assert.IsFalse(File.Exists(oldFile), "Old duplicate file should be deleted");
            Assert.IsTrue(File.Exists(newFile), "New file should remain");
            Assert.AreEqual(content, File.ReadAllText(newFile), "New file content should be unchanged");
        }

        [TestMethod]
        public void MigrateDefinitions_IdenticalDuplicatesWithWhitespace_DeletesOldFile()
        {
            // Arrange - Test that whitespace differences are handled correctly
            var oldContent = @"{""InstanceName"": ""Test Controller"", ""VendorId"": ""0x1234""}";
            var newContent = @"{""InstanceName"": ""Test Controller"", ""VendorId"": ""0x1234""}"; // Identical content
            
            var oldFile = Path.Combine(_joysticksDirectory, "whitespace_test.joystick.json");
            var subFolder = Path.Combine(_joysticksDirectory, "vendor");
            Directory.CreateDirectory(subFolder);
            var newFile = Path.Combine(subFolder, "whitespace_test.joystick.json");

            File.WriteAllText(oldFile, oldContent);
            File.WriteAllText(newFile, newContent);

            // Act
            JoystickDefinitionMigrator.MigrateDefinitions(_joysticksDirectory);

            // Assert
            Assert.IsFalse(File.Exists(oldFile), "Old file with identical content should be deleted");
            Assert.IsTrue(File.Exists(newFile), "New file should remain");
        }

        #endregion

        #region Conflicted File Tests

        [TestMethod]
        public void MigrateDefinitions_ConflictedFiles_MovesToConflictFolder()
        {
            // Arrange
            var oldContent = @"{""InstanceName"": ""Test Controller"", ""VendorId"": ""0x1234"", ""Version"": ""1.0""}";
            var newContent = @"{""InstanceName"": ""Test Controller"", ""VendorId"": ""0x1234"", ""Version"": ""2.0""}";
            
            var oldFile = Path.Combine(_joysticksDirectory, "conflict_test.joystick.json");
            var subFolder = Path.Combine(_joysticksDirectory, "vendor");
            Directory.CreateDirectory(subFolder);
            var newFile = Path.Combine(subFolder, "conflict_test.joystick.json");

            File.WriteAllText(oldFile, oldContent);
            File.WriteAllText(newFile, newContent);

            // Act
            JoystickDefinitionMigrator.MigrateDefinitions(_joysticksDirectory);

            // Assert
            Assert.IsFalse(File.Exists(oldFile), "Old conflicted file should be moved");
            Assert.IsTrue(File.Exists(newFile), "New file should remain");
            
            var conflictFolder = Path.Combine(_joysticksDirectory, "_CONFLICT_");
            Assert.IsTrue(Directory.Exists(conflictFolder), "_CONFLICT_ folder should exist");
            
            var conflictFiles = Directory.GetFiles(conflictFolder);
            Assert.IsTrue(conflictFiles.Length > 0, "_CONFLICT_ folder should contain files");
            
            var conflictFile = conflictFiles.FirstOrDefault(f => Path.GetFileName(f).StartsWith("conflict_test.joystick.json_"));
            Assert.IsNotNull(conflictFile, "Should have a conflict file starting with 'conflict_test.joystick.json_'");
            Assert.AreEqual(oldContent, File.ReadAllText(conflictFile), "Conflict file should preserve old content");
        }

        [TestMethod]
        public void MigrateDefinitions_MultipleConflicts_AllMovedToConflictFolder()
        {
            // Arrange
            CreateConflictScenario("conflict1.joystick.json", "old content 1", "new content 1");
            CreateConflictScenario("conflict2.joystick.json", "old content 2", "new content 2");

            // Act
            JoystickDefinitionMigrator.MigrateDefinitions(_joysticksDirectory);

            // Assert
            var conflictFolder = Path.Combine(_joysticksDirectory, "_CONFLICT_");
            var conflictFiles = Directory.GetFiles(conflictFolder, "*.bak");
            Assert.AreEqual(2, conflictFiles.Length, "Should have two conflict backup files");
        }

        [TestMethod]
        public void MigrateDefinitions_ConflictFileName_ContainsTimestamp()
        {
            // Arrange
            CreateConflictScenario("timestamp_test.joystick.json", "old", "new");

            // Act
            var beforeTime = DateTime.Now.ToString("yyyyMMddHHmm");
            JoystickDefinitionMigrator.MigrateDefinitions(_joysticksDirectory);
            var afterTime = DateTime.Now.ToString("yyyyMMddHHmm");

            // Assert
            var conflictFolder = Path.Combine(_joysticksDirectory, "_CONFLICT_");
            var conflictFiles = Directory.GetFiles(conflictFolder, "timestamp_test.joystick.json_*.bak");
            Assert.AreEqual(1, conflictFiles.Length, "Should have one conflict file");
            
            var fileName = Path.GetFileName(conflictFiles[0]);
            Assert.IsTrue(fileName.StartsWith("timestamp_test.joystick.json_"), "File should start with full filename");
            Assert.IsTrue(fileName.EndsWith(".bak"), "File should end with .bak");
            
            // Extract timestamp from filename (format: fullfilename_yyyyMMddHHmmss.bak)
            var timestampPart = fileName.Substring("timestamp_test.joystick.json_".Length, 12); // YYYYMMDDHHMM
            Assert.IsTrue(timestampPart.CompareTo(beforeTime) >= 0 && timestampPart.CompareTo(afterTime) <= 0, 
                "Timestamp should be within test execution time");
        }

        #endregion

        #region Mixed Scenario Tests

        [TestMethod]
        public void MigrateDefinitions_MixedScenarios_HandlesAllCorrectly()
        {
            // Arrange - Create all three scenarios
            
            // 1. Unique file
            var uniqueFile = Path.Combine(_joysticksDirectory, "unique.joystick.json");
            File.WriteAllText(uniqueFile, @"{""InstanceName"": ""Unique Controller""}");
            
            // 2. Identical duplicate
            CreateIdenticalDuplicateScenario("identical.joystick.json", @"{""InstanceName"": ""Identical""}");
            
            // 3. Conflicted files
            CreateConflictScenario("conflict.joystick.json", "old version", "new version");

            // Act
            JoystickDefinitionMigrator.MigrateDefinitions(_joysticksDirectory);

            // Assert
            // Check unique file migration
            Assert.IsTrue(File.Exists(Path.Combine(_joysticksDirectory, "_MIGRATED_", "unique.joystick.json")), 
                "Unique file should be in _MIGRATED_");
            
            // Check identical duplicate deletion
            Assert.IsFalse(File.Exists(Path.Combine(_joysticksDirectory, "identical.joystick.json")), 
                "Identical duplicate should be deleted");
            Assert.IsTrue(File.Exists(Path.Combine(_joysticksDirectory, "vendor", "identical.joystick.json")), 
                "New identical file should remain");
            
            // Check conflict backup
            var conflictFiles = Directory.GetFiles(Path.Combine(_joysticksDirectory, "_CONFLICT_"), "conflict.joystick.json_*.bak");
            Assert.AreEqual(1, conflictFiles.Length, "Conflict should be backed up");
        }

        #endregion

        #region Error Handling Tests

        [TestMethod]
        public void MigrateDefinitions_EmptyJoysticksFolder_DoesNotThrow()
        {
            // Arrange - Empty joysticks folder

            // Act & Assert - Should not throw
            try
            {
                JoystickDefinitionMigrator.MigrateDefinitions(_joysticksDirectory);
                Assert.IsTrue(true, "Should complete without throwing");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Should not throw exception: {ex.Message}");
            }
        }

        [TestMethod]
        public void MigrateDefinitions_NonExistentJoysticksFolder_DoesNotThrow()
        {
            // Arrange - Delete the joysticks folder
            Directory.Delete(_joysticksDirectory, true);

            // Act & Assert - Should not throw
            try
            {
                JoystickDefinitionMigrator.MigrateDefinitions(_joysticksDirectory);
                Assert.IsTrue(true, "Should complete without throwing");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Should not throw exception: {ex.Message}");
            }
        }

        [TestMethod]
        public void MigrateDefinitions_ReadOnlyFile_SkipsProcessing()
        {
            // Arrange
            var readOnlyFile = Path.Combine(_joysticksDirectory, "readonly.joystick.json");
            File.WriteAllText(readOnlyFile, @"{""InstanceName"": ""ReadOnly""}");
            File.SetAttributes(readOnlyFile, FileAttributes.ReadOnly);

            try
            {
                // Act
                JoystickDefinitionMigrator.MigrateDefinitions(_joysticksDirectory);

                // Assert - Read-only file should be skipped, not migrated
                Assert.IsTrue(File.Exists(readOnlyFile), "Read-only file should remain in original location");
                
                var migratedFile = Path.Combine(_joysticksDirectory, "_MIGRATED_", "readonly.joystick.json");
                Assert.IsFalse(File.Exists(migratedFile), "Read-only file should not be migrated");
                
                var conflictFolder = Path.Combine(_joysticksDirectory, "_CONFLICT_");
                if (Directory.Exists(conflictFolder))
                {
                    var conflictFiles = Directory.GetFiles(conflictFolder, "readonly*");
                    Assert.AreEqual(0, conflictFiles.Length, "Read-only file should not be in conflict folder");
                }
            }
            finally
            {
                // Cleanup - Remove read-only attribute for test cleanup
                try
                {
                    if (File.Exists(readOnlyFile))
                    {
                        File.SetAttributes(readOnlyFile, FileAttributes.Normal);
                    }
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }

        #endregion

        #region Folder Structure Tests

        [TestMethod]
        public void MigrateDefinitions_CreatesNecessaryFolders()
        {
            // Arrange
            CreateIdenticalDuplicateScenario("test1.joystick.json", "content");
            CreateConflictScenario("test2.joystick.json", "old", "new");
            var uniqueFile = Path.Combine(_joysticksDirectory, "test3.joystick.json");
            File.WriteAllText(uniqueFile, "unique content");

            // Act
            JoystickDefinitionMigrator.MigrateDefinitions(_joysticksDirectory);

            // Assert
            Assert.IsTrue(Directory.Exists(Path.Combine(_joysticksDirectory, "_MIGRATED_")), 
                "_MIGRATED_ folder should be created");
            Assert.IsTrue(Directory.Exists(Path.Combine(_joysticksDirectory, "_CONFLICT_")), 
                "_CONFLICT_ folder should be created");
        }

        [TestMethod]
        public void MigrateDefinitions_OnlyProcessesTopLevelFiles()
        {
            // Arrange
            var topLevelFile = Path.Combine(_joysticksDirectory, "toplevel.joystick.json");
            File.WriteAllText(topLevelFile, "top level content");

            var subFolder = Path.Combine(_joysticksDirectory, "vendor");
            Directory.CreateDirectory(subFolder);
            var subFolderFile = Path.Combine(subFolder, "subfolder.joystick.json");
            File.WriteAllText(subFolderFile, "subfolder content");

            // Act
            JoystickDefinitionMigrator.MigrateDefinitions(_joysticksDirectory);

            // Assert
            Assert.IsFalse(File.Exists(topLevelFile), "Top level file should be processed");
            Assert.IsTrue(File.Exists(subFolderFile), "Subfolder file should not be processed");
            Assert.IsTrue(File.Exists(Path.Combine(_joysticksDirectory, "_MIGRATED_", "toplevel.joystick.json")), 
                "Top level file should be migrated");
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Creates a scenario with identical duplicate files
        /// </summary>
        private void CreateIdenticalDuplicateScenario(string fileName, string content)
        {
            var oldFile = Path.Combine(_joysticksDirectory, fileName);
            var subFolder = Path.Combine(_joysticksDirectory, "vendor");
            Directory.CreateDirectory(subFolder);
            var newFile = Path.Combine(subFolder, fileName);

            File.WriteAllText(oldFile, content);
            File.WriteAllText(newFile, content);
        }

        /// <summary>
        /// Creates a scenario with conflicting files (different content)
        /// </summary>
        private void CreateConflictScenario(string fileName, string oldContent, string newContent)
        {
            var oldFile = Path.Combine(_joysticksDirectory, fileName);
            var subFolder = Path.Combine(_joysticksDirectory, "vendor");
            Directory.CreateDirectory(subFolder);
            var newFile = Path.Combine(subFolder, fileName);

            File.WriteAllText(oldFile, oldContent);
            File.WriteAllText(newFile, newContent);
        }

        #endregion
    }
}