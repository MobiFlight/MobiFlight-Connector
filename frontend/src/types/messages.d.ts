import { Settings } from "http2"
import { IConfigValueOnlyItem } from "./config"
import { JoystickDefinition, MidiControllerDefinition } from "./definitions"

export type AppMessageKey =
  | "StatusBarUpdate"
  | "ConfigFile"
  | "Project"
  | "ConfigValueFullUpdate"
  | "ConfigValuePartialUpdate"
  | "ConfigValueRawAndFinalUpdate"
  | "Settings"
  | "ExecutionState"
  | "JoystickDefinitions"
  | "MidiControllerDefinitions"

export type AppMessagePayload =
  | StatusBarUpdate
  | ConfigLoadedEvent  
  | ConfigValueFullUpdate
  | ConfigValuePartialUpdate
  | ConfigValueRawAndFinalUpdate
  | ExecutionState
  | JoystickDefinitions
  | MidiControllerDefinitions
  
// AppMessage is the message format
// when receiving messages from the backend
export type AppMessage = {
  key: AppMessageKey
  payload: AppMessagePayload | Settings | Project | ExecutionState | JoystickDefinitions
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
  ConfigIndex: number
  ConfigItems: IConfigItem[]
}

export interface ConfigValuePartialUpdate {
  ConfigItems: IConfigItem[]
}

export interface ConfigValueRawAndFinalUpdate {
  ConfigItems: IConfigValueOnlyItem[]
}

export interface ExecutionState {
  IsRunning: boolean
  IsTesting: boolean
  RunAvailable: boolean
  TestAvailable: boolean
}

export interface JoystickDefinitions {
  Definitions: JoystickDefinition[]
}

export interface MidiControllerDefinitions {
  Definitions: MidiControllerDefinition[]
}

// Not sure what this is for
// but we are using it in the tests
// for mocking the chrome API
export type Message = AppMessagePayload