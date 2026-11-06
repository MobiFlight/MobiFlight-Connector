import PresetPanel from "@/components/wizard/common/PresetPanel"
import PresetSummary from "@/components/wizard/common/PresetSummary"
import MsfsPresetDetails from "@/components/wizard/components/MsfsPresetDetails"
import { fetchHubHopPresets } from "@/lib/configWizard"
import { SimConnectSource } from "@/types/config"
import { useQuery } from "@tanstack/react-query"

export interface MsfsSourcePanelProps {
  variant: "summary" | "details"
  source: SimConnectSource
  onSourceChange: (source: SimConnectSource) => void
}

const MsfsSourcePanel = ({
  variant,
  source,
  onSourceChange,
}: MsfsSourcePanelProps) => {
  const { data: presets = [], isLoading } = useQuery({
    queryKey: ["msfs-presets"],
    queryFn: () => fetchHubHopPresets("msfs"),
    // presets don't change at runtime; HubHopState drives invalidation
    staleTime: Infinity,
  })

  const preset =
    presets.find(
      (p) => p.id === (source as SimConnectSource)?.SimConnectValue.UUID,
    ) ?? null

  if (variant === "summary") {
    return (
      <PresetSummary
        label={preset?.label}
        code={source?.SimConnectValue?.Value ?? null}
      />
    )
  }

  return (
    <>
      <PresetPanel
        sim="msfs"
        mode="output"
        isLoading={isLoading}
        selectedPresetId={preset?.id ?? null}
        setSelectedPreset={(preset) => {
          onSourceChange({
            ...source,
            SimConnectValue: {
              UUID: preset?.id ?? null,
              Value: preset?.code ?? null,
            },
          } as SimConnectSource)
        }}
      />
      <MsfsPresetDetails
        command={(source as SimConnectSource)?.SimConnectValue?.Value ?? ""}
        preset={preset}
        onCommandChange={(command) => {
          onSourceChange({
            ...source,
            SimConnectValue: {
              ...((source as SimConnectSource)?.SimConnectValue ?? {}),
              Value: command,
            },
          } as SimConnectSource)
        }}
      />
    </>
  )
}
export default MsfsSourcePanel
