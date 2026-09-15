# Path Inspection

This document tracks selected configuration-item interactions through the
application code structure.

The goal is to identify where each interaction starts, how the state is
modified, how the change is synchronized between backend and frontend,
which information would be required to undo the action, and which additinal information may be required for Redo

## Toggle Active

### Flow

When the user toggles the Active switch, `ConfigItemTableActiveCell` creates
a copy of the current config item with the `Active` value inverted.

The updated config item is sent to the backend through
`CommandUpdateConfigItem`.

`ExecutionManager` receives the command through `MessageExchange` and passes
the updated item to `HandleCommandUpdateConfigItem`.

The handler finds the corresponding config item by GUID and replaces the
existing item in `ConfigItems`.

After the backend state has been updated, `ExecutionManager` publishes a
`ConfigValuePartialUpdate` containing the modified config item.

`ConfigListPage` receives the update and synchronizes the corresponding item
in the frontend project store.

`ExecutionManager` then calls `OnInputConfigSettingsChanged`, which clears the
input-event caches, and invokes `OnConfigHasChanged`.

The change is therefore marked as an unsaved project change. It is written to
disk only when the user explicitly saves the project.

### State before

- `ConfigItem.Active = old value`

### State after

- `ConfigItem.Active = !old value`

### Required for Undo

- Config item GUID
- Previous `Active` value

Alternatively, the previous `IConfigItem` could be stored.

### State owner

- Backend project state
- Synchronized to the frontend through `ConfigValuePartialUpdate`

### Persistence

- Part of the project state
- Persisted to disk after an explicit save

### Cluster

- Simple Property Update

### Observation

Only one property of an existing config item changes.

The config item itself remains in the same collection and at the same
position, so Undo only needs to restore the previous property value or the
previous item state.


## Rename Config Item

### Flow

When the user starts renaming a config item, `InlineEditLabel` keeps the
edited name temporarily in frontend component state.

While the user is typing, the project state is not modified.

When the edit is confirmed, `ConfigItemTableNameCell` creates a copy of the
current config item with the new `Name`.

The updated config item is sent to the backend through
`CommandUpdateConfigItem`.

`ExecutionManager` receives the command through `MessageExchange` and passes
the updated item to `HandleCommandUpdateConfigItem`.

The handler finds the corresponding config item by GUID and replaces the
existing item in `ConfigItems`.

After the backend state has been updated, `ExecutionManager` publishes a
`ConfigValuePartialUpdate` containing the modified config item.

`ConfigListPage` receives the update and synchronizes the corresponding item
in the frontend project store.

`ExecutionManager` then calls `OnInputConfigSettingsChanged`, which clears the
input-event caches, and invokes `OnConfigHasChanged`.

The change is therefore marked as an unsaved project change. It is written to
disk only when the user explicitly saves the project.

### State before

- `ConfigItem.Name = old name`

### State after

- `ConfigItem.Name = new name`

### Required for Undo

- Config item GUID
- Previous `Name`

Alternatively, the previous `IConfigItem` could be stored.

### State owner

- Backend project state
- Synchronized to the frontend through `ConfigValuePartialUpdate`

### Persistence

- Part of the project state
- Persisted to disk after an explicit save

### Cluster

- Simple Property Update

### Observation

Rename and Toggle Active use the same backend command and the same update path.

The main difference is that Rename has a temporary frontend editing phase
before the change is committed.

From the Undo perspective, both actions can be treated as changes to a
property of an existing config item.


## Delete Config Item

### Flow

When the user selects Delete from the config item context menu,
`ConfigItemRowContextMenu` sends the selected config item to the backend
through `CommandConfigContextMenu` with the action `"delete"`.

`ExecutionManager` receives the command through `MessageExchange` and handles
the `"delete"` action inside the `CommandConfigContextMenu` subscription.

The backend removes the config item from the current `ConfigItems` collection
by matching its GUID.

At this point, the config item no longer exists in the backend project state.

`ExecutionManager` then calls `OnInputConfigSettingsChanged`, which clears the
input-event caches.

After the collection has been modified, `ExecutionManager` publishes a
`ConfigValueFullUpdate` containing the complete updated `ConfigItems` list.

`ConfigListPage` receives the full update and replaces the corresponding
config item list in the frontend project store.

Finally, `ExecutionManager` invokes `OnConfigHasChanged`.

