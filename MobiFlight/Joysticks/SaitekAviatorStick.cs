using System.Collections.Generic;

namespace MobiFlight.Joysticks
{
    internal class SaitekAviatorStick : LabeledJoystick
    {
        int VendorId = 0x06A3;
        int ProductId = 0x0461;

        HidDefinition Definition;

        public SaitekAviatorStick(SharpDX.DirectInput.Joystick joystick, HidDefinition definition) : base(joystick) {
            Definition = definition;
            Labels = new Dictionary<string, string>();

            definition.Inputs.ForEach(input => Labels.Add(input.Name, input.Label));
        }
    }
}
