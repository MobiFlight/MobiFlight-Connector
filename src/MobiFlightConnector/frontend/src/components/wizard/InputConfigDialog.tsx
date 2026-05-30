import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog"
import ConfigWizard from "@/components/wizard/ConfigWizard"
import { useProjectStore } from "@/stores/projectStore"
import { useRef } from "react"
import { useTranslation } from "react-i18next"
import { useNavigate } from "react-router"

export type InputConfigDialogProps = {
  configId: string
}

const InputConfigDialog = ({ configId }: InputConfigDialogProps) => {
  const { t } = useTranslation()
  const navigate = useNavigate()
  const { project, activeConfigFileIndex } = useProjectStore()
  const configFile = project?.ConfigFiles[activeConfigFileIndex]
  const configItem = configFile?.ConfigItems?.find(
    (item) => item.GUID === configId,
  )

  const closeDialog = () => {
    navigate(-1)
  }

  const containerRef = useRef<HTMLDivElement>(null)

  return (
    <Dialog open={true} onOpenChange={closeDialog}>
      <DialogContent ref={containerRef} className="vsm:min-h-[75%] vxl:min-h-[60%] flex min-h-[90%] flex-col overflow-y-auto select-none sm:max-w-150 lg:max-w-200 xl:max-w-250">
        <DialogHeader>
          <DialogTitle className="text-2xl">
            {t("Dialog.ConfigWizard.Title")}
          </DialogTitle>
          <DialogDescription className="text-md vsm:block hidden">
            {t("Dialog.ConfigWizard.Description")}
          </DialogDescription>
        </DialogHeader>
        {configItem && (
          <ConfigWizard configItem={configItem} onClose={closeDialog} drawerContainer={containerRef} />
        )}
      </DialogContent>
    </Dialog>
  )
}
export default InputConfigDialog
