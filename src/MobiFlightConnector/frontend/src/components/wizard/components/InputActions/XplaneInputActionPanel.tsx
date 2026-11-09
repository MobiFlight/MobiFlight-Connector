import { fetchHubHopPresets } from "@/lib/configWizard"
import { XplaneInputAction } from "@/types/config"
import { XplanePreset } from "@/types/preset"
import { useQuery } from "@tanstack/react-query"
import XplanePresetDetails from "@/components/wizard/common/XplanePresetDetails"
import PresetSummary from "@/components/wizard/common/PresetSummary"
import PresetPanel from "@/components/wizard/common/PresetPanel"

export type XplaneInputActionPanelProps = {
  variant: "summary" | "details"
  config: XplaneInputAction | null
  onConfigChange: (config: XplaneInputAction) => void
}

const XplaneInputActionPanel = ({
  variant,
  config,
  onConfigChange,
}: XplaneInputActionPanelProps) => {
  const { data: presets = [], isLoading } = useQuery({
    queryKey: ["xplane-presets"],
    queryFn: () => fetchHubHopPresets("xplane") as Promise<XplanePreset[]>,
    // presets don't change at runtime; HubHopState drives invalidation
    staleTime: Infinity,
  })
  const preset = presets.find((p) => p.code === config?.Path) ?? null

  if (variant === "summary") {
    return <PresetSummary label={preset?.label} code={config?.Path} />
  }

  return (
    <div className="flex flex-col gap-4">
      <PresetPanel
        sim="xplane"
        mode="input"
        isLoading={isLoading}
        selectedPresetId={config?.Path ?? null}
        setSelectedPreset={(preset) =>
          onConfigChange({
            ...(config as XplaneInputAction),
            Path: preset ? preset.code : null,
            InputType: preset ? (preset as XplanePreset).codeType : null,
          })
        }
      />
      <XplanePresetDetails
        variant="input"
        preset={preset}
        config={config}
        onConfigChange={onConfigChange}
      />
    </div>
  )
}

export default XplaneInputActionPanel
