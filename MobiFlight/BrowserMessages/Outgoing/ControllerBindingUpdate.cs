using MobiFlight.Controllers;
using System.Collections.Generic;

namespace MobiFlight.BrowserMessages.Outgoing
{
    internal class ControllerBindingUpdate
    {
        public List<ControllerBinding> Bindings { get; set; }
    }
}
