using MobiFlight.Base;
using System;

namespace MobiFlight.ConfigListUndo
{
    internal sealed class ProfileMovedUndoEntry : ConfigListUndoEntry
    {
        private readonly ConfigFile _file;
        private readonly ConfigFile _targetFile;
        private readonly int _originalIndex;
        private readonly int _targetIndex;

        internal ProfileMovedUndoEntry(
            ConfigFile file,
            int originalIndex,
            int targetIndex,
            ConfigFile targetFile)
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
            bool isOriginalIndexInvalid = !IsProfileIndexValid(project, _originalIndex);
            bool isTargetIndexInvalid = !IsProfileIndexValid(project, _targetIndex);
            bool isSameIndex = _originalIndex == _targetIndex;

            bool invalidIndex = isOriginalIndexInvalid || isTargetIndexInvalid || isSameIndex;

            if (invalidIndex)
            {
                reason = "Invalid profile move indices.";
                return false;
            }

            // The original target profile shifts by one position.
            int shiftedTargetIndex = _originalIndex < _targetIndex
                ? _targetIndex - 1
                : _targetIndex + 1;

            bool isMovedProfileMismatch = !IsProfileValid(project, _file, _targetIndex);
            bool isTargetProfileMismatch = !IsProfileValid(project, _targetFile, shiftedTargetIndex);

            if (isMovedProfileMismatch || isTargetProfileMismatch)
            {
                reason = "Project/profile mismatch";
                return false;
            }

            files.Move(_targetIndex, _originalIndex);

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
