namespace MobiFlight.Scripts
{
    public class ScriptMapping
    {
        public string VendorId { get; set; }
        public string[] ProductIds { get; set; }
        // Deprecated, use AircraftMatchPattern instead.
        public string AircraftIdSnippet { get; set; }
        // Regex to match against the aircraft ID. The script is run if the
        // pattern matches somewhere within the aicraft ID.
        public string AircraftMatchPattern { get; set; }
        public string ScriptName { get; set; }
    }
}
