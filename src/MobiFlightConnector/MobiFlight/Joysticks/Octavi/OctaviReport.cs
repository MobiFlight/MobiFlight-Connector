using System;

namespace MobiFlight.Joysticks.Octavi
{
    internal class OctaviReport
    {
        public uint reportId;
        public OctaviButtons buttonState;
        public OctaviState contextState;
        public sbyte outerEncoderDelta;
        public sbyte innerEncoderDelta;

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

        [Flags]
        public enum OctaviButtons : uint
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
            HID_BTN_AP_VS = (1 << 19)
        };

        public OctaviReport()
        {

        }

        public void parseReport(byte[] inputBuffer)
        {
            // get Report ID
            reportId = inputBuffer[0];
            // get 32 bit Button report field:
            buttonState = (OctaviReport.OctaviButtons)((uint)inputBuffer[1] + ((uint)inputBuffer[2] << 8) + ((uint)inputBuffer[3] << 16) + ((uint)inputBuffer[4] << 24));
            // get coarse increment/decrement:
            outerEncoderDelta = (sbyte)inputBuffer[5];
            // get fine increment/decrement:
            innerEncoderDelta = (sbyte)inputBuffer[6];
            // get coarse increment:
            contextState = (OctaviReport.OctaviState)inputBuffer[7];
        }
    }
}
