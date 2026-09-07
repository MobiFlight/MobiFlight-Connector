import { Button } from "@/components/ui/button"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog"
import { useTranslation } from "react-i18next"

export type ConfirmationDialogProps = {
  open: boolean
  onOpenChange: (open: boolean) => void
  saveChanges: () => void
  discardChanges: () => void
  cancel: () => void
}

const ConfirmationDialog = ({
  open,
  onOpenChange,
  saveChanges,
  discardChanges,
  cancel,
}: ConfirmationDialogProps) => {
  const { t } = useTranslation()

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader className="">
          <DialogTitle>{t("Project.UnsavedChanges.Title")}</DialogTitle>
          <DialogDescription>
            {t("Project.UnsavedChanges.Description")}
          </DialogDescription>
        </DialogHeader>
        <div className="flex flex-row justify-end gap-4">
          <Button variant="ghost" onClick={discardChanges}>
            {t("Project.UnsavedChanges.Discard")}
          </Button>
          <Button variant="outline" onClick={cancel}>
            {t("Project.UnsavedChanges.Cancel")}
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
