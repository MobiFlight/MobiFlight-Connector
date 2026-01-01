using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base;
using MobiFlight.Controllers;
using Moq;
using System.Collections.Generic;

namespace MobiFlight.Tests.Controllers
{
    [TestClass]
    public class ControllerBindingServiceTests
    {
        private Mock<IExecutionManager> mockExecutionManager;
        private Mock<MobiFlightCache> mockMobiFlightCache;
        private Mock<JoystickManager> mockJoystickManager;
        private Mock<MidiBoardManager> mockMidiBoardManager;
        private ControllerBindingService service;

        [TestInitialize]
        public void Setup()
        {
            mockExecutionManager = new Mock<IExecutionManager>();
            mockMobiFlightCache = new Mock<MobiFlightCache>();
            mockJoystickManager = new Mock<JoystickManager>();
            mockMidiBoardManager = new Mock<MidiBoardManager>();

            // Setup execution manager to return mocked caches
            mockExecutionManager.Setup(m => m.getMobiFlightModuleCache()).Returns(mockMobiFlightCache.Object);
            mockExecutionManager.Setup(m => m.GetJoystickManager()).Returns(mockJoystickManager.Object);
            mockExecutionManager.Setup(m => m.GetMidiBoardManager()).Returns(mockMidiBoardManager.Object);

            //// Default: no connected controllers
            mockMobiFlightCache.Setup(m => m.GetModules()).Returns(new List<MobiFlightModule>());
            mockJoystickManager.Setup(j => j.GetJoysticks()).Returns(new List<Joystick>());
            mockMidiBoardManager.Setup(m => m.GetMidiBoards()).Returns(new List<MidiBoard>());

            service = new ControllerBindingService(mockExecutionManager.Object);
        }

        #region Scenario 1: Exact Match Tests

        [TestMethod]
        public void AnalyzeProjectBindings_Scenario1_ExactMatch_ReturnsMatch()
        {
            // Arrange
            var project = CreateProjectWithController("MyBoard/ SN-1234567890");
            SetupConnectedController("MyBoard", "SN-1234567890");

            // Act
            var result = service.AnalyzeProjectBindings(project);

            // Assert
            Assert.HasCount(1, result);
            var binding = result.Find(b => b.OriginalController == "MyBoard/ SN-1234567890");
            Assert.AreEqual(ControllerBindingStatus.Match, binding.Status);
            Assert.AreEqual("MyBoard/ SN-1234567890", binding.BoundController);
            Assert.AreEqual("MyBoard/ SN-1234567890", binding.OriginalController);
        }

        [TestMethod]
        public void PerformAutoBinding_Scenario1_ExactMatch_NoChanges()
        {
            // Arrange
            var project = CreateProjectWithController("MyBoard/ SN-1234567890");
            var originalSerial = project.ConfigFiles[0].ConfigItems[0].ModuleSerial;
            SetupConnectedController("MyBoard", "SN-1234567890");

            // Act
            var result = service.PerformAutoBinding(project);

            // Assert
            var binding = result.Find(b => b.OriginalController == "MyBoard/ SN-1234567890");
            Assert.AreEqual(ControllerBindingStatus.Match, binding.Status);
            Assert.AreEqual(originalSerial, project.ConfigFiles[0].ConfigItems[0].ModuleSerial,
                "Serial should not change for exact match");
        }

        #endregion

        #region Scenario 2: Partial Match - Serial Differs Tests

        [TestMethod]
        public void AnalyzeProjectBindings_Scenario2_SerialDiffers_ReturnsAutoBound()
        {
            // Arrange
            var project = CreateProjectWithController("X1-Pro/ SN-OLD123");
            SetupConnectedController("X1-Pro", "SN-NEW456");

            // Act
            var result = service.AnalyzeProjectBindings(project);

            // Assert
            Assert.HasCount(1, result);
            var binding = result.Find(b => b.OriginalController == "X1-Pro/ SN-OLD123");
            Assert.AreEqual(ControllerBindingStatus.AutoBind, binding.Status);
            Assert.AreEqual("X1-Pro/ SN-NEW456", binding.BoundController);
            Assert.AreEqual("X1-Pro/ SN-OLD123", binding.OriginalController);
        }

