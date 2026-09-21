# Splash Screen

The Splash Screen is shown while the startup sequence is running. Currently, the app shows gold-tier sponsors' logos at the bottom.

The logos are clickable and lead to their websites with external browser, once clicked. That is only user-interactable action, which should not implement Undo/Redo mechanism for sure.

# Dashboard

The Dashboard is the first main application view shown after the startup sequence has completed.

The Dashboard contains both the Project area and the Community area.

On larger window sizes both areas can be visible at the same time.

On smaller window sizes the user can switch between the Project and Community views using the Dashboard navigation buttons.

The Dashboard is wrapped by the common application UI, including the main menu, theme and user controls, and the optional Log panel.

This catalog records interactions from the user's point of view.

The presence of an interaction in this catalog does not imply that it should be handled by the project Undo/Redo mechanism.

But it is still thinkable, that the Undo/Redo mechanism could be used when the application window is small and a user can change the view by clicking navigation button.

## Dashboard Navigation

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Project navigation button | Click | Shows the Project area | Navigation / View State | Normally not expected in project Undo history |
| Community navigation button | Click | Shows the Community area | Navigation / View State | Normally not expected in project Undo history |
| Project area scrollable content | Scroll | Changes visible content | View State | No application Undo expected |
| Community feed | Scroll | Changes visible feed content | View State | No application Undo expected |

The Project / Community navigation buttons are only visible on smaller window sizes.

On larger window sizes both areas can be visible at the same time: Project navigation on left side and Community navigation on right side.

# Main Menu Toolbar

The Main Menu Toolbar is displayed above the Dashboard and provides application-wide actions. Dashboard and Project view, to be exact.

## File Menu

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| File menu | Click | Opens File menu | Menu / Navigation | No Undo |
| New | Click / Ctrl+N | Opens Create Project dialog | Project Lifecycle | Dialog itself is not an Undo action |
| Open | Click / Ctrl+O | Opens project selection flow | Project Lifecycle | Replaces working context rather than normal editing Undo |
| Save | Click / Ctrl+S | Saves current project | Persistence | Not normally undone |
| Save As | Click / Ctrl+Shift+S | Opens Save As flow | Persistence | Not normally undone |
| Recent Projects | Open submenu | Shows recent project entries  | Menu / Navigation | No Undo |
| Recent Project entry | Click | Loads selected recent project | Project Lifecycle | May require unsaved-change confirmation |
| Exit | Click / Ctrl+Q | Exits application | Application Lifecycle | No Undo |

Keyboard shortcuts exposed by the application include:

```
Ctrl+N       New Project
Ctrl+O       Open
Ctrl+S       Save
Ctrl+Shift+S Save As
Ctrl+Q       Exit
```

## View Menu

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| View menu | Click | Opens View menu | Menu / Navigation  | No Undo |
| Reset Zoom | Click / Ctrl+0 | Restores default application zoom | View State | Usually not part of project Undo |
| Zoom In | Click / Ctrl++ / Ctrl+= | Increases application zoom | View State | Could conceptually be reversible, but not project Undo |
| Zoom Out | Click / Ctrl+- | Decreases application zoom | View State | Could conceptually be reversible, but not project Undo |
| Show Log | Click | Opens Log panel | View State | Normally no project Undo |
| Hide Log | Click | Hides Log panel | View State | Normally no project Undo |

Keyboard shortcuts include:

```
Ctrl+0  Reset Zoom
Ctrl++  Zoom In
Ctrl+=  Zoom In (alternative)
Ctrl+-  Zoom Out
```

## Extras Menu

| GUI Element | User Interaction | Visible Result | Interaction | Undo Expectation / Notes |
|---|---|---|---|---|
| Extras menu | Click | Opens Extras menu | Menu / Navigation | No Undo |
| HubHop submenu | Open | Displays HubHop actions | Menu / Navigation | No Undo |
| Download Latest Presets | Click | Starts preset download | External / Runtime | Not a normal Undo action |
| Microsoft Flight Simulator submenu | Open | Displays MSFS actions | Menu / Navigation | No Undo |
| Reinstall WASM Module | Click | Starts WASM reinstall operation  | External / Runtime | Not a normal Undo action |
| Copy Logs | Click | Copies application logs | Clipboard / External | No Undo |
| Controller Bindings | Click | Opens Controller Bindings dialog | Project Configuration | Detailed dialog interactions should be cataloged separately |
| Settings | Click | Opens Application Settings | Global / Application State | Detailed settings interactions should be cataloged separately |

## Help Menu

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Help menu | Click | Opens Help menu | Menu / Navigation | No Undo |
| Documentation | Click / F1 | Opens documentation | External | No Undo |
| Check for Updates | Click | Starts update check | Runtime / External | No Undo |
| Discord | Click | Opens Discord | External | No Undo |
| HubHop | Click | Opens HubHop | External | No Undo |
| YouTube | Click | Opens YouTube | External | No Undo |
| About | Click | Opens About information | Navigation / Information | No Undo |
| Release Notes | Click | Opens release notes | Navigation / External | No Undo |

# Main Menu Quick Actions

The right side of the main menu exposes additional direct interaction points.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Support us | Click | Opens donation/support flow | External | Only visible for eligible non-member users |
| Discord | Click | Opens Discord | External | Same destination as Help menu but separate GUI entry |
| YouTube | Click | Opens YouTube | External | Separate GUI entry |
| HubHop | Click | Opens HubHop | External | Separate GUI entry |
| Light / Dark Mode icon | Click | Changes UI theme | UI / Application Preference | Could theoretically restore previous theme, but not project Undo |
| Sign In | Click | Starts authentication | Authentication | No project Undo |
| User menu | Click | Opens authenticated user menu | Menu / Navigation | Only when authenticated |
| Upgrade | Click | Opens membership upgrade page | External | Only for eligible users |
| Profile | Click | Opens MobiFlight Club profile | External | No Undo |
| Sign Out | Click | Starts logout | Authentication | No project Undo |

The theme toggle is hidden on smaller screen sizes.

# Project Area

The Project area contains the current project, recent projects, project creation entry points, filters, and project-specific controls.

## Create Project

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| + Project button | Click | Opens Create Project dialog | Project Lifecycle | Dialog actions cataloged separately |

The same Create Project action can be exposed in different places depending on the current state:

- Dashboard header
- No active project placeholder
- Empty recent project state

These are separate GUI entry points to the same user flow.

# Current Project Card

If a project is active, the Dashboard displays a Current Project card.

## Open Current Project

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Project title | Click | Opens Config view | Navigation | No project-state Undo |
| Chevron / open button | Click | Opens Config view | Navigation | No project-state Undo |
| Project card | Double-click | Opens Config view | Navigation | No project-state Undo |

Multiple GUI interactions therefore lead to the same Config view.

## Project Information

The card also displays:

- Project image
- Simulator badge
- Connected controller indicators
- Additional controller count
- Favorite star indicator

Most of these are primarily informational.

### Favorite Star

The Favorite Star is visually exposed with a button role, but the current frontend component does not provide its own independent click action.

It should therefore be verified manually in the running application before it is treated as a functional user action.

## Controller Indicators

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Controller icon | Hover | Shows controller/status information | Information | No Undo |
| Binding issue icon | Click | Opens Controller Bindings dialog | Project Configuration / Navigation | Separate entry point to Controller Bindings |
| More controllers indicator | View / Hover | Indicates hidden controller count | Information | No independent action currently identified |

## Project Options Menu

The `...` button opens a project-specific menu.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| `...` menu | Click | Opens project options | Menu / Navigation | No Undo |
| Settings | Click | Opens Edit Project dialog | Project Configuration | Settings edits cataloged separately |
| Controller Bindings | Click | Opens Controller Bindings dialog | Project Configuration | Dialog interactions cataloged separately |

Controller Bindings can therefore currently be opened through several GUI entry points:

```
Main Menu → Extras → Controller Bindings
Current Project → ... → Controller Bindings
Controller Binding Issue icon
```

## Run / Stop

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Run button | Click | Starts configuration execution | Runtime | Not normal project Undo |
| Stop button | Click | Stops configuration execution | Runtime | Not normal project Undo |

The same control switches between Run and Stop depending on execution state.

The control is disabled while testing is active.

# Recent Project Filters

The recent project list exposes one text input and three simulator filter buttons.

## Project Search Field

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Search field | Focus | User can edit filter text | Text Editing | Focus affects Ctrl+Z routing |
| Search field | Type text | Project list is filtered by project name | Native Text Edit / UI State | Ctrl+Z may reasonably be expected to undo text input |
| Search field | Delete text | Project filter result updates immediately | Native Text Edit / UI State | Previous text may be restorable by native text Undo |
| Search field | Replace text | Filter result changes | Native Text Edit / UI State | Important for shortcut conflict with application Undo |

This interaction is particularly important for the Undo/Redo design.

When the search field has focus, `Ctrl+Z` may naturally be expected to undo text editing rather than undo the latest project mutation.

## Simulator Filter Buttons

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| All | Click | Shows projects from all supported simulators | UI / Filter State | Returning to previous filter could be considered, but not necessarily project Undo |
| Microsoft | Click | Shows MSFS projects | UI / Filter State | Previous filter state could conceptually be restored |
| X-Plane | Click | Shows X-Plane projects | UI / Filter State | Previous filter state could conceptually be restored |

Filter state is represented in the current route's search parameters.

These actions should be included in the catalog even if they are later excluded from the project Undo stack.

## Clear Filter

If the active filters produce no matching projects, a Clear Filter button is shown.

| GUI Element  | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Clear Filter | Click | Clears text and simulator filters | UI / Filter State | Could conceptually return to the previous filter state |

# Recent Project List

The recent project area contains a scrollable list of project items.

## Project Item Selection

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Inactive recent project item | Single click | Attempts to load that project | Project Lifecycle / Selection  | May trigger unsaved-change dialog |
| Recent project item | Double-click | Opens Config view for the project when allowed | Navigation / Project Lifecycle | Behavior depends on pending unsaved changes |
| Project item chevron | Click | Opens project/config view | Navigation | Verify interaction with parent item click in running app |
| Recent projects list | Scroll | Changes visible list area | View State | No application Undo expected |

Selecting another project can have different results depending on whether the current project contains unsaved changes.

# Unsaved Changes Confirmation

When the user attempts to load another project while the current project has unsaved changes, the Dashboard can display an Unsaved Changes confirmation dialog.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Discard | Click | Discards current unsaved changes and loads pending project | Project Lifecycle | Destructive decision; not normal Undo |
| Save changes | Click | Saves current project and loads pending project after successful save | Persistence / Project Lifecycle | Not normal editing Undo |
| Close dialog | Dismiss | Cancels pending project switch | Modal / Navigation | Returns to current state |

This is a user-visible transaction boundary and should be treated separately from normal configuration editing.

# Remove Recent Project

Inactive recent project items expose a Remove button.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Remove recent project | Click | Removes project entry from Recent Projects list | Application / Recent List State | Does not delete the project file itself |

This interaction affects the application's recent project list rather than the project configuration.

# Community Area

The Community area displays a feed of MobiFlight community content.

## Community Feed Filters

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| All | Click | Shows all feed items | UI / Filter State  | Could conceptually restore previous filter |
| Community | Click | Shows community posts | UI / Filter State | Same |
| Offers | Click | Shows offer/shop posts | UI / Filter State  | Same |
| Events | Click | Shows event posts | UI / Filter State  | Same |

Like project filters, these filters are stored in URL search parameters and are user-visible state changes even though they do not change the current project.

## Community Feed Content

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Feed | Scroll | Changes visible posts | View State | No application Undo |
| Link inside post text | Click | Opens external URL | External | No Undo |
| Post action button | Click | Opens action-specific external destination | External | Button depends on feed content |

Community action buttons are dynamic because their labels and destinations are defined by the feed content.

# Log Panel

The Log panel is shown only when log display is enabled.

It appears below the main application content and can be resized.

## Log Panel Layout

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Resize handle | Drag | Changes Log panel height | Layout / View State | Not project Undo |
| Log area | Scroll | Changes visible log entries | View State | No Undo |
| Log text | Select text | Selects log text | Native Selection | No project Undo |

## Log Controls

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Copy Logs | Click | Copies logs | Clipboard / External | No Undo |
| Pause | Click | Pauses automatic log following | Local UI / Runtime | Could be toggled back manually |
| Resume | Click | Resumes log following | Local UI / Runtime | Not project Undo |
| Close | Click | Hides Log panel | View State | Not project Undo |

## Log Filter

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Filter field | Focus | User can edit filter text | Text Editing | Important Ctrl+Z context |
| Filter field | Type | Filters visible log entries | Native Text Edit / UI State | Native text Undo is expected |
| Filter field | Delete / replace text | Updates visible log results | Native Text Edit / UI State | Same |
| Clear Filter button | Click | Clears current log filter text | UI / Text State | Previous filter text could conceptually be restored |

# Global Keyboard Context

The current Dashboard participates in the application's global keyboard accelerators.

Currently exposed accelerators include:

```
F1           Documentation
Ctrl+N       New Project
Ctrl+O       Open Project
Ctrl+S       Save
Ctrl+Shift+S Save As
Ctrl+Q       Exit
Ctrl+0       Reset Zoom
Ctrl++       Zoom In
Ctrl+=       Zoom In
Ctrl+-       Zoom Out
```

`Ctrl+Z` and `Ctrl+Y` are not currently registered as a global application accelerator.

This is important for the Undo/Redo design because the Dashboard contains text inputs such as:

```
Project Search
Log Filter
```

When one of those controls has keyboard focus, the user may expect `Ctrl+Z` to apply to the focused text field rather than to the global project history.

The Action Catalog therefore needs to distinguish at least between:

```
Native Text Undo
UI / View State
Project Undo
Application / Global State
Runtime / External Action
Navigation
```

# Dashboard Summary

The Dashboard demonstrates why the Action Catalog must be based on actual GUI interactions rather than only backend mutation commands.

A user can interact with:

- menus
- toolbar buttons
- project cards
- project list items
- text inputs
- filter buttons
- modal confirmations
- external links
- runtime controls
- layout handles
- authentication controls
- community feed controls

Many of these interactions do not modify project configuration, but they still matter when defining the expected behavior of a global `Ctrl+Z` shortcut.

In particular, Dashboard text inputs and filters reveal a distinction between local UI Undo and project-level Undo that would not be visible from project mutation paths alone.

# Project View

The Project View is the main configuration workspace of an opened project.

It is available under the `/config` route and contains project-level controls, profile tabs, configuration-item filters, the configuration-item table, and configuration-item creation and editing actions.

The common Main Menu Toolbar described in the Dashboard section is also available in this view.

This section focuses on the controls and interactions specific to the Project View.

# Project Panel

The upper part of the Project View contains:

- navigation back to the Dashboard
- project name
- project save control
- project options
- execution controls
- profile tabs
- profile creation and merge controls

## Back to Dashboard

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Back button | Click | Returns to Dashboard if there are no unsaved changes | Navigation | Not project Undo |
| Back button with unsaved changes | Click | Opens Unsaved Changes confirmation dialog | Navigation / Project Lifecycle | Dialog determines whether changes are saved or discarded |

## Unsaved Changes Confirmation

When the user attempts to return to the Dashboard while the project contains unsaved changes, a confirmation dialog is shown.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Save changes | Click | Saves the current project and returns to Dashboard | Persistence / Navigation | Not normal editing Undo |
| Discard changes | Click | Discards current unsaved project changes and returns to Dashboard | Project Lifecycle | Destructive action; normally not an Undo entry |
| Close / dismiss | Dismiss | Cancels navigation and keeps the Project View open | Modal / Navigation | No Undo |

