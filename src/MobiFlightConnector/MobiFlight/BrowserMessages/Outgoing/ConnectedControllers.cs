using MobiFlight.Base;
using System.Collections.Generic;

namespace MobiFlight.BrowserMessages.Outgoing
{
    /// <summary>
    /// Represents an outgoing message containing the list of connected controllers
    /// This information will be consumed by the browser/frontend
    /// </summary>
    internal class ConnectedControllers
    {
        /// <summary>
        /// Gets or sets the collection of controllers.
        /// The controllers are currently connected to PC.
        /// </summary>
        public List<Controller> Controllers { get; set; }
    }
}
