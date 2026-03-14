using MobiFlight.Base;

namespace MobiFlight.InputConfig
{
    public class InputShiftRegister : DeviceConfig
    {
        public int SubIndex { get; set; }

        public override object Clone()
        {
            return new InputShiftRegister { Name = Name, SubIndex = SubIndex };
        }

        public override bool Equals(object obj)
        {
            return obj != null
                && obj is InputShiftRegister other
                && Name == other.Name
                && SubIndex == other.SubIndex;
        }
    }
}