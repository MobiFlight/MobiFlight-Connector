using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.OutputConfig;
using MobiFlight.UI.Panels;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Tests
{
    [TestClass()]
    public class DisplayLedDisplayPanelTests
    {
        [TestMethod()]
        public void SetSizeDigits_WithValidEntries_ShouldNotThrowException()
        {
            // Arrange
            var panel = new DisplayLedDisplayPanel();
            var entries = new List<ListItem>
            {
                new ListItem { Label = "3", Value = "3" },
                new ListItem { Label = "4", Value = "4" },
                new ListItem { Label = "5", Value = "5" },
                new ListItem { Label = "6", Value = "6" }
            };

            // Act & Assert - should not throw FormatException
            panel.SetSizeDigits(entries);
            
            // Verify the ComboBox is properly configured
            var comboBox = panel.Controls.Find("displayLedModuleSizeComboBox", true)[0] as ComboBox;
            Assert.IsNotNull(comboBox);
            Assert.AreEqual(4, comboBox.Items.Count);
            Assert.AreEqual(3, comboBox.SelectedIndex); // Should select last item (entries.Count-1)
        }

        [TestMethod()]
        public void SetSizeDigits_WithEmptyList_ShouldNotThrowException()
        {
            // Arrange
            var panel = new DisplayLedDisplayPanel();
            var entries = new List<ListItem>();

            // Act & Assert - should not throw exception
            panel.SetSizeDigits(entries);
            
            // Verify the ComboBox state
            var comboBox = panel.Controls.Find("displayLedModuleSizeComboBox", true)[0] as ComboBox;
            Assert.IsNotNull(comboBox);
            Assert.IsFalse(comboBox.Enabled); // Should be disabled with empty list
        }

        [TestMethod()]
        public void SetSizeDigits_TriggersSizeChange_UpdatesVisibility()
        {
            // Arrange
            var panel = new DisplayLedDisplayPanel();
            var entries = new List<ListItem>
            {
                new ListItem { Label = "3", Value = "3" },
                new ListItem { Label = "4", Value = "4" }
            };

            // Act
            panel.SetSizeDigits(entries);
            
            // Assert - verify that digit checkboxes are visible/hidden correctly
            // For size 4 (last entry), digits 0-3 should be visible, 4-7 should be hidden
            for (int i = 0; i < 8; i++)
            {
                var digitCheckBox = panel.Controls.Find($"displayLedDigit{i}CheckBox", true)[0] as CheckBox;
                var decimalCheckBox = panel.Controls.Find($"displayLedDecimalPoint{i}CheckBox", true)[0] as CheckBox;
                
                if (i < 4)
                {
                    Assert.IsTrue(digitCheckBox.Visible, $"Digit {i} should be visible");
                    Assert.IsTrue(decimalCheckBox.Visible, $"Decimal point {i} should be visible");
                }
                else
                {
                    Assert.IsFalse(digitCheckBox.Visible, $"Digit {i} should be hidden");
                    Assert.IsFalse(decimalCheckBox.Visible, $"Decimal point {i} should be hidden");
                }
            }
        }

        [TestMethod()]
        public void syncFromConfig_WithLedModule_ShouldSetControlsCorrectly()
        {
            // Arrange
            var panel = new DisplayLedDisplayPanel();
            var config = new OutputConfigItem
            {
                Device = new LedModule
                {
                    DisplayLedAddress = "1",
                    DisplayLedConnector = 1,
                    DisplayLedModuleSize = 4,
                    DisplayLedPadding = true,
                    DisplayLedReverseDigits = false,
                    DisplayLedPaddingChar = "0",
                    DisplayLedDigits = new List<string> { "0", "1", "2" },
                    DisplayLedDecimalPoints = new List<string> { "1", "2" }
                }
            };

            // Setup addresses first so the combo boxes have data
            var addresses = new List<ListItem>
            {
                new ListItem { Label = "LED #0", Value = "0" },
                new ListItem { Label = "LED #1", Value = "1" }
            };
            panel.SetAddresses(addresses);

            // Setup connectors
            var connectors = new List<ListItem>
            {
                new ListItem { Label = "1", Value = "1" },
                new ListItem { Label = "2", Value = "2" }
            };
            panel.SetConnectors(connectors);

            // Setup sizes
            var sizes = new List<ListItem>
            {
                new ListItem { Label = "3", Value = "3" },
                new ListItem { Label = "4", Value = "4" },
                new ListItem { Label = "5", Value = "5" }
            };
            panel.SetSizeDigits(sizes);

            // Act
            panel.syncFromConfig(config);

            // Assert
            Assert.IsTrue(panel.displayLedPaddingCheckBox.Checked);
            Assert.IsFalse(panel.displayLedReverseDigitsCheckBox.Checked);
            Assert.IsTrue(panel.displayLedDigit0CheckBox.Checked);
            Assert.IsTrue(panel.displayLedDigit1CheckBox.Checked);
            Assert.IsTrue(panel.displayLedDigit2CheckBox.Checked);
            Assert.IsFalse(panel.displayLedDigit3CheckBox.Checked);
        }
    }
}
