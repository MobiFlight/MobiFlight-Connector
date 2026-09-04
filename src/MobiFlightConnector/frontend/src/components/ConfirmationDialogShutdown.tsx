import { Button } from "@/components/ui/button"
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog"
import { useTranslation } from "react-i18next"

export type ConfirmationDialogProps = {
  open: boolean
  onOpenShutdown: (open: boolean) => void
  discardChanges: () => void
  cancel: () => void
}

const ConfirmationDialogShutdown = ({
  open,
  onOpenShutdown,
  discardChanges,
  cancel,
}: ConfirmationDialogProps) => {
  const { t } = useTranslation()

  return (
    <Dialog open={open} onOpenChange={onOpenShutdown}>
      <DialogContent>
        <DialogHeader className="sr-only">
          <DialogTitle>{t("Project.Shutdown.Title")}</DialogTitle>
        </DialogHeader>
        <div>{t("Project.Shutdown.Description")}</div>
        <div className="flex flex-row justify-end gap-4">
          <Button variant="ghost" onClick={discardChanges}>
            {t("Project.Shutdown.Discard")}
          </Button>
          <Button onClick={cancel}>{t("Project.Shutdown.Cancel")}</Button>
        </div>
      </DialogContent>
    </Dialog>
  )
}
export default ConfirmationDialogShutdown