        [TestMethod]
        public void PerformAutoBinding_Scenario2_SerialDiffers_UpdatesSerial()
        {
            // Arrange
            var project = CreateProjectWithController("X1-Pro/ SN-OLD123");
            SetupConnectedController("X1-Pro", "SN-NEW456");

            // Act
            var result = service.PerformAutoBinding(project);

            // Assert
            var binding = result.Find(b => b.OriginalController == "X1-Pro/ SN-OLD123");
            Assert.AreEqual(ControllerBindingStatus.AutoBind, binding.Status);
            Assert.AreEqual("X1-Pro/ SN-NEW456", project.ConfigFiles[0].ConfigItems[0].ModuleSerial,
                "Serial should be updated to new serial");
        }

        #endregion

        #region Scenario 3: Partial Match - Name Differs Tests

        [TestMethod]
        public void AnalyzeProjectBindings_Scenario3_NameDiffers_ReturnsAutoBound()
        {
            // Arrange - name changed but serial is same
            var project = CreateProjectWithController("OldBoardName/ SN-1234567890");
            SetupConnectedController("NewBoardName", "SN-1234567890");

            // Act
            var result = service.AnalyzeProjectBindings(project);

            // Assert
            Assert.HasCount(1, result);
            var binding = result.Find(b => b.OriginalController == "OldBoardName/ SN-1234567890");
            Assert.AreEqual("NewBoardName/ SN-1234567890", binding.BoundController);
            Assert.AreEqual(ControllerBindingStatus.AutoBind, binding.Status);
            Assert.AreEqual("OldBoardName/ SN-1234567890", binding.OriginalController);
        }

        [TestMethod]
        public void PerformAutoBinding_Scenario3_NameDiffers_UpdatesName()
        {
            // Arrange
            var project = CreateProjectWithController("OldBoardName/ SN-1234567890");
            SetupConnectedController("NewBoardName", "SN-1234567890");

            // Act
            var result = service.PerformAutoBinding(project);

            // Assert
            var binding = result.Find(b => b.OriginalController == "OldBoardName/ SN-1234567890");
            Assert.AreEqual(ControllerBindingStatus.AutoBind, binding.Status);
            Assert.AreEqual("NewBoardName/ SN-1234567890", project.ConfigFiles[0].ConfigItems[0].ModuleSerial,
                "Name should be updated");
        }

        #endregion

        #region Scenario 4: Missing Controller Tests

        [TestMethod]
        public void AnalyzeProjectBindings_Scenario4_Missing_ReturnsMissing()
        {
            // Arrange
            var project = CreateProjectWithController("X1-Pro/ SN-1234567890");
            SetupConnectedController("DifferentBoard", "SN-9999999999");

            // Act
            var result = service.AnalyzeProjectBindings(project);

            // Assert
            Assert.HasCount(1, result);
            var binding = result.Find(b => b.OriginalController == "X1-Pro/ SN-1234567890");
            Assert.AreEqual(ControllerBindingStatus.Missing, binding.Status);
        }

        [TestMethod]
        public void PerformAutoBinding_Scenario4_Missing_NoChanges()
        {
            // Arrange
            var project = CreateProjectWithController("X1-Pro/ SN-1234567890");
            var originalSerial = project.ConfigFiles[0].ConfigItems[0].ModuleSerial;
            SetupConnectedController("DifferentBoard", "SN-9999999999");

            // Act
            var result = service.PerformAutoBinding(project);

            // Assert

            var binding = result.Find(b => b.OriginalController == "X1-Pro/ SN-1234567890");
            Assert.AreEqual(ControllerBindingStatus.Missing, binding.Status);
            Assert.AreEqual(originalSerial, project.ConfigFiles[0].ConfigItems[0].ModuleSerial,
                "Serial should not change when missing");
        }

        #endregion

        #region Scenario 5: Multiple Devices - Requires Manual Bind Tests

