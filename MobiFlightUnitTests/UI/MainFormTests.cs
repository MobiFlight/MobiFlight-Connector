using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Windows.Forms;

namespace MobiFlight.UI.Tests
{
    [TestClass()]
    public class MainFormTests
    {
        private MainForm _mainForm;

        [TestInitialize]
        public void SetUp()
        {
            // Initialize the MainForm
            _mainForm = new MainForm();
            var methodInfo = typeof(MainForm).GetMethod("InitializeExecutionManager", BindingFlags.NonPublic | BindingFlags.Instance);
            methodInfo.Invoke(_mainForm, new object[] { });
        }

        [TestMethod()]
        public void CreateNewProjectTest()
        {
            // Arrange
            Assert.IsFalse(_mainForm.ProjectHasUnsavedChanges, "ProjectHasUnsavedChanges should be false when starting with a fresh project.");

            // bring it into dirty state
            _mainForm.AddNewFileToProject();
            Assert.IsTrue(_mainForm.ProjectHasUnsavedChanges, "ProjectHasUnsavedChanges should be True after adding a new file to project.");
            
            // Act
            _mainForm.CreateNewProject();

            // Assert
            Assert.IsFalse(_mainForm.ProjectHasUnsavedChanges, "ProjectHasUnsavedChanges should be false when starting with a fresh project.");

            var mainFormTitle = _mainForm.Text;
            var expectedTitle = $"New Project - MobiFlight Connector - {MainForm.DisplayVersion()}";
            Assert.AreEqual(expectedTitle, mainFormTitle);
        }


        [TestMethod()]
        public void AddNewFileToProjectTest()
        {
            // Arrange
            Assert.IsFalse(_mainForm.ProjectHasUnsavedChanges, "ProjectHasUnsavedChanges should be true after adding a new file.");

            // Act
            _mainForm.AddNewFileToProject();

            // Assert
            var mainFormTitle = _mainForm.Text;
            Assert.IsTrue(_mainForm.ProjectHasUnsavedChanges, "ProjectHasUnsavedChanges should be true after adding a new file.");
            Assert.IsTrue(mainFormTitle.Contains("*"), "Project title should indicate that there are unsaved changes.");
        }
    }
}