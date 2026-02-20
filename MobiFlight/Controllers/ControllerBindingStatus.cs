using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace MobiFlight.Controllers
{
    /// <summary>
    /// Represents the binding status of a controller
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))] 
    public enum ControllerBindingStatus
    {
        /// <summary>
        /// Controller is connected with exact serial match (Scenario 1)
        /// </summary>
        [EnumMember(Value = "Match")]
        Match,

        /// <summary>
        /// Controller was automatically bound (Scenarios 2, 3, 6)
        /// </summary>
        [EnumMember(Value = "AutoBind")]
        AutoBind,

        /// <summary>
        /// Controller is not connected (Scenario 4)
        /// </summary>
        [EnumMember(Value = "Missing")]
        Missing,

        /// <summary>
        /// Multiple controllers found, requires manual selection (Scenario 5)
        /// </summary>
        [EnumMember(Value = "RequiresManualBind")]
        RequiresManualBind
    }
}