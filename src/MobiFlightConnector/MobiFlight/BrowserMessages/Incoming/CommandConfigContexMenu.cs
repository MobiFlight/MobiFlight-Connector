using MobiFlight.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MobiFlight.BrowserMessages.Incoming
{
    public enum CommandConfigContextMenuAction
    {
        delete,
        duplicate,
        test,
        settings,
        toggle,
        testStart,
        testStop
    }
    internal class CommandConfigContextMenu
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("action")]
        public CommandConfigContextMenuAction Action { get; set; }
        [JsonProperty("item")] // Matches the lowercase "item" in JSON
        public ConfigItem Item { get; set; }
    }
}
