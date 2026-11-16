import { ProSimDataRefDefinition } from "@/stores/prosimDataRefStore"
import { ConfigItemType } from "@/types/config"

export type Preset = {
  id: string
  vendor: string
  aircraft: string
  system: string
  label: string
  description: string
  code: string
  version: number
  status: string
  createdDate: string
  updatedBy?: string
  reported?: number
  score?: number
  presetType: "Input" | "Output" | "Input (Potentiometer)"
  author?: string
}

export type XplanePreset = Preset & {
  presetType: "Input" | "Output" | "InputOutput" | "Input (Potentiometer)"
  codeType: "DataRef" | "Command"
}

type PresetSim = "msfs" | "xplane"
type PresetType = "input" | "input (potentiometer)" | "output" | "inputoutput"
type PresetCodeType = "DataRef" | "Command"

// Where the preset values live in each saved config shape.
// Base for all sims: only what every preset panel needs.
export type PresetAdapter<T, P = Preset> = {
  mode: ConfigItemType
  getPresetId: (config: T) => string | null | undefined
  getCode: (config: T) => string | null | undefined
  withPreset: (config: T, preset: P | null) => T
  // A manually edited code no longer matches the preset,
  // so the preset id is cleared.
  withCode: (config: T, code: string) => T
}

export type MsfsPresetAdapter<T> = PresetAdapter<T>

// X-Plane input actions can be a DataRef or a Command,
// and a DataRef input writes a value.
export type XplaneInputPresetAdapter<T> = PresetAdapter<T, XplanePreset> & {
  mode: "InputConfigItem"
  getCodeType: (config: T) => PresetCodeType | null | undefined
  withCodeType: (config: T, codeType: PresetCodeType) => T
  getValue: (config: T) => string | null | undefined
  withValue: (config: T, value: string) => T
}

// X-Plane outputs always read a DataRef.
export type XplaneOutputPresetAdapter<T> = PresetAdapter<T, XplanePreset> & {
  mode: "OutputConfigItem"
  getCodeType: (config: T) => "DataRef"
}

export type XplanePresetAdapter<T> =
  | XplaneInputPresetAdapter<T>
  | XplaneOutputPresetAdapter<T>

export type ProSimOutputPresetAdapter<T> = PresetAdapter<T, ProSimDataRefDefinition> & {
  mode: "OutputConfigItem"
}

export type ProSimInputPresetAdapter<T> = PresetAdapter<T, ProSimDataRefDefinition> & {
  mode: "InputConfigItem"
  getValue: (config: T) => string | null | undefined
  withValue: (config: T, value: string) => T
}

export type ProSimPresetAdapter<T> =
  | ProSimInputPresetAdapter<T>
  | ProSimOutputPresetAdapter<T>