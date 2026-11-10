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
        private MethodInfo calculateMD5Method;

        [TestInitialize]
        public void Setup()
        {
            // Create a temporary directory for test files
            testDirectory = Path.Combine(Path.GetTempPath(), "WasmModuleUpdaterTests_" + Guid.NewGuid());
            Directory.CreateDirectory(testDirectory);
            
            // Create a test file with some content
            testFile = Path.Combine(testDirectory, "test.wasm");
            File.WriteAllText(testFile, "Test content for MD5 calculation");

            // Get reference to the private CalculateMD5 method once during setup
            calculateMD5Method = typeof(WasmModuleUpdater).GetMethod("CalculateMD5", 
                BindingFlags.NonPublic | BindingFlags.Static);
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

        private string InvokeCalculateMD5(string filename)
        {
            return calculateMD5Method.Invoke(null, new object[] { filename }) as string;
        }

        [TestMethod()]
        public void CalculateMD5_ShouldReturnValidHash_WhenFileExists()
        {
            // Arrange, Act
            var result = InvokeCalculateMD5(testFile);

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

            // Act
            var result = InvokeCalculateMD5(nonExistentFile);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod()]
        public void CalculateMD5_ShouldReturnNull_WhenFileIsLocked()
        {
            // Arrange, Act, Assert
            // Lock the file by opening it exclusively. We do not need a variable here;
            // the using statement ensures the FileStream remains open (and thus locked)
            // for the duration of this block and is disposed afterwards.
            using (new FileStream(testFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                var result = InvokeCalculateMD5(testFile);
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

        #region UserCfg.opt parsing

        private string WriteUserCfg(params string[] lines)
        {
            var userCfg = Path.Combine(testDirectory, "UserCfg.opt");
            File.WriteAllLines(userCfg, lines);
            return userCfg;
        }

        [TestMethod()]
        public void ReadInstalledPackagesPath_ShouldReturnPathWithoutQuotes()
        {
            // Arrange
            var userCfg = WriteUserCfg(
                "{",
                "InstalledPackagesPath \"D:\\MSFS\"",
                "}");

            // Act
            var result = WasmModuleUpdater.ReadInstalledPackagesPath(userCfg);

            // Assert
            Assert.AreEqual(@"D:\MSFS", result);
        }

        [TestMethod()]
        public void ReadInstalledPackagesPath_ShouldIgnoreNextBootEntry()
        {
            // Arrange
            var userCfg = WriteUserCfg(
                "InstalledPackagesPathNextBoot \"E:\\Other\"",
                "InstalledPackagesPath \"D:\\MSFS\"");

            // Act
            var result = WasmModuleUpdater.ReadInstalledPackagesPath(userCfg);

            // Assert
            Assert.AreEqual(@"D:\MSFS", result);
        }

        [TestMethod()]
        public void ReadInstalledPackagesPath_ShouldHandleIndentedEntry()
        {
            // Arrange
            var userCfg = WriteUserCfg("    InstalledPackagesPath \"D:\\MSFS\"");

            // Act
            var result = WasmModuleUpdater.ReadInstalledPackagesPath(userCfg);

            // Assert
            Assert.AreEqual(@"D:\MSFS", result);
        }

        [TestMethod()]
        public void ReadInstalledPackagesPath_ShouldReturnNull_WhenEntryIsMissing()
        {
            // Arrange
            var userCfg = WriteUserCfg("{", "Something \"else\"", "}");

            // Act
            var result = WasmModuleUpdater.ReadInstalledPackagesPath(userCfg);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod()]
        [DataRow("InstalledPackagesPath")]
        [DataRow("InstalledPackagesPath ")]
        [DataRow("InstalledPackagesPath \"\"")]
        public void ReadInstalledPackagesPath_ShouldReturnNull_WhenValueIsEmpty(string line)
        {
            // Arrange
            var userCfg = WriteUserCfg(line);

            // Act
            var result = WasmModuleUpdater.ReadInstalledPackagesPath(userCfg);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod()]
        public void ReadInstalledPackagesPath_ShouldNotKeepFileLocked()
        {
            // Arrange
            var userCfg = WriteUserCfg("Something \"else\"");

            // Act
            WasmModuleUpdater.ReadInstalledPackagesPath(userCfg);

            // Assert
            using (new FileStream(userCfg, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                // opening exclusively succeeds only if the reader was disposed
            }
        }

        #endregion
    }
}
