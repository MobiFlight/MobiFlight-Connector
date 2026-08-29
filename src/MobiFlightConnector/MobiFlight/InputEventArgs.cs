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

        /// <summary>Null (default) broadcasts to every matching config. Non-null restricts Execute() to that one config's GUID - used for HOLD/REPEAT/LONG_RELEASE, timed per-config.</summary>
        public string TargetConfigGUID { get; set; }

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
            clone.TargetConfigGUID = TargetConfigGUID;

            return clone;
        }

        /// <summary>
        /// Label is a UI/i18n concern and shouldn't really live on a backend type - that's why plain
        /// Clone() (and DeviceReference.Clone()) drop it. But backend log formatting still needs a
        /// display string today, so use this clone where that's the case, e.g. deriving
        /// RELEASE/HOLD/REPEAT/LONG_RELEASE from a PRESS (same physical button, same label). Once
        /// logging no longer needs Label, this method should go away rather than get replaced by
        /// plain Clone().
        /// </summary>
        public InputEventArgs CloneWithLabel()
        {
            var clone = (InputEventArgs)Clone();
            if (clone.Device != null) clone.Device.Label = Device.Label;
            return clone;
        }
    }
}