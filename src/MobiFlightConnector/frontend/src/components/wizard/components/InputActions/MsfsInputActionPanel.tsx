import MsfsPresetPanel from "@/components/wizard/components/InputActions/MsfsPresetPanel"
import { MsfsInputAction } from "@/types/config"
import { Label } from "@/components/ui/label"
import { useTranslation } from "react-i18next"
import CodeValueLabel from "@/components/wizard/components/CodeValueLabel"
import { fetchHubHopPresets } from "@/lib/configWizard"
import { useQuery } from "@tanstack/react-query"
import MsfsPresetDetails from "@/components/wizard/components/MsfsPresetDetails"

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
  const { t } = useTranslation()

  const { data: presets = [], isLoading } = useQuery({
    queryKey: ["msfs-presets"],
    queryFn: () => fetchHubHopPresets("msfs"),
    // presets don't change at runtime; HubHopState drives invalidation
    staleTime: Infinity,
  })
  
  const preset = presets.find((p) => p.id === config?.PresetId) ?? null

  if (variant === "summary") {
    return (
      <div className="flex grow flex-row items-center gap-2">
        <div className="flex w-1/3 flex-col gap-1">
          <Label htmlFor="preset">
            {t("Dialog.InputConfigWizard.InputActions.Common.Preset.Label")}:
          </Label>
          <div className="text-sm">
            {preset?.label ??
              t("Dialog.InputConfigWizard.InputActions.Common.Preset.Custom")}
          </div>
        </div>
        <div className="flex grow flex-col gap-1">
          <Label htmlFor="code">
            {t("Dialog.InputConfigWizard.InputActions.Common.CodeLabel")}
          </Label>
          <CodeValueLabel id="code" className="max-w-50 lg:max-w-100">
            {config?.Command ??
              t(
                "Dialog.InputConfigWizard.InputActions.Common.Preset.Code.None",
              )}
          </CodeValueLabel>
        </div>
      </div>
    )
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
