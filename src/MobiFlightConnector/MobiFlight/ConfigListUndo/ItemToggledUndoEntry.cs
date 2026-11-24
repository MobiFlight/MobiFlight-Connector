using MobiFlight.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.ConfigListUndo
{
    internal sealed class ItemToggledUndoEntry : ConfigListUndoEntry
    {
        private sealed class ToggledItemState
        {
            public string Guid { get; }
            public int OriginalIndex { get; }
            public bool OriginalActive { get; }

            public ToggledItemState(IConfigItem item, int originalIndex)
            {
                Guid = item.GUID;
                OriginalIndex = originalIndex;
                OriginalActive = item.Active;
            }
        }

        private readonly ConfigFile _file;
        private readonly int _configIndex;
        private readonly List<ToggledItemState> _items;

        internal ItemToggledUndoEntry(
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
                .Select(item => new ToggledItemState(
                    item,
                    sourceItems.FindIndex(cfg => cfg.GUID == item.GUID)))
                .OrderBy(state => state.OriginalIndex)
                .ToList();
        }

        public override string HistoryAction => "Toggle";

        protected override bool TryUndoCore(ConfigListUndoContext context, out string reason)
        {
            var project = context.Project;

            // Validate the recorded profile.
            if (_configIndex < 0 ||
                _configIndex >= project.ConfigFiles.Count ||
                !ReferenceEquals(project.ConfigFiles[_configIndex], _file))
            {
                reason = "Project/profile mismatch.";
                return false;
            }

            var items = _file.ConfigItems;

            // Every recorded item must still exist.
            bool hasMissingItem =
                _items.Count == 0 ||
                _items.Any(state => !items.Any(item => item.GUID == state.Guid));

            if (hasMissingItem)
            {
                reason =
                    "Toggle restore failed: one or more " +
                    "ConfigItems are missing.";
                return false;
            }

            // Restore each item's original Active value.
            foreach (var state in _items)
            {
                var item = items.Find(item => item.GUID == state.Guid);

                item.Active = state.OriginalActive;
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
