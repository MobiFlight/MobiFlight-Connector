using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Config;
using MobiFlight.InputConfig;
using MobiFlight.UI.Panels.Input;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace MobiFlight.UI.Dialogs.Tests
{
    [TestClass()]
    public class InputConfigWizardTests
    {
        [TestMethod()]
        public void InputConfigWizardTest()
        {
            // Arrange
            var executionManagerMock = new Mock<IExecutionManager>();
            var inputConfigItem = new InputConfigItem { GUID = "test-guid" };
            var outputConfigItems = new List<OutputConfigItem>
            {
                new OutputConfigItem { GUID = "output-guid-1", Name = "Output 1" },
                new OutputConfigItem { GUID = "output-guid-2", Name = "Output 2" },
                new OutputConfigItem { GUID = "test-guid", Name = "Ignore me, because I am the same GUID as the input config item." }
            };

            // Mock the GetAvailableVariables method
            var availableVariables = new Dictionary<string, MobiFlightVariable>
            {
                { "var1", new MobiFlightVariable { Name = "Variable 1" } },
                { "var2", new MobiFlightVariable { Name = "Variable 2" } }
            };
            
            // Act
            var wizard = new InputConfigWizard(executionManagerMock.Object, 
                inputConfigItem, 
                null, 
                null,
                outputConfigItems,
                availableVariables
                );

            // Assert
            var expectedList = new List<ListItem>
            {
                new ListItem { Label = "Output 1", Value = "output-guid-1" },
                new ListItem { Label = "Output 2", Value = "output-guid-2" }
            };
            Assert.IsTrue(expectedList.SequenceEqual(wizard.PreconditionPanel.Configs, new ListItemEqualityComparer()));

            expectedList.Add(new ListItem { Label = outputConfigItems[2].Name, Value = outputConfigItems[2].GUID });

            Assert.IsFalse(expectedList.SequenceEqual(wizard.PreconditionPanel.Configs, new ListItemEqualityComparer()));
        }

        [TestMethod()]
        public void InputTypeComboBox_SelectedIndexChanged_AllPanelTypesGetInitialized()
        {
            // Arrange
            var executionManagerMock = new Mock<IExecutionManager>();
            var availableVariables = new Dictionary<string, MobiFlightVariable>
            {
                { "var1", new MobiFlightVariable { Name = "Variable 1" } }
            };
            
            executionManagerMock.Setup(em => em.GetAvailableVariables()).Returns(availableVariables);
            
            var inputConfigItem = new InputConfigItem 
            { 
                GUID = "test-guid",
                ModuleSerial = "TestModule / TestSerial",
                button = new ButtonInputConfig(),
                encoder = new EncoderInputConfig(),
                analog = new AnalogInputConfig(),
                inputShiftRegister = new InputShiftRegisterConfig(),
                inputMultiplexer = new InputMultiplexerConfig()
            };

            var wizard = new InputConfigWizard(executionManagerMock.Object, 
                inputConfigItem, 
                null, 
                null,
                new List<OutputConfigItem>(),
                availableVariables);

            // Get private fields using reflection
            var inputTypeComboBoxField = typeof(InputConfigWizard).GetField("inputTypeComboBox", BindingFlags.NonPublic | BindingFlags.Instance);
            var inputTypeComboBox = (ComboBox)inputTypeComboBoxField.GetValue(wizard);
            
            var inputModuleNameComboBoxField = typeof(InputConfigWizard).GetField("inputModuleNameComboBox", BindingFlags.NonPublic | BindingFlags.Instance);
            var inputModuleNameComboBox = (ComboBox)inputModuleNameComboBoxField.GetValue(wizard);
            
            var groupBoxInputSettingsField = typeof(InputConfigWizard).GetField("groupBoxInputSettings", BindingFlags.NonPublic | BindingFlags.Instance);
            var groupBoxInputSettings = (GroupBox)groupBoxInputSettingsField.GetValue(wizard);

            // Set up the module combo box to have a serial
            inputModuleNameComboBox.Items.Add("TestModule / TestSerial");
            inputModuleNameComboBox.SelectedIndex = 0;

            // Test cases for each device type
            var testCases = new List<(DeviceType DeviceType, IBaseDevice Device)>
            {
                (DeviceType.Button, new Config.Button() { Name = "TestButton" }),
                (DeviceType.Encoder, new Config.Encoder() { Name = "TestEncoder" }),
                (DeviceType.AnalogInput, new Config.AnalogInput() { Name = "TestAnalog" }),
                (DeviceType.InputShiftRegister, new Config.InputShiftRegister() { Name = "TestShifter", NumModules = "1" }),
                (DeviceType.InputMultiplexer, new Config.InputMultiplexer() { Name = "TestMultiplexer", NumBytes = "1" })
            };

            foreach (var testCase in testCases)
            {
                // Reset the mock call counts for each test case
                executionManagerMock.Invocations.Clear();

                // Act: Set up the input type combo box with the test device
                inputTypeComboBox.Items.Clear();
                inputTypeComboBox.Items.Add(new ListItem<IBaseDevice> { Label = testCase.Device.Label, Value = testCase.Device });
                inputTypeComboBox.SelectedIndex = 0;

                // Trigger the event handler via reflection
                var methodInfo = typeof(InputConfigWizard).GetMethod("inputTypeComboBox_SelectedIndexChanged", BindingFlags.NonPublic | BindingFlags.Instance);
                methodInfo.Invoke(wizard, new object[] { inputTypeComboBox, EventArgs.Empty });

                // Assert: Check that a panel was created and it implements IInputPanel
                Assert.AreEqual(1, groupBoxInputSettings.Controls.Count, $"Expected 1 control for {testCase.DeviceType}, got {groupBoxInputSettings.Controls.Count}");
                
                var panel = groupBoxInputSettings.Controls[0];
                Assert.IsInstanceOfType(panel, typeof(IInputPanel), $"Panel for {testCase.DeviceType} should implement IInputPanel");
                
                // Verify the panel is the correct type
                switch (testCase.DeviceType)
                {
                    case DeviceType.Button:
                    case DeviceType.InputShiftRegister:
                    case DeviceType.InputMultiplexer:
                        Assert.IsInstanceOfType(panel, typeof(ButtonPanel), $"Panel for {testCase.DeviceType} should be ButtonPanel");
                        break;
                    case DeviceType.Encoder:
                        Assert.IsInstanceOfType(panel, typeof(EncoderPanel), $"Panel for {testCase.DeviceType} should be EncoderPanel");
                        break;
                    case DeviceType.AnalogInput:
                        Assert.IsInstanceOfType(panel, typeof(AnalogPanel), $"Panel for {testCase.DeviceType} should be AnalogPanel");
                        break;
                }

                // Verify that Init was called by checking that the panel has been initialized
                // We can verify this by checking if the execution manager was stored in the panel
                VerifyPanelInitialization(panel, executionManagerMock.Object, testCase.DeviceType);

                // Verify that GetAvailableVariables was called for this panel
                executionManagerMock.Verify(em => em.GetAvailableVariables(), Times.AtLeastOnce, 
                    $"GetAvailableVariables should be called at least once for {testCase.DeviceType}");
            }
        }

        private void VerifyPanelInitialization(Control panel, IExecutionManager executionManager, DeviceType deviceType)
        {
            // Use reflection to check that the _executionManager field was set
            // This confirms that Init() was called
            var executionManagerField = panel.GetType().GetField("_executionManager", BindingFlags.NonPublic | BindingFlags.Instance);
            
            Assert.IsNotNull(executionManagerField, $"Panel {deviceType} should have an _executionManager field");
            
            var storedExecutionManager = executionManagerField.GetValue(panel);
            Assert.AreSame(executionManager, storedExecutionManager, 
                $"Panel {deviceType} should have the execution manager set via Init() call");
        }

        public class ListItemEqualityComparer : IEqualityComparer<ListItem>
        {
            public bool Equals(ListItem x, ListItem y)
            {
                if (x == null || y == null) return x == y;
                return x.Label == y.Label && x.Value == y.Value;
            }

            public int GetHashCode(ListItem obj)
            {
                return (obj.Label, obj.Value).GetHashCode();
            }
        }

    }
}