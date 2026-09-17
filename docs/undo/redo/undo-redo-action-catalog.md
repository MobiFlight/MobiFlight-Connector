# Undo/Redo Action Catalog

This document provides an initial inventory and classification of user interactions that may be relevant for Undo/Redo support.

The purpose is to identify:
- which application state is affected by each interaction
- how that state is modified
- where the mutation happens
- what information would be required to reverse the operation
- how the change is persisted
- which actions are suitable for the initial Undo MVP

Detailed execution paths for selected actions are documented separately in `path-inspection.md`.

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
| Create project | Creates new project state | Project lifecycle |
| Load / select recent project | Replaces working context | Project lifecycle |
| Double-click project -> Config view | None | Navigation |
| Save before switching project | Persists current project state | File lifecycle |
| Discard changes before switching | Replaces current unsaved state | Project lifecycle |
| Remove item from Recent Projects | Changes application preference | Application preference |
| Edit Project Settings | Changes project metadata/settings | Compound project update |

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
| Search / filter | View / Navigation | No |
| Sort | View / Navigation | No |
| Select row(s) | View / Navigation | No |
| Add Output Config | Create / Delete | High |
| Add Input Config | Create / Delete | High |
| Edit Output Config | Compound / Replace | High |
| Edit Input Config | Compound / Replace | High |
| Rename config | Simple Property Update | High |
| Tooggle Active | Simple Property Update | High |
| Delete | Create / Delete | Very high |
| Duplicate | Create / Delete | Very high |
| Test | Runtime | No |
| Bulk Delete | Multiple Create / Delete | High |
| Bulk Toggle | Multiple Property Update | High |
| Drag reorder | Move / Reorder | Very high |
| Drag between profiles | Move / Reorder | Very high |


### Application Settings

Application Settings represent global state rather than the current project.

Examples include:

```
Language
Logging settings
Execution intervals
Update preferences
Joystcik / MIDI preferences
MobiFlight communication preferences
ProSim connection settings
```
These settings are persisted through `Propterties.Settings.Default`.

The native Settings dialog also exposes hardware-related operations such as firmware updates and module configuration.

Those operations should not automatically be treated as part of the Application Settings Undo action because they affect hardware or external runtime state.

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

`Cardinality` is kept separate from `Mutation Type`.

For example:

```
Toggle Active
Mutation Type: Update
Cardinality: Single

Bulk Toggle
Mutation Type: Update
Cardinality: Multiple

and:

Delete Config Item
Mutation Type: Delete
Cardinality: Single

Bulk Delete
Mutation Type: Delete
Cardinality: Multiple
```

The inspeciton of the bulk actions confirms that these are independent dimenions.

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
Firmware update
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

A typical state transition is:

```
Before : property = A
Action : property = B
Undo : property = A
```

The object continues to exist in the same container. Undo mainly needs the previous value and stable object identification.

This cluster maps naturally to a command-style inverse operation when stable identity is available.

Config Items have GUIDs, which makes them particularly suitable for this pattern.

Profiles currently do not have a dedicated GUID and are primarily addressed by their position in Project.ConfigFiles.

### Cluster 4 - Create / Delete

Examples:

```
Add config
Duplicate config
Delete config
Bulk Delete
Add profile
Remove profile
```

Create and Delete operations change the structure of a collection.

For a Delete action, Undo may required:

```
Deleted object
Original container
Original index
```

For multiple deletion:

```
[
    object + original index,
    object + original index,
    ...
]
```

For a Create operation, Undo may only need to identify the newly created object.

Redo can require additional information if recreating the action would produce a new GUID or different generated state.

This is particularly relevant for:

```
Duplicate Config Item
Add Config Item
Add Profile
```

### Cluster 5 - Move / Reorder

Examples:

```
Reorder config item
Move config item between profiles
Multi-item drag
```

Move operations keep the same objects and identities but change their location.

Undo requires the previous location:

```
Item GUID(s)
Source container
Original index / indices
```

Redo additinally requires the resulting location:

