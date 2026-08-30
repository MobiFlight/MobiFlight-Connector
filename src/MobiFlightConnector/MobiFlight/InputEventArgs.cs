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

        /// <summary>
        /// The HOLD/RepeatDelay/LongReleaseDelay (ms) that classified this event - also how a config
        /// decides an event is its own (see ButtonInputConfig.MatchesSyntheticDelay), not just how
        /// it's displayed. Null for PRESS/RELEASE.
        /// </summary>
        public int? SyntheticDelayMs { get; set; }

        public readonly DateTime Time = DateTime.Now;

        public string GetEventActionLabel() => GetEventActionLabel(Value);

        /// <summary>Same as GetEventActionLabel(), for a value other than this instance's own.</summary>
        public string GetEventActionLabel(double value)
        {
            var v = Convert.ToInt32(value);
            switch (InputType)
            {
                case DeviceType.Button:
                    var label = MobiFlightButton.InputEventIdToString(v);
                    // Only annotate when asking for this event's own value - a value normalized down
                    // to a fallback (e.g. LONG_RELEASE -> RELEASE) has no delay of its own to show.
                    return (SyntheticDelayMs.HasValue && v == Convert.ToInt32(Value)) ? $"{label} ({SyntheticDelayMs}ms)" : label;
                case DeviceType.Encoder:
                    return MobiFlightEncoder.InputEventIdToString(v);
                case DeviceType.AnalogInput:
                    return $"{MobiFlightAnalogInput.InputEventIdToString(0)} => {v}";
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
            clone.SyntheticDelayMs = SyntheticDelayMs;

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