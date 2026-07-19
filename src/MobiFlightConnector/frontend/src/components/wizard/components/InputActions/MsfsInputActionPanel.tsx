import { Textarea } from "@/components/ui/textarea"
import MsfsPresetPanel from "@/components/wizard/components/InputActions/MsfsPresetPanel"
import { MsfsInputAction } from "@/types/config"
import { Label } from "@/components/ui/label"
import { useTranslation } from "react-i18next"
import CodeValueLabel from "@/components/wizard/components/CodeValueLabel"
import { fetchHubHopPresets } from "@/lib/configWizard"
import { useQuery } from "@tanstack/react-query"
import { Card, CardContent } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import IconBrandHubHopLogo from "@/components/icons/IconBrandHubHopLogo"
import { IconExclamationCircle } from "@tabler/icons-react"

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
  const { data: presets = [] /*, isLoading */ } = useQuery({
    queryKey: ["msfs-presets"],
    queryFn: () => fetchHubHopPresets("msfs"),
    // presets don't change at runtime; HubHopState drives invalidation
    staleTime: Infinity,
  })
  const preset = presets.find((p) => p.id === config?.PresetId) ?? null

  if (variant === "summary") {
    return (
      <div className="flex grow flex-row items-center gap-2">
        <div className="flex w-1/3 flex-col gap-1">
          <Label htmlFor="preset">
            {t("Dialog.InputConfigWizard.InputActions.Common.PresetLabel")}:
          </Label>
          <div className="text-sm">
            {preset?.label ??
              t("Dialog.InputConfigWizard.InputActions.Msfs.CustomPreset")}
          </div>
        </div>
        <div className="flex grow flex-col gap-1">
          <Label htmlFor="code">
            {t("Dialog.InputConfigWizard.InputActions.Common.CodeLabel")}
          </Label>
          <CodeValueLabel id="code" className="max-w-50 lg:max-w-100">
            {config?.Command ??
              t("Dialog.InputConfigWizard.InputActions.Msfs.Code.None")}
          </CodeValueLabel>
        </div>
      </div>
    )
  }

  const labels = {
    vendor: t("Dialog.InputConfigWizard.InputActions.Msfs.Preset.Vendor"),
    aircraft: t("Dialog.InputConfigWizard.InputActions.Msfs.Preset.Aircraft"),
    system: t("Dialog.InputConfigWizard.InputActions.Msfs.Preset.System"),
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
      <Card>
        <CardContent className="flex flex-col gap-4 pt-4">
          <div className="flex flex-col">
            <div className="text-lg font-semibold">
              {t("Dialog.InputConfigWizard.InputActions.Msfs.Code.Title")}
            </div>
            <div className="text-muted-foreground text-sm">
              {t("Dialog.InputConfigWizard.InputActions.Msfs.Code.Description")}
            </div>
          </div>
          <div className="flex flex-col gap-4">
            {preset ? (
              <>
                <div className="flex flex-row gap-4">
                  <div className="flex flex-1 flex-col gap-1">
                    <Label htmlFor="code">
                      {t("Dialog.InputConfigWizard.InputActions.Common.NameLabel")}
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
                  {preset?.author && preset?.createdDate && (
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
                      >
                        <IconBrandHubHopLogo className="fill-orange-400 stroke-orange-400" />
                        HubHop
                      </Button>
                    </div>
                  )}
                </div>
              </>
            ) : (
              config?.Command !== "" && (
                <div className="flex flex-row items-center gap-2 rounded-md">
                  <IconExclamationCircle className="text-primary fill-background" />
                  <div className="text-primary text-sm">
                    {t(
                      "Dialog.InputConfigWizard.InputActions.Msfs.Code.Customized",
                    )}
                  </div>
                </div>
              )
            )}

            <div className="flex flex-col gap-1">
              <Label htmlFor="code">
                {t("Dialog.InputConfigWizard.InputActions.Common.CodeLabel")}
              </Label>
              <Textarea
                name="code"
                className="dark:focus:bg-input/20 font-mono text-sm whitespace-nowrap dark:bg-transparent"
                placeholder={t(
                  "Dialog.InputConfigWizard.InputActions.Msfs.Code.Placeholder",
                )}
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
        </CardContent>
      </Card>
    </div>
  )
}
export default MsfsInputActionPanel
