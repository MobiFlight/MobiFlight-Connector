import {
  CustomDeviceConfig,
  FsuipcOffset,
  FsuipcSource,
  LcdDisplayConfig,
  LedModuleConfig,
  OutputConfig,
  OutputDeviceConfig,
  ProSimSource,
  ServoConfig,
  ShiftRegisterConfig,
  SimConnectSource,
  StepperConfig,
  VariableSource,
  XplaneSource,
} from "@/types/config"
import { OutputDeviceType } from "@/types/controller"

export const FsuipcDefaults = {
  createOffset: (): FsuipcOffset => ({
    OffsetType: 0,
    Offset: 0x66c0,
    Size: 1,
    Mask: 0xff,
    BcdMode: false,
  }),
}

export const SourceDefaults = {
  createSimConnectSource: (): SimConnectSource => ({
    Type: "SimConnectSource",
    SimConnectValue: {
      UUID: "",
      Value: "",
    },
  }),
  createXplaneSource: (): XplaneSource => ({
    Type: "XplaneSource",
    XplaneDataRef: {
      Path: "",
    },
  }),
  createProSimSource: (): ProSimSource => ({
    Type: "ProSimSource",
    ProSimDataRef: {
      Path: "",
    },
  }),
  createVariableSource: (): VariableSource => ({
    Type: "VariableSource",
    MobiFlightVariable: {
      Name: "",
      TYPE: "number",
      Expression: "",
      Number: 0,
      Text: "",
    },
  }),
  createFsuipcSource: (): FsuipcSource => ({
    Type: "FsuipcSource",
    FSUIPC: FsuipcDefaults.createOffset(),
  }),
}

export const OutputDeviceConfigDefaults = {
  createByType: (type: OutputDeviceType, name: string): OutputDeviceConfig => {
    switch (type) {
      case "Output":
        return OutputDeviceConfigDefaults.createOutputConfig(name)
      case "LedModule":
        return OutputDeviceConfigDefaults.createLedModuleConfig(name)
      case "LcdDisplay":
        return OutputDeviceConfigDefaults.createLcdDisplayConfig(name)
      case "Servo":
        return OutputDeviceConfigDefaults.createServoConfig(name)
      case "Stepper":
        return OutputDeviceConfigDefaults.createStepperConfig(name)
      case "ShiftRegister":
        return OutputDeviceConfigDefaults.createShiftRegisterConfig(name)
      case "CustomDevice":
        return OutputDeviceConfigDefaults.createCustomDeviceConfig(name)
      default:
        throw new Error(`Unsupported output device type: ${type}`)
    }
  },
  createOutputConfig: (name: string): OutputConfig => ({
    Type: "Output",
    Name: name,
    Brightness: 255,
    PwmMode: false,
    Pin: name
  }),
  createLedModuleConfig: (name: string): LedModuleConfig => ({
    Type: "LedModule",
    Name: name,
    DisplayLedAddress: name,
    DisplayLedConnector: 1,
    DisplayLedModuleSize: 8,
    DisplayLedPadding: false,
    DisplayLedPaddingChar: "0",
    DisplayLedDigits: [],
    DisplayLedDecimalPoints: [],
    DisplayLedReverseDigits: false,
    DisplayLedBrightnessReference: "",
  }),
  createLcdDisplayConfig: (name: string): LcdDisplayConfig => ({
    Type: "LcdDisplay",
    Name: name,
    Address: name,
    Lines: [],
  }),
  // C# model has no defaults, values taken from legacy ServoPanel
  createServoConfig: (name: string): ServoConfig => ({
    Type: "Servo",
    Name: name,
    Address: name,
    Min: "0",
    Max: "255",
    MaxRotationPercent: "100",
  }),
  createStepperConfig: (name: string): StepperConfig => ({
    Type: "Stepper",
    Name: name,
    Address: name,
    InputRev: 0,
    OutputRev: 0,
    TestValue: 500,
    CompassMode: false,
    Acceleration: 0,
    Speed: 0,
  }),
  createShiftRegisterConfig: (name: string): ShiftRegisterConfig => ({
    Type: "ShiftRegister",
    Name: name,
    Address: name,
    Pin: "",
    Brightness: 255,
    PWM: false,
  }),
  createCustomDeviceConfig: (name: string): CustomDeviceConfig => ({
    Type: "CustomDevice",
    Name: name,
    CustomName: name,
    CustomType: "",
    MessageType: 0,
    Value: "",
  }),
}
