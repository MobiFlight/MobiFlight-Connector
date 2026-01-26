using HidSharp;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.Monitors
{
    public class HidDeviceMonitor : ControllerMonitor
    {
        /// <summary>
        /// Returns a list of connected HID controllers
        /// </summary>
        /// <returns>The list of currently connected HID controllers.</returns>
        override protected async void Scan()
        {
            // since this method can take a while
            // don't execute it in parallel
            if (isScanning) return;
            isScanning = true;
            var result = new List<IConnectionDetails>();

            var allHidDevices = DeviceList.Local.GetHidDevices().ToList();
            foreach ( var device in allHidDevices )
            {
                result.Add(new HidConnectionDetails()
                {
                    DevicePath = device.DevicePath,
                    VendorId = device.VendorID,
                    ProductId = device.ProductID,
                    Name = device.GetProductName()
                });
            }

            isScanning = false;
            UpdateConnectionDetails(result);
        }
    }
}
