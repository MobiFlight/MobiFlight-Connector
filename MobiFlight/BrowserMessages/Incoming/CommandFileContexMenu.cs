using MobiFlight.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MobiFlight.BrowserMessages.Incoming
{
    public enum CommandFileContextMenuAction
    {
        rename,
        remove,
        export
    }
    public class CommandFileContextMenu
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("action")]
        public CommandFileContextMenuAction Action { get; set; }
        [JsonProperty("index")] // Matches the lowercase "item" in JSON
        public int Index { get; set; }
        [JsonProperty("file")] // Matches the lowercase "item" in JSON
        public ConfigFile File { get; set; }
    }
}