# Project Name

The current project name is displayed directly in the Project Panel and can be edited inline.

## Project Name Inline Editing

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Project name | Double-click | Replaces label with text input | Text Editing / Project State | Starts a local edit session |
| Project name input | Type | Changes temporary project name | Native Text Edit | `Ctrl+Z` should normally affect the focused text input |
| Project name input | Enter | Commits new project name | Project State | Candidate for project Undo |
| Project name input | Blur | Commits new project name | Project State | Candidate for project Undo |
| Project name input | Escape | Cancels current edit and restores previous name | Local Edit Cancellation | Should not create an Undo entry |

The project name can also be put into edit mode through the Project Options menu.

The distinction between editing text and committing the rename is important:

```
Double-click
→ edit locally
→ type text
→ Enter / Blur
→ project rename committed
```

Only the committed rename represents a project-state change.

# Save Project

A save button is displayed next to the project name.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Save button | Click | Saves current project changes | Persistence | Not normally part of Undo history |
| Save button without changes | Click | Control is disabled | Persistence | No action |
| Save button after successful save | View | Brief success indication is displayed | Information | No Undo |

# Project Options Menu

The `...` button next to the project name opens project-level actions.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Project `...` button | Click | Opens project options menu | Menu / Navigation | No Undo |
| Rename | Click | Starts project-name inline editing | Text Editing / Project State | Commit behavior described above |
| Settings | Click | Opens Edit Project dialog | Project Configuration | Dialog interactions cataloged separately |
| Controller Bindings | Click | Opens Controller Bindings dialog | Project Configuration | Dialog interactions cataloged separately |

# Execution Toolbar

The Project View contains controls for AutoRun, normal execution, and test execution.

## AutoRun

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| AutoRun button | Click | Enables or disables AutoRun | Application / Runtime Preference | Should be evaluated separately from project Undo |

The button changes appearance according to the current AutoRun state.

## Run / Stop

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Run | Click | Starts project execution | Runtime | Not normal project Undo |
| Stop | Click | Stops project execution | Runtime | Not normal project Undo |

The same button changes between Run and Stop according to execution state.

Run is disabled while Test mode is active.

## Test / Stop Test

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Test | Click | Starts project test mode | Runtime | Not normal project Undo |
| Stop Test | Click | Stops test mode | Runtime | Not normal project Undo |

Test is disabled while normal project execution is active.

# Profile Tabs

Each configuration file in the project is represented as a Profile tab.

A user can select, rename, remove, create, merge, scroll, and use profiles as drop targets for Config Items.

## Select Profile

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Profile tab | Click | Makes selected Profile active and displays its Config Items | Navigation / View State | Usually not expected in project Undo history |

Selecting a Profile changes which Config Item list is currently visible.

# Rename Profile

The active Profile label supports inline editing.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Active Profile name | Double-click | Opens inline text editor | Text Editing | Starts local edit session |
| Profile name input | Type | Changes temporary Profile label | Native Text Edit | `Ctrl+Z` should normally affect the text field |
| Profile name input | Enter | Commits Profile rename | Project State | Candidate for project Undo |
| Profile name input | Blur | Commits Profile rename | Project State | Candidate for project Undo |
| Profile name input | Escape | Cancels edit | Local Edit Cancellation | No Undo entry expected |
| Profile menu → Rename | Click | Starts inline rename mode | Text Editing | Alternative entry point |

The current implementation only enables the inline editor for the active Profile.

The Rename menu is still visually present on Profile menus, so inactive Profile behavior should also be manually verified in the running application.

# Profile Options Menu

Each Profile tab contains a `...` menu.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Profile `...` | Click | Opens Profile menu | Menu / Navigation | No Undo |
| Rename | Click | Starts Profile inline rename | Project State / Text Editing | Candidate once committed |
| Remove | Click | Removes Profile from project | Project State | Strong project Undo candidate |

# Add / Merge Profile

A `+` control is available next to the Profile tabs.

Opening it exposes two actions.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| `+` Profile button | Click | Opens add-profile menu | Menu / Navigation | No Undo |
| New | Click | Creates a new Profile | Project State | Candidate for project Undo |
| Merge | Click | Starts merge-config-file flow | Project State / Import | Requires separate investigation |

# Profile Tab Scrolling

If the Profile tabs exceed the available horizontal space, additional navigation controls become available.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Scroll Profiles Left  | Click | Scrolls Profile tab strip left | View State | No project Undo |
| Scroll Profiles Right | Click | Scrolls Profile tab strip right | View State | No project Undo |
| Profile tab strip | Mouse wheel | Scrolls Profile tabs horizontally | View State | No project Undo |

# Config Item Filter Toolbar

The Config Item table includes a filter toolbar.

The toolbar can contain:

```
Filter items...
Config Type
Controller
Type
Name
Reset
Selected Rows
```

The toolbar is disabled when the active Profile contains no Config Items.

# Config Item Search Field

The `Filter items...` text field filters Config Items by name.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Filter items field | Focus | Enters local text-editing context | Text Editing | Important for Ctrl+Z routing |
| Filter items field | Type | Filters Config Items by name | Native Text Edit / UI State | Native text Undo should remain available |
| Filter items field | Backspace / Delete | Changes filter text and table result | Native Text Edit / UI State | Native text Undo expected |
| Filter items field | Replace selected text | Changes filter result | Native Text Edit / UI State | Native text Undo expected |

The field stops keyboard events from propagating to Config Item table keyboard actions.

This is important because keys such as Delete and Space have table-level meanings when focus is outside text controls.

# Faceted Filters

The Config Item table provides multiple multi-select filters.

## Config Type Filter

Filters by Config Item type, for example Input or Output configuration.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Config Type filter button | Click | Opens filter popover | UI / Filter State | No project-state change |
| Filter option | Click | Selects or deselects Config Type | UI / Filter State | Previous filter could conceptually be restored |
| Clear filter | Click | Clears Config Type filter | UI / Filter State | Could conceptually restore previous filter |

## Controller Filter

Filters Config Items according to the assigned controller.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Controller filter | Click | Opens controller filter | UI / Filter State | No project-state change |
| Controller option | Click | Adds/removes controller from filter | UI / Filter State | View-level state |
| Not Set option | Click | Filters items without controller | UI / Filter State | View-level state |
| Clear filter | Click | Removes Controller filter | UI / Filter State | View-level state |

## Device Type Filter

The `Type` filter selects Config Items according to their configured device type.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Type filter | Click | Opens Device Type filter | UI / Filter State  | No project-state change |
| Device Type option | Click | Adds/removes type from filter | UI / Filter State | View-level state |
| Clear filter | Click | Removes Type filter | UI / Filter State  | View-level state |

## Device Name Filter

The `Name` filter selects Config Items according to configured device name.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Name filter | Click | Opens Device Name filter | UI / Filter State | No project-state change |
| Device Name option | Click | Adds/removes name from filter | UI / Filter State | View-level state |
| Clear filter | Click | Removes Device Name filter | UI / Filter State | View-level state |

# Filter Popover Search

Each faceted-filter popover also contains its own text search field.

This means that opening a filter introduces an additional text-editing context.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Filter-option search | Focus | Starts local text-editing context | Text Editing | Ctrl+Z should remain local |
| Filter-option search | Type | Filters available filter options | Native Text Edit / UI State | Native Undo expected |
| Filter-option search | Delete / replace text | Changes visible filter options | Native Text Edit / UI State | Native Undo expected |

# Reset All Filters

When one or more filters are active, a Reset control becomes available.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Reset filters | Click | Clears all Config Item column filters | UI / Filter State | Previous complete filter state could conceptually be restored |

If active filters produce zero visible Config Items, a separate Reset button is displayed in the empty-results view.

This is a second GUI entry point to the same overall filter-reset behavior.

# Sorting

The Config Item table internally maintains sorting state, but the current Project View does not expose an active sorting control in the table headers.

A previous Name-column sorting control exists in source code only as commented code.

Therefore column sorting is currently **not** cataloged as an available user interaction.

# Config Item Table

The Config Item table displays the Config Items belonging to the active Profile.

Visible columns can include:

- Active
- Name
- Controller
- Device
- Status
- Raw Value
- Final Value
- Actions

Some columns are hidden depending on window size.

# Config Item Selection

Rows support single selection, additive selection, and range selection.

## Single Selection

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Config Item row | Click | Selects only that row | Selection / View State | Normally not project Undo |

## Additive Selection

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Config Item row | Ctrl+Click / Meta+Click | Adds or removes row from current selection | Selection / View State | Normally not project Undo |

## Range Selection

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Config Item row | Shift+Click | Selects range between previous and current row | Selection / View State | Normally not project Undo |

# Row Right-Click

Right-clicking a Config Item row performs two user-visible actions:

```
Select clicked row
→ Open Config Item context menu
```

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Config Item row | Right-click | Selects row and opens context menu | Selection / Menu | Selection itself is not project Undo |

# Selected Rows Menu

When one or more rows are selected, the toolbar displays a button showing the number of selected rows.

Clicking this button exposes bulk actions.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| `N rows selected` | Click | Opens selected-row actions | Menu | No Undo, but possible |
| Delete selected | Click | Deletes all selected Config Items | Project State | Strong Undo candidate |
| Toggle selected | Click | Toggles Active state of selected items | Project State | Strong Undo candidate |
| Clear selection | Click | Clears current row selection | Selection / View State | Normally not project Undo |

# Config Item Selection Keyboard Actions

When focus is not captured by another control, selected rows can also be manipulated through keyboard shortcuts.

| Key | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|
| Delete | Deletes selected Config Items | Project State | Same semantic action as Bulk Delete |
| Backspace | Deletes selected Config Items | Project State | Same semantic action as Bulk Delete |
| Space | Toggles Active state of selected Config Items | Project State | Same semantic action as Bulk Toggle |
| Escape | Clears current row selection | Selection / View State | No project Undo |

`Enter` is currently included in the table's recognized-key list but has no corresponding action implemented.

It should therefore not currently be treated as a functional user action.

# Drag and Drop

Each Config Item row exposes a drag handle when the row is hovered.

Drag and Drop supports both reordering within a Profile and moving Config Items between Profiles.

## Single Config Item Reorder

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Row drag handle | Drag and drop within table | Moves Config Item to a different position | Project State | Strong project Undo candidate |

## Drag Unselected Item

If the user starts dragging a row that was not previously selected, that row becomes the only dragged item.

| User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|
| Drag unselected row | Row becomes selected and is dragged alone | Selection + Project State | Selection is UI state; completed move is Undo candidate |

## Multi-Item Drag

If multiple rows are selected, dragging one of the selected rows moves the selected Config Items together.

| User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|
| Drag selected group | Selected Config Items move as a group | Project State | Strong Undo candidate |

# Cross-Profile Drag and Drop

Config Items can be dragged from one Profile to another.

The user interaction can consist of:

```
Select item(s)
→ start drag
→ hover another Profile tab
→ target Profile becomes active
→ choose target location
→ drop
```

| User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|
| Drag item(s) over another Profile | Target Profile becomes active after hover delay | Navigation + Temporary Drag State | Intermediate drag state, not yet committed Undo |
| Drop item(s) in another Profile | Config Items move to target Profile and target position | Project State | Strong Undo candidate |

The current implementation automatically switches to a hovered Profile after a short delay.

# Drag Into Empty Profile

If the target Profile contains no Config Items, a temporary drop target is shown.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Empty Profile drop target | Drop item(s) | Adds moved Config Items to empty Profile | Project State | Strong Undo candidate |

# Cancel Drag

An active drag operation can be cancelled.

| User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|
| Escape during drag | Cancels drag and restores item(s) to original position/Profile | Local Interaction Cancellation | Should not create a project Undo entry |

This is an important distinction:

```
Drag + Escape
= cancel uncommitted interaction

Drag + Drop + Ctrl+Z
= Undo committed project mutation
```

# Active Toggle

Each Config Item row contains an Active switch.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Active switch | Click | Toggles Config Item Active state | Project State | Strong project Undo candidate |

This is one of the simplest direct Project-state changes in the view.

# Config Item Name

The Name cell supports inline editing.

## Rename Config Item

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Config Item name | Double-click | Opens inline name editor | Text Editing | Starts local editing |
| Name input | Type | Changes temporary name | Native Text Edit | Ctrl+Z should affect local text edit |
| Name input | Enter | Commits Config Item rename | Project State | Candidate for project Undo |
| Name input | Blur | Commits Config Item rename | Project State | Candidate for project Undo |
| Name input | Escape | Cancels rename | Local Edit Cancellation | No Undo entry |
| Row menu → Rename | Click | Starts same inline editor | Text Editing | Alternative entry point |

# Controller Cell

A Config Item with an assigned controller displays its controller name.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Controller name | Hover | Shows controller name and serial tooltip | Information | No Undo |
| Controller Settings icon | Click | Opens controller settings | Project / Hardware Configuration | Separate interaction flow |

The Controller Settings icon becomes visible when the row is hovered.

The Controller column can be hidden at smaller window sizes.

# Device Cell

The Device cell displays the configured device and status information.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Device information | Hover | Shows device/status tooltip | Information | No Undo |

There is currently no direct edit action attached to the Device cell itself.

# Status Cell

The Status column can display indicators for:

- Precondition
- Test
- Config Reference

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Status indicator | Hover | Shows detailed status tooltip | Information | No Undo |

These indicators are informational and do not currently provide click actions.

# Raw and Final Value Cells

Raw and Final Value cells display runtime values and runtime error/status indicators.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Raw Value | Hover | Shows complete value or source-status tooltip | Runtime Information | No Undo |
| Final Value | Hover | Shows complete value or modifier-status tooltip | Runtime Information | No Undo |

These values can update automatically while the project is running.

# Config Item Edit

Each Config Item row exposes a dedicated Edit button.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Edit button on Input Config | Click | Opens Input Config editor | Project Configuration | Editor interactions cataloged separately |
| Edit button on Output Config | Click | Opens the current legacy Output Config editor | Legacy UI / Deferred | Detailed Undo/Redo analysis deferred until the new Output Config UI is available |
| Config Item row | Double-click | Opens corresponding Config Item editor | Project Configuration | Alternative entry point |

Input Config editing is cataloged in detail below.

The current Output Config editing flow uses the legacy WinForms interface and is intentionally deferred until the new Output Config UI is available.

# Config Item Actions Menu

Each row contains an `...` button.

Opening it displays:

```
Edit
Rename
Delete
Duplicate
Test
```

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Row `...` button | Click | Opens Config Item menu | Menu | No Undo |
| Edit | Click | Opens corresponding Config Item editor | Project Configuration | Detailed editor action |
| Rename | Click | Starts inline rename | Project State / Text Editing | Commit is Undo candidate |
| Delete | Click | Removes Config Item | Project State | Strong Undo candidate |
| Duplicate | Click | Creates duplicated Config Item | Project State | Strong Undo candidate |
| Test | Click | Tests this Config Item | Runtime | Not normal project Undo |

# Config Item Right-Click Context Menu

Right-clicking a row exposes the same semantic actions as the `...` menu:

```
Edit
Rename
Delete
Duplicate
Test
```

The context menu is therefore a separate GUI entry point to the same Config Item actions.

# Delete Config Item

Deleting a Config Item removes the row from the active Profile.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Delete | Click | Config Item disappears from table | Project State | Strong Undo candidate |

Delete can be triggered through the Config Item menu or right-click context menu.

Bulk deletion is a separate interaction described under Selected Rows.

# Duplicate Config Item

