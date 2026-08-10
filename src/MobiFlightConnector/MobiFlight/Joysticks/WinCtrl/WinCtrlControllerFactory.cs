using MobiFlightWwFcu;
using SharpDX.DirectInput;
using System.Linq;
using WebSocketSharp.Server;

namespace MobiFlight.Joysticks.WinCtrl
{
    /// <summary>
    /// Factory for creating WinCtrl controller instances based on product ID.
    /// </summary>
    internal static class WinCtrlControllerFactory
    {
        private const int WINCTRL_VENDOR_ID = 0x4098;

        /// <summary>
        /// Checks if a device can be created by this factory (WinCtrl devices).
        /// </summary>
        public static bool CanCreate(int vendorId, int productId)
        {
            if (vendorId != WINCTRL_VENDOR_ID)
            {
                return false;
            }

            return WinCtrlConstants.FCU_PRODUCTIDS.Contains(productId) ||
                   WinCtrlConstants.CDU_PRODUCTIDS.Contains(productId) ||
                   WinCtrlConstants.PAP3_PRODUCTIDS.Contains(productId) ||
                   WinCtrlConstants.AIRBUS_THROTTLE_PRODUCTIDS.Contains(productId) ||
                   WinCtrlConstants.AIRBUS_STICK_PRODUCTIDS.Contains(productId) ||
                   WinCtrlConstants.PDC3_PRODUCTIDS.Contains(productId) ||
                   WinCtrlConstants.RMP_PRODUCTIDS.Contains(productId) ||
                   WinCtrlConstants.NWS_PRODUCTIDS.Contains(productId) ||
                   productId == WinCtrlConstants.PRODUCT_ID_ECAM ||
                   productId == WinCtrlConstants.PRODUCT_ID_AGP ||
                   productId == WinCtrlConstants.PRODUCT_ID_TCAS ||
                   productId == WinCtrlConstants.PRODUCT_ID_PTO2;
        }

        /// <summary>
        /// Creates the appropriate WinCtrl controller instance based on product ID.
        /// </summary>
        public static Joystick Create(
            SharpDX.DirectInput.Joystick diJoystick,
            JoystickDefinition definition,
            int vendorId,
            int productId,
            WebSocketServer wsServer)
        {
            if (vendorId != WINCTRL_VENDOR_ID)
            {
                return null;
            }
            
            if (WinCtrlConstants.FCU_PRODUCTIDS.Contains(productId))
            {
                return new WinCtrlFcu(diJoystick, definition, productId, wsServer);
            }
            else if (WinCtrlConstants.CDU_PRODUCTIDS.Contains(productId))
            {
                return new WinCtrlCdu(diJoystick, definition, productId, wsServer);
            }
            else if (WinCtrlConstants.PAP3_PRODUCTIDS.Contains(productId))
            {
                return new WinCtrlPap3(diJoystick, definition, productId, wsServer);
            }
            else if (WinCtrlConstants.RMP_PRODUCTIDS.Contains(productId))
            {
                return new WinCtrlRmp(diJoystick, definition, productId, wsServer);
            }
            else if (WinCtrlConstants.AIRBUS_THROTTLE_PRODUCTIDS.Contains(productId) ||
                     WinCtrlConstants.AIRBUS_STICK_PRODUCTIDS.Contains(productId) ||
                     WinCtrlConstants.PDC3_PRODUCTIDS.Contains(productId) ||
                     WinCtrlConstants.NWS_PRODUCTIDS.Contains(productId) ||
                     productId == WinCtrlConstants.PRODUCT_ID_ECAM ||
                     productId == WinCtrlConstants.PRODUCT_ID_AGP ||
                     productId == WinCtrlConstants.PRODUCT_ID_TCAS ||
                     productId == WinCtrlConstants.PRODUCT_ID_PTO2)
            {
                return new WinCtrlBaseController(diJoystick, definition, productId, wsServer);
            }           

            return null;
        }
    }
}
