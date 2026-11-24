using MobiFlight.Base;
using System;

namespace MobiFlight.ConfigListUndo
{
    internal sealed class ItemUpdatedUndoEntry : ConfigListUndoEntry
    {
        private readonly ConfigFile _file;
        private readonly int _configIndex;
        private readonly string _guid;
        private readonly IConfigItem _snapshot;
        private readonly string _changeAnnotation;

        internal ItemUpdatedUndoEntry(
            Project project,
            ConfigFile file,
            int configIndex,
            IConfigItem originalItem,
            string changeAnnotation = null)
            : base(project)
        {
            _file = file ?? throw new ArgumentNullException(nameof(file));

            if (originalItem == null)
                throw new ArgumentNullException(nameof(originalItem));

            _configIndex = configIndex;
            _guid = originalItem.GUID;
            _snapshot = (IConfigItem)originalItem.Clone();
            _changeAnnotation = changeAnnotation;
        }

        public override string HistoryAction => _changeAnnotation ?? "ItemEdited";

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

            // Find the item by its original GUID.
            int itemIndex = items.FindIndex(item => item.GUID == _guid);

            if (itemIndex < 0 || _snapshot == null)
            {
                reason = $"ConfigItem not found or snapshot missing: {_guid}";
                return false;
            }

            // Restore the previous Config Item state.
            items[itemIndex] = (IConfigItem)_snapshot.Clone();

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
