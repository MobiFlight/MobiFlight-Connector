import { Label } from "@/components/ui/label"
import CodeValueLabel from "@/components/wizard/components/CodeValueLabel"
import { XplaneInputAction } from "@/types/config"
import { XplanePreset } from "@/types/preset"
import { useTranslation } from "react-i18next"

export interface XplanePresetSummaryProps {
  preset: XplanePreset | null
  config: Partial<XplaneInputAction> | null
}

const XplanePresetSummary = ({ preset, config }: XplanePresetSummaryProps) => {
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
        <CodeValueLabel id="code" className="max-w-100">
          {config?.Path ??
            t("Dialog.InputConfigWizard.InputActions.Common.Preset.Code.None")}
        </CodeValueLabel>
      </div>
    </div>
  )
}
export default XplanePresetSummary
