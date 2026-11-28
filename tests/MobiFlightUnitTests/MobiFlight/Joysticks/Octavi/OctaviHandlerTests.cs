using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MobiFlight.Joysticks.Octavi.Tests
{
    [TestClass]
    public class OctaviHandlerTests
    {
        private const int ButtonCountBeforeRightColumnInEveryContext = 96;
        private const int ButtonCountBeforeContextSelectionButtons = 140;
        private static readonly string[] ContextNames = ["COM1", "COM2", "NAV1", "NAV2", "FMS1", "FMS2", "AP", "XPDR"];

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

        private static OctaviReport Report(OctaviReport.OctaviState context, OctaviReport.OctaviButtons buttons = 0, sbyte outerEncoderDelta = 0)
        {
            var value = (uint)buttons;
            var report = new OctaviReport();
            report.parseReport(new byte[]
            {
                0x01,
                (byte)value, (byte)(value >> 8), (byte)(value >> 16), (byte)(value >> 24),
                (byte)outerEncoderDelta, 0x00,
                (byte)context
            });
            return report;
        }

        private static List<(int buttonIndex, MobiFlightButton.InputEvent inputEvent)> Press(
            OctaviHandler handler, OctaviReport.OctaviState context, OctaviReport.OctaviButtons button)
        {
            return handler.DetectButtonEvents(Report(context, button)).ToList();
        }

        private static (int buttonIndex, MobiFlightButton.InputEvent inputEvent)[] Tapped(OctaviHandler handler, string name)
        {
            var index = handler.JoystickButtonNames.ToList().IndexOf(name);
            return [(index, MobiFlightButton.InputEvent.PRESS), (index, MobiFlightButton.InputEvent.RELEASE)];
        }

        private static List<string> EventNames(OctaviHandler handler, IEnumerable<(int buttonIndex, MobiFlightButton.InputEvent inputEvent)> events)
        {
            var names = handler.JoystickButtonNames.ToList();
            return [.. events.Select(e => names[e.buttonIndex])];
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
            Assert.HasCount(ButtonCountBeforeRightColumnInEveryContext + contexts.Length * keys.Length + ContextNames.Length, names);
            foreach (var context in contexts)
            {
                foreach (var key in keys)
                {
                    Assert.Contains($"Button_{context}_{key}", names);
                }
            }
            Assert.DoesNotContain("Button_FMS1^_DCT", names);
        }

        [TestMethod]
        public void JoystickButtonNames_ShouldAddOneContextSelectionButtonPerContextAfterAllEarlierButtons()
        {
            // Arrange
            var handler = CreateHandler();

            // Act
            var names = handler.JoystickButtonNames.ToList();

            // Assert
            Assert.AreEqual("Button_XPDR^_ENT", names[ButtonCountBeforeContextSelectionButtons - 1]);
            CollectionAssert.AreEqual(
                ContextNames.Select(name => $"Button_CONTEXT_{name}").ToList(),
                names.GetRange(ButtonCountBeforeContextSelectionButtons, ContextNames.Length));
            Assert.DoesNotContain("Button_CONTEXT_COM1^", names);
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

        #region Context selection buttons

        // The enum is internal, so the public test method takes its raw value
        [TestMethod]
        [DataRow((byte)OctaviReport.OctaviState.STATE_COM1, "Button_CONTEXT_COM1")]
        [DataRow((byte)OctaviReport.OctaviState.STATE_FMS2, "Button_CONTEXT_FMS2")]
        public void DetectButtonEvents_ShouldPressContextSelectionButtonAfterUnshift_WhenFirstReportArrives(byte context, string expectedName)
        {
            // Arrange
            var handler = CreateHandler();
            var expected = Tapped(handler, "Button_UNSHIFT_SW").Concat(Tapped(handler, expectedName)).ToList();

            // Act
            var events = handler.DetectButtonEvents(Report((OctaviReport.OctaviState)context)).ToList();

            // Assert
            CollectionAssert.AreEqual(expected, events);
        }

        [TestMethod]
        public void DetectButtonEvents_ShouldUnshiftAndPressContextSelectionButton_WhenContextChangesInShiftMode()
        {
            // Arrange
            var handler = CreateHandler();
            handler.DetectButtonEvents(Report(OctaviReport.OctaviState.STATE_COM1));
            Press(handler, OctaviReport.OctaviState.STATE_COM1, OctaviReport.OctaviButtons.HID_ENC_SW);
            handler.DetectButtonEvents(Report(OctaviReport.OctaviState.STATE_COM1));
            var expected = Tapped(handler, "Button_UNSHIFT_SW").Concat(Tapped(handler, "Button_CONTEXT_NAV1")).ToList();

            // Act
            var events = handler.DetectButtonEvents(Report(OctaviReport.OctaviState.STATE_NAV1)).ToList();

            // Assert
            Assert.IsFalse(handler.IsInShiftMode);
            CollectionAssert.AreEqual(expected, events);
        }

        [TestMethod]
        public void DetectButtonEvents_ShouldPressContextSelectionButtonAfterUnshift_WhenContextChanges()
        {
            // Arrange
            var handler = CreateHandler();
            handler.DetectButtonEvents(Report(OctaviReport.OctaviState.STATE_COM1));
            var expected = Tapped(handler, "Button_UNSHIFT_SW").Concat(Tapped(handler, "Button_CONTEXT_AP")).ToList();

            // Act
            var events = handler.DetectButtonEvents(Report(OctaviReport.OctaviState.STATE_AP)).ToList();

            // Assert
            CollectionAssert.AreEqual(expected, events);
        }

        [TestMethod]
        public void DetectButtonEvents_ShouldPressContextSelectionButtonBeforeTheReportsKeys_WhenContextChangesWithAKeyDown()
        {
            // Arrange
            var handler = CreateHandler();
            handler.DetectButtonEvents(Report(OctaviReport.OctaviState.STATE_COM1));
            var names = handler.JoystickButtonNames.ToList();

            // Act
            var events = Press(handler, OctaviReport.OctaviState.STATE_NAV1, OctaviReport.OctaviButtons.HID_BTN_AP_HDG);

            // Assert
            var context = events.IndexOf((names.IndexOf("Button_CONTEXT_NAV1"), MobiFlightButton.InputEvent.PRESS));
            var key = events.IndexOf((names.IndexOf("Button_AP_HDG"), MobiFlightButton.InputEvent.PRESS));
            Assert.IsGreaterThanOrEqualTo(0, context);
            Assert.IsGreaterThan(context, key);
        }

        [TestMethod]
        public void DetectButtonEvents_ShouldNotPressContextSelectionButton_WhenContextStaysTheSame()
        {
            // Arrange
            var handler = CreateHandler();
            handler.DetectButtonEvents(Report(OctaviReport.OctaviState.STATE_COM1));

            // Act
            var events = Press(handler, OctaviReport.OctaviState.STATE_COM1, OctaviReport.OctaviButtons.HID_BTN_AP_VS);

            // Assert
            Assert.DoesNotContain(name => name.StartsWith("Button_CONTEXT_"), EventNames(handler, events));
        }

        [TestMethod]
        public void DetectButtonEvents_ShouldNotPressContextSelectionButton_WhenShiftModeToggles()
        {
            // Arrange
            var handler = CreateHandler();
            handler.DetectButtonEvents(Report(OctaviReport.OctaviState.STATE_NAV2));

            // Act
            var events = Press(handler, OctaviReport.OctaviState.STATE_NAV2, OctaviReport.OctaviButtons.HID_ENC_SW);

            // Assert
            Assert.IsTrue(handler.IsInShiftMode);
            Assert.DoesNotContain(name => name.StartsWith("Button_CONTEXT_"), EventNames(handler, events));
        }

        [TestMethod]
        public void DetectButtonEvents_ShouldNotPressContextSelectionButton_WhenContextIsUnknown()
        {
            // Arrange
            var handler = CreateHandler();

            // Act
            var events = handler.DetectButtonEvents(Report((OctaviReport.OctaviState)9)).ToList();

            // Assert
            Assert.DoesNotContain(name => name.StartsWith("Button_CONTEXT_"), EventNames(handler, events));
        }

        [TestMethod]
        public void DetectButtonEvents_ShouldIgnoreEncoderAndKeepTrackOfKeys_WhenContextIsUnknown()
        {
            // Arrange
            var handler = CreateHandler();
            handler.DetectButtonEvents(Report(OctaviReport.OctaviState.STATE_COM1));
            var unknown = (OctaviReport.OctaviState)9;

            // Act
            var events = handler.DetectButtonEvents(Report(unknown, OctaviReport.OctaviButtons.HID_BTN_AP, outerEncoderDelta: 1)).ToList();
            var afterwards = handler.DetectButtonEvents(Report(OctaviReport.OctaviState.STATE_COM1, OctaviReport.OctaviButtons.HID_BTN_AP)).ToList();

            // Assert
            CollectionAssert.AreEqual(Tapped(handler, "Button_UNSHIFT_SW").ToList(), events);
            CollectionAssert.AreEqual(
                Tapped(handler, "Button_UNSHIFT_SW").Concat(Tapped(handler, "Button_CONTEXT_COM1")).ToList(),
                afterwards);
        }

        [TestMethod]
        public void DetectButtonEvents_ShouldShiftTheNewContext_WhenKnobIsPushedWhileContextChanges()
        {
            // Arrange
            var handler = CreateHandler();
            handler.DetectButtonEvents(Report(OctaviReport.OctaviState.STATE_COM1));
            var names = handler.JoystickButtonNames.ToList();
            var expected = Tapped(handler, "Button_UNSHIFT_SW")
                .Concat(Tapped(handler, "Button_CONTEXT_NAV1"))
                .Append((names.IndexOf("Button_SHIFT_SW"), MobiFlightButton.InputEvent.PRESS))
                .ToList();

            // Act
            var events = Press(handler, OctaviReport.OctaviState.STATE_NAV1, OctaviReport.OctaviButtons.HID_ENC_SW);

            // Assert
            Assert.IsTrue(handler.IsInShiftMode);
            CollectionAssert.AreEqual(expected, events);
        }

        [TestMethod]
        public void DetectButtonEvents_ShouldPressTheEncoderButtonForTheFullDelta_WhenEncoderTurnsFast()
        {
            // Arrange
            var handler = CreateHandler();
            handler.DetectButtonEvents(Report(OctaviReport.OctaviState.STATE_COM1));
            var decrement = handler.JoystickButtonNames.ToList().IndexOf("Button_COM1_OD");

            // Act
            var events = handler.DetectButtonEvents(Report(OctaviReport.OctaviState.STATE_COM1, outerEncoderDelta: sbyte.MinValue)).ToList();

            // Assert
            Assert.HasCount(128, events);
            Assert.IsTrue(events.All(e => e == (decrement, MobiFlightButton.InputEvent.PRESS)));
        }

        #endregion

        #region Context report request

        [TestMethod]
        public void DetectButtonEvents_ShouldPressOnlyTheContextSelectionButton_WhenContextReportIsRequestedInShiftMode()
        {
            // Arrange
            var handler = CreateHandler();
            handler.DetectButtonEvents(Report(OctaviReport.OctaviState.STATE_NAV2));
            Press(handler, OctaviReport.OctaviState.STATE_NAV2, OctaviReport.OctaviButtons.HID_ENC_SW);
            handler.DetectButtonEvents(Report(OctaviReport.OctaviState.STATE_NAV2));
            handler.RequestContextReport();

            // Act
            var events = handler.DetectButtonEvents(Report(OctaviReport.OctaviState.STATE_NAV2)).ToList();

            // Assert
            Assert.IsTrue(handler.IsInShiftMode);
            CollectionAssert.AreEqual(Tapped(handler, "Button_CONTEXT_NAV2").ToList(), events);
        }

        [TestMethod]
        public void DetectButtonEvents_ShouldPressContextSelectionButtonBeforeTheReportsKeys_WhenContextReportIsRequested()
        {
            // Arrange
            var handler = CreateHandler();
            handler.DetectButtonEvents(Report(OctaviReport.OctaviState.STATE_COM2));
            handler.RequestContextReport();
            var names = handler.JoystickButtonNames.ToList();
            var expected = Tapped(handler, "Button_CONTEXT_COM2")
                .Append((names.IndexOf("Button_AP_AP"), MobiFlightButton.InputEvent.PRESS))
                .ToList();

            // Act
            var events = Press(handler, OctaviReport.OctaviState.STATE_COM2, OctaviReport.OctaviButtons.HID_BTN_AP);

            // Assert
            CollectionAssert.AreEqual(expected, events);
        }

        [TestMethod]
        public void DetectButtonEvents_ShouldPressContextSelectionButtonOnce_WhenContextReportIsRequested()
        {
            // Arrange
            var handler = CreateHandler();
            handler.DetectButtonEvents(Report(OctaviReport.OctaviState.STATE_XPDR));
            handler.RequestContextReport();
            handler.DetectButtonEvents(Report(OctaviReport.OctaviState.STATE_XPDR));

            // Act
            var events = handler.DetectButtonEvents(Report(OctaviReport.OctaviState.STATE_XPDR)).ToList();

            // Assert
            Assert.IsEmpty(events);
        }

        #endregion
    }
}