Duplicating a Config Item creates a new Config Item based on the selected source item.

The user-visible flow includes additional behavior after creation:

```
Duplicate
→ new row appears
→ duplicated row is selected
→ corresponding editor opens
```

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Duplicate | Click | Creates and selects duplicated Config Item | Project State | Strong Undo candidate |

The automatic editor opening should be considered separately from the actual Duplicate mutation when defining an Undo action boundary.

# Test Individual Config Item

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Test | Click | Starts test action for selected Config Item | Runtime | Not normal project Undo  |

This is separate from the Project-level Test control in the Execution Toolbar.

# Add Config Item

Two creation buttons are displayed below the Config Item table.

## Add Output Config

The Add Output Config action currently opens the legacy WinForms-based Output Config Wizard.

Undo/Redo analysis for this flow is intentionally deferred because the Output Config editor is planned to be replaced by the new UI.

The interaction should be cataloged again after the UI migration has been completed.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Add Output Config | Click | Opens the current legacy Output Config flow | Legacy UI / Deferred | Undo/Redo analysis deferred until the new Output Config UI is available |

## Add Input Config

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Add Input Config | Click | Creates new Input Config and opens Input editor | Project State | Creation is an Undo candidate |

After creation, the new Config Item is automatically selected.

# Newly Created Item Hidden by Filters

A newly created or duplicated Config Item can be invisible because of the currently active filters.

In this case, after the editor closes, the application can display a notification with an action to reset the filters.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Reset Filter notification action | Click | Clears filters, scrolls new item into view, and selects it | UI / Filter State | Not the Config creation Undo itself |

# Empty Profile State

When the active Profile contains no Config Items, the table displays an empty state.

The user can still use:

```
Add Output Config
Add Input Config
```

The Config Item filter controls are disabled while there are no Config Items.

# Responsive Behavior

Some Project View interactions depend on available screen width.

For example, the Controller column is hidden at smaller window sizes.

As a result, controls located inside responsive columns, such as the Controller Settings shortcut, may not always be visible.

This should be considered when cataloging GUI entry points.

# Project View Keyboard Context

The Project View contains several text-editing controls:

```
Project Name editor
Profile Name editor
Config Item Name editor
Config Item Search
Faceted Filter search fields
```

When one of these inputs has focus, normal text-editing behavior should be distinguished from application-level Undo/Redo.

The Project View also defines Config Item selection shortcuts:

```
Delete / Backspace → Delete selected Config Items
Space              → Toggle selected Config Items
Escape             → Clear selection
```

and Escape also has local meanings in other contexts:

```
Inline edit + Escape → cancel edit
Drag operation + Escape → cancel drag
```

Therefore keyboard handling must consider the current interaction context before applying global Undo/Redo shortcuts.

# Project View Summary

The Project View contains several fundamentally different classes of user
interaction:

- local text editing
- Profile navigation
- Project metadata editing
- Project persistence
- Config Item filtering
- Config Item selection
- Config Item creation
- Config Item property updates
- Config Item deletion
- bulk mutation
- drag and drop
- cross-Profile movement
- runtime execution and testing
- informational hover interactions

From an Undo/Redo perspective, the Project View also demonstrates an important difference between **cancelling an interaction that has not yet been committed** and **undoing a committed Project mutation**.

Examples:

```
Rename + Escape
→ local cancellation

Drag + Escape
→ local cancellation

Rename + Enter + Ctrl+Z
→ potential Project Undo

Drag + Drop + Ctrl+Z
→ potential Project Undo
```

The Project View is therefore the central area for defining how global Undo/Redo should coexist with local text editing, selection, filtering, navigation, and interaction-specific cancellation.

# Input Config Editor

The Input Config Editor is opened for an Input Config Item under the `/config/:configId` route.

It can be reached through several GUI entry points:

- Config Item Edit button
- double-clicking an Input Config Item row
- Config Item context menu → Edit
- creating a new Input Config Item
- duplicating an Input Config Item

The editor is displayed as a modal dialog on top of the Project View.

Changes made inside the editor are kept as local draft state until the user selects **Apply Changes**.

This distinction is important for Undo/Redo:

```
Edit fields inside dialog
→ local draft state only

Apply Changes
→ Config Item update is committed

Cancel
→ local draft is discarded
```

Therefore individual controls inside the dialog should not necessarily create independent project-level Undo entries.

# Dialog-Level Interactions

## Config Name

The Input Config Item name is displayed in the dialog header and supports inline editing.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Config name | Double-click | Opens inline name editor | Text Editing | Local draft edit |
| Name input | Type | Changes temporary Config Item name | Native Text Edit | Ctrl+Z should normally affect local text |
| Name input | Enter | Commits name into dialog draft | Draft Project State | Project itself is still unchanged |
| Name input | Blur | Commits name into dialog draft | Draft Project State | Same as Enter |
| Name input | Escape | Cancels current inline edit | Local Edit Cancellation | No project Undo |

For a newly created Input Config with the default name, the name field can automatically enter edit mode and receive focus.

## Dialog Closing and Commit

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Apply Changes | Click | Commits complete draft Config Item and closes dialog | Project State | Natural project Undo boundary |
| Cancel | Click | Discards draft changes and closes dialog | Local Edit Cancellation | Should not create Undo entry |
| Escape without pending changes | Press | Closes dialog | Modal / Navigation | No Undo |
| Click outside without pending changes | Click | Closes dialog | Modal / Navigation | No Undo |
| Escape with pending changes | Press | Dialog remains open and pending-changes warning is shown | Local Protection | No Undo |
| Click outside with pending changes | Click | Dialog remains open and pending-changes warning is shown | Local Protection | No Undo |
| Close `X` | Click | Closes the dialog | Modal / Navigation | Pending-change behavior should be manually verified |

When pending changes exist, attempting to dismiss the dialog with Escape or an outside click causes a warning message and a short visual indication rather than silently closing the editor.

# Trigger Panel

The Trigger panel defines which physical input causes the Input Config to execute.

## Scan for Input

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Scan for Input | Click | Starts input scanning | Runtime / Draft Editing | Does not yet commit project state |
| Use any input / scanning button | Click while scanning | Stops input scanning | Runtime | Local operation |
| Physical input during scan | Perform input | Detected Controller and Device are selected automatically | Draft Project State | Becomes project state only on Apply |

## Controller Selection

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Controller combobox | Click | Opens available Controller list | Selection / Draft State | No immediate project mutation |
| Controller search | Type | Filters available Controllers | Native Text Edit / UI State | Ctrl+Z should remain local |
| Controller option | Select | Sets selected Controller and clears previous Device selection | Draft Project State | Applied later |
| Selected Controller | Change | Replaces Controller and resets Device | Draft Project State | Applied later |

The currently configured Controller can still be shown even if it is not currently connected.

## Device Selection

The Device combobox becomes available after a Controller has been selected.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Device combobox | Click | Opens available input devices | Selection / Draft State | No immediate project mutation |
| Device search | Type | Filters available input devices | Native Text Edit / UI State | Local text Undo |
| Device option | Select | Sets the input Device | Draft Project State | Applied later |

Only supported input-device types are shown.

## Clear Input

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Clear Input | Click | Clears Controller and Device selections | Draft Project State | Becomes committed only on Apply |

# Additional Configuration Sections

The main editor provides direct entry points to:

```
+ Preconditions
+ Config References
+ Modifiers
```

Each button opens a detail drawer on the right side of the dialog.

| GUI Element | User Interaction | Visible Result | Interaction Domain  | Undo Expectation / Notes |
|---|---|---|---|---|
| Preconditions | Click | Opens Preconditions detail drawer | Draft Configuration | Local until Apply |
| Config References | Click | Opens Config References drawer | Draft Configuration | Local until Apply |
| Modifiers | Click | Opens Modifier drawer | Draft Configuration | Local until Apply |

If one of these sections already contains configuration, a summary card is displayed in the main dialog.

The summary can normally be opened through either:

- Edit button
- double-clicking the summary card

# Detail Drawer

Preconditions, Config References, Modifiers, and Action editors are displayed inside a right-side drawer.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Go Back | Click | Closes detail drawer and returns to main editor | Navigation within draft | No project Undo |
| Detail content | Scroll | Scrolls configuration controls | View State | No Undo |

The detail drawer is not independently committed.

Changes made there immediately update the dialog's draft Config Item, but they still require **Apply Changes** on the main dialog.

# Preconditions

Preconditions determine whether an Input Config action can execute.

## Open Preconditions

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Add Preconditions button  | Click | Opens Preconditions editor | Draft Configuration | No project mutation yet  |
| Preconditions summary | Double-click | Opens Preconditions editor | Navigation within draft | No project Undo |
| Preconditions Edit button | Click | Opens Preconditions editor | Navigation within draft | No project Undo |

## Add Precondition

The Add Precondition menu provides different precondition types.

Current types include:

```
Config
Variable
Arcaze-Pin
```

| GUI Element | User Interaction | Visible Result | Interaction Domain  | Undo Expectation / Notes |
|---|---|---|---|---|
| Add Precondition | Click | Opens type menu | Menu | No Undo |
| Config | Click | Adds Config precondition | Draft Project State | Included in final Apply  |
| Variable | Click | Adds Variable precondition | Draft Project State | Included in final Apply  |
| Arcaze-Pin | Click | Adds Pin precondition | Draft Project State | Included in final Apply  |

## Precondition Row

Each precondition exposes several controls.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Move Up | Click | Moves precondition earlier | Draft Ordering | Applied later |
| Move Down | Click | Moves precondition later | Draft Ordering | Applied later |
| Active switch | Toggle | Enables/disables precondition | Draft Project State | Applied later |
| Config selector | Select | Chooses referenced Output Config | Draft Project State | Config-type precondition only |
| Variable selector | Select | Chooses referenced variable | Draft Project State | Variable-type precondition only |
| Arcaze Port | Select | Selects Port A or B | Draft Project State | Pin-type only |
| Arcaze Pin | Select | Selects Pin | Draft Project State | Pin-type only |
| Operand | Select | Chooses comparison operator | Draft Project State | `=`, `<>`, `<`, `>`, `<=`, `>=` |
| Value | Type | Changes comparison value | Native Text / Draft State | Ctrl+Z should remain local |
| Logic | Select | Chooses `and` / `or` between conditions | Draft Project State | Not shown on final condition |
| Delete | Click | Removes precondition | Draft Project State | Applied later |

# Config References

Config References allow another Output Config to be referenced inside the Input Config.

## Open Config References

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Add Config Reference | Click | Opens Config Reference editor | Draft Configuration | No immediate project mutation |
| Config Reference summary | Double-click | Opens detail editor | Navigation within draft | No project Undo |
| Edit Config References | Click | Opens detail editor | Navigation within draft | No project Undo |

## Config Reference Editor

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Add Config Reference | Click | Adds new reference row | Draft Project State | Included in Apply |
| Active switch | Toggle | Enables/disables reference | Draft Project State | Included in Apply |
| Output Config selector | Select | Chooses referenced Config Item | Draft Project State | Included in Apply |
| Placeholder field | Type | Changes reference placeholder | Native Text / Draft State | Local Ctrl+Z expected |
| Delete | Click | Removes Config Reference | Draft Project State | Included in Apply |

New references receive a suggested placeholder automatically.

# Modifiers

Modifiers transform values before the Input Config action uses them.

## Open Modifier Editor

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Add Modifier | Click | Opens Modifier editor | Draft Configuration | No immediate project mutation |
| Modifier summary | Double-click | Opens Modifier editor | Navigation within draft | No project Undo |
| Modifier Edit button | Click | Opens Modifier editor | Navigation within draft | No project Undo |

## Add Modifier

The current Modifier types are:

```
Blink
Comparison
Interpolation
Padding
Substring
Transformation
```

The available types are displayed alphabetically.

| GUI Element | User Interaction | Visible Result | Interaction Domain  | Undo Expectation / Notes |
|---|---|---|---|---|
| Add Modifier | Click | Opens Modifier type menu | Menu | No Undo |
| Modifier type | Click | Adds Modifier | Draft Project State | New Modifier expands automatically |

## Modifier Item Controls

Every Modifier item provides common controls.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Move Up | Click | Moves Modifier earlier in chain | Draft Ordering | Included in Apply |
| Move Down | Click | Moves Modifier later in chain | Draft Ordering | Included in Apply |
| Active switch | Toggle | Enables/disables Modifier | Draft Project State | Included in Apply |
| Modifier header | Click | Expands/collapses Modifier | View State | No project Undo |
| Delete | Click | Removes Modifier | Draft Project State | Included in Apply |

## Transformation Modifier

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Expression | Type | Changes transformation expression | Native Text / Draft State | Local text Undo |

## Substring Modifier

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Start | Enter number | Changes substring start position | Draft State | Applied later |
| End | Enter number | Changes substring end position | Draft State | Applied later |

## Padding Modifier

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Length | Enter number | Changes target length | Draft State | Applied later |
| Character | Type character Changes padding character | Draft State | Limited to one character |
| Direction | Select | Selects Left or Right padding | Draft State | Applied later |

## Interpolation Modifier

Interpolation contains a table of mappings.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| From | Enter number | Changes source mapping value | Draft State | Applied later |
| To | Enter number | Changes destination mapping value | Draft State | Applied later |
| Add Mapping | Click | Adds interpolation mapping | Draft State | Applied later |
| Remove Mapping | Click | Removes mapping | Draft State | Disabled when minimum mapping count is reached |

## Comparison Modifier

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Operator | Select | Changes comparison operator | Draft State | Applied later |
| Value | Type | Changes comparison value/expression | Native Text / Draft State | Local text Undo |
| Then | Type | Changes result when condition is true | Native Text / Draft State | Local text Undo |
| Else | Type | Changes alternative result | Native Text / Draft State | Local text Undo |

