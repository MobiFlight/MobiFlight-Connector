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

## Edit Input Config Item

### Flow

When the user opens an Input Config Item for editing, `ConfigItemTableActionsCell` navigates to `/config/{GUID}`.

`InputConfigDialog` retrieves the corresponding config item from the frontend project stroe by its GUID.

The dialog keeps two local veersions of the config item:

- `draftConfigItem`
- `committtedConfigItem`

Changes made through `ConfigWizard` or the inline name editor modify only the draft config item.

While the user is editing the dialog, the backend project state is not modified.

If the user cancels the dialog, the dialog is closed without publishing an update command and the draft changes are discarded.

When the user applies the changes, `InputConfigDialog` first calls `resetTriggersBasedOnDeviceType`.

Depending on the selected input device type, trigger configurations that are not applicable to the selected device are removed from the final config item.

The complete resulting config item is then sent to the backend through `CommandUpdateConfigItem`.

`ExecutionManager` receives the command through `MessageExchange` and passes the item to `HandleCommandUpdateConfigItem`.

The handler searches the current `ConfigItems` collection for the existing item by GUID and replaces the complete item at the same index.

After the backend state has been updated, `ExecutionManager` publishes a `ConfigValuePartialUpdate` containing the modified config item.

The frontend receives the partial update and synchronizes the corresponding item in the project store.

`ExecutionManager` then calls `OnInputConfigSettingsChanged`, which rebuilds the input-event related state, and invokes `OnConfigHasChanged`.

The change is therefore marked as an unsaved project change. It is written to disk only when the user explicitly saves the project.

### State before

- The Input Config Item exists in `ConfigItems`
- The item has a specific GUID
- The item contains the previously committed input configuration
- The item has a specific position in the collection

### State after

- The same Input Config Item still exists
- The GUID remains unchanged
- The position in the collection remains unchanged
- One or more properties of the complete config item may have changed
- Trigger configurations incompatible with the selected device type may have been removed

### Required for Undo

Because several properties and nested configuration objects can be changed in one editing session, restoring only individual property values would not be sufficient in the general case.

Undo should therefore preserce:

- Config item GUID
- Previous complete `InputConfigItem`

The previous item can then replace the edited item at the same position.

### State owner

- Temporary edit state is owned by `InputConfigDialog` in the frontend
- The committed project state is owned by the backend
- The committed state is synchronized back to the frontend through `ConfigValuePartialUpdate`

### Persistence

- Part of the project state
- Persisted to disk after an explicit save

### Cluster

- Compound Edit / Replace Config Item

### Observation

Edit Input Config Item differs from Toggle Active and Rename because on user action may modify several properties and nested configuration objects at once.

The dialog acts as a transaction boundary.

Changes made while the dialog is open are only temporary draft state. The project state changes only when the user presses Apply Changes.

Therefore, one successful Apply operation should normally correspond to one Undo/Redo history entry, regardless of how many individual fields were modified inside the dialog.

From the Undo perspective, storing the complete previous `InputConfigItem` is safer than trying to reconstruct every changed property individually.

Undo and Redo also have to reproduce the same input configuration side effect as the original operation.

In particular, restoring or reapplying an Input Config Item must ensure that the input-event state is refreshed in the same way as `OnInputConfigSettingsChanged`.

For Redo, the complete resulting `InputConfigItem` after the original Apply operation can be preserved together with the previous item.

This produces a natural before/after representation:

```
Before: previous InputConfigItem
After: edited InputConfigItem
```

Cancel does not modify project state and should therefore not create an Undo/Redo history entry.

## Edit Output Config Item

### Flow

When the user opens an Output Config Item for editing, `ConfigItemTableActionsCell` publishes a `CommandConfigContextMenu` message with the action `"edit"` and the selected config item.

`MainForm` receives the command through `MessageExchange`.

For an Output Config Item, `MainForm` calls `OpenOutputConfigWizardForId` with the item's GUID.

The method retrieves the corresponding item from `ExecutionManager.ConfigItems` and opens the existing WinForms `ConfigWizard`.

When the wiard is initialized, it creates a clone of the original `OutputConfigItem`.

The user therefore edits the cloned config item instead of directly modifying the item stored in the backend project state.

