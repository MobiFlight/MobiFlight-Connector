import { Textarea } from "@/components/ui/textarea"
import MsfsPresetPanel from "@/components/wizard/components/InputActions/MsfsPresetPanel"
import { MsfsInputAction } from "@/types/config"
import { Label } from "@/components/ui/label"
import { useTranslation } from "react-i18next"
import { Separator } from "@/components/ui/separator"

export type MsfsInputActionPanelProps = {
  variant: "summary" | "details"
  config: MsfsInputAction | null
  onConfigChange: (config: MsfsInputAction) => void
}

const MsfsInputActionPanel = ({
  variant,
  config,
  onConfigChange,
}: MsfsInputActionPanelProps) => {
  const { t } = useTranslation()

  if (variant === "summary") {
    return (
      <div className="flex grow flex-row items-center gap-8">
        <div className="flex flex-col gap-1 w-1/3">
          <Label htmlFor="preset">
            {t("Dialog.InputConfigWizard.InputActions.Common.PresetLabel")}:
          </Label>
          <div>
            AP Panel Heading Hold
          </div>
        </div>
        <div className="flex grow flex-col gap-1">
          <Label htmlFor="code">
            {t("Dialog.InputConfigWizard.InputActions.Common.CodeLabel")}
          </Label>
          <div
            id="code"
            className="bg-accent rounded px-2 py-1 font-mono text-sm whitespace-pre-wrap"
          >
            {config?.Command ??
              t("Dialog.InputConfigWizard.InputActions.Msfs.NoneCode")}
          </div>
        </div>
      </div>
    )
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
        <Label htmlFor="code">
          {t("Dialog.InputConfigWizard.InputActions.Common.CodeLabel")}
        </Label>
        <Textarea
          name="code"
          className="font-mono text-sm whitespace-nowrap"
          placeholder={t("Dialog.InputConfigWizard.InputActions.Msfs.CodePlaceholder")}
          value={config?.Command ?? ""}
          onChange={(e) => {
            onConfigChange({
              ...(config as MsfsInputAction),
              Command: e.target.value,
              PresetId: "", // Clear preset if user manually edits command
            } as MsfsInputAction)
          }}
        />
        <div className="text-muted-foreground text-sm">
          {t(
            "Dialog.InputConfigWizard.InputActions.Common.SupportedPlaceholders",
          )}
        </div>
      </div>
    </div>
  )
}
export default MsfsInputActionPanel
