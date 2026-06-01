import ComboBox from "@/components/ComboBox"
import { useQuery } from "@tanstack/react-query"

export type EventIdPresetsPanelProps = {
  variant: "default" | "pmdg"
  aircraft?: "B737" | "B747" | "B777"
  selectedPresetId: string | null
  setSelectedPreset: (preset: { name: string; eventId: string } | null) => void
}

const presetUrls = {
  default: "/presets/presets_eventids.cip",
  pmdg: {
    B737: "/presets/presets_eventids_pmdg_737.cip",
    B747: "/presets/presets_eventids_pmdg_747.cip",
    B777: "/presets/presets_eventids_pmdg_777.cip",
  },
}

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

const EventIdPresetsPanel = ({
  variant,
  aircraft,
  selectedPresetId,
  setSelectedPreset,
}: EventIdPresetsPanelProps) => {
  const presetUrl =
    variant === "default"
      ? presetUrls.default
      : presetUrls.pmdg[aircraft || "B737"]
  // In MsfsPresetPanel (or a dedicated hook)
  const { data: presets = [] /*, isLoading */ } = useQuery({
    queryKey: [`presets-${variant}-${aircraft}`],
    queryFn: () =>
      fetch(presetUrl)
        .then((r) => r.text())
        .then((content) => parsePresets(content)) as Promise<
        { name: string; eventId: string; description: string }[]
      >,
    staleTime: Infinity, // presets don't change at runtime; HubHopState drives invalidation
  })

  const selectedPreset = presets.find(
    (item) => item.eventId === selectedPresetId?.toString(),
  )

  return (
    <div>
      <div>Select Preset</div>
      <ComboBox
        selected={selectedPreset}
        placeholder="Select preset..."
        getLabel={(item) => item.name}
        getValue={(item) => item.eventId}
        items={presets}
        isSelected={(item) => item.eventId === selectedPreset?.eventId}
        setSelected={(item) => setSelectedPreset(item ? item : null)}
        widthClass="w-100"
      />
    </div>
  )
}

export default EventIdPresetsPanel
