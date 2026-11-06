import { useTranslation } from "react-i18next"
import { Textarea } from "@/components/ui/textarea"
import { IconExclamationCircle } from "@tabler/icons-react"
import { Preset } from "@/types/preset"
import { Label } from "@/components/ui/label"
import { HubHopPresetDetails } from "@/components/wizard/components/HubHopPresetDetails"
import SectionHeadline from "@/components/wizard/common/SectionHeadline"
import WizardSection from "@/components/wizard/common/WizardSection"

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
    <WizardSection data-testid="preset-code-panel">
      <SectionHeadline
        title={t(
          "Dialog.InputConfigWizard.InputActions.Common.Preset.Code.Title",
        )}
        description={t(
          "Dialog.InputConfigWizard.InputActions.Common.Preset.Code.Description",
        )}
      />
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
    </WizardSection>
  )
}
export default MsfsPresetDetails
