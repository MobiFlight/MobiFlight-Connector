using System.Collections.Generic;

namespace MobiFlightWwFcu
{
    internal class WinwingConstants
    {
        internal static readonly byte[] DEST_FCU = new byte[] { 0x10, 0xbb };
        internal static readonly byte[] DEST_EFISL = new byte[] { 0x0d, 0xbf };
        internal static readonly byte[] DEST_EFISR = new byte[] { 0x0e, 0xbf };

        internal static readonly byte[] DEST_MCDU = new byte[] { 0x32, 0xbb };
        internal static readonly byte[] DEST_PFP3N = new byte[] { 0x31, 0xbb };
        internal static readonly byte[] DEST_PFP7 = new byte[] { 0x33, 0xbb };
        internal static readonly byte[] DEST_PFP4 = new byte[] { 0x34, 0xbb };

        internal static readonly byte[] DEST_PAP3 = new byte[] { 0x0f, 0xbf };

        internal static readonly byte[] DEST_3NPDC = new byte[] { 0x60, 0xbb };
        internal static readonly byte[] DEST_3MPDC = new byte[] { 0x50, 0xbb };

        internal static readonly byte[] DEST_AIRBUS_THROTTLE = new byte[] { 0x10, 0xb9 };
        internal static readonly byte[] DEST_AIRBUS_PAC = new byte[] { 0x01, 0xb9 };

        internal static readonly byte[] DEST_AIRBUS_STICK = new byte[] { 0x20, 0xbb };
        internal static readonly byte[] DEST_AIRBUS_STICK_VIBRATION_R = new byte[] { 0x08, 0xbf };
        internal static readonly byte[] DEST_AIRBUS_STICK_VIBRATION_L = new byte[] { 0x07, 0xbf };

        internal static readonly byte[] DEST_ECAM = new byte[] { 0x70, 0xbb };
        internal static readonly byte[] DEST_AGP  = new byte[] { 0x80, 0xbb };

        internal static readonly byte[] DEST_PTO2 = new byte[] { 0x05, 0xbf };


        internal const int PRODUCT_ID_FCU_ONLY = 0xBB10;
        internal const int PRODUCT_ID_FCU_EFISL = 0xBC1D;
        internal const int PRODUCT_ID_FCU_EFISR = 0xBC1E;
        internal const int PRODUCT_ID_FCU_EFISL_EFISR = 0xBA01;

        internal const int PRODUCT_ID_PFP3N_CPT = 0xBB35;
        internal const int PRODUCT_ID_PFP3N_OBS = 0xBB39;
        internal const int PRODUCT_ID_PFP3N_FO = 0xBB3D;

        internal const int PRODUCT_ID_MCDU_CPT = 0xBB36;
        internal const int PRODUCT_ID_MCDU_OBS = 0xBB3A;
        internal const int PRODUCT_ID_MCDU_FO = 0xBB3E;

        internal const int PRODUCT_ID_PFP7_CPT = 0xBB37;
        internal const int PRODUCT_ID_PFP7_OBS = 0xBB3B;
        internal const int PRODUCT_ID_PFP7_FO = 0xBB3F;

        internal const int PRODUCT_ID_PFP4_CPT = 0xBB38;
        internal const int PRODUCT_ID_PFP4_OBS = 0xBB3C;
        internal const int PRODUCT_ID_PFP4_FO = 0xBB40;

        internal const int PRODUCT_ID_PAP3 = 0xBF0F;

        internal const int PRODUCT_ID_3NPDCL = 0xBB61;
        internal const int PRODUCT_ID_3NPDCR = 0xBB62;
        internal const int PRODUCT_ID_3MPDCL = 0xBB51;
        internal const int PRODUCT_ID_3MPDCR = 0xBB52;

        internal const int PRODUCT_ID_AIRBUS_THROTTLE_L = 0xB920;
        internal const int PRODUCT_ID_AIRBUS_THROTTLE_R = 0xB930;

        internal const int PRODUCT_ID_AIRBUS_STICK_L = 0xBC27;
        internal const int PRODUCT_ID_AIRBUS_STICK_R = 0xBC28;

        internal const int PRODUCT_ID_ECAM = 0xBB70;
        internal const int PRODUCT_ID_AGP  = 0xBB80;

        internal const int PRODUCT_ID_PTO2 = 0xBF05;

        internal static readonly int[] FCU_PRODUCTIDS = { PRODUCT_ID_FCU_ONLY, PRODUCT_ID_FCU_EFISL, PRODUCT_ID_FCU_EFISR, PRODUCT_ID_FCU_EFISL_EFISR };
        internal static readonly int[] CDU_PRODUCTIDS = { PRODUCT_ID_MCDU_CPT, PRODUCT_ID_MCDU_OBS, PRODUCT_ID_MCDU_FO,
                                                          PRODUCT_ID_PFP3N_CPT, PRODUCT_ID_PFP3N_OBS, PRODUCT_ID_PFP3N_FO,
                                                          PRODUCT_ID_PFP7_CPT, PRODUCT_ID_PFP7_OBS, PRODUCT_ID_PFP7_FO,
                                                          PRODUCT_ID_PFP4_CPT, PRODUCT_ID_PFP4_OBS, PRODUCT_ID_PFP4_FO };

