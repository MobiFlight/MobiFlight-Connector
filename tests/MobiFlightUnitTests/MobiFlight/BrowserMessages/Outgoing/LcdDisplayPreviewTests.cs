using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.BrowserMessages.Outgoing;
using Newtonsoft.Json;

namespace MobiFlightUnitTests.MobiFlight.BrowserMessages.Outgoing
{
    [TestClass]
    public class LcdDisplayPreviewTests
    {
        #region FromDisplayText

        [TestMethod]
        public void FromDisplayText_SplitsTextIntoRowsOfColsCharacters()
        {
            // Arrange
            var displayText = "COM1: 123.45        " +
                              "COM2:   1.23        ";

            // Act
            var preview = LcdDisplayPreview.FromDisplayText("guid-1", displayText, 20);

            // Assert
            Assert.AreEqual("guid-1", preview.GUID);
            Assert.AreEqual(2, preview.Lines.Count);
            Assert.AreEqual("COM1: 123.45        ", preview.Lines[0]);
            Assert.AreEqual("COM2:   1.23        ", preview.Lines[1]);
        }

        [TestMethod]
        public void FromDisplayText_KeepsShorterLastRow()
        {
            // Arrange
            var displayText = "1234567890123456" + "abc";

            // Act
            var preview = LcdDisplayPreview.FromDisplayText("guid-2", displayText, 16);

            // Assert
            Assert.AreEqual(2, preview.Lines.Count);
            Assert.AreEqual("abc", preview.Lines[1]);
        }

        [TestMethod]
        public void FromDisplayText_NullTextGivesNoRows()
        {
            // Arrange
            string displayText = null;

            // Act
            var preview = LcdDisplayPreview.FromDisplayText("guid-3", displayText, 16);

            // Assert
            Assert.AreEqual("guid-3", preview.GUID);
            Assert.AreEqual(0, preview.Lines.Count);
        }

        #endregion

        #region Serialization

        [TestMethod]
        public void LcdDisplayPreview_Json_SerializesGuidAndLines()
        {
            // Arrange
            var preview = LcdDisplayPreview.FromDisplayText("guid-4", "ab" + "cd", 2);

            // Act
            var json = JsonConvert.SerializeObject(preview);

            // Assert
            Assert.Contains("\"GUID\":\"guid-4\"", json);
            Assert.Contains("\"Lines\":[\"ab\",\"cd\"]", json);
        }

        #endregion
    }
}
