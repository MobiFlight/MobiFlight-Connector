using MobiFlight.Joysticks.AuthentiKit;
using MobiFlight.Joysticks.Octavi;
using MobiFlight.Joysticks.VKB;
using MobiFlight.Joysticks.Winwing;
using SharpDX.DirectInput;
using System;
using System.Linq;
using WebSocketSharp.Server;

namespace MobiFlight.Joysticks
{
    /// <summary>
    /// Factory for creating DirectInput-based controller instances.
    /// </summary>
    internal static class ControllerFactory
    {
        /// <summary>
        /// Checks if a device can be created by this factory (returns true for specialized controllers).
        /// </summary>
        public static bool CanCreate(DeviceInstance deviceInstance, int vendorId, int productId)
        {
            var instanceName = deviceInstance.InstanceName;

            // Check for Octavi/IFR1
            if (instanceName == "Octavi" || instanceName == "IFR1")
            {
                return true;
            }

            // Check for Winwing devices
            if (WinwingControllerFactory.CanCreate(vendorId, productId))
            {
                return true;
            }

            // Check for VKB devices
            if (vendorId == VKBDevice.VKB_VENDOR_ID)
            {
                return true;
            }

            // Check for AuthentiKit
            if (instanceName.Trim() == "AuthentiKit")
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the correct product name for a device, handling special cases like VKB.
        /// </summary>
        public static string GetProductName(DeviceInstance deviceInstance, SharpDX.DirectInput.Joystick diJoystick, int vendorId)
        {
            var instanceName = deviceInstance.InstanceName;

            // VKB devices: get product name from HID device
            if (vendorId == VKBDevice.VKB_VENDOR_ID)
            {
                // VKB devices are highly configurable. DirectInput names can have old values cached in the registry, 
                // but HID names seem to be immune to that. Also trim the extraneous whitespaces on VKB device names.
                var hidDevice = VKBDevice.GetMatchingHidDevice(diJoystick);
                if (hidDevice != null)
                {
                    return hidDevice.GetProductName().Trim();
                }
            }

            return instanceName.Trim();
        }

        /// <summary>
        /// Creates the appropriate controller instance based on the device information.
        /// Returns null if the device should be handled as a standard Joystick.
        /// </summary>
        public static Joystick Create(
            DeviceInstance deviceInstance,
            SharpDX.DirectInput.Joystick diJoystick,
            int vendorId,
            int productId,
            JoystickDefinition definition,
            WebSocketServer wsServer)
        {
            var instanceName = deviceInstance.InstanceName;

            // Handle Octavi/IFR1 devices by instance name
            if (instanceName == "Octavi" || instanceName == "IFR1")
            {
                return new Octavi.Octavi(diJoystick, definition);
            }

            // Handle Winwing devices by vendor/product ID
            if (WinwingControllerFactory.CanCreate(vendorId, productId))
            {
                return WinwingControllerFactory.Create(diJoystick, definition, vendorId, productId, wsServer);
            }

            // Handle VKB devices by vendor ID
            if (vendorId == VKBDevice.VKB_VENDOR_ID)
            {
                return new VKBDevice(diJoystick, definition);
            }

            // Handle AuthentiKit by instance name
            if (instanceName.Trim() == "AuthentiKit")
            {
                return new AuthentiKit.AuthentiKit(diJoystick, definition);
            }

            // Return null to indicate this should be handled as a standard Joystick
            return null;
        }
    }
}
