using System;
using System.Collections.Generic;
using System.IO;

namespace MobiFlight.Monitors
{
    public class UsbDeviceMonitor : DeviceMonitor
    {
        /// <summary>
        /// Returns a list of connected USB drives that are supported with MobiFlight and are in flash mode already,
        /// as opposed to being connected as COM port.
        /// </summary>
        /// <returns>The list of connected USB drives supported by MobiFlight.</returns>
        override protected void Scan()
        {
            var result = new List<PortDetails>();
            
            foreach (var drive in DriveInfo.GetDrives())
            {
                // Issue 1089: Network drives take *forever* to return the drive info slowing down startup. Only check removable drives.
                if (drive.DriveType != DriveType.Removable)
                {
                    continue;
                }

                // Issue 1074: Failing to check for IsReady caused an IOException on certain machines
                // when trying to read the volume label when the drive wasn't actually ready.
                if (!drive.IsReady)
                {
                    continue;
                }

                Board candidateBoard;
                try
                {
                    candidateBoard = BoardDefinitions.GetBoardByUsbVolumeLabel(drive.VolumeLabel);
                }
                catch (Exception ex)
                {
                    // Per the MSDN code sample for the DriveInfo object, Name and DriveType should be valid
                    // at all times so it's safe to use them in the log message.
                    Log.Instance.log($"Unable to get volume label for drive {drive.Name} ({drive.DriveType}): {ex.Message}", LogSeverity.Error);
                    continue;
                }

                if (candidateBoard != null)
                {
                    result.Add(new UsbPortDetails()
                    {
                        Board = candidateBoard,
                        HardwareId = drive.VolumeLabel,
                        Name = drive.Name,
                        // It's important that this is the drive letter for the connected USB device. This is
                        // used elsewhere in the flashing code to know that it wasn't connected via a COM
                        // port and to skip the COM port toggle before flashing.
                        Path = drive.RootDirectory.FullName
                    });
                }
            }

            UpdatePorts(result);
        }
    }
}
