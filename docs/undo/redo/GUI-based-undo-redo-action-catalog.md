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