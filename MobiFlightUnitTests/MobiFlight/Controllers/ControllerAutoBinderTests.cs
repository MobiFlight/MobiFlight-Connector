using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base;
using MobiFlight.Controllers;
using SharpDX;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.Tests.Controllers
{
    [TestClass]
    public class ControllerAutoBinderTests
    {
        #region Scenario Tests

        [TestMethod]
        public void Scenario1_ExactMatch_ReturnsMatchAndNoChanges()
        {
            // Arrange
            var connectedControllers = new List<string> { "MyBoard # / SN-1234567890" };
            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("MyBoard # / SN-1234567890")
            };
            var binder = new ControllerAutoBinder(connectedControllers);
            var existingBindings = new List<ControllerBinding>();

            // Act
            var results = binder.AnalyzeBindings(configItems, existingBindings);
            var serialMappings = binder.ApplyAutoBinding(configItems, results);

            // Assert
            Assert.HasCount(1, results);
            var binding = results.Find(b => b.OriginalController == "MyBoard # / SN-1234567890");
            Assert.AreEqual(ControllerBindingStatus.Match, binding.Status);
            Assert.IsEmpty(serialMappings);
            Assert.AreEqual("MyBoard # / SN-1234567890", configItems[0].ModuleSerial);
            Assert.AreEqual("MyBoard # / SN-1234567890", binding.BoundController);
        }

        [TestMethod]
        public void Scenario2_SerialDiffers_ReturnsAutoBoundAndUpdatesSerial()
        {
            // Arrange
            var connectedControllers = new List<string> { "X1-Pro # / SN-NEW456" };
            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("X1-Pro # / SN-OLD123")
            };
            var binder = new ControllerAutoBinder(connectedControllers);
            var existingBindings = new List<ControllerBinding>();

            // Act
            var results = binder.AnalyzeBindings(configItems, existingBindings);
            var serialMappings = binder.ApplyAutoBinding(configItems, results);

            // Assert
            Assert.HasCount(1, results);
            var binding = results.Find(b => b.OriginalController == "X1-Pro # / SN-OLD123");
            Assert.AreEqual(ControllerBindingStatus.AutoBind, binding.Status);
            Assert.HasCount(1, serialMappings);
            Assert.AreEqual("X1-Pro # / SN-OLD123", serialMappings[0].OriginalController);
            Assert.AreEqual("X1-Pro # / SN-NEW456", configItems[0].ModuleSerial);
        }

        [TestMethod]
        public void Scenario3_NameDiffers_ReturnsAutoBoundAndUpdatesName()
        {
            // Arrange
            var connectedControllers = new List<string> { "NewBoardName # / SN-1234567890" };
            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("OldBoardName # / SN-1234567890")
            };
            var binder = new ControllerAutoBinder(connectedControllers);
            var existingBindings = new List<ControllerBinding>();

            // Act
            var results = binder.AnalyzeBindings(configItems, existingBindings);
            var serialMappings = binder.ApplyAutoBinding(configItems, results);

            // Assert
            Assert.HasCount(1, results);
            var binding = results.Find(b => b.OriginalController == "OldBoardName # / SN-1234567890");
            Assert.AreEqual(ControllerBindingStatus.AutoBind, binding.Status);
            Assert.HasCount(1, serialMappings);
            Assert.AreEqual("NewBoardName # / SN-1234567890", serialMappings[0].BoundController);
            Assert.AreEqual("NewBoardName # / SN-1234567890", configItems[0].ModuleSerial);
        }

        [TestMethod]
        public void Scenario4_Missing_ReturnsMissingAndNoChanges()
        {
            // Arrange
            var connectedControllers = new List<string> { "DifferentBoard # / SN-9999" };
            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("X1-Pro # / SN-1234")
            };
            var binder = new ControllerAutoBinder(connectedControllers);
            var existingBindings = new List<ControllerBinding>();

            // Act
            var results = binder.AnalyzeBindings(configItems, existingBindings);
            var serialMappings = binder.ApplyAutoBinding(configItems, results);

            // Assert
            Assert.HasCount(1, results);
            var binding = results.Find(b => b.OriginalController == "X1-Pro # / SN-1234");
            Assert.AreEqual(ControllerBindingStatus.Missing, binding.Status);
            Assert.IsEmpty(serialMappings);
            Assert.AreEqual("X1-Pro # / SN-1234", configItems[0].ModuleSerial);
        }

        [TestMethod]
        public void Scenario5_MultipleControllerMatches_RequiresManualBindAndNoChanges()
        {
            // Arrange
            var connectedControllers = new List<string>
            {
                "Joystick X # / JS-111111",
                "Joystick X # / JS-222222"
            };
            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("Joystick X # / JS-999999")
            };
            var binder = new ControllerAutoBinder(connectedControllers);
            var existingBindings = new List<ControllerBinding>();

            // Act
            var results = binder.AnalyzeBindings(configItems, existingBindings);
            var serialMappings = binder.ApplyAutoBinding(configItems, results);

            // Assert
            Assert.HasCount(1, results);
            var binding = results.Find(b => b.OriginalController == "Joystick X # / JS-999999");
            Assert.AreEqual(ControllerBindingStatus.RequiresManualBind, binding.Status);
            Assert.IsEmpty(serialMappings);
            Assert.AreEqual("Joystick X # / JS-999999", configItems[0].ModuleSerial);
        }

        [TestMethod]
        public void Scenario5_MultipleConfigMatches_RequiresManualBindAndNoChanges()
        {
            // Arrange
            var connectedControllers = new List<string>
            {
                "Joystick X # / JS-111111"
            };
            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("Joystick X # / JS-222222"),
                CreateConfigItem("Joystick X # / JS-333333")
            };
            var binder = new ControllerAutoBinder(connectedControllers);
            var existingBindings = new List<ControllerBinding>();

            // Act
            var results = binder.AnalyzeBindings(configItems, existingBindings);
            var serialMappings = binder.ApplyAutoBinding(configItems, results);

            // Assert
            Assert.HasCount(2, results);
            var binding = results.Find(b => b.OriginalController == "Joystick X # / JS-222222");
            Assert.AreEqual(ControllerBindingStatus.RequiresManualBind, binding.Status);
            Assert.IsEmpty(serialMappings);
            Assert.AreEqual("Joystick X # / JS-222222", configItems[0].ModuleSerial);
            
            binding = results.Find(b => b.OriginalController == "Joystick X # / JS-333333");
            Assert.AreEqual(ControllerBindingStatus.RequiresManualBind, binding.Status);
            Assert.IsEmpty(serialMappings);
            Assert.AreEqual("Joystick X # / JS-333333", configItems[1].ModuleSerial);
        }

        #endregion

        #region Multiple Config Items Tests

        [TestMethod]
        public void AnalyzeBindings_MultipleConfigItems_AnalyzesAll()
        {
            // Arrange
            var connectedControllers = new List<string>
            {
                "Board1 # / SN-111",
                "Board2 # / SN-222"
            };
            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("Board1 # / SN-111"),
                CreateConfigItem("Board2 # / SN-OLD"),
                CreateConfigItem("Board3 # / SN-333")
            };
            var binder = new ControllerAutoBinder(connectedControllers);
            var existingBindings = new List<ControllerBinding>();

            // Act
            var results = binder.AnalyzeBindings(configItems, existingBindings);

            // Assert
            Assert.HasCount(3, results);
            var binding1 = results.Find(b => b.OriginalController == "Board1 # / SN-111");
            var binding2 = results.Find(b => b.OriginalController == "Board2 # / SN-OLD");
            var binding3 = results.Find(b => b.OriginalController == "Board3 # / SN-333");

            Assert.AreEqual(ControllerBindingStatus.Match, binding1.Status);
            Assert.AreEqual(ControllerBindingStatus.AutoBind, binding2.Status);
            Assert.AreEqual(ControllerBindingStatus.Missing, binding3.Status);
        }

        [TestMethod]
        public void ApplyAutoBinding_MultipleConfigItems_UpdatesOnlyAutoBound()
        {
            // Arrange
            var connectedControllers = new List<string>
            {
                "Board1 # / SN-111",
                "Board2 # / SN-222"
            };
            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("Board1 # / SN-111"),
                CreateConfigItem("Board2 # / SN-OLD"),
                CreateConfigItem("Board3 # / SN-333")
            };
            var existingBindings = new List<ControllerBinding>();

            var binder = new ControllerAutoBinder(connectedControllers);
            var bindingStatus = binder.AnalyzeBindings(configItems, existingBindings);

            // Act
            var serialMappings = binder.ApplyAutoBinding(configItems, bindingStatus);

            // Assert
            Assert.HasCount(1, serialMappings);
            Assert.AreEqual("Board1 # / SN-111", configItems[0].ModuleSerial, "Exact match unchanged");
            Assert.AreEqual("Board2 # / SN-222", configItems[1].ModuleSerial, "Auto-bound updated");
            Assert.AreEqual("Board3 # / SN-333", configItems[2].ModuleSerial, "Missing unchanged");
        }

        [TestMethod]
        public void ApplyAutoBinding_MultipleConfigItems_IgnoreMissingMatch()
        {
            // Arrange
            var connectedControllers = new List<string>
            {
                "Board1 # / SN-111",
            };
            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("Board1 # / SN-111"),
                CreateConfigItem("Board1 # / SN-OTHER")
            };
            var existingBindings = new List<ControllerBinding>();
            var binder = new ControllerAutoBinder(connectedControllers);
            var bindingStatus = binder.AnalyzeBindings(configItems, existingBindings);

            // Act
            var serialMappings = binder.ApplyAutoBinding(configItems, bindingStatus);

            // Assert
            Assert.HasCount(0, serialMappings);
            Assert.AreEqual("Board1 # / SN-111", configItems[0].ModuleSerial, "Exact match unchanged");
            Assert.AreEqual("Board1 # / SN-OTHER", configItems[1].ModuleSerial, "Missing unchanged");
        }

        [TestMethod]
        public void ApplyAutoBinding_MultipleConfigItems_IgnoreMissingMatchAndOrder()
        {
            // Arrange
            var connectedControllers = new List<string>
            {
                "Board1 # / SN-111",
            };
            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("Board1 # / SN-OTHER"),
                CreateConfigItem("Board1 # / SN-111")
            };
            var existingBindings = new List<ControllerBinding>();
            var binder = new ControllerAutoBinder(connectedControllers);
            var bindingStatus = binder.AnalyzeBindings(configItems, existingBindings);

            // Act
            var serialMappings = binder.ApplyAutoBinding(configItems, bindingStatus);

            // Assert
            Assert.HasCount(0, serialMappings);
            Assert.AreEqual("Board1 # / SN-OTHER", configItems[0].ModuleSerial, "Exact match unchanged");
            Assert.AreEqual("Board1 # / SN-111", configItems[1].ModuleSerial, "Missing unchanged");
        }

        [TestMethod]
        public void ApplyAutoBinding_MultipleConfigItems_UseExistingBindingsInformation_OrderTest()
        {
            // Arrange
            var connectedControllers = new List<string>
            {
                "Board1 # / SN-111",
            };
            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("Board1 # / SN-222"),
                CreateConfigItem("Board1 # / SN-333")
            };
            var existingBindings = new List<ControllerBinding>()
            {
                new ControllerBinding()
                {
                    OriginalController = "Board1 # / SN-333", BoundController = "Board1 # / SN-111", Status = ControllerBindingStatus.AutoBind
                }
            };
            var binder = new ControllerAutoBinder(connectedControllers);
            var bindingStatus = binder.AnalyzeBindings(configItems, existingBindings);

            // Act
            var serialMappings = binder.ApplyAutoBinding(configItems, bindingStatus);

            // Assert
            var binding1 = bindingStatus.Find(b => b.OriginalController == "Board1 # / SN-222");
            var binding2 = bindingStatus.Find(b => b.OriginalController == "Board1 # / SN-333");

            Assert.AreEqual(ControllerBindingStatus.Missing, binding1.Status);
            Assert.AreEqual(ControllerBindingStatus.AutoBind, binding2.Status);
            Assert.HasCount(1, serialMappings);
            Assert.AreEqual("Board1 # / SN-222", configItems[0].ModuleSerial, "Missing unchanged");
            Assert.AreEqual("Board1 # / SN-111", configItems[1].ModuleSerial, "Auto-bind changed");
        }

        [TestMethod]
        public void ApplyAutoBinding_MultipleConfigItems_UseExistingBindingsInformation_AutoBindFresh()
        {
            // in the last file we did auto bind to SN-444
            // but in the current profile, SN-444 is not referenced
            // so we can do a fresh auto-bind to SN-111

            // Arrange
            var connectedControllers = new List<string>
            {
                "Board1 # / SN-111",
            };
            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("Board1 # / SN-222"),
                CreateConfigItem("Board2 # / SN-333")
            };

            var existingBindings = new List<ControllerBinding>()
            {
                new ControllerBinding()
                {
                    OriginalController = "Board1 # / SN-444", BoundController = "Board1 # / SN-111", Status = ControllerBindingStatus.AutoBind
                }
            };

            var binder = new ControllerAutoBinder(connectedControllers);
            var bindingStatus = binder.AnalyzeBindings(configItems, existingBindings);

            // Act
            var serialMappings = binder.ApplyAutoBinding(configItems, bindingStatus);

            // Assert
            var binding1 = bindingStatus.Find(b => b.OriginalController == "Board1 # / SN-222");
            var binding2 = bindingStatus.Find(b => b.OriginalController == "Board2 # / SN-333");

            Assert.AreEqual(ControllerBindingStatus.AutoBind, binding1.Status);
            Assert.AreEqual(ControllerBindingStatus.Missing, binding2.Status);
            Assert.HasCount(1, serialMappings);
            Assert.AreEqual("Board1 # / SN-111", configItems[0].ModuleSerial, "Missing unchanged");
            Assert.AreEqual("Board2 # / SN-333", configItems[1].ModuleSerial, "Auto-bind changed");
        }

        #endregion

        #region Duplicate Serials Tests

        [TestMethod]
        public void AnalyzeBindings_DuplicateSerials_ReturnsOnlyUnique()
        {
            // Arrange
            var connectedControllers = new List<string> { "Board # / SN-NEW" };
            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("Board # / SN-OLD"),
                CreateConfigItem("Board # / SN-OLD"),  // Duplicate
                CreateConfigItem("Board # / SN-OLD")   // Duplicate
            };
            var existingBindings = new List<ControllerBinding>();
            var binder = new ControllerAutoBinder(connectedControllers);

            // Act
            var results = binder.AnalyzeBindings(configItems, existingBindings);

            // Assert
            Assert.HasCount(1, results, "Should only analyze unique serials");
            var binding = results.Find(b => b.OriginalController == "Board # / SN-OLD");
            Assert.AreEqual(ControllerBindingStatus.AutoBind, binding.Status);
        }

        [TestMethod]
        public void ApplyAutoBinding_DuplicateSerials_UpdatesAllInstances()
        {
            // Arrange
            var connectedControllers = new List<string> { "Board # / SN-NEW" };
            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("Board # / SN-OLD"),
                CreateConfigItem("Board # / SN-OLD"),
                CreateConfigItem("Board # / SN-OLD")
            };
            var existingBindings = new List<ControllerBinding>();
            var binder = new ControllerAutoBinder(connectedControllers);
            var bindingStatus = binder.AnalyzeBindings(configItems, existingBindings);

            // Act
            binder.ApplyAutoBinding(configItems, bindingStatus);

            // Assert
            Assert.IsTrue(configItems.All(c => c.ModuleSerial == "Board # / SN-NEW"),
                "All duplicate serials should be updated");
        }

        #endregion

        #region Empty and Null Tests

        [TestMethod]
        public void AnalyzeBindings_EmptyConfigItems_ReturnsEmptyDictionary()
        {
            // Arrange
            var connectedControllers = new List<string> { "Board # / SN-123" };
            var configItems = new List<IConfigItem>();
            var existingBindings = new List<ControllerBinding>();

            var binder = new ControllerAutoBinder(connectedControllers);

            // Act
            var results = binder.AnalyzeBindings(configItems, existingBindings);

            // Assert
            Assert.IsEmpty(results);
        }

        [TestMethod]
        public void AnalyzeBindings_IgnoresEmptyAndDashSerials()
        {
            // Arrange
            var connectedControllers = new List<string> { "Board # / SN-123" };
            var configItems = new List<IConfigItem>
            {
                CreateConfigItem(""),
                CreateConfigItem("-"),
                CreateConfigItem(null),
                CreateConfigItem("Board # / SN-123")
            };
            var existingBindings = new List<ControllerBinding>();
            var binder = new ControllerAutoBinder(connectedControllers);

            // Act
            var results = binder.AnalyzeBindings(configItems, existingBindings);

            // Assert
            Assert.HasCount(1, results);
            Assert.IsTrue(results.Any(b => b.OriginalController == "Board # / SN-123"));
        }

        [TestMethod]
        public void Constructor_NullConnectedControllers_HandlesGracefully()
        {
            // Arrange & Act
            var binder = new ControllerAutoBinder(null);
            var configItems = new List<IConfigItem> { CreateConfigItem("Board # / SN-123") };
            var existingBindings = new List<ControllerBinding>();

            // Act
            var results = binder.AnalyzeBindings(configItems, existingBindings);

            // Assert
            Assert.HasCount(1, results);
            var binding = results.Find(b => b.OriginalController == "Board # / SN-123");
            Assert.AreEqual(ControllerBindingStatus.Missing, binding.Status);
        }

        #endregion

        #region Helper Methods

        private IConfigItem CreateConfigItem(string moduleSerial)
        {
            return new OutputConfigItem
            {
                ModuleSerial = moduleSerial,
                Active = true,
                GUID = System.Guid.NewGuid().ToString()
            };
        }

        #endregion

        #region GetTypeAndName Tests

        [TestMethod]
        public void GetTypeAndName_StandardFormat_ReturnsTypeAndName()
        {
            // Arrange
            var serial = "Board #/ SN-1234567890";

            // Act
            var result = ControllerAutoBinder.GetTypeAndName(serial);

            // Assert
            Assert.AreEqual("Board #", result);
        }

        [TestMethod]
        public void GetTypeAndName_WithWhitespace_TrimsProperly()
        {
            // Arrange
            var serial = "  X1-Pro #  / SN-ABC123  ";

            // Act
            var result = ControllerAutoBinder.GetTypeAndName(serial);

            // Assert
            Assert.AreEqual("X1-Pro #", result);
        }

        [TestMethod]
        public void GetTypeAndName_NoSeparator_ReturnsFullString()
        {
            // Arrange
            var serial = "BoardWithoutSeparator";

            // Act
            var result = ControllerAutoBinder.GetTypeAndName(serial);

            // Assert
            Assert.AreEqual("BoardWithoutSeparator", result);
        }

        [TestMethod]
        public void GetTypeAndName_EmptyString_ReturnsEmptyString()
        {
            // Arrange
            var serial = "";

            // Act
            var result = ControllerAutoBinder.GetTypeAndName(serial);

            // Assert
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void GetTypeAndName_OnlySeparator_ReturnsEmptyString()
        {
            // Arrange
            var serial = "/ ";

            // Act
            var result = ControllerAutoBinder.GetTypeAndName(serial);

            // Assert
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void GetTypeAndName_MultipleSeparators_ReturnFirstPart()
        {
            // Arrange
            var serial = "Board #/ SN-123/ Extra";

            // Act
            var result = ControllerAutoBinder.GetTypeAndName(serial);

            // Assert
            Assert.AreEqual("Board #", result);
        }

        #endregion

        #region ApplyBindingUpdate Tests

        [TestMethod]
        public void ApplyBindingUpdate_WithValidBindings_UpdatesAllMatchingConfigItems()
        {
            // Arrange
            var connectedControllers = new List<string> { "Board # / SN-123" };
            var binder = new ControllerAutoBinder(connectedControllers);

            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("Board # / SN-OLD"),
                CreateConfigItem("Board # / SN-OLD"),
                CreateConfigItem("OtherBoard # / SN-999")
            };

            var controllerBindings = new List<ControllerBinding>
            {
                new ControllerBinding
                {
                    OriginalController = "Board # / SN-OLD",
                    BoundController = "Board # / SN-NEW",
                    Status = ControllerBindingStatus.AutoBind
                }
            };

            // Act
            binder.ApplyBindingUpdate(configItems, controllerBindings);

            // Assert
            Assert.AreEqual("Board # / SN-NEW", configItems[0].ModuleSerial, "First item should be updated");
            Assert.AreEqual("Board # / SN-NEW", configItems[1].ModuleSerial, "Second item should be updated");
            Assert.AreEqual("OtherBoard # / SN-999", configItems[2].ModuleSerial, "Unmatched item should remain unchanged");
        }

        [TestMethod]
        public void ApplyBindingUpdate_WithMultipleBindings_UpdatesEachCorrectly()
        {
            // Arrange
            var connectedControllers = new List<string>();
            var binder = new ControllerAutoBinder(connectedControllers);

            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("Board1 # / SN-OLD1"),
                CreateConfigItem("Board2 # / SN-OLD2"),
                CreateConfigItem("Board3 # / SN-OLD3")
            };

            var controllerBindings = new List<ControllerBinding>
            {
                new ControllerBinding
                {
                    OriginalController = "Board1 # / SN-OLD1",
                    BoundController = "Board1 # / SN-NEW1",
                    Status = ControllerBindingStatus.AutoBind
                },
                new ControllerBinding
                {
                    OriginalController = "Board2 # / SN-OLD2",
                    BoundController = "Board2 # / SN-NEW2",
                    Status = ControllerBindingStatus.AutoBind
                },
                new ControllerBinding
                {
                    OriginalController = "Board3 # / SN-OLD3",
                    BoundController = "Board3 # / SN-NEW3",
                    Status = ControllerBindingStatus.Match
                }
            };

            // Act
            binder.ApplyBindingUpdate(configItems, controllerBindings);

            // Assert
            Assert.AreEqual("Board1 # / SN-NEW1", configItems[0].ModuleSerial);
            Assert.AreEqual("Board2 # / SN-NEW2", configItems[1].ModuleSerial);
            Assert.AreEqual("Board3 # / SN-NEW3", configItems[2].ModuleSerial);
        }

        [TestMethod]
        public void ApplyBindingUpdate_WithEmptyBindings_MakesNoChanges()
        {
            // Arrange
            var connectedControllers = new List<string>();
            var binder = new ControllerAutoBinder(connectedControllers);

            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("Board # / SN-123"),
                CreateConfigItem("OtherBoard # / SN-456")
            };

            var controllerBindings = new List<ControllerBinding>();

            // Act
            binder.ApplyBindingUpdate(configItems, controllerBindings);

            // Assert
            Assert.AreEqual("Board # / SN-123", configItems[0].ModuleSerial);
            Assert.AreEqual("OtherBoard # / SN-456", configItems[1].ModuleSerial);
        }

        [TestMethod]
        public void ApplyBindingUpdate_IgnoresEmptyModuleSerials()
        {
            // Arrange
            var connectedControllers = new List<string>();
            var binder = new ControllerAutoBinder(connectedControllers);

            var configItems = new List<IConfigItem>
            {
                CreateConfigItem(""),
                CreateConfigItem(null),
                CreateConfigItem("Board # / SN-OLD")
            };

            var controllerBindings = new List<ControllerBinding>
            {
                new ControllerBinding
                {
                    OriginalController = "Board # / SN-OLD",
                    BoundController = "Board # / SN-NEW",
                    Status = ControllerBindingStatus.AutoBind
                }
            };

            // Act
            binder.ApplyBindingUpdate(configItems, controllerBindings);

            // Assert
            Assert.AreEqual("", configItems[0].ModuleSerial, "Empty serial should remain empty");
            Assert.IsNull(configItems[1].ModuleSerial, "Null serial should remain null");
            Assert.AreEqual("Board # / SN-NEW", configItems[2].ModuleSerial, "Valid serial should be updated");
        }

        [TestMethod]
        public void ApplyBindingUpdate_IgnoresDashModuleSerials()
        {
            // Arrange
            var connectedControllers = new List<string>();
            var binder = new ControllerAutoBinder(connectedControllers);

            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("-"),
                CreateConfigItem("Board # / SN-OLD")
            };

            var controllerBindings = new List<ControllerBinding>
            {
                new ControllerBinding
                {
                    OriginalController = "-",
                    BoundController = "Board # / SN-NEW",
                    Status = ControllerBindingStatus.AutoBind
                },
                new ControllerBinding
                {
                    OriginalController = "Board # / SN-OLD",
                    BoundController = "Board # / SN-NEW2",
                    Status = ControllerBindingStatus.AutoBind
                }
            };

            // Act
            binder.ApplyBindingUpdate(configItems, controllerBindings);

            // Assert
            Assert.AreEqual("-", configItems[0].ModuleSerial, "Dash serial should remain unchanged");
            Assert.AreEqual("Board # / SN-NEW2", configItems[1].ModuleSerial, "Valid serial should be updated");
        }

        [TestMethod]
        public void ApplyBindingUpdate_WithNoMatchingBinding_LeavesConfigUnchanged()
        {
            // Arrange
            var connectedControllers = new List<string>();
            var binder = new ControllerAutoBinder(connectedControllers);

            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("Board # / SN-123"),
                CreateConfigItem("OtherBoard # / SN-456")
            };

            var controllerBindings = new List<ControllerBinding>
            {
                new ControllerBinding
                {
                    OriginalController = "DifferentBoard # / SN-999",
                    BoundController = "DifferentBoard # / SN-000",
                    Status = ControllerBindingStatus.AutoBind
                }
            };

            // Act
            binder.ApplyBindingUpdate(configItems, controllerBindings);

            // Assert
            Assert.AreEqual("Board # / SN-123", configItems[0].ModuleSerial, "Unmatched items should remain unchanged");
            Assert.AreEqual("OtherBoard # / SN-456", configItems[1].ModuleSerial, "Unmatched items should remain unchanged");
        }

        [TestMethod]
        public void ApplyBindingUpdate_WithMissingStatusBinding_StillUpdates()
        {
            // Arrange
            var connectedControllers = new List<string>();
            var binder = new ControllerAutoBinder(connectedControllers);

            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("Board # / SN-OLD")
            };

            var controllerBindings = new List<ControllerBinding>
            {
                new ControllerBinding
                {
                    OriginalController = "Board # / SN-OLD",
                    BoundController = "Board # / SN-NEW",
                    Status = ControllerBindingStatus.Missing
                }
            };

            // Act
            binder.ApplyBindingUpdate(configItems, controllerBindings);

            // Assert
            Assert.AreEqual("Board # / SN-NEW", configItems[0].ModuleSerial, "Should update regardless of status");
        }

        [TestMethod]
        public void ApplyBindingUpdate_WithNullBoundController_SkipsUpdate()
        {
            // Arrange
            var connectedControllers = new List<string>();
            var binder = new ControllerAutoBinder(connectedControllers);

            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("Board # / SN-OLD")
            };

            var controllerBindings = new List<ControllerBinding>
            {
                new ControllerBinding
                {
                    OriginalController = "Board # / SN-OLD",
                    BoundController = null,
                    Status = ControllerBindingStatus.Missing
                }
            };

            // Act
            binder.ApplyBindingUpdate(configItems, controllerBindings);

            // Assert
            Assert.AreEqual("Board # / SN-OLD", configItems[0].ModuleSerial, "Should skip update when Bound Controller is null");
        }

        [TestMethod]
        public void ApplyBindingUpdate_IntegrationWithAnalyzeBindings_WorksTogether()
        {
            // This test validates that ApplyBindingUpdate works correctly with output from AnalyzeBindings
            // Arrange
            var connectedControllers = new List<string>
            {
                "Board1 # / SN-NEW1",
                "Board2 # / SN-NEW2"
            };
            var binder = new ControllerAutoBinder(connectedControllers);

            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("Board1 # / SN-OLD1"),
                CreateConfigItem("Board2 # / SN-OLD2"),
                CreateConfigItem("Board3 # / SN-MISSING")
            };

            var existingBindings = new List<ControllerBinding>();

            // Act
            var analyzedBindings = binder.AnalyzeBindings(configItems, existingBindings);
            binder.ApplyBindingUpdate(configItems, analyzedBindings);

            // Assert
            Assert.AreEqual("Board1 # / SN-NEW1", configItems[0].ModuleSerial, "Should be auto-bound");
            Assert.AreEqual("Board2 # / SN-NEW2", configItems[1].ModuleSerial, "Should be auto-bound");
            Assert.AreEqual("Board3 # / SN-MISSING", configItems[2].ModuleSerial, "Missing controller should remain unchanged");
        }

        #endregion
    }
}