The original config item is retained so that `ConfigWizard` can determine whether the configuration has changed.

If the user cancels the wizard, the cloned configuration is discarded and the backend project state remains unchanged.

If the user confirms the wizard with OK, the wizard synchronizes the current form values into the cloned config item.

`MainForm` then checks `ConfigHasChanged`.

If the config has changed, it searches `ExecutionManager.ConfigItems` for the original item by GUID.

The existing item is replaced at the same index with the edited `wizard.Config`.

`MainForm` then publishes a `ConfigValuePartialUpdate` containing the modified Output Config Item.

The frontend receives the partial update and synchronizes the corresponding item in the project store.

Finally, `MainForm` calls `OnConfigItemHasChanged`, which marks the project as containing unsaved changes.

The change is written to disk only when the user explicitly saves the project.

### State before

- The Output Config Item exists in `ConfigItems`
- The item has a specific GUID
- The item contains the previously committed output configuration
- The item has a specific position in the collection

### State after

- The same Output Config Item still exists
- The GUID remains unchanged
- The position in the collection remains unchanged
- One or more properties or nested configuration objects may have changed
- The original item has been replaced by the edited clone

### Required for Undo

Because the Output Config Wizard can modify several properties and nested configuration objects in a single editing session, Undo should preserve:

- Config item GUID
- Previous complete `OutputConfigItem`

The previous item can then replace the edited item at the same position.

### State owner

- The committed project state is owned by the backend
- Temporary edit state is held by the cloned `OutputConfigItem` inside the WinForms `ConfigWizard`
- The resulting committed state is synchronized to the frontend through `ConfigValuePartialUpdate`

### Persistence

- Part of the project state
- Persisted to disk after an explicit save

### Cluster

- Compound Edit / Replace Config Item

### Observation

Edit Output Config Item has the same high-level state transition as Edit Input Config Item:

```
Before: previous ConfigItem
After: edited ConfigItem
```

In both cases, the existing config item keeps its GUID and collection position but its complete configuration may be replaced.

However, the two actions currently use different editing and commit paths.

Input Config Items are edited in the React frontend and committed through `CommandUpdateConfigItem`.

Output Config Items are edited through the WinForms `ConfigWizard`, and `MainForm` directly replaces the item in `ExecutionManager.ConfigItems`.

The temporary edit models are also different.

For Input Config Items, the dialog maintains a frontend draft.

For Out Config Items, `ConfigWizard` creates and modifies a cloned `OutputConfigItem`.

Despite these implementation differences, both actions can potentially use the same Undo/Redo concept: replacing one complete config item state with another.

A history entry could therefore preserve both versions:

```
Before: previous ConfigItem
After: edited ConfigItem
```

Undo restores `Before`.

Redo restores `After`.

The implementation must still preserve the action-specific side effects and synchronization behavior of the original edit path.

Cancel, or confirming the Output Config Wizard without an actual change, does not modify project state and should therefore not create an Undo/Redo history entry.

## Rename Profile

### Flow

When the user renames a profile, the interaction starts from the `InlineEditLabel` inside `ProfileTab`.

Rename mode can be entered either by double-clicking the profile label or by selecting Rename from `ProfileTabContextMenu`.

The context menu itself does not modify the profile. It only calls `startEditing()` on the `InlineEditLabel`.

While the user is editing the name, the new value is stored only in the local `tempValue` state of `InlineEditLabel`.

The project state is therefore not modified while the user is typing.

If the user presses Escape, the temporary value is reset to the current label and no command is published.

The rename is confirmed when the input loses focus or the user presses Enter.

`InlineEditLabel` only calls its `onSave` callback if the entered value differs from teh current value.

`ProfileTab.onSave` first updates `optimisticLabel` so that the new name is immediately visible in the frontend.

It then publishes a `CommandFileContextMenu` message containing:

- the action `"rename"`
- the profile index
- a copy of the `ConfigFile` with the new `Label`

`ExexutionManager` receives the command through `MessageExchange`.

The handler retrieves the corresponding `ConfigFile` from `Project.ConfigFiles` using the supplied index.

For the rename action, the backend changes only the `Label` property:

```
file.Label = message. File.Label;
```

