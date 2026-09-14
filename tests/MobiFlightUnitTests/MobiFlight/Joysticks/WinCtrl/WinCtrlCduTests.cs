using System.Collections.Generic;
using MobiFlightWwFcu;
using Newtonsoft.Json.Linq;
using WebSocketSharp.Server;

namespace MobiFlight.Joysticks.WinCtrl.Tests
{
    [TestClass]
    public class WinCtrlCduTests
    {
        // Captures the JSON ShowUserMessage would otherwise send to the display pipeline.
        private class TestableWinCtrlCdu : WinCtrlCdu
        {
            public List<string> CapturedLcdValues = new List<string>();

            public TestableWinCtrlCdu(JoystickDefinition def, WebSocketServer server)
                : base(null, def, WinCtrlConstants.PRODUCT_ID_MCDU_CPT, server) { }

            public override void SetLcdDisplay(string address, string value)
            {
                CapturedLcdValues.Add(value);
            }
        }

        private static TestableWinCtrlCdu CreateDevice()
        {
            // Never started, so the port number is never actually bound.
            var server = new WebSocketServer(System.Net.IPAddress.Loopback, 8320);
            var definition = new JoystickDefinition();
            return new TestableWinCtrlCdu(definition, server);
        }

        #region Payload structure

        [TestMethod]
        public void ShowUserMessage_SendsExactlyOneLcdUpdate()
        {
            // Arrange
            var device = CreateDevice();

            // Act
            device.ShowUserMessage(UserMessageCodes.PYTHON_NOT_READY);

            // Assert
            Assert.HasCount(1, device.CapturedLcdValues);
        }

        [TestMethod]
        public void ShowUserMessage_Produces336CellDisplayPayload()
        {
            // Arrange
            var device = CreateDevice();

            // Act
            device.ShowUserMessage(UserMessageCodes.PYTHON_NOT_READY);

            // Assert
            var json = JObject.Parse(device.CapturedLcdValues[0]);
            var data = (JArray)json["Data"];
            Assert.AreEqual("Display", (string)json["Target"]);
            Assert.HasCount(24 * 14, data);
        }

        [TestMethod]
        public void ShowUserMessage_BlankCellsSerializeAsEmptyArray()
        {
            // Arrange
            var device = CreateDevice();

            // Act
            device.ShowUserMessage(UserMessageCodes.PYTHON_NOT_READY);

            // Assert
            var json = JObject.Parse(device.CapturedLcdValues[0]);
            var data = (JArray)json["Data"];

            // Last row is always blank for this short message, and blank cells are [].
            var lastCell = data[data.Count - 1];
            Assert.IsFalse(lastCell.HasValues);
        }

        #endregion

        #region Word wrap and formatting

        [TestMethod]
        public void ShowUserMessage_FirstWordOfMessageStartsOnItsOwnLine()
        {
            // Arrange
            var device = CreateDevice();

            // Act
            device.ShowUserMessage(UserMessageCodes.PYTHON_NOT_READY);

            // Assert
            // "Error: Python not ready. Check logging." -> first token "ERROR:" gets its own
            // line, then a blank line, per SendToDisplay/ShowUserMessage's word-wrap.
            string firstRowText = RowText(device.CapturedLcdValues[0], FirstNonEmptyRow(device.CapturedLcdValues[0]));
            Assert.StartsWith("ERROR:", firstRowText.Trim());
        }

        [TestMethod]
        public void ShowUserMessage_SubstitutesFormatParameters()
        {
            // Arrange
            var device = CreateDevice();

            // Act
            device.ShowUserMessage(UserMessageCodes.STARTING_SCRIPT, "pmdg_737_winwing_cdu.py");

            // Assert
            string allText = string.Concat(AllRowTexts(device.CapturedLcdValues[0]));
            Assert.Contains("PMDG_737_WINWING_CDU.PY", allText);
        }

        [TestMethod]
        public void ShowUserMessage_TokenLongerThan24Chars_TruncatesToLineWidth()
        {
            // Arrange
            var device = CreateDevice();

            // Act
            // SCRIPT_START_FAILED's template appends "." right after {0}, so a 30-char
            // parameter forms a 31-char token, which word-wrap truncates to the 24-column
            // line width: Substring(0, COLUMNS) up front, before any wrapping/padding.
            device.ShowUserMessage(UserMessageCodes.SCRIPT_START_FAILED, new string('A', 30));

            // Assert
            string allText = string.Concat(AllRowTexts(device.CapturedLcdValues[0]));
            Assert.Contains(new string('A', 24), allText);
            Assert.DoesNotContain(new string('A', 25), allText);
        }

        #endregion

        #region Helpers

        // Renders every one of the 14 rows as plain text, for substring assertions.
        private static string[] AllRowTexts(string json)
        {
            var rows = new string[14];
            for (int r = 0; r < 14; r++)
            {
                rows[r] = RowText(json, r);
            }
            return rows;
        }

        // Finds the first row with visible text, skipping the blank rows SendToDisplay
        // adds up top to vertically center short messages.
        private static int FirstNonEmptyRow(string json)
        {
            for (int r = 0; r < 14; r++)
            {
                if (RowText(json, r).Trim().Length > 0) return r;
            }
            return 0;
        }

        // Renders one 24-cell row as plain text; blank cells ([]) become spaces.
        private static string RowText(string json, int row)
        {
            var data = (JArray)JObject.Parse(json)["Data"];
            var chars = new char[24];
            for (int c = 0; c < 24; c++)
            {
                var cell = data[row * 24 + c];
                chars[c] = cell.HasValues ? cell[0].Value<char>() : ' ';
            }
            return new string(chars);
        }

        #endregion
    }
}
