namespace MobiFlight.Base
{
    /// <summary>
    /// Represents a generic controller device
    /// </summary>
    internal class Controller
    {
        /// <summary>
        /// Gets or sets the name of the controller
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the vendor of this controller.
        /// </summary>
        public string Vendor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the connection to the resource is currently established.
        /// </summary>
        public bool Connected { get; set; }

        /// <summary>
        /// Gets or sets the serial number associated with the object.
        /// </summary>
        public string Serial { get; set; }
    }
}
