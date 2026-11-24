using MobiFlight.Base;
using MobiFlight.BrowserMessages;
using MobiFlight.BrowserMessages.Outgoing;
using MobiFlight.ConfigListUndo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight
{
    internal sealed class ConfigListUndoManager
    {
        private readonly ExecutionManager _executionManager;
        private readonly ConfigListUndoContext _context;
        private readonly List<IConfigListUndoEntry> _listUndoHistory =
            new List<IConfigListUndoEntry>();

        private Project Project => _executionManager.Project;

        private List<IConfigItem> ConfigItems => _executionManager.ConfigItems;

        private int ActiveConfigIndex => _executionManager.ActiveConfigIndex;
        private const int MaxListUndoSteps = 50;
        private long _listHistoryStart = 0;

        internal ConfigListUndoManager(ExecutionManager executionManager)
        {
            _executionManager = executionManager
                ?? throw new ArgumentNullException(
                    nameof(executionManager));
            _context = new ConfigListUndoContext(executionManager);
        }

        private void PushConfigListUndo(IConfigListUndoEntry entry)
        {
            if (_listUndoHistory.Count >= MaxListUndoSteps)
            {
                _listUndoHistory.RemoveAt(0);
                _listHistoryStart++;
            }
            _listUndoHistory.Add(entry);
            PublishUndoState();
            _executionManager.NotifyConfigListActionRecorded();
        }

        public void ClearConfigListUndoHistory() 
        {
            if (_listUndoHistory.Count == 0 && _listHistoryStart == 0) return;
            _listUndoHistory.Clear();
            _listHistoryStart = 0;
            PublishUndoState();
        }

        private void PublishUndoState()
        {
            MessageExchange.Instance.Publish(
                new ConfigListUndoState
                {
                    CanUndo = _listUndoHistory.Count > 0,
                    History = _listUndoHistory.Select(entry => entry.HistoryAction).ToArray(),
                    HistoryStart = _listHistoryStart
                });
        }

        internal void RecordConfigListAddUndo(IConfigItem item, int index)
        {
            PushConfigListUndo(
                new ItemCreatedUndoEntry(
                    Project,
                    Project.ConfigFiles[ActiveConfigIndex],
                    ActiveConfigIndex,
                    item.GUID,
                    isDuplicate: false));
        }
        internal void RecordConfigListDuplicateUndo(IConfigItem item, int index)
        {
            PushConfigListUndo(
                new ItemCreatedUndoEntry(
                    Project,
                    Project.ConfigFiles[ActiveConfigIndex],
                    ActiveConfigIndex,
                    item.GUID,
                    isDuplicate: true));
        }

        internal void RecordConfigListDeleteUndo(List<IConfigItem> affectedItems)
        {
            if (affectedItems == null || affectedItems.Count == 0) return;

            PushConfigListUndo(
                new ItemDeletedUndoEntry(
                    Project,
                    Project.ConfigFiles[ActiveConfigIndex],
                    ActiveConfigIndex,
                    ConfigItems,
                    affectedItems));
        }

        internal void RecordConfigListToggleUndo(List<IConfigItem> affectedItems)
        {
            if (affectedItems == null || affectedItems.Count == 0) return; ;

            PushConfigListUndo(
                new ItemToggledUndoEntry(
                    Project,
                    Project.ConfigFiles[ActiveConfigIndex],
                    ActiveConfigIndex,
                    ConfigItems,
                    affectedItems));
        }

        internal void RecordConfigListResortUndo(
            int sourceIndex,
            int targetIndex,
            List<IConfigItem> sourceBefore,
            List<IConfigItem> targetBefore)
        {
            PushConfigListUndo(
                new ItemResortedUndoEntry(
                    Project,
                    Project.ConfigFiles[sourceIndex],
                    sourceIndex,
                    Project.ConfigFiles[targetIndex],
                    targetIndex,
                    sourceBefore,
                    targetBefore));
        }

        internal void RecordProfileDeleted(ConfigFile file, int index)
        {
            PushConfigListUndo(new ProfileDeletedUndoEntry(
                Project, file, index));
        }

        internal void RecordProfileRename(ConfigFile file, int index)
        {
            PushConfigListUndo(new ProfileRenamedUndoEntry(
                Project, file, index));
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

            PushConfigListUndo(new ProfileMovedUndoEntry(
                Project, file, originalIndex, targetIndex, Project.ConfigFiles[targetIndex]));

            return true;
        }

        internal bool RecordConfigListItemEditUndo(IConfigItem originalItem, ConfigFile file, int itemIndex, string change = null)
        {
            bool isLatestCreation = change == null &&
                _listUndoHistory.LastOrDefault() is ItemCreatedUndoEntry created &&
                created.MatchesCreation(Project, file, ActiveConfigIndex, originalItem.GUID);

            if (!isLatestCreation) return true;

            PushConfigListUndo(new ItemUpdatedUndoEntry(
                Project, file, ActiveConfigIndex, originalItem, change));

            return false;
        }

        public void RecordProfileAdded(ConfigFile file, int index)
        {
            if (file == null || Project == null) return;

            PushConfigListUndo(new ProfileAddedUndoEntry(Project, file));
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

            PushConfigListUndo(new ProfileMergedUndoEntry(Project, mergedFiles));
        }

        internal void UndoSteps(int requestedSteps)
        {
            int steps = Math.Min(Math.Max(requestedSteps, 1), _listUndoHistory.Count);
            
            for (int step = 0; step < steps; step++)
            {
                if (_listUndoHistory.Count == 0) return;

                int lastIndex = _listUndoHistory.Count - 1;
                IConfigListUndoEntry entry = _listUndoHistory[lastIndex];

                // Each entry validates and restores its own recorded state.
                if (!entry.TryUndo(_context, out string reason)) 
                {
                    RejectConfigListUndo(entry, reason);
                    return;
                }
                // Remove entry only after a successful restoration.
                _listUndoHistory.RemoveAt(lastIndex);
                // Preserve the original post-Undo order.
                _context.RefreshExecutionCaches();

                entry.PublishChanges(_context);
                _executionManager.NotifyConfigListActionUndone();
                PublishUndoState();
            }
        }

        private void RejectConfigListUndo(IConfigListUndoEntry entry, string reason)
        {
            Log.Instance.log(
                "ConfigList Undo rejected (history preserved): " +
                $"Action={entry.HistoryAction}, " +
                $"Reason={reason}",
                LogSeverity.Warn
            );

            MessageExchange.Instance.Publish(new Notification
            {
                Event = "ConfigListUndoFailed",
                Context = new Dictionary<string, string>
                {
                    { "Action", entry.HistoryAction },
                    { "Reason", reason }
                }
            });
        }

        public void CancelLastProfileMerge()
        {
            if (_listUndoHistory.Count == 0) return;

            int lastIndex = _listUndoHistory.Count - 1;

            // Only a pending merge can be rolled back here.
            if (_listUndoHistory[lastIndex] is not ProfileMergedUndoEntry entry) return;

            if (!entry.RollbackPendingMerge(_context)) return;

            // Preserve all earlier history entries.
            _listUndoHistory.RemoveAt(lastIndex);
            _context.RefreshExecutionCaches();
            entry.PublishChanges(_context);
            _executionManager.NotifyConfigListActionUndone();
            PublishUndoState();
        }
    }
}
