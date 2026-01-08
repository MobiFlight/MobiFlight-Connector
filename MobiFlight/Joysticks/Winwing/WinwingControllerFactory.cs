using SharpDX.DirectInput;
using System.Linq;
using WebSocketSharp.Server;

namespace MobiFlight.Joysticks.Winwing
{
    /// <summary>
    /// Factory for creating Winwing controller instances based on product ID.
    /// </summary>
    internal static class WinwingControllerFactory
    {
        private const int WINWING_VENDOR_ID = 0x4098;

        /// <summary>
        /// Checks if a device can be created by this factory (Winwing devices).
        /// </summary>
        public static bool CanCreate(int vendorId, int productId)
        {
            if (vendorId != WINWING_VENDOR_ID)
            {
                return false;
            }

            return WinwingConstants.FCU_PRODUCTIDS.Contains(productId) ||
                   WinwingConstants.CDU_PRODUCTIDS.Contains(productId) ||
                   WinwingConstants.PAP3_PRODUCTIDS.Contains(productId) ||
                   WinwingConstants.AIRBUS_THROTTLE_PRODUCTIDS.Contains(productId) ||
                   WinwingConstants.AIRBUS_STICK_PRODUCTIDS.Contains(productId) ||
                   WinwingConstants.PDC3_PRODUCTIDS.Contains(productId) ||
                   productId == WinwingConstants.PRODUCT_ID_ECAM ||
                   productId == WinwingConstants.PRODUCT_ID_AGP;
        }

        /// <summary>
        /// Creates the appropriate Winwing controller instance based on product ID.
        /// </summary>
        public static Joystick Create(
            SharpDX.DirectInput.Joystick diJoystick,
            JoystickDefinition definition,
            int vendorId,
            int productId,
            WebSocketServer wsServer)
        {
            if (vendorId != WINWING_VENDOR_ID)
            {
                return null;
            }
            
            if (WinwingConstants.FCU_PRODUCTIDS.Contains(productId))
            {
                return new WinwingFcu(diJoystick, definition, productId, wsServer);
            }
            else if (WinwingConstants.CDU_PRODUCTIDS.Contains(productId))
            {
                return new WinwingCdu(diJoystick, definition, productId, wsServer);
            }
            else if (WinwingConstants.PAP3_PRODUCTIDS.Contains(productId))
            {
                return new WinwingPap3(diJoystick, definition, productId, wsServer);
            }
            else if (WinwingConstants.AIRBUS_THROTTLE_PRODUCTIDS.Contains(productId) ||
                     WinwingConstants.AIRBUS_STICK_PRODUCTIDS.Contains(productId) ||
                     WinwingConstants.PDC3_PRODUCTIDS.Contains(productId) ||
                     productId == WinwingConstants.PRODUCT_ID_ECAM ||
                     productId == WinwingConstants.PRODUCT_ID_AGP)
            {
                return new WinwingBaseController(diJoystick, definition, productId, wsServer);
            }           

            return null;
        }
    }
}
