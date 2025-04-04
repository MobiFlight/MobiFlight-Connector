import { Settings } from "http2"
import { IConfigValueOnlyItem } from "./config"

export type AppMessageKey =
  | "StatusBarUpdate"
  | "ConfigFile"
  | "ConfigValueFullUpdate"
  | "ConfigValuePartialUpdate"
  | "ConfigValueRawAndFinalUpdate"
  | "Settings"

export type AppMessagePayload =
  | StatusBarUpdate
  | ConfigLoadedEvent
  | ConfigValueFullUpdate
  | ConfigValuePartialUpdate
  | ConfigValueRawAndFinalUpdate
  
// AppMessage is the message format
// when receiving messages from the backend
export type AppMessage = {
  key: AppMessageKey
  payload: AppMessagePayload | Settings
}

// ConfigLoadedEvent
// is sent from the backend
// when the config file was loaded
// the payload contains the config items
export interface ConfigLoadedEvent {
  FileName: string
  ConfigItems: IConfigItem[]
}

// StatusBarUpdate
// the status bar shall be updated
// with a new text and value
// this happens during startup
export interface StatusBarUpdate {
  Text: string
  Value: number
}

export interface ConfigValueFullUpdate {
  ConfigItems: IConfigItem[]
}

export interface ConfigValuePartialUpdate {
  ConfigItems: IConfigItem[]
}

export interface ConfigValueRawAndFinalUpdate {
  ConfigItems: IConfigValueOnlyItem[]
}

// Not sure what this is for
// but we are using it in the tests
// for mocking the chrome API
export type Message = AppMessagePayload