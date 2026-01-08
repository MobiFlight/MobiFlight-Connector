using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base;
using MobiFlight.BrowserMessages.Incoming;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace MobiFlight.UI.Tests
{
    [TestClass()]
    public class MainFormTests
    {
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
        }

        private TestableMainForm _mainForm;

        [TestInitialize]
        public void SetUp()
        {
            // Initialize the MainForm
            _mainForm = new TestableMainForm();
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
        public void RecentFilesRemove_ViaCommandMainMenu()
        {
            // Arrange
            _mainForm.InitializeExecutionManager();

            var testFiles = new StringCollection
            {
                "C:\\project1.mfproj",
                "C:\\project2.mfproj",
                "C:\\project3.mfproj"
            };

            Properties.Settings.Default.RecentFiles = testFiles;
            Properties.Settings.Default.Save();

            // Create the command message
            var command = new CommandMainMenu
            {
                Action = CommandMainMenuAction.virtual_recent_remove,
                Index = 1  // Remove the middle entry
            };

            // Get the handler
            var handler = new MobiFlight.BrowserMessages.Incoming.Handler.CommandMainMenuHandler(_mainForm);

            // Act
            handler.Handle(command);

            // Assert
            var recentFiles = Properties.Settings.Default.RecentFiles;
            Assert.HasCount(2, recentFiles, "Should have 2 files remaining");
            Assert.AreEqual("C:\\project1.mfproj", recentFiles[0]);
            Assert.AreEqual("C:\\project3.mfproj", recentFiles[1]);
            Assert.DoesNotContain("C:\\project2.mfproj", recentFiles, "Removed file should not be in the list");
        }

        [TestMethod]
        public void FindMissingFiles_ReturnsMissingAndIgnoresExisting()
        {
            var existing = Path.GetTempFileName();
            var missing = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".mfproj");
            try
            {
                var inputs = new List<string> { existing, missing, "   " };
                var result = MainForm.CheckForMissingFiles(inputs);

                // missing path and whitespace entry are reported missing
                Assert.Contains(missing, result, "Expected missing file to be reported.");
                Assert.IsTrue(result.Any(x => string.IsNullOrWhiteSpace(x)), "Expected whitespace entry to be reported as missing.");
                // existing file should not be reported missing
                Assert.DoesNotContain(existing, result, "Existing file should not be reported missing.");
            }
            finally
            {
                // This sometimes fails in CI environments due to file locks.
                // Just continue because Delete is not critical for the test.
                try { File.Delete(existing); } catch { }
            }
        }

        [TestMethod]
        public void RemoveMissingFilesFromSettings_RemovesEntriesFromSettings()
        {
            var existing = Path.GetTempFileName();
            var missing = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".mfproj");

            try
            {
                // Prepare settings recent list
                Properties.Settings.Default.RecentFiles.Clear();
                Properties.Settings.Default.RecentFiles.Add(existing);
                Properties.Settings.Default.RecentFiles.Add(missing);
                Properties.Settings.Default.Save();

                // Call instance method without running ctor to avoid UI initialization
                var mainFormInstance = FormatterServices.GetUninitializedObject(typeof(MainForm));
                var mi = typeof(MainForm).GetMethod("RemoveMissingFilesFromSettings", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                Assert.IsNotNull(mi, "RemoveMissingFilesFromSettings method not found via reflection.");

                // Invoke with the missing list
                mi.Invoke(mainFormInstance, new object[] { new List<string> { missing } });

                var current = Properties.Settings.Default.RecentFiles.Cast<string>().ToList();

                Assert.Contains(existing, current, "Existing file should remain in settings.");
                Assert.DoesNotContain(missing, current, "Missing file should have been removed from settings.");
            }
            finally
            {
                // This sometimes fails in CI environments due to file locks.
                // Just continue because Delete is not critical for the test.
                try { File.Delete(existing); } catch { }

                // Cleanup settings to avoid test pollution
                Properties.Settings.Default.RecentFiles.Clear();
                Properties.Settings.Default.Save();
            }
        }
    }
}