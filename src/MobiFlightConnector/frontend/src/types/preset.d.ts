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

// Where the MSFS values live in each saved config shape.
export type PresetAdapter<T> = {
  mode: "input" | "output"
  getPresetId: (config: T) => string | null | undefined
  getCode: (config: T) => string | null | undefined
  getCodeType?: (config: T) => PresetCodeType | null | undefined
  getValue?: (config: T) => string | null | undefined
  withPreset: (config: T, preset: Preset | null) => T
  // A manually edited code no longer matches the preset,
  // so the preset id is cleared.
  withCode: (config: T, code: string) => T
  withCodeType?: (config: T, codeType: PresetCodeType) => T
  withValue?: (config: T, value: string) => T
}

export type XplanePresetAdapter<T> = PresetAdapter<T> & {
  getCodeType?: (config: T) => PresetCodeType | null | undefined
  getValue?: (config: T) => string | null | undefined
  withCodeType?: (config: T, codeType: PresetCodeType) => T
  withValue?: (config: T, value: string) => T
}