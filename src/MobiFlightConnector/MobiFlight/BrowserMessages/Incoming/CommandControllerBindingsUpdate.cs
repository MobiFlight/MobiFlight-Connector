using MobiFlight.Controllers;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MobiFlight.BrowserMessages.Incoming
{
    /// <summary>
    /// Represents an incoming message to update controller bindings
    /// Message is originated by the browser and handled by the backend
    /// </summary>
    internal class CommandControllerBindingsUpdate
    {
        /// <summary>
        /// Gets or sets the list of controller bindings to update
        /// </summary>
        [JsonProperty]
        public List<ControllerBinding> Bindings { get; set; }
    }
}
