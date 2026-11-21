using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MobiFlight.Joysticks.Octavi.Tests
{
    [TestClass]
    public class OctaviHandlerTests
    {
        private const int ButtonCountBeforeRightColumnInEveryContext = 96;

        private static OctaviHandler CreateHandler()
        {
            // The contexts with a shift mode, as in octavi.joystick.json: COM1, COM2, NAV1, NAV2, XPDR
            var definition = new JoystickDefinition
            {
                Inputs = new[] { 0, 1, 2, 3, 7 }
                    .Select(id => new JoystickInput { Id = id, Type = JoystickDeviceType.Button })
                    .ToList()
            };
            return new OctaviHandler(definition);
        }

        private static OctaviReport Report(OctaviReport.OctaviState context, OctaviReport.OctaviButtons buttons = 0)
        {
            var value = (uint)buttons;
            var report = new OctaviReport();
            report.parseReport(new byte[]
            {
                0x01,
                (byte)value, (byte)(value >> 8), (byte)(value >> 16), (byte)(value >> 24),
                0x00, 0x00,
                (byte)context
            });
            return report;
        }

        private static List<(int buttonIndex, MobiFlightButton.InputEvent inputEvent)> Press(
            OctaviHandler handler, OctaviReport.OctaviState context, OctaviReport.OctaviButtons button)
        {
            return handler.DetectButtonEvents(Report(context, button)).ToList();
        }

        #region Button list

        [TestMethod]
        public void JoystickButtonNames_ShouldKeepEarlierButtonsFirst_WhenRightColumnIsAddedToEveryContext()
        {
            // Arrange
            var handler = CreateHandler();

            // Act
            var names = handler.JoystickButtonNames.ToList();

            // Assert
            Assert.AreEqual("Button_SHIFT_SW", names[0]);
            Assert.AreEqual("Button_UNSHIFT_SW", names[1]);
            Assert.AreEqual("Button_FMS1_DCT", names[names.IndexOf("Button_FMS1_CRSR") + 1]);
            Assert.AreEqual(ButtonCountBeforeRightColumnInEveryContext, names.IndexOf("Button_COM1_DCT"));
            Assert.AreEqual("Button_XPDR^_TOG", names[ButtonCountBeforeRightColumnInEveryContext - 1]);
        }

        [TestMethod]
        public void JoystickButtonNames_ShouldContainRightColumnOncePerNonFmsContext()
        {
            // Arrange
            var handler = CreateHandler();
            string[] contexts = ["COM1", "COM2", "NAV1", "NAV2", "AP", "XPDR", "COM1^", "COM2^", "NAV1^", "NAV2^", "XPDR^"];
            string[] keys = ["DCT", "MENU", "CLR", "ENT"];

            // Act
            var names = handler.JoystickButtonNames.ToList();

            // Assert
            Assert.HasCount(ButtonCountBeforeRightColumnInEveryContext + contexts.Length * keys.Length, names);
            foreach (var context in contexts)
            {
                foreach (var key in keys)
                {
                    Assert.Contains($"Button_{context}_{key}", names);
                }
            }
            Assert.DoesNotContain("Button_FMS1^_DCT", names);
        }

        #endregion

        #region Button events

        // The enums are internal, so the public test method takes their raw values
        [TestMethod]
        [DataRow((byte)OctaviReport.OctaviState.STATE_COM1, (uint)OctaviReport.OctaviButtons.HID_BTN_DCT, "Button_COM1_DCT")]
        [DataRow((byte)OctaviReport.OctaviState.STATE_NAV2, (uint)OctaviReport.OctaviButtons.HID_BTN_MENU, "Button_NAV2_MENU")]
        [DataRow((byte)OctaviReport.OctaviState.STATE_AP, (uint)OctaviReport.OctaviButtons.HID_BTN_CLR, "Button_AP_CLR")]
        [DataRow((byte)OctaviReport.OctaviState.STATE_XPDR, (uint)OctaviReport.OctaviButtons.HID_BTN_ENT, "Button_XPDR_ENT")]
        [DataRow((byte)OctaviReport.OctaviState.STATE_FMS1, (uint)OctaviReport.OctaviButtons.HID_BTN_DCT, "Button_FMS1_DCT")]
        [DataRow((byte)OctaviReport.OctaviState.STATE_FMS2, (uint)OctaviReport.OctaviButtons.HID_BTN_ENT, "Button_FMS2_ENT")]
        public void DetectButtonEvents_ShouldPressContextButton_WhenRightColumnKeyIsPressed(
            byte context, uint button, string expectedName)
        {
            // Arrange
            var handler = CreateHandler();
            var expectedIndex = handler.JoystickButtonNames.ToList().IndexOf(expectedName);

            // Act
            var events = Press(handler, (OctaviReport.OctaviState)context, (OctaviReport.OctaviButtons)button);

            // Assert
            Assert.Contains((expectedIndex, MobiFlightButton.InputEvent.PRESS), events);
        }

        [TestMethod]
        public void DetectButtonEvents_ShouldReleaseContextButton_WhenRightColumnKeyIsLetGo()
        {
            // Arrange
            var handler = CreateHandler();
            var expectedIndex = handler.JoystickButtonNames.ToList().IndexOf("Button_COM2_CLR");
            Press(handler, OctaviReport.OctaviState.STATE_COM2, OctaviReport.OctaviButtons.HID_BTN_CLR);

            // Act
            var events = handler.DetectButtonEvents(Report(OctaviReport.OctaviState.STATE_COM2)).ToList();

            // Assert
            Assert.Contains((expectedIndex, MobiFlightButton.InputEvent.RELEASE), events);
        }

        [TestMethod]
        public void DetectButtonEvents_ShouldPressShiftedContextButton_WhenRightColumnKeyIsPressedInShiftMode()
        {
            // Arrange
            var handler = CreateHandler();
            var expectedIndex = handler.JoystickButtonNames.ToList().IndexOf("Button_COM1^_DCT");
            handler.DetectButtonEvents(Report(OctaviReport.OctaviState.STATE_COM1));
            Press(handler, OctaviReport.OctaviState.STATE_COM1, OctaviReport.OctaviButtons.HID_ENC_SW);
            handler.DetectButtonEvents(Report(OctaviReport.OctaviState.STATE_COM1));

            // Act
            var events = Press(handler, OctaviReport.OctaviState.STATE_COM1, OctaviReport.OctaviButtons.HID_BTN_DCT);

            // Assert
            Assert.IsTrue(handler.IsInShiftMode);
            Assert.Contains((expectedIndex, MobiFlightButton.InputEvent.PRESS), events);
        }

        #endregion
    }
}
