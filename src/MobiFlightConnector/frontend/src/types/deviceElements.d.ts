export type DeviceElementType =
  | "Button"
  | "Output"
  | "Encoder"
  | "LedModule"
  | "Display Module"
  | "Stepper"
  | "Servo"
  | "LcdDisplay"
  | "ShiftRegister"
  | "AnalogInput"
  | "InputShiftRegister"
  | "InputMultiplexer"
  | "CustomDevice"
  | "InputAction"

export const DeviceElementTypes: DeviceElementType[] = [
  "Button",
  "Encoder",
  "AnalogInput",
  "InputShiftRegister",
  "InputMultiplexer",
  "Output",
  "LedModule",
  "Stepper",
  "Servo",
  "LcdDisplay",
  "ShiftRegister",
  "CustomDevice",
  "InputAction"
]
