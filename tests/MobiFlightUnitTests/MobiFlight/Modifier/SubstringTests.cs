using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace MobiFlight.Modifier.Tests
{
    [TestClass()]
    public class SubstringTests
    {
        #region Apply

        [TestMethod()]
        [DataRow(FSUIPCOffsetType.Integer, 12345d, "234")]
        [DataRow(FSUIPCOffsetType.Float, 12345d, "234")]
        public void Apply_ShouldReturnSubstringAsString_WhenValueIsNumeric(FSUIPCOffsetType type, double number, string expected)
        {
            // Arrange
            var substring = new Substring { Active = true, Start = 1, End = 4 };
            var value = new ConnectorValue { type = type, Float64 = number };

            // Act
            var result = substring.Apply(value, new List<ConfigRefValue>());

            // Assert
            Assert.AreEqual(FSUIPCOffsetType.String, result.type);
            Assert.AreEqual(expected, result.String);
            Assert.AreEqual(expected, result.ToString());
        }

        [TestMethod()]
        public void Apply_ShouldNotChangeInputValue_WhenValueIsNumeric()
        {
            // Arrange
            var substring = new Substring { Active = true, Start = 1, End = 4 };
            var value = new ConnectorValue { type = FSUIPCOffsetType.Integer, Float64 = 12345 };

            // Act
            substring.Apply(value, new List<ConfigRefValue>());

            // Assert
            Assert.AreEqual(FSUIPCOffsetType.Integer, value.type);
            Assert.AreEqual(12345d, value.Float64);
            Assert.IsNull(value.String);
        }

        [TestMethod()]
        [DataRow("ABCDEFGH", 0, 5, "ABCDE")]
        [DataRow("ABCDEFGH", 2, 4, "CD")]
        [DataRow("ABCDEFGH", 5, 20, "FGH")]
        [DataRow("ABCDEFGH", 8, 10, "")]
        [DataRow("ABCDEFGH", 9, 10, "")]
        public void Apply_ShouldReturnSubstring_WhenValueIsString(string text, int start, int end, string expected)
        {
            // Arrange
            var substring = new Substring { Active = true, Start = start, End = end };
            var value = new ConnectorValue { type = FSUIPCOffsetType.String, String = text };

            // Act
            var result = substring.Apply(value, new List<ConfigRefValue>());

            // Assert
            Assert.AreEqual(FSUIPCOffsetType.String, result.type);
            Assert.AreEqual(expected, result.String);
        }

        [TestMethod()]
        public void Apply_ShouldReturnInputValue_WhenNotActive()
        {
            // Arrange
            var substring = new Substring { Active = false, Start = 1, End = 4 };
            var value = new ConnectorValue { type = FSUIPCOffsetType.Integer, Float64 = 12345 };

            // Act
            var result = substring.Apply(value, new List<ConfigRefValue>());

            // Assert
            Assert.AreSame(value, result);
        }

        #endregion
    }
}
