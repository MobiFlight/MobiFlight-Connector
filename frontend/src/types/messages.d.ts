import { IDeviceItem } from "."

export interface ExecutionUpdateMessage {
    key: "ExecutionUpdate"
    payload: ExecutionState
}

export interface EditConfigMessage {
    key: "config.edit"
    payload: IConfigItem
}

export interface GlobalSettingsUpdateMessage {
    key: "GlobalSettingsUpdate"
    payload: IGlobalSettings
}

export interface ElementEditMessage {
    key: FrontendMessageKey
    payload: {
        device : IDeviceItem, 
        element: IDeviceElement
    }
}

export type FrontendMessageKey = "ExecutionUpdate" | "config.edit" | "GlobalSettingsUpdate" |"ElementEdit"
export type FrontendMessageType = ExecutionUpdateMessage | EditConfigMessage | GlobalSettingsUpdateMessage | ElementEditMessage

export type AppMessageKey = "config.update" | "GlobalSettings" | "StatusBarUpdate" | "ConfigFile" | "LogMessage" | "ExecutionUpdate" | "ConfigValueUpdate" | "DeviceUpdate" | "ElementEdit"
export type AppMessagePayload = ConfigLoadedEvent | EventMessage | StatusBarUpdate | ExecutionUpdate | ConfigValueUpdate | ILogMessage | IGlobalSettings | IConfigItem


export type AppMessage = {
    key: AppMessageKey
    payload: AppMessagePayload
}

export interface ConfigLoadedEvent {
    FileName: string
    ConfigItems: Types.IConfigItem[]
}

export interface StatusBarUpdate {
    Text: string
    Value: number
}

export interface ExecutionUpdate {
    State: ExecutionState
}

export interface ConfigValueUpdate {
    ConfigItems: Types.IConfigItem[]
}

export interface DeviceUpdate {
    Devices: Types.IDeviceItem[]
}

export type Message = ConfigLoadedEvent | EventMessage | StatusBarUpdate | ExecutionUpdate | ConfigValueUpdate