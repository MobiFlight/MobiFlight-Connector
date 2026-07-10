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
}

export type XplanePreset = Preset & {
  presetType: "Input" | "Output" | "InputOutput" | "Input (Potentiometer)"
  codeType: "DataRef" | "Command"
}
