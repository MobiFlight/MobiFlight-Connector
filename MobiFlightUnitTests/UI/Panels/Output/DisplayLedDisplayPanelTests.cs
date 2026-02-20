using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.UI.Panels;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Tests
{
    /// <summary>
    /// Tests for DisplayLedDisplayPanel focusing on the FormatException fix.
    /// These tests verify that SetSizeDigits() doesn't throw FormatException
    /// when called with a valid list of entries and modules are connected.
    /// </summary>
    [TestClass()]
    public class DisplayLedDisplayPanelTests
    {
        [TestMethod()]
        public void SetSizeDigits_WithValidEntries_ShouldNotThrowException()
        {
            // Arrange
            // This test reproduces the bug scenario: SetSizeDigits is called with valid entries
            // (simulating connected modules). The bug was that this would trigger a
            // FormatException in DisplayLedModuleSize_SelectedIndexChanged.
            var panel = new DisplayLedDisplayPanel();
            var entries = new List<ListItem>
            {
                new ListItem { Label = "3", Value = "3" },
                new ListItem { Label = "4", Value = "4" },
                new ListItem { Label = "5", Value = "5" },
                new ListItem { Label = "6", Value = "6" }
            };

            // Act & Assert
            // Should not throw FormatException or any other exception
            panel.SetSizeDigits(entries);
            
            // Verify the ComboBox is properly configured
            var comboBox = panel.Controls.Find("displayLedModuleSizeComboBox", true)[0] as ComboBox;
            Assert.IsNotNull(comboBox, "Module size ComboBox should exist");
            Assert.HasCount(4, comboBox.Items, "ComboBox should have 4 items");
            Assert.AreEqual(3, comboBox.SelectedIndex, "Selected index should be last item (entries.Count-1)");
        }

        [TestMethod()]
        public void SetSizeDigits_WithEmptyList_ShouldNotThrowException()
        {
            // Arrange
            // This test verifies the scenario where no modules are connected (empty list).
            // In this case, the SelectedIndexChanged event should not fire.
            var panel = new DisplayLedDisplayPanel();
            var entries = new List<ListItem>();

            // Act & Assert
            // Should not throw exception
            panel.SetSizeDigits(entries);
            
            // Verify the ComboBox state
            var comboBox = panel.Controls.Find("displayLedModuleSizeComboBox", true)[0] as ComboBox;
            Assert.IsNotNull(comboBox, "Module size ComboBox should exist");
            Assert.IsFalse(comboBox.Enabled, "ComboBox should be disabled with empty list");
        }

        [TestMethod()]
        public void SetSizeDigits_WithSingleEntry_ShouldNotThrowException()
        {
            // Arrange
            var panel = new DisplayLedDisplayPanel();
            var entries = new List<ListItem>
            {
                new ListItem { Label = "4", Value = "4" }
            };

            // Act & Assert
            // Should not throw exception even with a single entry
            panel.SetSizeDigits(entries);
            
            // Verify the ComboBox state
            var comboBox = panel.Controls.Find("displayLedModuleSizeComboBox", true)[0] as ComboBox;
            Assert.IsNotNull(comboBox, "Module size ComboBox should exist");
            Assert.HasCount(1, comboBox.Items, "ComboBox should have 1 item");
            Assert.AreEqual(0, comboBox.SelectedIndex, "Selected index should be 0 for single entry");
            Assert.IsFalse(comboBox.Enabled, "ComboBox should be disabled with only 1 entry");
        }
    }
}
