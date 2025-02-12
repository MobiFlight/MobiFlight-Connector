using System.Collections.Generic;

namespace MobiFlight.Joysticks.Winwing
{
    internal class WinwingConstants
    {
        internal static readonly byte[] DEST_FCU = new byte[] { 0x10, 0xbb };
        internal static readonly byte[] DEST_EFISL = new byte[] { 0x0d, 0xbf };
        internal static readonly byte[] DEST_EFISR = new byte[] { 0x0e, 0xbf };

        internal static readonly byte[] DEST_MCDU = new byte[] { 0x32, 0xbb };
        internal static readonly byte[] DEST_PFP3N = new byte[] { 0x31, 0xbb };

        internal const int PRODUCT_ID_FCU_ONLY = 0xBB10;
        internal const int PRODUCT_ID_FCU_EFISL = 0xBC1D;
        internal const int PRODUCT_ID_FCU_EFISR = 0xBC1E;
        internal const int PRODUCT_ID_FCU_EFISL_EFISR = 0xBA01;

        internal const int PRODUCT_ID_MCDU_CPT = 0xBB36;
        internal const int PRODUCT_ID_MCDU_OBS = 0xBB3A;
        internal const int PRODUCT_ID_MCDU_FO = 0xBB3E;

        internal const int PRODUCT_ID_PFP3N_CPT = 0xBB35;
        internal const int PRODUCT_ID_PFP3N_OBS = 0xBB39;
        internal const int PRODUCT_ID_PFP3N_FO = 0xBB3D;

        internal static readonly int[] FCU_PRODUCTIDS = { PRODUCT_ID_FCU_ONLY, PRODUCT_ID_FCU_EFISL, PRODUCT_ID_FCU_EFISR, PRODUCT_ID_FCU_EFISL_EFISR };
        internal static readonly int[] CDU_PRODUCTIDS = { PRODUCT_ID_MCDU_CPT, PRODUCT_ID_MCDU_OBS, PRODUCT_ID_MCDU_FO,
                                                          PRODUCT_ID_PFP3N_CPT, PRODUCT_ID_PFP3N_OBS, PRODUCT_ID_PFP3N_FO };

        internal const string CDU_DATA = "Cdu Data";


        // Renaming would be a breaking change, since this names are used in mobi configuration.
        internal const string EFISL_NAME = "Left";
        internal const string EFISR_NAME = "Right";


        internal static Dictionary<string, byte[]> DisplayCmdHeaders = new Dictionary<string, byte[]>()
        {
            { "0201",   new byte[] { 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00 } },
            { "0201_E", new byte[] { 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00 } },
            { "0301",   new byte[] { 0x03, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } },
            { "0401",   new byte[] { 0x04, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 } },
        };
    }
}
