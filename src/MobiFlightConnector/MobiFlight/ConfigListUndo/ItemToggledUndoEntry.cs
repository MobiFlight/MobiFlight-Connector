using MobiFlight.Base;
using System.Collections.Generic;

namespace MobiFlight.ConfigListUndo
{
    internal sealed class ItemToggledUndoEntry : ItemUpdatedUndoEntry
    {
        internal ItemToggledUndoEntry(
            ConfigFile file,
            int configIndex,
            List<IConfigItem> affectedItems)
            : base(
                file,
                configIndex,
                affectedItems,
                "Toggle")
        {
        }
    }
}
