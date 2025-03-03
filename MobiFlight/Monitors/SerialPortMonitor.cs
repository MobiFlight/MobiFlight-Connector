using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;

namespace MobiFlight.Monitors
{
    public class SerialPortMonitor : DeviceMonitor
    {
        /// <summary>
        /// Uses WMI to retrieve property data from a ManagementObject.
        /// </summary>
        /// <param name="managementObject">The object to retrieve the property from</param>
        /// <param name="propertyName">The name of the property to retrieve</param>
        /// <returns>The value of the property or an empty string if the property doesn't exist</returns>
        private static string GetDeviceProperty(ManagementObject managementObject, string propertyName)
        {
            // The GetDeviceProperties method is a method of Win32_PnPEntity objects, but that's not
            // a type that's exposed via System.Management. To call the method we need to use
            // InvokeMethod() instead.
            // The method takes two parameters, a string array of property names and an out parameter
            // that will contain an array of ManagementBaseObjects.
            var args = new object[] { new string[] { propertyName }, null };
            managementObject.InvokeMethod("GetDeviceProperties", args);

            // Grab the properties that were returned by the method.
            var returnedProperties = (ManagementBaseObject[])args[1];

            // It's possible that the property doesn't exist. If so just return an empty string.
            if (returnedProperties.Length == 0) return "";

            // Attempt to read the data from the returned property. Not all properties will have data so FirstOrDefault()()
            // is used to handle the case of no data being returned.
            var data = returnedProperties[0].Properties.OfType<PropertyData>().FirstOrDefault(p => p.Name == "Data");

            // Return the data or an empty string if there was no data.
            return (string)data?.Value ?? "";
        }

        /// <summary>
        /// Many Arduinos (mostly Nanos, but also some Megas) have a bad CH340 chip that
        /// doesn't work properly with Windows 11 drivers after Feburary, 2023. This method
        /// checks the name returned from the chip to see if it is the garbled name
        /// from the bad chip and returns true if it is.
        /// </summary>
        /// <param name="managementObject">A management object for the detected board</param>
        /// <returns>True if the chip is a bad CH340 chip</returns>
        private static bool IsBadCH340(ManagementObject managementObject)
        {
            var deviceName = GetDeviceProperty(managementObject, "DEVPKEY_Device_BusReportedDeviceDesc");

            // First check is for the bad name. If it doesn't match then it's not a problem board and
            // just return false.
            if (deviceName != "USB2.0-Ser!")
            {
                return false;
            }

            // Second test is the version number of the driver. Anything after 3.5.2019.1 is bad.
            var rawDriverVersion = GetDeviceProperty(managementObject, "DEVPKEY_Device_DriverVersion");

            try
            {
                var versionInfo = new Version(rawDriverVersion);
                var goodDriverVersion = new Version("3.5.2019.1");

                return versionInfo > goodDriverVersion;
            }
            catch
            {
                return false;
            }
        }
        override protected void Scan()
        {
            var portNameRegEx = "\\(.*\\)";
            var result = new List<PortDetails>();
            var regex = new Regex(@"(?<id>VID_\S*)"); // Pattern to match the VID/PID of the connected devices

            // Code from https://stackoverflow.com/questions/45165299/wmi-get-list-of-all-serial-com-ports-including-virtual-ports
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity WHERE ClassGuid=\"{4d36e978-e325-11ce-bfc1-08002be10318}\"");
                foreach (ManagementObject queryObj in searcher.Get().Cast<ManagementObject>())
                {
                    // At this point we have a list of possibly valid connected devices. Since everything at this point
                    // depends on the VID/PID extract that to start. USB devices seem to consistently have two hardwareID
                    // entries, in this order:
                    //
                    // USB\VID_1B4F&PID_9206&REV_0100&MI_00
                    // USB\VID_1B4F&PID_9206&MI_00
                    //
                    // Either will work with how the BoardDefinitions class and existing board definition files do regular expression
                    // lookups so just grab the first one in the array every time. Note the use of '?' to handle the (never seen)
                    // case where no hardware IDs are available.
                    var rawHardwareID = (queryObj["HardwareID"] as string[])?[0];

                    if (String.IsNullOrEmpty(rawHardwareID))
                    {
                        //Log.Instance.log($"Skipping module with no available VID/PID.", LogSeverity.Debug);
                        continue;
                    }

                    // Historically MobiFlight expects a straight VID/PID string without a leading USB\ or FTDI\ or \COMPORT so get
                    // pick that out of the raw hardware ID.
                    var match = regex.Match(rawHardwareID);
                    if (!match.Success)
                    {
                        //Log.Instance.log($"Skipping device with no available VID/PID ({rawHardwareID}).", LogSeverity.Debug);
                        continue;
                    }

                    // Get the matched hardware ID and use it going forward to identify the board.
                    var hardwareId = match.Groups["id"].Value;

                    //Log.Instance.log($"Checking for compatible module: {hardwareId}.", LogSeverity.Debug);
                    var board = BoardDefinitions.GetBoardByHardwareId(hardwareId);

                    // If no matching board definition is found at this point then it's an incompatible board and just keep going.
                    if (board == null)
                    {
                        //Log.Instance.log($"Incompatible module skipped: {hardwareId}.", LogSeverity.Debug);
                        continue;
                    }

                    // The board is a known type so grab the COM port for it. Every USB device seen so far has the
                    // COM port in the full name of the device surrounded by (), for example:
                    //
                    // USB Serial Device (COM22)
                    var portNameMatch = Regex.Match(queryObj["Caption"].ToString(), portNameRegEx); // Find the COM port.
                    var portName = portNameMatch?.Value.Trim(new char[] { '(', ')' }); // Remove the surrounding ().

                    if (string.IsNullOrEmpty(portName))
                    {
                        // Issue 1778: If someone renames the device in Device Manager and removes the COM
                        // port from the device name then this will be blank. Log an error and continue.
                        Log.Instance.log($"Detected {queryObj["Caption"].ToString()} but it has no COM port in its name.", LogSeverity.Error);
                        continue;
                    }

                    // Safety check to ensure duplicate entires in the registry don't result in duplicate entires in the list.
                    if (result.Any(p => p.Name == portName))
                    {
                        //Log.Instance.log($"Duplicate entry for port: {board.Info.FriendlyName} {portName}.", LogSeverity.Error);
                        continue;
                    }

                    // If the device has a bad CH340 chip then log a warning.
                    if (IsBadCH340(queryObj))
                    {
                        Log.Instance.log($"The device on port {portName} has a CH340 chip that will not work with the installed CH340 driver version. Install driver version 3.5.2019.1 or earlier, otherwise the board will not work with MobiFlight.", LogSeverity.Error);
                    }

                    result.Add(new PortDetails
                    {
                        Board = board,
                        HardwareId = hardwareId,
                        Name = portName
                    });

                    //Log.Instance.log($"Found potentially compatible module ({board.Info.FriendlyName}): {hardwareId}@{portName}.", LogSeverity.Debug);
                }

                UpdatePorts(result);
            }
            catch (ManagementException ex)
            {
                // Issue #1122 and #1600: A corrupted WMI registry caused exceptions when attempting to enumerate connected devices with searcher.Get().
                // Running "winmgmt /resetrepository" fixed it.
                Log.Instance.log($"Unable to read connected devices. This is usually caused by a corrupted WMI registry. Run 'winmgmt /resetrepository' from an elevated command line to resolve the issue. (${ex.Message})", LogSeverity.Error);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Unable to read connected devices: ${ex.Message}", LogSeverity.Error);
            }
        }
    }
}
