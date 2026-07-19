import ComboBox from "@/components/ComboBox"
import IconBrandHubHopLogo from "@/components/icons/IconBrandHubHopLogo"
import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import CodeValueLabel from "@/components/wizard/components/CodeValueLabel"
import XplanePresetPanel from "@/components/wizard/components/InputActions/XplanePresetPanel"
import { fetchHubHopPresets } from "@/lib/configWizard"
import useOpenUrl from "@/lib/hooks/useOpenUrl"
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
  const openUrl = useOpenUrl()

  const { data: presets = [] } = useQuery({
    queryKey: ["xplane-presets"],
    queryFn: () => fetchHubHopPresets("xplane") as Promise<XplanePreset[]>,
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
              t("Dialog.InputConfigWizard.InputActions.Common.Preset.Code.None")}
          </CodeValueLabel>
        </div>
      </div>
    )
  }

  const labels = {
    vendor: t("Dialog.InputConfigWizard.InputActions.Common.Preset.Vendor"),
    aircraft: t("Dialog.InputConfigWizard.InputActions.Common.Preset.Aircraft"),
    system: t("Dialog.InputConfigWizard.InputActions.Common.Preset.System"),
    author: t("Dialog.InputConfigWizard.InputActions.Common.Preset.Author"),
    date: t("Dialog.InputConfigWizard.InputActions.Common.Preset.Date"),
  }

  const openHubHopDetails = () => {
    const hubHopUrl = `https://hubhop.mobiflight.com/preset/?simType=xplane&id=${preset?.id}`
    openUrl(hubHopUrl)
  }

  return (
    <div className="flex flex-col gap-4">
      <XplanePresetPanel
        variant="input"
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
        <CardContent className="flex flex-col gap-4 pt-4" data-testid="preset-code-panel">
          <div className="flex flex-col">
            <div className="text-lg font-semibold">
              {t("Dialog.InputConfigWizard.InputActions.Common.Preset.Code.Title")}
            </div>
            <div className="text-muted-foreground text-sm">
              {t(
                "Dialog.InputConfigWizard.InputActions.Common.Preset.Code.Description",
              )}
            </div>
          </div>
          <div className="flex flex-col gap-4">
            {preset ? (
              <>
                <div className="flex flex-row gap-4">
                  <div className="flex flex-1 flex-col gap-1">
                    <Label htmlFor="code">
                      {t(
                        "Dialog.InputConfigWizard.InputActions.Common.NameLabel",
                      )}
                    </Label>
                    <div className="text-sm font-semibold">{preset?.label}</div>
                  </div>

                  <div className="flex flex-1 flex-col gap-1">
                    <Label htmlFor="code">
                      {labels.vendor} / {labels.aircraft} / {labels.system}
                    </Label>
                    <div className="text-sm">
                      {preset?.vendor} / {preset?.aircraft} / {preset?.system}
                    </div>
                  </div>
                </div>
                <div className="flex flex-row gap-4">
                  <div className="flex flex-1 flex-col gap-1">
                    <Label htmlFor="code">
                      {t(
                        "Dialog.InputConfigWizard.InputActions.Common.DescriptionLabel",
                      )}
                    </Label>
                    <div className="text-sm">{preset?.description ?? "-"}</div>
                  </div>
                  {preset?.author && (
                    <div className="flex flex-1 flex-row items-center gap-1">
                      <div className="flex flex-1 flex-col gap-1">
                        <Label htmlFor="code">Author / Date</Label>
                        <div className="text-sm">
                          {preset?.author}
                          {preset?.createdDate && (
                            <>
                              {" "}
                              /{" "}
                              {new Date(
                                preset.createdDate,
                              ).toLocaleDateString()}{" "}
                            </>
                          )}
                        </div>
                      </div>
                      <Button
                        className="h-8 gap-1 rounded-full px-4 py-1 [&_svg]:size-6"
                        variant={"ghost"}
                        onClick={openHubHopDetails}
                      >
                        <IconBrandHubHopLogo className="fill-orange-400 stroke-orange-400" />
                        HubHop
                      </Button>
                    </div>
                  )}
                </div>
              </>
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
