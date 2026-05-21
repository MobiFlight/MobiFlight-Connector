using MobiFlight.Base;
using MobiFlight.Config;
using System;

namespace MobiFlight
{
    namespace Base
    {
        public class DeviceReference
        {
            public string Name { get; set; }

            // temporay property, will be removed before merging with the main branch
            public string Label { get; set; }
            public string SubId { get; set; }
            public DeviceType Type { get; set; }

            public DeviceReference() { }
            public DeviceReference(DeviceType type, string name, string subId = null)
            {
                Type = type;
                Name = name;
                SubId = subId;
            }

            public object Clone()
            {
                return new DeviceReference(Type, Name, SubId);
            }

            public override bool Equals(object obj)
            {
                if (!(obj is DeviceReference reference))
                {
                    return false;
                }

                return
                       Type == reference.Type &&
                       Name == reference.Name &&
                       SubId == reference.SubId;
            }

            public override int GetHashCode()
            {
                unchecked // Overflow is fine, just wrap
                {
                    int hash = 17;
                    hash = hash * 23 + Type.GetHashCode();
                    hash = hash * 23 + (Name?.GetHashCode() ?? 0);
                    hash = hash * 23 + (SubId?.GetHashCode() ?? 0);
                    return hash;
                }
            }
        }
    }

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

            var subPinLabel = Device.SubId != null ? $":{Device.SubId}" : null;
            return $"{Controller.Name} => {Device.Name}{subPinLabel} => {eventAction}";
        }

        public object Clone()
        {
            InputEventArgs clone = new InputEventArgs();
            clone.Controller = Controller.Clone() as Controller;
            clone.Device = Device.Clone() as DeviceReference;
            clone.InputType = InputType;
            clone.Value = Value;
            clone.StrValue = StrValue;

            return clone;
        }
    }
}