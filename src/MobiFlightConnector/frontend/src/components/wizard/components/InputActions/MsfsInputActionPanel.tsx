import MsfsPresetPanel from "@/components/wizard/components/InputActions/MsfsPresetPanel"
import { MsfsInputAction } from "@/types/config"
import { fetchHubHopPresets } from "@/lib/configWizard"
import { useQuery } from "@tanstack/react-query"
import MsfsPresetDetails from "@/components/wizard/components/MsfsPresetDetails"
import PresetSummary from "@/components/wizard/common/PresetSummary"

export type MsfsInputActionPanelProps = {
  variant: "summary" | "details"
  config: MsfsInputAction | null
  onConfigChange: (config: MsfsInputAction) => void
}

const MsfsInputActionPanel = ({
  variant,
  config,
  onConfigChange,
}: MsfsInputActionPanelProps) => {
  const { data: presets = [], isLoading } = useQuery({
    queryKey: ["msfs-presets"],
    queryFn: () => fetchHubHopPresets("msfs"),
    // presets don't change at runtime; HubHopState drives invalidation
    staleTime: Infinity,
  })

  const preset = presets.find((p) => p.id === config?.PresetId) ?? null

  if (variant === "summary") {
    return <PresetSummary label={preset?.label} code={config?.Command} />
  }

  return (
    <div className="flex flex-col gap-4">
      <MsfsPresetPanel
        variant="input"
        isLoading={isLoading}
        selectedPresetId={config?.PresetId ?? null}
        setSelectedPreset={(preset) =>
          onConfigChange({
            ...(config as MsfsInputAction),
            PresetId: preset ? preset.id : null,
            Command: preset ? preset.code : null,
          } as MsfsInputAction)
        }
      />
      <MsfsPresetDetails
        command={config?.Command ?? ""}
        preset={preset}
        onCommandChange={(command) =>
          onConfigChange({
            ...(config as MsfsInputAction),
            Command: command,
            PresetId: "", // Clear preset if user manually edits command
          } as MsfsInputAction)
        }
      />
    </div>
  )
}
export default MsfsInputActionPanel
