namespace MobiFlight.ConfigListUndo
{
    internal interface IConfigListUndoEntry
    {
        string HistoryAction { get; }

        bool TryUndo(ConfigListUndoContext context, out string reason);

        void PublishChanges(ConfigListUndoContext context);
    }
}
