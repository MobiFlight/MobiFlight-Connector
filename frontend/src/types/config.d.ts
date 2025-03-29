import { DeviceElementType } from "./deviceElements"

// eslint-disable-next-line @typescript-eslint/no-unused-vars
export interface IDictionary<T, K extends string | number | symbol> {
  [Key in K]: T
}

export interface IConfigValueOnlyItem {
  GUID: string
  RawValue?: string | null
  Value?: string | null
}

export interface IConfigItem extends IConfigValueOnlyItem {
  Active: boolean
  Type: string
  // This is the name of the config item
  Name: string
  // name / serial of the device
  ModuleSerial: string
  Device?: IDeviceConfig | null 
  // this is the type of the Device
  // Type: DeviceElementType;
  DeviceName?: string | null
  DeviceType: DeviceElementType | string
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
  Type: string
  Name: string
}
