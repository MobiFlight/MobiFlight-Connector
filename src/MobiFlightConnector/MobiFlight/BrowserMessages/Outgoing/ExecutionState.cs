namespace MobiFlight.BrowserMessages.Outgoing
{
    internal class ExecutionState
    {
        public bool IsRunning { get; set; }
        public bool IsTesting { get; set; }
        public bool RunAvailable { get; set; }
        public bool TestAvailable { get; set; }
    }
}
