using MobiFlight.Base;
using MobiFlight.BrowserMessages;
using MobiFlight.BrowserMessages.Outgoing;
using MobiFlight.Controllers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;

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
        private enum ConfigListUndoAction
        {
            Delete,
            Add,
            Duplicate,
            Toggle,
            Resort,
            ItemUpdated,
            ProfileAdded,
            ProfileRename,
            ProfileDeleted,
            ProfileMerged,
            ProfileMoved

        }

        private sealed class ConfigListUndoItemState
        {
            public string Guid { get; set; }
            public int OriginalIndex { get; set; }
            public bool OriginalActive { get; set; }
            public IConfigItem Snapshot { get; set; }
        }

        private sealed class ConfigListProfileSnapshot
        {
            public ConfigFile File { get; set; }
            public List<IConfigItem> Items { get; set; }
        }

        private sealed class ConfigListUndoEntry
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
            public string ChangeAnnotation { get; set; }
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

        private void RecordConfigListUndo(
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
            };

            PushConfigListUndo(entry);
        }

        private void PushConfigListUndo(ConfigListUndoEntry entry)
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

        private void RecordConfigListItemsUndo(ConfigListUndoAction action, List<IConfigItem> affectedItems)
        {
            if (affectedItems.Count == 0) return;
            var states = affectedItems.Select(item => new ConfigListUndoItemState
            {
                Guid = item.GUID,
                OriginalIndex = ConfigItems.FindIndex(
                    cfg => cfg.GUID == item.GUID),
                OriginalActive = item.Active,
                Snapshot = action == ConfigListUndoAction.Delete
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

        internal void RecordConfigListAddUndo(IConfigItem item, int index)
        {
            RecordConfigListUndo(ConfigListUndoAction.Add, item, index);
        }

        internal void RecordConfigListDeleteUndo(List<IConfigItem> affectedItems)
        {
            RecordConfigListItemsUndo(ConfigListUndoAction.Delete, affectedItems);
        }

        internal void RecordConfigListDuplicateUndo(IConfigItem item, int index)
        {
            RecordConfigListUndo(ConfigListUndoAction.Duplicate, item, index);
        }

        internal void RecordConfigListToggleUndo(List<IConfigItem> affectedItems)
        {
            RecordConfigListItemsUndo(ConfigListUndoAction.Toggle, affectedItems);
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

        internal void RecordProfileDeleted(ConfigFile file, int index)
        {
            PushConfigListUndo(new ConfigListUndoEntry
            {
                Action = ConfigListUndoAction.ProfileDeleted,
                Project = Project,
                File = file,
                ConfigIndex = index,
                OriginalIndex = index
            });
        }

        internal void RecordProfileRename(ConfigFile file, int index)
        {
            PushConfigListUndo(new ConfigListUndoEntry
            {
                Action = ConfigListUndoAction.ProfileRename,
                Project = Project,
                File = file,
                ConfigIndex = index,
                OriginalLabel = file.Label
            });
        }

        internal bool RecordProfileMoved(ConfigFile file, int originalIndex, int targetIndex)
        {
            if (Project == null ||
                file == null ||
                originalIndex < 0 ||
                originalIndex >= Project.ConfigFiles.Count ||
                targetIndex < 0 ||
                targetIndex >= Project.ConfigFiles.Count ||
                Math.Abs(originalIndex - targetIndex) != 1 ||
                !ReferenceEquals(Project.ConfigFiles[originalIndex], file))
                {
                return false;
            }

            PushConfigListUndo(new ConfigListUndoEntry
            {
                Action = ConfigListUndoAction.ProfileMoved,
                Project = Project,
                File = file,
                ConfigIndex = originalIndex,
                OriginalIndex = originalIndex,
                TargetConfigIndex = targetIndex,
                TargetFile = Project.ConfigFiles[targetIndex]
            });

            return true;
        }

        internal bool RecordConfigListItemEditUndo(IConfigItem originalItem, ConfigFile file, int itemIndex, string change = null)
        {
            bool isLatestCreation = false;

            if (change == null && _listUndoHistory.Count > 0)
            {
                var lastEntry = _listUndoHistory[_listUndoHistory.Count - 1];

                isLatestCreation = (
                    lastEntry.Action == ConfigListUndoAction.Add ||
                    lastEntry.Action == ConfigListUndoAction.Duplicate) &&
                    lastEntry.Guid == originalItem.GUID &&
                    ReferenceEquals(lastEntry.Project, Project) &&
                    ReferenceEquals(lastEntry.File, file) &&
                    lastEntry.ConfigIndex == ActiveConfigIndex;
            }

            if (!isLatestCreation)
            {
                PushConfigListUndo(new ConfigListUndoEntry
                {
                    Action = ConfigListUndoAction.ItemUpdated,
                    ChangeAnnotation = change,
                    Project = Project,
                    File = file,
                    ConfigIndex = ActiveConfigIndex,
                    OriginalIndex = itemIndex,
                    Guid = originalItem.GUID,
                    Snapshot = (IConfigItem)originalItem.Clone()
                });
            }

            return isLatestCreation;
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

        private void PublishDeleteUndoState()
        {
            MessageExchange.Instance.Publish(new ConfigListUndoState
            {
                CanUndo = _listUndoHistory.Count > 0,
                History = _listUndoHistory
                    .Select(entry =>
                        entry.Action == ConfigListUndoAction.ItemUpdated
                            ? entry.ChangeAnnotation ?? "ItemEdited"
                            : entry.Action.ToString()
                        )
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
                        : entry.Action == ConfigListUndoAction.ProfileMoved
                            ? entry.OriginalIndex >= 0 && entry.OriginalIndex < Project.ConfigFiles.Count &&
                              actualProfileIndex == entry.TargetConfigIndex &&
                              ReferenceEquals(Project.ConfigFiles[entry.OriginalIndex], entry.TargetFile)
                            : entry.Action == ConfigListUndoAction.ProfileAdded ||
                              actualProfileIndex == entry.ConfigIndex;

                if (!validProject || !validProfile || !validIndex)
                {
                    RejectConfigListUndo(
                        entry,
                        $"Project/profile mismatch. " +
                        $"ActualIndex={actualProfileIndex}, " +
                        $"ProfileCount={Project.ConfigFiles.Count}, " +
                        $"SameProject={validProject}, " +
                        $"ValidProfile={validProfile}, " +
                        $"ValidIndex={validIndex}"
                    );

                    return;
                }

                var items = entry.File.ConfigItems;

                if (
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
                        RejectConfigListUndo(entry, $"Created ConfigItem not found: {entry.Guid}");

                        return;
                    }

                    items.RemoveAt(itemIndex);
                }

                else if (entry.Action == ConfigListUndoAction.Delete)
                {
                    bool hasConflict =
                        entry.Items == null ||
                        entry.Items.Count == 0 ||
                        entry.Items.Any(state =>
                            state.Snapshot == null ||
                            items.Any(item => item.GUID == state.Guid)
                        );

                    if (hasConflict)
                    {
                        RejectConfigListUndo(
                            entry,
                            "Delete restore failed: missing snapshot " +
                            "or conflicting ConfigItem GUID."
                        );

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
                    bool hasMissingItem =
                        entry.Items == null ||
                        entry.Items.Count == 0 ||
                        entry.Items.Any(state =>
                            !items.Any(item => item.GUID == state.Guid)
                        );

                    if (hasMissingItem)
                    {
                        RejectConfigListUndo(
                            entry,
                            "Toggle restore failed: one or more " +
                            "ConfigItems are missing."
                        );

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
                        entry.TargetFile == null ||
                        !ReferenceEquals(
                            Project.ConfigFiles[entry.TargetConfigIndex],
                            entry.TargetFile)
                        )
                    {
                        RejectConfigListUndo(
                            entry,
                            $"Resort target profile mismatch. " +
                            $"TargetIndex={entry.TargetConfigIndex}, " +
                            $"ProfileCount={Project.ConfigFiles.Count}"
                        );

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
                        !new HashSet<string>(currentGuids).SetEquals(expectedGuids)
                        )
                    {
                        RejectConfigListUndo(
                            entry,
                            $"Resort GUID mismatch. " +
                            $"CurrentCount={currentGuids.Count}, " +
                            $"ExpectedCount={expectedGuids.Count}"
                        );

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
                    if (entry.File.ConfigItems.Count > 0)
                    {
                        RejectConfigListUndo(
                            entry,
                            "Added profile contains ConfigItems."
                        );

                        return;
                    }

                    Project.ConfigFiles.RemoveAt(actualProfileIndex);

                    ActiveConfigIndex = Math.Clamp(
                        ActiveConfigIndex,
                        0,
                        Math.Max(0, Project.ConfigFiles.Count - 1)
                    );
                }

                else if (entry.Action == ConfigListUndoAction.ProfileRename)
                {
                    entry.File.Label = entry.OriginalLabel;
                }

                else if (entry.Action == ConfigListUndoAction.ProfileMoved)
                {
                    int fromIndex = entry.ConfigIndex;
                    int toIndex = entry.TargetConfigIndex;

                    Project.ConfigFiles.Move(fromIndex, toIndex);

                    if (ActiveConfigIndex == fromIndex)
                    {
                        ActiveConfigIndex = toIndex;
                    }
                    else if (ActiveConfigIndex == toIndex)
                    {
                        ActiveConfigIndex = fromIndex;
                    }
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
                        RejectConfigListUndo(
                            entry,
                            "Merged profile list no longer matches the recorded state."
                        );

                        return;
                    }

                    RestoreProfileMerge(entry);

                    ActiveConfigIndex = Math.Min(
                        ActiveConfigIndex,
                        Math.Max(0, Project.ConfigFiles.Count - 1)
                    );
                }

                else if (entry.Action == ConfigListUndoAction.ItemUpdated)
                {
                    int itemIndex = items.FindIndex(i => i.GUID == entry.Guid);

                    if (itemIndex < 0 || entry.Snapshot == null)
                    {
                        RejectConfigListUndo(
                            entry,
                            $"ConfigItem not found or snapshot missing: {entry.Guid}"
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
                    entry.Action == ConfigListUndoAction.ProfileMerged ||
                    entry.Action == ConfigListUndoAction.ProfileMoved)
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
                    entry.Action != ConfigListUndoAction.ProfileMerged &&
                    entry.Action != ConfigListUndoAction.ProfileMoved)
                {
                    _executionManager.NotifyConfigListUndoChanged(items);
                }

                if (entry.Action == ConfigListUndoAction.ProfileMoved)
                {
                    MessageExchange.Instance.Publish(new ProfileMoveUndone
                    {
                        FromIndex = entry.TargetConfigIndex,
                        ToIndex = entry.OriginalIndex
                    });
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
                    {
                        "Action",
                        entry.Action == ConfigListUndoAction.ItemUpdated
                            ? entry.ChangeAnnotation ?? "ItemEdited"
                            : entry.Action.ToString()
                    },
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