The change is therefore marked as an unsaved project change. It is written to
disk only when the user explicitly saves the project.

### State before

- The config item exists in `ConfigItems`
- The config item belongs to a specific config file
- The config item has a specific position in the collection

### State after

- The config item no longer exists in `ConfigItems`

### Required for Undo

- Deleted `IConfigItem`
- Original config file
- Original index

### State owner

- Backend project state
- Synchronized to the frontend through `ConfigValueFullUpdate`

### Persistence

- Part of the project state
- Persisted to disk after an explicit save

### Cluster

- Create / Delete

### Observation

Delete differs from Toggle Active and Rename because it changes the structure
of the collection instead of only changing a property of an existing item.

Undo therefore has to restore both the deleted object and its original
location.

Simply adding the item back to the end of the collection would not restore
the exact previous state.

## Duplicate Config Item

### Flow

When the user selects Duplicate from the config item context menu, `ConfigItemRowContextMenu` publishes a `CommandConfigContextMenu` message with the action `duplicate` and the current config item as its payload.

`ExecutionManager` receives the command through `MessageExchange` and handles the `duplicate` action inside the `CommandConfigContextMenu` subscription.

The backend searches the current `ConfigItems` collection for the original config item by comparing its GUID with `message.Item.GUID`.

If no matching item is found, the operation stops.

If the item exists, `ExecutionManager` calls `Duplicate()` on the original config item.

`Duplicate()` creates a copy of the config item and assigns a new GUID to the duplicated item.

The duplicated item is then inserted directly after the original item using the original item's index plus one.

At this point, the backend project state contains both the original config item and the newly created duplicate.

After the collection has been modified, `ExecutionManager` publishes a `ConfigValueFullUpdate` containing the complete updated `ConfigItems` list.

`ConfigListPage` receives the full update and replaces the corresponding config item list in the frontend project store.

Finally, `ExecutionManager` invokes `OnConfigHasChanged`.

The change is therefore marked as an unsaved project change. It is written to disk only when the user explicitly saves the project.

### State before

- The original config item exists in `ConfigItems`
- The original config item has a specific GUID
- The original config item has a specific position in the collection

### State after

- The original config item remains unchanged
- A duplicated config item exists directly after the original item
- The duplicated item contains copied configuration data
- The duplicated item has a newly generated GUID

### Required for Undo

- GUID of the created duplicate
- Config file / container containing the duplicate

Undo can remove the newly created item by identifying it through its generated GUID.

### State owner

- Backend project state
- Synchronized to the frontend through `ConfigValueFullUpdate`

### Persistence

- Part of the project state
- Persisted to disk after an explicit save

### Cluster

- Create / Delete

### Observation

Duplicate differs from Toggle Active and Rename because it creates a new object instead of modifying an existing object.

For Undo only, removing the newly created config item is sufficient.

However, Redo introduces an additional consideration.

Calling `Duplicate()` again during Redo would create another new GUID and may duplicate the current state of the source item rather than restoring the exact resul of the original Duplicate action.

Therefore, an exact Redo may need to preserve:

- The duplicated `IConfigItem`
- Its generated GUID
- Its original config file / container
- Its insertion index

This would allow Redo to restore the same duplicated item at the same position instead of executing a new duplication operation.

There is also action-boundary question.

After Duplicate completes, the application automatically opens the newly created config item for editing.

For the Undo/Redo history model it must be decided whether:

- Duplicate is one history entry and the following edit is another entry
- or Duplicate and the following edit should be grouped into one user action

A first implementation should preferably treat Duplicate and the subsequent Edit as seperate history actions unless there is a strong reason to group them.

## Reorder Config Item

### Flow

When the user drags a config item using the drag handle, `ConfigItemTableActiveCell` registers the item as sortable using its GUID.

During the drag operation, the frontend keeps track of the dragged config items and calculates the target position when the item is dropped.

When the drop is completed, the frontend first moves the dragged item or items inside the frontend project store.

It then publishes a `CommandResortConfigItem` message containing:

- the moved `IConfigItem` items
- the new insertion index
- the source config file index
- the target config file index

`ExecutionManager` receives the command through `MessageExchange` and handles it inside the `CommandResortConfigItem` subscription.

The backend retrieves the `ConfigItems` collection of the source config file.

For each moved item, it searches the corresponding backend config item by GUID.

The matching items are collected and removed from the source `ConfigItems` collection.

