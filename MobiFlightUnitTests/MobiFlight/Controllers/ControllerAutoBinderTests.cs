using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base;
using MobiFlight.Controllers;
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
            var connectedControllers = new List<Controller> {
                SerialNumber.CreateController("MyBoard #/ SN-1234567890")
            };

            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("MyBoard #/ SN-1234567890")
            };
            var binder = new ControllerAutoBinder(connectedControllers);
            var existingBindings = new List<ControllerBinding>();

            // Act
            var results = binder.AnalyzeBindings(configItems, existingBindings);
            var serialMappings = binder.ApplyAutoBinding(configItems, results);

            // Assert
            Assert.HasCount(1, results);
            var binding = results.Find(b => b.OriginalController.Equals(SerialNumber.CreateController("MyBoard #/ SN-1234567890")));
            Assert.AreEqual(ControllerBindingStatus.Match, binding.Status);
            Assert.IsEmpty(serialMappings);
            Assert.AreEqual("MyBoard #", configItems[0].Controller.Name);
            Assert.AreEqual("SN-1234567890", configItems[0].Controller.Serial);
            Assert.AreEqual("MyBoard #", binding.BoundController.Name);
            Assert.AreEqual("SN-1234567890", binding.BoundController.Serial);
        }

        [TestMethod]
        public void Scenario2_SerialDiffers_ReturnsAutoBoundAndUpdatesSerial()
        {
            // Arrange
            var connectedControllers = new List<Controller> {
                SerialNumber.CreateController("X1-Pro #/ SN-NEW456")
            };
            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("X1-Pro #/ SN-OLD123")
            };
            var binder = new ControllerAutoBinder(connectedControllers);
            var existingBindings = new List<ControllerBinding>();

            // Act
            var results = binder.AnalyzeBindings(configItems, existingBindings);
            var serialMappings = binder.ApplyAutoBinding(configItems, results);

            // Assert
            Assert.HasCount(1, results);
            var binding = results.Find(b => b.OriginalController.Equals(SerialNumber.CreateController("X1-Pro #/ SN-OLD123")));
            Assert.AreEqual(ControllerBindingStatus.AutoBind, binding.Status);
            Assert.HasCount(1, serialMappings);
            Assert.IsTrue(serialMappings[0].OriginalController.Equals(SerialNumber.CreateController("X1-Pro #/ SN-OLD123")));
            Assert.AreEqual("X1-Pro #", configItems[0].Controller.Name);
            Assert.AreEqual("SN-NEW456", configItems[0].Controller.Serial);
        }

        [TestMethod]
        public void Scenario3_NameDiffers_ReturnsAutoBoundAndUpdatesName()
        {
            // Arrange
            var connectedControllers = new List<Controller> {
                SerialNumber.CreateController("NewBoardName #/ SN-1234567890")
            };
            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("OldBoardName #/ SN-1234567890")
            };
            var binder = new ControllerAutoBinder(connectedControllers);
            var existingBindings = new List<ControllerBinding>();

            // Act
            var results = binder.AnalyzeBindings(configItems, existingBindings);
            var serialMappings = binder.ApplyAutoBinding(configItems, results);

            // Assert
            Assert.HasCount(1, results);
            var binding = results.Find(b => b.OriginalController.Equals(SerialNumber.CreateController("OldBoardName #/ SN-1234567890")));
            Assert.AreEqual(ControllerBindingStatus.AutoBind, binding.Status);
            Assert.HasCount(1, serialMappings);
            Assert.AreEqual("OldBoardName #", serialMappings[0].OriginalController.Name);
            Assert.AreEqual("SN-1234567890", serialMappings[0].OriginalController.Serial);
            Assert.AreEqual("NewBoardName #", serialMappings[0].BoundController.Name);
            Assert.AreEqual("SN-1234567890", serialMappings[0].BoundController.Serial);
            Assert.AreEqual("NewBoardName #", configItems[0].Controller.Name);
            Assert.AreEqual("SN-1234567890", configItems[0].Controller.Serial);
        }

        [TestMethod]
        public void Scenario4_Missing_ReturnsMissingAndNoChanges()
        {
            // Arrange
            var connectedControllers = new List<Controller> {
                SerialNumber.CreateController("DifferentBoard #/ SN-9999")
            };
            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("X1-Pro #/ SN-1234")
            };
            var binder = new ControllerAutoBinder(connectedControllers);
            var existingBindings = new List<ControllerBinding>();

            // Act
            var results = binder.AnalyzeBindings(configItems, existingBindings);
            var serialMappings = binder.ApplyAutoBinding(configItems, results);

            // Assert
            Assert.HasCount(1, results);
            var binding = results.Find(b => b.OriginalController.Equals(SerialNumber.CreateController("X1-Pro #/ SN-1234")));
            Assert.AreEqual(ControllerBindingStatus.Missing, binding.Status);
            Assert.IsEmpty(serialMappings);
            Assert.AreEqual("X1-Pro #", configItems[0].Controller.Name);
            Assert.AreEqual("SN-1234", configItems[0].Controller.Serial);
        }

        [TestMethod]
        public void Scenario5_MultipleControllerMatches_RequiresManualBindAndNoChanges()
        {
            // Arrange
            var connectedControllers = new List<Controller>
            {
                SerialNumber.CreateController("Joystick X #/ JS-111111"),
                SerialNumber.CreateController("Joystick X #/ JS-222222")
            };
            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("Joystick X #/ JS-999999")
            };
            var binder = new ControllerAutoBinder(connectedControllers);
            var existingBindings = new List<ControllerBinding>();

            // Act
            var results = binder.AnalyzeBindings(configItems, existingBindings);
            var serialMappings = binder.ApplyAutoBinding(configItems, results);

            // Assert
            Assert.HasCount(1, results);
            var binding = results.Find(b => b.OriginalController.Equals(SerialNumber.CreateController("Joystick X #/ JS-999999")));
            Assert.AreEqual(ControllerBindingStatus.RequiresManualBind, binding.Status);
            Assert.IsEmpty(serialMappings);
            Assert.AreEqual("Joystick X #", configItems[0].Controller.Name);
            Assert.AreEqual("JS-999999", configItems[0].Controller.Serial);
        }

        [TestMethod]
        public void Scenario6_MultipleConfigMatches_RequiresManualBindAndNoChanges()
        {
            // Arrange
            var connectedControllers = new List<Controller>
            {
                SerialNumber.CreateController("Joystick X #/ JS-111111")
            };
            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("Joystick X #/ JS-222222"),
                CreateConfigItem("Joystick X #/ JS-333333")
            };
            var binder = new ControllerAutoBinder(connectedControllers);
            var existingBindings = new List<ControllerBinding>();

            // Act
            var results = binder.AnalyzeBindings(configItems, existingBindings);
            var serialMappings = binder.ApplyAutoBinding(configItems, results);

            // Assert
            Assert.HasCount(2, results);
            var binding = results.Find(b => b.OriginalController.Equals(SerialNumber.CreateController("Joystick X #/ JS-222222")));
            Assert.AreEqual(ControllerBindingStatus.RequiresManualBind, binding.Status);
            Assert.IsEmpty(serialMappings);
            Assert.AreEqual("Joystick X #", configItems[0].Controller.Name);
            Assert.AreEqual("JS-222222", configItems[0].Controller.Serial);

            binding = results.Find(b => b.OriginalController.Equals(SerialNumber.CreateController("Joystick X #/ JS-333333")));
            Assert.AreEqual(ControllerBindingStatus.RequiresManualBind, binding.Status);
            Assert.AreEqual("Joystick X #", configItems[1].Controller.Name);
            Assert.AreEqual("JS-333333", configItems[1].Controller.Serial);
        }

        #endregion

        #region Multiple Config Items Tests

        [TestMethod]
        public void AnalyzeBindings_MultipleConfigItems_AnalyzesAll()
        {
            // Arrange
            var connectedControllers = new List<Controller>
            {
                SerialNumber.CreateController("Board1 #/ SN-111"),
                SerialNumber.CreateController("Board2 #/ SN-222")
            };
            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("Board1 #/ SN-111"),
                CreateConfigItem("Board2 #/ SN-OLD"),
                CreateConfigItem("Board3 #/ SN-333")
            };
            var binder = new ControllerAutoBinder(connectedControllers);
            var existingBindings = new List<ControllerBinding>();

            // Act
            var results = binder.AnalyzeBindings(configItems, existingBindings);

            // Assert
            Assert.HasCount(3, results);
            var binding1 = results.Find(b => b.OriginalController.Equals(SerialNumber.CreateController("Board1 #/ SN-111")));
            var binding2 = results.Find(b => b.OriginalController.Equals(SerialNumber.CreateController("Board2 #/ SN-OLD")));
            var binding3 = results.Find(b => b.OriginalController.Equals(SerialNumber.CreateController("Board3 #/ SN-333")));

            Assert.AreEqual(ControllerBindingStatus.Match, binding1.Status);
            Assert.AreEqual(ControllerBindingStatus.AutoBind, binding2.Status);
            Assert.AreEqual(ControllerBindingStatus.Missing, binding3.Status);
        }

        [TestMethod]
        public void ApplyAutoBinding_MultipleConfigItems_UpdatesOnlyAutoBound()
        {
            // Arrange
            var connectedControllers = new List<Controller>
            {
                SerialNumber.CreateController("Board1 #/ SN-111"),
                SerialNumber.CreateController("Board2 #/ SN-222")
            };
            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("Board1 #/ SN-111"),
                CreateConfigItem("Board2 #/ SN-OLD"),
                CreateConfigItem("Board3 #/ SN-333")
            };
            var existingBindings = new List<ControllerBinding>();

            var binder = new ControllerAutoBinder(connectedControllers);
            var bindingStatus = binder.AnalyzeBindings(configItems, existingBindings);

            // Act
            var serialMappings = binder.ApplyAutoBinding(configItems, bindingStatus);

            // Assert
            Assert.HasCount(1, serialMappings);
            Assert.AreEqual("Board1 #", configItems[0].Controller.Name, "Exact match Name unchanged");
            Assert.AreEqual("SN-111", configItems[0].Controller.Serial, "Exact match Serial unchanged");
            Assert.AreEqual("Board2 #", configItems[1].Controller.Name, "Auto-bound Name unchanged");
            Assert.AreEqual("SN-222", configItems[1].Controller.Serial, "Auto-bound Serial updated");
            Assert.AreEqual("Board3 #", configItems[2].Controller.Name, "Missing Name unchanged");
            Assert.AreEqual("SN-333", configItems[2].Controller.Serial, "Missing Serial unchanged");
        }

        [TestMethod]
        public void ApplyAutoBinding_MultipleConfigItems_IgnoreMissingMatch()
        {
            // Arrange
            var connectedControllers = new List<Controller>
            {
                SerialNumber.CreateController("Board1 #/ SN-111"),
            };
            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("Board1 #/ SN-111"),
                CreateConfigItem("Board1 #/ SN-OTHER")
            };
            var existingBindings = new List<ControllerBinding>();
            var binder = new ControllerAutoBinder(connectedControllers);
            var bindingStatus = binder.AnalyzeBindings(configItems, existingBindings);

            // Act
            var serialMappings = binder.ApplyAutoBinding(configItems, bindingStatus);

            // Assert
            Assert.HasCount(0, serialMappings);
            Assert.AreEqual("Board1 #", configItems[0].Controller.Name, "Exact match Name unchanged");
            Assert.AreEqual("SN-111", configItems[0].Controller.Serial, "Exact match Serial unchanged");
            Assert.AreEqual("Board1 #", configItems[1].Controller.Name, "Missing Name unchanged");
            Assert.AreEqual("SN-OTHER", configItems[1].Controller.Serial, "Missing Serial unchanged");
        }

        [TestMethod]
        public void ApplyAutoBinding_MultipleConfigItems_IgnoreMissingMatchAndOrder()
        {
            // Arrange
            var connectedControllers = new List<Controller>
            {
                SerialNumber.CreateController("Board1 #/ SN-111"),
            };
            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("Board1 #/ SN-OTHER"),
                CreateConfigItem("Board1 #/ SN-111")
            };
            var existingBindings = new List<ControllerBinding>();
            var binder = new ControllerAutoBinder(connectedControllers);
            var bindingStatus = binder.AnalyzeBindings(configItems, existingBindings);

            // Act
            var serialMappings = binder.ApplyAutoBinding(configItems, bindingStatus);

            // Assert
            Assert.HasCount(0, serialMappings);
            Assert.AreEqual("Board1 #", configItems[0].Controller.Name, "Missing Name unchanged");
            Assert.AreEqual("SN-OTHER", configItems[0].Controller.Serial, "Missing Serial unchanged");
            Assert.AreEqual("Board1 #", configItems[1].Controller.Name, "Exact match Name unchanged");
            Assert.AreEqual("SN-111", configItems[1].Controller.Serial, "Exact match Serial unchanged");
        }

        [TestMethod]
        public void ApplyAutoBinding_MultipleConfigItems_UseExistingBindingsInformation_OrderTest()
        {
            // Arrange
            var connectedControllers = new List<Controller>
            {
                SerialNumber.CreateController("Board1 #/ SN-111"),
            };
            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("Board1 #/ SN-222"),
                CreateConfigItem("Board1 #/ SN-333")
            };
            var existingBindings = new List<ControllerBinding>()
            {
                new ControllerBinding()
                {
                    OriginalController = SerialNumber.CreateController("Board1 #/ SN-333"),
                    BoundController = SerialNumber.CreateController("Board1 #/ SN-111"),
                    Status = ControllerBindingStatus.AutoBind
                }
            };
            var binder = new ControllerAutoBinder(connectedControllers);
            var bindingStatus = binder.AnalyzeBindings(configItems, existingBindings);

            // Act
            var serialMappings = binder.ApplyAutoBinding(configItems, bindingStatus);

            // Assert
            var binding1 = bindingStatus.Find(b => b.OriginalController.Equals(SerialNumber.CreateController("Board1 #/ SN-222")));
            var binding2 = bindingStatus.Find(b => b.OriginalController.Equals(SerialNumber.CreateController("Board1 #/ SN-333")));

            Assert.AreEqual(ControllerBindingStatus.Missing, binding1.Status);
            Assert.AreEqual(ControllerBindingStatus.AutoBind, binding2.Status);
            Assert.HasCount(1, serialMappings);
            Assert.AreEqual("Board1 #", configItems[0].Controller.Name, "Missing Name unchanged");
            Assert.AreEqual("SN-222", configItems[0].Controller.Serial, "Missing Serial unchanged");
            Assert.AreEqual("Board1 #", configItems[1].Controller.Name, "Auto-bind Name unchanged");
            Assert.AreEqual("SN-111", configItems[1].Controller.Serial, "Auto-bind Serial changed");
        }

        [TestMethod]
        public void ApplyAutoBinding_MultipleConfigItems_UseExistingBindingsInformation_AutoBindFresh()
        {
            // in the last file we did auto bind to SN-444
            // but in the current profile, SN-444 is not referenced
            // so we can do a fresh auto-bind to SN-111

            // Arrange
            var connectedControllers = new List<Controller>
            {
                SerialNumber.CreateController("Board1 #/ SN-111"),
            };
            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("Board1 #/ SN-222"),
                CreateConfigItem("Board2 #/ SN-333")
            };

            var existingBindings = new List<ControllerBinding>()
            {
                new ControllerBinding()
                {
                    OriginalController = SerialNumber.CreateController("Board1 #/ SN-444"),
                    BoundController = SerialNumber.CreateController("Board1 #/ SN-111"),
                    Status = ControllerBindingStatus.AutoBind
                }
            };

            var binder = new ControllerAutoBinder(connectedControllers);
            var bindingStatus = binder.AnalyzeBindings(configItems, existingBindings);

            // Act
            var serialMappings = binder.ApplyAutoBinding(configItems, bindingStatus);

            // Assert
            var binding1 = bindingStatus.Find(b => b.OriginalController.Equals(SerialNumber.CreateController("Board1 #/ SN-222")));
            var binding2 = bindingStatus.Find(b => b.OriginalController.Equals(SerialNumber.CreateController("Board2 #/ SN-333")));

            Assert.AreEqual(ControllerBindingStatus.AutoBind, binding1.Status);
            Assert.AreEqual(ControllerBindingStatus.Missing, binding2.Status);
            Assert.HasCount(1, serialMappings);
            Assert.AreEqual("Board1 #", configItems[0].Controller.Name, "Auto-bind Name unchanged");
            Assert.AreEqual("SN-111", configItems[0].Controller.Serial, "Auto-bind Serial updated");
            Assert.AreEqual("Board2 #", configItems[1].Controller.Name, "Missing Name unchanged");
            Assert.AreEqual("SN-333", configItems[1].Controller.Serial, "Missing Serial unchanged");
        }

        #endregion

        #region Duplicate Serials Tests

        [TestMethod]
        public void AnalyzeBindings_DuplicateControllers_ReturnsOnlyUnique()
        {
            // Arrange
            var connectedControllers = new List<Controller> {
                SerialNumber.CreateController("Board #/ SN-NEW")
            };
            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("Board #/ SN-OLD"),
                CreateConfigItem("Board #/ SN-OLD"),  // Duplicate
                CreateConfigItem("Board #/ SN-OLD")   // Duplicate
            };
            var existingBindings = new List<ControllerBinding>();
            var binder = new ControllerAutoBinder(connectedControllers);

            // Act
            var results = binder.AnalyzeBindings(configItems, existingBindings);

            // Assert
            Assert.HasCount(1, results, "Should only analyze unique controllers");
            var binding = results.Find(b => b.OriginalController.Equals(SerialNumber.CreateController("Board #/ SN-OLD")));
            Assert.AreEqual(ControllerBindingStatus.AutoBind, binding.Status);
        }

        [TestMethod]
        public void ApplyAutoBinding_DuplicateControllers_UpdatesAllInstances()
        {
            // Arrange
            var connectedControllers = new List<Controller> {
                SerialNumber.CreateController("Board #/ SN-NEW")
            };
            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("Board #/ SN-OLD"),
                CreateConfigItem("Board #/ SN-OLD"),
                CreateConfigItem("Board #/ SN-OLD")
            };
            var existingBindings = new List<ControllerBinding>();
            var binder = new ControllerAutoBinder(connectedControllers);
            var bindingStatus = binder.AnalyzeBindings(configItems, existingBindings);

            // Act
            binder.ApplyAutoBinding(configItems, bindingStatus);

            // Assert
            Assert.IsTrue(configItems.All(c => c.Controller.Name == "Board #" && c.Controller.Serial == "SN-NEW"),
                "All duplicate controllers should be updated");
        }

        #endregion

        #region Empty and Null Tests

        [TestMethod]
        public void AnalyzeBindings_EmptyConfigItems_ReturnsEmptyList()
        {
            // Arrange
            var connectedControllers = new List<Controller> {
                SerialNumber.CreateController("Board #/ SN-123")
            };
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
            var connectedControllers = new List<Controller> {
                SerialNumber.CreateController("Board #/ SN-123")
            };
            var configItems = new List<IConfigItem>
            {
                CreateConfigItem(""),
                CreateConfigItem("-"),
                CreateConfigItem(null),
                CreateConfigItem("Board #/ SN-123")
            };
            var existingBindings = new List<ControllerBinding>();
            var binder = new ControllerAutoBinder(connectedControllers);

            // Act
            var results = binder.AnalyzeBindings(configItems, existingBindings);

            // Assert
            Assert.HasCount(1, results);
            Assert.IsTrue(results.Any(b => b.OriginalController.Equals(SerialNumber.CreateController("Board #/ SN-123"))));
        }

        [TestMethod]
        public void Constructor_NullConnectedControllers_HandlesGracefully()
        {
            // Arrange & Act
            var binder = new ControllerAutoBinder(null);
            var configItems = new List<IConfigItem> {
                CreateConfigItem("Board #/ SN-123")
            };
            var existingBindings = new List<ControllerBinding>();

            // Act
            var results = binder.AnalyzeBindings(configItems, existingBindings);

            // Assert
            Assert.HasCount(1, results);
            var binding = results.Find(b => b.OriginalController.Equals(SerialNumber.CreateController("Board #/ SN-123")));
            Assert.AreEqual(ControllerBindingStatus.Missing, binding.Status);
        }

        #endregion

        #region ApplyBindingUpdate Tests

        [TestMethod]
        public void ApplyBindingUpdate_WithValidBindings_UpdatesAllMatchingConfigItems()
        {
            // Arrange
            var connectedControllers = new List<Controller> {
                SerialNumber.CreateController("Board #/ SN-123")
            };
            var binder = new ControllerAutoBinder(connectedControllers);

            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("Board #/ SN-OLD"),
                CreateConfigItem("Board #/ SN-OLD"),
                CreateConfigItem("OtherBoard #/ SN-999")
            };

            var controllerBindings = new List<ControllerBinding>
            {
                new ControllerBinding
                {
                    OriginalController = SerialNumber.CreateController("Board #/ SN-OLD"),
                    BoundController = SerialNumber.CreateController("Board #/ SN-NEW"),
                    Status = ControllerBindingStatus.AutoBind
                }
            };

            // Act
            binder.ApplyBindingUpdate(configItems, controllerBindings);

            // Assert
            Assert.AreEqual("Board #", configItems[0].Controller.Name, "First item Name should be unchanged");
            Assert.AreEqual("SN-NEW", configItems[0].Controller.Serial, "First item Serial should be updated");
            Assert.AreEqual("Board #", configItems[1].Controller.Name, "Second item Name should be unchanged");
            Assert.AreEqual("SN-NEW", configItems[1].Controller.Serial, "Second item Serial should be updated");
            Assert.AreEqual("OtherBoard #", configItems[2].Controller.Name, "Unmatched item Name should remain unchanged");
            Assert.AreEqual("SN-999", configItems[2].Controller.Serial, "Unmatched item Serial should remain unchanged");
        }

        [TestMethod]
        public void ApplyBindingUpdate_WithMultipleBindings_UpdatesEachCorrectly()
        {
            // Arrange
            var connectedControllers = new List<Controller>();
            var binder = new ControllerAutoBinder(connectedControllers);

            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("Board1 #/ SN-OLD1"),
                CreateConfigItem("Board2 #/ SN-OLD2"),
                CreateConfigItem("Board3 #/ SN-OLD3")
            };

            var controllerBindings = new List<ControllerBinding>
            {
                new ControllerBinding
                {
                    OriginalController = SerialNumber.CreateController("Board1 #/ SN-OLD1"),
                    BoundController = SerialNumber.CreateController("Board1 #/ SN-NEW1"),
                    Status = ControllerBindingStatus.AutoBind
                },
                new ControllerBinding
                {
                    OriginalController = SerialNumber.CreateController("Board2 #/ SN-OLD2"),
                    BoundController = SerialNumber.CreateController("Board2 #/ SN-NEW2"),
                    Status = ControllerBindingStatus.AutoBind
                },
                new ControllerBinding
                {
                    OriginalController = SerialNumber.CreateController("Board3 #/ SN-OLD3"),
                    BoundController = SerialNumber.CreateController("Board3 #/ SN-NEW3"),
                    Status = ControllerBindingStatus.Match
                }
            };

            // Act
            binder.ApplyBindingUpdate(configItems, controllerBindings);

            // Assert
            Assert.AreEqual("Board1 #", configItems[0].Controller.Name);
            Assert.AreEqual("SN-NEW1", configItems[0].Controller.Serial);
            Assert.AreEqual("Board2 #", configItems[1].Controller.Name);
            Assert.AreEqual("SN-NEW2", configItems[1].Controller.Serial);
            Assert.AreEqual("Board3 #", configItems[2].Controller.Name);
            Assert.AreEqual("SN-NEW3", configItems[2].Controller.Serial);
        }

        [TestMethod]
        public void ApplyBindingUpdate_WithEmptyBindings_MakesNoChanges()
        {
            // Arrange
            var connectedControllers = new List<Controller>();
            var binder = new ControllerAutoBinder(connectedControllers);

            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("Board #/ SN-123"),
                CreateConfigItem("OtherBoard #/ SN-456")
            };

            var controllerBindings = new List<ControllerBinding>();

            // Act
            binder.ApplyBindingUpdate(configItems, controllerBindings);

            // Assert
            Assert.AreEqual("Board #", configItems[0].Controller.Name);
            Assert.AreEqual("SN-123", configItems[0].Controller.Serial);
            Assert.AreEqual("OtherBoard #", configItems[1].Controller.Name);
            Assert.AreEqual("SN-456", configItems[1].Controller.Serial);
        }

        [TestMethod]
        public void ApplyBindingUpdate_IgnoresEmptyControllerSerials()
        {
            // Arrange
            var connectedControllers = new List<Controller>();
            var binder = new ControllerAutoBinder(connectedControllers);

            var configItems = new List<IConfigItem>
            {
                CreateConfigItem(""),
                CreateConfigItem(null),
                CreateConfigItem("Board #/ SN-OLD")
            };

            var controllerBindings = new List<ControllerBinding>
            {
                new ControllerBinding
                {
                    OriginalController = SerialNumber.CreateController("Board #/ SN-OLD"),
                    BoundController = SerialNumber.CreateController("Board #/ SN-NEW"),
                    Status = ControllerBindingStatus.AutoBind
                }
            };

            // Act
            binder.ApplyBindingUpdate(configItems, controllerBindings);

            // Assert
            Assert.AreEqual("", configItems[0].Controller?.Serial ?? "", "Empty serial should remain empty");
            Assert.IsNull(configItems[1].Controller, "Null controller should remain null");
            Assert.AreEqual("Board #", configItems[2].Controller.Name, "Valid Name should be unchanged");
            Assert.AreEqual("SN-NEW", configItems[2].Controller.Serial, "Valid Serial should be updated");
        }

        [TestMethod]
        public void ApplyBindingUpdate_IgnoresDashControllerSerials()
        {
            // Arrange
            var connectedControllers = new List<Controller>();
            var binder = new ControllerAutoBinder(connectedControllers);

            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("-"),
                CreateConfigItem("Board #/ SN-OLD")
            };

            var controllerBindings = new List<ControllerBinding>
            {
                new ControllerBinding
                {
                    OriginalController = SerialNumber.CreateController("-"),
                    BoundController = SerialNumber.CreateController("Board #/ SN-NEW"),
                    Status = ControllerBindingStatus.AutoBind
                },
                new ControllerBinding
                {
                    OriginalController = SerialNumber.CreateController("Board #/ SN-OLD"),
                    BoundController = SerialNumber.CreateController("Board #/ SN-NEW2"),
                    Status = ControllerBindingStatus.AutoBind
                }
            };

            // Act
            binder.ApplyBindingUpdate(configItems, controllerBindings);

            // Assert
            Assert.IsNull(configItems[0].Controller, "This controller should be null");
            Assert.AreEqual("Board #", configItems[1].Controller.Name, "Valid Name should be unchanged");
            Assert.AreEqual("SN-NEW2", configItems[1].Controller.Serial, "Valid Serial should be updated");
        }

        [TestMethod]
        public void ApplyBindingUpdate_WithNoMatchingBinding_LeavesConfigUnchanged()
        {
            // Arrange
            var connectedControllers = new List<Controller>();
            var binder = new ControllerAutoBinder(connectedControllers);

            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("Board #/ SN-123"),
                CreateConfigItem("OtherBoard #/ SN-456")
            };

            var controllerBindings = new List<ControllerBinding>
            {
                new ControllerBinding
                {
                    OriginalController = SerialNumber.CreateController("DifferentBoard #/ SN-999"),
                    BoundController = SerialNumber.CreateController("DifferentBoard #/ SN-000"),
                    Status = ControllerBindingStatus.AutoBind
                }
            };

            // Act
            binder.ApplyBindingUpdate(configItems, controllerBindings);

            // Assert
            Assert.AreEqual("Board #", configItems[0].Controller.Name, "Unmatched Name should remain unchanged");
            Assert.AreEqual("SN-123", configItems[0].Controller.Serial, "Unmatched Serial should remain unchanged");
            Assert.AreEqual("OtherBoard #", configItems[1].Controller.Name, "Unmatched Name should remain unchanged");
            Assert.AreEqual("SN-456", configItems[1].Controller.Serial, "Unmatched Serial should remain unchanged");
        }

        [TestMethod]
        public void ApplyBindingUpdate_WithMissingStatusBinding_StillUpdates()
        {
            // Arrange
            var connectedControllers = new List<Controller>();
            var binder = new ControllerAutoBinder(connectedControllers);

            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("Board #/ SN-OLD")
            };

            var controllerBindings = new List<ControllerBinding>
            {
                new ControllerBinding
                {
                    OriginalController = SerialNumber.CreateController("Board #/ SN-OLD"),
                    BoundController = SerialNumber.CreateController("Board #/ SN-NEW"),
                    Status = ControllerBindingStatus.Missing
                }
            };

            // Act
            binder.ApplyBindingUpdate(configItems, controllerBindings);

            // Assert
            Assert.AreEqual("Board #", configItems[0].Controller.Name, "Name should be unchanged");
            Assert.AreEqual("SN-NEW", configItems[0].Controller.Serial, "Should update Serial regardless of status");
        }

        [TestMethod]
        public void ApplyBindingUpdate_WithNullBoundController_SkipsUpdate()
        {
            // Arrange
            var connectedControllers = new List<Controller>();
            var binder = new ControllerAutoBinder(connectedControllers);

            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("Board #/ SN-OLD")
            };

            var controllerBindings = new List<ControllerBinding>
            {
                new ControllerBinding
                {
                    OriginalController = SerialNumber.CreateController("Board #/ SN-OLD"),
                    BoundController = null,
                    Status = ControllerBindingStatus.Missing
                }
            };

            // Act
            binder.ApplyBindingUpdate(configItems, controllerBindings);

            // Assert
            Assert.AreEqual("Board #", configItems[0].Controller.Name, "Name should remain unchanged");
            Assert.AreEqual("SN-OLD", configItems[0].Controller.Serial, "Should skip update when BoundController is null");
        }

        [TestMethod]
        public void ApplyBindingUpdate_IntegrationWithAnalyzeBindings_WorksTogether()
        {
            // This test validates that ApplyBindingUpdate works correctly with output from AnalyzeBindings
            // Arrange
            var connectedControllers = new List<Controller>
            {
                SerialNumber.CreateController("Board1 #/ SN-NEW1"),
                SerialNumber.CreateController("Board2 #/ SN-NEW2")
            };
            var binder = new ControllerAutoBinder(connectedControllers);

            var configItems = new List<IConfigItem>
            {
                CreateConfigItem("Board1 #/ SN-OLD1"),
                CreateConfigItem("Board2 #/ SN-OLD2"),
                CreateConfigItem("Board3 #/ SN-MISSING")
            };

            var existingBindings = new List<ControllerBinding>();

            // Act
            var analyzedBindings = binder.AnalyzeBindings(configItems, existingBindings);
            binder.ApplyBindingUpdate(configItems, analyzedBindings);

            // Assert
            Assert.AreEqual("Board1 #", configItems[0].Controller.Name, "Name should be unchanged");
            Assert.AreEqual("SN-NEW1", configItems[0].Controller.Serial, "Should be auto-bound");
            Assert.AreEqual("Board2 #", configItems[1].Controller.Name, "Name should be unchanged");
            Assert.AreEqual("SN-NEW2", configItems[1].Controller.Serial, "Should be auto-bound");
            Assert.AreEqual("Board3 #", configItems[2].Controller.Name, "Missing Name should remain unchanged");
            Assert.AreEqual("SN-MISSING", configItems[2].Controller.Serial, "Missing Serial should remain unchanged");
        }

        #endregion

        #region Helper Methods

        private IConfigItem CreateConfigItem(string moduleSerial)
        {
            return new OutputConfigItem
            {
                Controller = SerialNumber.CreateController(moduleSerial),
                Active = true,
                GUID = System.Guid.NewGuid().ToString()
            };
        }

        #endregion
    }
}