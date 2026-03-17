using MobiFlight.Base;

namespace MobiFlight.InputConfig
{
    public class Encoder : DeviceConfig
    {
        public override object Clone()
        {
            return new Encoder { Name = Name };
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is Encoder other && Name == other.Name;
        }
    }
}