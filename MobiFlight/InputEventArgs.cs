using System;

namespace MobiFlight
{
    public class InputEventArgs : EventArgs, ICloneable
    {
        public string Serial { get; set; }
        public string DeviceId { get; set; }
        public string DeviceLabel { get; set; }
        public string Name { get; set; }
        public DeviceType Type { get; set; }
        public int? ExtPin { get; set; }
        public int Value { get; set; }

        public String StrValue { get; set; }

        public readonly DateTime Time = DateTime.Now;

        public object Clone()
        {
            InputEventArgs clone = new InputEventArgs();
            clone.Serial = Serial;
            clone.DeviceId = DeviceId;
            clone.DeviceLabel = DeviceLabel;
            clone.Name = Name;
            clone.Type = Type;
            clone.ExtPin = ExtPin;
            clone.Value = Value;
            clone.StrValue = StrValue;
            return clone;
        }
    }
}