## Blink Modifier

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Alternate Value | Edit | Changes blink alternate value | Draft State | Applied later |
| On time | Enter number | Changes ON duration | Draft State | Applied later |
| Off time | Enter number | Changes OFF duration | Draft State | Applied later |
| Add Blink Step | Click | Adds ON/OFF pair | Draft State | Applied later |
| Remove Blink Step | Click | Removes ON/OFF pair  | Draft State | Minimum sequence cannot be removed` |

# Action Bindings

Available action events depend on the selected input Device type.

## Button Inputs

A Button supports:

```
On Press
On Release
On Hold
On Long Release
```

## Encoder Inputs

An Encoder supports:

```
On Left
On Right
On Left Fast
On Right Fast
```

## Analog Inputs

An Analog Input supports:

```
On Change
```

If no supported Trigger is configured, the Action panel only displays an informational message.

# Add Action

If an event has no action, its event button is displayed.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Event button | Click | Opens Action Editor drawer | Draft Configuration | No project commit yet |

Examples include:

```
On Press
On Release
On Hold
On Left
On Right
On Change
```

# Existing Action

Configured actions are shown as summary rows.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Action summary | Double-click | Opens Action Editor | Navigation within draft | No project Undo |
| Edit Action | Click | Opens Action Editor  | Navigation within draft | No project Undo |
| Remove Action | Click | Removes event action | Draft Project State | Included in Apply |

The Remove button becomes visible when the action row is hovered.

# Action Editor

The Action Editor is displayed inside the right-side detail drawer.

## Action Type

The Action Type combobox determines which action configuration panel is shown.

Available choices depend on the current project simulator and enabled features.

Possible action types include:

```
Microsoft Flight Simulator
X-Plane
ProSim
MobiFlight Variable
Retrigger Switches
Keyboard Input
Virtual Joystick (vJoy)
FSUIPC Offset
FSUIPC PMDG Event ID
FSUIPC Lua Macro
FSUIPC Jeehell Event
FSUIPC Event ID
```

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Action Type combobox | Click | Opens available action types | Selection / Draft State | Depends on project settings |
| Action Type search | Type | Filters action types | Native Text / UI State | Local Undo |
| Action Type option | Select | Changes event action type | Draft Project State | Included in final Apply |
| Selected action type | Deselect | Removes current action | Draft Project State | Included in Apply |

# Copy / Paste Action

The Action Editor provides an application-internal clipboard for actions.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Copy | Click | Copies current Action to application clipboard | Clipboard / Draft Utility | No project state change |
| Paste | Click | Replaces current draft Action with copied Action | Draft Project State | Included in Apply |

Copy is disabled if no action exists.

Paste is disabled if the application action clipboard is empty.

# Button Event Timing Options

Certain Button events expose additional timing inputs.

## On Hold

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Hold Delay | Enter value | Changes hold delay | Draft State | Applied later |
| Repeat Delay | Enter value | Changes repeat delay | Draft State | Applied later |

## On Long Release

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Long Release Delay | Enter value | Changes long-release threshold | Draft State | Applied later |

# Microsoft Flight Simulator Action

The MSFS action supports selecting a HubHop preset or entering custom command code.

## Preset Browser

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Project Aircraft Only | Toggle | Restricts presets to project aircraft | UI / Filter State | Local editor state |
| Filter Presets | Type | Filters preset list | Native Text / UI State | Local Ctrl+Z |
| Vendor filter | Select | Filters presets by vendor | UI / Filter State | Local only |
| Vendor filter search | Type | Filters vendor options | Native Text | Local Undo |
| Aircraft filter | Select | Filters by aircraft | UI / Filter State | Local only |
| Aircraft filter search | Type | Filters aircraft options | Native Text | Local Undo |
| System filter | Select | Filters by system | UI / Filter State | Local only |
| System filter search | Type | Filters system options | Native Text | Local Undo |
| Reset Filters | Click | Clears preset filters | UI / Filter State | No project mutation |
| Preset list item | Click  | Selects preset and fills command | Draft Project State | Applied later |
| Preset list | Scroll | Scrolls available presets | View State | No Undo |

## Preset Details

If the selected preset contains HubHop metadata, a HubHop button is displayed.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| HubHop | Click | Opens selected preset on HubHop | External | No project Undo |

## Command Code

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Command code textarea | Type | Changes MSFS command | Native Text / Draft State | Local Ctrl+Z expected |

Manually changing the command disconnects the draft Action from its selected preset.

# X-Plane Action

The X-Plane preset browser provides the same preset-filter interactions as the MSFS browser:

- Project Aircraft Only
- free-text preset filter
- Vendor filter
- Aircraft filter
- System filter
- individual combobox searches
- Reset Filters
- preset list selection
- preset list scrolling
- HubHop link

Additional X-Plane controls include:

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Input Type  | Select | Chooses DataRef or Command | Draft Project State | Applied later |
| Path | Type | Changes X-Plane path/command | Native Text / Draft State | Local text Undo |
| Value | Type | Changes DataRef expression | Native Text / Draft State | Only shown for DataRef |

# MobiFlight Variable Action

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Existing Variable | Select | Uses existing MobiFlight variable | Draft Project State | Applied later |
| Variable Type | Select | Selects Number or String | Draft Project State | Applied later |
| Variable Name | Type | Changes variable name | Native Text / Draft State | Local Ctrl+Z |
| Expression | Type | Changes variable expression | Native Text / Draft State | Local Ctrl+Z |

# Retrigger Action

The Retrigger action currently contains explanatory information but no additional user-configurable fields after the action type has been selected.

Selecting or deselecting the action type is therefore the main interaction.

# Keyboard Input Action

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Scan for Keyboard | Click | Starts keyboard-shortcut capture | Local Input Capture | No project commit yet |
| Keyboard keys         | Press            | Captures Ctrl / Alt / Shift / key combination      | Draft Project State | Applied later            |
| Stop Scanning         | Click            | Stops capture and stores scanned shortcut in draft | Draft Project State | Applied later            |
| Release captured key  | Key Up           | Finishes capture                                   | Draft Project State | Applied later            |
| Escape while scanning | Press            | Cancels keyboard scan                              | Local Cancellation  | No project Undo          |
| Clear Input           | Click            | Resets keyboard combination to None                | Draft Project State | Applied later            |

Keyboard capture intentionally consumes keyboard input while scanning.

# Virtual Joystick (vJoy) Action

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| vJoy Controller     | Select           | Chooses virtual joystick device | Draft Project State       | Applied later                |
| Button tab          | Click            | Configures button action        | Draft / View State        | Switching clears axis mode   |
| Axis tab            | Click            | Configures axis action          | Draft / View State        | Switching clears button mode |
| Button Number       | Select           | Chooses vJoy button             | Draft Project State       | Button mode                  |
| Button State switch | Toggle           | Selects Pressed / Released      | Draft Project State       | Button mode                  |
| Axis                | Select           | Chooses vJoy axis               | Draft Project State       | Axis mode                    |
| Axis Value          | Type             | Changes value sent to axis      | Native Text / Draft State | Axis mode                    |

# FSUIPC Offset Action

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Type        | Select                 | Selects Integer, Float, or String | Draft Project State       | Changes available controls                |
| Size        | Select                 | Selects FSUIPC data size          | Draft Project State       | Applied later                             |
| Offset      | Type hexadecimal value | Changes FSUIPC offset             | Native Text / Draft State | Input restricts characters to hexadecimal |
| Mask        | Type hexadecimal value | Changes bit mask                  | Native Text / Draft State | Integer mode only                         |
| BCD Mode    | Toggle                 | Enables/disables BCD mode         | Draft Project State       | Integer mode only                         |
| Value       | Type                   | Changes value/expression          | Native Text / Draft State | Applied later                             |

Mask and BCD controls are only displayed for Integer offset types.

# FSUIPC Event ID Action

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Preset           | Select           | Selects Event ID preset   | Draft Project State       | Applied later            |
| Preset search    | Type             | Filters Event ID presets  | Native Text / UI State    | Local Undo               |
| Event ID         | Type             | Changes Event ID manually | Native Text / Draft State | Applied later            |
| Custom Parameter | Type             | Changes event parameter   | Native Text / Draft State | Applied later            |

# FSUIPC PMDG Event ID Action

The PMDG variant additionally exposes aircraft and mouse-event controls.

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| B737             | Select radio     | Chooses B737 preset set             | Draft Project State       | Applied later                      |
| B747             | Select radio     | Chooses B747 preset set             | Draft Project State       | Applied later                      |
| B777             | Select radio     | Chooses B777 preset set             | Draft Project State       | Applied later                      |
| Event preset     | Select           | Selects PMDG event                  | Draft Project State       | Applied later                      |
| Event ID         | Type             | Changes Event ID manually           | Native Text / Draft State | Applied later                      |
| Mouse Parameter  | Select           | Selects predefined PMDG mouse flag  | Draft Project State       | Applied later                      |
| Custom Parameter | Type             | Enters custom mouse/event parameter | Native Text / Draft State | Available when parameter is custom |

# FSUIPC Jeehell Action

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Function        | Select           | Chooses Jeehell event/function | Draft Project State       | Applied later            |
| Function search | Type             | Filters available functions    | Native Text / UI State    | Local Undo               |
| Value           | Type             | Changes parameter value        | Native Text / Draft State | Applied later            |

The selected preset description is displayed as information.

# FSUIPC Lua Macro Action

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Macro Name  | Type             | Changes Lua macro name  | Native Text / Draft State | Local Ctrl+Z             |
| Macro Value | Type             | Changes Lua macro value | Native Text / Draft State | Local Ctrl+Z             |

# ProSim Action

## ProSim Presets

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Filter Presets  | Type             | Filters available ProSim DataRefs | Native Text / UI State | Local Undo                          |
| Preset row      | Click            | Selects ProSim DataRef            | Draft Project State    | Applied later                       |
| Preset list     | Scroll           | Scrolls available DataRefs        | View State             | No Undo                             |
| Refresh Presets | Click            | Requests updated ProSim presets   | Runtime / External     | Shown when no presets are available |

The selected ProSim path is displayed as read-only information.

## ProSim Parameter

| GUI Element | User Interaction | Visible Result | Interaction Domain | Undo Expectation / Notes |
|---|---|---|---|---|
| Parameter   | Type             | Changes ProSim expression/parameter | Native Text / Draft State | Local Ctrl+Z             |

# Input Config Editor Undo Boundary

The Input Config Editor is especially important for the Undo/Redo design because it contains many small controls, but they all operate on one temporary draft Config Item.

For example:

```
Open Input Config
→ change Controller
→ change Device
→ add Precondition
→ add Modifier
→ edit On Press Action
→ rename Config
→ Apply Changes
```

From the user's point of view, all of these edits occur inside one dialog session.

The current implementation sends the updated Config Item to the project only when **Apply Changes** is selected.

Therefore a natural project-level Undo model is:

```
Before opening / applying:
Config Item A

Apply Changes
→ Config Item A'

Ctrl+Z
→ restore Config Item A
```

rather than creating a separate project Undo entry for every combobox, checkbox, text field, or modifier interaction inside the dialog.

At the same time, native text Undo should remain available while the user is actively editing text fields inside the dialog.

Examples include:

```
Config name
Controller / Device searches
Precondition values
Config Reference placeholders
Modifier expressions
Preset filters
Action expressions
FSUIPC fields
Lua Macro fields
```

This creates two distinct levels of Undo behavior:

```
Ctrl+Z while editing a focused text field
→ local/native text Undo

Ctrl+Z after Apply Changes and dialog close
→ project-level Config Item Undo
```

# Input Config Editor Summary

The Input Config Editor contains the following major interaction groups:

- Config Item name editing
- Controller and Device selection
- automatic input scanning
- Preconditions
- Config References
- Modifiers
- Button / Encoder / Analog event bindings
- Action type selection
- action-specific configuration
- application-internal copy/paste
- local draft cancellation
- final Apply / Cancel

The central observation is that the editor acts as a **compound form-edit transaction**.

Most GUI interactions change only temporary dialog state.

`Apply Changes` is the primary boundary where those interactions become one committed Project-state mutation.

# Create / Edit Project

Project creation and Project Settings editing use the same Project Form.

The form is shown as a modal dialog and supports two modes:

```
Create Project
→ "Create New Project"
→ Create button

Edit Project
→ "Edit Project"
→ Update button
```

Possible GUI entry points include:

```
Dashboard → + Project
Main Menu → File → New

Dashboard → Current Project → ... → Settings
Project View → Project ... → Settings
```

The same form controls are used in both modes.

Changes made inside the form are kept as local frontend state until the user selects **Create** or **Update**.

# Form-Level Interaction Model

The Project Form contains:

```
Project Name
Simulator
Additional Simulator Features
Aircraft
Cancel
Create / Update
```

The dialog content is scrollable if the available vertical space is too small.

The action buttons remain accessible independently of the scrollable form content.

# Project Name

The Project Name is a text input.

| GUI Element  | User Interaction      | Visible Result              | Interaction Domain        | Undo Expectation / Notes            |
| ------------ | --------------------- | --------------------------- | ------------------------- | ----------------------------------- |
| Project Name | Focus                 | Starts text-editing context | Text Editing              | Ctrl+Z should normally remain local |
| Project Name | Type                  | Changes draft Project name  | Native Text / Draft State | Project not changed yet             |
| Project Name | Backspace / Delete    | Modifies draft name         | Native Text / Draft State | Native text Undo expected           |
| Project Name | Select / replace text | Replaces draft name         | Native Text / Draft State | Native text Undo expected           |
| Project Name | Enter                 | Submits the current form    | Form Commit               | Same submit path as Create / Update |

The form intercepts Enter and uses it to submit the Project Form.

The Project name is trimmed before submission.

An empty Project name prevents the form from being submitted.

If validation fails, the dialog remains open and an error state is shown for the Project Name.

Browser autocomplete suggestions are disabled for this field.

# Simulator Selection

The Project Form displays four simulator choices as large clickable cards:

```
Microsoft Flight Simulator
X-Plane
Prepar3D
FSX / FS2004
```

Each simulator card behaves as a radio option.

| GUI Element                | User Interaction | Visible Result   | Interaction Domain  | Undo Expectation / Notes          |
| -------------------------- | ---------------- | ---------------- | ------------------- | --------------------------------- |
| Microsoft Flight Simulator | Click            | Selects MSFS     | Draft Project State | Also resets Features and Aircraft |
| X-Plane                    | Click            | Selects X-Plane  | Draft Project State | Also resets Features and Aircraft |
| Prepar3D                   | Click            | Selects Prepar3D | Draft Project State | Also resets Features and Aircraft |
| FSX / FS2004               | Click            | Selects FSX      | Draft Project State | Also resets Features and Aircraft |

The selected simulator is visually highlighted.

# Simulator Change Side Effects

Selecting a simulator is not an isolated property change.

Every simulator selection also resets:

```
FSUIPC = false in the local form state
ProSim = false in the local form state

Aircraft
→ reset to simulator default
```

The default Aircraft values are:

```
MSFS
→ Microsoft - Generic

X-Plane
→ Laminar Research - Generic

Prepar3D
→ no selected Aircraft

FSX
→ no selected Aircraft
```

Therefore the GUI interaction:

```
Select another Simulator
```

can simultaneously change several visible form values.

Example:

```
MSFS
FSUIPC enabled
ProSim enabled
custom Aircraft selected

→ click X-Plane

Result:
Simulator = X-Plane
FSUIPC = false
ProSim = false
Aircraft = Laminar Research - Generic
```

This is important when considering form-level Undo semantics.

Even clicking a simulator card after other form values were configured can reset those dependent values.

# Simulator Features

Available feature controls depend on the selected Simulator.

## Microsoft Flight Simulator

MSFS displays:

```
☐ FSUIPC
☐ ProSim
```

| GUI Element | User Interaction | Visible Result                            | Interaction Domain  | Undo Expectation / Notes       |
| ----------- | ---------------- | ----------------------------------------- | ------------------- | ------------------------------ |
| FSUIPC      | Toggle           | Enables/disables FSUIPC for draft Project | Draft Project State | Committed with Create / Update |
| ProSim      | Toggle           | Enables/disables ProSim                   | Draft Project State | Committed with Create / Update |

## Prepar3D

Prepar3D exposes:

```
☐ ProSim
```

FSUIPC is not shown as an optional checkbox because it is treated as a default feature for this Simulator.

When the form is submitted:

```
Prepar3D
→ FSUIPC enabled automatically
```

regardless of the local FSUIPC checkbox state.

## FSX / FS2004

No additional feature controls are displayed.

FSUIPC is treated as a default feature and is enabled in the submitted Project settings.

## X-Plane

No additional feature controls are currently displayed.

A message informs the user that no additional features are available.

# Feature Reset on Simulator Change

Changing the simulator resets manually selected feature checkboxes.

For example:

```
MSFS
FSUIPC = enabled
ProSim = enabled

→ select another Simulator
→ select MSFS again

FSUIPC = disabled
ProSim = disabled
```

The previous feature choices are not restored automatically.

This interaction should be treated as part of the simulator-selection behavior, rather than as separate independent changes.

# Aircraft Selection

Aircraft selection is currently supported for:

```
MSFS
X-Plane
```

For Prepar3D and FSX, the form displays an informational message instead of Aircraft-selection controls.

# Selected Aircraft Summary

When Aircraft selection is supported, the form shows the currently selected Aircraft as badges.

Examples:

```
Microsoft - Generic

