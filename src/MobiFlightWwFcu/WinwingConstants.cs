using System;
using System.Collections.Generic;
using System.Text;

namespace MobiFlightWwFcu
{
    internal class WinwingConstants
    {
        internal static readonly byte[] DEST_FCU = new byte[] { 0x10, 0xbb };
        internal static readonly byte[] DEST_EFISL = new byte[] { 0x0d, 0xbf };
        internal static readonly byte[] DEST_EFISR = new byte[] { 0x0e, 0xbf };

        internal const int PRODUCT_ID_FCU_ONLY = 0xBB10;
        internal const int PRODUCT_ID_FCU_EFISL = 0xBC1D;
        internal const int PRODUCT_ID_FCU_EFISR = 0xBC1E;
        internal const int PRODUCT_ID_FCU_EFISL_EFISR = 0xBA01;

        // Renaming would be a breaking change, since this names are used in mobi configuration.
        internal const string EFISL_NAME = "Left";
        internal const string EFISR_NAME = "Right";
    }
}
