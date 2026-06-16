import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"
import { IconEdit } from "@tabler/icons-react"
import { useTranslation } from "react-i18next"

export type RetriggerPanelProps = {
  variant: "summary" | "details"
  onEditAction: () => void
}

const RetriggerPanel = ({ variant, onEditAction }: RetriggerPanelProps) => {
  const { t } = useTranslation()

  if (variant === "summary") {
    return (
      <div className="flex grow flex-row items-center gap-8">
        <div className="flex w-1/3 flex-col gap-1">
          <Label htmlFor="preset">Note:</Label>
          {t("Dialog.InputConfigWizard.InputActions.Retrigger.Summary")}
        </div>
        <Button size={"sm"} variant="ghost" onClick={() => onEditAction()}>
          <IconEdit />
        </Button>
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
