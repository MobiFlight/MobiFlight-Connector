// FrontendMessages are messages
// that are sent from the frontend to the backend
export type CommandMessageKey =
  | "ConfigEdit"
  | "CommandConfigContextMenu"
  | "CommandConfigBulkAction"
  | "CommandUpdateConfigItem"
  | "CommandAddConfigItem"
  | "CommandResortConfigItem"

export type CommandMessage =
  | CommandConfigContextMenu
  | CommandConfigBulkAction
  | CommandUpdateConfigItem
  | CommandAddConfigItem
  | CommandResortConfigItem
  | CommandActiveConfigFile


export interface CommandMessageBase {
  key: CommandMessageKey
}

// EditConfigMessage
// is sent to the backend
// when a config item shall be edited
export interface CommandConfigContextMenu extends CommandMessageBase{
  key: "CommandConfigContextMenu"
  payload: { 
    action: "edit" | "delete" | "duplicate" | "test" | "settings" | "toggle",
    item: IConfigItem
  }
}

// EditConfigMessage
// is sent to the backend
// when a config item shall be edited
export interface CommandConfigBulkAction extends CommandMessageBase{
  key: "CommandConfigBulkAction"
  payload: { 
    action: "delete" | "toggle",
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
    name: string,
    type: ConfigItemType
  }
}


export interface CommandResortConfigItem extends CommandMessageBase {
  key: "CommandResortConfigItem"
  payload: { 
    items: IConfigItem[],
    newIndex: number
  }
}

export interface CommandActiveConfigFile extends CommandMessageBase {
  key: "CommandActiveConfigFile"
  payload: { 
    index: number
  }
}