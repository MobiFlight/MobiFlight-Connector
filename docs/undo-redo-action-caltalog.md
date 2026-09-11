# Undo/Redo Action Catalog

## Analysis Dimensions

- View
- Interaction
- State Scope
- Mutation Type
- State Owner
- Persistence
- Restore Data
- Undo Candidate

### View
Start with the views exposed by the current React routing.
Additional native dialogs and application-level views will be added where relevant.

| View | Entry | Character |
|---|---|---|
| Splash / Startup | /, /start | System condition |
| Dashboard - Project | /home | Choose / create a project |
| Dashboard - Community | /home/community | Information / news |
| Config / Profile | /config | Core configuration actions |
| Input Config Editor | /config/:configId | Config item editing dialog within Config view |
| New Project | /project/new modal | Create a new project |
| Edit Project | /project/edit modal | Edit project settings |
| Controller Bindings | /bindings modal | Controller binding configuration |
| Authentication | /auth/... | login/logout |

### User Interaction from each View

#### Dashboard-Project Interactions

| Interaction | State-Changing | Initial Classification |
|---|---|---|
| Create project | Yes | Project mutation |
| Load / select recent project | Working context change | File / project lifecycle |
| Double-click project -> Config view | No | Navigation |
| Save before switching project | Yes | File lifecycle |
| Discard changes before switching | Yes | State replacement |
| Remove item from Recent Projects | Yes | Application preference |

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
| Rename project | Project metadata |
| Edit project settings | Project metadata |

#### Config Item View

| Interaction | Cluster Candidate | Undo relevance |
|---|---|---|
| Search / filter | UI state | No |
| Sort | UI state | No |
| Select row(s) | UI state | No |
| Add Output Config | Create | High |
| Add Input Config | Create | High |
| Edit config | Update | High |
| Rename config | Update field | High |
| Tooggle Active | Update field | High |
| Delete | Delete | Very high |
| Duplicate | Create from existing | Very high |
| Test | Runtime | No |
| Bulk Delete | Batch delete | High |
| Bulk Toggle | Batch update | High |
| Drag reorder | Move / order mutation | Very high |
| Drag between profiles | Move + ownership change | Very high |

## Clustering Criteria
Based on the current code structure, the following six dimensions seem useful for classifying user interactions.

| Criterion | Values Example | Importance |
|---|---|---|
| State scope | UI / Config Item / Profile / Project / Global / Runtime | What to undo |
| Mutation type | Create / Update / Delete / Move / Batch | Different inverse operations |
| State owner | Frontend / Backend / Hybrid | Affects Undo manager location |
| Persistence | Transient / Project - persisted / Application - persisted / Runtime | Undo target decision |
| Required restore data | old value / full object / object + index / multiple objects / snapshot | Command or Snapshot decision |
| Interaction risk | Immediate / shortcut / drag / dialog - confirmed | MVP priority decision |

### Cluster 1 - View / Navigation State
```
Filter
Sort
Select row
Select profile tab
Scroll tabs
Zoom
Navigate Project <-> Config
```

### Cluster 2 - Runtime / External Action
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

### Cluster 3 - Simple Property Update
This could be a useful clusterfor evaluating command-based Undo.
```
Toggle Config.Active
Rename config item
Rename profile
Rename project
```
For example,
```
Before : property = A
Action : property = B
Undo : property = A
```

### Cluster 4 - Create / Delete
```
Add config
Duplicate config
Delete config
Add profile
Remove profile
```
For a Delete action, the following information may additionally be required:
```
Full deleted object
Original container
Original index
```

### Cluster 5 - Move / Reorder
```
Reorder config item
Move config item between profiles
Possibly multi-item drag
```
The current DnD implementation already keeps track of:
```
items
source
target
original positions
```

### Cluster 6 - Compound / Form Edit

Project Settings is a good example for this.
Doing one update, a user can change following in project forms:
```
Project Name
Simulator
FSUIPC
ProSim
Aircraft
```
So, it is similar to following structure:
```
Before ProjectInfo
↓
Multiple Edits
↓
Submit
↓
New ProjectInfo
```
Therefore, this interaction is closer to a compound state change than to a single-property update, which may make it a candidate for a snapshot-based or hybrid Undo approach.

## Actions

| View | Interaction | State Scope | Mutation Type | State Owner | Persistence | Restore Data | Undo Candidate | Source | Notes |
|---|---|---|---|---|---|---|---|---|---|
| Config List | Toggle Active | Config Item | Update | TBD | Project | Previous value | Yes | ConfigItemTableActiveCell.tsx |
| Config List | Rename | Config Item | Update | TBD | Project | Previous name | Yes | ConfigItemTableNameCell.tsx |
| Config List | Delete | Config Item | Delete | TBD | Project | Item + index | Yes | ConfigItemRowContextMenu.tsx |
| Config List | Duplicate | Config Item | Create | TBD | Project | Created item | Yes |
| Config List | Reorder | Config Item | Move | TBD | Project | Original index | Yes | DragDropProvider.tsx | Original position are already tracked during dragging |
| Profile Bar | Select Profile | UI | Selection | Frontend | Transient | - | No |
| Execution | Run / Stop | Runtime | Runtime | Backend | Runtime | - | No |