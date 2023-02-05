using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight
{
    public static class HidDefinitions
    {
        private static readonly List<HidDefinition> definitions = new List<HidDefinition>();

        /// <summary>
        /// Finds a HidDefinition by the device's instance name.
        /// </summary>
        /// <param name="instanceName">The instance name of the device.</param>
        /// <returns>The first definition matching the instanceMae, or null if none found.</returns>
        public static HidDefinition GetHidByInstanceName(String instanceName)
        {
            return definitions.Find(definition => definition.InstanceName == instanceName);
        }

        /// <summary>
        /// Loads all HID definitions from disk.
        /// </summary>
        public static void Load()
        {
            foreach (var definitionFile in Directory.GetFiles("HIDs", "*.hid.json"))
            {
                try
                {
                    var hid = JsonConvert.DeserializeObject<HidDefinition>(File.ReadAllText(definitionFile));
                    hid.Migrate();
                    definitions.Add(hid);
                    Log.Instance.log($"Loaded HID definition for {hid.InstanceName}", LogSeverity.Info);
                }
                catch (Exception ex)
                {
                    Log.Instance.log($"Unable to load {definitionFile}: {ex.Message}", LogSeverity.Error);
                }
            }

        }
    }
}
