export type DeviceElementType =
  | "Button"
  | "Output"
  | "Encoder"
  | "LedModule"
  | "Stepper"
  | "Servo"
  | "LcdDisplay"
  | "ShiftRegister"
  | "AnalogInput"
  | "InputShiftRegister"
  | "InputMultiplexer"
  | "CustomDevice";

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
];

export const DeviceElementRequiredPins: { [key in DeviceElementType]: { Pins: number, PwmPins: number, i2c: boolean } } = {
  "Button": { Pins: 1, PwmPins: 0 },
  "Encoder": { Pins: 2, PwmPins: 0 },
  "AnalogInput": { Pins: 1, PwmPins: 0 },
  "InputShiftRegister": { Pins: 4, PwmPins: 0 },
  "InputMultiplexer": { Pins: 4, PwmPins: 0 },
  "Output": { Pins: 1, PwmPins: 0 },
  "LedModule": { Pins: 4, PwmPins: 0 },
  "Stepper": { Pins: 4, PwmPins: 0 },
  "Servo": { Pins: 1, PwmPins: 1 },
  "LcdDisplay": { Pins: 0, PwmPins: 0, i2c: true},
  "ShiftRegister": { Pins: 4, PwmPins: 0 },
  "CustomDevice": { Pins: 1, PwmPins: 0 },
};


export interface IDeviceElement {
  Id: string;
  Name: string;
  Type: DeviceElementType;
  ConfigData: IDictionary<string>
}

export interface ButtonElement extends IDeviceElement {
  Type: "Button";
  ConfigData: {
    Pin: string;
    Pullup: string;
  };

}

export type StepperProfilePreset = {
  id : string
  Mode: string
  Speed: number
  Acceleration: number
  Backlash: number
  StepsPerRevolution: number
}

export const StepperProfilesPresets : { label : string, value: StepperProfilePreset }[] = [
  { label: "28BYJ - Half-step mode (recommended)", value: { id: "1", Mode: "0", Speed: 1400, Acceleration: 2800, Backlash: 6, StepsPerRevolution: 4096 } },
  { label: "28BYJ - Full-step mode (classic)", value: { id: "0", Mode: "1", Speed: 467, Acceleration: 800, Backlash: 3, StepsPerRevolution: 2040 } },
  { label: "x.27 - Half-step mode", value: { id: "2", Mode: "0", Speed: 100, Acceleration: 100, Backlash: 0, StepsPerRevolution: 1100 } },
  { label: "Generic - EasyDriver", value: { id: "3", Mode: "2", Speed: 400, Acceleration: 800, Backlash: 0, StepsPerRevolution: 1000 } },
  { label: "Custom Stepper", value: { id: "255", Mode: "0", Speed: 400, Acceleration: 800, Backlash: 0, StepsPerRevolution: 1000 } },
] 
  