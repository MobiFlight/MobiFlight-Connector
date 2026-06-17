import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { LuaMacroInputAction } from "@/types/config"
import { useTranslation } from "react-i18next"

export type LuaMacroInputActionPanelProps = {
  variant: "summary" | "details"
  config: LuaMacroInputAction | null
  onConfigChange: (config: LuaMacroInputAction) => void
}

const LuaMacroInputActionPanel = ({
  variant,
  config,
  onConfigChange,
}: LuaMacroInputActionPanelProps) => {
  const { t } = useTranslation()

  if (variant === "summary") {
    return (
      <div className="flex grow flex-row items-center justify-between gap-8">
        <div className="flex grow flex-row items-center gap-8">
          <div className="flex flex-col gap-1 w-1/4">
            <Label htmlFor="macroName">
              {t(
                "Dialog.InputConfigWizard.InputActions.LuaMacro.MacroNameLabel",
              )}
            </Label>
            <div>{config?.MacroName ?? "-"}</div>
          </div>
          <div className="flex flex-col gap-1 w-1/4">
            <Label htmlFor="macroValue">
              {t(
                "Dialog.InputConfigWizard.InputActions.LuaMacro.MacroValueLabel",
              )}
            </Label>
            <div>{config?.MacroValue ?? "-"}</div>
          </div>
        </div>
      </div>
    )
  }

  return (
    <div className="flex flex-col gap-4">
      <div className="flex flex-col">
        <div className="text-lg font-semibold">
          {t("Dialog.InputConfigWizard.InputActions.LuaMacro.Title")}
        </div>
        <div className="text-muted-foreground text-sm">
          {t("Dialog.InputConfigWizard.InputActions.LuaMacro.Description")}
        </div>
      </div>
      <div className="flex flex-col gap-2">
        <Label htmlFor="macroName">
          {t("Dialog.InputConfigWizard.InputActions.LuaMacro.MacroNameLabel")}
        </Label>
        <Input
          id="macroName"
          placeholder={t(
            "Dialog.InputConfigWizard.InputActions.LuaMacro.MacroNamePlaceholder",
          )}
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
        <Label htmlFor="macroValue">
          {t("Dialog.InputConfigWizard.InputActions.LuaMacro.MacroValueLabel")}
        </Label>
        <Input
          id="macroValue"
          placeholder={t(
            "Dialog.InputConfigWizard.InputActions.LuaMacro.MacroValuePlaceholder",
          )}
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
