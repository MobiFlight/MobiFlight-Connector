namespace MobiFlight.Joysticks.FlightSimBuilder
{
    internal class Gns530Report
    {
        public uint reportID;
        public int rxAxis;
        public uint buttonState;
        
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

        public Gns530Report()
        {

        }

    public bool parseReport(byte[] inputBuffer)
        {
            // get Report ID
            reportID = inputBuffer[0];

            // Extract the 10-bit value (bitmask 0x03FF isolates the first 10 bits)
            rxAxis = (inputBuffer[2] << 8 | inputBuffer[1]) & 0x03FF;
            // get 32 bit Button report field:
            buttonState = (uint)inputBuffer[3] + ((uint)inputBuffer[4] << 8) + ((uint)inputBuffer[5] << 16) + ((uint)inputBuffer[6] << 24);
            
            return true;
        }
    }
}
