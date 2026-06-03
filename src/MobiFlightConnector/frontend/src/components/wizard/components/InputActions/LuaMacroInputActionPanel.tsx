import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { LuaMacroInputAction } from "@/types/config"
import { useTranslation } from "react-i18next"

export type LuaMacroInputActionPanelProps = {
  config: LuaMacroInputAction | null
  onConfigChange: (config: LuaMacroInputAction) => void
}

const LuaMacroInputActionPanel = ({
  config,
  onConfigChange,
}: LuaMacroInputActionPanelProps) => {
  const { t } = useTranslation()
  return (
    <div className="flex flex-col gap-4">
      <div className="flex flex-col">
        <div className="text-lg font-semibold">
          {t("Wizard.InputActions.LuaMacro.Title")}
        </div>
        <div className="text-muted-foreground text-sm">
          {t("Wizard.InputActions.LuaMacro.Description")}
        </div>
      </div>
      <div className="flex flex-col gap-2">
      <Label htmlFor="macroName">{t("Wizard.InputActions.LuaMacro.MacroNameLabel")}</Label>
      <Input
        id="macroName"
        placeholder={t("Wizard.InputActions.LuaMacro.MacroNamePlaceholder")}
        value={config?.MacroName ?? ""}
        onChange={(e) =>
          onConfigChange({
            ...(config as LuaMacroInputAction),
            MacroName: e.target.value,
          } as LuaMacroInputAction)
        }
      />
      </div>
      <div className="flex flex-col gap-2">
      <Label htmlFor="macroValue">{t("Wizard.InputActions.LuaMacro.MacroValueLabel")}</Label>
      <Input
        id="macroValue"
        placeholder={t("Wizard.InputActions.LuaMacro.MacroValuePlaceholder")}
        value={config?.MacroValue ?? ""}
        onChange={(e) =>
          onConfigChange({
            ...(config as LuaMacroInputAction),
            MacroValue: e.target.value,
          } as LuaMacroInputAction)
        }
      />
      </div>
    </div>
  )
}
export default LuaMacroInputActionPanel