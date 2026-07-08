import { InlineEditLabel, InlineEditLabelRef } from "@/components/InlineEditLabel"
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
import { IconChevronRight } from "@tabler/icons-react"
import { useEffect, useRef, useState } from "react"
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
  const inlineEditRef = useRef<InlineEditLabelRef>(null)
  
  const [currentConfigItem, setCurrentConfigItem] = useState(configItem)
  
  // Automatically start editing the config item name if it is the default name
  const configItemName = configItem?.Name
  // this is a simple test to check if the config item is a newly created default config item
  const defaultLabel = t("ConfigList.Actions.InputConfigItem.DefaultName")
  const configIsDefaultConfig = configItem?.Controller===null && configItemName === defaultLabel

  useEffect(() => {
    setTimeout(() => {
      if (configIsDefaultConfig) {
        inlineEditRef.current?.startEditing()
      }
    }, 500)
  }, [inlineEditRef, configIsDefaultConfig])

  return (
    <Dialog open={true} onOpenChange={closeDialog}>
      <DialogContent
        // prevents focusing on the inactive inline edit label
       
        onEscapeKeyDown={(e) => {
          if ((e.target as HTMLElement).dataset.preventModalCloseOnEscape) {
            e.preventDefault()
          }
        }}
        ref={containerRef}
        className="vlg:min-h-[80%] flex min-h-full flex-col justify-between overflow-x-hidden overflow-y-auto select-none sm:max-w-full lg:max-w-300"
      >
        <DialogHeader>
          <DialogTitle  tabIndex={undefined} className="flex flex-row items-center gap-2 text-2xl" data-testid="dialog-config-name">
            {t("Dialog.InputConfigWizard.Title")}{" "}
            <IconChevronRight className="mt-1" />
            <InlineEditLabel
              labelClassName="text-2xl px-4.5 -ml-5"
              inputClassName="h-8 w-fit text-2xl! -ml-3 h-12 -my-2"
              value={currentConfigItem?.Name || ""}
              onSave={(newName) => {
                if (currentConfigItem) {
                  setCurrentConfigItem({ ...currentConfigItem, Name: newName })
                }
              }}
              ref={inlineEditRef}
            />
          </DialogTitle>
          <DialogDescription className="vsm:block hidden">
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
                  liveData={{ rawValue: configItem?.RawValue, finalValue: configItem?.Value }}
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
