using MobiFlight.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.ConfigListUndo
{
    internal class ItemUpdatedUndoEntry : ConfigListUndoEntry
    {
        private sealed class UpdatedItemState
        {
            public string Guid { get; }
            public IConfigItem Snapshot { get; }
            public UpdatedItemState(IConfigItem item)
            {
                Guid = item.GUID;
                Snapshot = (IConfigItem)item.Clone();
            }
        }
        private readonly ConfigFile _file;
        private readonly int _configIndex;
        private readonly List<UpdatedItemState> _items;
        private readonly string _changeAnnotation;

        internal ItemUpdatedUndoEntry(
            Project project,
            ConfigFile file,
            int configIndex,
            IConfigItem originalItem,
            string changeAnnotation = null)
            : this(
                  project,
                  file,
                  configIndex,
                  new[] 
                {
                    originalItem ?? throw new ArgumentNullException(nameof(originalItem))
                }, 
                  changeAnnotation)
        {
        }
        protected ItemUpdatedUndoEntry(
            Project project,
            ConfigFile file,
            int configIndex,
            IEnumerable<IConfigItem> originalItems,
            string changeAnnotation = null)
            : base(project)
        {
            _file = file ?? throw new ArgumentNullException(nameof(file));

            if (originalItems == null) throw new ArgumentNullException(nameof(originalItems));

            _configIndex = configIndex;
            _changeAnnotation = changeAnnotation;

            _items = originalItems
                .Select(item => new UpdatedItemState(item))
                .ToList();
        }

        internal static bool ShouldMergeWithCreation(
            Project project,
            ConfigFile file,
            int configIndex,
            IConfigItem originalItem,
            string change,
            IConfigListUndoEntry lastEntry)
        {
            bool hasChangeAnnotation = change != null;

            if (hasChangeAnnotation)
            {
                return false;
            }

            if (lastEntry is not ItemCreatedUndoEntry created)
            {
                return false;
            }

            return created.IsCreatedItem(project, file, configIndex, originalItem.GUID);
        }

        public override string HistoryAction =>
            _changeAnnotation ?? "ItemEdited";

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

            if (_items.Count == 0)
            {
                reason = GetRestoreFailureReason(null);
                return false;
            }

            var itemIndices = new List<int>();

            // Validate all affected items before restoring any.
            foreach (var state in _items)
            {
                int itemIndex = items.FindIndex(item => item.GUID == state.Guid);

                if (itemIndex < 0 || state.Snapshot == null)
                {
                    reason = GetRestoreFailureReason(state.Guid);
                    return false;
                }

                itemIndices.Add(itemIndex);
            }

            // Restore each affected item.
            for (int i = 0; i < _items.Count; i++)
            {
                RestoreItem(items, itemIndices[i], _items[i].Snapshot);
            }

            reason = null;
            return true;
        }

        protected virtual void RestoreItem(List<IConfigItem> items, int itemIndex, IConfigItem snapshot)
        {
            items[itemIndex] = (IConfigItem)snapshot.Clone();
        }

        protected virtual string GetRestoreFailureReason(string guid)
        {
            return $"ConfigItem not found or snapshot missing: {guid}";
        }

        public override void PublishChanges(ConfigListUndoContext context)
        {
            context.PublishConfigItems(_configIndex, _file.ConfigItems);

            context.NotifyConfigChanged(_file.ConfigItems);
        }
    }
}