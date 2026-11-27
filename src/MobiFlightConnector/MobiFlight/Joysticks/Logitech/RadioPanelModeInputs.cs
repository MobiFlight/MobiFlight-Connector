namespace MobiFlight.Joysticks.Logitech
{
    /// <summary>The per-row controls that also exist as mode-specific inputs.</summary>
    internal enum RadioPanelControl
    {
        InnerCw = 0,
        InnerCcw = 1,
        OuterCw = 2,
        OuterCcw = 3,
        ActStby = 4
    }

    /// <summary>
    /// Derives mode-specific buttons from the physical Radio Panel buttons, e.g. "COM1 inner CW"
    /// is pressed while the inner encoder of a row turns clockwise and that row's selector is on COM1.
    /// The mode buttons do not depend on the row, like the displays. The physical buttons 0-23 are
    /// left untouched, so configs using them keep working.
    /// </summary>
    internal sealed class RadioPanelModeInputs
    {
        public const int FirstButton = RadioPanelReport.ButtonCount;
        public const int ControlsPerMode = 5;
        public const int ButtonCount = RadioPanelModes.Count * ControlsPerMode;

        private const int RowCount = 2;

        // Physical button per row and control, see RadioPanelReport.
        private static readonly int[,] PhysicalButtons =
        {
            { 16, 17, 18, 19, 14 }, // upper: inner CW, inner CCW, outer CW, outer CCW, ACT/STBY
            { 20, 21, 22, 23, 15 }  // lower
        };

        // Mode each physical control was pressed in, so its release goes to the same mode button
        // even if the selector moved while the control was held.
        private readonly RadioPanelMode[,] PressedIn = new RadioPanelMode[RowCount, ControlsPerMode];

        public RadioPanelModeInputs()
        {
            for (var row = 0; row < RowCount; row++)
            {
                for (var control = 0; control < ControlsPerMode; control++)
                {
                    PressedIn[row, control] = RadioPanelMode.None;
                }
            }
        }

        public static int GetButtonIndex(RadioPanelMode mode, RadioPanelControl control) =>
            FirstButton + (int)mode * ControlsPerMode + (int)control;

        /// <summary>
        /// Sets the mode buttons in <paramref name="state"/> from its physical buttons and the
        /// current selector mode of each row.
        /// </summary>
        public void Apply(JoystickState state, RadioPanelMode upperMode, RadioPanelMode lowerMode)
        {
            for (var row = 0; row < RowCount; row++)
            {
                var rowMode = row == 0 ? upperMode : lowerMode;

                for (var control = 0; control < ControlsPerMode; control++)
                {
                    if (!state.Buttons[PhysicalButtons[row, control]])
                    {
                        PressedIn[row, control] = RadioPanelMode.None;
                        continue;
                    }

                    if (PressedIn[row, control] == RadioPanelMode.None)
                    {
                        PressedIn[row, control] = rowMode;
                    }

                    var mode = PressedIn[row, control];
                    if (mode == RadioPanelMode.None) continue;

                    state.Buttons[GetButtonIndex(mode, (RadioPanelControl)control)] = true;
                }
            }
        }
    }
}
