export interface IConfigItem {
  GUID: string
  Active: boolean
  Description: string
  Device: string
  Component: string
  Type: string
  Tags: string[]
  Status: string[]
  RawValue: string
  ModifiedValue: string
  Modifiers: IModifier[]
  Event: IConfigEvent
  Action: IConfigAction
  Context: IConfigContext
}

export interface IConfigEvent {
  Type: "FSUIPC" | "SIMCONNECT" | "VARIABLE" | "XPLANE"
  Settings:
    | SimConnectVarEventSettings
    | FsuipcEventSettings
    | VariableEventSettings
    | XplaneEventSettings
}

export interface SimConnectVarEventSettings {
  UUID: string
  Value: string
  VarType: "CODE" | "LVAR" | "AVAR"
}

export interface FsuipcEventSettings {
  Offset: number
  Size: number
  Type: "Integer" | "Float" | "String"
  Mask: number
  BcdMode: boolean
}

export interface VariableEventSettings {
  Type: "number" | "string"
  Name: string
  Number: number
  Text: string
  Expression: string
}

export interface XplaneEventSettings {
  Path: string
}

export interface IConfigAction {
  Type: string
  Settings: object
}

export interface IConfigContext {
  Preconditions: {
    Type: string
    Ref: string
    Serial: string
    Pin: string
    Operand: string
    Value: string
    Logic: string
    Active: boolean
  }[]
  ConfigReferences: {
    Active: boolean
    Ref: string
    Placeholder: string
    TestValue: string
  }[]
}

export interface IDictionary<T> {
  [Key: string]: T
}

export type DeviceType = "MobiFlight" | "Joystick" | "Midi"

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
