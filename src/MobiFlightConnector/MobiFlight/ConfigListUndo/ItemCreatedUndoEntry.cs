using MobiFlight.Base;
using System;

namespace MobiFlight.ConfigListUndo
{
    internal sealed class ItemCreatedUndoEntry : ConfigListUndoEntry
    {
        private readonly ConfigFile _file;
        private readonly int _configIndex;
        private readonly string _guid;
        private readonly string _historyAction;

        internal ItemCreatedUndoEntry(
            Project project,
            ConfigFile file,
            int configIndex,
            string guid,
            bool isDuplicate)
            : base(project)
        {
            _file = file ?? throw new ArgumentNullException(nameof(file));

            _configIndex = configIndex;

            _guid = guid ?? throw new ArgumentNullException(nameof(guid));

            _historyAction = isDuplicate ? "Duplicate" : "Add";
        }

        public override string HistoryAction => _historyAction;

        internal bool MatchesCreation(
            Project project,
            ConfigFile file,
            int configIndex,
            string guid)
        {
            return ReferenceEquals(RecordedProject, project)
                && ReferenceEquals(_file, file)
                && _configIndex == configIndex
                && _guid == guid;
        }

        protected override bool TryUndoCore(ConfigListUndoContext context, out string reason)
        {
            var project = context.Project;
            // The original profile must still exist
            // at its recorded index.
            if (_configIndex < 0 ||
                _configIndex >= project.ConfigFiles.Count ||
                !ReferenceEquals(project.ConfigFiles[_configIndex], _file))
            {
                reason = "Project/profile mismatch.";
                return false;
            }

            var items = _file.ConfigItems;

            // Find the created item by GUID.
            // Never remove the original item of a duplication.
            int itemIndex = items.FindIndex(item => item.GUID == _guid);

            if (itemIndex < 0)
            {
                reason = $"Created ConfigItem not found: {_guid}";
                return false;
            }

            // All validation has passed.
            items.RemoveAt(itemIndex);

            reason = null;
            return true;
        }

        public override void PublishChanges(ConfigListUndoContext context)
        {
            context.PublishConfigItems( _configIndex, _file.ConfigItems);

            context.NotifyConfigChanged(_file.ConfigItems);
        }
    }
}
