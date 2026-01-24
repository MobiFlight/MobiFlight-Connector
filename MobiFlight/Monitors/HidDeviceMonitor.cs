using HidSharp;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.Monitors
{
    public class HidDeviceMonitor : ControllerMonitor
    {
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
            var result = new List<IConnectionDetails>();

            var allHidDevices = DeviceList.Local.GetHidDevices().ToList();
            foreach ( var device in allHidDevices )
            {
                var candidate = new JoystickDefinition()
                {
                    InstanceName = device.GetProductName()
                };

                if (candidate != null)
                {
                    result.Add(new HidConnectionDetails()
                    {
                        Path = device.DevicePath,
                        VendorId = device.VendorID,
                        ProductId = device.ProductID,
                        Name = device.GetProductName()
                    });
                }

            }

            isScanning = false;
            UpdateConnectionDetails(result);
        }
    }
}
