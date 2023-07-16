using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.Joysticks.Octavi
{
    internal class OctaviReport
    {
        public uint reportID;
        public uint buttonState;
        public byte contextState;
        public sbyte incrCoarse;
        public sbyte incrFine;

        public enum OctaviState : byte
        {
            STATE_COM1,
            STATE_COM2,
            STATE_NAV1,
            STATE_NAV2,
            STATE_FMS1,
            STATE_FMS2,
            STATE_AP,
            STATE_XPDR
        };

        public enum OctaviButton
        {
            HID_BTN_DCT = (1 << 4),
            HID_BTN_MENU = (1 << 5),
            HID_BTN_CLR = (1 << 6),
            HID_BTN_ENT = (1 << 7),
            HID_BTN_TOG = (1 << 8),
            HID_ENC_SW = (1 << 9),
            HID_BTN_AP = (1 << 14),
            HID_BTN_AP_HDG = (1 << 15),
            HID_BTN_AP_NAV = (1 << 16),
            HID_BTN_AP_APR = (1 << 17),
            HID_BTN_AP_ALT = (1 << 18),
            HID_BTN_AP_BC = (1 << 19)
        };

        public OctaviReport()
        {

        }

    public bool parseReport(byte[] inputBuffer)
        {
            // get Report ID
            reportID = inputBuffer[0];
            // get 32 bit Button report field:
            buttonState = (uint)inputBuffer[1] + ((uint)inputBuffer[2] << 8) + ((uint)inputBuffer[3] << 16) + ((uint)inputBuffer[4] << 24);
            // get coarse increment:
            incrCoarse = (sbyte)inputBuffer[5];
            // get fine increment:
            incrFine = (sbyte)inputBuffer[6];
            // get coarse increment:
            contextState = inputBuffer[7];

            return true;
        }
    }
}
