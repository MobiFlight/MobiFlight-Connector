import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Separator } from "@/components/ui/separator"
import CodeValueLabel from "@/components/wizard/components/CodeValueLabel"
import ProSimDataRefPanel from "@/components/wizard/components/InputActions/ProsimDataRefPanel"
import { ProSimInputAction } from "@/types/config"
import { useTranslation } from "react-i18next"

export type ProSimInputActionPanelProps = {
  variant: "summary" | "details"
  config: ProSimInputAction | null
  onConfigChange: (config: ProSimInputAction) => void
}

const ProSimInputActionPanel = ({
  variant,
  config,
  onConfigChange,
}: ProSimInputActionPanelProps) => {
  const { t } = useTranslation()

  if (variant === "summary") {
    return (
      <div className="flex grow flex-row items-center gap-8">
        <div className="flex w-1/3 flex-col gap-1">
          <Label htmlFor="preset">
            {t("Dialog.InputConfigWizard.InputActions.ProSim.PathLabel")}:
          </Label>
          <div className="text-sm">
            {config?.Path
              ? config?.Path
              : t(
                  "Dialog.InputConfigWizard.InputActions.ProSim.NoPresetSelected",
                )}
          </div>
        </div>
        <div className="flex grow flex-col gap-1">
          <Label htmlFor="code">
            {t("Dialog.InputConfigWizard.InputActions.ProSim.ParameterLabel")}
          </Label>
          <CodeValueLabel id="code">
            {config?.Expression
              ? config?.Expression
              : t("Dialog.InputConfigWizard.InputActions.ProSim.ParameterNone")}
          </CodeValueLabel>
        </div>
      </div>
    )
  }

  return (
    <div className="flex flex-col gap-4">
      <div className="flex flex-col">
        <div className="text-lg font-semibold">
          {t("Dialog.InputConfigWizard.InputActions.ProSim.Title")}
        </div>
        <div className="text-muted-foreground text-sm">
          {t("Dialog.InputConfigWizard.InputActions.ProSim.Description")}
        </div>
      </div>
      <ProSimDataRefPanel
        variant="input"
        selectedPath={config?.Path ?? null}
        onPresetChange={(preset) =>
          onConfigChange({
            ...(config as ProSimInputAction),
            Path: preset.Name,
          } as ProSimInputAction)
        }
      />
      <Separator />
      <div className="flex flex-col gap-2">
        <Label htmlFor="path">
          {t("Dialog.InputConfigWizard.InputActions.ProSim.PathLabel")}
        </Label>
        <div
          id="path"
          className="rounded border p-2 text-sm"
          data-testid="pathValue"
        >
          {config?.Path && config?.Path !== ""
            ? config?.Path
            : t(
                "Dialog.InputConfigWizard.InputActions.ProSim.NoPresetSelected",
              )}
        </div>
      </div>
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
          value={config?.Expression}
          onChange={(e) =>
            onConfigChange({
              ...(config as ProSimInputAction),
              Expression: e.target.value,
            } as ProSimInputAction)
          }
        />
      </div>
    </div>
  )
}
export default ProSimInputActionPanel