Laminar Research - Generic
```

If no Aircraft is selected, the form shows an informational state indicating that no Aircraft filter will be applied.

An Edit button opens the Aircraft Selection drawer.

| GUI Element        | User Interaction | Visible Result        | Interaction Domain           | Undo Expectation / Notes |
| ------------------ | ---------------- | --------------------- | ---------------------------- | ------------------------ |
| Edit Aircraft List | Click            | Opens Aircraft drawer | Navigation within Draft Form | No Project commit        |

# Aircraft Selection Drawer

The Aircraft drawer opens inside the Project Form.

It contains:

```
Selected Aircraft
Available Aircraft
Aircraft Search
Go Back
```

The drawer cannot normally be dismissed through an outside click or Escape.

The user returns to the main Project Form using **Go Back**.

| GUI Element          | User Interaction | Visible Result         | Interaction Domain           | Undo Expectation / Notes         |
| -------------------- | ---------------- | ---------------------- | ---------------------------- | -------------------------------- |
| Go Back              | Click            | Closes Aircraft drawer | Navigation within Draft Form | Aircraft choices remain in draft |
| Click outside drawer | Click            | Drawer remains open    | Protected Modal State        | No state change                  |
| Escape               | Press            | Drawer remains open    | Protected Modal State        | No state change                  |

# Selected Aircraft List

The upper list displays Aircraft already selected for the Project.

Each row shows information such as:

```
Aircraft Name
Vendor
Preset Count
```

| GUI Element                | User Interaction | Visible Result                  | Interaction Domain  | Undo Expectation / Notes          |
| -------------------------- | ---------------- | ------------------------------- | ------------------- | --------------------------------- |
| Selected Aircraft row      | Click            | Removes Aircraft from selection | Draft Project State | Applied only with Create / Update |
| Selected Aircraft checkbox | Click            | Removes Aircraft from selection | Draft Project State | Same semantic action              |
| Selected Aircraft list     | Scroll           | Scrolls selected entries        | View State          | No Project Undo                   |

Removing an Aircraft moves it back into the Available Aircraft list.

If all selected Aircraft are removed, the form informs the user that no Aircraft filtering will be applied and all presets can be available.

# Available Aircraft List

Available Aircraft are derived from the installed HubHop preset data.

Rows display:

```
Aircraft Name
Vendor
Number of Presets
```

Aircraft already selected are excluded from the available list.

| GUI Element             | User Interaction | Visible Result                     | Interaction Domain  | Undo Expectation / Notes   |
| ----------------------- | ---------------- | ---------------------------------- | ------------------- | -------------------------- |
| Available Aircraft row  | Click            | Adds Aircraft to Project selection | Draft Project State | Applied on Create / Update |
| Aircraft checkbox       | Click            | Adds Aircraft to selection         | Draft Project State | Same semantic action       |
| Available Aircraft list | Scroll           | Scrolls Aircraft choices           | View State          | No Project Undo            |

After selection:

```
Available list
→ selected Aircraft removed from this list

Selected list
→ Aircraft added
```

The displayed counts update accordingly.

# Aircraft Search

The Available Aircraft area provides a text search field.

The filter matches both:

```
Aircraft Name
Vendor
```

| GUI Element     | User Interaction | Visible Result                | Interaction Domain     | Undo Expectation / Notes   |
| --------------- | ---------------- | ----------------------------- | ---------------------- | -------------------------- |
| Aircraft Search | Focus            | Starts text editing           | Text Editing           | Ctrl+Z should remain local |
| Aircraft Search | Type             | Filters available Aircraft    | Native Text / UI State | Native text Undo expected  |
| Aircraft Search | Delete / replace | Updates visible Aircraft list | Native Text / UI State | No Project mutation        |

The Aircraft list is sorted consistently by Aircraft Name and Vendor.

If no Aircraft matches the current filter, an empty-state message is shown.

# Create Project Mode

When the form is opened in Create mode, the dialog title is:

```
Create New Project
```

and the primary action is:

```
Create
```

A new form starts with defaults equivalent to:

```
Name = empty
Simulator = MSFS
FSUIPC = false
ProSim = false
Aircraft = Microsoft - Generic
```

## Create

| GUI Element | User Interaction | Visible Result                                             | Interaction Domain | Undo Expectation / Notes      |
| ----------- | ---------------- | ---------------------------------------------------------- | ------------------ | ----------------------------- |
| Create      | Click            | Creates Project with current form values and closes dialog | Project Lifecycle  | Not ordinary Config-item Undo |
| Enter       | Press            | Performs same form submission                              | Project Lifecycle  | Subject to validation         |

After successful creation the application navigates to the Project View (`/config`).

The created Project receives the complete form state:

```
Name
Simulator
Features
Aircraft
```

# Edit Project Mode

When an existing Project is edited, the dialog title is:

```
Edit Project
```

and the primary action is:

```
Update
```

The current Project values initialize the local form state.

The user can change:

```
Project Name
Simulator
FSUIPC / ProSim features
Aircraft
```

Other Project data such as Profiles and Config Items are not directly edited through this form.

## Update

| GUI Element | User Interaction | Visible Result                                | Interaction Domain | Undo Expectation / Notes                         |
| ----------- | ---------------- | --------------------------------------------- | ------------------ | ------------------------------------------------ |
| Update      | Click            | Commits current form values and closes dialog | Project State      | Natural compound Project-settings Undo candidate |
| Enter       | Press            | Performs same form submission                 | Project State      | Subject to validation                            |

The updated settings are applied as one Project Settings operation.

The current backend path also attempts to save the Project immediately after the Project Settings update.

Therefore Edit Project differs from normal unsaved Config Item editing:

```
Update Project Settings
→ apply settings
→ attempt Project save
```

# Cancel and Dialog Closing

The form provides a Cancel button and standard dialog close control.

| GUI Element          | User Interaction | Visible Result                       | Interaction Domain      | Undo Expectation / Notes        |
| -------------------- | ---------------- | ------------------------------------ | ----------------------- | ------------------------------- |
| Cancel               | Click            | Closes form and discards local draft | Local Edit Cancellation | No project Undo                 |
| Close `X`            | Click            | Closes form                          | Local Edit Cancellation | Draft changes are not submitted |
| Click outside dialog | Click            | Dialog remains open                  | Protected Modal State   | Outside interaction prevented   |

The Project Form explicitly prevents outside-click dismissal.

Changes made in the form are only submitted through Create / Update.

# Local Text Undo Context

The Project form introduces additional native text-editing contexts:

```
Project Name
Aircraft Search
```

While those fields have focus, `Ctrl+Z` should normally operate on the current text input.

Example:

```
Project Name:
"My Project Test"

Backspace several characters
Ctrl+Z
→ restore text edit
```

This behavior is distinct from application-level Project Undo.

# Compound Simulator Interaction

The Simulator control deserves special attention for Undo/Redo because one click can alter several dependent draft values.

Conceptually:

```
Simulator change
├── Simulator
├── FSUIPC
├── ProSim
└── Aircraft
```

For example:

```
Before:
Simulator = MSFS
FSUIPC = true
ProSim = true
Aircraft = PMDG 737

User selects X-Plane

After:
Simulator = X-Plane
FSUIPC = false
ProSim = false
Aircraft = Laminar Research - Generic
```

Inside the dialog this is still only a local form-state transition.

If Project-level Undo is based on the final Update action, however, all of these settings would naturally be restored together as part of the previous Project Settings snapshot.

# Create / Edit Project Undo Boundary

Like the Input Config editor, the Project Form has a clear transaction boundary.

Example:

```
Open Edit Project
→ rename Project
→ select different Simulator
→ toggle ProSim
→ change Aircraft selection
→ Update
```

The individual controls modify only the local form state.

The natural Project-level model is therefore:

```
Before:
Project Settings A

Update
→ Project Settings B

Ctrl+Z
→ restore Project Settings A
```

rather than:

```
Ctrl+Z #1 → undo Aircraft
Ctrl+Z #2 → undo ProSim
Ctrl+Z #3 → undo Simulator
Ctrl+Z #4 → undo Project Name
```

while outside the form.

Inside focused text fields, native text Undo remains a separate interaction.

# Create / Edit Project Summary

The Project Form contains these major interaction groups:

- Project Name text editing
- Simulator selection
- FSUIPC feature selection
- ProSim feature selection
- dependent feature resets
- dependent Aircraft resets
- Aircraft drawer navigation
- Aircraft search
- Aircraft add/remove selection
- form scrolling
- validation
- Create
- Update
- Cancel / close

The main Undo/Redo observation is that the form represents another compound draft transaction:

```
Individual GUI controls
→ local form state

Create / Update
→ committed application/project operation
```

Simulator selection is particularly important because a single GUI action can modify multiple dependent form values before the final commit.

# Controller Bindings

The Controller Bindings dialog is used to map Controllers referenced by the current Project to Controllers that are currently connected to MobiFlight.

It can be opened through several GUI entry points, including:

```
Main Menu → Extras → Controller Bindings

Dashboard
→ Current Project → ... → Controller Bindings

Dashboard
→ Controller Binding warning icon

Project View
→ Project ... → Controller Bindings
```

These entry points all open the same Controller Bindings dialog.

# Dialog Structure

The dialog contains:

```
Binding Status Filters

Original Project Controller
        ↕
Binding Status
        ↕
Connected Controller selector

Close
Apply Changes
```

The list of Controller Bindings is scrollable.

Changes made to individual bindings are first stored in local dialog state.

The Project itself is not updated until the user selects **Apply Changes**.

Conceptually:

```
Open Controller Bindings

→ change Binding A
→ change Binding B
→ filter rows
→ search Controllers

Apply Changes
→ commit complete Controller Binding list
```

# Binding Status Filters

The top of the dialog exposes status filters.

Possible filters are:

```
All
Manual bind
Missing
Auto-bind
Match
```

| GUI Element | User Interaction | Visible Result                                     | Interaction Domain | Undo Expectation / Notes |
| ----------- | ---------------- | -------------------------------------------------- | ------------------ | ------------------------ |
| All         | Click            | Shows all Controller Bindings                      | UI / Filter State  | No Project mutation      |
| Manual bind | Click            | Shows bindings originally requiring manual binding | UI / Filter State  | No Project mutation      |
| Missing     | Click            | Shows originally missing Controller bindings       | UI / Filter State  | No Project mutation      |
| Auto-bind   | Click            | Shows automatically matched bindings               | UI / Filter State  | No Project mutation      |
| Match       | Click            | Shows exact Controller matches                     | UI / Filter State  | No Project mutation      |

A filter button is disabled if the original Project bindings do not contain that status.

# Initial Filter

The dialog does not necessarily start with `All`.

Bindings are sorted by status priority:

```
RequiresManualBind
Missing
AutoBind
Match
```

and the initial filter is derived from the first/highest-priority binding in the current list.

This means that when a Project contains problematic bindings, the dialog can initially focus the user on those bindings rather than displaying all rows.

# Filter State Uses Original Binding Status

Filtering intentionally uses the status that the binding had when the dialog was opened.

For example:

```
Original state:
Controller A → RequiresManualBind

User manually binds Controller A
→ draft status becomes Match

Current filter:
Manual bind
```

Controller A can still remain visible under the `Manual bind` filter because the filter classification is based on its **original** status.

This keeps filter categories stable while the user edits bindings.

# Controller Counts

The dialog displays informational counts for:

```
Controllers in Project
Connected Controllers
```

These values are informational and do not provide direct user actions.

# Binding List

Each binding row visually represents:

```
Original Project Controller

        status indicator

Connected / Bound Controller
```

The original Controller contains information such as:

```
Controller Name
Controller Serial
Controller icon/type
```

The right side contains the Controller selector used to change the binding.

# Original Controller

The left side of each row represents the Controller referenced by the Project.

It is informational.

| GUI Element              | User Interaction | Visible Result                           | Interaction Domain | Undo Expectation / Notes |
| ------------------------ | ---------------- | ---------------------------------------- | ------------------ | ------------------------ |
| Original Controller      | View             | Displays Controller name and serial      | Information        | No Undo                  |
| Original Controller icon | Hover            | Can expose Controller status information | Information        | No Project mutation      |

The Original Controller itself is not edited through this dialog.

# Binding Status Indicator

A status indicator appears between the original and connected Controllers.

It displays a connected or disconnected visual state.

Possible binding statuses are:

```
Match
AutoBind
Missing
RequiresManualBind
```

| GUI Element      | User Interaction | Visible Result                                           | Interaction Domain | Undo Expectation / Notes |
| ---------------- | ---------------- | -------------------------------------------------------- | ------------------ | ------------------------ |
| Status indicator | Hover            | Shows whether Controller is bound and its binding status | Information        | No Project mutation      |

A bound Controller is represented with a check-style indicator.

An unbound Controller is represented with a dashed-circle indicator.

# Bound Controller Selector

The right side of each binding row contains a combobox-style Controller selector.

If a Controller is currently assigned, it displays:

```
Controller Name
Controller Serial
Controller icon
```

If no Controller is assigned, it displays a prompt to select a Controller.

## Open Controller Selector

| GUI Element               | User Interaction | Visible Result                  | Interaction Domain      | Undo Expectation / Notes |
| ------------------------- | ---------------- | ------------------------------- | ----------------------- | ------------------------ |
| Bound Controller selector | Click            | Opens connected Controller list | Selection / Draft State | No Project commit yet    |

The selector opens as a searchable popover.

# Controller Search

The Controller selector contains a text search field.

| GUI Element       | User Interaction   | Visible Result                          | Interaction Domain     | Undo Expectation / Notes                                      |
| ----------------- | ------------------ | --------------------------------------- | ---------------------- | ------------------------------------------------------------- |
| Search Controller | Focus              | Starts local text-editing context       | Text Editing           | Ctrl+Z should remain local                                    |
| Search Controller | Type               | Filters available Controller options    | Native Text / UI State | No Project mutation                                           |
| Search Controller | Backspace / Delete | Changes search query                    | Native Text / UI State | Native text Undo expected                                     |
| Search Controller | Escape             | Closes the Controller-selection popover | Local UI Cancellation  | Does not close the entire dialog while popover handles Escape |

The search field explicitly captures normal editing keys so that actions such as Backspace affect its text rather than the underlying Config Item table.

If no Controller matches the current search, an empty-result message is shown.

# Select Connected Controller

Every currently connected Controller is presented as a selectable option.

| GUI Element       | User Interaction | Visible Result                                             | Interaction Domain  | Undo Expectation / Notes          |
| ----------------- | ---------------- | ---------------------------------------------------------- | ------------------- | --------------------------------- |
| Controller option | Click            | Assigns selected Controller to original Project Controller | Draft Project State | Committed only with Apply Changes |

After selecting another Controller:

```
BoundController
→ selected connected Controller

Status
→ Match
```

The selector closes automatically after selection.

The selected Controller is immediately shown in the row, but the Project has not yet been updated.

# Change Existing Binding

A binding that already points to a Controller can be changed by reopening the selector and choosing a different Controller.

Example:

```
Original:
Alpha Flight Controls
→ Alpha Flight Controls

User selects:
Alpha Flight Controls Lite

Draft result:
Alpha Flight Controls
→ Alpha Flight Controls Lite
```

This is a local dialog-state change until **Apply Changes** is selected.

# Remove / Clear a Binding

There is no separate visible `Remove Binding` button.

Instead, selecting the Controller that is already selected toggles that selection off.

Conceptually:

```
Current selection:
Controller A

Open selector
→ select Controller A again