        [TestMethod]
        public void AnalyzeProjectBindings_Scenario5_MultipleDevices_RequiresManualBind()
        {
            // Arrange
            var project = CreateProjectWithController("Joystick X / JS-999999");
            SetupConnectedJoysticks(new[]
            {
                ("Joystick X", "JS-111111"),
                ("Joystick X", "JS-222222")
            });

            // Act
            var result = service.AnalyzeProjectBindings(project);

            // Assert
            Assert.HasCount(1, result);
            var binding = result.Find(b => b.OriginalController == "Joystick X / JS-999999");
            Assert.AreEqual(ControllerBindingStatus.RequiresManualBind, binding.Status);
        }

        [TestMethod]
        public void PerformAutoBinding_Scenario5_MultipleDevices_NoAutoBind()
        {
            // Arrange
            var project = CreateProjectWithController("Joystick X / JS-999999");
            var originalSerial = project.ConfigFiles[0].ConfigItems[0].ModuleSerial;
            SetupConnectedJoysticks(new[]
            {
                ("Joystick X", "JS-111111"),
                ("Joystick X", "JS-222222")
            });

            // Act
            var result = service.PerformAutoBinding(project);

            // Assert
            var binding = result.Find(b => b.OriginalController == "Joystick X / JS-999999");
            Assert.AreEqual(ControllerBindingStatus.RequiresManualBind, binding.Status);
            Assert.AreEqual(originalSerial, project.ConfigFiles[0].ConfigItems[0].ModuleSerial,
                "Serial should not auto-bind when multiple matches exist");
        }

        #endregion

        #region Scenario 6: Multiple Profiles - One Connected Tests

        [TestMethod]
        public void AnalyzeProjectBindings_Scenario6_OneProfile_TwoController_OneConnected_ManualRebindRequired()
        {
            // Arrange
            var project = CreateProjectWithControllers(new[]
            {
                "X1-Pro/ SN-1111111111",
                "X1-Pro/ SN-2222222222"
            });
            SetupConnectedController("X1-Pro", "SN-9876543210");

            // Act
            var result = service.AnalyzeProjectBindings(project);

            // Assert
            Assert.HasCount(2, result);
            var binding1 = result.Find(b => b.OriginalController == "X1-Pro/ SN-1111111111");
            var binding2 = result.Find(b => b.OriginalController == "X1-Pro/ SN-2222222222");
            Assert.AreEqual(ControllerBindingStatus.RequiresManualBind, binding1.Status);
            Assert.AreEqual(ControllerBindingStatus.RequiresManualBind, binding2.Status);

            // Act
            var bindingResult = service.PerformAutoBinding(project);
            Assert.HasCount(2, bindingResult);
            Assert.AreEqual("X1-Pro/ SN-1111111111", project.ConfigFiles[0].ConfigItems[0].ModuleSerial);
            Assert.AreEqual("X1-Pro/ SN-2222222222", project.ConfigFiles[0].ConfigItems[1].ModuleSerial);
        }

        [TestMethod]
        public void AnalyzeProjectBindings_Scenario6_OneProfile_TwoController_TwoConnected_RequiresManualBind()
        {
            // Arrange
            var project = CreateProjectWithControllers(new[]
            {
                "X1-Pro/ SN-1111111111",
                "X1-Pro/ SN-2222222222"
            });
            SetupConnectedController("X1-Pro", "SN-9876543210");
            SetupConnectedController("X1-Pro", "SN-0123456789");

            // Act
            var result = service.AnalyzeProjectBindings(project);

            // Assert
            Assert.HasCount(2, result);
            var binding1 = result.Find(b => b.OriginalController == "X1-Pro/ SN-1111111111");
            var binding2 = result.Find(b => b.OriginalController == "X1-Pro/ SN-2222222222");
            Assert.AreEqual(ControllerBindingStatus.RequiresManualBind, binding1.Status);
            Assert.AreEqual(ControllerBindingStatus.RequiresManualBind, binding2.Status);

            // Act
            var bindingResult = service.PerformAutoBinding(project);
            Assert.HasCount(2, bindingResult);
            Assert.AreEqual("X1-Pro/ SN-1111111111", project.ConfigFiles[0].ConfigItems[0].ModuleSerial);
            Assert.AreEqual("X1-Pro/ SN-2222222222", project.ConfigFiles[0].ConfigItems[1].ModuleSerial);
        }

