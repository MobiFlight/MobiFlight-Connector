import ComboBox from "@/components/ComboBox"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import CodeValueLabel from "@/components/wizard/components/CodeValueLabel"
import XplanePresetPanel, { XplanePreset } from "@/components/wizard/components/InputActions/XplanePresetPanel"
import { fetchHubHopPresets } from "@/lib/configWizard"
import { XplaneInputAction } from "@/types/config"
import { useQuery } from "@tanstack/react-query"
import { useTranslation } from "react-i18next"

const CODE_TYPE_OPTIONS: ("DataRef" | "Command")[] = ["DataRef", "Command"]

export type XplaneInputActionPanelProps = {
  variant: "summary" | "details"
  config: XplaneInputAction | null
  onConfigChange: (config: XplaneInputAction) => void
}

const XplaneInputActionPanel = ({
  variant,
  config,
  onConfigChange,
}: XplaneInputActionPanelProps) => {
  const { t } = useTranslation()

  const { data: presets = [] } = useQuery({
    queryKey: ["xplane-presets"],
    queryFn: () => fetchHubHopPresets("xplane") as Promise<XplanePreset[]>,
    staleTime: Infinity,
  })

  const presetLabel = presets.find((p) => p.code === config?.Path)?.label ?? null
  
  if (variant === "summary") {
    return (
      <div className="flex grow flex-row items-center gap-8">
        <div className="flex w-1/3 flex-col gap-1">
          <Label htmlFor="preset">
            {t("Dialog.InputConfigWizard.InputActions.Common.PresetLabel")}:
          </Label>
          <div className="text-sm">{presetLabel ?? t("Dialog.InputConfigWizard.InputActions.Xplane.CustomPreset")}</div>
        </div>
        <div className="flex grow flex-col gap-1">
          <Label htmlFor="code">
            {t("Dialog.InputConfigWizard.InputActions.Common.CodeLabel")}
          </Label>
          <CodeValueLabel id="code">
            {config?.Path ??
              t("Dialog.InputConfigWizard.InputActions.Xplane.NonePath")}
          </CodeValueLabel>
        </div>
      </div>
    )
  }
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
        <Label>
          {t("Dialog.InputConfigWizard.InputActions.Xplane.InputTypeLabel")}
        </Label>
        <ComboBox
          items={CODE_TYPE_OPTIONS}
          selected={(config?.InputType as "DataRef" | "Command") ?? undefined}
          placeholder={t(
            "Dialog.InputConfigWizard.InputActions.Xplane.SelectInputTypePlaceholder",
          )}
          getLabel={(item) => item}
          getValue={(item) => item}
          isSelected={(item) => item === config?.InputType}
          setSelected={(item) => {
            if (!item) return
            onConfigChange({
              ...(config as XplaneInputAction),
              InputType: item,
            })
          }}
          variant="nofilter"
          widthClass="w-48"
        />
      </div>
      <div className="flex flex-col gap-2">
        <Label htmlFor="path">
          {t("Dialog.InputConfigWizard.InputActions.Xplane.PathLabel")}
        </Label>
        <Input
          id="path"
          className="font-mono text-sm whitespace-nowrap"
          value={config?.Path ?? ""}
          onChange={(e) =>
            onConfigChange({
              ...(config as XplaneInputAction),
              Path: e.target.value,
            })
          }
          placeholder={t(
            "Dialog.InputConfigWizard.InputActions.Xplane.PathPlaceholder",
          )}
        />
        <div className="text-muted-foreground text-sm">
          {t("Dialog.InputConfigWizard.InputActions.Xplane.PathDescription")}
        </div>
      </div>
      {config?.InputType === "DataRef" && (
        <div className="flex flex-col gap-2">
          <Label htmlFor="value">
            {t("Dialog.InputConfigWizard.InputActions.Xplane.ValueLabel")}
          </Label>
          <Input
            className="font-mono text-sm whitespace-nowrap"
            id="value"
            value={config?.Expression ?? ""}
            onChange={(e) =>
              onConfigChange({
                ...(config as XplaneInputAction),
                Expression: e.target.value,
              })
            }
            placeholder={t(
              "Dialog.InputConfigWizard.InputActions.Xplane.ValuePlaceholder",
            )}
          />
          <div className="text-muted-foreground text-sm">
            {t(
              "Dialog.InputConfigWizard.InputActions.Common.SupportedPlaceholders",
            )}
          </div>
        </div>
      )}
    </div>
  )
}

export default XplaneInputActionPanel
