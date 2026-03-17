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
  onOpenChange: (open: boolean) => void
  saveChanges: () => void
  discardChanges: () => void
}

const ConfirmationDialog = ({
  open,
  onOpenChange,
  saveChanges,
  discardChanges,
}: ConfirmationDialogProps) => {
  const { t } = useTranslation()

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader className="sr-only">
          <DialogTitle>{t("Project.UnsavedChanges.Title")}</DialogTitle>
        </DialogHeader>
        <div>{t("Project.UnsavedChanges.Description")}</div>
        <div className="flex flex-row justify-end gap-4">
          <Button variant="ghost" onClick={discardChanges}>
            {t("Project.UnsavedChanges.Discard")}
          </Button>
          <Button onClick={saveChanges}>
            {t("Project.UnsavedChanges.Save")}
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  )
}
export default ConfirmationDialog
