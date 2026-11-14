import { Preset } from "@/types/preset"
import { useTranslation } from "react-i18next"
import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"
import IconBrandHubHopLogo from "@/components/icons/IconBrandHubHopLogo"
import useOpenUrl from "@/lib/hooks/useOpenUrl"

export const HubHopPresetDetails = ({
  preset,
  variant,
}: {
  preset: Preset | null
  variant: "msfs" | "xplane"
}) => {
  const { t } = useTranslation()
  const openUrl = useOpenUrl()
  const simType = variant === "msfs" ? "msfs2020" : "xplane"

  const openHubHopDetails = () => {
    const hubHopUrl = `https://hubhop.mobiflight.com/preset/?simType=${simType}&id=${preset?.id}`
    openUrl(hubHopUrl)
  }

  const labels = {
    vendor: t("Dialog.InputConfigWizard.InputActions.Common.Preset.Vendor"),
    aircraft: t("Dialog.InputConfigWizard.InputActions.Common.Preset.Aircraft"),
    system: t("Dialog.InputConfigWizard.InputActions.Common.Preset.System"),
    author: t("Dialog.InputConfigWizard.InputActions.Common.Preset.Author"),
    date: t("Dialog.InputConfigWizard.InputActions.Common.Preset.Date"),
  }
  return (
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
            {t("Dialog.InputConfigWizard.InputActions.Common.DescriptionLabel")}
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
                  <> / {new Date(preset.createdDate).toLocaleDateString()} </>
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
  )
}