```
Target container
Target insertion index
```

The current DnD implementation already has access during a drag operation to:

```
Dragged items
Source config
Original positions
Target config
Target insertion index
```

However, `originalPositions` are derived from the current table row model.

Because filtering and sorting can change table row positions, those positions must not automatically be assumed to equal the underlying `ConfigItems` collection indices.

Authoritative collection positions should be captured from project state.

### Cluster 6 - Compound / Form Edit

Examples:

```
Edit Input Config Item
Edit Output Config Item
Project Settings
Application Settings
```

These interactions use a temporary editing state and commit several changes together.

A typical transition is:

```
Before state
↓
Temporary edits
↓
Apply / OK / Update
↓
After state
```

The natural Undo boundary is the form submission rather than each individual control change.

### Config Item Editors

Input and Output Config Items use different implementation paths, but both have the same semantic pattern:

```
Before: preivous ConfigItem
After: edited ConfigItem
```

A complete before/after Config Item representation is safer than trying to store every nested property change independently.

### Project Settings

One submission can modify:

```
Name
Simulator
FSUIPC
ProSim
Aircraft
```

The relevant state can therefore be represented as:

```
Before Project Settings
After Project Settings
```

rather than a complete Project snapshot.

Project Settings also trigger the project save path immediatley after the settings are committed.

### Application Settings

Application Settings can modify many global properties in one dialog submission.

They are persisted through:

```
Properties.Settings.Default
```

and some values have additional runtime effects, such as restarting Joystick or MIDI managers or resetting ProSim connection state.

Application Settings therefore raise an additional design question:

```
Whether global application state should participate in the same history as project configuration actions at all.
```

## Inspected Actions

The following actions have already been traced through the current frontend and backend implementation.

### Config Item Actions

| Interaction | State Scope | Mutation | Cardinality | State Owner | Persistence | Required Restore Data | Interaction Risk | Undo Candidate | MVP |
|---|---|---|---|---|---|---|---|---|---|
| Toggle Active | Config Item | Update | Single | Backend, frontend synchronized | Project-persisted | GUID + previous `Active` | Immediate | Yes | Selected |
| Rename Config Item | Config Item | Update | Single | Backend, frontend synchronized | Project-persisted | GUID + previous `Name` | Inline-confirmed | Yes | No |
| Delete Config Item | Config Item | Delete | Single | Backend | Project-persisted | Deleted item + container + original index | Immediate | Yes | Selected |
| Duplicate Config Item | Config Item | Create | Single | Backend | Project-persisted | Created GUID + container; exact Redo also needs created item + index | Immediate + editor follow-up | Yes | Selected |
| Reorder / Drag & Drop | Config Item(s) | Move | Single / Multiple | Hybrid | Project-persisted | GUID(s) + source + original indices; target + new index for Redo | Drag | Yes | Selected |
| Bulk Toggle | Config Item(s) | Update | Multiple | Backend | Project-persisted | GUID + previous `Active` for every affected item | Immediate | Yes | No |
| Bulk Delete | Config Item(s) | Delete | Multiple | Backend | Project-persisted | Deleted items + original indices + container | Immediate | Yes | No |
| Add Config Item | Config Item | Create | Single | Backend | Project-persisted | Created GUID + container; created item for exact Redo | Immediate + editor follow-up | Yes | No |
| Edit Input Config Item | Config Item | Compound / Replace | Single | Frontend draft, backend commit | Project-persisted | GUID + complete previous item; resulting item for Redo | Dialog-confirmed | Yes | No |
| Edit Output Config Item | Config Item | Compound / Replace | Single | WinForms draft, backend commit | Project-persisted | GUID + complete previous item; resulting item for Redo | Dialog-confirmed | Yes | No |

### Config Item Findings

Toggle Active and Rename Config Item use the same general backend replacement pattern.

Delete and Bulk Delete require object restoration rather than property restoration.

Duplicate and Add generate new objects, making exact Redo different from simply executing the original Create operation again.

