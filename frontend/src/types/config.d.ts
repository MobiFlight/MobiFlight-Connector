import { DeviceElementType } from "@/types/deviceElements.d";

export interface IConfigItem {
  GUID: string;
  Active: boolean;
  Description: string;
  Device: string;
  Component: string;
  Type: string;
  Tags: string[];
  Status: string[];
  RawValue: string;
  ModifiedValue: string;
}

interface IDictionary<T> {
  [Key: string]: T;
}

export interface IDeviceElement {
  Id: string;
  Name: string;
  Type: DeviceElementType;
  ConfigData: IDictionary<string>
}

export type DeviceType = "MobiFlight" | "Joystick" | "Midi";

export type ElementPin = {
  isAnalog: boolean;
  isPWM: boolean;
  isI2C: boolean;
  Used: boolean;
  Pin: number;
}

export interface IDeviceItem {
  Id: string;
  Type: DeviceType;
  Name: string;
  MetaData: IDictionary<string>;  
  Elements?: IDeviceElement[];
  Pins: ElementPin[];
}
