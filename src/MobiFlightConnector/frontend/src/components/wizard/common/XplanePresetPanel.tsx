import { XplanePreset, XplanePresetAdapter } from "@/types/preset"
import PresetDetails from "@/components/wizard/common/PresetDetails"
import PresetSummary from "@/components/wizard/common/PresetSummary"
import PresetPanel from "@/components/wizard/common/PresetPanel"
import { useHubHopPresets } from "@/lib/hooks/useHubHopPresets"

export type XplanePresetPanelProps<T> = {
  variant: "summary" | "details"
  adapter: XplanePresetAdapter<T>
  config: T
  onConfigChange?: (config: T) => void
}

const XplanePresetPanel = <T,>({
  variant,
  adapter,
  config,
  onConfigChange,
}: XplanePresetPanelProps<T>) => {
  const { data: presets = [], isLoading } = useHubHopPresets("xplane")
  const presetId = adapter.getPresetId(config) ?? null
  const code = adapter.getCode(config) ?? null
  const codeType = adapter.getCodeType(config) ?? null
  const value =
    adapter.mode === "InputConfigItem" ? (adapter.getValue(config) ?? null) : null
  const preset = presets.find((p) => p.code === code) ?? null

  if (variant === "summary") {
    return <PresetSummary label={preset?.label} code={code} />
  }

  return (
    <div className="flex flex-col gap-4">
      <PresetPanel
        sim="xplane"
        mode={adapter.mode}
        isLoading={isLoading}
        selectedPresetId={presetId ?? null}
        setSelectedPreset={(selected) =>
          onConfigChange?.(
            adapter.withPreset(config, selected as XplanePreset | null),
          )
        }
      />
      <PresetDetails
        sim="xplane"
        mode={adapter.mode}
        preset={preset}
        code={code}
        codeType={codeType}
        value={value}
        onCodeChange={(code) =>
          onConfigChange?.(
            adapter.withCode(config, code)
          )
        }
        onCodeTypeChange={
          adapter.mode === "InputConfigItem"
            ? (codeType) =>
                onConfigChange?.(adapter.withCodeType(config, codeType))
            : undefined
        }
        onValueChange={
          adapter.mode === "InputConfigItem"
            ? (value) => onConfigChange?.(adapter.withValue(config, value))
            : undefined
        }
      />
    </div>
  )
}

export default XplanePresetPanel
