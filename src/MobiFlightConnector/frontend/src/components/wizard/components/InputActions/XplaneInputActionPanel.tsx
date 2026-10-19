import ComboBox from "@/components/ComboBox"
import { Card, CardContent } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import CodeValueLabel from "@/components/wizard/components/CodeValueLabel"
import { HubHopPresetDetails } from "@/components/wizard/components/HubHopPresetDetails"
import XplanePresetPanel from "@/components/wizard/components/InputActions/XplanePresetPanel"
import { fetchHubHopPresets } from "@/lib/configWizard"
import { XplaneInputAction } from "@/types/config"
import { XplanePreset } from "@/types/preset"
import { IconExclamationCircle } from "@tabler/icons-react"
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

  const { data: presets = [], isLoading } = useQuery({
    queryKey: ["xplane-presets"],
    queryFn: () => fetchHubHopPresets("xplane") as Promise<XplanePreset[]>,
    // presets don't change at runtime; HubHopState drives invalidation
    staleTime: Infinity,
  })
  const preset = presets.find((p) => p.code === config?.Path) ?? null

  if (variant === "summary") {
    return (
      <div className="flex grow flex-row items-center gap-2">
        <div className="flex w-1/3 flex-col gap-1">
          <Label htmlFor="preset">
            {t("Dialog.InputConfigWizard.InputActions.Common.Preset.Label")}:
          </Label>
          <div className="text-sm">
            {preset?.label ??
              t("Dialog.InputConfigWizard.InputActions.Common.Preset.Custom")}
          </div>
        </div>
        <div className="flex grow flex-col gap-1">
          <Label htmlFor="code">
            {t("Dialog.InputConfigWizard.InputActions.Common.CodeLabel")}
          </Label>
          <CodeValueLabel id="code" className="max-w-100">
            {config?.Path ??
              t(
                "Dialog.InputConfigWizard.InputActions.Common.Preset.Code.None",
              )}
          </CodeValueLabel>
        </div>
      </div>
    )
  }

  return (
    <div className="flex flex-col gap-4">
      <XplanePresetPanel
        variant="input"
        isLoading={isLoading}
        selectedPath={config?.Path ?? null}
        setSelectedPreset={(preset) =>
          onConfigChange({
            ...(config as XplaneInputAction),
            Path: preset ? preset.code : null,
            InputType: preset ? (preset as XplanePreset).codeType : null,
          })
        }
      />
      <Card>
        <CardContent
          className="flex flex-col gap-4 pt-4"
          data-testid="preset-code-panel"
        >
          <div className="flex flex-col">
            <div className="text-lg font-semibold">
              {t(
                "Dialog.InputConfigWizard.InputActions.Common.Preset.Code.Title",
              )}
            </div>
            <div className="text-muted-foreground text-sm">
              {t(
                "Dialog.InputConfigWizard.InputActions.Common.Preset.Code.Description",
              )}
            </div>
          </div>
          <div className="flex flex-col gap-4">
            {preset ? (
              <HubHopPresetDetails preset={preset} variant="xplane" />
            ) : (
              (config?.Path ?? "") !== "" && (
                <div className="flex flex-row items-center gap-2 rounded-md">
                  <IconExclamationCircle className="text-primary fill-background" />
                  <div className="text-primary text-sm">
                    {t(
                      "Dialog.InputConfigWizard.InputActions.Common.Preset.Code.Customized",
                    )}
                  </div>
                </div>
              )
            )}
            <div className="flex flex-row gap-2">
              <div className="flex flex-col gap-1">
                <Label>
                  {t(
                    "Dialog.InputConfigWizard.InputActions.Xplane.InputTypeLabel",
                  )}
                </Label>
                <ComboBox
                  items={CODE_TYPE_OPTIONS}
                  selected={
                    (config?.InputType as "DataRef" | "Command") ?? undefined
                  }
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
                  widthClass="w-32"
                />
              </div>
              <div className="flex flex-col gap-1">
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
                  {t(
                    "Dialog.InputConfigWizard.InputActions.Xplane.PathDescription",
                  )}
                </div>
              </div>
            </div>
            {config?.InputType === "DataRef" && (
              <div className="flex flex-col gap-1">
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
        </CardContent>
      </Card>
    </div>
  )
}

export default XplaneInputActionPanel
