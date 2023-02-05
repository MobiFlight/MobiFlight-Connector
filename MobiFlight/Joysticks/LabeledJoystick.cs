using System;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.Joysticks
{
    public class LabeledJoystick : Joystick
    {
        protected Dictionary<string, string> Labels = new Dictionary<string, string>();
        readonly private HidDefinition Definition;

        public LabeledJoystick(SharpDX.DirectInput.Joystick joystick, HidDefinition definition) : base(joystick) 
        {
            Definition = definition;
            Definition?.Inputs.ForEach(input => Labels.Add(input.Name, input.Label));
        }

        protected override List<JoystickDevice> GetButtonsSorted()
        {
            var buttons = Buttons.ToArray().ToList();
            buttons.Sort(SortByPositionInDictionary);

            return buttons;
        }

        protected override void EnumerateOutputDevices()
        {
            base.EnumerateOutputDevices();
            Definition?.Outputs.ForEach(output => Lights.Add(new JoystickOutputDevice() { Label = output.Label, Name = output.Name, Byte = output.Byte, Bit = output.Bit }));
        }

        public override string MapDeviceNameToLabel(string name)
        {
            var result = name;

            if (Labels.ContainsKey(name))
                result = Labels[name];
            else
                result = base.MapDeviceNameToLabel(name);

            return result;
        }

        protected override List<JoystickDevice> GetAxisSorted()
        {
            var axes = Axes.ToArray().ToList();
            Axes.Sort(SortByPositionInDictionary);

            return axes;
        }

        public int GetIndexForKey(string key)
        {
            int result = Array.IndexOf(Labels.Keys.ToArray(), key);
            return result;
        }

        int SortByPositionInDictionary(JoystickDevice b1, JoystickDevice b2)
        {
            if (GetIndexForKey(b1.Name) == GetIndexForKey(b2.Name)) return 0;
            if (GetIndexForKey(b1.Name) > GetIndexForKey(b2.Name)) return 1;
            return -1;
        }
    }
}