        [TestMethod]
        public void AnalyzeProjectBindings_Scenario6_TwoProfiles_OneController_OneConnected_AutoBindsBoth()
        {
            // Arrange
            var project = CreateProjectWithControllers(new[]
            {
                "X1-Pro/ SN-1111111111"
            });

            project.ConfigFiles.Add(CreateConfigFileWithControllers(new[]
            {
                "X1-Pro/ SN-2222222222"
            }));
            SetupConnectedController("X1-Pro", "SN-9876543210");

            // Act
            var result = service.AnalyzeProjectBindings(project);

            // Assert
            Assert.HasCount(2, result);
            var binding1 = result.Find(b => b.OriginalController == "X1-Pro/ SN-1111111111");
            var binding2 = result.Find(b => b.OriginalController == "X1-Pro/ SN-2222222222");
            Assert.AreEqual(ControllerBindingStatus.AutoBind, binding1.Status);
            Assert.AreEqual(ControllerBindingStatus.AutoBind, binding2.Status);

            // Act
            var bindingResult = service.PerformAutoBinding(project);
            Assert.HasCount(2, bindingResult);
            Assert.AreEqual("X1-Pro/ SN-9876543210", project.ConfigFiles[0].ConfigItems[0].ModuleSerial);
            Assert.AreEqual("X1-Pro/ SN-9876543210", project.ConfigFiles[1].ConfigItems[0].ModuleSerial);
        }

        [TestMethod]
        public void AnalyzeProjectBindings_Scenario6_OneProfile_TwoController_TwoConnected_AutoBindsNone()
        {
            // Arrange
            var project = CreateProjectWithControllers(new[]
            {
                "X1-Pro/ SN-1111111111",
                "X1-Pro/ SN-2222222222"
            });
            SetupConnectedController("X1-Pro", "SN-9876543210");
            SetupConnectedController("X1-Pro", "SN-0123456789");

            // Act
            var result = service.AnalyzeProjectBindings(project);

            // Assert
            Assert.HasCount(2, result);
            var binding1 = result.Find(b => b.OriginalController == "X1-Pro/ SN-1111111111");
            var binding2 = result.Find(b => b.OriginalController == "X1-Pro/ SN-2222222222");
            Assert.AreEqual(ControllerBindingStatus.RequiresManualBind, binding1.Status);
            Assert.AreEqual(ControllerBindingStatus.RequiresManualBind, binding2.Status);

            // Act
            var bindingResult = service.PerformAutoBinding(project);
            Assert.HasCount(2, bindingResult);
            Assert.AreEqual("X1-Pro/ SN-1111111111", project.ConfigFiles[0].ConfigItems[0].ModuleSerial);
            Assert.AreEqual("X1-Pro/ SN-2222222222", project.ConfigFiles[0].ConfigItems[1].ModuleSerial);
        }

        #endregion

        #region Scenario 7: Two Controllers - One Missing Tests

        [TestMethod]
        public void AnalyzeProjectBindings_Scenario7_TwoControllers_OneMissing()
        {
            // Arrange
            var project = CreateProjectWithControllers(new[]
            {
                "X1-Pro/ SN-1111111111",
                "X1-Pro/ SN-2222222222"
            });
            SetupConnectedController("X1-Pro", "SN-1111111111");

            // Act
            var result = service.AnalyzeProjectBindings(project);

            // Assert
            Assert.HasCount(2, result);
            var binding1 = result.Find(b => b.OriginalController == "X1-Pro/ SN-1111111111");
            var binding2 = result.Find(b => b.OriginalController == "X1-Pro/ SN-2222222222");
            Assert.AreEqual(ControllerBindingStatus.Match, binding1.Status);
            Assert.AreEqual(ControllerBindingStatus.Missing, binding2.Status);
        }

        [TestMethod]
        public void PerformAutoBinding_Scenario7_TwoControllers_OneMissing_NoChanges()
        {
            // Arrange
            var project = CreateProjectWithControllers(new[]
            {
                "X1-Pro/ SN-1111111111",
                "X1-Pro/ SN-2222222222"
            });
            SetupConnectedController("X1-Pro", "SN-1111111111");

            // Act
            service.PerformAutoBinding(project);

            // Assert
            Assert.AreEqual("X1-Pro/ SN-1111111111", project.ConfigFiles[0].ConfigItems[0].ModuleSerial,
                "Connected controller should not change");
            Assert.AreEqual("X1-Pro/ SN-2222222222", project.ConfigFiles[0].ConfigItems[1].ModuleSerial,
                "Missing controller should not change");
        }

