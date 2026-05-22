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
        public int Value { get; set; }

        public String StrValue { get; set; }

        public readonly DateTime Time = DateTime.Now;

        public string GetEventActionLabel()
        {
            switch (InputType)
            {
                case DeviceType.Button:
                    return MobiFlightButton.InputEventIdToString(Value);
                case DeviceType.Encoder:
                    return MobiFlightEncoder.InputEventIdToString(Value);
                case DeviceType.AnalogInput:
                    return $"{MobiFlightAnalogInput.InputEventIdToString(0)} => {Value}";
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