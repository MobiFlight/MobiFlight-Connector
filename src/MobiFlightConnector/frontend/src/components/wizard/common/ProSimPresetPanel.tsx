import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import SectionHeadline from "@/components/wizard/common/SectionHeadline"
import WizardSection from "@/components/wizard/common/WizardSection"
import CodeValueLabel from "@/components/wizard/common/CodeValueLabel"
import ProSimPresetListPanel from "@/components/wizard/common/ProSimPresetListPanel"
import { ProSimPresetAdapter } from "@/types/preset"
import { useTranslation } from "react-i18next"

export type ProSimPresetPanelProps<T> = {
  variant: "summary" | "details"
  adapter: ProSimPresetAdapter<T>
  config: T
  onConfigChange?: (config: T) => void
}

export const ProSimPresetPanel = <T,>({
  variant,
  adapter,
  config,
  onConfigChange,
}: ProSimPresetPanelProps<T>) => {
  const { t } = useTranslation()
  const path = adapter.getPresetId(config) || null
  const value =
    adapter.mode === "InputConfigItem" ? (adapter.getValue(config) ?? null) : null

  if (variant === "summary") {
    return (
      <div className="flex grow flex-row items-center gap-2">
        <div className="flex w-1/2 flex-col gap-1">
          <Label htmlFor="preset">
            {t("Dialog.InputConfigWizard.InputActions.ProSim.PathLabel")}:
          </Label>
          <div className="text-sm">
            {path ??
              t("Dialog.InputConfigWizard.InputActions.ProSim.NoPresetSelected")}
          </div>
        </div>
        {adapter.mode === "InputConfigItem" && (
          <div className="flex w-1/2 flex-col gap-1">
            <Label htmlFor="code">
              {t("Dialog.InputConfigWizard.InputActions.ProSim.ParameterLabel")}
            </Label>
            <CodeValueLabel id="code">
              {value ||
                t("Dialog.InputConfigWizard.InputActions.ProSim.ParameterNone")}
            </CodeValueLabel>
          </div>
        )}
      </div>
    )
  }

  return (
    <div className="flex flex-col gap-4">
      <ProSimPresetListPanel
        mode={adapter.mode}
        selectedPath={path}
        onPresetChange={(preset) =>
          onConfigChange?.(adapter.withPreset(config, preset))
        }
      />
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
            {path ??
              t("Dialog.InputConfigWizard.InputActions.ProSim.NoPresetSelected")}
          </div>
        </div>
        {adapter.mode === "InputConfigItem" && (
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
              value={value ?? ""}
              onChange={(e) =>
                onConfigChange?.(adapter.withValue(config, e.target.value))
              }
            />
          </div>
        )}
      </WizardSection>
    </div>
  )
}
export default ProSimPresetPanel
