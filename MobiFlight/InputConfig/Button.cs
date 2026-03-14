using MobiFlight.Base;

namespace MobiFlight.InputConfig
{
    public class Button : DeviceConfig
    {
        public override object Clone()
        {
            return new Button { Name = Name };
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is Button other && Name == other.Name;
        }
    }
}