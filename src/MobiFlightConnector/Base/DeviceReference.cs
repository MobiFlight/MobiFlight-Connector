using MobiFlight.Firmware;
using System.Collections.Generic;

namespace MobiFlight.Base
{
    public record CustomDeviceMessageType(int Id, string Label, string Description);

    /// <summary>Runtime facts the UI needs about a device; null members mean not applicable.</summary>
    public class DeviceCapability
    {
        public bool? Pwm { get; set; }
        public int? Rows { get; set; }
        public int? Columns { get; set; }
        public int? Digits { get; set; }
        public StepperProfilePreset Profile { get; set; }
        public bool? AutoZero { get; set; }
        public List<CustomDeviceMessageType> MessageTypes { get; set; }
    }

    /// <summary>A device of a connected controller, including what it is capable of.</summary>
    public class ControllerDevice : DeviceReference
    {
        public DeviceCapability Capabilities { get; set; }
    }

    public class DeviceReference
    {
        public virtual string Name { get; set; }

        // temporary property, will be removed before merging with the main branch
        public string Label { get; set; }
        public DeviceType Type { get; set; }

        public DeviceReference() { }
        public DeviceReference(DeviceType type, string name)
        {
            Type = type;
            Name = name;
        }

        public virtual object Clone()
        {
            return new DeviceReference(Type, Name);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is DeviceReference reference))
            {
                return false;
            }

            return
                   Type == reference.Type &&
                   Name == reference.Name;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + Type.GetHashCode();
                hash = hash * 23 + (Name?.GetHashCode() ?? 0);
                return hash;
            }
        }
    }
}