import { Textarea } from "@/components/ui/textarea"
import MsfsPresetPanel from "@/components/wizard/components/InputActions/MsfsPresetPanel"
import { MsfsInputAction } from "@/types/config"
import { Label } from "@/components/ui/label"
import { useTranslation } from "react-i18next"
import { Separator } from "@/components/ui/separator"
import { useState } from "react"

export type MsfsInputActionPanelProps = {
  config: MsfsInputAction | null
  onConfigChange: (config: MsfsInputAction) => void
}

const MsfsInputActionPanel = ({
  config,
  onConfigChange,
}: MsfsInputActionPanelProps) => {
  const { t } = useTranslation()
  const [ currentCommand, setCurrentCommand ] = useState(config?.Command ?? "")
  const [ prevCommand, setPrevCommand ] = useState(config?.Command ?? "")

  if (config?.Command !== prevCommand) {
    setPrevCommand(config?.Command ?? "")
    setCurrentCommand(config?.Command ?? "")
  }

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
      <Separator />
      <div className="flex flex-col gap-2">
        <Label htmlFor="code">{t("Dialog.InputConfigWizard.InputActions.Common.CodeLabel")}</Label>
        <Textarea
          id="code"
          placeholder={t("Dialog.InputConfigWizard.InputActions.Msfs.NoneCode")}
          value={currentCommand}
          onChange={(e) => {
            setCurrentCommand(e.target.value)
          }}

          onBlur={() => {
            onConfigChange({
              ...(config as MsfsInputAction), 
              Command: currentCommand,
              PresetId: "", // Clear preset if user manually edits command
            } as MsfsInputAction)
          }}
        />
        <div className="text-sm text-muted-foreground">{t("Dialog.InputConfigWizard.InputActions.Common.SupportedPlaceholders")}</div>
      </div>
    </div>
  )
}
export default MsfsInputActionPanel
