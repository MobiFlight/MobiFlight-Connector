using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight
{
    public class HidDefinition
    {
        /// <summary>
        /// List of inputs supported by the device.
        /// </summary>
        public List<HidInput> Inputs;
        /// <summary>
        /// Instance name for the device. This is used to match the definition with a connected device.
        /// </summary>
        public string InstanceName;
        /// <summary>
        /// List of options supported by the device.
        /// </summary>
        public List<HidOutput> Outputs;
        /// <summary>
        /// The device's ProductId.
        /// </summary>
        public int ProductId;
        /// <summary>
        /// The device's VendorId.
        /// </summary>
        public int VendorId;

        public void Migrate() { }
    }
}
