using MobiFlight.Base;
using System;

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
            if (!IsProfileValid(context.Project, _file, _configIndex))
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
