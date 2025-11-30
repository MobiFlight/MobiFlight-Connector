using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MobiFlight.UI.Tests
{
    [TestClass()]
    public class MainFormTests
    {
        private class TestableMainForm : MainForm
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
        }

        private TestableMainForm _mainForm;

        public void InitializeExecutionManager()
        {
            var methodInfo = typeof(MainForm).GetMethod("InitializeExecutionManager", BindingFlags.NonPublic | BindingFlags.Instance);
            methodInfo.Invoke(_mainForm, new object[] { });
        }

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
            InitializeExecutionManager();
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
                    File.Delete(tempFilePath);
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
            InitializeExecutionManager();
            Assert.IsFalse(_mainForm.ProjectHasUnsavedChanges, "ProjectHasUnsavedChanges should be true after adding a new file.");

            // Act
            _mainForm.AddNewFileToProject();

            // Assert
            var mainFormTitle = _mainForm.Text;
            Assert.IsTrue(_mainForm.ProjectHasUnsavedChanges, "ProjectHasUnsavedChanges should be true after adding a new file.");
            Assert.IsTrue(mainFormTitle.Contains("*"), "Project title should indicate that there are unsaved changes.");
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
    }
}