# Path Inspection

This Markdown file tracks selected configuration-item interactions through the
application code structure.

The goal is to identify where each interaction starts, how the state is
modified, how the change is synchronized between backend and frontend, and
which information would be required to undo the action.

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