The backend then retrieves the target config file and inserts the same config items into its `ConfigItems` collection starting at the requested new index.

The config items themselves are not duplicated and their GUIDs do not change. Only their location in the project structure changes.

After the move has been applied, `ExecutionManager` publishes a `ConfigValueFullUpdate` for the source config file and another `ConfigValueFullUpdate` for the target config file.

The frontend receives these updates and synchronizes the corresponding config item lists with the backend state.

Finally, `ExecutionManager` invokes `OnConfigHasChanged`.

The change is therefore marked as an unsaved project change. It is written to disk only when the user explicitly saves the project.

### State before

- The moved config item or items exist in a specific source config file
- Each moved item has a specific GUID
- Each moved item has a specific position in the source collection

### State after

- The same config item or items still exist with the same GUIDs
- The item or items are located at a new position
- The target config file may be the same as or different from the source config file

### Required for Undo

- Moved config item GUID(s)
- Original config file
- Original index / indices

### State owner

- Hybrid

The frontend project store is already modified during the DnD interaction.

The backend project store is updated through `CommandResortConfigItem`, after which `ConfigValueFullUpdate` messages synchronize the final state back to the frontend.

### Persistence

- Part of the project state
- Persisted to disk after an explicit save

### Cluster

- Move / Reorder

### Observation

Reorder differs from property updates and create/delete actions because the config item itself is not modified or recreated.

Instead, the action changes the location of an existing item.

Undo therefore has to restore the original location of each moved item rather than restore a previous property value or object.

For Redo, the target location also has to be known:

- Target config file
- Target insertion index

This means that a Reorder action needs to preserve both the before-location and the after-location of the moved item or items.

For example:

Before:

Config 0:

A
B
C
D

Move C to index 1.

After:

A
C
B
D

Undo requires the original position of C:

- Config file: 0
- Index: 2

Redo requires the new position:

- Config file: 0
- Index: 1

The same principle also applies when an item is moved between different config files.

For multiple selected items, the original position of each item may have to be stored separately.

### Existing DnD Restore Data

The current frontend DnD implementation already records:

```
draggedItems
source config
originalPositions
current config
target config
target insertion index
```

This makes Reorder particularly interesting for an Undo/Redo prototype because some inverse-operation data already exists during the user interaction.

However, the current `originalPositions` map is derived from the table's current row model.

Because the table supports sorting and filtering, these row indices may not always be identical to the indices in the underlying project's `ConfigItems` collection.

Before this data is reused as persistent Undo history, this relationship must be verified.

If the row-model positions do not always correspond to the project collection positions, the original project indices should instead be captured directly from the underlying `ConfigItems` list.

## Bulk Toggle Config Items

### Flow

When the user triggers Bulk Toggle, `ConfigItemTable` collects all currently
selected rows and extracts their underlying `IConfigItem` objects.

If no config items are selected, the operation stops without publishing a
command.

If one or more items are selected, the frontend publishes a
`CommandConfigBulkAction` message containing:

- the action `"toggle"`
- the selected config items

`ExecutionManager` receives the command through `MessageExchange` and handles
it inside the `CommandConfigBulkAction` subscription.

For a `"toggle"` action, the backend first determines the target Active state
from the first selected item:

```
toggleValue = !firstSelectedItem.Active
```

The backend then iterates over all selected items.

For each item, it searches the current backend `ConfigItems` collection for
the corresponding config item by GUID.

If a matching config item exists, its `Active` property is set to the same
`toggleValue`.

This means that Bulk Toggle does not independently invert the Active value
of every selected item.

Instead, all selected items receive the same final Active state determined by
the first selected item.

For example:

```
State before:

A = true
B = false
C = true
```

If `A` is the first selected item:

```
toggleValue = false
```

The resulting state is:

```
A = false
B = false
C = false
```

After all matching config items have been updated, `ExecutionManager`
publishes a `ConfigValueFullUpdate`.

The message contains:

- the current `ActiveConfigIndex`
- the complete current `ConfigItems` collection

The frontend receives the full update and synchronizes the corresponding
config item list with the backend state.

Finally, `ExecutionManager` invokes `OnConfigHasChanged`.

The project is therefore marked as containing unsaved changes and is written
to disk only when the user explicitly saves it.

### State before

- Multiple selected config items exist in the current config file
- Each selected item has its own GUID
- Each selected item may have its own `Active` value

For example:

