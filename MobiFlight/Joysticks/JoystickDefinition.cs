using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight
{
    public class JoystickDefinition
    {
        /// <summary>
        /// List of inputs supported by the device.
        /// </summary>
        public List<JoystickInput> Inputs;
        /// <summary>
        /// Instance name for the device. This is used to match the definition with a connected device.
        /// </summary>
        public string InstanceName;
        /// <summary>
        /// List of options supported by the device.
        /// </summary>
        public List<JoystickOutput> Outputs;
        /// <summary>
        /// The device's ProductId.
        /// </summary>
        public int ProductId;
        /// <summary>
        /// The device's VendorId.
        /// </summary>
        public int VendorId;

        public JoystickInput FindInputByName(string name)
        {
            return Inputs.Find(input => input.Name == name);
        }

        public void Migrate() { }
    }
}
