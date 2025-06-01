using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace MobiFlight.BrowserMessages.Incoming
{
    public enum CommandMainMenuAction
    {
        [EnumMember(Value = "file.new")]
        file_new,
        [EnumMember(Value = "file.open")]
        file_open,
        [EnumMember(Value = "file.save")]
        file_save,
        [EnumMember(Value = "file.saveas")]
        file_saveas,
        [EnumMember(Value = "file.exit")]
        file_exit,
        [EnumMember(Value = "file.recent")]
        file_recent,
        [EnumMember(Value = "extras.hubhop.download")]
        extras_hubhop_download,
        [EnumMember(Value = "extras.msfs.reinstall")]
        extras_msfs_reinstall,
        [EnumMember(Value = "extras.copylogs")]
        extras_copylogs,
        [EnumMember(Value = "extras.serials")]
        extras_serials,
        [EnumMember(Value = "extras.settings")]
        extras_settings,
        [EnumMember(Value = "help.docs")]
        help_docs,
        [EnumMember(Value = "help.checkforupdate")]
        help_checkforupdate,
        [EnumMember(Value = "help.discord")]
        help_discord,
        [EnumMember(Value = "help.youtube")]
        help_youtube,
        [EnumMember(Value = "help.hubhop")]
        help_hubhop,
        [EnumMember(Value = "help.about")]
        help_about,
        [EnumMember(Value = "help.releasenotes")]
        help_releasenotes
    }

    public class CommandMainMenu
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("action")]
        public CommandMainMenuAction Action { get; set; }
        [JsonProperty("index")] // Matches the lowercase "item" in JSON
        public int Index { get; set; }
    }
}