Result:
Bound Controller = none
Status = Missing
```

| GUI Element                          | User Interaction | Visible Result          | Interaction Domain  | Undo Expectation / Notes          |
| ------------------------------------ | ---------------- | ----------------------- | ------------------- | --------------------------------- |
| Currently selected Controller option | Select again     | Removes current binding | Draft Project State | Committed only with Apply Changes |

This interaction is less visually explicit than a dedicated Remove button and should be manually verified from a usability perspective.

# Scroll Binding List

The Controller Binding list is displayed in a scrollable area.

| GUI Element  | User Interaction | Visible Result                          | Interaction Domain | Undo Expectation / Notes |
| ------------ | ---------------- | --------------------------------------- | ------------------ | ------------------------ |
| Binding list | Scroll           | Changes visible Controller Binding rows | View State         | No Undo                  |

# Automatic Bound-State Summary

If every binding in the draft list has a status of either:

```
Match
AutoBind
```

the dialog displays an `All Set` success indicator.

This is informational.

Changing bindings can therefore make the indicator appear or disappear before the changes have been committed.

# Close

The dialog contains a **Close** button rather than a Cancel button.

| GUI Element          | User Interaction | Visible Result                                                                       | Interaction Domain      | Undo Expectation / Notes                  |
| -------------------- | ---------------- | ------------------------------------------------------------------------------------ | ----------------------- | ----------------------------------------- |
| Close                | Click            | Closes dialog without publishing draft Controller Bindings                           | Local Edit Cancellation | No Project Undo                           |
| Dialog `X`           | Click            | Closes dialog without Apply                                                          | Local Edit Cancellation | Draft changes are discarded               |
| Escape               | Press            | Closes dialog through standard dialog behavior when no child popover consumes Escape | Local Edit Cancellation | Draft changes are not applied             |
| Click outside dialog | Click            | Dialog remains open                                                                  | Protected Modal State   | Outside dismissal is explicitly prevented |

Therefore:

```
change Controller binding
→ Close
```

does not update the Project.

# Apply Changes

The dialog contains an **Apply Changes** button.

| GUI Element   | User Interaction | Visible Result                                                  | Interaction Domain | Undo Expectation / Notes        |
| ------------- | ---------------- | --------------------------------------------------------------- | ------------------ | ------------------------------- |
| Apply Changes | Click            | Sends complete edited Controller Binding list and closes dialog | Project State      | Natural compound Undo candidate |

The frontend sends the complete draft list rather than only the individual binding that was changed.

Conceptually:

```
Before:
Binding Set A

Dialog edits:
A → B
C → D
E → unbound

Apply Changes

After:
Binding Set B
```

The backend updates the Project Controller Bindings, publishes the updated Project, and marks the Project as changed.

# Apply Without Changes

The Apply Changes button is available even if the user has not actually modified a binding.

There is currently no frontend no-change check before sending the complete binding list.

Therefore:

```
Open Controller Bindings
→ change nothing
→ Apply Changes
```

still sends a Controller Bindings update to the backend.

The backend currently treats that update as a Project change.

This behavior should be considered when deciding whether a history entry should only be created after a semantic state difference has been detected.

# Multiple Binding Changes

The dialog allows several bindings to be changed before Apply.

Example:

```
Controller A
→ bind to Controller X

Controller B
→ bind to Controller Y

Controller C
→ clear binding

Apply Changes
```

All three changes are submitted as one complete binding set.

From the user's perspective, this strongly suggests one compound dialog transaction rather than three independent project-level actions.

# Controller Binding Undo Boundary

The natural Undo boundary is therefore:

```
Open Controller Bindings

→ modify one or more bindings
→ search/filter Controllers
→ inspect statuses
→ Apply Changes

Project Binding Set A
→ Project Binding Set B
```

A project-level Undo could conceptually restore:

```
ControllerBindings before Apply
```

and Redo could restore:

```
ControllerBindings after Apply
```

The local operations inside the dialog should remain separate from global Project Undo:

```
Search text
Filter selection
Open/close Controller popover
Scroll
```

do not change the Project.

# Native Text Undo Context

The Controller search field introduces another local text-editing context.

Example:

```
Search Controller:
"Alpha"

Backspace
→ "Alph"

Ctrl+Z
→ expected local text restoration
```

A global Project-level Ctrl+Z should therefore not override native text Undo while this field has focus.

# Filter State vs Binding State

The dialog contains two independent kinds of state:

```
Filter State
→ which original-status rows are visible

Binding Draft State
→ which connected Controller each row is assigned to
```

For example:

```
Manual bind filter active

Controller A originally RequiresManualBind
→ user binds Controller A
→ draft status becomes Match

Controller A remains inside Manual bind filter
```

because the filter continues to use the original status.

This is a deliberate distinction between view-state filtering and the draft Project mutation.

# Controller Bindings Summary

The Controller Bindings dialog contains these major interaction groups:

- status filtering
- binding-list scrolling
- Controller selector opening
- Controller searching
- Controller selection
- Controller reassignment
- Controller unbinding
- binding-status inspection
- Close / local cancellation
- Apply Changes

The central Undo/Redo observation is:

```
Individual binding selections
→ local dialog draft

Apply Changes
→ one complete ControllerBindings Project mutation
```

while:

```
Close / X / Escape
→ discard draft changes
```

and:

```
Filter / Search / Scroll
→ local UI state only
```

This makes Controller Bindings another clear compound form-edit transaction, similar to Input Config and Project Settings.

# Application Settings (Global Settings)

Application Settings are opened from:

```
Main Menu
→ Extras
→ Settings
```

The Settings interface is a native WinForms dialog.

Unlike Project Settings, the values configured here generally belong to the application rather than to the currently opened Project.

The main tabs are:

```
General
MobiFlight
Peripherals
ProSim
```

A legacy Arcaze tab can also exist when the application is built with Arcaze support enabled.

# Application Settings Interaction Model

The Settings dialog contains several different kinds of state.

```
Application preference state
→ General / Peripherals / ProSim / some MobiFlight options
→ normally persisted when OK is selected

MobiFlight module configuration
→ edited inside the module/device tree
→ separate from normal application preferences

Runtime / hardware actions
→ firmware update
→ configuration upload
→ board reset
→ serial regeneration
→ manager reconnects
```

These categories should not be treated as one Undo domain.

# Settings Tabs

| GUI Element     | User Interaction | Visible Result                                   | Interaction Domain             | Undo Expectation / Notes  |
| --------------- | ---------------- | ------------------------------------------------ | ------------------------------ | ------------------------- |
| General tab     | Click            | Shows general application preferences            | Navigation / Global Settings   | No Project Undo           |
| MobiFlight tab  | Click            | Shows connected modules and module configuration | Navigation / Hardware Settings | No Project Undo           |
| Peripherals tab | Click            | Shows Joystick and MIDI settings                 | Navigation / Global Settings   | No Project Undo           |
| ProSim tab      | Click            | Shows ProSim connection settings                 | Navigation / Global Settings   | No Project Undo           |
| Arcaze tab      | Click            | Shows legacy Arcaze configuration                | Navigation / Hardware Settings | Conditional build feature |

Switching tabs does not itself persist configuration.

# Dialog OK

The **OK** button validates the dialog and saves normal Settings values from all Settings panels.

Conceptually:

```
General draft
MobiFlight preference draft
Peripheral draft
ProSim draft
        ↓
       OK
        ↓
Properties.Settings.Default.Save()
```

| GUI Element        | User Interaction | Visible Result                           | Interaction Domain       | Undo Expectation / Notes |
| ------------------ | ---------------- | ---------------------------------------- | ------------------------ | ------------------------ |
| OK                 | Click            | Validates and saves application Settings | Global Application State | Not part of Project Undo |
| Validation failure | Click OK         | Dialog remains open                      | Validation               | No committed change      |

Saving Settings also triggers application-level side effects such as applying logging settings and publishing updated Settings state.

The ProSim connection state is reset after Settings are saved.

# Dialog Cancel

| GUI Element  | User Interaction | Visible Result                                     | Interaction Domain       | Undo Expectation / Notes             |
| ------------ | ---------------- | -------------------------------------------------- | ------------------------ | ------------------------------------ |
| Cancel       | Click            | Closes dialog without calling normal Settings save | Local Edit Cancellation  | No Project Undo                      |
| Escape       | Press            | Activates Cancel behavior                          | Local Edit Cancellation  | No Project Undo                      |
| Window close | Click            | Closes Settings dialog                             | Local / Dialog Lifecycle | Should be treated separately from OK |

Normal preference changes made in General, Peripherals, and ProSim are not saved through the normal Settings save path when Cancel is used.

MobiFlight module configuration is more complicated because some actions in that tab can affect hardware immediately and therefore cannot be cancelled by closing the Settings dialog.

# Unsaved MobiFlight Module Configuration Warning

If the MobiFlight module configuration tree has been changed, both OK and Cancel can display a warning before the dialog closes.

If the user cancels the warning:

```
warning
→ Cancel
→ return to MobiFlight tab
```

The Settings dialog remains open so that the module configuration can be reviewed.

This is separate from normal application preference validation.

# General Tab

The General tab controls application-wide behavior.

## Recent Projects Count

| GUI Element          | User Interaction     | Visible Result                                     | Interaction Domain    | Undo Expectation / Notes |
| -------------------- | -------------------- | -------------------------------------------------- | --------------------- | ------------------------ |
| Recent Files maximum | Change numeric value | Changes maximum number of recent Projects retained | Global Settings Draft | Saved on OK              |

# Test Mode Speed

A trackbar controls the interval used by Test Mode.

The available internal intervals correspond approximately to:

```
1000 ms
500 ms
250 ms
125 ms
50 ms
```

| GUI Element            | User Interaction       | Visible Result                  | Interaction Domain    | Undo Expectation / Notes |
| ---------------------- | ---------------------- | ------------------------------- | --------------------- | ------------------------ |
| Test Mode Speed slider | Drag / keyboard change | Changes test execution interval | Global Settings Draft | Saved on OK              |

# Config Execution Speed

A second slider controls the application's polling/execution interval.

| GUI Element            | User Interaction       | Visible Result                      | Interaction Domain    | Undo Expectation / Notes |
| ---------------------- | ---------------------- | ----------------------------------- | --------------------- | ------------------------ |
| Execution Speed slider | Drag / keyboard change | Changes configuration polling speed | Global Settings Draft | Saved on OK              |

# Logging

The General tab contains logging controls.

| GUI Element       | User Interaction | Visible Result                         | Interaction Domain    | Undo Expectation / Notes |
| ----------------- | ---------------- | -------------------------------------- | --------------------- | ------------------------ |
| Enable Log        | Toggle           | Enables/disables log-panel setting     | Global Settings Draft | Saved on OK              |
| Log Level         | Select           | Changes logging severity threshold     | Global Settings Draft | Saved on OK              |
| Log Joystick Axis | Toggle           | Enables/disables joystick-axis logging | Global Settings Draft | Saved on OK              |

Available log severity choices are derived from the application's
`LogSeverity` values.

After Settings are saved, logging configuration is applied to the running application.

# Language

The Language combobox currently includes choices such as:

```
System Default
English
Deutsch
Español
Suomi
Português
Русский
日本語
한국어
```

| GUI Element | User Interaction | Visible Result                          | Interaction Domain    | Undo Expectation / Notes |
| ----------- | ---------------- | --------------------------------------- | --------------------- | ------------------------ |
| Language    | Select           | Changes configured application language | Global Settings Draft | Saved on OK              |

# Update Preferences

| GUI Element       | User Interaction | Visible Result                                 | Interaction Domain    | Undo Expectation / Notes |
| ----------------- | ---------------- | ---------------------------------------------- | --------------------- | ------------------------ |
| Beta Updates      | Toggle           | Enables/disables beta update preference        | Global Settings Draft | Saved on OK              |
| HubHop Auto Check | Toggle           | Enables/disables automatic HubHop preset check | Global Settings Draft | Saved on OK              |

# Community Feedback

| GUI Element        | User Interaction | Visible Result                                  | Interaction Domain    | Undo Expectation / Notes |
| ------------------ | ---------------- | ----------------------------------------------- | --------------------- | ------------------------ |
| Community Feedback | Toggle           | Changes telemetry/community-feedback preference | Global Settings Draft | Saved on OK              |

# Runtime Preferences

| GUI Element         | User Interaction | Visible Result                                | Interaction Domain    | Undo Expectation / Notes |
| ------------------- | ---------------- | --------------------------------------------- | --------------------- | ------------------------ |
| Auto Retrigger      | Toggle           | Changes automatic retrigger behavior          | Global Settings Draft | Saved on OK              |
| Minimize on AutoRun | Toggle           | Changes minimize behavior when AutoRun starts | Global Settings Draft | Saved on OK              |

# General Tab Undo Context

Most General-tab controls behave as a single Settings transaction:

```
change language
→ change logging
→ change Test speed
→ enable beta updates
→ OK
```

The individual controls change local WinForms state first.

The normal persistence boundary is **OK**.

These settings are application-wide and should normally not participate in a Project-specific Undo history.

# MobiFlight Tab

The MobiFlight tab is fundamentally different from the General tab.

It contains both:

```
Application preferences
```

and:

```
Connected hardware configuration
```

The tab displays a tree of detected MobiFlight-compatible modules and their configured devices.

# MobiFlight Application Preferences

## Firmware Auto Update Check

| GUI Element          | User Interaction | Visible Result                                     | Interaction Domain    | Undo Expectation / Notes |
| -------------------- | ---------------- | -------------------------------------------------- | --------------------- | ------------------------ |
| Firmware Auto Update | Toggle           | Changes automatic firmware-update check preference | Global Settings Draft | Saved on OK              |

## Ignore COM Ports

| GUI Element          | User Interaction | Visible Result                                  | Interaction Domain         | Undo Expectation / Notes |
| -------------------- | ---------------- | ----------------------------------------------- | -------------------------- | ------------------------ |
| Ignore COM Ports     | Toggle           | Enables/disables ignored COM-port configuration | Global Settings Draft      | Saved on OK              |
| Ignored COM Ports    | Type             | Changes comma-separated ignored-port list       | Native Text / Global Draft | Ctrl+Z should be local   |
| Ignore COM Ports off | Toggle           | Disables ignored-port text input                | UI + Draft State           | Saved on OK              |

Changing the ignored-port configuration can display a restart-required hint.

# Module Tree

Detected controllers/modules appear in a TreeView.

A module can contain device children such as:

```
Output
LED / 7-Segment
Servo
Stepper
LCD Display
Shift Register

