import { IDeviceItem } from "."

// FrontendMessages are messages
// that are sent from the frontend to the backend
export type FrontendMessageKey =
  | "ExecutionUpdate"
  | "ConfigEdit"
  | "GlobalSettingsUpdate"
  | "DeviceUpload"
  | "DeviceFirmwareUpdateRequest"
  | "DeviceFileOpenRequest"
  | "DeviceFileSaveRequest"

export type CommandMessageType =
  | ExecutionUpdateMessage
  | CommandConfigContextMenu
  | GlobalSettingsUpdateMessage
  | ElementCreateMessage
  | DeviceUploadMessage
  | DeviceFirmwareUpdateRequestMessage
  | DeviceFileOpenRequest
  | DeviceFileSaveRequest
  | CommandUpdateConfigItem
  | CommandAddConfigItem


// ExecutionUpdateMessage
// is sent to the backend
// when execution state shall change, e.g.
// when the user starts or stops MobiFlight
export interface ExecutionUpdateMessage {
  key: "ExecutionUpdate"
  payload: ExecutionState
}

// EditConfigMessage
// is sent to the backend
// when a config item shall be edited
export interface CommandConfigContextMenu {
  key: "CommandConfigContextMenu"
  payload: { 
    action: "edit" | "delete" | "duplicate",
    item: IConfigItem
  }
}

// CommandUpdateConfigItem
// Update basic properties of a config item
// Active, Name
export interface CommandUpdateConfigItem {
  key: "CommandUpdateConfigItem"
  payload: { 
    item: IConfigItem
  }
}

// CommandUpdateConfigItem
// Update basic properties of a config item
// Active, Name
export interface CommandAddConfigItem {
  key: "CommandAddConfigItem"
  payload: { 
    name: string,
    type: ConfigItemType
  }
}
// GlobalSettingsUpdateMessage
// is sent to the backend
// when global settings shall be updated
// the expected response will be a GlobalSettings message
// this is currently the only message
// that is also used by the backend
export interface GlobalSettingsUpdateMessage {
  key: "GlobalSettingsUpdate"
  payload: IGlobalSettings
}

// DeviceUploadMessage
// is sent to the backend
// when the new device configuration shall be uploaded
export interface DeviceUploadMessage {
  key: "DeviceUpload"
  payload: { 
    item: IDeviceItem
  }
}

// DeviceUploadMessage
// is sent to the backend
// when the new device configuration shall be uploaded
export interface DeviceFirmwareUpdateRequestMessage {
  key: "DeviceFirmwareUpdateRequest"
  payload: IDeviceItem
}

// DeviceUploadMessage
// is sent to the backend
// when the new device configuration shall be uploaded
export interface DeviceFileOpenRequest {
  key: "DeviceFileOpenRequest"
  payload: IDeviceItem
}

export interface DeviceFileSaveRequestRequest {
  key: "DeviceFileSaveRequestRequest"
  payload: IDeviceItem
}

export type AppMessageKey =
  | "config.update"
  | "GlobalSettings"
  | "StatusBarUpdate"
  | "ConfigFile"
  | "LogEntry"
  | "ExecutionUpdate"
  | "ConfigValueUpdate"
  | "DeviceUpdate"
  | "DeviceElementCreateResponse"
  | "Notification"

export type AppMessagePayload =
  | ConfigLoadedEvent
  | EventMessage
  | StatusBarUpdate
  | ExecutionUpdate
  | ConfigValueUpdate
  | ILogMessage
  | IGlobalSettings
  | IConfigItem
  | Notification

// AppMessage is the message format
// when receiving messages from the backend
export type AppMessage = {
  key: AppMessageKey
  payload: AppMessagePayload
}

// ConfigLoadedEvent
// is sent from the backend
// when the config file was loaded
// the payload contains the config items
export interface ConfigLoadedEvent {
  FileName: string
  ConfigItems: Types.IConfigItem[]
}

// StatusBarUpdate
// the status bar shall be updated
// with a new text and value
// this happens during startup
export interface StatusBarUpdate {
  Text: string
  Value: number
}

// ExecutionUpdate
// the execution state has changed
export interface ExecutionUpdate {
  State: ExecutionState
}

export interface ConfigValueUpdate {
  ConfigItems: Types.IConfigItem[]
}

// DeviceUpdate
// a device was updated
export interface DeviceUpdate {
  Devices: Types.IDeviceItem[]
}

export interface DeviceElementCreateResponse {
  Device: IDeviceItem
  Element: IDeviceElement
}

export interface Notification {
  Type: string
  Value: IDeviceItem | IDeviceElement | IConfigItem | string | undefined
  Action: string | undefined
}


// Not sure what this is for
// but we are using it in the tests
// for mocking the chrome API
export type Message = AppMessagePayload
