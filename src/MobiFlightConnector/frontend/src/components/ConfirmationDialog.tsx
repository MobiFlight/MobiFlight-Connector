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
  discardChanges: () => void
  cancel: () => void
}

const ConfirmationDialog = ({
  open,
  onOpenChange,
  discardChanges,
  cancel,
}: ConfirmationDialogProps) => {
  const { t } = useTranslation()

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader className="">
          <DialogTitle>{t("Dialog.UnsavedChanges.Title")}</DialogTitle>
          <DialogDescription>
            {t("Dialog.UnsavedChanges.Description")}
          </DialogDescription>
        </DialogHeader>
        <div className="flex flex-row justify-end gap-4">
          <Button variant="outline" onClick={cancel}>
            {t("Dialog.UnsavedChanges.Cancel")}
          </Button>
          <Button variant="destructive" onClick={discardChanges}>
            {t("Dialog.General.DiscardChanges")}
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  )
}
export default ConfirmationDialog
