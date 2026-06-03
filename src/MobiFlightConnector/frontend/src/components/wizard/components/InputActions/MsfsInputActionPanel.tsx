import { Textarea } from "@/components/ui/textarea"
import MsfsPresetPanel from "@/components/wizard/components/InputActions/MsfsPresetPanel"
import { MsfsInputAction } from "@/types/config"
import { Label } from "@/components/ui/label"
import { useTranslation } from "react-i18next"

export type MsfsInputActionPanelProps = {
  config: MsfsInputAction | null
  onConfigChange: (config: MsfsInputAction) => void
}

const MsfsInputActionPanel = ({
  config,
  onConfigChange,
}: MsfsInputActionPanelProps) => {
  const { t } = useTranslation()
  const command = config?.Command ?? ""
  return (
    <div className="flex flex-col gap-4">
      <MsfsPresetPanel
        variant="input"
        selectedPresetId={config?.PresetId ?? null}
        setSelectedPreset={(preset) =>
          onConfigChange({
            ...(config as MsfsInputAction),
            PresetId: preset ? preset.id : null,
            Command: preset ? preset.code : null,
          } as MsfsInputAction)
        }
      />
      <div className="flex flex-col gap-2">
        <Label htmlFor="code">{t("Wizard.InputActions.Common.CodeLabel")}</Label>
        <Textarea
        id="code"
          value={
            command
              ? command
              : t("Wizard.InputActions.Msfs.NoneCode")
          }
        />
        <div className="text-sm text-muted-foreground">{t("Wizard.InputActions.Common.SupportedPlaceholders")}</div>
      </div>
    </div>
  )
}
export default MsfsInputActionPanel
