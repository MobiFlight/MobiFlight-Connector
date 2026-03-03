using MobiFlight.Base;

namespace MobiFlight.InputConfig
{
    public class InputShiftRegister : DeviceConfig
    {
        public int ExtPin { get; set; }

        public override object Clone()
        {
            return new InputShiftRegister { Name = Name, ExtPin = ExtPin };
        }

        public override bool Equals(object obj)
        {
            return obj != null
                && obj is InputShiftRegister other
                && Name == other.Name
                && ExtPin == other.ExtPin;
        }
    }
}