Reorder is the main inspected Hybrid action because frontend project state is modified during DnD before the backend receives the final move command.

Input and Output editors use different frontend/native implementations but can share the same conceptual Undo model based on complete before/after Config Item state.

### Profile Actions

| Interaction | State Scope | Mutation | Cardinality | State Owner | Persistence | Required Restore Data | Interaction Risk | Undo Candidate | MVP |
|---|---|---|---|---|---|---|---|---|---|
| Select Profile | UI | Selection | Single | Frontend | Transient | - | Navigation | No | No |
| Rename Profile | Profile | Update | Single | Frontend draft, backend commit | Project-persisted | Profile identification + previous label | Inline-confirmed | Yes | Extension |
| Remove Profile | Profile / Project | Delete | Single | Backend | Project-persisted | Removed `ConfigFile` + original index | Immediate | Yes | Extension |
| Add Profile | Profile / Project | Create | Single | Backend | Project-persisted | Created `ConfigFile` + insertion index | Immediate | Yes | Extension |

### Profile Findings

Profile Add and Remove form the same structural Create/Delete pair as Config Item Add and Delete.

However, `ConfigFile` currently has no dedicated GUID or equivalent stable profile identifier.

Current profile operations mainly use numeric collection indices.

This is an important difference from Config Items and should be considered if profile actions are added to Undo/Redo later.

Add and Remove Profile also interact with active-profile state.

It must be decided whether Undo should restore only persisted project structure or also restore the previous user-visible active profile.

## Project and Global Settings

| Interaction | State Scope | Mutation | Cardinality | State Owner | Persistence | Required Restore Data | Interaction Risk | Undo Candidate | MVP |
|---|---|---|---|---|---|---|---|---|---|
| Project Settings | Project | Compound Update | Multiple properties | Frontend draft, backend commit | Project-persisted, normally saved immediately | Before / after Project Settings subset | Dialog-confirmed | Yes | Extension |
| Application Settings | Global / Application | Compound Update | Multiple properties | WinForms draft, backend settings | Application-persisted | Before / after settings subset + required runtime refresh | Dialog-confirmed | Separate history question | Extension |

### Project Settings Findings

Project Settings update the existing backend Project rather than replacing the complete Project object.

The relevant settings subset is:

```
Name
Sim
Features
Aircraft
```

For an existing saved project, submitting Project Settings also immediately invokes the normal project save path.

This creates a policy question for Undo:

```
Restore and save immediately
```

or:

```
Restore in memory and mark the project dirty
```

That decision should be made by the Undo architecture rather than by the individual Project Settings action.

### Application Settings Findings

Application Settings are global and are independent of the currently loaded project.

They are persisted immediately through application user settings.

Some settings also have runtime side effects.

For example:

```
Joystick settings
    → manager shutdown / reconnect

MIDI settings
    → manager shutdown / reconnect

ProSim settings
    → connection state reset

Logging settings
    → live logging configuration refreshed
```

Restoring only the persisted property values would therefore not necessarily restore the complete previous runtime state.

Global settings may also be confusing if mixed into the normal project Undo history.

For example:

```
Delete Config Item
Change application language
Toggle Config Item
```

A project-oriented Ctrl+Z workflow would eventually reach the global language change if both used the same stack.

Whether global state requires a separate history or no Undo support should be decided separately from the project Undo mechanism.

# Inventory-Only / Not Deeply Inspected Actions

The following interactions remain part of the catalog but are not required to be deeply inspected for the current MVP:

| Interaction | Scope | Classification | Current Treatment |
|---|---|---|---|
| Create Project | Project lifecycle | Create / lifecycle | Outside project edit history |
| Load Project | Project lifecycle | Context replacement | Not normal Undo |
| Save / Save As | File lifecycle | Persistence | Not normal Undo |
| Discard Changes | Project lifecycle | State replacement | Separate lifecycle behavior |
| Merge Config File | Profile / Project | Import / Compound | Future extension |
| Rename Project | Project | Simple Property Update | Future extension |
| Toggle AutoRun | Global / Runtime | Preference update | Separate from project Undo |
| Controller Bindings | Project | Compound update | Future extension |
| Remove Recent Project | Application | Preference update | Separate from project Undo |
| Run / Stop / Test | Runtime | Runtime action | Not Undo candidate |
| Hardware / Firmware actions | Hardware / External | External operation | Not project Undo candidate |