```
A = true
B = false
C = true
```

### State after

- The same config items still exist
- Their GUIDs remain unchanged
- All selected items have the same `Active` value
- The final value is the inverse of the first selected item's previous `Active` value

For example:

```
A = false
B = false
C = false
```

### Required for Undo

Undo has to restore the previous `Active` value of each affected config item.

Therefore, the required restore data is not only one previous Active value.

It has to preserve the previous value for every affected item, for example:

```
[
    { GUID: A, Active: true },
    { GUID: B, Active: false },
    { GUID: C, Active: true }
]
```

### State owner

- Backend project state
- Synchronized to the frontend through `ConfigValueFullUpdate`

### Persistence

- Part of the project state
- Persisted to disk after an explicit save

### Cluster

- Simple Property Update
- Multiple-item mutation

### Observation

Bulk Toggle is structurally similar to Toggle Active because it changes the
same `Active` property on existing config items.

However, its cardinality is different:

```
Toggle Active
Mutation Type: Update
Cardinality: Single

Bulk Toggle
Mutation Type: Update
Cardinality: Multiple
```

This supports treating `Cardinality` as a separate classification dimension
instead of using `Batch` as a mutation type.

Bulk Toggle also introduces an important Undo requirement.

Because selected items may have different Active states before the operation,
Undo cannot restore the previous state using only one Boolean value.

The previous `Active` value has to be recorded separately for every affected
item.

For Redo, the final target value can be stored directly, or the resulting
after-state of each affected item can be preserved.

Another implementation detail is that Bulk Toggle does not call
`OnInputConfigSettingsChanged`, while Bulk Delete does.

This difference should be kept in mind when comparing the side effects of the
two bulk actions.

## Bulk Delete Config Items

### Flow

When the user triggers Bulk Delete, `ConfigItemTable` collects all currently selected rows and extracts their underlying `IConfigItem` objects.

If no config items are selected, the operation stops without publishing a command.

If one ore more items are selected, the frontend publishes a `CommandConfigBulkAction` message containing:

- the action `"delete"`
- the selected config items

After publishing the command, the frontend clears the current row selection.

`ExecutionManager` receives the command through `MessageExchange` and handles it inside the `CommandConfigBulkAction` subscription.

For a `"delete"` action, the backend iterates over all items contained in `message.Items`.

For each item, it searches the current backend `ConfigItems` collection for the corresponding config item by GUID.

If a matching config item is found, that item is removed from the `ConfigItems` collection.

After all selected items have been removed, `ExecutionManager` calls `OnInputConfigSettingsChanged`.

This updates the input configuration state and clears cached input-event bindings so that deleted config items are no longer referenced by the input execution logic.

After the collection has been modified, `ExecutionManager` publishes a `ConfigValueFullUpdate` containing:

- the current `ActiveConfigIndex`
- the complete remaining `ConfigItems` collection

The frontend receives the full update and synchronizes the corresponding config item list with the backend state.

Finally, `ExecutionManager` invokes `OnConfigHasChanged`.

The project is therefore marked as containing unsaved changes and is written to disk only when the user explicitly saves it.

### State before

- Multiple selected config items exist in the current config file
- Each selected item has its own GUID
- Each selected item has a specific position in the underlying `ConfigItems` collection

For example:

```
index 0: A
index 1: B
index 2: C
index 3: D
index 4: E
```

If `B` and `D` are selected for deletion, both items still exist before the operation.

### State after

- All selected config items have been removed from `ConfigItmes`
- Non-selected items remain in the collection
- The remaining items shift to file the removed positions

For example:

```
A
C
E
```

### Required for Undo

Undo has to restore every deleted config item and return it to its original position in the collection.

Therefore, the restore data has to preserve the full deleted item together with its original collection index.

For example:

```
[
    {
        item: B,
        originalIndex: 1
    },
    {
        item: D,
        originalIndex: 3
    }
]
```

The original config file / container also has to be known.

### State owner

- Backend project state
- Synchronized to the frontend through `ConfigValueFullUpdate`

### Persistence

- Part of the project state
- Persisted to disk after an explicit save

### Cluster

- Create / Delete
- Multiple-item mutation

### Observation

Bulk Delete is structurally similar to Delete Config Item because both remove existing objects from the same backend collection.

The main difference is cardinality:

