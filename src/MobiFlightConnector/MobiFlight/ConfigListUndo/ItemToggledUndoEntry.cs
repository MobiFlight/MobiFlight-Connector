using MobiFlight.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.ConfigListUndo
{
    internal sealed class ItemToggledUndoEntry : ItemUpdatedUndoEntry
    {
        internal ItemToggledUndoEntry(
            Project project,
            ConfigFile file,
            int configIndex,
            List<IConfigItem> affectedItems)
            : base(
                project,
                file,
                configIndex,
                affectedItems,
                "Toggle")
        {
        }

        protected override void RestoreItem(List<IConfigItem> items, int itemIndex, IConfigItem snapshot)
        {
            items[itemIndex].Active = snapshot.Active;
        }

        protected override string GetRestoreFailureReason(string guid)
        {
            return "Toggle restore failed: one or more ConfigItems are missing.";
        }
    }
}