The `ConfigFile` remains in the same collection and its contained config items are not modified.

After the backend state has been updated, `ExecutionManager` publishes the complete `Project`.

The frontend receives the `Project` message through `useBackendStateAppMessages` and replaces the current project state in the project store.

`ExecutionManager` then invokes `OnConfigHasChanged`.

The change is therefore marked as an unsaved project change. It is written to disk only when the user explicitly saves the project.

### State before

- The profile exists in `Project.ConfigFiles`
- The profile has a specific position in the collection
- `ConfigFile.Label = old label`

### State after

- The same profile still exists in `Project.ConfigFiles`
- Its position in the collection remains unchanged
- Its contained config items remain unchanged
- `ConfigFile.Label = new label`

### Required for Undo

- Profile identification
- Previous `Label`

With the current implementation, the profile is addressed by its index in `Project.ConfigFiles`.

For Redo, the new `Label` also has to be preserved.

### State owner

- Temporary edit state is owned by `InlineEditLabel` in the frontend
- `ProfileTab` temporarily displays the committed name through `optimisticLabel`
- The committed project state is owned by the backend
- The frontend is synchronized by publishing the complete `Project`

### Persistence

- Part of the project state
- Persisted to disk after an explicit save

### Cluster

- Simple Property Update

### Observation

Rename Profile has the same general mutation pattern as Rename Config Item:

```
Before: Label = old value
After: Label = new value
```

Only one property of an existing object changes.

The profile itself remains in the same `Project.ConfigFiles` collection and its contained configuration is not modified.

The editing phase is temporary frontend state and should not create Undo history entries.

A history entry should only be created when the rename is successfully committed with a value different from the current label.

Pressing Escape or confirming the unchanged label does not modify project state and should therefore not create an Undo/Redo history entry.

The current command identifies the profile using its collection index.

This is sufficient for the immediate rename operation, but using only the stored index for a later Undo may require additional consideration.

If profiles can be added, removed, or reordered between the original action and its reversal, the same index may no longer refer to the same profile.

Before using the index as persistent Undo restore data, the profile identity model should therefore be inspected together with Add Profile and Remove Profile.

For an exact Redo, the resulting new label should be preserved in addition to the previous label.

## Remove Profile

### Flow

When the user selects Remove from the profile tab context menu, `ProfileTabContextMenu` publishes a `CommandFileContextMenu` message containing:

- the action `"remove"`
- the profile index
- the corresponding `ConfigFile`

Unlike Rename Profile, there is no temporary editing phase before the command is published.

`ExecutionManager` receives the command through `MessageExchange`.

The handler first validates the supplied profile index and retrieves the corresponding `ConfigFile` from `Project.ConfigFiles`.

For the remove action, the profile is removed from the project collection using:

```
Project.ConfigFiles.RemoveAt(message.Index);
```

This removes the complete `ConfigFile`, including all config items contained inside it.

`Project.ConfigFiles` is an `ObservableCollection<ConfigFile>`.

Removing the profile therefore raises the collection change event in `Project`, which invokes `OnProjectChanged`.

`ExecutionManager` forwards this event through its `OnProjectChanged` event.

Its project-change handler then:

- resets `ActiveConfigIndex` to `0`
- rebuilds the input event executors through `InitInputEventExecutor`
- publishes the updated `Project`

After the remove branch completes, the `CommandFileContextMenu` handler also publishes the current `Project` and invokes `OnConfigHasChanged`.

The frontend receives the project update through `useBackendStateAppMessages` and replaces the project stored in the frontend project store.

`ProjectPanel` then checks whether the current frontend `activeConfigFileIndex` is still valid.

If the previous active index is outside the bounds of the remaining profile collection, it is changed to the last available profile index.

Changes to the frontend active profile index are subsequently sent back to the backend through `CommandActiveConfigFile`.

The project is marked as containing unsaved changes and is persisted to disk only when the user explicitly saves the project.

### State before

- The profile exists in `Project.ConfigFiles`
- The profile has a specific position in the collection
- The profile contains a complete `ConfigFile`
- The `ConfigFile` may contain multiple Input and Output Config Items
- A profile is currently selected through the active profile index

### State after

