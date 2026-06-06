import { ActionTypeOption } from "@/components/wizard/components/ActionTypeComboBox";

export const ActionTypeOptions: ActionTypeOption[] = [
  {
    value: "MSFS2020CustomInputAction",
    label: "MSFS2020CustomInputAction",
    isAvailable: (settings) => settings.Sim === "msfs",
  },
  {
    value: "XplaneInputAction",
    label: "XplaneInputAction",
    isAvailable: (settings) => settings.Sim === "xplane",
  },
  {
    value: "ProSimInputAction",
    label: "ProSimInputAction",
    isAvailable: (settings) => settings.Features?.ProSim ?? false,
  },
  {
    value: "VariableInputAction",
    label: "VariableInputAction",
    isAvailable: () => true,
  },
  {
    value: "RetriggerInputAction",
    label: "RetriggerInputAction",
    isAvailable: () => true,
  },
  { value: "KeyInputAction", label: "KeyInputAction", isAvailable: () => true },
  {
    value: "VJoyInputAction",
    label: "VJoyInputAction",
    isAvailable: () => true,
  },
  {
    value: "FsuipcOffsetInputAction",
    label: "FsuipcOffsetInputAction",
    isAvailable: (settings) => settings.Features?.FSUIPC ?? false,
  },
  {
    value: "PmdgEventIdInputAction",
    label: "PmdgEventIdInputAction",
    isAvailable: (settings) => settings.Features?.FSUIPC ?? false,
  },
  {
    value: "LuaMacroInputAction",
    label: "LuaMacroInputAction",
    isAvailable: (settings) => settings.Features?.FSUIPC ?? false,
  },
  {
    value: "JeehellInputAction",
    label: "JeehellInputAction",
    isAvailable: (settings) => settings.Features?.FSUIPC ?? false,
  },
  {
    value: "EventIdInputAction",
    label: "EventIdInputAction",
    isAvailable: (settings) => settings.Features?.FSUIPC ?? false,
  },
]

export const parsePresets = (content: string) => {
  const lines = content.split("\n")
  return lines
    .map((line) => {
      const [name, eventId, description] = line.split(":").map((part) => part.trim())
      const isGroup = eventId === "GROUP"
      if (name && eventId && !isGroup) {
        return { name, eventId: eventId.toString(), description }
      }
      return null
    })
    .filter((item): item is { name: string; eventId: string; description: string } => item !== null)
}