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
  Type: string;
}

export type DeviceType = "MobiFlight" | "Joystick" | "Midi";

export interface IDeviceItem {
  Id: string;
  Type: DeviceType;
  Name: string;
  MetaData: IDictionary<string>;
  Elements: IDeviceElement[];
}
