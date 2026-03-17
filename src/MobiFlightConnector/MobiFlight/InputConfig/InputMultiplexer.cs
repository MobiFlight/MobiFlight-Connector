using MobiFlight.Base;

namespace MobiFlight.InputConfig
{
    public class InputMultiplexer : DeviceConfig
    {
        public int SubIndex { get; set; }

        public override object Clone()
        {
            return new InputMultiplexer { Name = Name, SubIndex = SubIndex };
        }

        public override bool Equals(object obj)
        {
            return obj != null
                && obj is InputMultiplexer other
                && Name == other.Name
                && SubIndex == other.SubIndex;
        }
    }
}