        #endregion

        #region Multiple Config Files Tests

        [TestMethod]
        public void AnalyzeProjectBindings_MultipleConfigFiles_AnalyzesAll()
        {
            // Arrange
            var project = new Project();
            project.ConfigFiles.Add(CreateConfigFileWithController("Board1/ SN-111"));
            project.ConfigFiles.Add(CreateConfigFileWithController("Board2/ SN-222"));

            SetupConnectedControllers(new[]
            {
                ("Board1", "SN-111"),
                ("Board2", "SN-333")
            });

            // Act
            var result = service.AnalyzeProjectBindings(project);

            // Assert
            Assert.HasCount(2, result);
            var binding1 = result.Find(b => b.OriginalController == "Board1/ SN-111");
            var binding2 = result.Find(b => b.OriginalController == "Board2/ SN-222");
            Assert.AreEqual(ControllerBindingStatus.Match, binding1.Status);
            Assert.AreEqual(ControllerBindingStatus.AutoBind, binding2.Status);
        }

        [TestMethod]
        public void PerformAutoBinding_MultipleConfigFiles_UpdatesAll()
        {
            // Arrange
            var project = new Project();
            project.ConfigFiles.Add(CreateConfigFileWithController("Board1/ SN-111"));
            project.ConfigFiles.Add(CreateConfigFileWithController("Board2/ SN-222"));

            SetupConnectedControllers(new[]
            {
                ("Board1", "SN-111"),
                ("Board2", "SN-333")
            });

            // Act
            service.PerformAutoBinding(project);

            // Assert
            Assert.AreEqual("Board1/ SN-111", project.ConfigFiles[0].ConfigItems[0].ModuleSerial);
            Assert.AreEqual("Board2/ SN-333", project.ConfigFiles[1].ConfigItems[0].ModuleSerial);
        }

        #endregion

        #region Edge Cases Tests

        [TestMethod]
        public void AnalyzeProjectBindings_EmptyProject_ReturnsEmptyDictionary()
        {
            // Arrange
            var project = new Project();

            // Act
            var result = service.AnalyzeProjectBindings(project);

            // Assert
            Assert.IsEmpty(result);
        }

        [TestMethod]
        public void AnalyzeProjectBindings_NoConnectedControllers_AllMissing()
        {
            // Arrange
            var project = CreateProjectWithControllers(new[]
            {
                "Board1/ SN-111",
                "Board2/ SN-222"
            });
            // No controllers connected (default mock setup)

            // Act
            var result = service.AnalyzeProjectBindings(project);

            // Assert
            Assert.HasCount(2, result);
            var binding1 = result.Find(b => b.OriginalController == "Board1/ SN-111");
            var binding2 = result.Find(b => b.OriginalController == "Board2/ SN-222");
            Assert.AreEqual(ControllerBindingStatus.Missing, binding1.Status);
            Assert.AreEqual(ControllerBindingStatus.Missing, binding2.Status);
        }

        [TestMethod]
        public void AnalyzeProjectBindings_IgnoresEmptySerials()
        {
            // Arrange
            var project = CreateProjectWithControllers(new[]
            {
                "",
                "-",
                "ValidBoard/ SN-123"
            });
            SetupConnectedController("ValidBoard", "SN-123");

            // Act
            var result = service.AnalyzeProjectBindings(project);

            // Assert
            Assert.HasCount(1, result);
            Assert.AreEqual("ValidBoard/ SN-123", result[0].OriginalController);
        }

        #endregion

        #region Mixed Controller Types Tests

