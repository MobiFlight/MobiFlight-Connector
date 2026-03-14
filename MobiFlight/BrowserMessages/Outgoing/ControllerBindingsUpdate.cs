using MobiFlight.Controllers;
using System.Collections.Generic;

namespace MobiFlight.BrowserMessages.Outgoing
{
    /// <summary>
    /// Outgoing message to update the frontend with the current controller bindings.
    /// </summary>
    internal class ControllerBindingsUpdate
    {
        /// <summary>
        /// Gets or sets the collection of controller bindings included in this update.
        /// </summary>
        public List<ControllerBinding> Bindings { get; set; }
    }
}
