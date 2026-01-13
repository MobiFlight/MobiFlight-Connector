import { Settings } from "http2"
import { IConfigValueOnlyItem } from "./config"
import { JoystickDefinition, MidiControllerDefinition } from "./definitions"
import { ProjectInfo } from "@/types/project"

export type AppMessageKey =
  | "StatusBarUpdate"
  | "ConfigFile"
  | "Project"
  | "RecentProjects"
  | "ConfigValueFullUpdate"
  | "ConfigValuePartialUpdate"
  | "ConfigValueRawAndFinalUpdate"
  | "Settings"
  | "ExecutionState"
  | "BoardDefinitions"
  | "JoystickDefinitions"
  | "MidiControllerDefinitions"
  | "ProjectStatus"
  | "OverlayState"
  | "Notification"
  | "HubHopState"
  | "ConnectedControllers"

export type AppMessagePayload =
  | StatusBarUpdate
  | ConfigLoadedEvent
  | ConfigValueFullUpdate
  | ConfigValuePartialUpdate
  | ConfigValueRawAndFinalUpdate
  | ExecutionState
  | BoardDefinitions
  | JoystickDefinitions
  | MidiControllerDefinitions
  | ProjectStatus
  | OverlayState
  | Notification
  | HubHopState
  | RecentProjects
  | ConnectedControllers

// AppMessage is the message format
// when receiving messages from the backend
export type AppMessage = {
  key: AppMessageKey
  payload: AppMessagePayload | Settings | Project
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

export interface BoardDefinitions {
  Definitions: BoardDefinition[]
}

export interface JoystickDefinitions {
  Definitions: JoystickDefinition[]
}

export interface MidiControllerDefinitions {
  Definitions: MidiControllerDefinition[]
}

export interface ProjectStatus {
  HasChanged: boolean
}

export interface OverlayState {
  Visible: boolean
}

export interface Notification {
  Event:
    | "ControllerAutoBindSuccessful"
    | "ControllerManualBindRequired"
    | "ProjectFileExtensionMigrated"
    | "SimConnectionLost"
    | "SimStopped"
    | "TestModeException"
  Guid?: string
  Context: Record<string, string> | null
}

export interface HubHopState {
  LastUpdate: Date | null
  ShouldUpdate: boolean
  UpdateProgress: number
  Result: "Success" | "Error" | "InProgress" | "Pending"
}

export interface RecentProjects {
  Projects: ProjectInfo[]
}

export interface ConnectedControllers {
  Controllers: Controller[]
}

// Not sure what this is for
// but we are using it in the tests
// for mocking the chrome API
export type Message = AppMessagePayload