- The removed `ConfigFile` no longer exists in `Project.ConfigFiles`
- All config items belonging to the removed profile disappear with it
- Profiles after the removed profile shift to a lower collection index
- The backend active config index is reset as part of the project-change path
- The frontend active profile index may also be adjusted if its previous value is no longer valid

### Rquired for Undo

Undo has to restore the complete removed profile and its original position.

Therefore, the minimum restore data is:

- Removed `ConfigFile`
- Original index in `Project.ConfigFiles`.

Conceptually, Undo can restore the profile by inserting the stored `ConfigFile` at its previous index.

The previous active profile state may also need to be preserved if Undo is expected to restore the exact user-visible state in addition to the persisted project structure.

### State owner

- The profile collection is owned by the backend project state
- The frontend receives the updated complete `Project`
- The active profile state exists in both frontend and backend and is synchronized through `CommandActiveConfigFile`

### Persistence

- Part of the project state
- Persisted to disk after an explicit save

### Cluster

- Create / Delete

### Observation

Remove Profile changes the structure of `Project.ConfigFiles` rather than a property of an existing profile.

The action therefore has the same general inverse pattern as deleting a config item:

```
Before:
[A, B, C]

Remove B

After:
[A, C]
```

Undo must restore both the removed object and its original position:

```
Removed object: B
Original index: 1
```

Restoring only the profile context without restoring its original index would not reproduce the exact previous project structure.

The removed `ConfigFile` itself contains all of its config items, so preserving the complete `ConfigFile` also preserves the configuration contained in the profile.

`ConfigFile` currently has no dedicated GUID or other stable profile identity. Profiles are primarily addressd by their position in `Project.ConfigFiles`.

For Delete Undo this is less problematic because the complete removed object can be stored together with its original index.

However, profile identity and index stability should still be considered when designing Redo or when combining profile actions with other structural project changes.

Remove Profile also has important side effects beyond the collection removal.

Because `Project.ConfigFiles` is observable, `RemoveAt` triggers the project change path.

This currently causes `ExecutionManager` to reset `ActiveConfigIndex`, reinitialize the input event executors, and publish the project.

The `CommandFileContextMenu` handler then also publishes the project after the remove action.

An Undo implementation should therefore not treat profile restoration as only as isolated collection insertion.

It should also ensure that the same project-related runtime state is refreshed appropriately after the profile is restored.

The active profile introduces an additional question for the Undo model.

For example:

```
Before:

Profile A
Profile B <- active
Profile C

Remove Profile B
```

Undoing the removal could either:

- restore Profile B without changing the currently active profile
- or restore Profile B and also restore the previously active profile state

The first option restores only persisted project data.

The second option restores the complete user-visible state of the action.

This distinction should be decided consistently when the Undo/Redo history model is designed.

For Redo, removing only by the previously stored numeric index may not always be sufficient if the profile collection has changed since the original action.

In a standard linear Undo/Redo model this risk is reduced because performing a new action after Undo normally clears the Redo history, but the assumption should still be documented.

## Add Profile

### Flow

When the user selects Add new profile from `AddProfileTabMenu`, `ProjectPanel` executes `addConfigFile`.

`addConfigFile` publishes a `CommandAddConfigFile` message containing:

- the type `"create"`
- the default profile label

The frontend does not create the `ConfigFile` itself.

`MainForm` receives `CommandAddConfigFile` through `MessageExchange`.

For the `create` type, `MainForm` calls `AddNewFileToProject`.

`AddNewFileToProject` first stops the current execution through `execManager.Stop`.

It then creates a new profile using `CreateDefaultConfigFile`.

The default profile is created as a new `ConfigFile` with:

```
Label = "New file"
EmbedContent = true
```

If a label was provided by the frontend, the default label is replaced with the supplied value.

The new `ConfigFile` is then appended to the end of `Project.ConfigFile`s.

`Project.ConfigFiles `is an `ObservableCollection<ConfigFile>`.

Adding the new profile therefore raises the collection change event in `Project`, which invokes `OnProjectChanged`.

`ExecutionManager` forwards the project change and its project-change handler:

- resets `ActiveConfigIndex` to `0`
- rebuilds the input event executors through `InitInputEventExecutor`
- publishes the updated `Project`

