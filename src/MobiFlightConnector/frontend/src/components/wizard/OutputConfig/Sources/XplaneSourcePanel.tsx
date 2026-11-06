import PresetSummary from "@/components/wizard/common/PresetSummary"
import XplanePresetDetails from "@/components/wizard/components/InputActions/XplanePresetDetails"
import XplanePresetPanel from "@/components/wizard/components/InputActions/XplanePresetPanel"
import { fetchHubHopPresets } from "@/lib/configWizard"
import { XplaneInputAction, XplaneSource } from "@/types/config"
import { XplanePreset } from "@/types/preset"
import { useQuery } from "@tanstack/react-query"

export interface XplaneSourcePanelProps {
  variant: "summary" | "details"
  source: XplaneSource
  onSourceChange: (source: XplaneSource) => void
}

const XplaneSourcePanel = ({
  variant,
  source,
  onSourceChange,
}: XplaneSourcePanelProps) => {
  const { data: presets = [], isLoading } = useQuery({
    queryKey: ["xplane-presets"],
    queryFn: () => fetchHubHopPresets("xplane") as Promise<XplanePreset[]>,
    // presets don't change at runtime; HubHopState drives invalidation
    staleTime: Infinity,
  })
  const preset =
    presets.find((p) => p.code === source?.XplaneDataRef.Path) ?? null

  if (variant === "summary") {
    return (
      <PresetSummary
        label={preset?.label ?? null}
        code={source?.XplaneDataRef?.Path ?? null}
      />
    )
  }

  // refactor alert!
  const inputActionShape = {
    InputType: "DataRef",
    Path: source?.XplaneDataRef?.Path ?? "",
  } as XplaneInputAction

  return (
    <div className="flex flex-col gap-4">
      <XplanePresetPanel
        variant="output"
        isLoading={isLoading}
        selectedPath={source?.XplaneDataRef?.Path ?? null}
        setSelectedPreset={(preset) =>
          onSourceChange({
            ...source,
            XplaneDataRef: {
              ...((source as XplaneSource)?.XplaneDataRef ?? {}),
              Path: preset ? preset.code : "",
            },
          })
        }
      />
      <XplanePresetDetails
        variant="output"
        preset={preset}
        config={inputActionShape}
        onConfigChange={(config) =>
          onSourceChange({
            ...source,
            XplaneDataRef: {
              Path: config.Path ?? "",
            },
          })
        }
      />
    </div>
  )
}

export default XplaneSourcePanel
