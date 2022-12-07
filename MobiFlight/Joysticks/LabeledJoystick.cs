using System.Collections.Generic;
using System;
using System.Linq;

namespace MobiFlight.Joysticks
{
    public class LabeledJoystick : Joystick
    {
        protected Dictionary<string, string> Labels = new Dictionary<string, string>();

        public LabeledJoystick(SharpDX.DirectInput.Joystick joystick) : base(joystick) { }

        protected override List<JoystickDevice> GetButtonsSorted()
        {
            var buttons = Buttons.ToArray().ToList();
            buttons.Sort(SortByPositionInDictionary);

            return buttons;
        }

        protected override string MapDeviceNameToLabel(string name)
        {
            var result = name;

            if (Labels.ContainsKey(name))
                result = Labels[name];

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
