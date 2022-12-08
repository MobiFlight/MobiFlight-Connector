using System.Collections.Generic;

namespace MobiFlight.Joysticks
{
    internal class SaitekAviatorStick : LabeledJoystick
    {
        int VendorId = 0x06A3;
        int ProductId = 0x0461;

        public SaitekAviatorStick(SharpDX.DirectInput.Joystick joystick) : base(joystick) {
            Labels = new Dictionary<string, string>()
            {
                // Button 1-4 are not labeled
                // ---
                // Front switches
                { "Button 5", "Button - T1" },
                { "Button 6", "Button - T2" },
                { "Button 7", "Button - T3" },
                { "Button 8", "Button - T4" },
                { "Button 9", "Button - T5" },
                { "Button 10", "Button - T6" },
                { "Button 11", "Button - T7" },
                { "Button 12", "Button - T8" },
                { "Button 13", "Mode A" },
                { "Button 14", "Mode B" }
                // Throttles & Axis are not labeled
            };
        }
    }
}
