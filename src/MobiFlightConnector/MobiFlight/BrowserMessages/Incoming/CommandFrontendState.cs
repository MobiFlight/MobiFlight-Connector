using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace MobiFlight.BrowserMessages.Incoming
{
    public class CommandFrontendState
    {
        public enum RouteState
        {
            [EnumMember(Value = "ready")]
            Ready,
            [EnumMember(Value = "loading")]
            Loading,
            [EnumMember(Value = "error")]
            Error
        }

        [JsonProperty("route")]
        public string Route { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("state")]
        public RouteState State { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}