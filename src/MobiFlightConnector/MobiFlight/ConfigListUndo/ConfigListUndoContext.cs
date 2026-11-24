using MobiFlight.Base;
using MobiFlight.BrowserMessages;
using MobiFlight.BrowserMessages.Outgoing;
using System;
using System.Collections.Generic;

namespace MobiFlight.ConfigListUndo
{
    internal class ConfigListUndoContext
    {
        private readonly ExecutionManager _executionManager;

        internal ConfigListUndoContext(ExecutionManager executionManager)
        {
            _executionManager = executionManager ?? throw new ArgumentNullException(nameof(executionManager));
        }

        public Project Project => _executionManager.Project;

        public int ActiveConfigIndex
        {
            get => _executionManager.ActiveConfigIndex;
            set => _executionManager.ActiveConfigIndex = value;
        }

        public void RefreshExecutionCaches()
        {
            _executionManager.OnInputConfigSettingsChanged(this, null);
        }

        public void PublishProject()
        {
            MessageExchange.Instance.Publish(Project);
        }

        public void PublishConfigItems(int configIndex, List<IConfigItem> items)
        {
            MessageExchange.Instance.Publish(new ConfigValueFullUpdate(configIndex, items));
        }

        public void NotifyConfigChanged(object sender)
        {
            _executionManager.NotifyConfigListUndoChanged(sender);
        }

        public void PublishProfileMoveUndone(int fromIndex, int toIndex)
        {
            MessageExchange.Instance.Publish(
                new ProfileMoveUndone
                {
                    FromIndex = fromIndex,
                    ToIndex = toIndex
                });
        }
    }
}
