import ComboBox from "@/components/ComboBox"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import XplanePresetPanel from "@/components/wizard/components/InputActions/XplanePresetPanel"
import { XplaneInputAction } from "@/types/config"
import { useTranslation } from "react-i18next"

const CODE_TYPE_OPTIONS: ("DataRef" | "Command")[] = ["DataRef", "Command"]

export type XplaneInputActionPanelProps = {
  config: XplaneInputAction | null
  onConfigChange: (config: XplaneInputAction) => void
}

const XplaneInputActionPanel = ({
  config,
  onConfigChange,
}: XplaneInputActionPanelProps) => {
  const { t } = useTranslation()
  return (
    <div className="flex flex-col gap-4">
      <XplanePresetPanel
        variant="input"
        selectedPath={config?.Path ?? null}
        onPresetSelect={(preset) =>
          onConfigChange({
            ...(config as XplaneInputAction),
            Path: preset.code,
            InputType: preset.codeType,
          })
        }
      />
      <div className="flex flex-col gap-2">
        <Label>{t("Dialog.InputConfigWizard.InputActions.Xplane.InputTypeLabel")}</Label>
        <ComboBox
          items={CODE_TYPE_OPTIONS}
          selected={(config?.InputType as "DataRef" | "Command") ?? undefined}
          placeholder={t("Dialog.InputConfigWizard.InputActions.Xplane.SelectInputTypePlaceholder")}
          getLabel={(item) => item}
          getValue={(item) => item}
          isSelected={(item) => item === config?.InputType}
          setSelected={(item) => {
            if (!item) return
            onConfigChange({ ...(config as XplaneInputAction), InputType: item })
          }}
          variant="nofilter"
          widthClass="w-48"
        />
      </div>
      <div className="flex flex-col gap-2">
        <Label htmlFor="path">{t("Dialog.InputConfigWizard.InputActions.Xplane.PathLabel")}</Label>
        <Input
          id="path"
          value={config?.Path ?? ""}
          onChange={(e) =>
            onConfigChange({
              ...(config as XplaneInputAction),
              Path: e.target.value,
            })
          }
          placeholder={t("Dialog.InputConfigWizard.InputActions.Xplane.PathPlaceholder")}
        />
        <div className="text-sm text-muted-foreground">
          {t("Dialog.InputConfigWizard.InputActions.Xplane.PathDescription")}
        </div>
      </div>
      {config?.InputType === "DataRef" && <div className="flex flex-col gap-2">
        <Label htmlFor="value">{t("Dialog.InputConfigWizard.InputActions.Xplane.ValueLabel")}</Label>
        <Input
          id="value"
          value={config?.Expression ?? ""}
          onChange={(e) =>
            onConfigChange({
              ...(config as XplaneInputAction),
              Expression: e.target.value,
            })
          }
          placeholder={t("Dialog.InputConfigWizard.InputActions.Xplane.ValuePlaceholder")}
        />
        <div className="text-sm text-muted-foreground">
          {t("Dialog.InputConfigWizard.InputActions.Common.SupportedPlaceholders")}
        </div>
      </div>}
    </div>
  )
}

export default XplaneInputActionPanel
