namespace MobiFlightMoza.Protocol
{
    /// <summary>
    /// MOZA's real USB vendor/product IDs are not yet known. Every discovery path gates on
    /// <see cref="AreConfigured"/> so nothing matches these placeholder zeros against a real
    /// device before they're filled in.
    /// </summary>
    internal static class MozaHardwareIds
    {
        // TODO: replace with MOZA's real vendor ID once supplied.
        public const int VendorId = 0x0000;

        // TODO: replace with the FCD Display's real CDC product ID(s) once supplied.
        public static readonly int[] McduProductIds = [];

        public static bool AreConfigured => VendorId != 0x0000 && McduProductIds.Length > 0;
    }
}
