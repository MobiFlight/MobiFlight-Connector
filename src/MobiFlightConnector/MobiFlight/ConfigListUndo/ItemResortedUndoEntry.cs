using MobiFlight.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight.ConfigListUndo
{
    internal sealed class ItemResortedUndoEntry : ConfigListUndoEntry
    {
        private readonly ConfigFile _sourceFile;
        private readonly int _sourceIndex;

        private readonly ConfigFile _targetFile;
        private readonly int _targetIndex;

        private readonly List<string> _sourceOrderBefore;
        private readonly List<string> _targetOrderBefore;

        internal ItemResortedUndoEntry(
            Project project,
            ConfigFile sourceFile,
            int sourceIndex,
            ConfigFile targetFile,
            int targetIndex,
            List<IConfigItem> sourceBefore,
            List<IConfigItem> targetBefore)
            : base(project)
        {
            _sourceFile = sourceFile ?? throw new ArgumentNullException(nameof(sourceFile));

            _targetFile = targetFile ?? throw new ArgumentNullException(nameof(targetFile));

            if (sourceBefore == null)
                throw new ArgumentNullException(nameof(sourceBefore));

            bool sameProfile = ReferenceEquals(sourceFile, targetFile);

            if (!sameProfile && targetBefore == null)
                throw new ArgumentNullException(nameof(targetBefore));

            _sourceIndex = sourceIndex;
            _targetIndex = targetIndex;

            // Store GUID order instead of entire item snapshots.
            _sourceOrderBefore = sourceBefore
                .Select(item => item.GUID)
                .ToList();

            _targetOrderBefore = sameProfile
                ? null
                : targetBefore
                    .Select(item => item.GUID)
                    .ToList();
        }

        public override string HistoryAction => "Resort";

        protected override bool TryUndoCore(ConfigListUndoContext context, out string reason)
        {
            var project = context.Project;

            bool isSourceIndexInvalid = _sourceIndex < 0 || _sourceIndex >= project.ConfigFiles.Count;

            bool isSourceProfileMismatch =
                !isSourceIndexInvalid &&
                !ReferenceEquals(project.ConfigFiles[_sourceIndex], _sourceFile);

            if (isSourceIndexInvalid || isSourceProfileMismatch)
            {
                reason = "Project/profile mismatch.";
                return false;
            }

            bool isTargetIndexInvalid = _targetIndex < 0 || _targetIndex >= project.ConfigFiles.Count;

            bool isTargetProfileMismatch =
                !isTargetIndexInvalid &&
                !ReferenceEquals(project.ConfigFiles[_targetIndex], _targetFile);

            if (isTargetIndexInvalid || isTargetProfileMismatch)
            {
                reason =
                    "Resort target profile mismatch. " +
                    $"TargetIndex={_targetIndex}, " +
                    $"ProfileCount={project.ConfigFiles.Count}";

                return false;
            }

            bool sameProfile = ReferenceEquals(_sourceFile, _targetFile);

            var sourceItems = _sourceFile.ConfigItems;
            var targetItems = _targetFile.ConfigItems;

            // Collect all current items affected by the move.
            var currentItems = sameProfile
                ? sourceItems.ToList()
                : sourceItems.Concat(targetItems).ToList();

            // Collect the GUIDs expected before the move.
            var expectedGuids = sameProfile
                ? _sourceOrderBefore.ToList()
                : _sourceOrderBefore
                    .Concat(_targetOrderBefore)
                    .ToList();

            var currentGuids = currentItems
                .Select(item => item.GUID)
                .ToList();

            bool hasDifferentItemCount = currentGuids.Count != expectedGuids.Count;

            bool hasDuplicateCurrentGuids = currentGuids.Distinct().Count() != currentGuids.Count;

            bool hasDuplicateExpectedGuids =
                expectedGuids.Distinct().Count() != expectedGuids.Count;

            bool hasDifferentGuidSet =
                !new HashSet<string>(currentGuids).SetEquals(expectedGuids);

            bool hasConflict =
                hasDifferentItemCount ||
                hasDuplicateCurrentGuids ||
                hasDuplicateExpectedGuids ||
                hasDifferentGuidSet;

            if (hasConflict)
            {
                reason =
                    "Resort GUID mismatch. " +
                    $"CurrentCount={currentGuids.Count}, " +
                    $"ExpectedCount={expectedGuids.Count}";

                return false;
            }

            // Build restored lists before modifying either profile.
            var itemsByGuid = currentItems.ToDictionary(item => item.GUID);

            var restoredSource = _sourceOrderBefore
                .Select(guid => itemsByGuid[guid])
                .ToList();

            var restoredTarget = sameProfile
                ? null
                : _targetOrderBefore
                    .Select(guid => itemsByGuid[guid])
                    .ToList();

            // All validation has passed.
            sourceItems.Clear();
            sourceItems.AddRange(restoredSource);

            if (!sameProfile)
            {
                targetItems.Clear();
                targetItems.AddRange(restoredTarget);
            }

            reason = null;
            return true;
        }

        public override void PublishChanges(ConfigListUndoContext context)
        {
            // Always update the source profile.
            context.PublishConfigItems( _sourceIndex, _sourceFile.ConfigItems);

            // Update the target profile when it is different.
            if (_targetIndex != _sourceIndex)
            {
                context.PublishConfigItems(_targetIndex, _targetFile.ConfigItems);
            }

            // Preserve the original change notification behavior.
            context.NotifyConfigChanged(_sourceFile.ConfigItems);
        }
    }
}
