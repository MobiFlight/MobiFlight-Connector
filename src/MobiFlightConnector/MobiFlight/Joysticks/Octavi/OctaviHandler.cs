using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MobiFlight.Joysticks.Octavi
{
    internal class OctaviHandler
    {
        private const string Shift_VButton_Name = "Button_SHIFT_SW";
        private const string Unshift_VButton_Name = "Button_UNSHIFT_SW";

        public bool IsInShiftMode { get; private set; } = false;
        // null until the first report, which counts as a context change
        private OctaviReport lastReport = null;
        // 1 when the next report has to press the current context's selection button again; set from other threads
        private int isContextReportRequested = 0;
        private readonly List<string> buttons = new List<string>();
        private readonly Dictionary<OctaviReport.OctaviState, int> contextMappings;
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
            contextMappings = new Dictionary<OctaviReport.OctaviState, int>();

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

            /* create at least one instance for the Buttons for the virtual (un)shift events on context change.
             * always created first to keep them together in the Button list.
             * (shift button will only be called from the buttonmapping later,
             * unshift either virtually without context or from the mapping)
             */
            _ = ToButton(Shift_VButton_Name);
            _ = ToButton(Unshift_VButton_Name);


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
                    
                    /* do not create the unshift button in the mapping, will be a duplicate key in the dictionary.
                     * will be called with the buttons.IndexOf method later in this "virtual" case.
                     */
                    //buttonMappings.Add((context.state, context.isShifted, OctaviReport.OctaviButtons.HID_ENC_SW), ToButton(Unshift_VButton_Name));
                }
                else
                {
                    /* Will be used to raise the "we are shifted now" flag
                    * Can be used to control a "shifted" variable that turns output pin 7 on/off
                    * We need two buttons, because context switching forces "unshifting",
                    * so the shift state always has to be sent explicitly
                    * (i.e. not a variable that will be toggled when a single button is pressed can be used)
                    * States that do not support a shift mode will always be "unshifted".
                    */
                    buttonMappings.Add((context.state, context.isShifted, OctaviReport.OctaviButtons.HID_ENC_SW), ToButton(context.isShifted ? Shift_VButton_Name : Unshift_VButton_Name));
                }

                // DCT, MENU, CLR, ENT, CDI, OBS, MSG, FLP, VNAV, PROC
                if (IsFmsContext(context))
                {
                    AddRightColumnButtonMappings(context);
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
                if (!IsFmsContext(context) && context.state != OctaviReport.OctaviState.STATE_AP)
                {
                    AddAutopilotButtonMappings(context);
                }
            }

            // DCT, MENU, CLR, ENT (remaining contexts, added last to keep the order of all earlier buttons)
            foreach (var context in contexts.Where(c => !IsFmsContext(c)))
            {
                AddRightColumnButtonMappings(context);
            }

            // Context selection buttons, added last to keep the order of earlier buttons. AP..VS share one button
            // outside FMS1/FMS2, so a profile can keep the context in a variable to use them per context.
            foreach (var context in contexts.Where(c => !c.isShifted))
            {
                contextMappings.Add(context.state, ToButton($"Button_CONTEXT_{context.name}"));
            }

            JoystickButtonNames = buttons.AsReadOnly();
        }

        private static bool IsFmsContext(Context context)
        {
            return context.state == OctaviReport.OctaviState.STATE_FMS1 ||
                   context.state == OctaviReport.OctaviState.STATE_FMS2;
        }

        private void AddRightColumnButtonMappings(Context context)
        {
            buttonMappings.Add((context.state, context.isShifted, OctaviReport.OctaviButtons.HID_BTN_DCT), ToButton($"Button_{context.name}_DCT"));
            buttonMappings.Add((context.state, context.isShifted, OctaviReport.OctaviButtons.HID_BTN_MENU), ToButton($"Button_{context.name}_MENU"));
            buttonMappings.Add((context.state, context.isShifted, OctaviReport.OctaviButtons.HID_BTN_CLR), ToButton($"Button_{context.name}_CLR"));
            buttonMappings.Add((context.state, context.isShifted, OctaviReport.OctaviButtons.HID_BTN_ENT), ToButton($"Button_{context.name}_ENT"));
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

        /// <summary>
        /// Presses the current context's selection button again with the next report, for a retrigger or after a stop:
        /// MobiFlight clears the profile's variables on Stop and drops input events while it is stopped.
        /// Only the selection button is sent, not Button_SHIFT_SW or Button_UNSHIFT_SW, so bindings of those don't fire.
        /// Safe to call from any thread; the events are sent on the reader thread, after the current report's events.
        /// </summary>
        public void RequestContextReport()
        {
            Interlocked.Exchange(ref isContextReportRequested, 1);
        }

        private static (int buttonIndex, MobiFlightButton.InputEvent inputEvent)[] Tapped(int buttonIndex)
        {
            return [(buttonIndex, MobiFlightButton.InputEvent.PRESS), (buttonIndex, MobiFlightButton.InputEvent.RELEASE)];
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

            var lastButtonState = lastReport?.buttonState ?? 0;
            OctaviReport.OctaviButtons pressed = (OctaviReport.OctaviButtons)((uint)report.buttonState & ~(uint)lastButtonState); // rising edges
            OctaviReport.OctaviButtons released = (OctaviReport.OctaviButtons)((uint)lastButtonState & ~(uint)report.buttonState); // falling edges

            // "Shift Mode" for supported contexts
            var isContextChange = lastReport == null || report.contextState != lastReport.contextState;
            if (isContextChange)
            {
                // reset shift mode on context changes, including the first report (and synthesize unshift press/release)
                int unshift_vButton = buttons.IndexOf(Unshift_VButton_Name);

                if (unshift_vButton >= 0)
                {
                    buttonEvents.Add((unshift_vButton, MobiFlightButton.InputEvent.PRESS));
                    buttonEvents.Add((unshift_vButton, MobiFlightButton.InputEvent.RELEASE));
                }

                this.IsInShiftMode = false;
            }

            // toggle shift mode on a knob push, after a context change in the same report, so it shifts the new context
            if (pressed.HasFlag(OctaviReport.OctaviButtons.HID_ENC_SW) && this.definition.Inputs.Any(JoystickInput => JoystickInput.Id == (int)report.contextState))
            {
                /* Button events will be generated by finding the corresponding ButtonMapping in the loop at the end. */
                this.IsInShiftMode = !this.IsInShiftMode;
            }

            // Context selection button: on every context change and when requested; a request waits for a known context
            if (contextMappings.TryGetValue(report.contextState, out int contextButton))
            {
                var isContextReportDue = Interlocked.Exchange(ref isContextReportRequested, 0) == 1;
                if (isContextChange || isContextReportDue)
                {
                    buttonEvents.AddRange(Tapped(contextButton));
                }
            }

            // Encoders (Note: No RELEASE events required for encoders)
            AddEncoderEvents(buttonEvents, report.contextState, report.outerEncoderDelta, OctaviEncoder.OUTER_INC, OctaviEncoder.OUTER_DEC);
            AddEncoderEvents(buttonEvents, report.contextState, report.innerEncoderDelta, OctaviEncoder.INNER_INC, OctaviEncoder.INNER_DEC);

            // Buttons
            foreach (OctaviReport.OctaviButtons button in Enum.GetValues(typeof(OctaviReport.OctaviButtons)))
            {
                if (pressed.HasFlag(button) || released.HasFlag(button))
                {
                    if (buttonMappings.TryGetValue((report.contextState, this.IsInShiftMode, button), out int buttonIndex))
                    {
                        var inputEvent = pressed.HasFlag(button) ? MobiFlightButton.InputEvent.PRESS : MobiFlightButton.InputEvent.RELEASE;
                        buttonEvents.Add((buttonIndex, inputEvent));
                    }
                }
            }

            lastReport = report;
            return buttonEvents;
        }

        private void AddEncoderEvents(List<(int buttonIndex, MobiFlightButton.InputEvent inputEvent)> buttonEvents,
            OctaviReport.OctaviState state, sbyte delta, OctaviEncoder increment, OctaviEncoder decrement)
        {
            // an unknown context has no encoder buttons
            if (delta == 0 || !encoderMappings.TryGetValue((state, this.IsInShiftMode, delta > 0 ? increment : decrement), out int buttonIndex))
            {
                return;
            }

            for (int i = 0; i < Math.Abs((int)delta); i++)
            {
                buttonEvents.Add((buttonIndex, MobiFlightButton.InputEvent.PRESS));
            }
        }
    }
}
