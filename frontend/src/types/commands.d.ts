// FrontendMessages are messages
// that are sent from the frontend to the backend
export type CommandMessageKey =
  | "ConfigEdit"
  | "CommandConfigContextMenu"
  | "CommandUpdateConfigItem"
  | "CommandAddConfigItem"
  | "CommandResortConfigItem"

export type CommandMessage =
  | CommandConfigContextMenu
  | CommandUpdateConfigItem
  | CommandAddConfigItem
  | CommandResortConfigItem


export interface CommandMessageBase {
  key: CommandMessageKey
}

// EditConfigMessage
// is sent to the backend
// when a config item shall be edited
export interface CommandConfigContextMenu extends CommandMessageBase{
  key: "CommandConfigContextMenu"
  payload: { 
    action: "edit" | "delete" | "duplicate" | "test",
    item: IConfigItem
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