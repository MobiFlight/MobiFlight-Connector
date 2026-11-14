import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import SectionHeadline from "@/components/wizard/common/SectionHeadline"
import WizardSection from "@/components/wizard/common/WizardSection"
import CodeValueLabel from "@/components/wizard/components/CodeValueLabel"
import ProSimPresetsPanel from "@/components/wizard/components/InputActions/ProsimDataRefPanel"
import { ConfigItemType, ProSimInputAction } from "@/types/config"
import { ProSimInputPresetAdapter, ProSimPresetAdapter } from "@/types/preset"
import { useTranslation } from "react-i18next"

export type ProSimInputActionPanelProps<T> = {
  variant: "summary" | "details"
  mode: ConfigItemType
  config: T
  adapter: ProSimPresetAdapter<T>
  onConfigChange: (config: ProSimInputAction) => void
}

const ProSimPresetPanel = <T,>({
  variant,
  config,
  mode,
  adapter,
  onConfigChange,
}: ProSimInputActionPanelProps<T>) => {
  const { t } = useTranslation()
  const value = adapter.mode === "input" ? adapter.getValue(config) : null
  const getValue = adapter.mode === "input" ? adapter.getValue : null


  if (variant === "summary") {
    return (
      <div className="flex grow flex-row items-center gap-2">
        <div className="flex w-1/2 flex-col gap-1">
          <Label htmlFor="preset">
            {t("Dialog.InputConfigWizard.InputActions.ProSim.PathLabel")}:
          </Label>
          <div className="text-sm">
            {adapter.getPresetId(config)
              ? adapter.getPresetId(config)
              : t(
                  "Dialog.InputConfigWizard.InputActions.ProSim.NoPresetSelected",
                )}
          </div>
        </div>
        {mode === "InputConfigItem" && (
          <div className="flex w-1/2 flex-col gap-1">
            <Label htmlFor="code">
              {t("Dialog.InputConfigWizard.InputActions.ProSim.ParameterLabel")}
            </Label>
            <CodeValueLabel id="code">
              {value
                ? value
                : t(
                    "Dialog.InputConfigWizard.InputActions.ProSim.ParameterNone",
                  )}
            </CodeValueLabel>
          </div>
        )}
      </div>
    )
  }

  return (
    <div className="flex flex-col gap-4">
      <ProSimPresetsPanel
        mode={mode}
        selectedPath={config?.Path ?? null}
        onPresetChange={(preset) =>
          onConfigChange({
            ...(config as ProSimInputAction),
            Path: preset.Name,
          } as ProSimInputAction)
        }
      />
      {/* ProSim Preset Details Panel */}
      <WizardSection>
        <SectionHeadline
          title={t(
            "Dialog.InputConfigWizard.InputActions.Common.Preset.Code.Title",
          )}
          description={t(
            "Dialog.InputConfigWizard.InputActions.Common.Preset.Code.Description",
          )}
        />
        <div className="flex flex-col gap-2">
          <Label htmlFor="path">
            {t("Dialog.InputConfigWizard.InputActions.ProSim.PathLabel")}
          </Label>
          <div
            id="path"
            className="rounded border p-2 text-sm"
            data-testid="prosim-path"
          >
            {config?.Path && config?.Path !== ""
              ? config?.Path
              : t(
                  "Dialog.InputConfigWizard.InputActions.ProSim.NoPresetSelected",
                )}
          </div>
        </div>
        {mode === "InputConfigItem" && (
          <div className="flex flex-col gap-2">
            <Label htmlFor="param">
              {t("Dialog.InputConfigWizard.InputActions.ProSim.ParameterLabel")}
            </Label>
            <Input
              className="font-mono text-sm whitespace-nowrap"
              id="param"
              placeholder={t(
                "Dialog.InputConfigWizard.InputActions.ProSim.ParameterPlaceholder",
              )}
              value={getValue?.(config) ?? undefined}
              onChange={(e) =>
                adapter.mode === "input"
                  ? onConfigChange?.(adapter.withValue(config, e.target.value))
                  : undefined
              }
            />
          </div>
        )}
      </WizardSection>
    </div>
  )
}
export default ProSimPresetPanel
