using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MobiFlight.Monitors
{
    public class UsbDeviceMonitor : DeviceMonitor
    {
        /// <summary>
        /// Returns the IsReady value of the DriveInfo but also
        /// applies a timeout to not wait for it too long.
        /// </summary>
        /// <param name="drive"></param>
        /// <returns>Actual `IsReady` value, or false if timed out</returns>
        async Task<bool> DriveIsReady(DriveInfo drive)
        {
            // Issue 1074: Failing to check for IsReady caused an IOException on certain machines
            // when trying to read the volume label when the drive wasn't actually ready.
            // Issue 1437: IsReady takes forever to timeout when called on USB card readers that have no
            // cards in them. Wrap the test in a task so we limit the damage to only 100ms.
            var task = Task.Run(() =>
            {
                return drive.IsReady;
            });

            if (await Task.WhenAny(task, Task.Delay(100)) != task)
            {
                return false;
            }

            return task.Result;
        }
        /// <summary>
        /// Returns a list of connected USB drives that are supported with MobiFlight and are in flash mode already,
        /// as opposed to being connected as COM port.
        /// </summary>
        /// <returns>The list of connected USB drives supported by MobiFlight.</returns>
        override protected async void Scan()
        {
            // since this method can take a while
            // don't execute it in parallel
            if (isScanning) return;
            isScanning = true;

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
                if (!await DriveIsReady(drive))
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
            isScanning = false;
            UpdatePorts(result);
        }
    }
}
