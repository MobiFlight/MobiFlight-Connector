namespace MobiFlightMoza.Protocol
{
    /// <summary>
    /// MOZA's real USB vendor/product IDs. Every discovery path gates on
    /// <see cref="AreConfigured"/> so nothing matches before these are filled in.
    /// </summary>
    public static class MozaHardwareIds
    {
        public const int VendorId = 0x346E;

        // The FCD Display's composite device reports this PID on both the HID base-unit
        // interface and the CDC display interface this list is used to locate.
        public static readonly int[] McduProductIds = [0x1305];

        public static bool AreConfigured => VendorId != 0x0000 && McduProductIds.Length > 0;
    }
}
