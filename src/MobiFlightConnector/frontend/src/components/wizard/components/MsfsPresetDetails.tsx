import { useTranslation } from "react-i18next"
import { Card, CardContent } from "@/components/ui/card"
import { Textarea } from "@/components/ui/textarea"
import { IconExclamationCircle } from "@tabler/icons-react"
import { Preset } from "@/types/preset"
import { Label } from "@/components/ui/label"
import { HubHopPresetDetails } from "@/components/wizard/components/HubHopPresetDetails"

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
            <HubHopPresetDetails preset={preset} variant={"msfs"} />
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
