import { Label } from "@/components/ui/label"
import { useTranslation } from "react-i18next"

export type RetriggerPanelProps = {
  variant: "summary" | "details"
}

const RetriggerPanel = ({ variant }: RetriggerPanelProps) => {
  const { t } = useTranslation()

  if (variant === "summary") {
    return (
      <div className="flex grow flex-row items-center gap-8">
        <div className="flex grow flex-col gap-1">
          <Label htmlFor="preset">{t("Dialog.InputConfigWizard.InputActions.Retrigger.NoteLabel")}:</Label>
          {t("Dialog.InputConfigWizard.InputActions.Retrigger.Summary")}
        </div>
      </div>
    )
  }

  return (
    <div className="flex flex-col gap-4">
      <div className="flex flex-col">
        <div className="text-lg font-semibold">
          {t("Dialog.InputConfigWizard.InputActions.Retrigger.Title")}
        </div>
        <div className="text-muted-foreground text-sm">
          {t("Dialog.InputConfigWizard.InputActions.Retrigger.Description1")}
        </div>
        <div className="text-muted-foreground text-sm">
          {t("Dialog.InputConfigWizard.InputActions.Retrigger.Description2")}
        </div>
      </div>
    </div>
  )
}
export default RetriggerPanel
