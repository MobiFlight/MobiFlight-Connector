using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
            _mainForm.AddNewFileToProject();

            // Act
            _mainForm.CreateNewProject();

            // Assert
            Assert.IsNotNull(saveButton, "Save button could not be accessed.");
            Assert.IsFalse(saveButton.Enabled, "Save button should be disabled after creating a new project.");
        }


        [TestMethod()]
        public void AddNewFileToProjectTest()
        {
            // Arrange
            var saveButton = GetPrivateField<ToolStripButton>(_mainForm, "saveToolStripButton");

            // Act
            _mainForm.AddNewFileToProject();
            // Assert
            Assert.IsNotNull(saveButton, "Save button could not be accessed.");
            Assert.IsTrue(saveButton.Enabled, "Save button should be enabled after adding a new file.");
        }
    }
}