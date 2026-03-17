using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MobiFlight.BrowserMessages.Incoming
{
    public enum CommandProjectToolbarAction
    {
        run,
        stop,
        test,
        toggleAutoRun,
        rename
    }

    public class CommandProjectToolbar
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("action")]
        public CommandProjectToolbarAction Action { get; set; }

        [JsonProperty("value")] // Matches the lowercase "item" in JSON
        public string Value { get; set; }
    }
}
