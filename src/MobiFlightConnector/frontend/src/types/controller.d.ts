import { ControllerType } from "./config"

export type OutputDeviceType = "Output" | "LedModule" | "LcdDisplay" | "Servo" | "Stepper" | "ShiftRegister" | "CustomDevice"
export type InputDeviceType = "Button" | "Encoder" | "AnalogInput"
export type BaseDevice = {
  Name: string
  Label: string
  Type: InputDeviceType | OutputDeviceType
}

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
  Devices: BaseDevice[]
}

export type ControllerBinding = {
  BoundController: Partial<Controller> | null
  Status: ControllerBindingStatus
  OriginalController: Partial<Controller>
}

export type DeviceReference = {
  Name: string
  Label: string
  Type: InputDeviceType | OutputDeviceType
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