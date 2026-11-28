using MobiFlight.Base;
using MobiFlight.Controllers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MobiFlight.ConfigListUndo
{
    internal sealed class ProfileMergedUndoEntry : ConfigListUndoEntry
    {
        private sealed class ProfileSnapshot
        {
            public ConfigFile File { get; }
            public List<IConfigItem> Items { get; }

            public ProfileSnapshot(ConfigFile file)
            {
                File = file;
                Items = file.ConfigItems
                    .Select(item => (IConfigItem)item.Clone())
                    .ToList();
            }
        }

        private readonly int _originalCount;
        private readonly List<ConfigFile> _mergedFiles;
        private readonly List<ProfileSnapshot> _originalProfiles;
        private readonly List<ControllerBinding> _originalControllerBindings;

        internal ProfileMergedUndoEntry(
            Project project,
            List<ConfigFile> mergedFiles)
        {
            if (project == null) throw new ArgumentNullException(nameof(project));
            if (mergedFiles == null)
                throw new ArgumentNullException(nameof(mergedFiles));

            if (mergedFiles.Count == 0)
                throw new ArgumentException(
                    "At least one merged profile is required.",
                    nameof(mergedFiles));

            _originalCount = project.ConfigFiles.Count;
            _mergedFiles = mergedFiles.ToList();

            // Capture the state before the merge.
            _originalProfiles = project.ConfigFiles
                .Select(file => new ProfileSnapshot(file))
                .ToList();

            _originalControllerBindings = project.ControllerBindings?.ToList();
        }

        public override string HistoryAction => "ProfileMerged";

        protected override bool TryUndoCore(ConfigListUndoContext context, out string reason)
        {
            var project = context.Project;
            var files = project.ConfigFiles;

            // Preserve the existing first-profile check.
            bool validOriginalProfile =
                _originalCount == 0 ||
                IsProfileValid(project, _originalProfiles[0].File, 0);

            // The recorded merged profiles must still be
            // appended in the expected order.
            bool validMerge =
                validOriginalProfile &&
                files.Count == _originalCount + _mergedFiles.Count &&
                Enumerable.Range(0, _mergedFiles.Count)
                    .All(i => IsProfileValid(project, _mergedFiles[i], _originalCount + i));

            if (!validMerge)
            {
                reason =
                    "Merged profile list no longer matches " +
                    "the recorded state.";
                return false;
            }

            int activeIndexBeforeUndo = context.ActiveConfigIndex;
            RestoreOriginalState(project);

            context.ActiveConfigIndex = Math.Min(
                activeIndexBeforeUndo,
                Math.Max(0, project.ConfigFiles.Count - 1));

            reason = null;
            return true;
        }

        // Used when a merge must be rolled back after
        // a failure during the merge process.
        internal bool RollbackPendingMerge(ConfigListUndoContext context)
        {
            if (context == null)
            {
                return false;
            }

            // Unlike normal Undo, a failed merge may be
            // incomplete, so do not require all merged
            // profiles to be present.
            RestoreOriginalState(context.Project);
            return true;
        }

        private void RestoreOriginalState(Project project)
        {
            foreach (var snapshot in _originalProfiles)
            {
                snapshot.File.ConfigItems = snapshot.Items;
            }

            project.ControllerBindings = _originalControllerBindings;

            project.ConfigFiles =
                new ObservableCollection<ConfigFile>(
                    project.ConfigFiles.Take(_originalCount));
        }

        public override void PublishChanges(ConfigListUndoContext context)
        {
            context.PublishProject();
        }
    }
}
