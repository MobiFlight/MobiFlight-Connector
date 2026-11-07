import { MsfsInputAction, SimConnectSource } from "@/types/config"
import type { MsfsPresetAdapter } from "@/types/preset"

export const msfsInputActionAdapter: MsfsPresetAdapter<MsfsInputAction> = {
  mode: "input",
  getPresetId: (action) => action.PresetId,
  getCode: (action) => action.Command,
  withPreset: (action, preset) =>
    ({
      ...action,
      PresetId: preset?.id ?? null,
      Command: preset?.code ?? null,
    }) as MsfsInputAction,
  withCode: (action, code) => ({ ...action, Command: code, PresetId: "" }),
}

export const msfsSourceAdapter: MsfsPresetAdapter<SimConnectSource> = {
  mode: "output",
  getPresetId: (source) => source.SimConnectValue?.UUID,
  getCode: (source) => source.SimConnectValue?.Value,
  // SimConnectValue is spread, so properties like VarType are kept.
  withPreset: (source, preset) =>
    ({
      ...source,  
      SimConnectValue: {
        ...source.SimConnectValue,
        UUID: preset?.id ?? null,
        Value: preset?.code ?? null,
      },
    }) as SimConnectSource,
  withCode: (source, code) => ({
    ...source,
    SimConnectValue: { ...source.SimConnectValue, Value: code, UUID: "" },
  }),
}