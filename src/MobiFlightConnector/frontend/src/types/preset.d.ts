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

// Where the MSFS values live in each saved config shape.
export type PresetAdapter<T> = {
  mode: "input" | "output"
  getPresetId: (config: T) => string | null | undefined
  getCode: (config: T) => string | null | undefined
  withPreset: (config: T, preset: Preset | null) => T
  // A manually edited code no longer matches the preset,
  // so the preset id is cleared.
  withCode: (config: T, code: string) => T
  withValue?: (config: T, value: string) => T
}
