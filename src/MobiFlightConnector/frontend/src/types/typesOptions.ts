import { PreconditionType } from "@/types/config"
import { InputDeviceType, OutputDeviceType } from "@/types/controller"

export const InputDeviceTypes: InputDeviceType[] = [
  "Button",
  "Encoder",
  "AnalogInput",
]
export const OutputDeviceTypes: OutputDeviceType[] = [
  "Output",
  "LedModule",
  "LcdDisplay",
  "Servo",
  "Stepper",
  "ShiftRegister",
  "CustomDevice",
]

export const PRECONDITION_TYPES = ["variable", "config", "pin"] as PreconditionType[]
