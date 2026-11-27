using MobiFlight.Base;
using System;
using System.Linq;

namespace MobiFlight.ConfigListUndo
{
    internal sealed class ProfileRenamedUndoEntry : ConfigListUndoEntry
    {
        private readonly ConfigFile _file;
        private readonly int _configIndex;
        private readonly string _originalLabel;

        internal ProfileRenamedUndoEntry(ConfigFile file, int configIndex)
        {
            _file = file ?? throw new ArgumentNullException(nameof(file));

            _configIndex = configIndex;
            _originalLabel = file.Label;
        }

        // Keep the original backend history action name.
        public override string HistoryAction => "ProfileRename";

        protected override bool TryUndoCore(ConfigListUndoContext context, out string reason)
        {
            var project = context.Project;

            int currentIndex = project.ConfigFiles
                .ToList()
                .FindIndex(file => ReferenceEquals(file, _file));

            if (currentIndex != _configIndex)
            {
                reason = "Project/profile mismatch.";
                return false;
            }

            _file.Label = _originalLabel;

            reason = null;
            return true;
        }

        public override void PublishChanges(ConfigListUndoContext context)
        {
            context.PublishProject();

            // The original manager also sent a configuration
            // change notification for ProfileRename.
            context.NotifyConfigChanged(_file.ConfigItems);
        }
    }
}
