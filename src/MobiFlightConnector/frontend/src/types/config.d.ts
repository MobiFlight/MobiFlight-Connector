import { Controller } from "./controller"
import { ModifierList, Modifier } from "./modifier"

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
  Type: ConfigItemType
  // This is the name of the config item
  Name: string
  // name / serial of the device
  Controller?: Partial<Controller>
  Device?: IDeviceConfig | null
  DeviceType?: string | null
  DeviceName?: string | null
  // Tags: string[];
  Preconditions: Precondition[]
  Status: IDictionary<string, ConfigItemStatusType>
  button?: ButtonTrigger
  inputMultiplexer?: ButtonTrigger
  inputShiftRegister?: ButtonTrigger
  encoder?: EncoderTrigger
  analog?: AnalogTrigger
  ConfigRefs: ConfigReference[]
  Modifiers?: ModifierList
}

export type ConfigItemStatusType =
  | "Precondition"
  | "Source"
  | "Modifier"
  | "Test"
  | "Device"
  | "ConfigRef"

export interface IDictionary<T> {
  [Key: string]: T
}

export type ControllerType = "MobiFlight" | "Joystick" | "Midi" | "Unknown"

export type ConfigItemType = "InputConfigItem" | "OutputConfigItem"

export interface IDeviceConfig {
  Type: string
  Name: string
}

export interface ExtendedDeviceConfig extends IDeviceConfig {
  Pin?: string | null
  SubIndex?: number | null
}

export interface ConfigFile {
  Label: string
  FileName: string | null
  ConfigItems: IConfigItem[]
}

export interface Action {
  Type: string | null
}

export type ButtonHoldOptions = {
  HoldDelay: number
  RepeatDelay: number
}

export type ButtonLongReleaseOptions = {
  LongReleaseDelay: number
}

export type ButtonTrigger = {
  onPress?: Action
  onRelease?: Action
  onHold?: Action
  onLongRelease?: Action
}

export type ButtonTrigger = ButtonTrigger & Partial<ButtonHoldOptions> & Partial<ButtonLongReleaseOptions>

export interface EncoderTrigger {
  onLeft?: Action
  onRight?: Action
  onLeftFast?: Action
  onRightFast?: Action
}

export interface AnalogTrigger {
  onChange?: Action
}

export interface MsfsInputAction extends Action {
  Type: "MSFS2020CustomInputAction"
  Command: string
  PresetId: string
}

export interface XplaneInputAction extends Action {
  Type: "XplaneInputAction"
  InputType: string
  Path: string
  Expression: string
}

export interface MobiFlightVariableAction extends Action {
  Type: "VariableInputAction"
  Variable: MobiFlightVariable
}

export type MobiFlightVariableType = "number" | "string"
export interface MobiFlightVariable {
  TYPE: MobiFlightVariableType
  Name: string
  Number: number
  Text: string
  Expression: string
}

export interface VJoyInputAction extends Action {
  Type: "VJoyInputAction"
  vJoyID: number
  buttonNr: number
  axisString: string
  buttonComand: boolean
  sendValue: string
}

export interface RetriggerInputAction extends Action {
  Type: "RetriggerInputAction"
}

export interface ProSimInputAction extends Action {
  Type: "ProSimInputAction"
  Path: string
  Expression: string
}

export interface EventIdInputAction extends Action {
  Type: "EventIdInputAction"
  EventId: string
  Param: string
}

export interface PmdgEventIdInputAction extends EventIdInputAction {
  Type: "PmdgEventIdInputAction"
  AircraftType: "B737" | "B747" | "B777"
}

export interface LuaMacroInputAction extends Action {
  Type: "LuaMacroInputAction"
  MacroName: string
  MacroValue: string
}

export interface KeyInputAction extends Action {
  Type: "KeyInputAction"
  Code: string
  Control: boolean
  Alt: boolean
  Shift: boolean
}

export interface JeehellInputAction extends EventIdInputAction {
  Type: "JeehellInputAction"
}

export type FsuipcOffset = {
  Offset: number
  Size: number
  Mask: number
  BcdMode: boolean
  OffsetType: 0 | 1 | 2 // 0 = Integer, 1 = Float, 2 = String
}

export interface FsuipcOffsetInputAction extends Action {
  Type: "FsuipcOffsetInputAction"
  FSUIPC: FsuipcOffset
  Value: string
  Modifiers: Modifier[]
}

export type Precondition = {
  Type: "variable" | "pin" | "config"
  Ref: string
  Pin: string | null
  Operand: "=" | "<>" | "<" | ">" | "<=" | ">="
  Value: string
  Logic: "and" | "or"
  Active: boolean
}

export type ConfigReference = {
  Active: boolean
  Placeholder: string
  Ref: string
  TestValue: string
}