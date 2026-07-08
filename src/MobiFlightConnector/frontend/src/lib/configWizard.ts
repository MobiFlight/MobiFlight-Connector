import { ActionTypeOption } from "@/components/wizard/components/ActionTypeComboBox"
import { Preset } from "@/components/wizard/components/InputActions/MsfsPresetPanel"

export const ActionTypeOptions: ActionTypeOption[] = [
  {
    value: "MSFS2020CustomInputAction",
    isAvailable: (settings) => settings.Sim === "msfs",
  },
  {
    value: "XplaneInputAction",
    isAvailable: (settings) => settings.Sim === "xplane",
  },
  {
    value: "ProSimInputAction",
    isAvailable: (settings) => settings.Features?.ProSim ?? false,
  },
  {
    value: "VariableInputAction",
    isAvailable: () => true,
  },
  {
    value: "RetriggerInputAction",
    isAvailable: () => true,
  },
  { value: "KeyInputAction", isAvailable: () => true },
  {
    value: "VJoyInputAction",
    isAvailable: () => true,
  },
  {
    value: "FsuipcOffsetInputAction",
    isAvailable: (settings) => settings.Features?.FSUIPC ?? false,
  },
  {
    value: "PmdgEventIdInputAction",
    isAvailable: (settings) => settings.Features?.FSUIPC ?? false,
  },
  {
    value: "LuaMacroInputAction",
    isAvailable: (settings) => settings.Features?.FSUIPC ?? false,
  },
  {
    value: "JeehellInputAction",
    isAvailable: (settings) => settings.Features?.FSUIPC ?? false,
  },
  {
    value: "EventIdInputAction",
    isAvailable: (settings) => settings.Features?.FSUIPC ?? false,
  },
]

export const parsePresets = (content: string) => {
  const lines = content.split("\n")
  return lines
    .map((line) => {
      const [name, eventId, description] = line
        .split(":")
        .map((part) => part.trim())
      const isGroup = eventId === "GROUP"
      if (name && eventId && !isGroup) {
        return { name, eventId: eventId.toString(), description }
      }
      return null
    })
    .filter(
      (item): item is { name: string; eventId: string; description: string } =>
        item !== null,
    )
}

export const fetchHubHopPresets = async (sim: "msfs" | "xplane") => {
  const presetFile =
    sim === "msfs"
      ? "/presets/msfs2020_hubhop_presets.json"
      : "/presets/xplane_hubhop_presets.json"
  return fetch(presetFile).then((r) => r.json() as Promise<Preset[]>)
}
