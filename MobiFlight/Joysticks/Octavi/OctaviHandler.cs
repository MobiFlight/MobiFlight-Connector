using System;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.Joysticks.Octavi
{
    internal class OctaviHandler
    {
        private bool isInShiftMode = false;
        private OctaviReport lastReport = new OctaviReport();
        private readonly List<string> buttons = new List<string>();
        private readonly Dictionary<(OctaviReport.OctaviState state, bool isShifted, OctaviEncoder encoder), int> encoderMappings;
        private readonly Dictionary<(OctaviReport.OctaviState state, bool isShifted, OctaviReport.OctaviButtons button), int> buttonMappings;

        private enum OctaviEncoder { OUTER_INC, OUTER_DEC, INNER_INC, INNER_DEC }

        public IEnumerable<string> JoystickButtonNames { get; private set; }
        public List<Context> contexts;
        public struct Context
        {
            public OctaviReport.OctaviState state;
            public bool isShifted;
            public string name;

            public Context(OctaviReport.OctaviState State, bool IsShifted, string Name)
            {
                state = State;
                isShifted = IsShifted;
                name = Name;
            }
        }

        private JoystickDefinition definition;

        public OctaviHandler(JoystickDefinition definition = null)
        {
            this.definition = definition;
            encoderMappings = new Dictionary<(OctaviReport.OctaviState state, bool isShifted, OctaviEncoder encoder), int>();
            buttonMappings = new Dictionary<(OctaviReport.OctaviState state, bool isShifted, OctaviReport.OctaviButtons button), int>();

            this.contexts = new List<Context>();
            var octaviStateType = typeof(OctaviReport.OctaviState);
            foreach(var state in Enum.GetValues(octaviStateType).Cast<OctaviReport.OctaviState>())
            {
                var stateName = Enum.GetName(octaviStateType, state).Replace("STATE_", "");
                var context = new Context(state, false, stateName);
                this.contexts.Add(context);
            }

            foreach (var input in definition.Inputs)
            {
                var state = (OctaviReport.OctaviState)input.Id;
                var stateName = Enum.GetName(octaviStateType, state).Replace("STATE_", "");
                var context = new Context(state, true, stateName + "^");
                this.contexts.Add(context);
            }
            foreach (var context in contexts)
            {
                // Encoders
                encoderMappings.Add((context.state, context.isShifted, OctaviEncoder.OUTER_INC), ToButton($"Button_{context.name}_OI"));
                encoderMappings.Add((context.state, context.isShifted, OctaviEncoder.OUTER_DEC), ToButton($"Button_{context.name}_OD"));
                encoderMappings.Add((context.state, context.isShifted, OctaviEncoder.INNER_INC), ToButton($"Button_{context.name}_II"));
                encoderMappings.Add((context.state, context.isShifted, OctaviEncoder.INNER_DEC), ToButton($"Button_{context.name}_ID"));

                // TOG
                buttonMappings.Add((context.state, context.isShifted, OctaviReport.OctaviButtons.HID_BTN_TOG), ToButton($"Button_{context.name}_TOG"));

                // CRSR
                if (!definition.Inputs.Any(JoystickInput => JoystickInput.Id == (int) context.state))
                {
                    buttonMappings.Add((context.state, context.isShifted, OctaviReport.OctaviButtons.HID_ENC_SW), ToButton($"Button_{context.name}_CRSR"));
                }

                // DCT, MENU, CLR, ENT, CDI, OBS, MSG, FLP, VNAV, PROC
                if (context.state == OctaviReport.OctaviState.STATE_FMS1 ||
                    context.state == OctaviReport.OctaviState.STATE_FMS2)
                {
                    buttonMappings.Add((context.state, context.isShifted, OctaviReport.OctaviButtons.HID_BTN_DCT), ToButton($"Button_{context.name}_DCT"));
                    buttonMappings.Add((context.state, context.isShifted, OctaviReport.OctaviButtons.HID_BTN_MENU), ToButton($"Button_{context.name}_MENU"));
                    buttonMappings.Add((context.state, context.isShifted, OctaviReport.OctaviButtons.HID_BTN_CLR), ToButton($"Button_{context.name}_CLR"));
                    buttonMappings.Add((context.state, context.isShifted, OctaviReport.OctaviButtons.HID_BTN_ENT), ToButton($"Button_{context.name}_ENT"));
                    buttonMappings.Add((context.state, context.isShifted, OctaviReport.OctaviButtons.HID_BTN_AP), ToButton($"Button_{context.name}_CDI"));
                    buttonMappings.Add((context.state, context.isShifted, OctaviReport.OctaviButtons.HID_BTN_AP_HDG), ToButton($"Button_{context.name}_OBS"));
                    buttonMappings.Add((context.state, context.isShifted, OctaviReport.OctaviButtons.HID_BTN_AP_NAV), ToButton($"Button_{context.name}_MSG"));
                    buttonMappings.Add((context.state, context.isShifted, OctaviReport.OctaviButtons.HID_BTN_AP_APR), ToButton($"Button_{context.name}_FPL"));
                    buttonMappings.Add((context.state, context.isShifted, OctaviReport.OctaviButtons.HID_BTN_AP_ALT), ToButton($"Button_{context.name}_VNAV"));
                    buttonMappings.Add((context.state, context.isShifted, OctaviReport.OctaviButtons.HID_BTN_AP_VS), ToButton($"Button_{context.name}_PROC"));
                }

                // AP, HDG, NAV, APR, ALT, VS (AP context only for now to not mess up the ordering of buttons)
                if (context.state == OctaviReport.OctaviState.STATE_AP)
                {
                    AddAutopilotButtonMappings(context);
                }
            }

            // AP, HDG, NAV, APR, ALT, VS (remaining Autopilot-enabled contexts)
            foreach (var context in contexts)
            {
                if (context.state != OctaviReport.OctaviState.STATE_FMS1 &&
                    context.state != OctaviReport.OctaviState.STATE_FMS2 &&
                    context.state != OctaviReport.OctaviState.STATE_AP)
                {
                    AddAutopilotButtonMappings(context);
                }
            }

            JoystickButtonNames = buttons.AsReadOnly();
        }

        private void AddAutopilotButtonMappings(Context context)
        {
            buttonMappings.Add((context.state, context.isShifted, OctaviReport.OctaviButtons.HID_BTN_AP), ToButton("Button_AP_AP"));
            buttonMappings.Add((context.state, context.isShifted, OctaviReport.OctaviButtons.HID_BTN_AP_HDG), ToButton("Button_AP_HDG"));
            buttonMappings.Add((context.state, context.isShifted, OctaviReport.OctaviButtons.HID_BTN_AP_NAV), ToButton("Button_AP_NAV"));
            buttonMappings.Add((context.state, context.isShifted, OctaviReport.OctaviButtons.HID_BTN_AP_APR), ToButton("Button_AP_APR"));
            buttonMappings.Add((context.state, context.isShifted, OctaviReport.OctaviButtons.HID_BTN_AP_ALT), ToButton("Button_AP_ALT"));
            buttonMappings.Add((context.state, context.isShifted, OctaviReport.OctaviButtons.HID_BTN_AP_VS), ToButton("Button_AP_VS"));
        }

        private int ToButton(string buttonName)
        {
            if (!buttons.Contains(buttonName))
            {
                buttons.Add(buttonName);
            }
            return buttons.IndexOf(buttonName);
        }

        public IEnumerable<(int buttonIndex, MobiFlightButton.InputEvent inputEvent)> DetectButtonEvents(OctaviReport report)
        {
            var buttonEvents = new List<(int buttonIndex, MobiFlightButton.InputEvent inputEvent)>();

            OctaviReport.OctaviButtons pressed = (OctaviReport.OctaviButtons)((uint)report.buttonState & ~(uint)lastReport.buttonState); // rising edges
            OctaviReport.OctaviButtons released = (OctaviReport.OctaviButtons)((uint)lastReport.buttonState & ~(uint)report.buttonState); // falling edges

            // "Shift Mode" for supported contexts
            if (report.contextState != lastReport.contextState)
            {
                isInShiftMode = false; // reset shift mode on context change
            }
            else if (pressed.HasFlag(OctaviReport.OctaviButtons.HID_ENC_SW) && this.definition.Inputs.Any(JoystickInput => JoystickInput.Id == (int)report.contextState))
            {
                isInShiftMode = !isInShiftMode;
            }

            // Encoders (Note: No RELEASE events required for encoders)
            for (int i = 0; i < report.outerEncoderDelta; i++)
            {
                buttonEvents.Add((encoderMappings[(report.contextState, isInShiftMode, OctaviEncoder.OUTER_INC)], MobiFlightButton.InputEvent.PRESS));
            }
            for (int i = 0; i > report.outerEncoderDelta; i--)
            {
                buttonEvents.Add((encoderMappings[(report.contextState, isInShiftMode, OctaviEncoder.OUTER_DEC)], MobiFlightButton.InputEvent.PRESS));
            }
            for (int i = 0; i < report.innerEncoderDelta; i++)
            {
                buttonEvents.Add((encoderMappings[(report.contextState, isInShiftMode, OctaviEncoder.INNER_INC)], MobiFlightButton.InputEvent.PRESS));
            }
            for (int i = 0; i > report.innerEncoderDelta; i--)
            {
                buttonEvents.Add((encoderMappings[(report.contextState, isInShiftMode, OctaviEncoder.INNER_DEC)], MobiFlightButton.InputEvent.PRESS));
            }

            // Buttons
            foreach (OctaviReport.OctaviButtons button in Enum.GetValues(typeof(OctaviReport.OctaviButtons)))
            {
                if (pressed.HasFlag(button) || released.HasFlag(button))
                {
                    if (buttonMappings.TryGetValue((report.contextState, isInShiftMode, button), out int buttonIndex))
                    {
                        var inputEvent = pressed.HasFlag(button) ? MobiFlightButton.InputEvent.PRESS : MobiFlightButton.InputEvent.RELEASE;
                        buttonEvents.Add((buttonIndex, inputEvent));
                    }
                }
            }

            lastReport = report;
            return buttonEvents;
        }
    }
}
