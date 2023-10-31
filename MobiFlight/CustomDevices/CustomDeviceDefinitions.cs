using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Xml;
using System.Xml.Serialization;

namespace MobiFlight.CustomDevices
{
    public static class CustomDeviceDefinitions
    {
        private static readonly List<CustomDevice> devices = new List<CustomDevice>();

        /// <summary>
        /// Finds a device definition by matching type.
        /// </summary>
        /// <param name="type">The type that identifies the device</param>
        /// <returns>The first device definition matching the type, or null if none found.</returns>
        public static CustomDevice GetDeviceByType(String type)
        {
            return devices.Find(device => device.Info.Type == type);
        }

        /// <summary>
        /// Get all device definitions.
        /// </summary>
        /// <returns>Return all device defintions.</returns>
        public static List<CustomDevice> GetAll()
        {
            return devices;
        }

        /// <summary>
        /// Loads all device definintions from disk.
        /// </summary>
        public static void Load()
        {
            foreach (var definitionFile in Directory.GetFiles("Devices", "*.device.json"))
            {
                try
                {
                    var device = JsonConvert.DeserializeObject<CustomDevice>(File.ReadAllText(definitionFile));
                    devices.Add(device);
                    Log.Instance.log($"Loaded custom device definition for {device.Info.Label} ({device.Info.Version})", LogSeverity.Info);
                }
                catch (Exception ex)
                {
                    Log.Instance.log($"Unable to load {definitionFile}: {ex.Message}", LogSeverity.Error);
                }
            }
        }
    }
}