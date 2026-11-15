import { Label } from "@/components/ui/label"
import WizardSection from "@/components/wizard/common/WizardSection"
import SectionHeadline from "@/components/wizard/common/SectionHeadline"
import { useTranslation } from "react-i18next"

export type RetriggerPanelProps = {
  variant: "summary" | "details"
}

const RetriggerPanel = ({ variant }: RetriggerPanelProps) => {
  const { t } = useTranslation("wizard")

  if (variant === "summary") {
    return (
      <div className="flex grow flex-row items-center gap-8">
        <div className="flex grow flex-col gap-1 text-sm">
          <Label htmlFor="preset">
            {t("ConfigWizard.Input.InputActions.Retrigger.NoteLabel")}:
          </Label>
          {t("ConfigWizard.Input.InputActions.Retrigger.Summary")}
        </div>
      </div>
    )
  }

  return (
    <WizardSection>
      <SectionHeadline
        title={t("ConfigWizard.Input.InputActions.Retrigger.Title")}
        description={t(
          "ConfigWizard.Input.InputActions.Retrigger.Description1",
        )}
        description2={t(
          "ConfigWizard.Input.InputActions.Retrigger.Description2",
        )}
      />
    </WizardSection>
  )
}
export default RetriggerPanel
