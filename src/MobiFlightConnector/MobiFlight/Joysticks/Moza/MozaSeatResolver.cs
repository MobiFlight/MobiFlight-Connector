namespace MobiFlight.Joysticks.Moza
{
    /// <summary>
    /// Maps the MCDU settings channel's <c>cabinPosition</c> (guide §6.2, setting 0x13) to
    /// the CDU websocket path the existing Winwing scripts already publish to.
    /// </summary>
    internal static class MozaSeatResolver
    {
        private const string CaptainPath = "/winwing/cdu-captain";
        private const string FirstOfficerPath = "/winwing/cdu-co-pilot";

        /// <summary>0 = captain, 1 = first officer. Anything else falls back to captain
        /// with a logged warning - a documented fallback, not a silent guess.</summary>
        public static string ResolvePath(byte cabinPosition)
        {
            switch (cabinPosition)
            {
                case 0: return CaptainPath;
                case 1: return FirstOfficerPath;
                default:
                    Log.Instance.log($"MozaSeatResolver - Unknown cabinPosition {cabinPosition}, defaulting to captain.", LogSeverity.Warn);
                    return CaptainPath;
            }
        }
    }
}