Button
Encoder
Analog Input
Input Shift Register
Input Multiplexer
Custom Device
```

## Tree Navigation

| GUI Element | User Interaction  | Visible Result                                  | Interaction Domain                 | Undo Expectation / Notes     |
| ----------- | ----------------- | ----------------------------------------------- | ---------------------------------- | ---------------------------- |
| Module node | Click             | Selects module and shows module settings        | Hardware Configuration / Selection | Selection itself is not Undo |
| Device node | Click             | Selects device and shows device-specific editor | Hardware Configuration / Selection | Selection itself is not Undo |
| Module node | Expand / collapse | Shows/hides configured devices                  | View State                         | No Undo                      |
| Tree node   | Right-click       | Selects node and exposes context actions        | Selection / Menu                   | No Project Undo              |

Available toolbar and context-menu actions change depending on the selected node and board type.

# Add Module Device

A selected MobiFlight board can expose an Add Device menu.

Possible device types include:

```
Output
LED / 7-Segment
Servo
Stepper
LCD Display
Shift Register
Button
Encoder
Analog Input
Input Shift Register
Input Multiplexer
Custom Device
```

| GUI Element | User Interaction | Visible Result                           | Interaction Domain         | Undo Expectation / Notes       |
| ----------- | ---------------- | ---------------------------------------- | -------------------------- | ------------------------------ |
| Add Device  | Click            | Opens device-type menu                   | Menu                       | No immediate hardware write    |
| Device Type | Click            | Adds device to module configuration tree | Module Configuration Draft | Requires later hardware upload |

New devices are assigned generated unique names and suitable default pin settings where possible.

The module is visually marked as changed.

# Remove Module Device

| GUI Element   | User Interaction | Visible Result                           | Interaction Domain         | Undo Expectation / Notes              |
| ------------- | ---------------- | ---------------------------------------- | -------------------------- | ------------------------------------- |
| Remove Device | Click            | Removes selected device from module tree | Module Configuration Draft | Not yet necessarily uploaded to board |

The action is unavailable when a module node rather than a device node is selected.

# Module-Level Editor

Selecting the module itself opens a Module panel.

The panel can include:

| GUI Element           | User Interaction | Visible Result                                                    | Interaction Domain         | Undo Expectation / Notes |
| --------------------- | ---------------- | ----------------------------------------------------------------- | -------------------------- | ------------------------ |
| Module Name           | Type             | Changes configured module name                                    | Native Text / Module Draft | Local text Undo          |
| Website               | Click            | Opens board/community website                                     | External                   | No Undo                  |
| Documentation         | Click            | Opens board documentation                                         | External                   | No Undo                  |
| Support               | Click            | Opens board support page                                          | External                   | No Undo                  |
| Upload Default Config | Click            | Loads selected default board configuration and starts upload flow | Hardware / External        | Not normal Undo          |

Available external links depend on board definition metadata.

# Output Device Configuration

Selecting an Output device exposes controls such as:

```
Device Name
Pin
```

| GUI Element | User Interaction | Visible Result              | Interaction Domain         | Undo Expectation / Notes        |
| ----------- | ---------------- | --------------------------- | -------------------------- | ------------------------------- |
| Device Name | Type             | Renames configured output   | Native Text / Module Draft | Local Undo                      |
| Pin         | Select           | Changes assigned output pin | Module Configuration Draft | Applied to board through upload |

# Button Device Configuration

A Button device exposes:

```
Device Name
Pin
```

Changing these values marks the module configuration as changed.

# Analog Input Configuration

An Analog Input device exposes:

```
Device Name
Pin
Sensitivity
```

| GUI Element | User Interaction | Visible Result             | Interaction Domain         | Undo Expectation / Notes |
| ----------- | ---------------- | -------------------------- | -------------------------- | ------------------------ |
| Device Name | Type             | Renames Analog Input       | Native Text / Module Draft | Local Undo               |
| Pin         | Select           | Changes analog pin         | Module Configuration Draft | Applied through upload   |
| Sensitivity | Move slider      | Changes analog sensitivity | Module Configuration Draft | Applied through upload   |

# Encoder Configuration

Encoder configuration exposes:

```
Device Name
Encoder Type
Left Pin
Right Pin
Swap Pins
```

| GUI Element  | User Interaction | Visible Result                         | Interaction Domain         | Undo Expectation / Notes |
| ------------ | ---------------- | -------------------------------------- | -------------------------- | ------------------------ |
| Device Name  | Type             | Renames Encoder                        | Native Text / Module Draft | Local Undo               |
| Encoder Type | Select           | Changes encoder type                   | Module Configuration Draft | Applied through upload   |
| Left Pin     | Select           | Changes first pin                      | Module Configuration Draft | Applied through upload   |
| Right Pin    | Select           | Changes second pin                     | Module Configuration Draft | Applied through upload   |
| Swap         | Click            | Exchanges left/right pin configuration | Module Configuration Draft | Local module edit        |

# Servo Configuration

A Servo device exposes:

```
Device Name
Pin
```

Changes remain part of the module configuration until uploaded.

# LED / 7-Segment Configuration

The module-device editor exposes settings such as:

```
Device Name
Display Type
Number of Modules
Data Pin
Clock Pin
Latch Pin
Intensity
```

| GUI Element       | User Interaction | Visible Result                              | Interaction Domain         | Undo Expectation / Notes |
| ----------------- | ---------------- | ------------------------------------------- | -------------------------- | ------------------------ |
| Device Name       | Type             | Renames LED module                          | Native Text / Module Draft | Local Undo               |
| Display Type      | Select           | Changes display-driver type where supported | Module Configuration Draft | Applied through upload   |
| Number of Modules | Select           | Changes chained module count                | Module Configuration Draft | Applied through upload   |
| Pin selectors     | Select           | Changes hardware pins                       | Module Configuration Draft | Applied through upload   |
| Intensity         | Move slider      | Changes configured intensity                | Module Configuration Draft | Applied through upload   |

# LCD Configuration

LCD configuration exposes:

```
Name
I2C Address
Columns
Lines
```

| GUI Element | User Interaction | Visible Result          | Interaction Domain         | Undo Expectation / Notes |
| ----------- | ---------------- | ----------------------- | -------------------------- | ------------------------ |
| Name        | Type             | Renames LCD device      | Native Text / Module Draft | Local Undo               |
| Address     | Select           | Changes display address | Module Configuration Draft | Applied through upload   |
| Columns     | Type             | Changes LCD dimensions  | Module Configuration Draft | Applied through upload   |
| Lines       | Type             | Changes LCD dimensions  | Module Configuration Draft | Applied through upload   |

# Stepper Configuration

Stepper module configuration exposes controls including:

```
Device Name
Pin 1
Pin 2
Pin 3
Pin 4
Button / Zero Pin
Mode
Preset
Auto Zero
Deactivate
Backlash
```

| GUI Element | User Interaction | Visible Result                  | Interaction Domain         | Undo Expectation / Notes      |
| ----------- | ---------------- | ------------------------------- | -------------------------- | ----------------------------- |
| Name        | Type             | Renames Stepper                 | Native Text / Module Draft | Local Undo                    |
| Motor Pins  | Select           | Changes Stepper pin assignments | Module Configuration Draft | Applied through upload        |
| Button Pin  | Select           | Changes zero/reference input    | Module Configuration Draft | Applied through upload        |
| Mode        | Select           | Changes Stepper mode            | Module Configuration Draft | May update dependent controls |
| Preset      | Select           | Loads Stepper preset values     | Module Configuration Draft | Compound local change         |
| Auto Zero   | Toggle           | Changes automatic zero behavior | Module Configuration Draft | Applied through upload        |
| Deactivate  | Toggle           | Changes deactivation behavior   | Module Configuration Draft | Applied through upload        |
| Backlash    | Type             | Changes backlash setting        | Native Text / Module Draft | Applied through upload        |

# Shift Register Configuration

A Shift Register device exposes:

```
Device Name
Number of Modules
Pin 1
Pin 2
Pin 3
```

Changing these controls modifies the module configuration draft.

# Input Shift Register Configuration

Input Shift Register settings similarly expose:

```
Device Name
Number of Modules
multiple pin selectors
```

These settings become physical board configuration only after upload.

# Input Multiplexer Configuration

The Input Multiplexer editor exposes controls such as:

```
Device Name
number of modules
pin selection
```

It can also participate in multiplexer-driver ordering/association logic.

Those changes are module configuration rather than Project configuration.

# Custom Device Configuration

Custom devices can expose:

```
Device Name
Additional Configuration
device-specific Pins
```

The exact available fields depend on the selected custom-device definition.

# Open Module Configuration

The MobiFlight toolbar/context menu contains an Open action.

| GUI Element | User Interaction | Visible Result                            | Interaction Domain          | Undo Expectation / Notes        |
| ----------- | ---------------- | ----------------------------------------- | --------------------------- | ------------------------------- |
| Open        | Click            | Opens module-configuration file selection | Module Configuration / File | Loads configuration into editor |

Loading a configuration can replace the visible module-device tree.

Compatibility warnings can be shown if the configuration does not match the current board.

# Save Module Configuration

| GUI Element | User Interaction | Visible Result                               | Interaction Domain | Undo Expectation / Notes       |
| ----------- | ---------------- | -------------------------------------------- | ------------------ | ------------------------------ |
| Save        | Click            | Saves current module configuration to a file | File / Export      | Does not itself change Project |

This is distinct from uploading the configuration to the physical board.

# Upload Configuration

The **Upload** action has immediate external effects.

The flow includes:

```
Upload
→ confirmation dialog
→ serialize current module configuration
→ write configuration to board
→ reset board
→ reload board configuration
```

| GUI Element                | User Interaction | Visible Result                    | Interaction Domain   | Undo Expectation / Notes             |
| -------------------------- | ---------------- | --------------------------------- | -------------------- | ------------------------------------ |
| Upload                     | Click            | Opens upload confirmation         | Hardware / External  | Not normal application Undo          |
| Upload confirmation OK     | Click            | Writes current config to hardware | Hardware Side Effect | Cannot be treated as ordinary Ctrl+Z |
| Upload confirmation Cancel | Click            | Cancels hardware upload           | Local Cancellation   | No external change                   |

A progress dialog is displayed while uploading/resetting/reloading.

This action is fundamentally different from changing a Settings checkbox.

# Firmware Update

Firmware Update is available through toolbar/context actions depending on the selected board.

For unknown/unflashed boards, multiple compatible firmware/board definitions can be offered.

| GUI Element           | User Interaction | Visible Result              | Interaction Domain  | Undo Expectation / Notes |
| --------------------- | ---------------- | --------------------------- | ------------------- | ------------------------ |
| Update Firmware       | Click            | Starts firmware-update flow | Hardware / External | Not application Undo     |
| Firmware/Board choice | Select           | Chooses firmware target     | Hardware / External | Leads to flash operation |

During firmware updates the normal auto-connect and module scanning processes are paused and later resumed.

A firmware update cannot be reversed through ordinary application Undo.

# Reset Board

Where supported:

| GUI Element   | User Interaction | Visible Result        | Interaction Domain   | Undo Expectation / Notes     |
| ------------- | ---------------- | --------------------- | -------------------- | ---------------------------- |
| Reset Board   | Click            | Opens confirmation    | Hardware / Runtime   | No normal Undo               |
| Confirm Reset | Click            | Resets selected board | Hardware Side Effect | Immediate external operation |
| Cancel Reset  | Click            | Cancels reset         | Local Cancellation   | No side effect               |

# Regenerate Serial

| GUI Element       | User Interaction | Visible Result                                             | Interaction Domain | Undo Expectation / Notes |
| ----------------- | ---------------- | ---------------------------------------------------------- | ------------------ | ------------------------ |
| Regenerate Serial | Click            | Generates a new hardware serial where firmware supports it | Hardware State     | Not normal Undo          |

After serial regeneration the displayed module information is refreshed.

# Reload Configuration

| GUI Element   | User Interaction | Visible Result                             | Interaction Domain     | Undo Expectation / Notes             |
| ------------- | ---------------- | ------------------------------------------ | ---------------------- | ------------------------------------ |
| Reload Config | Click            | Reloads configuration from selected module | Hardware → Local State | Replaces current module editor state |

This is not equivalent to project Undo because the source of truth is the physical device.

# Ignore / Do Not Ignore COM Port

Module context actions allow the module's COM port to be added to or removed from the ignored-port list.

| GUI Element            | User Interaction | Visible Result                               | Interaction Domain    | Undo Expectation / Notes |
| ---------------------- | ---------------- | -------------------------------------------- | --------------------- | ------------------------ |
| Ignore COM Port        | Click            | Adds selected port to ignored-port text/list | Global Settings Draft | Restart hint displayed   |
| Do Not Ignore COM Port | Click            | Removes selected port from ignored list      | Global Settings Draft | Restart hint displayed   |

These actions also update the Ignore COM Ports checkbox depending on whether the ignored list is empty.

# MobiFlight Module Editing vs Hardware Commit

The module editor therefore has its own important boundary:

```
Add device
Rename device
Change pins
Remove device
        ↓
local module configuration marked "Changed"
```

versus:

```text
Upload
        ↓
physical controller configuration changed
```

`Ctrl+Z` at application level should not be assumed to reverse an already uploaded hardware configuration.

# Peripherals Tab

The Peripherals tab controls Joystick and MIDI support.

It contains:

```
Enable Joystick Support
Enable MIDI Support

Joystick list
MIDI Board list
```

# Joystick Support

| GUI Element             | User Interaction | Visible Result                                      | Interaction Domain    | Undo Expectation / Notes |
| ----------------------- | ---------------- | --------------------------------------------------- | --------------------- | ------------------------ |
| Enable Joystick Support | Toggle           | Enables/disables Joystick support in Settings draft | Global Settings Draft | Applied on OK            |
| Joystick checkbox       | Toggle           | Includes/excludes individual Joystick               | Global Settings Draft | Applied on OK            |
| Joystick list           | Scroll / select  | Navigates available Joysticks                       | View / Draft State    | No Project Undo          |

Checked devices are treated as enabled.

Unchecked devices are placed in the exclusion list.

# MIDI Support

| GUI Element         | User Interaction | Visible Result                          | Interaction Domain    | Undo Expectation / Notes |
| ------------------- | ---------------- | --------------------------------------- | --------------------- | ------------------------ |
| Enable MIDI Support | Toggle           | Enables/disables MIDI support           | Global Settings Draft | Applied on OK            |
| MIDI Board checkbox | Toggle           | Includes/excludes individual MIDI board | Global Settings Draft | Applied on OK            |

# Peripheral Runtime Side Effects on OK

Peripheral changes are not only passive preferences.

When the Settings dialog is saved:

```
Disable Joystick support
→ Joystick manager Shutdown

Enable Joystick / change exclusion
→ Shutdown
→ Connect

Disable MIDI support
→ MIDI manager Shutdown

Enable MIDI / change exclusion
→ Shutdown
→ Connect
```

Therefore:

```
toggle checkbox inside dialog
```

is draft state, while:

```
OK
```

can immediately cause runtime device disconnection/reconnection.

This is another reason these Settings should not be modeled as ordinary Project Undo actions.

# ProSim Tab

The ProSim tab exposes connection settings.

| GUI Element            | User Interaction     | Visible Result                       | Interaction Domain         | Undo Expectation / Notes |
| ---------------------- | -------------------- | ------------------------------------ | -------------------------- | ------------------------ |
| Host                   | Type                 | Changes ProSim host                  | Native Text / Global Draft | Local Ctrl+Z             |
| Port                   | Type                 | Changes ProSim port                  | Native Text / Global Draft | Saved on OK              |
| Auto Connect           | Toggle               | Enables/disables ProSim auto-connect | Global Settings Draft      | Saved on OK              |
| Maximum Retry Attempts | Change numeric value | Changes retry limit                  | Global Settings Draft      | Saved on OK              |

If the Port text cannot be parsed as an integer, the existing saved port value is retained.

# ProSim Connection Reset

After Settings are saved, the application calls:

```
ResetProSimConnectionState()
```

This occurs as part of the Settings save process.

Therefore even though the ProSim fields are draft controls until OK, confirming the Settings dialog can also affect active connection state.

This runtime effect should be distinguished from persistence of the text and checkbox values themselves.

# OK Without Semantic Changes

The Settings dialog does not perform a complete semantic no-op check before saving.

Selecting OK causes the Settings save process to run for all panels.

This includes actions such as:

```
Properties.Settings.Default.Save()
Apply log settings
publish updated Settings
reset ProSim connection state
```

Therefore:

```
Open Settings
→ change nothing
→ OK
```

can still execute application-level save/refresh side effects.

This should be considered before generating any future Undo history entry for Settings.

# Native Text Undo Context

The Settings interface contains multiple native WinForms text fields, including:

```
Ignored COM Ports
ProSim Host
ProSim Port
Module Name
Device Names
Stepper numeric values
LCD dimensions
Custom Device configuration
```

When these controls have focus, Ctrl+Z should normally remain a local native text-editing operation.

This should not trigger Project-level Undo.

# Application Settings Undo Boundary

For ordinary preferences, the user interaction model is:

```
Open Settings

