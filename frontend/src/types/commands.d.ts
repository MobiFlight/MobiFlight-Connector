// FrontendMessages are messages
// that are sent from the frontend to the backend
export type CommandMessageKey =
  | "ConfigEdit"
  | "CommandConfigContextMenu"
  | "CommandFileContextMenu"
  | "CommandConfigBulkAction"
  | "CommandUpdateConfigItem"
  | "CommandAddConfigItem"
  | "CommandResortConfigItem"
  | "CommandAddConfigFile"
  | "CommandMainMenu"
  | "CommandProjectToolbar"

export type CommandMessage =
  | CommandConfigContextMenu
  | CommandFileContextMenu
  | CommandConfigBulkAction
  | CommandUpdateConfigItem
  | CommandAddConfigItem
  | CommandResortConfigItem
  | CommandActiveConfigFile
  | CommandAddConfigFile
  | CommandMainMenu
  | CommandProjectToolbar

export interface CommandMessageBase {
  key: CommandMessageKey
}

// EditConfigMessage
// is sent to the backend
// when a config item shall be edited
export interface CommandConfigContextMenu extends CommandMessageBase {
  key: "CommandConfigContextMenu"
  payload: {
    action: "edit" | "delete" | "duplicate" | "test" | "settings" | "toggle"
    item: IConfigItem
  }
}

// EditConfigMessage
// is sent to the backend
// when a config item shall be edited
export interface CommandConfigBulkAction extends CommandMessageBase {
  key: "CommandConfigBulkAction"
  payload: {
    action: "delete" | "toggle"
    items: IConfigItem[]
  }
}

// CommandUpdateConfigItem
// Update basic properties of a config item
// Active, Name
export interface CommandUpdateConfigItem extends CommandMessageBase {
  key: "CommandUpdateConfigItem"
  payload: {
    item: IConfigItem
  }
}

// CommandUpdateConfigItem
// Update basic properties of a config item
// Active, Name
export interface CommandAddConfigItem extends CommandMessageBase {
  key: "CommandAddConfigItem"
  payload: {
    name: string
    type: ConfigItemType
  }
}

export interface CommandResortConfigItem extends CommandMessageBase {
  key: "CommandResortConfigItem"
  payload: {
    items: IConfigItem[]
    newIndex: number
  }
}

export interface CommandActiveConfigFile extends CommandMessageBase {
  key: "CommandActiveConfigFile"
  payload: {
    index: number
  }
}

// CommandAddConfigFile
// Add a new or existing config file to the project
export interface CommandAddConfigFile extends CommandMessageBase {
  key: "CommandAddConfigFile"
  payload: {
    type: "create" | "merge"
  }
}

// EditConfigMessage
// is sent to the backend
// when a config item shall be edited
export interface CommandFileContextMenu extends CommandMessageBase {
  key: "CommandFileContextMenu"
  payload: {
    action: "rename" | "remove" | "export"
    index: number
    file: ConfigFile
  }
}

export type CommandMainMenuPayload = {
  action:
    | "file.new"
    | "file.open"
    | "file.save"
    | "file.saveas"
    | "file.exit"
    | "file.recent"
    | "extras.hubhop.download"
    | "extras.msfs.reinstall"
    | "extras.copylogs"
    | "extras.serials"
    | "extras.settings"
    | "help.docs"
    | "help.checkforupdate"
    | "help.discord"
    | "help.youtube"
    | "help.hubhop"
    | "help.about"
    | "help.releasenotes"
  index?: number
}

export interface CommandMainMenu extends CommandMessageBase {
  key: "CommandMainMenu"
  payload: CommandMainMenuPayload
}

export type CommandProjectToolbarPayload = {
  action:
    | "run"
    | "test"
    | "stop"
    | "toggleAutoRun"
}

export interface CommandProjectToolbar extends CommandMessageBase {
  key: "CommandProjectToolbar"
  payload: CommandProjectToolbarPayload
}
