using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MobiFlight.BrowserMessages.Incoming
{
    public enum CommandShutdownAction
    {
        [EnumMember(Value = "discardChanges")]
        discardChanges
    }

    public class  CommandShutdown
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("action")]
        public CommandShutdownAction Action { get; set; }
    }
}
