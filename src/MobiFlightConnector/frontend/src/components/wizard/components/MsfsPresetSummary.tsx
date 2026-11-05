import { useTranslation } from "react-i18next"
import { Label } from "@/components/ui/label"
import CodeValueLabel from "@/components/wizard/components/CodeValueLabel"

export type MsfsPresetSummaryProps = {
  preset: { label: string } | null
  config: { Command: string } | null
}

const MsfsPresetSummary = ({ preset, config }: MsfsPresetSummaryProps) => {
  const { t } = useTranslation()
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
export default MsfsPresetSummary