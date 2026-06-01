using System.Runtime.Serialization;

namespace MobiFlight.BrowserMessages.Incoming
{

    public enum PresetType
    {
        [EnumMember(Value = "prosim")]
        PROSIM,
        [EnumMember(Value = "eventid")]
        EVENTID,
        [EnumMember(Value = "eventid.pmdg")]
        EVENTID_PMDG,
    }
    internal class CommandRefreshPresets
    {
        public PresetType type;
    }
}