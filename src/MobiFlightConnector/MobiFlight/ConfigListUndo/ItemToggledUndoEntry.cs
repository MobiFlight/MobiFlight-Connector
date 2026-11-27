using MobiFlight.Base;
using System.Collections.Generic;

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
    }
}
