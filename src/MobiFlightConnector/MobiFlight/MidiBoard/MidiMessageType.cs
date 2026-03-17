using System.Runtime.Serialization;

namespace MobiFlight
{
    public enum MidiMessageType
    {
        [EnumMember(Value = "Note")]
        Note,
        [EnumMember(Value = "CC")]
        CC,
        [EnumMember(Value = "Pitch")]
        Pitch
    }
}
