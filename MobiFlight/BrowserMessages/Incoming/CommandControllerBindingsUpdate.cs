using MobiFlight.Controllers;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MobiFlight.BrowserMessages.Incoming
{
    internal class CommandControllerBindingsUpdate
    {
        [JsonProperty]
        public List<ControllerBinding> Bindings { get; set; }
    }
}
