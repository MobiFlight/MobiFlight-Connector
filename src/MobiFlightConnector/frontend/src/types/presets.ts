import {
  MsfsInputAction,
  ProSimInputAction,
  ProSimSource,
  SimConnectSource,
  XplaneInputAction,
  XplaneSource,
} from "@/types/config"
import type {
  MsfsPresetAdapter,
  ProSimInputPresetAdapter,
  ProSimOutputPresetAdapter,
  XplaneInputPresetAdapter,
  XplaneOutputPresetAdapter,
} from "@/types/preset"

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

export const xplaneInputActionAdapter: XplaneInputPresetAdapter<XplaneInputAction> =
  {
    mode: "input",
    getPresetId: (action) => action.Path,
    getCode: (action) => action.Path,
    getCodeType: (action) => action.InputType,
    getValue: (action) => action.Expression,
    withPreset: (action, preset) =>
      ({
        ...action,
        Path: preset?.code ?? null,
        InputType: preset?.codeType ?? null,
      }) as XplaneInputAction,
    withCode: (action, code) => ({ ...action, Path: code }),
    withCodeType: (action, codeType) => ({ ...action, InputType: codeType }),
    withValue: (action, value) => ({ ...action, Expression: value }),
  }

export const xplaneSourceAdapter: XplaneOutputPresetAdapter<XplaneSource> = {
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
  }),
}

export const proSimInputActionAdapter: ProSimInputPresetAdapter<ProSimInputAction> =
  {
    mode: "input",
    getPresetId: (action) => action.Path,
    getCode: (action) => action.Path,
    getValue: (action) => action.Expression,
    withPreset: (action, preset) =>
      ({
        ...action,
        Path: preset?.Name ?? null,
      }) as ProSimInputAction,
    withCode: (action, code) => ({ ...action, Path: code }),
    withValue: (action, value) => ({ ...action, Expression: value }),
  }

export const proSimSourceAdapter: ProSimOutputPresetAdapter<ProSimSource> = {
  mode: "output",
  getPresetId: (source) => source.ProSimDataRef.Path,
  getCode: (source) => source.ProSimDataRef.Path,
  withPreset: (source, preset) =>
    ({
      ...source,
      ProSimDataRef: {
        Path: preset?.Name ?? "",
      },
    }) as ProSimSource,
  withCode: (source, code) => ({
    ...source,
    ProSimDataRef: { ...source.ProSimDataRef, Path: code },
  }),
}
