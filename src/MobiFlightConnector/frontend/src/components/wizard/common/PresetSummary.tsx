import { Label } from "@/components/ui/label"
import CodeValueLabel from "@/components/wizard/components/CodeValueLabel"
import { useTranslation } from "react-i18next"

export type PresetSummaryProps = {
  label: string | null | undefined
  code: string | null | undefined
}

const PresetSummary = ({ label, code }: PresetSummaryProps) => {
  const { t } = useTranslation()

  return (
    <div className="flex grow flex-row items-center gap-2">
      <div className="flex w-1/3 flex-col gap-1">
        <Label>
          {t("Dialog.InputConfigWizard.InputActions.Common.Preset.Label")}:
        </Label>
        <div className="text-sm">
          {label ??
            t("Dialog.InputConfigWizard.InputActions.Common.Preset.Custom")}
        </div>
      </div>
      <div className="flex grow flex-col gap-1">
        <Label>
          {t("Dialog.InputConfigWizard.InputActions.Common.CodeLabel")}
        </Label>
        <CodeValueLabel className="max-w-50 lg:max-w-100">
          {code ||
            t("Dialog.InputConfigWizard.InputActions.Common.Preset.Code.None")}
        </CodeValueLabel>
      </div>
    </div>
  )
}
export default PresetSummary
