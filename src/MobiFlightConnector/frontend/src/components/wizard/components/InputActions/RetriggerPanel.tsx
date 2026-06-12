import { useTranslation } from "react-i18next"

const RetriggerPanel = () => {
  const { t } = useTranslation()
  return (
    <div className="flex flex-col gap-4">
      <div className="flex flex-col">
        <div className="text-lg font-semibold">{t("Dialog.InputConfigWizard.InputActions.Retrigger.Title")}</div>
        <div className="text-muted-foreground text-sm">{t("Dialog.InputConfigWizard.InputActions.Retrigger.Description1")}</div>
        <div className="text-muted-foreground text-sm">{t("Dialog.InputConfigWizard.InputActions.Retrigger.Description2")}</div>
      </div>
    </div>
  )
}
export default RetriggerPanel
