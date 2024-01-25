using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace MobiFlight.CustomDevices
{
    public static class CustomDeviceDefinitions
    {
        // Set to true if any errors occurred when loading the definition files.
        // Used as part of the unit test automation to determine if the checked-in
        // JSON files are valid.
        public static bool LoadingError = false;

        private static List<CustomDevice> devices = new List<CustomDevice>();

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
        public static void LoadDefinitions()
        {
            var files = new List<String>(Directory.GetFiles("Devices", "*.device.json"));
            files.AddRange(Directory.GetFiles("Community/", "*.device.json", SearchOption.AllDirectories));

            devices = JsonBackedObject.LoadDefinitions<CustomDevice>(files.ToArray(), "Devices/mfdevice.schema.json",
                onSuccess: (device, definitionFile) => {
                    Log.Instance.log($"Loaded custom device definition for {device.Info.Label} ({device.Info.Version})", LogSeverity.Info);
                    device.BasePath = Path.GetDirectoryName(Path.GetDirectoryName(definitionFile));
                },
                onError: () => LoadingError = true
            );
        }
    }
}