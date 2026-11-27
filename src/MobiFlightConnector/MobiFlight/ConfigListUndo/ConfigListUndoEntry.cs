using MobiFlight.Base;
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

        protected static bool IsProfileIndexValid(Project project, int index)
        {
            return index >= 0 && index < project.ConfigFiles.Count;
        }

        protected static bool IsProfileValid(Project project, ConfigFile file, int index)
        {
            return IsProfileIndexValid(project, index) &&
                ReferenceEquals(project.ConfigFiles[index], file);
        }

        protected abstract bool TryUndoCore(ConfigListUndoContext context, out string reason);

        public abstract void PublishChanges(ConfigListUndoContext context);
    }
}
