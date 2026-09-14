# Undo/Redo Action Catalog

This document provides an initial inventory and classification of user interactions that may be relevant for Undo/Redo support.

The purpose is to identify which application state is affected by each interaction, how that state is modified, where the mutation happens, and what information would be required to reverse the operation.

## Analysis Dimensions

The interactions are analyzed using the following dimensions:

- View
- Interaction
- State Scope
- Mutation Type
- Cardinality
- State Owner
- Persistence
- Restore Data
- Undo Candidate
- Interaction Risk

### View

The initial inventory starts with the views exposed by the current React routing and includes relevant native dialogs that participate in project or application state changes.

| View | Entry | Character |
|---|---|---|
| Splash / Startup | `/`, `/start` | System condition |
| Dashboard - Project | `/home` | Choose / create a project |
| Dashboard - Community | `/home/community` | Information / news |
| Config / Profile | `/config` | Core configuration actions |
| Input Config Editor | `/config/:configId` | Config item editing dialog within Config view |
| New Project | `/project/new` modal | Create a new project |
| Edit Project | `/project/edit` modal | Edit project settings |
| Controller Bindings | `/bindings` modal | Controller binding configuration |
| Authentication | `/auth/...` | Login/logout |
| Output Config Wizard | Native WinForms modal | Edit Output Config Item |
| Application Settings | Native WinForms dialog | Global / application settings |

### User Interactions by View

#### Dashboard-Project Interactions

| Interaction | State-Changing | Initial Classification |
|---|---|---|
| Create project | Creates new project state | Project mutation |
| Load / select recent project | Replaces working context | File / project lifecycle |
| Double-click project -> Config view | None | Navigation |
| Save before switching project | Persists current project state | File lifecycle |
| Discard changes before switching | Replaces current unsaved state | State replacement |
| Remove item from Recent Projects | Changes application preference | Application preference |

#### Project / Profile Bar Interactions

| Interaction | Cluster Candidate |
|---|---|
| Select profile | Navigation / view state |
| Scroll profile tabs | View - only |
| Add profile | Create |
| Merge config file | Import / compound mutation |
| Rename profile | Update |
| Remove profile | Delete |
| Run / Stop / Test | Runtime action |
| Toggle AutoRun | Application / runtime preference |
| Rename project | Project metadata update |
| Edit project settings | Compound project update |

#### Config Item View

| Interaction | Cluster Candidate | Undo relevance |
|---|---|---|
| Search / filter | UI state | No |
| Sort | UI state | No |
| Select row(s) | UI state | No |
| Add Output Config | Create | High |
| Add Input Config | Create | High |
| Edit config | Update / compound edit | High |
| Rename config | Update field | High |
| Tooggle Active | Update field | High |
| Delete | Delete | Very high |
| Duplicate | Create from existing | Very high |
| Test | Runtime | No |
| Bulk Delete | Multiple delete | High |
| Bulk Toggle | Multiple update | High |
| Drag reorder | Move / order mutation | Very high |
| Drag between profiles | Move + container change | Very high |

## Classification and Prioritization Criteria

Based on the current code structure, the following dimensions seem useful for classifying user interactions.

| Criterion | Values Example | Importance |
|---|---|---|
| State scope | UI / Config Item / Profile / Project / Global / Runtime | Identifies what state is affected |
| Mutation type | Create / Update / Delete / Move / Compound | Determines the type of inverse operation |
| Cardinality | Single / Multiple | Determines whether one or multiple objects must be restored |
| State owner | Frontend / Backend / Hybrid | Helps determine where Undo history could be managed |
| Persistence | Transient / Project - persisted / Application - persisted / Runtime | Determines whether the change belongs in Undo history |
| Required restore data | old value / full object / object + index / multiple objects / snapshot | Helps evaluate Command vs Snapshot approaches |
| Interaction risk | Immediate / shortcut / drag / dialog - confirmed | Helps prioritize actions for the MVP |

## Action Clusters

### Cluster 1 - View / Navigation State

Examples:

```
Filter
Sort
Select row
Select profile tab
Scroll tabs
Zoom
Navigate Project <-> Config
```

These interactions mainly affect the current user intercace and do not normally change persisted project data.

They are therefore not initial Undo candidates.

### Cluster 2 - Runtime / External Action

Examples:

```
Run
Stop
Test
Check update
Download presets
Reinstall WASM
Open website
Copy logs
```

These actions affect runtime state or trigger external operations rather than editing project configuration.

