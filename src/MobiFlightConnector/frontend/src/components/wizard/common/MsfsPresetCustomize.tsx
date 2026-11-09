import { Label } from "@/components/ui/label"
import { Textarea } from "@/components/ui/textarea"
import { useTranslation } from "react-i18next"

type MsfsPresetCustomizeProps = {
  code: string | null
  onCodeChange: (code: string) => void
}

const MsfsPresetCustomize = ({
  code,
  onCodeChange
}: MsfsPresetCustomizeProps) => {
  const { t } = useTranslation()

  return (
    <div className="flex flex-col gap-1">
      <Label htmlFor="code">
        {t("Dialog.InputConfigWizard.InputActions.Common.CodeLabel")}
      </Label>
      <Textarea
        name="code"
        className="dark:focus:bg-input/20 font-mono text-sm whitespace-nowrap dark:bg-transparent"
        placeholder={t(
          "Dialog.InputConfigWizard.InputActions.Common.Preset.Code.Placeholder",
        )}
        value={code ?? ""}
        onChange={(e) => onCodeChange(e.target.value)}
      />
      <div className="text-muted-foreground text-sm">
        {t(
          "Dialog.InputConfigWizard.InputActions.Common.SupportedPlaceholders",
        )}
      </div>
    </div>
  )
}

export default MsfsPresetCustomize