        [TestMethod]
        public void AnalyzeProjectBindings_MixedControllerTypes_AnalyzesAll()
        {
            // Arrange
            var project = CreateProjectWithControllers(new[]
            {
                "MyBoard/ SN-111",           // MobiFlight
                "Joystick X / JS-222",        // Joystick
                "MIDI Controller / MI-333"    // MIDI
            });

            var module = new Mock<MobiFlightModule>("COM1", new Board() { Info = new Info() { FriendlyName = "MyBoard" } });
            module.Setup(m => m.Name).Returns("MyBoard");
            module.Setup(m => m.Serial).Returns("SN-111");

            var joystick = new Mock<Joystick>(null, new JoystickDefinition() { InstanceName = "Joystick X" });
            joystick.Setup(j => j.Name).Returns("Joystick X");
            joystick.Setup(j => j.Serial).Returns("JS-222");

            var midiBoard = new Mock<MidiBoard>(null, null, "MIDI Controller", new MidiBoardDefinition() { InstanceName = "MIDI Controller" });
            midiBoard.Setup(m => m.Name).Returns("MIDI Controller");
            midiBoard.Setup(m => m.Serial).Returns("MI-333");

            mockMobiFlightCache.Setup(m => m.GetModules()).Returns(new List<MobiFlightModule> { module.Object });
            mockJoystickManager.Setup(j => j.GetJoysticks()).Returns(new List<Joystick> { joystick.Object });
            mockMidiBoardManager.Setup(m => m.GetMidiBoards()).Returns(new List<MidiBoard> { midiBoard.Object });

            // Act
            var result = service.AnalyzeProjectBindings(project);

            // Assert
            Assert.HasCount(3, result);
            var binding1 = result.Find(b => b.OriginalController == "MyBoard/ SN-111");
            var binding2 = result.Find(b => b.OriginalController == "Joystick X / JS-222");
            var binding3 = result.Find(b => b.OriginalController == "MIDI Controller / MI-333");
            Assert.AreEqual(ControllerBindingStatus.Match, binding1.Status);
            Assert.AreEqual(ControllerBindingStatus.Match, binding2.Status);
            Assert.AreEqual(ControllerBindingStatus.Match, binding3.Status);
        }

        #endregion

        #region Helper Methods

        private Project CreateProjectWithController(string moduleSerial)
        {
            return CreateProjectWithControllers(new[] { moduleSerial });
        }

        private Project CreateProjectWithControllers(string[] moduleSerials)
        {
            var project = new Project();
            var configFile = CreateConfigFileWithControllers(moduleSerials);
            project.ConfigFiles.Add(configFile);
            project.Sim = "msfs";
            return project;
        }

        private ConfigFile CreateConfigFileWithController(string moduleSerial)
        {
            return CreateConfigFileWithControllers(new[] { moduleSerial });
        }

        private ConfigFile CreateConfigFileWithControllers(string[] moduleSerials)
        {
            var configFile = new ConfigFile
            {
                Label = "Test Config",
                EmbedContent = true
            };

            foreach (var serial in moduleSerials)
            {
                configFile.ConfigItems.Add(new OutputConfigItem
                {
                    ModuleSerial = serial,
                    Active = true,
                    GUID = System.Guid.NewGuid().ToString()
                });
            }

            return configFile;
        }

        private void SetupConnectedController(string name, string serial)
        {
            SetupConnectedControllers(new[] { (name, serial) });
        }

        private void SetupConnectedControllers((string name, string serial)[] controllers)
        {
            var modules = new List<MobiFlightModule>();
            foreach (var (name, serial) in controllers)
            {
                var module = new Mock<MobiFlightModule>("com5", new Board() { Info = new Info() { FriendlyName = name } });
                module.Setup(m => m.Name).Returns(name);
                module.Setup(m => m.Serial).Returns(serial);
                modules.Add(module.Object);
            }
            mockMobiFlightCache.Setup(m => m.GetModules()).Returns(modules);
        }

        private void SetupConnectedJoysticks((string name, string serial)[] joysticks)
        {
            var joystickList = new List<Joystick>();
            foreach (var (name, serial) in joysticks)
            {
                var joystick = new Mock<Joystick>(null, new JoystickDefinition() { InstanceName = name });
                joystick.Setup(j => j.Name).Returns(name);
                joystick.Setup(j => j.Serial).Returns(serial);
                joystickList.Add(joystick.Object);
            }
            mockJoystickManager.Setup(j => j.GetJoysticks()).Returns(joystickList);
        }

        #endregion
    }
}