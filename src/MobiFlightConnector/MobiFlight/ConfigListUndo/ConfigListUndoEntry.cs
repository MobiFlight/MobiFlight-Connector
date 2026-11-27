using System;

namespace MobiFlight.ConfigListUndo
{
    internal abstract class ConfigListUndoEntry : IConfigListUndoEntry
    {
        public abstract string HistoryAction { get; }

        public bool TryUndo(ConfigListUndoContext context, out string reason)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return TryUndoCore(context, out reason);
        }

        protected abstract bool TryUndoCore(ConfigListUndoContext context, out string reason);

        public abstract void PublishChanges(ConfigListUndoContext context);
    }
}
