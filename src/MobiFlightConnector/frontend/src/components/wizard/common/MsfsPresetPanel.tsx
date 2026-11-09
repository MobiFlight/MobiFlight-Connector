import PresetPanel from "@/components/wizard/common/PresetPanel"
import PresetSummary from "@/components/wizard/common/PresetSummary"
import { Preset } from "@/types/preset"

import type { PresetAdapter } from "@/types/preset"
import { useHubHopPresets } from "@/lib/hooks/useHubHopPresets"
import PresetDetails from "@/components/wizard/common/PresetDetails"

export type MsfsPresetPanelProps<T> = {
  variant: "summary" | "details"
  adapter: PresetAdapter<T>
  config: T
  onConfigChange?: (config: T) => void
}

const MsfsPresetPanel = <T,>({
  variant,
  adapter,
  config,
  onConfigChange,
}: MsfsPresetPanelProps<T>) => {
  const { data: presets = [], isLoading } = useHubHopPresets("msfs")

  const presetId = adapter.getPresetId(config) ?? null
  const code = adapter.getCode(config) ?? ""
  const preset = presets.find((p) => p.id === presetId) ?? null

  if (variant === "summary") {
    return <PresetSummary label={preset?.label} code={code} />
  }

  return (
    <div className="flex flex-col gap-4">
      <PresetPanel
        sim="msfs"
        mode={adapter.mode}
        isLoading={isLoading}
        selectedPresetId={presetId}
        setSelectedPreset={(selected) =>
          onConfigChange?.(
            adapter.withPreset(config, selected as Preset | null),
          )
        }
      />
      <PresetDetails
        sim="msfs"
        mode={adapter.mode}
        preset={preset}
        code={code}
        onCodeChange={(newCode) =>
          onConfigChange?.(adapter.withCode(config, newCode))
        }
      />
    </div>
  )
}
export default MsfsPresetPanel
