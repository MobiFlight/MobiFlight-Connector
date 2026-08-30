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
        /// The HoldDelay/RepeatDelay (ms) that classified this event - also how a config decides a
        /// HOLD/REPEAT is its own (see ButtonInputConfig.MatchesSyntheticDelay), not just how it's
        /// displayed. Null except on HOLD/REPEAT.
        /// </summary>
        public int? SyntheticDelayMs { get; set; }

        /// <summary>
        /// The HoldDelay of the binding that produced this REPEAT - disambiguates two configs that
        /// share the same RepeatDelay but not the same HoldDelay (see
        /// ButtonInputConfig.MatchesSyntheticDelay). Null except on REPEAT.
        /// </summary>
        public int? SyntheticHoldDelayMs { get; set; }

        /// <summary>
        /// How long the button was held (ms) before this RELEASE. RELEASE is always raised once, as
        /// RELEASE - LONG_RELEASE is a per-config decision made at dispatch time (see
        /// ButtonInputConfig.ResolveDispatchedEvent), not a reclassification of the raw event. Null
        /// except on RELEASE.
        /// </summary>
        public int? HeldDurationMs { get; set; }

        public readonly DateTime Time = DateTime.Now;

        public string GetEventActionLabel() => GetEventActionLabel(Value);

        /// <summary>Same as GetEventActionLabel(), for a value other than this instance's own.</summary>
        public string GetEventActionLabel(double value)
        {
            var v = Convert.ToInt32(value);
            switch (InputType)
            {
                case DeviceType.Button:
                    return MobiFlightButton.InputEventIdToString(v);
                case DeviceType.Encoder:
                    return MobiFlightEncoder.InputEventIdToString(v);
                case DeviceType.AnalogInput:
                    return $"{MobiFlightAnalogInput.InputEventIdToString(0)} => {v}";
                default:
                    return "n/a";
            }
        }

        /// <summary>
        /// The one place SyntheticDelayMs is shown - "an event was raised" (RawValue and the
        /// per-config "Executing" line show what actually dispatched instead, deliberately without it).
        /// </summary>
        public string GetMsgEventLabel()
        {
            var eventAction = GetEventActionLabel();
            if (SyntheticDelayMs.HasValue) eventAction += $" ({SyntheticDelayMs}ms)";

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
            clone.SyntheticHoldDelayMs = SyntheticHoldDelayMs;
            clone.HeldDurationMs = HeldDurationMs;

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