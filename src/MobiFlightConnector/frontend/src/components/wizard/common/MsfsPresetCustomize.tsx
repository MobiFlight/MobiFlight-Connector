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
        {t("ConfigWizard.Input.InputActions.Common.CodeLabel")}
      </Label>
      <Textarea
        name="code"
        spellCheck={false}
        className="dark:focus:bg-input/20 font-mono text-sm wrap-break-word dark:bg-transparent"
        placeholder={t(
          "ConfigWizard.Input.InputActions.Common.Preset.Code.Placeholder",
        )}
        value={code ?? ""}
        onChange={(e) => onCodeChange(e.target.value)}
      />
      <div className="text-muted-foreground text-sm">
        {t(
          "ConfigWizard.Input.InputActions.Common.SupportedPlaceholders",
        )}
      </div>
    </div>
  )
}

export default MsfsPresetCustomize