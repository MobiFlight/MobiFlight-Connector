using HidSharp;
using HidSharp.Reports;
using HidSharp.Reports.Input;

namespace MobiFlight.Joysticks.Logitech
{
    internal class Multipanel : MobiFlight.Joysticks.HidDevice
    {
        public Multipanel(JoystickDefinition definition) : base(definition){
        }
    }
}
