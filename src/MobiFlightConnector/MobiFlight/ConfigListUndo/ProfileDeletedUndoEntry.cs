using MobiFlight.Base;
using System;
using System.Linq;

namespace MobiFlight.ConfigListUndo
{
    internal sealed class ProfileDeletedUndoEntry : ConfigListUndoEntry
    {
        private readonly ConfigFile _file;
        private readonly int _originalIndex;

        internal ProfileDeletedUndoEntry(
            Project project,
            ConfigFile file,
            int originalIndex)
            : base(project)
        {
            _file = file ?? throw new ArgumentNullException(nameof(file));

            _originalIndex = originalIndex;
        }

        public override string HistoryAction => "ProfileDeleted";

        protected override bool TryUndoCore(
            ConfigListUndoContext context,
            out string reason)
        {
            var project = context.Project;

            // The deleted profile must not already exist.
            bool alreadyExists = project.ConfigFiles
                .Any(file => ReferenceEquals(file, _file));

            bool validIndex =
                _originalIndex >= 0 &&
                _originalIndex <= project.ConfigFiles.Count;

            if (alreadyExists || !validIndex)
            {
                reason = "Project/profile mismatch.";
                return false;
            }

            // Restore the original profile object.
            int activeIndexBeforeUndo = context.ActiveConfigIndex;
            project.ConfigFiles.Insert(_originalIndex, _file);

            context.ActiveConfigIndex = Math.Min(
                activeIndexBeforeUndo,
                project.ConfigFiles.Count - 1);

            reason = null;
            return true;
        }

        public override void PublishChanges(ConfigListUndoContext context)
        {
            context.PublishProject();
        }
    }
}
