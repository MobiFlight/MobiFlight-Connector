
const historyActionKeys: Record<string, string> = {
    // Config Item
    Add: "Item.Added",
    Duplicate: "Item.Duplicated",
    Delete: "Item.Deleted",
    BulkDelete: "Item.Deleted",
    Toggle: "Item.Toggled",
    Resort: "Item.Moved",
    ItemUpdated: "Item.Edited",
    ItemEdited: "Item.Edited",
    ItemRename: "Item.Renamed",
    rename: "Item.Renamed",
    toggle: "Item.Toggled",

    // Profile
    ProfileAdded: "Profile.Added",
    ProfileRename: "Profile.Rename",
    ProfileDeleted: "Profile.Deleted",
    ProfileMerged: "Profile.Merged",
    ProfileMoved: "Profile.Moved",
}

export const getConfigListHistoryTranslationKey = (
    action: string,
): string => {
    const key = historyActionKeys[action] ?? action

    return `ConfigList.History.${key}`
}
