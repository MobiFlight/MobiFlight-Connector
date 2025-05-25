using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Windows.Forms;

namespace MobiFlight.UI.Tests
{
    [TestClass()]
    public class MainFormTests
    {
        private MainForm _mainForm;
        private T GetPrivateField<T>(object obj, string fieldName)
        {
            var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)field?.GetValue(obj);
        }

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
            var saveButton = GetPrivateField<ToolStripButton>(_mainForm, "saveToolStripButton");

            // bring it into dirty state
            _mainForm.AddNewFileToProject();
            Assert.IsTrue(saveButton.Enabled, "Save button should be enabled after adding a new file to project.");
            
            // Act
            _mainForm.CreateNewProject();

            // Assert
            Assert.IsNotNull(saveButton, "Save button could not be accessed.");
            Assert.IsFalse(saveButton.Enabled, "Save button should be disabled after creating a new project.");

            var mainFormTitle = _mainForm.Text;
            var expectedTitle = $"New Project - MobiFlight Connector - {MainForm.DisplayVersion()}";
            Assert.AreEqual(expectedTitle, mainFormTitle);
        }


        [TestMethod()]
        public void AddNewFileToProjectTest()
        {
            // Arrange
            var saveButton = GetPrivateField<ToolStripButton>(_mainForm, "saveToolStripButton");
            Assert.IsNotNull(saveButton, "Save button could not be accessed.");

            // Act
            _mainForm.AddNewFileToProject();

            // Assert
            var mainFormTitle = _mainForm.Text;
            Assert.IsNotNull(saveButton, "Save button could not be accessed.");
            Assert.IsTrue(saveButton.Enabled, "Save button should be enabled after adding a new file.");
            Assert.IsTrue(mainFormTitle.Contains("*"), "Project title should indicate that there are unsaved changes.");
        }
    }
}