After the new profile has been added, `AddNewFileToProject` calls `ProjectOrConfigFileHasChanged`, which marks the project as containing unsaved changes.

It also invokes `ProjectLoaded`.

The existing `ProjectLoaded` handler stops execution and publishes the project to the frontend.

The frontend receives the updated `Project` and replaces the current project state in the frontend project store.

After sending the create command, `ProjectPanel` also schedules the newly created profile to become the active profile.

Because the profile is appended to the collection, the previous `ConfigFiles.length` corresponds to the index of the new profile.

The frontend therefore updates `activeConfigFileIndex` to that index.

The active profile change is then sent back to the backend through `CommandActiveConfigFile`.

The project state is persisted to disk only when the user explicitly saves the project.

### State before

- The project contains an existing `Project.ConfigFiles` collection
- The collection contains a specific number of profiles
- A specific profile is currently active

### State after

- A new `ConfigFile` exists at the end of `Project.ConfigFiles`
- The new profile has the supplied default label
- `EmbedContent` is enabled on the new profile
- The new profile initially contains no config items
- The project contains one additional profile
- The newly created profile becomes the active profile
- The project is marked as containing unsaved changes

### Required for Undo

Undo has to remove the profile created by the Add Profile action.

The required restore data should therefore include:

- Crated `ConfigFile`
- Insertion index

Because the current implementation always appends the new profile, the insertion index is the previous size of `Project.ConfigFiles`.

The previous active profile/index may also need to be preserved if Undo is expected to restore the exact user-visible state.

### State owner

- The created profile is owned by the backend project state
- The frontend receives the updated complete `Project`
- The active profile state exists in both frontend and backend and is synchronized through `CommandActiveConfigFile`

### Persistence

- Par of the project state
- Persisted to disk after an explicit save

### Cluster

- Create / Delete

### Observation

Add Profile changes the structure of `Project.ConfigFiles` by creating and appending a new `ConfigFile`.

For example:

```
Before:
[A, B] 

Add Profile

After:
[A, B, C]
```

The inverse operation is therefore structurally similar to Remove Profile.

Undo can remove the `ConfigFile` that was created by the original action.

Unlike Config Items, `ConfigFile` currently has no dedicated GUID or other stable profile identifier.

This means that an Undo implementation should not assume that a numeric index alone is always sufficient to identify the originally created profile.

Preserving the created `ConfigFile` together with its insertion index provides a clearer representation of the original action.

For an exact Redo, recreating the action by calling `AddNewFileToProject` again is not necessarily equivalent to restoring the original result.

Calling the create path again constructs a new `ConfigFile`.

A more deterministic Redo can instead preserve the created profile state and restore the same state at the original insertion position.

Add Profile also has side effects beyond the collection mutation.

Because `Project.ConfigFiles` is observable, adding the profile triggers the project change path.

This resets the backend `ActiveConfigIndex`, rebuilds the input event executors, and publishes the project.

The frontend then explicitly selects the newly created profile and sends that selection back through `CommandActiveConfigFile`.

Execution is also stopped as part of the Add Profile path.

These runtime and selection effects are separate from the persisted structural change but should be considered when implementing the inverse operation.

The active profile raises the same Undo policy question as Remove Profile.

For example:

```
Before:
A <- active
B

Add C

After:
A
B
C <- active
```

Undo could either:

- remove C while leaving the currently selected profile unchanged where possible
- or remove C and restore the profile that was active before the Add action

The first option restores only the persisted project structure.

The second option restores the complete user-visible state.

The same policy should be applied consistently to both Add Profile and Remove Profile.

## Project Settings

### Flow

Project Settings can be opened from either the project card on the dashboard or the project menu in the config view.

Both entry points use `useProjectModal` with `mode`: `"edit"` and navigate to the `/project/edit` modal route.

`ProjectFormModal` opens `ProjectForm` using the current `ProjectInfo`.

The form initializes local frontend state for:

- Project name
- Simulator
- FSUIPC feature
- ProSim feature
- Aircraft selection
- 
Changes made inside the form modify only this temporary React state.

The backend project is not modified while the user is editing the form.

Changing the simulator also resets related temporary state.

