using MobiFlight.Base;

namespace MobiFlight.InputConfig
{
    public class InputMultiplexer : DeviceConfig
    {
        public int DataPin { get; set; }

        public override object Clone()
        {
            return new InputMultiplexer { Name = Name, DataPin = DataPin };
        }

        public override bool Equals(object obj)
        {
            return obj != null
                && obj is InputMultiplexer other
                && Name == other.Name
                && DataPin == other.DataPin;
        }
    }
}