        internal const string CDU_DATA = "Cdu Data";
        internal const string FONT_DATA = "Font Data";


        // Renaming would be a breaking change, since these names are used in mobi configuration.
        // The name is part of the Lcd and Led descriptors.
        internal const string EFISL_NAME = "Left";
        internal const string EFISR_NAME = "Right";
  
        internal const string PDC3NL_NAME = "3N PDC Left";
        internal const string PDC3NR_NAME = "3N PDC Right";
        internal const string PDC3ML_NAME = "3M PDC Left";
        internal const string PDC3MR_NAME = "3M PDC Right";
   
        internal const string AIRBUS_THROTTLE_L_NAME = "Airbus Throttle Left";
        internal const string AIRBUS_THROTTLE_R_NAME = "Airbus Throttle Right";
   
        internal const string AIRBUS_STICK_L_NAME = "Airbus Sidestick Left";
        internal const string AIRBUS_STICK_R_NAME = "Airbus Sidestick Right";

        internal static Dictionary<string, byte[]> DisplayCmdHeaders = new Dictionary<string, byte[]>()
        {
            { "0201",   new byte[] { 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00 } }, // FCU
            { "0201_E", new byte[] { 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00 } }, // EFIS
            { "0201_PAP", new byte[] { 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x2B, 0x00, 0x00, 0x00 } }, // PAP3
            { "0201_PAC", new byte[] { 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x24, 0x00, 0x00, 0x00 } }, // PAC
            { "0201_AGP", new byte[] { 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x24, 0x00, 0x00, 0x00 } }, // AGP
            { "0301",   new byte[] { 0x03, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } },
            { "0401",   new byte[] { 0x04, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 } },
            { "0501",   new byte[] { 0x05, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } },
            { "0601",   new byte[] { 0x06, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x19, 0x00, 0x00, 0x00 } },
            { "0701",   new byte[] { 0x07, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, } }, // Without Length
            { "1001",   new byte[] { 0x10, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00 } },
            { "1201",   new byte[] { 0x12, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00 } },
            { "1301",   new byte[] { 0x13, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00 } },
            { "1501",   new byte[] { 0x15, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } },
            { "1801",   new byte[] { 0x18, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00 } },
            { "1901",   new byte[] { 0x19, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0e, 0x00, 0x00, 0x00 } },
            { "1a01",   new byte[] { 0x1a, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 } },
            { "1c01",   new byte[] { 0x1c, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } },
            { "1e01",   new byte[] { 0x1e, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } },
        };


        // For 7 segment it expects: 
        // top, topright, bottomright, bottom, bottomleft, topleft, middle
        internal static Dictionary<char, bool[]> CharacterDict = new Dictionary<char, bool[]>()
        {
            {'0', new bool[] { true, true, true, true, true, true, false } },
            {'1', new bool[] { false, true, true, false, false, false, false } },
            {'2', new bool[] { true, true, false, true, true, false, true } },
            {'3', new bool[] { true, true, true, true, false, false, true } },
            {'4', new bool[] { false, true, true, false, false, true, true } },
            {'5', new bool[] { true, false, true, true, false, true, true } },
            {'6', new bool[] { true, false, true, true, true, true, true } },
            {'7', new bool[] { true, true, true, false, false, false, false } },
            {'8', new bool[] { true, true, true, true, true, true, true } },
            {'9', new bool[] { true, true, true, true, false, true, true } },
            {'A', new bool[] { true, true, true, false, true, true, true } },
            {'B', new bool[] { true, true, true, true, true, true, true } },
            {'b', new bool[] { false, false, true, true, true, true, true } },
            {'C', new bool[] { true, false, false, true, true, true, false } },
            {'d', new bool[] { false, true, true, true, true, false, true } },
            {'E', new bool[] { true, false, false, true, true, true, true } },
            {'F', new bool[] { true, false, false, false, true, true, true } },
            {'S', new bool[] { true, false, true, true, false, true, true } },
            {'G', new bool[] { true, false, true, true, true, true, true } },
            {'P', new bool[] { true, true, false, false, true, true, true } },
            {'L', new bool[] { false, false, false, true, true, true, false } },
            {'l', new bool[] { false, false, false, false, true, true, false } },
            {'t', new bool[] { false, false, false, true, true, true, true } },
            {'o', new bool[] { false, false, true, true, true, false, true } },
            {'i', new bool[] { false, false, false, false, true, false, false } },
            {'-', new bool[] { false, false, false, false, false, false, true } },
            {'_', new bool[] { false, false, false, true, false, false, false } },
            {'[', new bool[] { true, false, false, true, true, true, false } },
            {']', new bool[] { true, true, true, true, false, false, false } },
            {'{', new bool[] { true, true, false, false, true, true, false } },
            {'}', new bool[] { true, true, true, false, false, true, false } },
            {'*', new bool[] { false, false, false, false, false, false, false } },
        };
    }
}
