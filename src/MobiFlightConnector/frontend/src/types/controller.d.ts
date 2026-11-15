import { ControllerType } from "./config"

export type OutputDeviceType = "Output" | "LedModule" | "LcdDisplay" | "Servo" | "Stepper" | "ShiftRegister" | "CustomDevice"
export type InputDeviceType = "Button" | "Encoder" | "AnalogInput"

export type Controller = {
  Name: string
  Vendor: string
  ProductId: string
  VendorId: string
  Type: ControllerType
  Connected: boolean
  Serial: string
  ImageUrl: string | null
  certified: boolean
  firmwareUpdate?: boolean
  Devices: DeviceReference[]
}

export type ControllerBinding = {
  BoundController: Partial<Controller> | null
  Status: ControllerBindingStatus
  OriginalController: Partial<Controller>
}

export type CustomDeviceMessageType = {
  Id: number
  Label: string
  Description: string
}

export type StepperProfilePreset = {
  id: number
  Mode: number
  StepsPerRevolution: number
  Speed: number
  Acceleration: number
  BacklashCompensation: number
  Deactivate: boolean
}

export type DeviceCapability = {
  Pwm?: boolean | null
  Rows?: number | null
  Columns?: number | null
  Digits?: number | null
  Pins?: number | null
  Profile?: StepperProfilePreset | null
  AutoZero?: boolean | null
  MessageTypes?: CustomDeviceMessageType[] | null
}

export type DeviceReference = {
  Name: string
  Label: string
  Type: InputDeviceType | OutputDeviceType
  Capabilities?: DeviceCapability | null
}

export type ControllerBindingStatus = "Match" | "AutoBind" | "Missing" | "RequiresManualBind"


export type vJoyDefinition = {
  Id: number
  Name: string
  Buttons: number
  Axis: VJoyAxisState
}

export type VJoyAxisState = {
  X: boolean
  Y: boolean
  Z: boolean
  RX: boolean
  RY: boolean
  RZ: boolean
}