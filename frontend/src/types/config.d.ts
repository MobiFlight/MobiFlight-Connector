// eslint-disable-next-line @typescript-eslint/no-unused-vars
export interface IDictionary<T, K extends string | number | symbol> {
  [Key in K]: T
}

export interface IConfigItem {
  GUID: string
  Active: boolean
  Type: string
  // This is the name of the config item
  Name: string
  // name / serial of the device
  ModuleSerial: string
  Device: IDeviceConfig
  // this is the type of the Device
  // Type: DeviceElementType;
  // Tags: string[];
  RawValue: string | null
  Value: string | null
  Status: IDictionary<string, ConfigItemStatusType>
  // Preconditions: Precondition[]
  // Modifiers: Precondition[]
  // ConfigRefs: Precondition[]
}

export type ConfigItemStatusType = "Precondition" | "Source" | "Modifier" | "Test" | "Device" | "ConfigRef"

export interface IDictionary<T> {
  [Key: string]: T
}

export type DeviceType = "MobiFlight" | "Joystick" | "Midi"

export type ConfigItemType = "InputConfig" | "OutputConfig"

export type ElementPin = {
  isAnalog: boolean
  isPWM: boolean
  isI2C: boolean
  Used: boolean
  Pin: number
}

export interface IDeviceItem {
  Id: string
  Type: DeviceType
  Name: string
  MetaData: IDictionary<string>
  Elements: IDeviceElement[]
  Pins?: ElementPin[]
}

export interface IDeviceConfig {
  Type: string
  Name: string
}
