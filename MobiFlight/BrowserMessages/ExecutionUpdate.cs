using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace MobiFlight.BrowserMessages
{
    public enum ExecutionState
    {
        Running,
        Paused,
        Stopped,
        Testing
    }

    public class ExecutionUpdate
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ExecutionState State { get; set; }
    }
}
