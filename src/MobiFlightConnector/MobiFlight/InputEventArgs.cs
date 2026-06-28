using MobiFlight.Base;
using MobiFlight.Firmware;
using System;

namespace MobiFlight
{
    public class InputEventArgs : EventArgs, ICloneable
    {
        public Controller Controller { get; set; }
        public DeviceReference Device { get; set; }
        public DeviceType InputType { get; set; }
        public double Value { get; set; }

        public String StrValue { get; set; }

        public readonly DateTime Time = DateTime.Now;

        public string GetEventActionLabel()
        {
            var value = Convert.ToInt32(Value);
            switch (InputType)
            {
                case DeviceType.Button:
                    return MobiFlightButton.InputEventIdToString(value);
                case DeviceType.Encoder:
                    return MobiFlightEncoder.InputEventIdToString(value);
                case DeviceType.AnalogInput:
                    return $"{MobiFlightAnalogInput.InputEventIdToString(0)} => {value}";
                default:
                    return "n/a";
            }
        }

        public string GetMsgEventLabel()
        {
            var eventAction = GetEventActionLabel();

            return $"{Controller.Name} => {Device.Label} => {eventAction}";
        }

        public object Clone()
        {
            InputEventArgs clone = new InputEventArgs();
            clone.Controller = Controller?.Clone() as Controller;
            clone.Device = Device?.Clone() as DeviceReference;
            clone.InputType = InputType;
            clone.Value = Value;
            clone.StrValue = StrValue;

            return clone;
        }
    }
}