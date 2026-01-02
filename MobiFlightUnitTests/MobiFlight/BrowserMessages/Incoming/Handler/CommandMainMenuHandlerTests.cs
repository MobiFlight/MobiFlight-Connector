using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.BrowserMessages.Incoming;
using MobiFlight.BrowserMessages.Incoming.Handler;
using MobiFlight.UI;
using MobiFlight.Base;
using System.IO;
using System;
using System.Reflection;
using Newtonsoft.Json;
using static MobiFlight.UI.Tests.MainFormTests;

namespace MobiFlightUnitTests.MobiFlight.BrowserMessages.Incoming.Handler
{
    [TestClass]
    public class CommandMainMenuHandlerTests
    {
        private TestableMainForm _mainForm;
        private CommandMainMenuHandler _handler;
        private string _tempFilePath;

        [TestInitialize]
        public void Setup()
        {
            _mainForm = new TestableMainForm();
            _mainForm.InitializeExecutionManager();
            _handler = new CommandMainMenuHandler(_mainForm);

            // Create a temp file for testing
            _tempFilePath = Path.Combine(Path.GetTempPath(), $"commandmainmenuhandler_tests_project_{Guid.NewGuid()}.mfproj");
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(_tempFilePath))
            {
                File.Delete(_tempFilePath);
            }
        }

        [TestMethod]
        public void Handle_ProjectEdit_CallsUpdateProjectSettings()
        {
            // Arrange - Create and save a project first
            var initialProject = new Project
            {
                Name = "Initial Project",
                Sim = "fsx",
                Features = new ProjectFeatures { FSUIPC = false, ProSim = false },
                Aircraft = new System.Collections.ObjectModel.ObservableCollection<string>() { "Cessna 172" }
            };
            _mainForm.CreateNewProject(initialProject);

            // Save the project to give it a file path
            var saveMethod = typeof(MainForm).GetMethod("SaveConfig", BindingFlags.NonPublic | BindingFlags.Instance);
            saveMethod.Invoke(_mainForm, new object[] { _tempFilePath });

            // Now prepare the updated project
            var updatedProject = new Project
            {
                Name = "Updated Project Name",
                Sim = "msfs",
                Features = new ProjectFeatures { FSUIPC = true, ProSim = false },
                Aircraft = new System.Collections.ObjectModel.ObservableCollection<string>() { "Boeing 737" }
            };

            var message = new CommandMainMenu
            {
                Action = CommandMainMenuAction.project_edit,
                Options = new CommandMainMenuOptions
                {
                    Project = updatedProject
                }
            };

            // Act
            _handler.Handle(message);

            // Assert - Verify the file was updated with the new values
            Assert.IsTrue(File.Exists(_tempFilePath), "Project file should exist");

            var fileContent = File.ReadAllText(_tempFilePath);
            var savedProject = JsonConvert.DeserializeObject<Project>(fileContent);

            Assert.AreEqual("Updated Project Name", savedProject.Name);
            Assert.AreEqual("msfs", savedProject.Sim);
            Assert.IsTrue(savedProject.Features.FSUIPC);
            Assert.IsFalse(savedProject.Features.ProSim);
            Assert.HasCount(1, savedProject.Aircraft);
            Assert.AreEqual("Boeing 737", savedProject.Aircraft[0]);
        }
    }
}