```
Delete Config Item
Mutation Type: Delete
Cardinality: Single

Bulk Delete
Mutation Type: Delete
Cardianlity: Multiple
```

This further supports treating `Cardinality` as a separate classification dimension instead of using `Batch` as a mutation type.

Bulk Delete also requires more restore data than Bulk Toggle.

Bulk Toggle can restore the previous property values of existing objects.

Bulk Delete has to reconstruct removed objects and restore their original positions in the collection.

The selected items are collected in the frontend from the table's selected row model.

However, the row order in the current table may be affected by sorting or filtering.

Therefore, the frontend table position should not automatically be treated as the authoritative project collection index.

For Undo history, the original index of each deleted item should preferably be captured from the backend `ConfigItems` collection by GUID before the items are removed.

Another difference from Bulk Toggle is that Bulk Delete calls `OnInputConfigSettingsChanged` after the removal.

This is required so that cached input-event bindings no longer reference the deleted config items.

The actual project change notification still happens through `OnConfigHasChanged` after the full update is published.

## Add Config Item

### Flow

The user can add either an Output Config Item or an Input Config Item from the Config Item Table.

The frontend uses on of two handlers:

- `handleAddOutputConfig`
- `handleAddInputConfig`

Both handlers set:

```
lastAction.current = "add"
```

and publish a `CommandAddConfigItem` message.

The payload contains:

- a default name for the new config item
- the config type

For an Output Config Item:

```
type = "OutputConfig"
```

For an Input Config Item:

```
type = "InputConfig"
```

`ExecutionManager` receives the message through the `CommandAddConfigItem` subscription.

A new config item instance is then created.

For an Output Config Item, an `OutputConfigItem` is created and its `Source` is initialized according to the current project's simulator.

For an Input Config Item, an `InputConfigItem` is created.

The backend then initializes the common properties of the new item:

```
GUID = newly generated GUID
Name = name from the message
Active = true
```

The new item is appended to the current backend `ConfigItems` collection.

After the item has been added, `ExecutionManager` publishes a `ConfigValueFullUpdate` containing:

- the current `ActiveConfigIndex`
- the complete current `ConfigItems` collection

The frontend receives the full update and synchronizes its config item list with the backend state.

Finally, `ExecutionManager` invokes `OnConfigHasChanged`.

This causes the project to be marked as containing unsaved changes.

After the frontend receives the updated config item list, it detects the newly created item by comparing the new GUIDs with the previous list.

The new item is then selected and its editor is opened.

For an Input Config Item, the frontend navigates to:

```
/config/{newItem.GUID}
```

For an Output Config Item, the frontend publishes a `CommandConfigContextMenu` message with:

```
action = "edit"
```

which opens the correspondinng Output Config editor.

### State before

- The current config file contains an existing `ConfigItems` collection
- The new config item does not yet exist
- No GUID has been assigned to the new item

For example:

```
A
B
C
```

### State after

- One new config item has been appended to `ConfigItems`
- The new item has its own generated GUID
- The new item has the default name supplied by the frontend
- The new item is active
- An Output Config Item also contains an initialized `Source`

For example:

```
A
B
C
NewItem
```

### Required for Undo

Undo has to remove the config item that was created by the Add operation.

The most important restore information is:

```
created item GUID
config file / container
```

Because the item is appended to the current `ConfigItems` collection, Undo can identify and remove the created item by GUID.

For Redo, recreating the exact previous state may require preserving the created item itself, including its generated GUID and initialized properties, instead of creating a completely new item with a different GUID.

### State owner

- Backend project state
- Synchronized to the frontend through `ConfigValueFullUpdate`

### Persistence

- Part of the project state
- Persisted to disk after an explicit save

### Cluster

- Create / Delete
- Single-item mutation

### Observation

Add Config Item is tthe inverse structural case of Delete Config Item.

Delete removes an existing object from the backend collection, while Add creates a new object and appends it to the collection.

The generated GUID is especially important for Undo/Redo.

For Undo, the GUID can identify the exact object that has to be removed.

For exact Redo, generating a completely new config item would also generate a new GUID.

Therefore, an Undo/Redo implementation may need to preserve the originally created object if the exact previous state should be restored.

The Add operation also has a frontend follow-up step that is separate from the backend state mutation.

After the new item has been synchronized back to the frontend, the frontend automatically selects the item and opens the corresponding editor.

This editor-opening behavior is UI/navigation state, while the creation of the config item itself is project state.