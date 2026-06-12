import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Separator } from "@/components/ui/separator"
import ProSimDataRefPanel from "@/components/wizard/components/InputActions/ProsimDataRefPanel"
import { ProSimInputAction } from "@/types/config"
import { useTranslation } from "react-i18next"

export type ProSimInputActionPanelProps = {
  config: ProSimInputAction | null
  onConfigChange: (config: ProSimInputAction) => void
}

const ProSimInputActionPanel = ({
  config,
  onConfigChange,
}: ProSimInputActionPanelProps) => {
  const { t } = useTranslation()
  return (
    <div className="flex flex-col gap-4">
      <div className="flex flex-col">
        <div className="text-lg font-semibold">{t("Dialog.InputConfigWizard.InputActions.ProSim.Title")}</div>
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
        <Label htmlFor="path">{t("Dialog.InputConfigWizard.InputActions.ProSim.PathLabel")}</Label>
        <div id="path" className="rounded border p-2 text-sm">
          {config?.Path !== "" ? config?.Path : t("Dialog.InputConfigWizard.InputActions.ProSim.NoPresetSelected")}
        </div>
      </div>
      <div className="flex flex-col gap-2">
        <Label htmlFor="param">{t("Dialog.InputConfigWizard.InputActions.ProSim.ParameterLabel")}</Label>
        <Input
          id="param"
          placeholder={t("Dialog.InputConfigWizard.InputActions.ProSim.ParameterPlaceholder")}
          value={config?.Expression ?? ""}
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
