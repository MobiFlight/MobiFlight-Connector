using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MobiFlight.BrowserMessages.Incoming
{
    public enum CommandAddConfigFileType
    {
        create,
        merge
    }
    public class CommandAddConfigFile
    {
        [JsonProperty("type")] // Matches the lowercase "item" in JSON
        [JsonConverter(typeof(StringEnumConverter))]
        public CommandAddConfigFileType Type { get; set; }
    }
}