The selected simulator can reset the FSUIPC and ProSim options and replaces the current aircraft selection with the default aircraft list for the newly selected simulator.

Aircraft selection is also maintained only as local form state until the form is submitted.

If the user presses Cancel, the dialog closes without publishing a backend mutation command.

The change is committed when the user presses Update or submits the form using Enter.

Before submitting, `ProjectForm` validates and trims the project name and normalizes the feature settings based on the selected simulator.

The final submitted settings contain:

```
Name
Sim
Features
    FSUIPC
    ProSim
Aircraft
```

`ProjectFormModal` then publishes a `CommandMainMenu` message with:

```
action = "project.edit"
options.project = submitted settings
```

`CommandMainMenuHandler` receives the command and forwards it to `MainForm.updateProjectSettings`.

The backend does not replace the complete `Project` object.

Instead, it updates the existing project properties individually:

```
execManager.Project.Name = project.Name;
execManager.Project.Sim = project.Sim;
execManager.Project.Features = project.Features;
execManager.Project.Aircraft = project.Aircraft;
```

These property changes can trigger the project-change path.

`ExecutionManager` reacts to project changes by resetting the active config index, rebuilding the input event executors, and publishing the updated project.

`MainForm` also reacts to project changes through `ExecManager_OnProjectChanged`, which calls `ProjectOrConfigFileHasChanged`.

This temporarily marks the project as containing unsaved changes and refreshes related project-dependent state.

After all project settings have been assigned, `updateProjectSettings` immediately calls `saveToolStripButton_Click`.

For an already saved project, this writes the updated project to disk through `SaveConfig`.

After the save completes, the project is published again and the unsaved project state is reset.

If the project does not yet have a file path, the normal Save As flow is triggered instead.

### State before

The project contains a previous Project Settings state:

```
Name = previous name
Sim = previous simulator
Features.FSUIPC = previous value
Features.ProSim = previous value
Aircraft = previous aircraft list
```

Other project state such as:

```
ConfigFiles
ControllerBindings
FilePath
```

is not directly modified by this form submission.

### State after

The same backend `Project` object still exists.

The following properties contain the submitted values:

```
Name = new name
Sim = new simulator
Features.FSUIPC = new value
Features.ProSim = new value
Aircraft = new aircraft list
```

The project's config files remain unchanged.

For an already persisted project, the updated settings are also written to disk immediately as part of the same user interaction.

### Required for Undo

Undo should restore the complete previous Project Settings state.

The required restore data is:

- Previous `Name`
- Previous `Sim`
- Previous `Features`
  + `FSUIPC`
  + `ProSim`
- Previous `Aircraft`

For Redo, the resulting settings should also be preserved:

- New `Name`
- New `Sim`
- New `Features`
- New `Aircraft`

The complete `Project` does not necessarily need to be stored because the
action modifies only the Project Settings subset.

A conceptual history entry could therefore preserve:

```
Before Project Settings
After Project Settings
```

rather than a complete project snapshot.

### State owner

- Temporary form state is owned by the React frontend
- The committed Project Settings state is owned by the backend `Project`
- The frontend is synchronized by publishing the updated `Project`
- Disk persistence is handled by the backend save path

### Persistence

- Project-persisted
- For an existing saved project, the form submit normally triggers an immediate project save
- For a project without a file path, the Save As flow is triggered
- If Save As is cancelled, the settings can remain changed in memory while the project remains unsaved

### Cluster

- Compound / Form Edit

### Observation

Project Settings is a compound edit rather than a simple property update.

A single user-confirmed action can modify several related project properties:

```
Name
Simulator
FSUIPC
ProSim
Aircraft
```

The semantic operation is therefore closer to:

```
Before Project Settings
↓
Multiple temporary edits
↓
Update
↓
After Project Settings

```

than to several independent property changes.

For Undo history purposes, one successful form submission should therefore be treated as one history entry.

Individual changes made while the form is open should not create separate history entries.

Cancel should not create an Undo entry because no backend mutation has occurred.

Changing the simulator has additional compound effects inside the form.

Selecting another simulator resets feature options and also replaces the temporary aircraft selection with simulator-specific defaults.

