using MobiFlight.Base;

namespace MobiFlight.InputConfig
{
    public class AnalogInput : DeviceConfig
    {
        public override object Clone()
        {
            return new AnalogInput { Name = Name };
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is AnalogInput other && Name == other.Name;
        }
    }
}