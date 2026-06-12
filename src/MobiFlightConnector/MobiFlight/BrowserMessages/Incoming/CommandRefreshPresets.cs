using System.Runtime.Serialization;

namespace MobiFlight.BrowserMessages.Incoming
{

    public enum PresetType
    {
        [EnumMember(Value = "prosim")]
        PROSIM,
        [EnumMember(Value = "vjoy")]
        VJOY
    }
    public class CommandRefreshPresets
    {
        public PresetType type;
    }
}