They are therefore not initial project Undo candidates.

### Cluster 3 - Simple Property Update

Examples:

```
Toggle Config.Active
Rename config item
Rename profile
Rename project
```

This could be a useful clusterfor evaluating command-based Undo.

A typical state transition is:

```
Before : property = A
Action : property = B
Undo : property = A
```

The object continues to exist in the same container. Undo mainly needs the previous value, or alternatively a previous object snapshot.

### Cluster 4 - Create / Delete

Examples:

```
Add config
Duplicate config
Delete config
Add profile
Remove profile
```

Create and Delete operations change the structure of a collection.

For a Delete action, Undo may required:

```
Full deleted object
Original container
Original index
```

For a Create operation, Undo may only need to identify the newly created object.

Redo can require additional information if recreating the action would produce a different identify or state.

### Cluster 5 - Move / Reorder

Examples:

```
Reorder config item
Move config item between profiles
Multi-item drag
```

The current DnD implementation already has access during a drag operation to:

```
Dragged items
Source config
Original positions in the current table row model
Target config
Calculated insertion index
```

The frontend also already contains logic for restoring dragged items to their original positions when a drag operation is cancelled.

However, these values are currently transient drag state and are not stored as Undo history.

Additionally, the stored `originalPositions` are derived from the current table row model. Before they can be reused directly for Undo, it must be verified that these indices always correspond to the underlying `ConfigItems` collection indicies, especially when filtering or sorting is active.

### Cluster 6 - Compound / Form Edit

Project Settings is a good example of a compound state change.

A single form submission can modify several properties:

```
Project Name
Simulator
FSUIPC
ProSim
Aircraft
```

The resulting operation is closer to:

```
Before ProjectInfo
↓
Multiple Edits
↓
Submit
↓
After ProjectInfo
```

than to a single-property update.

This may make compound form edits a useful candidate for comparing snapshot-based and command-based approaches.

## Inspected Actions

The following actions have already been traced through the current frontend and backend implementation.

| View | Interaction | State Scope | Mutation Type | Cardinality | State Owner | Persistence | Restore Data | Undo Candidate | Source | Notes |
|---|---|---|---|---|---|---|---|---|---|---|
| Config List | Toggle Active | Config Item | Update | Single | Backend (frontend synchronized) | Project-persisted | GUID + previous Active value | Yes | ConfigItemTableActiveCell.tsx → CommandUpdateConfigItem → ExecutionManager.HandleCommandUpdateConfigItem | Uses ConfigValuePartialUpdate |
| Config List | Rename | Config Item | Update | Single | Backend (frontend synchronized) | Project-persisted | GUID + Previous name / previous item snapshot | Yes | ConfigItemTableNameCell.tsx → CommandUpdateConfigItem → ExecutionManager.HandleCommandUpdateConfigItem | Same backend update path as Toggle Active |
| Config List | Delete | Config Item | Delete | Single | Backend (frontend synchronized) | Project - persisted | Deleted item + original config + original index | Yes | ConfigItemRowContextMenu.tsx → CommandConfigContextMenu → ExecutionManager | Uses ConfigValueFullUpdate |
| Config List | Duplicate | Config Item | Create | Single | Backend (frontend synchronized) | Project - persisted | Created duplicate GUID + containing config | Yes | ConfigItemRowContextMenu.tsx → CommandConfigContextMenu → ExecutionManager | Exact Redo my additionally require duplicated item + insertion position |
| Config List | Reorder / Drag & Drop | Config Item(s) | Move | Single / Multiple | Hybrid | Project-persisted | Item GUID(s) + original config + original index/indices | Yes | ConfigItemTableActiveCell.tsx / DnD row → DragDropProvider → DnD utilities → CommandResortConfigItem → ExecutionManager | Target config/index also needed for Redo; existing drag positions must be validated against underlying collection indices |
| Profile Bar | Select Profile | UI | Selection | Single | Frontend | Transient | - | No | - | View-state only |
| Execution | Run / Stop | Runtime | Runtime | - | Backend | Runtime | - | No | - | Not a project configuration mutation |

## Next Inspection Candidates

The next useful interations to inspect are:

```
Bulk Toggle
Bulk Delete
Add Config Item
Edit Config Item
Rename Profile
Remove Profile
Project Settings
Application Settings
```

Bulk Toggle and Bulk Delete are especially useful next because they can verify whether `Cardinality` shoud remain a separate classification dimension from `Mutation Type`.