The presence of an interaction in this section does not mean that it can never support Undo.

It means that deep inspection or implementation is not necessary for the current project MVP.

# Cross-Action Findings

## 1. User Action Boundary Matters

Undo history should represent semantic user actions rather than every internal property mutation.

Examples:

```
Rename
→ commit on Enter / blur

Input Config Edit
→ commit on Apply

Output Config Edit
→ commit on OK with actual changes

Project Settings
→ commit on Update

Application Settings
→ commit on OK
```

Temporary draft changes and cancelled dialogs should not create history entries.

No-op submissions should preferably not create history entries either.

## 2. Identity Is Important

Config Items have GUIDs and can be identified reliably across many operations.

Profiles do not currently have a dedicated stable identifier.

This makes Config Item actions easier to represent in persistent Undo history than profile actions.

## 3. Restore Data Depends on Mutation Type

The inspection produced recurring restore patterns:

```
Simple Update
→ object identity + previous value

Create
→ identity of created object
→ full created object may be required for exact Redo

Delete
→ deleted object + original container + original index

Move
→ object identity + before location + after location

Compound Edit
→ complete before / after state of the edited unit
```

These patterns can serve as the basis for reusable Undo action representations.

## 4. Side Effects Must Be Reproduced

Restoring serialized values alone is not always sufficient.

Examples include:

```
Input Config changes
→ input event state refresh

Profile structural changes
→ ProjectChanged path and active profile handling

Application Settings
→ Joystick / MIDI / ProSim runtime refresh
```

An Undo implementation must reproduce the relevant synchronization and runtime side effects of the original mutation.

## 5. Persistence Is Not Uniform

Most Config Item and Profile actions behave as:

```
Change state
↓
Mark project dirty
↓
User saves later
```

Project Settings normally behave as:

```
Change state
↓
Mark project dirty
↓
Save immediately
```

Application Settings behave as:

```
Change global state
↓
Save application settings immediately
```

Undo architecture therefore cannot assume that every reversible action uses the same persistence lifecycle.

## 6. Redo Is Not Always Re-execution

For actions such as Duplicate or Add, simply executing the original command again can produce different object identity or generated state.

Exact Redo may therefore need to restore the original resulting object rather than recreate the operation.

# MVP Boundary

The catalog is intentionally broader than the initial implementation scope.

The current MVP focuses on four Config Item actions:

```
Toggle Active
Delete Config Item
Duplicate Config Item
Reorder / Drag & Drop
```

These actions provide representative coverage of several important mutation patterns:

```
Toggle
→ Simple Property Update

Delete
→ Delete

Duplicate
→ Create

Reorder
→ Move
```

This allows the Undo architecture to be evaluated against different state transition types without requiring complete application-wide coverage.

The following remain extensions rather than requirements of the initial MVP:

```
Redo
Bulk actions
Add Config Item
Input / Output compound editing
Profile actions
Project Settings
Application / Global Settings
Further action clusters
```

Inspection of these extension actions is still useful because it verifies that the chosen architecture can potentially be extended beyond the first four supported actions.

# Result

The action catalog shows that MobiFlight does not have one single type of configuration mutation.

The inspected interactions fall into several recurring patterns:

```
View / Navigation
Runtime / External
Simple Property Update
Create / Delete
Move / Reorder
Compound / Form Edit
```

The main architectural requirement is therefore not simply to add an Undo button.

The Undo mechanism must provide:

```
clear action boundaries
stable object identification
appropriate restore data
correct frontend/backend synchronization
required runtime side effects
consistent persistence behavior
```

The selected MVP actions provide a contained set of real application cases with which these requirements can be implemented and evaluated.
