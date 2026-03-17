using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace MobiFlight.Scripts.Tests
{
    [TestClass()]
    public class PythonEnvironmentTests
    {
        private string testBaseDirectory;
        private string testPythonBaseFolder;
        private string testPythonRuntimeFolder;
        private string sourceZipFile;

        // Shared test directory for all tests in this class
        private static string sharedTestDirectory;

        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {           
            sharedTestDirectory = Path.Combine(Path.GetTempPath(), "MobiFlightTests_PythonEnv");

            // Clean the directory if it exists from previous test runs
            if (Directory.Exists(sharedTestDirectory))
            {
                try
                {
                    Directory.Delete(sharedTestDirectory, true);
                    Console.WriteLine($"Cleaned existing test directory: {sharedTestDirectory}");
                }
                catch (Exception ex)
                {
                    // If cleanup fails, it's not critical - tests might still work
                    Console.WriteLine($"Warning: Could not clean test directory: {ex.Message}");
                }
            }

            // Create fresh directory
            Directory.CreateDirectory(sharedTestDirectory);
            Console.WriteLine($"Created test directory: {sharedTestDirectory}");
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            // Optional: Try to clean up after all tests complete
            // (This may still fail due to file locks, but worth trying)
            if (Directory.Exists(sharedTestDirectory))
            {
                try
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    System.Threading.Thread.Sleep(500);
                    
                    Directory.Delete(sharedTestDirectory, true);
                    Console.WriteLine("Successfully cleaned up test directory after all tests.");
                }
                catch
                {
                    Console.WriteLine($"Note: Test directory will be cleaned on next test run: {sharedTestDirectory}");
                }
            }
        }

        [TestInitialize]
        public void Setup()
        {
            // Use the actual Python folder from the repository
            string solutionDirectory = GetSolutionDirectory();
            string sourcePythonFolder = Path.Combine(solutionDirectory, "Python");
            
            // Find the zip file in the source Python folder
            string[] zipFiles = Directory.GetFiles(sourcePythonFolder, "*.zip");
            if (zipFiles.Length == 0)
            {
                Assert.Fail("No zip file found in the Python folder. Expected RuntimeAndPackages.*.zip");
            }
            sourceZipFile = zipFiles[0];

            // Use the shared test directory (same for all tests)
            testBaseDirectory = sharedTestDirectory;
            testPythonBaseFolder = Path.Combine(testBaseDirectory, PythonEnvironment.PYTHON_BASE_FOLDER);
            testPythonRuntimeFolder = Path.Combine(testPythonBaseFolder, PythonEnvironment.PYTHON_RUNTIME_VERSION);

            // Ensure Python folder exists
            Directory.CreateDirectory(testPythonBaseFolder);

            // Copy the zip file to the test folder (only if it doesn't exist)
            string destZipFile = Path.Combine(testPythonBaseFolder, Path.GetFileName(sourceZipFile));
            if (!File.Exists(destZipFile))
            {
                File.Copy(sourceZipFile, destZipFile);
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clear static reference
            PythonEnvironment.PathPythonExecutable = null;
          
            if (Directory.Exists(testPythonBaseFolder))
            {
                Directory.Delete(testPythonBaseFolder, true);
            }
        }

        [TestMethod()]
        public void Initialize_DeletesFoldersAndExtractsZip_CreatesRuntimeWithPythonExe()
        {
            // Arrange: Create some existing folders that should be deleted
            string oldFolder1 = Path.Combine(testPythonBaseFolder, "OldVersion");
            string oldFolder2 = Path.Combine(testPythonBaseFolder, "3.13.0");
            Directory.CreateDirectory(oldFolder1);
            Directory.CreateDirectory(oldFolder2);
            File.WriteAllText(Path.Combine(oldFolder1, "test.txt"), "old content");

            // Act: Initialize Python environment
            PythonEnvironment.Initialize(testBaseDirectory);

            // Assert: Old folders should be deleted
            Assert.IsFalse(Directory.Exists(oldFolder1), "Old folder 1 should be deleted");
            Assert.IsFalse(Directory.Exists(oldFolder2), "Old folder 2 should be deleted");

            // Assert: New runtime folder should be created
            Assert.IsTrue(Directory.Exists(testPythonRuntimeFolder), 
                $"Python runtime folder should exist at {testPythonRuntimeFolder}");

            // Assert: python.exe should exist
            string pythonExePath = Path.Combine(testPythonRuntimeFolder, "python.exe");
            Assert.IsTrue(File.Exists(pythonExePath), 
                $"python.exe should exist at {pythonExePath}");
        }

        [TestMethod()]
        public void Initialize_WithExistingPythonExe_SkipsExtraction()
        {
            // Arrange: Create runtime folder with python.exe
            Directory.CreateDirectory(testPythonRuntimeFolder);
            string pythonExePath = Path.Combine(testPythonRuntimeFolder, "python.exe");
            File.WriteAllText(pythonExePath, "existing python");
            DateTime originalWriteTime = File.GetLastWriteTime(pythonExePath);

            // Wait a moment to ensure timestamp would change if file is modified
            System.Threading.Thread.Sleep(100);

            // Act: Initialize should skip extraction
            PythonEnvironment.Initialize(testBaseDirectory);

            // Assert: File timestamp should not change (extraction was skipped)
            DateTime newWriteTime = File.GetLastWriteTime(pythonExePath);
            Assert.AreEqual(originalWriteTime, newWriteTime, 
                "python.exe should not be modified when it already exists");
        }


        [TestMethod()]
        public void Initialize_ZipPreserved_AfterExtraction()
        {
            // Arrange: Get original zip file
            string zipFile = Directory.GetFiles(testPythonBaseFolder, "*.zip").FirstOrDefault();
            Assert.IsNotNull(zipFile, "Zip file should exist before initialization");

            // Act: Initialize
            PythonEnvironment.Initialize(testBaseDirectory);

            // Assert: Zip file should still exist (not deleted)
            Assert.IsTrue(File.Exists(zipFile), 
                "Zip file should be preserved after extraction");
        }

        #region Helper Methods

        /// <summary>
        /// Gets the solution directory by walking up from the test assembly location
        /// </summary>
        private string GetSolutionDirectory()
        {
            string assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            DirectoryInfo directory = new DirectoryInfo(Path.GetDirectoryName(assemblyLocation));

            // Walk up until we find the solution file or reach the root
            while (directory != null && !File.Exists(Path.Combine(directory.FullName, "MobiFlightConnector.sln")))
            {
                directory = directory.Parent;
            }

            if (directory == null)
            {
                Assert.Fail("Could not find solution directory");
            }

            return directory.FullName;
        }

        #endregion
    }
}
