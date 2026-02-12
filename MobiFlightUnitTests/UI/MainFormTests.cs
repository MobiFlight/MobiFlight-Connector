using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base;
using MobiFlight.BrowserMessages.Incoming;
using MobiFlight.Controllers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace MobiFlight.UI.Tests
{
    [TestClass()]
    public class MainFormTests
    {
        private StringCollection originalRecentFiles;
        private string _tempDirectory;

        public class TestableMainForm : MainForm
        {
            // Expose protected/private members for testing if needed
            public new Dictionary<string, string> AutoLoadConfigs
            {
                get => base.AutoLoadConfigs;
                set => base.AutoLoadConfigs = value;
            }

            public new void UpdateAutoLoadMenu()
            {
                base.UpdateAutoLoadMenu();
            }

            public void InitializeExecutionManager()
            {
                var methodInfo = typeof(MainForm).GetMethod("InitializeExecutionManager", BindingFlags.NonPublic | BindingFlags.Instance);
                methodInfo.Invoke(this, new object[] { });
            }

            public async Task InitializeProjectListManagerAsync()
            {
                var propertyInfo = typeof(MainForm).GetProperty("ProjectListManager", BindingFlags.NonPublic | BindingFlags.Instance);
                var projectListManager = new ProjectListManager();

                await projectListManager.InitializeFromSettingsAsync(new ControllerBindingService(ExecutionManager));
                propertyInfo.SetValue(this, projectListManager);
            }
        }

        private TestableMainForm _mainForm;

        [TestInitialize]
        public void SetUp()
        {
            // Initialize the MainForm
            _mainForm = new TestableMainForm();

            // Save original RecentFiles
            originalRecentFiles = Properties.Settings.Default.RecentFiles;

            // Initialize with clean state
            Properties.Settings.Default.RecentFiles = new StringCollection();

            // Create a temporary test directory
            _tempDirectory = Path.Combine(Path.GetTempPath(), "MainFormTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDirectory);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Restore original RecentFiles
            Properties.Settings.Default.RecentFiles = originalRecentFiles;
            Properties.Settings.Default.Save();

            try
            {
                // Clean up test directory
                if (Directory.Exists(_tempDirectory))
                {
                    Directory.Delete(_tempDirectory, true);
                }
            }
            catch { }
        }

        /// <summary>
        /// Helper method to create a temporary test file
        /// </summary>
        private string CreateTestFile(string fileName)
        {
            var filePath = Path.Combine(_tempDirectory, fileName);
            var testProject = new Project() { FilePath = filePath };
            testProject.SaveFile();
            return filePath;
        }


        [TestMethod()]
        public void CreateNewProjectTest()
        {
            // Arrange
            _mainForm.InitializeExecutionManager();
            Assert.IsFalse(_mainForm.ProjectHasUnsavedChanges, "ProjectHasUnsavedChanges should be False when initializing MainForm.");

            _mainForm.CreateNewProject(new Project());
            Assert.IsTrue(_mainForm.ProjectHasUnsavedChanges, "ProjectHasUnsavedChanges should be True when creating a new project.");

            // save it to bring it into clean state
            var tempFilePath = Path.Combine(Path.GetTempPath(), $"test_project_{Guid.NewGuid()}.mfproj");
            try
            {
                var saveMethod = typeof(MainForm).GetMethod("SaveConfig", BindingFlags.NonPublic | BindingFlags.Instance);
                saveMethod.Invoke(_mainForm, new object[] { tempFilePath });
                Assert.IsFalse(_mainForm.ProjectHasUnsavedChanges, "ProjectHasUnsavedChanges should be false when the project has been saved.");
            }
            finally
            {
                if (File.Exists(tempFilePath))
                {
                    // This sometimes fails in CI environments due to file locks.
                    // Just continue because Delete is not critical for the test.
                    try { File.Delete(tempFilePath); } catch { }
                }
            }

            // Act
            _mainForm.CreateNewProject(new Project());

            // Assert
            Assert.IsTrue(_mainForm.ProjectHasUnsavedChanges, "ProjectHasUnsavedChanges should be true when starting with a fresh project.");

            var mainFormTitle = _mainForm.Text;
            var expectedTitle = $"New MobiFlight Project* - MobiFlight Connector - {MainForm.DisplayVersion()}";
            Assert.AreEqual(expectedTitle, mainFormTitle);
        }


        [TestMethod()]
        public void AddNewFileToProjectTest()
        {
            // Arrange
            _mainForm.InitializeExecutionManager();
            Assert.IsFalse(_mainForm.ProjectHasUnsavedChanges, "ProjectHasUnsavedChanges should be true after adding a new file.");

            // Act
            _mainForm.AddNewFileToProject();

            // Assert
            var mainFormTitle = _mainForm.Text;
            Assert.IsTrue(_mainForm.ProjectHasUnsavedChanges, "ProjectHasUnsavedChanges should be true after adding a new file.");
            Assert.Contains("*", mainFormTitle, "Project title should indicate that there are unsaved changes.");
        }

        [TestMethod()]
        public void UpdateAutoLoadMenu_Doesnt_Throw_Exception()
        {
            // Arrange
            var exceptionThrown = false;
            try
            {
                _mainForm.UpdateAutoLoadMenu();
            }
            catch
            {
                exceptionThrown = true;
            }

            // Act & Assert
            Assert.IsFalse(exceptionThrown, "UpdateAutoLoadMenu should not throw an exception.");

            // Arrange
            _mainForm.AutoLoadConfigs = new Dictionary<string, string>
            {
                { "NONE:No aircraft detected", "Path/To/TestConfig1" },
            };


            try
            {
                // Act
                _mainForm.UpdateAutoLoadMenu();
            }
            catch
            {
                exceptionThrown = true;
            }

            // Act & Assert
            Assert.IsFalse(exceptionThrown, "UpdateAutoLoadMenu should not throw an exception.");
        }

        [TestMethod()]
        public async Task RecentFilesRemove_ViaCommandMainMenu_ShouldRemoveFromBothLists()
        {
            // Arrange
            _mainForm.InitializeExecutionManager();
            await _mainForm.InitializeProjectListManagerAsync();

            var recentFilesCollection = new StringCollection
            {
                CreateTestFile("project1.mfproj"),
                CreateTestFile("project2.mfproj"),
                CreateTestFile("project3.mfproj")
            };

            var testFiles = new String[recentFilesCollection.Count];
            recentFilesCollection.CopyTo(testFiles, 0);

            Properties.Settings.Default.RecentFiles = recentFilesCollection;
            Properties.Settings.Default.Save();

            // Re-initialize after adding files
            await _mainForm.InitializeProjectListManagerAsync();

            // Create the command message
            var command = new CommandMainMenu
            {
                Action = CommandMainMenuAction.virtual_recent_remove,
                Index = 1  // Remove the middle entry (project2)
            };

            // Get the handler
            var handler = new MobiFlight.BrowserMessages.Incoming.Handler.CommandMainMenuHandler(_mainForm);

            // Act
            handler.Handle(command);

            // Assert - RecentFiles updated
            var recentFiles = Properties.Settings.Default.RecentFiles;
            Assert.HasCount(2, recentFiles, "Should have 2 files remaining in RecentFiles");
            Assert.AreEqual(testFiles[0], recentFiles[0]);
            Assert.AreEqual(testFiles[2], recentFiles[1]);
            Assert.DoesNotContain(testFiles[1], recentFiles, "Removed file should not be in RecentFiles");

            // Assert - ProjectList also updated
            var propertyInfo = typeof(MainForm).GetProperty("ProjectListManager", BindingFlags.NonPublic | BindingFlags.Instance);
            var projectListManager = propertyInfo.GetValue(_mainForm) as ProjectListManager;
            var projectFiles = projectListManager.GetProjects();

            Assert.HasCount(2, projectFiles, "Should have 2 files remaining in ProjectList");
            Assert.AreEqual(testFiles[0], projectFiles[0].FilePath);
            Assert.AreEqual(testFiles[2], projectFiles[1].FilePath);
            Assert.DoesNotContain(testFiles[1], projectFiles, "Removed file should not be in ProjectList");
        }
    }
}