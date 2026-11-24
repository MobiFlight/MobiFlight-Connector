using MobiFlight.Base;
using System;

namespace MobiFlight.ConfigListUndo
{
    internal abstract class ConfigListUndoEntry : IConfigListUndoEntry
    {
        protected ConfigListUndoEntry(Project project)
        {
            RecordedProject = project ?? throw new ArgumentNullException(nameof(project));
        }

        protected Project RecordedProject { get; }

        public abstract string HistoryAction { get; }

        public bool TryUndo(ConfigListUndoContext context, out string reason)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (!ReferenceEquals(RecordedProject, context.Project))
            {
                reason = "Project mismatch.";
                return false;
            }

            return TryUndoCore(context, out reason);
        }

        protected abstract bool TryUndoCore(ConfigListUndoContext context, out string reason);

        public abstract void PublishChanges(ConfigListUndoContext context);
    }
}
