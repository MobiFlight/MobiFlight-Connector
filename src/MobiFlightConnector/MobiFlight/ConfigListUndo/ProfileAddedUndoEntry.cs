using MobiFlight.Base;
using System;
using System.Linq;

namespace MobiFlight.ConfigListUndo
{
    internal sealed class ProfileAddedUndoEntry : ConfigListUndoEntry
    {
        private readonly ConfigFile _file;

        internal ProfileAddedUndoEntry(
            Project project,
            ConfigFile file)
            : base(project)
        {
            _file = file ?? throw new ArgumentNullException(nameof(file));
        }

        public override string HistoryAction => "ProfileAdded";

        protected override bool TryUndoCore(ConfigListUndoContext context, out string reason)
        {
            var project = context.Project;

            int currentIndex = project.ConfigFiles
                .ToList()
                .FindIndex(file => ReferenceEquals(file, _file));

            if (currentIndex < 0)
            {
                reason = "Project/profile mismatch.";
                return false;
            }

            // A newly added profile can only be removed
            // if it does not contain configuration items.
            if (_file.ConfigItems.Count > 0)
            {
                reason = "Added profile contains ConfigItems.";
                return false;
            }

            int activeIndexBeforeUndo = context.ActiveConfigIndex;
            project.ConfigFiles.RemoveAt(currentIndex);

            context.ActiveConfigIndex = Math.Clamp(
                activeIndexBeforeUndo,
                0,
                Math.Max(0, project.ConfigFiles.Count - 1));

            reason = null;
            return true;
        }

        public override void PublishChanges(ConfigListUndoContext context)
        {
            context.PublishProject();
        }
    }
}
