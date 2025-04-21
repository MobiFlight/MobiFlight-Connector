using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

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