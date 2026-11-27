namespace MobiFlight.Joysticks.Logitech.Tests
{
    [TestClass]
    public class RadioPanelModeInputsTests
    {
        private const int UpperInnerCw = 16;
        private const int UpperOuterCcw = 19;
        private const int UpperActStby = 14;
        private const int LowerInnerCw = 20;

        private static JoystickState Physical(params int[] pressedButtons)
        {
            var state = new JoystickState();
            foreach (var button in pressedButtons)
            {
                state.Buttons[button] = true;
            }

            return state;
        }

        private static int[] PressedModeButtons(JoystickState state)
        {
            return Enumerable
                .Range(RadioPanelModeInputs.FirstButton, RadioPanelModeInputs.ButtonCount)
                .Where(index => state.Buttons[index])
                .ToArray();
        }

        #region GetButtonIndex

        [TestMethod]
        public void GetButtonIndex_FollowsModeThenControlOrder()
        {
            Assert.AreEqual(24, RadioPanelModeInputs.GetButtonIndex(RadioPanelMode.Com1, RadioPanelControl.InnerCw));
            Assert.AreEqual(28, RadioPanelModeInputs.GetButtonIndex(RadioPanelMode.Com1, RadioPanelControl.ActStby));
            Assert.AreEqual(29, RadioPanelModeInputs.GetButtonIndex(RadioPanelMode.Com2, RadioPanelControl.InnerCw));
            Assert.AreEqual(58, RadioPanelModeInputs.GetButtonIndex(RadioPanelMode.Xpdr, RadioPanelControl.ActStby));
        }

        #endregion

        #region Apply

        [TestMethod]
        public void Apply_EncoderPulse_PressesButtonOfRowMode()
        {
            // Arrange
            var inputs = new RadioPanelModeInputs();
            var state = Physical(UpperInnerCw);

            // Act
            inputs.Apply(state, RadioPanelMode.Nav1, RadioPanelMode.Com1);

            // Assert
            CollectionAssert.AreEqual(
                new[] { RadioPanelModeInputs.GetButtonIndex(RadioPanelMode.Nav1, RadioPanelControl.InnerCw) },
                PressedModeButtons(state));
        }

        [TestMethod]
        public void Apply_KeepsPhysicalButtons()
        {
            // Arrange
            var inputs = new RadioPanelModeInputs();
            var state = Physical(UpperOuterCcw, UpperActStby);

            // Act
            inputs.Apply(state, RadioPanelMode.Com1, RadioPanelMode.Com1);

            // Assert
            Assert.IsTrue(state.Buttons[UpperOuterCcw]);
            Assert.IsTrue(state.Buttons[UpperActStby]);
        }

        [TestMethod]
        public void Apply_LowerRow_UsesLowerMode()
        {
            // Arrange
            var inputs = new RadioPanelModeInputs();
            var state = Physical(LowerInnerCw);

            // Act
            inputs.Apply(state, RadioPanelMode.Com1, RadioPanelMode.Xpdr);

            // Assert
            CollectionAssert.AreEqual(
                new[] { RadioPanelModeInputs.GetButtonIndex(RadioPanelMode.Xpdr, RadioPanelControl.InnerCw) },
                PressedModeButtons(state));
        }

        [TestMethod]
        public void Apply_BothRowsOnSameMode_ShareTheModeButton()
        {
            // Arrange
            var inputs = new RadioPanelModeInputs();
            var state = Physical(UpperInnerCw, LowerInnerCw);

            // Act
            inputs.Apply(state, RadioPanelMode.Com2, RadioPanelMode.Com2);

            // Assert
            CollectionAssert.AreEqual(
                new[] { RadioPanelModeInputs.GetButtonIndex(RadioPanelMode.Com2, RadioPanelControl.InnerCw) },
                PressedModeButtons(state));
        }

        [TestMethod]
        public void Apply_ActStby_PressesModeActStby()
        {
            // Arrange
            var inputs = new RadioPanelModeInputs();
            var state = Physical(UpperActStby);

            // Act
            inputs.Apply(state, RadioPanelMode.Adf, RadioPanelMode.Com1);

            // Assert
            CollectionAssert.AreEqual(
                new[] { RadioPanelModeInputs.GetButtonIndex(RadioPanelMode.Adf, RadioPanelControl.ActStby) },
                PressedModeButtons(state));
        }

        [TestMethod]
        public void Apply_SelectorMovedWhileHeld_StaysOnModeItWasPressedIn()
        {
            // Arrange
            var inputs = new RadioPanelModeInputs();
            inputs.Apply(Physical(UpperActStby), RadioPanelMode.Com1, RadioPanelMode.Com1);
            var stillHeld = Physical(UpperActStby);

            // Act
            inputs.Apply(stillHeld, RadioPanelMode.Com2, RadioPanelMode.Com1);

            // Assert
            CollectionAssert.AreEqual(
                new[] { RadioPanelModeInputs.GetButtonIndex(RadioPanelMode.Com1, RadioPanelControl.ActStby) },
                PressedModeButtons(stillHeld));
        }

        [TestMethod]
        public void Apply_ReleaseThenPressAfterSelectorMoved_UsesNewMode()
        {
            // Arrange
            var inputs = new RadioPanelModeInputs();
            inputs.Apply(Physical(UpperInnerCw), RadioPanelMode.Com1, RadioPanelMode.Com1);
            var released = Physical();
            inputs.Apply(released, RadioPanelMode.Com2, RadioPanelMode.Com1);
            var pressedAgain = Physical(UpperInnerCw);

            // Act
            inputs.Apply(pressedAgain, RadioPanelMode.Com2, RadioPanelMode.Com1);

            // Assert
            Assert.IsEmpty(PressedModeButtons(released), "Release clears the mode button");
            CollectionAssert.AreEqual(
                new[] { RadioPanelModeInputs.GetButtonIndex(RadioPanelMode.Com2, RadioPanelControl.InnerCw) },
                PressedModeButtons(pressedAgain));
        }

        [TestMethod]
        public void Apply_UnknownRowMode_PressesNoModeButton()
        {
            // Arrange
            var inputs = new RadioPanelModeInputs();
            var state = Physical(UpperInnerCw);

            // Act
            inputs.Apply(state, RadioPanelMode.None, RadioPanelMode.Com1);

            // Assert
            Assert.IsEmpty(PressedModeButtons(state));
        }

        [TestMethod]
        public void Apply_EveryRowModeAndControl_MapsToItsOwnButton()
        {
            var physicalButtons = new[,]
            {
                { 16, 17, 18, 19, 14 },
                { 20, 21, 22, 23, 15 }
            };

            for (var row = 0; row < 2; row++)
            {
                foreach (var mode in RadioPanelModes.All)
                {
                    for (var control = 0; control < RadioPanelModeInputs.ControlsPerMode; control++)
                    {
                        // Arrange
                        var inputs = new RadioPanelModeInputs();
                        var state = Physical(physicalButtons[row, control]);
                        var upper = row == 0 ? mode : RadioPanelMode.None;
                        var lower = row == 1 ? mode : RadioPanelMode.None;

                        // Act
                        inputs.Apply(state, upper, lower);

                        // Assert
                        var expected = RadioPanelModeInputs.GetButtonIndex(mode, (RadioPanelControl)control);
                        CollectionAssert.AreEqual(new[] { expected }, PressedModeButtons(state), $"Row {row}, {mode}, control {control}");
                    }
                }
            }
        }

        #endregion

        #region Definition

        [TestMethod]
        public void Definition_InputIdsAreSequentialSoButtonIndexMatchesId()
        {
            // Arrange
            var definition = RadioPanelDefinition.Load();

            // Assert
            Assert.HasCount(RadioPanelReport.ButtonCount + RadioPanelModeInputs.ButtonCount, definition.Inputs);
            for (var index = 0; index < definition.Inputs.Count; index++)
            {
                Assert.AreEqual(index, definition.Inputs[index].Id, $"Input at position {index}");
                Assert.AreEqual(JoystickDeviceType.Button, definition.Inputs[index].Type, $"Input {index} type");
            }
        }

        [TestMethod]
        public void Definition_ModeInputLabelsMatchTheirModeAndControl()
        {
            // Arrange
            var definition = RadioPanelDefinition.Load();
            var controlLabels = new[] { "inner CW", "inner CCW", "outer CW", "outer CCW", "ACT/STBY" };

            // Assert
            foreach (var mode in RadioPanelModes.All)
            {
                for (var control = 0; control < RadioPanelModeInputs.ControlsPerMode; control++)
                {
                    var index = RadioPanelModeInputs.GetButtonIndex(mode, (RadioPanelControl)control);
                    var expected = $"{RadioPanelModes.GetName(mode)} {controlLabels[control]}";
                    Assert.AreEqual(expected, definition.Inputs[index].Label, $"Input {index}");
                }
            }
        }

        #endregion
    }
}
