using MobiFlight.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.ConfigListUndo
{
    internal sealed class ItemDeletedUndoEntry : ConfigListUndoEntry
    {
        private sealed class DeletedItemState
        {
            public string Guid { get; }
            public int OriginalIndex { get; }
            public IConfigItem Snapshot { get; }

            public DeletedItemState(IConfigItem item, int originalIndex)
            {
                Guid = item.GUID;
                OriginalIndex = originalIndex;
                Snapshot = (IConfigItem)item.Clone();
            }
        }

        private readonly ConfigFile _file;
        private readonly int _configIndex;
        private readonly List<DeletedItemState> _items;

        internal ItemDeletedUndoEntry(
            Project project,
            ConfigFile file,
            int configIndex,
            List<IConfigItem> sourceItems,
            List<IConfigItem> affectedItems)
            : base(project)
        {
            _file = file ?? throw new ArgumentNullException(nameof(file));

            _configIndex = configIndex;

            if (sourceItems == null)
                throw new ArgumentNullException(nameof(sourceItems));

            if (affectedItems == null)
                throw new ArgumentNullException(nameof(affectedItems));

            _items = affectedItems
                .Select(item => new DeletedItemState(
                    item,
                    sourceItems.FindIndex(cfg => cfg.GUID == item.GUID)))
                .OrderBy(state => state.OriginalIndex)
                .ToList();
        }

        public override string HistoryAction => "Delete";

        protected override bool TryUndoCore(ConfigListUndoContext context, out string reason)
        {
            var project = context.Project;

            bool isConfigIndexInvalid = _configIndex < 0 || _configIndex >= project.ConfigFiles.Count;

            bool isProfileMismatch =
                !isConfigIndexInvalid &&
                !ReferenceEquals(project.ConfigFiles[_configIndex], _file);

            if (isConfigIndexInvalid || isProfileMismatch)
            {
                reason = "Project/profile mismatch.";
                return false;
            }

            var items = _file.ConfigItems;

            bool hasConflict =
                _items.Count == 0 ||
                _items.Any(state =>
                    state.Snapshot == null ||
                    items.Any(item => item.GUID == state.Guid));

            if (hasConflict)
            {
                reason =
                    "Delete restore failed: missing snapshot " +
                    "or conflicting ConfigItem GUID.";
                return false;
            }

            // Restore deleted items at their original positions.
            foreach (var state in _items)
            {
                int restoreIndex = Math.Min(Math.Max(state.OriginalIndex, 0), items.Count);

                items.Insert(restoreIndex, state.Snapshot);
            }

            reason = null;
            return true;
        }

        public override void PublishChanges(ConfigListUndoContext context)
        {
            context.PublishConfigItems(_configIndex, _file.ConfigItems);

            context.NotifyConfigChanged(_file.ConfigItems);
        }
    }
}
