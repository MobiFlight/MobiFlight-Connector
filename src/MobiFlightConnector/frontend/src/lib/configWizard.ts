import { ActionTypeOption } from "@/components/wizard/components/ActionTypeComboBox"
import { Preset } from "@/types/preset"

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

// Preset objects are stable references while their data doesn't change
// (they're held by the TanStack Query cache with staleTime: Infinity), so a
// WeakMap actually hits here and saves rebuilding the lowercased haystack
// string on every filter pass across ~25k presets.
const haystackCache = new WeakMap<object, string>()

const presetHaystack = (preset: Preset) => {
  let haystack = haystackCache.get(preset)
  if (haystack === undefined) {
    haystack =
      `${preset.label} ${preset.description ?? ""} ${preset.code}`.toLowerCase()
    haystackCache.set(preset, haystack)
  }
  return haystack
}

export const filterPresetByText = (preset: Preset, filter: string) => {
  const terms = filter.toLowerCase().trim().split(/\s+/).filter(Boolean)
  if (terms.length === 0) return true
  const haystack = presetHaystack(preset)
  return terms.every((t) => haystack.includes(t))
}
