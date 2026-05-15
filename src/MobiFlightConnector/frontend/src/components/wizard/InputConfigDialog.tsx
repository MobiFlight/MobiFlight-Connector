import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog"
import ConfigWizard from "@/components/wizard/ConfigWizard"
import { useProjectStore } from "@/stores/projectStore"
import { useTranslation } from "react-i18next"


const InputConfigDialog = ({ configId }: { configId: string }) => {
  const { t } = useTranslation()
  const { project, activeConfigFileIndex } = useProjectStore()
  const configFile = project?.ConfigFiles[activeConfigFileIndex]
  const configItem = configFile?.ConfigItems?.find((item) => item.GUID === configId)

  return (
    <Dialog open={true} onOpenChange={() => {}}>
      <DialogContent className="vsm:min-h-[75%] vxl:min-h-[60%] flex min-h-[90%] flex-col overflow-y-auto select-none sm:max-w-150 lg:max-w-200 xl:max-w-250">
        <DialogHeader>
          <DialogTitle className="text-2xl">
            {t("Dialog.ConfigWizard.Title")}
          </DialogTitle>
          <DialogDescription className="text-md vsm:block hidden">
            {t("Dialog.ConfigWizard.Description")}
          </DialogDescription>
        </DialogHeader>
          { configItem && <ConfigWizard configItem={configItem} /> }
      </DialogContent>
    </Dialog>
  )
}
export default InputConfigDialog
