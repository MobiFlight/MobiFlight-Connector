import { MsfsInputAction, SimConnectSource, XplaneInputAction, XplaneSource } from "@/types/config"
import type { PresetAdapter } from "@/types/preset"

export const msfsInputActionAdapter: PresetAdapter<MsfsInputAction> = {
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

export const msfsSourceAdapter: PresetAdapter<SimConnectSource> = {
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

export const xplaneInputActionAdapter: PresetAdapter<XplaneInputAction> = {
  mode: "input",
  getPresetId: (action) => action.Path,
  getCode: (action) => action.Path,
  getCodeType: (action) => action.InputType,
  withPreset: (action, preset) =>
    ({
      ...action,
      Path: preset?.code ?? null,
    }) as XplaneInputAction,
  withCode: (action, code) => ({ ...action, Path: code }),
  withCodeType: (action, codeType) => ({ ...action, InputType: codeType }),
  withValue: (action, value) => ({ ...action, Expression: value }),
}

export const xplaneSourceAdapter: PresetAdapter<XplaneSource> = {
  mode: "output",
  getPresetId: (source) => source.XplaneDataRef.Path,
  getCode: (source) => source.XplaneDataRef.Path,
  getCodeType: () => "DataRef",
  withPreset: (source, preset) =>
    ({
      ...source,
      XplaneDataRef: {
        ...source.XplaneDataRef,
        Path: preset?.code ?? null,
      },
    }) as XplaneSource,
  withCode: (source, code) => ({
    ...source,
    XplaneDataRef: { ...source.XplaneDataRef, Path: code },
  })
}