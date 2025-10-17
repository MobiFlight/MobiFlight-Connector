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

        public string GetEventActionLabel()
        {
            switch (Type)
            {
                case DeviceType.Button:
                    return MobiFlightButton.InputEventIdToString(Value);
                case DeviceType.Encoder:
                    return MobiFlightEncoder.InputEventIdToString(Value);
                case DeviceType.InputShiftRegister:
                    return MobiFlightInputShiftRegister.InputEventIdToString(Value);
                case DeviceType.InputMultiplexer:
                    return MobiFlightInputMultiplexer.InputEventIdToString(Value);
                case DeviceType.AnalogInput:
                    return $"{MobiFlightAnalogInput.InputEventIdToString(0)} => {Value}";
                default:
                    return "n/a";
            }
        }

        public string GetMsgEventLabel()
        {
            var eventAction = GetEventActionLabel();
            return $"{Name} => {DeviceLabel}{(ExtPin.HasValue ? $":{ExtPin}" : "")} => {eventAction}";
        }

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
