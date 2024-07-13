using System;
using System.Collections.Generic;
using System.Text;

namespace MobiFlightWwFcu
{
    internal class WinwingConstants
    {
        internal static readonly byte[] DEST_FCU = new byte[] { 0x10, 0xbb };
        internal static readonly byte[] DEST_EFISL = new byte[] { 0x0d, 0xcf };
    }
}
