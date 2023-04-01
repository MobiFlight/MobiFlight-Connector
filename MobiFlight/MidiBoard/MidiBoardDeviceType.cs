using System.Runtime.Serialization;

namespace MobiFlight
{
    public enum MidiBoardDeviceType
    {
        [EnumMember(Value = "Button")]
        Button,
        [EnumMember(Value = "EndlessKnob")]
        EndlessKnob,
        [EnumMember(Value = "LimitedKnob")]
        LimitedKnob,
        [EnumMember(Value = "LED")]
        LED,
        [EnumMember(Value = "Fader")]
        Fader,
        [EnumMember(Value = "Pitch")]
        Pitch
    }
}
