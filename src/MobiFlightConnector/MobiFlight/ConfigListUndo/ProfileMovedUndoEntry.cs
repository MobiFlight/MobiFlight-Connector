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

        internal static bool SkipRecording(Project project, ConfigFile file, int originalIndex, int targetIndex)
        {
            bool isProjectMissing = project == null;
            bool isFileMissing = file == null;

            if (isProjectMissing || isFileMissing)
            {
                return true;
            }

            bool isOriginalIndexInvalid = originalIndex < 0 || originalIndex >= project.ConfigFiles.Count;

            bool isTargetIndexInvalid = targetIndex < 0 || targetIndex >= project.ConfigFiles.Count;

            if (isOriginalIndexInvalid || isTargetIndexInvalid)
            {
                return true;
            }

            bool isNotAdjacent = Math.Abs(originalIndex - targetIndex) != 1;

            bool isProfileMismatch = !ReferenceEquals(project.ConfigFiles[originalIndex], file);

            return isNotAdjacent || isProfileMismatch;
        }

        public override string HistoryAction => "ProfileMoved";

        protected override bool TryUndoCore(ConfigListUndoContext context, out string reason)
        {
            var project = context.Project;
            var files = project.ConfigFiles;

            int currentIndex = files
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
                currentIndex == _targetIndex &&
                ReferenceEquals(files[_originalIndex], _targetFile);

            if (!validState)
            {
                reason = "Project/profile mismatch.";
                return false;
            }

            // Preserve the original adjacent-swap
            // restoration behavior.
            int activeIndexBeforeUndo = context.ActiveConfigIndex;
            int restoredActiveIndex = activeIndexBeforeUndo;

            if (activeIndexBeforeUndo == _originalIndex)
            {
                restoredActiveIndex = _targetIndex;
            }
            else if (activeIndexBeforeUndo == _targetIndex)
            {
                restoredActiveIndex = _originalIndex;
            }

            // Moving a profile triggers ProjectChanged,
            // which resets ActiveConfigIndex.
            files.Move(_originalIndex, _targetIndex);

            // Restore the calculated index after ProjectChanged.
            context.ActiveConfigIndex = restoredActiveIndex;

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
