import { DeviceElementType } from "./deviceElements"

// eslint-disable-next-line @typescript-eslint/no-unused-vars
export interface IDictionary<T, K extends string | number | symbol> {
  [Key in K]: T
}

export interface IConfigValueOnlyItem {
  GUID: string
  RawValue?: string | null
  Value?: string | null
  Status: IDictionary<string, ConfigItemStatusType>
}

export interface IConfigItem extends IConfigValueOnlyItem {
  Active: boolean
  Type: string
  // This is the name of the config item
  Name: string
  // name / serial of the device
  ModuleSerial: string
  Device?: IDeviceConfig | null 
  // Tags: string[];
  Status: IDictionary<string, ConfigItemStatusType>
}

export type ConfigItemStatusType = "Precondition" | "Source" | "Modifier" | "Test" | "Device" | "ConfigRef"

export interface IDictionary<T> {
  [Key: string]: T
}

export type ControllerType = "MobiFlight" | "Joystick" | "Midi" | "Unknown"

export type ConfigItemType = "InputConfig" | "OutputConfig"

export interface IDeviceConfig {
  Type: DeviceElementType | string
  Name: string
}

export interface ConfigFile {
  Label: string
  FileName: string
  ConfigItems: IConfigItem[]
}