→ change General setting
→ change Peripheral setting
→ change ProSim setting

OK
→ persist complete Settings selection
```

If an application-level Settings Undo mechanism were ever introduced, a natural transaction would therefore be the complete Settings state before and after OK.

However this should remain separate from Project Undo because these values are global to the application.

# Important Exception: Hardware Actions

The MobiFlight tab breaks the simple draft/OK model.

Actions such as:

```
Upload Configuration
Update Firmware
Reset Board
Regenerate Serial
Reload Config
```

can act on hardware immediately.

For example:

```
Settings
→ MobiFlight
→ Upload
→ Confirm

physical board changes immediately
```

Closing the Settings dialog afterward does not conceptually reverse that external side effect.

Therefore the GUI inventory must distinguish:

```
Application setting
Module configuration draft
Hardware command
```

rather than treating every interaction inside Settings as the same type of action.

# Conditional Arcaze Settings

When MobiFlight is compiled without Arcaze support, the Arcaze tab is removed from the Settings dialog.

If Arcaze support is enabled, an additional legacy hardware-configuration tab is exposed.

Because it is conditional and legacy-specific, it should be cataloged separately if the target runtime build actually exposes it.

# Application Settings Summary

The Application Settings dialog contains four major interaction groups:

```
General application preferences
MobiFlight hardware and module configuration
Joystick / MIDI peripheral preferences
ProSim connection preferences
```

The most important Undo/Redo distinction is:

```
General / Peripheral / ProSim control change
→ local Settings draft
→ OK
→ persistent application setting
```

while:

```
MobiFlight device tree edit
→ local module configuration

Upload
→ physical board state changes
```

and:

```
Firmware Update / Reset / Regenerate Serial
→ immediate external hardware operation
```

These should not share the same Project Undo history.

Application Settings are therefore primarily a **Global/Application State**domain, while MobiFlight hardware actions belong to a separate **Runtime / External Hardware** domain.

# Merge Profiles from Existing Project

The Project View allows the user to import Profiles from another existing MobiFlight Project.

The interaction is available through:

```
Profile Tabs
→ +
→ From existing project
```

The `+` menu therefore contains two distinct Project-state operations:

```
New profile

From existing project
```

# Start Merge

| GUI Element           | User Interaction | Visible Result                     | Interaction Domain | Undo Expectation / Notes |
| --------------------- | ---------------- | ---------------------------------- | ------------------ | ------------------------ |
| `+` Profile button    | Click            | Opens Profile creation menu        | Menu               | No Project mutation      |
| From existing project | Click            | Opens native file-selection dialog | Project Import     | No Project mutation yet  |

Selecting the Merge action itself does not immediately change the current Project.

# File Selection

The merge flow uses the native file-selection dialog.

| User Interaction    | Visible Result         | Interaction Domain | Undo Expectation / Notes |
| ------------------- | ---------------------- | ------------------ | ------------------------ |
| Navigate folders    | Changes visible files  | Native File Dialog | No Project Undo          |
| Select Project file | Selects source Project | Native File Dialog | No Project mutation yet  |
| Cancel              | Closes file dialog     | Local Cancellation | No Undo entry            |
| Open selected file  | Starts Project merge   | Project Import     | Commit process begins    |

The file-picker interactions themselves should not be added to Project Undo history.

# Merge Result

After a valid source Project has been selected, MobiFlight loads the selected Project and adds all of its Config Files to the currently opened Project.

Conceptually:

```
Current Project

Profiles:
A
B

Selected external Project

Profiles:
C
D
E

Merge

Current Project:

A
B
C
D
E
```

The imported Profiles are appended to the current Project's existing `ConfigFiles` collection.

The current Project itself remains the active Project.

The external Project is used only as the source of the imported Profiles.

# Merge Is a Compound Project Mutation

Merge can add more than one Profile in a single user action.

Each imported Profile can itself contain multiple Config Items.

Therefore the semantic action is:

```
Merge Project Profiles
```

rather than a sequence such as:

```
Add Profile C
Add Profile D
Add Profile E
```

From the user's perspective these additions originate from one command and should naturally be treated as one history operation.

# Additional Merge Side Effects

After the Profiles have been added, the application performs additional Project processing.

This includes:

```
Controller auto-binding

duplicate Config Item GUID detection

duplicate GUID repair where required

Project changed / dirty state

Project refresh
```

Therefore an exact Merge Undo may need to restore more than only the `ConfigFiles` collection.

For example, if the import causes Controller Binding or GUID-related state to change, Undo should restore the state that existed before the complete Merge operation.

# Failed Merge

If the selected file cannot be loaded or parsed successfully:

```
Merge fails
→ error notification
→ imported Profiles are not treated as a successful user action
```

A failed or cancelled Merge should not create an Undo history entry.

# Merge Undo Boundary

The natural transaction is:

```
Before:
Project A

User selects:
From existing project

File picker
→ choose Project B

Successful import
→ Profiles from B appended to A
→ related Project processing completes

After:
Project A'
```

The appropriate Project Undo behavior would therefore conceptually be:

```
Ctrl+Z
→ restore complete Project state from immediately before Merge
```

and:

```
Ctrl+Y
→ restore complete successfully merged state
```

This makes Merge a valid Project Undo candidate, but a more complex one than a simple Config Item property update.

---

# Undo Candidate Classification

After cataloging the GUI interactions, the actions can now be separated according to whether they represent committed Project-state mutations, temporary/local interactions, view state, or external/runtime operations.

The presence of an action in the GUI inventory does not automatically mean that the action belongs in the Project Undo history.

The primary classification is:

```
A. Direct Project Undo Candidates

B. Profile-Level Undo Candidates

C. Project-Wide / Extended Undo Candidates

D. Local / Native Editing Contexts

E. UI / View-State Interactions

F. Runtime / External / Lifecycle Actions
```

# A. Direct Project Undo Candidates

These actions directly modify the contents of the currently opened Project and have a relatively clear user-visible before/after state.

## Config Item Actions

| Action                            | Commit Boundary                | Undo Candidate | Scope / Concern                         |
| --------------------------------- | ------------------------------ | -------------- | --------------------------------------- |
| Toggle Active                     | Active switch click            | Yes            | Single Config Item property             |
| Rename Config Item                | Enter / Blur after inline edit | Yes            | Config Item metadata                    |
| Delete Config Item                | Delete command                 | Yes            | Config Item removal / collection state  |
| Duplicate Config Item             | Successful duplicate creation  | Yes            | Config Item creation                    |
| Add Input Config                  | Successful creation            | Yes            | Config Item creation + editor follow-up |
| Edit Input Config                 | Apply Changes                  | Yes            | Compound Config Item state              |
| Reorder Config Item               | Successful Drop                | Yes            | Config Item ordering within Profile     |
| Move Config Item between Profiles | Successful Drop                | Yes            | Config Item location across Profiles    |
| Bulk Toggle                       | Bulk Toggle command            | Yes            | Multiple Config Item properties         |
| Bulk Delete                       | Bulk Delete command            | Yes            | Multiple Config Item removals           |

These are the clearest Config Item history candidates because:

```
Before state
→ explicit editing action
→ After state
```

is visible to the user.

# B. Profile-Level Undo Candidates

Profile operations also change persisted Project structure.

| Action                      | Commit Boundary                   | Undo Candidate | Scope / Concern                            |
| --------------------------- | --------------------------------- | -------------- | ------------------------------------------ |
| Add Profile                 | New Profile successfully added    | Yes            | Profile collection / active Profile state  |
| Rename Profile              | Enter / Blur after inline rename  | Yes            | Profile metadata                           |
| Remove Profile              | Profile removal                   | Yes            | Profile structure / active Profile state   |
| Merge Profiles from Project | Successful external Project merge | Yes            | Compound Profile / Project import state    |

## Add Profile

Undo should remove the Profile that was created and restore relevant active Profile state if necessary.

## Rename Profile

Only the committed label change belongs in Project history.

Typing inside the inline editor remains local/native editing.

## Remove Profile

Undo needs to restore:

```
removed Profile
original Profile position
possibly previous active Profile
```

## Merge

Merge is the most complex Profile candidate because one action can import multiple Profiles and can trigger additional Project processing.

A complete before/after Project-level representation may therefore be safer than attempting to independently reverse every merge side effect.

# C. Project-Wide / Extended Undo Candidates

Some actions clearly mutate Project state but operate at a wider scope than individual Config Items.

These are valid candidates for a broader Undo implementation.

| Action                | Commit Boundary | Undo Candidate | Scope / Concern        |
| --------------------- | --------------- | -------------- | ---------------------- |
| Rename Project        | Enter / Blur    | Yes            | Project metadata       |
| Edit Project Settings | Update          | Yes            | Compound Project state |
| Controller Bindings   | Apply Changes   | Yes            | Compound Project state |

# Rename Project

The committed Project name is persisted Project state.

Therefore:

```
Project A
→ Rename
→ Project B

Ctrl+Z
→ Project A
```

is semantically reasonable.

The typing that occurs before Enter/Blur remains native text editing.

# Edit Project Settings

`Update` can modify multiple Project properties together:

```
Name
Simulator
Features
Aircraft
```

Because simulator changes can also reset dependent values, the complete Project Settings state should be treated as one compound transaction.

Conceptually:

```
Project Settings A
→ Update
→ Project Settings B
```

rather than one history record per checkbox or field.

A remaining design concern is that the current update path also attempts to save the Project immediately.

The desired dirty/save state after Undo therefore needs an explicit policy.

# Controller Bindings

Controller Binding changes are kept as a dialog draft until:

```
Apply Changes
```

The complete binding list is then committed.

A natural Undo unit is therefore:

```
Binding Set A
→ Binding Set B
```

rather than one history item for every combobox selection.

A semantic no-op check should also be considered because Apply Changes can currently be triggered without an actual binding difference.

# D. Local / Native Editing Contexts

Several GUI interactions introduce local editing contexts that need to be distinguished from committed Project-state changes.

## Text Editing

Examples include:

```
Project Search
Config Item Search
Filter searches

Project Name while inline editing
Profile Name while inline editing
Config Item Name while inline editing

Input Config text fields
Project Settings text fields
Controller Binding search
Application Settings text fields
```

While these controls have focus:

```
Ctrl+Z
```

should normally mean:

```
Undo text editing
```

not:

```
Undo latest Project mutation
```

Only after a Project edit is committed should it become part of Project history.

# Local Cancellation and Open Undo Question

Several editing flows already provide a way to cancel an interaction before the changes are committed.

Examples include:

```
Inline Rename + Escape

Drag + Escape

Input Config + Cancel

Project Settings + Cancel

Controller Bindings + Close
```

Cancellation currently provides a way to discard an unfinished interaction and return to the state that existed before the editing session.

However, the presence of Cancel raises an additional Undo/Redo design question.

For an active editing session, Undo could potentially behave in two different ways:

```
Draft-level Undo

Edit state A
→ change 1
→ change 2
→ Ctrl+Z
→ return to the draft state after change 1
```

or:

```
Transaction-level Undo

Project state A
→ open editor
→ perform several changes
→ Apply / Update
→ Project state B
→ Ctrl+Z
→ restore Project state A
```

These models are not equivalent to Cancel.

Cancel discards the complete unfinished editing session, while draft-level Undo would allow the user to move backward through changes without leaving the editor.

At this stage, it is not yet decided whether Undo/Redo should be available inside an active draft-editing session or only after the complete edit has been committed.

This question will be evaluated when comparing the different Undo/Redo approaches in the next project phase.

# E. UI / View-State Interactions

These interactions are important to the GUI inventory but are normally poor candidates for the Project Undo stack.

Examples include:

```
Project filters
Config Item filters
Community filters

row selection
Ctrl/Shift selection
Clear Selection

Profile selection

Dashboard Project / Community navigation

tab selection
scrolling
panel resizing
expand / collapse

theme
Log panel visibility
```

Some of these states could theoretically have their own UI-level history, but mixing them into Project Undo would create behavior such as:

```
Delete Config Item
→ change filter
→ change Profile
→ Ctrl+Z

What should be undone?
```

For the Project Undo mechanism, keeping UI/View state outside the Project history is considerably clearer.

# F. Runtime / External Actions

These actions should normally be excluded from Project Undo because they represent runtime execution, external systems, hardware effects, or application lifecycle.

Examples include:

```
Run
Stop
Test
Stop Test
Individual Config Test

Save
Save As
Open Project
Exit

Sign In
Sign Out

external links

Copy Logs

Firmware Update
Upload Controller Configuration
Reset Board
Regenerate Serial

Joystick/MIDI runtime connection changes
```

Once an external hardware or system operation has happened, restoring a data snapshot does not necessarily reverse its real-world side effect.

These actions therefore belong outside normal Project history.

# Candidate Classification Summary

The GUI-based Action Catalog shows that user interactions in MobiFlight affect different kinds of state and have different interaction boundaries.

The presence of an interaction in the catalog does not imply that it should belong to the same Undo/Redo mechanism.

The interactions can be classified along three main dimensions.

## State Scope

```
UI / View State
Native Text / Local Draft State
Config Item State
Profile State
Project State
Application / Global State
Runtime / External State
Project Lifecycle / Persistence
```

Examples:

```
Search or Filter
→ UI / View State

Typing inside an input field
→ Native Text / Local Draft State

Toggle Config Item Active
→ Config Item / Project State

Rename Profile
→ Profile / Project State

Edit Project Settings
→ Project State

Change Application Language
→ Application / Global State

Run / Test / Firmware Update
→ Runtime / External State

Open / Save Project
→ Project Lifecycle / Persistence
```

## Mutation Pattern

Project-related interactions also fall into recurring mutation patterns:

```
Update
Create
Delete
Move / Reorder
Compound Edit
Import / Merge
```

Examples:

```
Toggle Active
→ Update

Duplicate Config Item
→ Create

Delete Config Item
→ Delete

Drag and Drop
→ Move / Reorder

Input Config Editor
→ Compound Edit

Merge Profiles from Existing Project
→ Import / Merge
```

## Interaction Boundary

Another important distinction is when a user interaction becomes committed.

Examples include:

```
Immediate Action
→ Toggle Active
→ Delete

Inline Commit
→ Rename + Enter / Blur

Dialog Commit
→ Apply Changes
→ Update

Drag-and-Drop Commit
→ Successful Drop

Local Cancellation
→ Escape
→ Cancel
→ Close without Apply

Native Text Editing
→ typing inside a focused text field
```

This distinction is important because an interaction can pass through several temporary UI states before it becomes a committed Project-state change.

For example:

```
Rename
→ open inline editor
→ type text
→ Escape
= local cancellation

Rename
→ open inline editor
→ type text
→ Enter
= committed Project-state change
```

and:

```
Drag
→ move item temporarily
→ Escape
= local cancellation

Drag
→ Drop
= committed Project-state change
```

## Classification Result

The Action Catalog therefore distinguishes between:

```
What the user can do

What state the interaction affects

What kind of mutation occurs

When the interaction becomes committed
```

These classifications provide the basis for comparing Undo/Redo approaches in the following project phase.

No MVP action set is selected at this stage.