The final submitted feature state is also normalized before the backend command is sent.

This means Undo should restore the actual committed previous settings rather than attempting to reverse individual UI interactions that occurred inside the form.

Project Settings also differs from most previously inspected configuration actions because submitting the form immediately triggers the project save path.

The effective flow for an existing saved project is:

```
Edit Settings
↓
Commit backend state
↓
Project becomes dirty
↓
Save project immediately
↓
Project becomes clean again

```

This creates an important Undo design question.

If Undo restores only the previous settings in memory, the disk state may still contain the settings produced by the original Project Settings action.

An implementation must therefore decide whether Undo should:

```
A. Restore the previous settings and immediately persist them

or

B. Restore the previous settings in memory and leave the project dirty until the user saves manually
```

The first option mirrors the current Project Settings persistence behavior.

The second option is closer to the behavior of other configuration Undo operations.

This policy should be decided at the architecture level rather than inside the individual Project Settings action.

The current form also does not appear to perform a semantic comparison between the original settings and the submitted settings before publishing `project.edit`.

Therefore, pressing Update without making a meaningful change can still follow the backend update and save path.

Undo history should preferably create an entry only if the resulting committed Project Settings state differs from the previous state.

Project Settings is therefore a useful example for evaluating a snapshot-style restore model, because multiple related properties can be captured and restored as one contained settings state.

## Application(Global) Settings

### Flow

Application Settings can be opened from the React main menu through:

```
Extras
↓
Settings
```

The frontend publishes a `CommandMainMenu` message containing:

```
action = "extras.settings"
```

`CommandMainMenuHandler` forwards the command to `MainForm.settingsToolStripMenuItem_Click`.

`MainForm` then opens the native WinForms `SettingsDialog`.

The settings dialog loads the current application settings into its individual settings panels.

The relevant settings panels include:

```
Generla
MobiFlight
Joystick / MIDI
ProSim
```

The settings are primarily loaded from:

```
Properties.Settings.Default
```

The values displayed in the WinForms controls therefore represent temporary dialog state while the user is editing the settings.

Changing controls inside the dialog does not normally persist the corresponding application settings immediately.

The General panel contains application-wide preferences such as:

```
RecentFilesMaxCound
TestTimerInterval
PollInterval
LogEnabled
LogLevel
LogJoystickAxis
Language
BetaUpdates
CommunityFeedback
AutoRetrigger
MinimizeOnAutoRun
HupHopAutoCheck
```

The MobiFlight panel also contains application-level preferences such as:

```
FwAutoUpdateCheck
IgnoreComPorts
IgnoredComPortsList
```

The Joystick / MIDI panel contains settings such as:

```
EnableJoystickSupport
EnableMidiSupport
ExcludedJoysticks
ExcludedMidiBoards
```

The ProSim panel contains:

```
ProSimHost
ProSimPort
ProSimAutoConnectEnabled
ProSimMaxRetryAttempts
```

If the user pressess Cancel, the dialog closes without calling the common `saveSettings` path.

If the user presses OK, `SettingsDialog.okButton_Click` validates the dialog and calls `saveSettings`.

`SettingsDialog.saveSettings` then delegates the save operation to the individual panels:

```
generalPanel.saveSettings();
mobiFlightPanel.SaveSettings();
peripheralsPanel.SaveSettings();
proSimPanel.SaveSettings();
```

Each panel updates its corresponding values in:

```
Properties.Settings.Default
```

The dialog then performs additional runtime updates.

For example, the Joystick / MIDI settings can restart the corresponding device managers when support or exclusion settings change.

The ProSim connection state is also reset after the settings are saved.

Finally, the dialog persists the complete updated application settings using:

```
Properties.Settings.Default.Save();
```

`MainForm` listens to the application settings save event.

When the settings are saved, it reapplies logging-related configuration and publishes the updated application settings to the frontend.

The settings are therefore application-wide and are not stored as part of the current project file.

### State before

The application contains a previous global settings state stored through `Properties.Settings.Default`.

The relevant state can include values such as:

```
General preferences
Logging preferences
Language
Update preferences
Runtime preferences
MobiFlight communication preferences
Joystick / MIDI preferences
ProsSim connection preferences
```

The current project configuration does not own these values.

