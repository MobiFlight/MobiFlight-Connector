import { Button } from "@/components/ui/button"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog"
import { ScrollArea } from "@/components/ui/scroll-area"
import ConfigWizard from "@/components/wizard/ConfigWizard"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { useProjectStore } from "@/stores/projectStore"
import { useRef, useState } from "react"
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

  const saveChanges = () => {
    const { publish } = publishOnMessageExchange()
    publish({
      key: "CommandUpdateConfigItem",
      payload: {
        item: currentConfigItem,
      },
    })
    closeDialog()
  }

  const containerRef = useRef<HTMLDivElement>(null)

  const [currentConfigItem, setCurrentConfigItem] = useState(configItem)

  return (
    <Dialog open={true} onOpenChange={closeDialog}>
      <DialogContent
        onKeyDown={(e) => {
          e.stopPropagation()
        }}
        ref={containerRef}
        className="vlg:min-h-[80%] vxl:min-h-[75%] flex min-h-full flex-col justify-between overflow-x-hidden overflow-y-auto select-none sm:max-w-full lg:max-w-200 xl:max-w-250"
      >
        <DialogHeader>
          <DialogTitle className="text-2xl">
            {t("Dialog.InputConfigWizard.Title")}
          </DialogTitle>
          <DialogDescription className="text-md vsm:block hidden">
            {t("Dialog.InputConfigWizard.Description")}
          </DialogDescription>
        </DialogHeader>
        <div className="relative flex grow flex-col gap-2">
          <ScrollArea className="grow">
            <div className="pr-3">
              {currentConfigItem && (
                <ConfigWizard
                  configItem={currentConfigItem}
                  onConfigChange={(item) => {
                    setCurrentConfigItem(item)
                  }}
                  drawerContainer={containerRef}
                />
              )}
            </div>
          </ScrollArea>
        </div>
        <DialogFooter>
          <Button variant="outline" onClick={closeDialog}>
            {t("Dialog.General.Cancel")}
          </Button>
          <Button onClick={saveChanges}>{t("Dialog.General.Save")}</Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
export default InputConfigDialog
