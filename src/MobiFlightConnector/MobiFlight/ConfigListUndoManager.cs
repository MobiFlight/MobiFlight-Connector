using MobiFlight.Base;
using MobiFlight.BrowserMessages;
using MobiFlight.BrowserMessages.Outgoing;
using MobiFlight.Controllers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MobiFlight
{
    internal sealed class ConfigListUndoManager
    {
        private readonly ExecutionManager _executionManager;

        private Project Project => _executionManager.Project;

        private List<IConfigItem> ConfigItems => _executionManager.ConfigItems;

        private int ActiveConfigIndex
        {
            get => _executionManager.ActiveConfigIndex;
            set => _executionManager.ActiveConfigIndex = value;
        }

        internal ConfigListUndoManager(ExecutionManager executionManager)
        {
            _executionManager = executionManager;
        }
        internal enum ConfigListUndoAction
        {
            Delete,
            Add,
            Duplicate,
            BulkDelete,
            Toggle,
            Resort,
            ProfileAdded,
            ProfileRename,
            ProfileDeleted,
            ItemRename,
            ProfileMerged,
            ItemEdited
        }

        internal sealed class ConfigListUndoItemState
        {
            public string Guid { get; set; }
            public int OriginalIndex { get; set; }
            public bool OriginalActive { get; set; }
            public IConfigItem Snapshot { get; set; }
        }

        internal sealed class ConfigListProfileSnapshot
        {
            public ConfigFile File { get; set; }
            public List<IConfigItem> Items { get; set; }
        }

        internal sealed class ConfigListUndoEntry
        {
            public ConfigListUndoAction Action { get; set; }
            public Project Project { get; set; }
            public ConfigFile File { get; set; }
            public int ConfigIndex { get; set; }
            public int OriginalIndex { get; set; }
            public string Guid { get; set; }
            public IConfigItem Snapshot { get; set; }
            public List<ConfigListUndoItemState> Items { get; set; }
            public ConfigFile TargetFile { get; set; }
            public int TargetConfigIndex { get; set; }
            public List<string> SourceOrderBefore { get; set; }
            public List<string> TargetOrderBefore { get; set; }
            public string OriginalLabel { get; set; }
            public string OriginalName { get; set; }
            public List<ConfigFile> MergedFiles { get; set; }
            public List<ConfigListProfileSnapshot> OriginalProfiles
            {
                get;
                set;
            }
            public List<ControllerBinding> OriginalControllerBindings
            {
                get;
                set;
            }
        }

        private const int MaxListUndoSteps = 50;

        private readonly List<ConfigListUndoEntry> _listUndoHistory =
            new List<ConfigListUndoEntry>();

        private long _listHistoryStart = 0;

        internal List<ConfigListUndoEntry> UndoHistory => _listUndoHistory;

        internal void RecordConfigListUndo(
            ConfigListUndoAction action,
            IConfigItem item,
            int originalIndex)
        {
            var entry = new ConfigListUndoEntry
            {
                Action = action,
                Project = Project,
                File = Project.ConfigFiles[ActiveConfigIndex],
                ConfigIndex = ActiveConfigIndex,
                OriginalIndex = originalIndex,
                Guid = item.GUID,

                // Only deleted items need their full configuration
                // restored. Added items only need to be removed.
                Snapshot = action == ConfigListUndoAction.Delete
                    ? (IConfigItem)item.Clone()
                    : null
            };

            PushConfigListUndo(entry);
        }

        internal void PushConfigListUndo(ConfigListUndoEntry entry)
        {
            if (_listUndoHistory.Count >= MaxListUndoSteps)
            {
                _listUndoHistory.RemoveAt(0);
                _listHistoryStart++;
            }
            _listUndoHistory.Add(entry);
            PublishDeleteUndoState();
            _executionManager.NotifyConfigListActionRecorded();
        }

        internal void RecordConfigListBulkUndo(ConfigListUndoAction action, List<IConfigItem> affectedItems)
        {
            if (affectedItems.Count == 0) return;
            var states = affectedItems.Select(item => new ConfigListUndoItemState
            {
                Guid = item.GUID,
                OriginalIndex = ConfigItems.FindIndex(
                    cfg => cfg.GUID == item.GUID),
                OriginalActive = item.Active,
                Snapshot = action == ConfigListUndoAction.BulkDelete
                    ? (IConfigItem)item.Clone()
                    : null
            })
            .OrderBy(item => item.OriginalIndex)
            .ToList();

            var entry = new ConfigListUndoEntry
            {
                Action = action,
                Project = Project,
                File = Project.ConfigFiles[ActiveConfigIndex],
                ConfigIndex = ActiveConfigIndex,
                Items = states
            };

            PushConfigListUndo(entry);
        }

        internal void RecordConfigListResortUndo(
            int sourceIndex,
            int targetIndex,
            List<IConfigItem> sourceBefore,
            List<IConfigItem> targetBefore)
        {
            var entry = new ConfigListUndoEntry
            {
                Action = ConfigListUndoAction.Resort,
                Project = Project,
                File = Project.ConfigFiles[sourceIndex],
                ConfigIndex = sourceIndex,
                TargetFile = Project.ConfigFiles[targetIndex],
                TargetConfigIndex = targetIndex,
                SourceOrderBefore = sourceBefore
                    .Select(item => item.GUID)
                    .ToList(),
                TargetOrderBefore = sourceIndex == targetIndex
                    ? null
                    : targetBefore
                        .Select(item => item.GUID)
                        .ToList()
            };
            PushConfigListUndo(entry);
        }

        public void RecordProfileAdded(ConfigFile file, int index)
        {
            if (file == null || Project == null)
                return;

            PushConfigListUndo(new ConfigListUndoEntry
            {
                Action = ConfigListUndoAction.ProfileAdded,
                Project = Project,
                File = file,
                ConfigIndex = index,
                OriginalIndex = index
            });
        }

        public void RecordProfileMerged(List<ConfigFile> mergedFiles)
        {
            if (
                Project == null ||
                mergedFiles == null ||
                mergedFiles.Count == 0
            )
            {
                return;
            }

            var originalProfiles = Project.ConfigFiles
                .Select(file => new ConfigListProfileSnapshot
                {
                    File = file,
                    Items = file.ConfigItems
                        .Select(item => (IConfigItem)item.Clone())
                        .ToList()
                })
                .ToList();

            PushConfigListUndo(new ConfigListUndoEntry
            {
                Action = ConfigListUndoAction.ProfileMerged,
                Project = Project,

                File = Project.ConfigFiles.FirstOrDefault(),
                ConfigIndex = 0,

                OriginalIndex = Project.ConfigFiles.Count,

                MergedFiles = mergedFiles.ToList(),
                OriginalProfiles = originalProfiles,

                OriginalControllerBindings =
                    Project.ControllerBindings?.ToList()
            });
        }

        public void ClearConfigListUndoHistory()
        {
            if (_listUndoHistory.Count == 0 && _listHistoryStart == 0) return;

            _listUndoHistory.Clear();
            _listHistoryStart = 0;
            PublishDeleteUndoState();
        }

        internal void PublishDeleteUndoState()
        {
            MessageExchange.Instance.Publish(new ConfigListUndoState
            {
                CanUndo = _listUndoHistory.Count > 0,
                History = _listUndoHistory
                    .Select(entry => entry.Action.ToString())
                    .ToArray(),
                HistoryStart = _listHistoryStart
            });
        }

        internal void UndoSteps(int requestedSteps)
        {
            int steps = Math.Min(
                Math.Max(requestedSteps, 1),
                _listUndoHistory.Count);

            for (int step = 0; step < steps; step++)
            {
                if (_listUndoHistory.Count == 0)
                    return;

                int lastIndex = _listUndoHistory.Count - 1;
                var entry = _listUndoHistory[lastIndex];

                int actualProfileIndex = Project.ConfigFiles
                    .ToList()
                    .FindIndex(file => ReferenceEquals(file, entry.File));

                bool validProject = ReferenceEquals(Project, entry.Project);

                bool isProfileDeleted = entry.Action == ConfigListUndoAction.ProfileDeleted;
                bool isProfileMerged = entry.Action == ConfigListUndoAction.ProfileMerged;
                bool validProfile = isProfileDeleted
                    ? actualProfileIndex == -1
                    : isProfileMerged
                        ? entry.OriginalIndex == 0 || actualProfileIndex == 0
                        : actualProfileIndex >= 0;

                bool validIndex = isProfileDeleted
                    ? entry.ConfigIndex >= 0 && entry.ConfigIndex <= Project.ConfigFiles.Count
                    : isProfileMerged
                        ? true
                        : entry.Action == ConfigListUndoAction.ProfileAdded ||
                          actualProfileIndex == entry.ConfigIndex;

                if (!validProject || !validProfile || !validIndex)
                {
                    Log.Instance.log(
                        $"ConfigList Undo validation failed: " +
                        $"Action={entry.Action}, " +
                        $"RecordedIndex={entry.ConfigIndex}, " +
                        $"ActualIndex={actualProfileIndex}, " +
                        $"ProfileCount={Project.ConfigFiles.Count}, " +
                        $"SameProject={validProject}",
                        LogSeverity.Warn
                    );

                    return;
                }

                var items = entry.File.ConfigItems;

                if (entry.Action == ConfigListUndoAction.Delete)
                {
                    // The deleted GUID must not already exist.
                    if (items.Any(i => i.GUID == entry.Guid))
                    {
                        ClearConfigListUndoHistory();
                        return;
                    }

                    int restoreIndex = Math.Min(
                        Math.Max(entry.OriginalIndex, 0),
                        items.Count
                    );

                    items.Insert(
                        restoreIndex,
                        (IConfigItem)entry.Snapshot.Clone()
                    );
                }
                else if (
                    entry.Action == ConfigListUndoAction.Add ||
                    entry.Action == ConfigListUndoAction.Duplicate)
                {
                    // Undo Add/Duplicate by removing the newly
                    // created item, never the original item.
                    int itemIndex = items.FindIndex(
                        i => i.GUID == entry.Guid
                    );

                    if (itemIndex < 0)
                    {
                        ClearConfigListUndoHistory();
                        return;
                    }

                    items.RemoveAt(itemIndex);
                }

                else if (entry.Action == ConfigListUndoAction.BulkDelete)
                {
                    bool hasConflict = entry.Items.Any(
                        state => items.Any(
                            item => item.GUID == state.Guid
                        )
                    );
                    if (hasConflict)
                    {
                        ClearConfigListUndoHistory();
                        return;
                    }
                    foreach (var state in entry.Items.OrderBy(
                        state => state.OriginalIndex))
                    {
                        int restoreIndex = Math.Min(
                            Math.Max(state.OriginalIndex, 0),
                            items.Count
                        );
                        items.Insert(
                            restoreIndex,
                            (IConfigItem)state.Snapshot.Clone()
                        );
                    }
                }

                else if (entry.Action == ConfigListUndoAction.Toggle)
                {
                    bool hasMissingItem = entry.Items.Any(
                        state => !items.Any(
                            item => item.GUID == state.Guid
                        )
                    );
                    if (hasMissingItem)
                    {
                        ClearConfigListUndoHistory();
                        return;
                    }
                    foreach (var state in entry.Items)
                    {
                        var item = items.Find(
                            item => item.GUID == state.Guid
                        );
                        item.Active = state.OriginalActive;
                    }
                }

                else if (entry.Action == ConfigListUndoAction.Resort)
                {
                    if (
                        entry.TargetConfigIndex < 0 ||
                        entry.TargetConfigIndex >= Project.ConfigFiles.Count ||
                        !ReferenceEquals(
                            Project.ConfigFiles[entry.TargetConfigIndex],
                            entry.TargetFile)
                        )
                    {
                        ClearConfigListUndoHistory();
                        return;
                    }

                    bool sameProfile = ReferenceEquals(entry.File, entry.TargetFile);
                    var targetItems = entry.TargetFile.ConfigItems;
                    var currentItems = sameProfile
                        ? items.ToList()
                        : items.Concat(targetItems).ToList();
                    var expectedGuids = entry.SourceOrderBefore
                        .Concat(
                            sameProfile
                                ? Enumerable.Empty<string>()
                                : entry.TargetOrderBefore).ToList();
                    var currentGuids = currentItems.Select(item => item.GUID).ToList();

                    if (
                        currentGuids.Count != expectedGuids.Count ||
                        currentGuids.Distinct().Count() != currentGuids.Count ||
                        expectedGuids.Distinct().Count() != expectedGuids.Count ||
                        !new HashSet<string>(currentGuids)
                            .SetEquals(expectedGuids))
                    {
                        ClearConfigListUndoHistory();
                        return;
                    }
                    var itemsByGuid = currentItems.ToDictionary(item => item.GUID);
                    items.Clear();

                    foreach (string guid in entry.SourceOrderBefore)
                    {
                        items.Add(itemsByGuid[guid]);
                    }
                    if (!sameProfile)
                    {
                        targetItems.Clear();
                        foreach (string guid in entry.TargetOrderBefore)
                        {
                            targetItems.Add(itemsByGuid[guid]);
                        }
                    }
                }

                else if (entry.Action == ConfigListUndoAction.ProfileAdded)
                {
                    if (
                        Project.ConfigFiles.Count <= 1 ||
                        entry.File.ConfigItems.Count > 0
                    )
                    {
                        Log.Instance.log(
                            $"Cannot undo ProfileAdded: " +
                            $"ProfileCount={Project.ConfigFiles.Count}, " +
                            $"ItemCount={entry.File.ConfigItems.Count}",
                            LogSeverity.Warn
                        );
                        return;
                    }

                    Project.ConfigFiles.RemoveAt(actualProfileIndex);

                    ActiveConfigIndex = Math.Min(
                        ActiveConfigIndex,
                        Project.ConfigFiles.Count - 1
                    );
                }

                else if (entry.Action == ConfigListUndoAction.ProfileRename)
                {
                    entry.File.Label = entry.OriginalLabel;
                }

                else if (entry.Action == ConfigListUndoAction.ProfileDeleted)
                {
                    Project.ConfigFiles.Insert(entry.OriginalIndex, entry.File);
                    ActiveConfigIndex = Math.Min(ActiveConfigIndex, Project.ConfigFiles.Count - 1);
                }

                else if (entry.Action == ConfigListUndoAction.ProfileMerged)
                {
                    int originalCount = entry.OriginalIndex;
                    var mergedFiles = entry.MergedFiles;

                    bool validMerge =
                        mergedFiles != null &&
                        mergedFiles.Count > 0 &&
                        Project.ConfigFiles.Count ==
                            originalCount + mergedFiles.Count &&
                        Enumerable.Range(0, mergedFiles.Count)
                            .All(i => ReferenceEquals(
                                Project.ConfigFiles[originalCount + i],
                                mergedFiles[i]
                            ));

                    if (!validMerge)
                    {
                        Log.Instance.log(
                            "Cannot undo ProfileMerged: " +
                            "merged profile list has changed.",
                            LogSeverity.Warn
                        );
                        return;
                    }

                    RestoreProfileMerge(entry);

                    ActiveConfigIndex = Math.Min(
                        ActiveConfigIndex,
                        Math.Max(0, Project.ConfigFiles.Count - 1)
                    );
                }

                else if (entry.Action == ConfigListUndoAction.ItemRename)
                {
                    var item = items.Find(i => i.GUID == entry.Guid);
                    if (item == null)
                    {
                        Log.Instance.log($"Cannot undo ItemRename: {entry.Guid} not found.", LogSeverity.Warn);
                        return;
                    }
                    item.Name = entry.OriginalName;
                }

                else if (entry.Action == ConfigListUndoAction.ItemEdited)
                {
                    int itemIndex = items.FindIndex(i => i.GUID == entry.Guid);

                    if (itemIndex < 0 || entry.Snapshot == null)
                    {
                        Log.Instance.log(
                            $"Cannot undo ItemEdited: {entry.Guid} not found or snapshot is missing.",
                            LogSeverity.Warn
                            );
                        return;
                    }

                    items[itemIndex] = (IConfigItem)entry.Snapshot.Clone();
                }
                // Remove only the last history entry.
                _listUndoHistory.RemoveAt(lastIndex);

                // Refresh execution caches.
                _executionManager.OnInputConfigSettingsChanged(this, null);

                // Synchronize frontend from authoritative .NET state.

                if (
                    entry.Action == ConfigListUndoAction.ProfileAdded ||
                    entry.Action == ConfigListUndoAction.ProfileRename ||
                    entry.Action == ConfigListUndoAction.ProfileDeleted ||
                    entry.Action == ConfigListUndoAction.ProfileMerged)
                {
                    MessageExchange.Instance.Publish(Project);
                }
                else
                {
                    MessageExchange.Instance.Publish(
                    new ConfigValueFullUpdate(
                        entry.ConfigIndex,
                        items
                    ));
                    if (
                        entry.Action == ConfigListUndoAction.Resort &&
                        entry.TargetConfigIndex != entry.ConfigIndex)
                    {
                        MessageExchange.Instance.Publish(
                        new ConfigValueFullUpdate(
                            entry.TargetConfigIndex,
                            entry.TargetFile.ConfigItems
                        ));
                    }
                }

                if (
                    entry.Action != ConfigListUndoAction.ProfileAdded &&
                    entry.Action != ConfigListUndoAction.ProfileDeleted &&
                    entry.Action != ConfigListUndoAction.ProfileMerged)
                {
                    _executionManager.NotifyConfigListUndoChanged(items);
                }

                _executionManager.NotifyConfigListActionUndone();

                PublishDeleteUndoState();
            }
        }

        private void RejectConfigListUndo(ConfigListUndoEntry entry, string reason)
        {
            string guid = entry.Guid ?? "(none)";
            string profile = entry.File?.Label ?? "(unknown)";

            Log.Instance.log(
                $"ConfigList Undo rejected (history preserved): " +
                $"Action={entry.Action}, " +
                $"Guid={guid}, " +
                $"RecordedIndex={entry.ConfigIndex}, " +
                $"Profile={profile}, " +
                $"Reason={reason}",
                LogSeverity.Warn
            );

            MessageExchange.Instance.Publish(new Notification
            {
                Event = "ConfigListUndoFailed",
                Context = new Dictionary<string, string>
        {
            { "Action", entry.Action.ToString() },
            { "Reason", reason }
        }
            });
        }

        private void RestoreProfileMerge(ConfigListUndoEntry entry)
        {
            foreach (var snapshot in entry.OriginalProfiles)
            {
                snapshot.File.ConfigItems = snapshot.Items
                    .Select(item => (IConfigItem)item.Clone())
                    .ToList();
            }

            Project.ControllerBindings =
                entry.OriginalControllerBindings?.ToList();

            Project.ConfigFiles = new ObservableCollection<ConfigFile>(
                Project.ConfigFiles.Take(entry.OriginalIndex)
            );
        }

        public void CancelLastProfileMerge()
        {
            if (_listUndoHistory.Count == 0)
                return;

            int lastIndex = _listUndoHistory.Count - 1;
            var entry = _listUndoHistory[lastIndex];

            if (
                entry.Action != ConfigListUndoAction.ProfileMerged ||
                !ReferenceEquals(entry.Project, Project)
            )
            {
                return;
            }

            RestoreProfileMerge(entry);

            _listUndoHistory.RemoveAt(lastIndex);

            _executionManager.OnInputConfigSettingsChanged(this, null);

            MessageExchange.Instance.Publish(Project);

            _executionManager.NotifyConfigListActionUndone();

            PublishDeleteUndoState();
        }
    }
}
