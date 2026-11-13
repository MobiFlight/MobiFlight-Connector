namespace MobiFlight.BrowserMessages.Outgoing
{
    internal class ConfigListUndoState
    {
        public bool CanUndo { get; set; }
        public string[] History { get; set; }
        public long HistoryStart { get; set; }
    }
}