### State after

The selected application settings contain the values confirmed by the user in the Settings dialog.

The updated values are stored in `Properties.Settings.Default`.

They are also persisted through:

```
Properties.Settings.Default.Save();
```

Some settings can additionally change live runtime state.

For example:

```
Joystick manager may restart
MIDI manager may restart
ProSim connection state is reset
Logging settings are reapplied
Frontend application settings are republished
```

The current project's persisted configuration remains unchanged.

### Required for Undo

Undo would have to restore the previous values of the application settings modified by the dialog.

A useful restore representation would therefore contain:

```
Before Application Settings
After Application Settings
```

The required state my include:

- General settings values
- MobiFlight application preference values
- Joystick / MIDI support and exclusion values
- ProSim connection settings

The complete application settings object does not necessarily need to be stored if the history entry records only the subset that can be modified by the Settings dialog.

Restoring only persisted property values may not be sufficient ofr all settings.

Settings that have runtime effects may also require their corresponding runtime state to be refreshed.

For example, restoring Joystick or MIDI settings may require reconnection or shutting down the respective managers.

### State owner

- Temporary settings state is owned by the WinForms Settings dialog controls
- Committed settings are owned by `Properties.Settings.Default`
- Runtime managers consume some of the committed settings
- The React frontend receives synchronized application settings from the backend

### Persistence

- Application-persisted
- Not part of the current project file
- Persisted when the user confirms the dialog with OK
- `Properties.Settings.Default.Save()` writes the settings immediately
- Cancel does not use the common application settings save path

### Cluster

- Compound / Form Edit

### Observation

Application Settings represents global application state rather than project configuration state.

For the Action Catalog, Application Settings and Global Settings therefore describe the same state scope.

A single Settings dialog confirmation can modify many unrelated application properties across several tabs.

The semantic action is therefore:

```
Before Application Settings
↓
Multiple temporary edits
↓
OK
↓
After Application Settings
```

rather than a collection of independent property updates.

For Undo history purposes, one successful OK confirmation would therefore be the natural action boundary.

Individual control changes inside the dialog should not create separate Undo entries.

Cancel should not create an Undo entry because the common settings persistence path has not been executed.

Application Settings differs from Project Settings because the state is stored outside the project.

The state belongs to:

```
Properties.Settings.Default
```

and affects the application independently of which project is currently open.

The settings are also immediately persisted when the user confirms the dialog.

The effective flow is:

```
Edit Application Settings
↓
Commit Properties.Settings values
↓
Apply runtime side effects
↓
Save application settings
↓
Publish updated settings
```

This creates an important boundary question for Undo/Redo.

Application Settings may not belong in the same history stack as project configuration actions.

For example:

```
Rename Config Item
Delete Config Item
Change Application Language
Toggle Config Item
```

If all actions shared one history stack, Undo could eventually attempt to retore an application preference while the user is primarily undoing project configuration changes.

This behavior may not match user expectations.

Application Settings should therefore remain in the Action Catalog because it is a state-changing user interaction, but whether it belongs in the same Undo history as project configuration actions should be decided separately.

Application Settings also has runtime side effects.

Restoring only a previous persisted value can leave runtime managers in a state that no longer matches the restored preference.

For example:

```
EnableJoystickSupport = true
↓
user disables support
↓
JoystickMgr.Shutdown()
```

An Undo that restores only:

```
EnableJoystickSupport = true
```

without reconnecting the joystick manage would not restore the actual previous application state.

The inverse operation must therefore consider both:

```
Persisted setting state
+
Runtime effect
```

for settings that directly affect runtime services.

The MobiFlight tab also contains hardware and module operations such as firmware updates, serial regeneration, device configuration, and uploads.

These operations should not automatically be considered part of the Application Settings Undo action.

They are better classified separately as runtime, hardware, or external operations.

The Application Settings action should therefore refer only to the global preferences that are loaded from and saved to `Properties.Settings.Default`.

Because one dialog can modify many unrelated global properties at once, this action is another useful example of a snapshot-style restore model.

However, compared with Project Settings, it is a weaker candidate for the initial project Undo MVP because it belongs to global application state and can also produce runtime side effects outside the project configuration model.