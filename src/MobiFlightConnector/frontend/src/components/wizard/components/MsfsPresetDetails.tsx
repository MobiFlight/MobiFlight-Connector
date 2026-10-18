import IconBrandHubHopLogo from "@/components/icons/IconBrandHubHopLogo"
import { useTranslation } from "react-i18next"
import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import { Textarea } from "@/components/ui/textarea"
import { IconExclamationCircle } from "@tabler/icons-react"
import { Preset } from "@/types/preset"
import { Label } from "@/components/ui/label"
import useOpenUrl from "@/lib/hooks/useOpenUrl"

export interface MsfsPresetDetailsProps {
  preset: Preset | null
  command: string
  onCommandChange: (command: string) => void
}

const MsfsPresetDetails = ({
  command,
  preset,
  onCommandChange,
}: MsfsPresetDetailsProps) => {
  const { t } = useTranslation()
  const openUrl = useOpenUrl()

  const labels = {
    vendor: t("Dialog.InputConfigWizard.InputActions.Common.Preset.Vendor"),
    aircraft: t("Dialog.InputConfigWizard.InputActions.Common.Preset.Aircraft"),
    system: t("Dialog.InputConfigWizard.InputActions.Common.Preset.System"),
    author: t("Dialog.InputConfigWizard.InputActions.Common.Preset.Author"),
    date: t("Dialog.InputConfigWizard.InputActions.Common.Preset.Date"),
  }

  const openHubHopDetails = () => {
    const hubHopUrl = `https://hubhop.mobiflight.com/preset/?simType=msfs2020&id=${preset?.id}`
    openUrl(hubHopUrl)
  }

  return (
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
                {preset?.author && preset?.createdDate && (
                  <div className="flex flex-1 flex-row items-center gap-1">
                    <div className="flex flex-1 flex-col gap-1">
                      <Label htmlFor="code">
                        {labels.author} / {labels.date}
                      </Label>
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
            (command ?? "") !== "" && (
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

          <div className="flex flex-col gap-1">
            <Label htmlFor="code">
              {t("Dialog.InputConfigWizard.InputActions.Common.CodeLabel")}
            </Label>
            <Textarea
              name="code"
              className="dark:focus:bg-input/20 font-mono text-sm whitespace-nowrap dark:bg-transparent"
              placeholder={t(
                "Dialog.InputConfigWizard.InputActions.Common.Preset.Code.Placeholder",
              )}
              value={command ?? ""}
              onChange={(e) => {
                onCommandChange(e.target.value)
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
  )
}
export default MsfsPresetDetails
