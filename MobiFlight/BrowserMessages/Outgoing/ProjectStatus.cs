namespace MobiFlight.BrowserMessages.Outgoing
{
    internal class ProjectStatus
    {
        public bool HasChanged { get; set; }
        public string SaveStatus { get; set; } = "idle";
    }
}
