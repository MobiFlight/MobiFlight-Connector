using MobiFlight.Base;
using MobiFlight.ConfigListUndo;
using System;
using System.Linq;

namespace MobiFlight.ConfigListUndo
{
    internal sealed class ProfileMovedUndoEntry : ConfigListUndoEntry
    {
        private readonly ConfigFile _file;
        private readonly ConfigFile _targetFile;
        private readonly int _originalIndex;
        private readonly int _targetIndex;

        internal ProfileMovedUndoEntry(
            Project project,
            ConfigFile file,
            int originalIndex,
            int targetIndex,
            ConfigFile targetFile)
            : base(project)
        {
            _file = file ?? throw new ArgumentNullException(nameof(file));

            _targetFile = targetFile ?? throw new ArgumentNullException(nameof(targetFile));

            _originalIndex = originalIndex;
            _targetIndex = targetIndex;
        }

        public override string HistoryAction => "ProfileMoved";

        protected override bool TryUndoCore(ConfigListUndoContext context, out string reason)
        {
            var project = context.Project;
            var files = project.ConfigFiles;

            int actualIndex = files
                .ToList()
                .FindIndex(file => ReferenceEquals(file, _file));

            bool validIndex =
                _originalIndex >= 0 &&
                _originalIndex < files.Count &&
                _targetIndex >= 0 &&
                _targetIndex < files.Count &&
                Math.Abs(_originalIndex - _targetIndex) == 1;

            bool validState =
                validIndex &&
                actualIndex == _targetIndex &&
                ReferenceEquals(files[_originalIndex], _targetFile);

            if (!validState)
            {
                reason = "Project/profile mismatch.";
                return false;
            }

            // Preserve the original adjacent-swap
            // restoration behavior.
            files.Move(_originalIndex, _targetIndex);

            if (context.ActiveConfigIndex == _originalIndex)
            {
                context.ActiveConfigIndex = _targetIndex;
            }
            else if (context.ActiveConfigIndex == _targetIndex)
            {
                context.ActiveConfigIndex = _originalIndex;
            }

            reason = null;
            return true;
        }

        public override void PublishChanges(ConfigListUndoContext context)
        {
            context.PublishProject();

            context.PublishProfileMoveUndone(_targetIndex, _originalIndex);
        }
    }
}
