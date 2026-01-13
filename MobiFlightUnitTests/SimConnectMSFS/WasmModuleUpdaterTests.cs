using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Reflection;

namespace MobiFlight.SimConnectMSFS.Tests
{
    [TestClass()]
    public class WasmModuleUpdaterTests
    {
        private string testDirectory;
        private string testFile;

        [TestInitialize]
        public void Setup()
        {
            // Create a temporary directory for test files
            testDirectory = Path.Combine(Path.GetTempPath(), "WasmModuleUpdaterTests_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(testDirectory);
            
            // Create a test file with some content
            testFile = Path.Combine(testDirectory, "test.wasm");
            File.WriteAllText(testFile, "Test content for MD5 calculation");
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up test directory
            if (Directory.Exists(testDirectory))
            {
                Directory.Delete(testDirectory, true);
            }
        }

        [TestMethod()]
        public void CalculateMD5_ShouldReturnValidHash_WhenFileExists()
        {
            // Arrange
            // Use reflection to access the private CalculateMD5 method
            var method = typeof(WasmModuleUpdater).GetMethod("CalculateMD5", 
                BindingFlags.NonPublic | BindingFlags.Static);

            // Act
            var result = method.Invoke(null, new object[] { testFile }) as string;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(32, result.Length); // MD5 hash is 32 hex characters
            Assert.IsTrue(System.Text.RegularExpressions.Regex.IsMatch(result, "^[a-f0-9]{32}$"));
        }

        [TestMethod()]
        public void CalculateMD5_ShouldReturnNull_WhenFileDoesNotExist()
        {
            // Arrange
            string nonExistentFile = Path.Combine(testDirectory, "nonexistent.wasm");
            var method = typeof(WasmModuleUpdater).GetMethod("CalculateMD5",
                BindingFlags.NonPublic | BindingFlags.Static);

            // Act
            var result = method.Invoke(null, new object[] { nonExistentFile }) as string;

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod()]
        public void CalculateMD5_ShouldReturnNull_WhenFileIsLocked()
        {
            // Arrange
            var method = typeof(WasmModuleUpdater).GetMethod("CalculateMD5",
                BindingFlags.NonPublic | BindingFlags.Static);

            // Lock the file by opening it exclusively
            using (var fileStream = new FileStream(testFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                // Act
                var result = method.Invoke(null, new object[] { testFile }) as string;

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod()]
        public void WasmModulesAreDifferent_ShouldReturnTrue_WhenCommunityFolderIsNull()
        {
            // Arrange
            var updater = new WasmModuleUpdater();

            // Act
            var result = updater.WasmModulesAreDifferent(null);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void WasmModulesAreDifferent_ShouldReturnTrue_WhenCommunityFolderIsEmpty()
        {
            // Arrange
            var updater = new WasmModuleUpdater();

            // Act
            var result = updater.WasmModulesAreDifferent("");

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void WasmModulesAreDifferent_ShouldReturnTrue_WhenWasmFileDoesNotExist()
        {
            // Arrange
            var updater = new WasmModuleUpdater();

            // Act
            var result = updater.WasmModulesAreDifferent(testDirectory);

            // Assert
            Assert.IsTrue(result);
        }
    }
}
