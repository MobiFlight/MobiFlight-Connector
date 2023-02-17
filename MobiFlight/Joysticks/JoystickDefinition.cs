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
        /// The device's USB ProductId. Required if Outputs are provided.
        /// </summary>
        public int ProductId;

        /// <summary>
        /// The device's USB VendorId. Required if Outputs are provided.
        /// </summary>
        public int VendorId;

        /// <summary>
        /// Finds a JoystickInput given an input name. This will eventually get replaced with a method that
        /// looks up by Id instead.
        /// </summary>
        /// <param name="name">The name of the input to look up</param>
        /// <returns>The first JoystickInput that matches the specified name or null if none found.</returns>
        public JoystickInput FindInputByName(string name)
        {
            return Inputs.Find(input => input.Name == name);
        }

        /// <summary>
        /// Migrates values from a prior version of the JSON schema to the newest version.
        /// </summary>
        public void Migrate() { }
    }
}
