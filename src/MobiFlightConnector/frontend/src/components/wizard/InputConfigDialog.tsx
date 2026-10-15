import {
  InlineEditLabel,
  InlineEditLabelRef,
} from "@/components/InlineEditLabel"
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
import messageExchange from "@/lib/messageExchange"
import { cn } from "@/lib/utils"
import { useProjectStore } from "@/stores/projectStore"
import { useConfigItemStateValue } from "@/stores/configItemStateStore"
import { IConfigItem } from "@/types/config"
import { IconChevronRight, IconExclamationCircle } from "@tabler/icons-react"
import _ from "lodash"
import { useEffect, useRef, useState } from "react"
import { useTranslation } from "react-i18next"
import { useNavigate } from "react-router"

export type InputConfigDialogProps = {
  configId: string
}

const MAX_UNDO_STEPS = 50

const isTextEditingTarget = (target: EventTarget | null): boolean => {
  if (!(target instanceof HTMLElement)) return false

  if (target.closest('[contenteditable="true"]')) return true

  if (target instanceof HTMLTextAreaElement) return true

  return (
    target instanceof HTMLInputElement &&
    ["text", "search", "number", "email", "password", "tel", "url"].includes(
      target.type,
    )
  )
}

type EditorState = {
  draftConfigItem: IConfigItem | undefined
  undoHistory: IConfigItem[]
  lastTextEditId: number | null
}

const resetTriggersBasedOnDeviceType = (
  configItem: IConfigItem | undefined,
) => {
  if (!configItem?.Device) return configItem

  switch (configItem.Device.Type) {
    case "Button":
      return { ...configItem, analog: undefined, encoder: undefined }
    case "Encoder":
      return { ...configItem, analog: undefined, button: undefined }
    case "AnalogInput":
      return { ...configItem, encoder: undefined, button: undefined }
    default:
      return configItem
  }
}

const InputConfigDialog = ({ configId }: InputConfigDialogProps) => {
  const { t } = useTranslation()
  const navigate = useNavigate()
  const { project, activeConfigFileIndex } = useProjectStore()
  const configFile = project?.ConfigFiles[activeConfigFileIndex]
  const configItem = configFile?.ConfigItems?.find(
    (item) => item.GUID === configId,
  )
  const runtimeState = useConfigItemStateValue(configId)

  const containerRef = useRef<HTMLDivElement>(null)
  const inlineEditRef = useRef<InlineEditLabelRef>(null)

  const nextTextEditIdRef = useRef(0)
  const activeTextEditIdRef = useRef<number | null>(null)

  const [editorState, setEditorState] = useState<EditorState>(() => ({
    draftConfigItem: configItem ? structuredClone(configItem) : undefined,
    undoHistory: [],
    lastTextEditId: null,
  }))

  const draftConfigItem = editorState.draftConfigItem
  const canUndo = editorState.undoHistory.length > 0

  const [committedConfigItem, setCommittedConfigItem] = useState(() =>
    configItem ? structuredClone(configItem) : undefined,
  )

  const [isWiggling, setIsWiggling] = useState(false)
  const [showPendingChangesMessage, setShowPendingChangesMessage] =
    useState(false)
  const [preventDialogAnimation, setPreventDialogAnimation] = useState(false)

  const closeDialog = () => {
    navigate(-1)
  }

  const updateDraftConfigItem = (nextConfigItem: IConfigItem) => {
    const textEditId = activeTextEditIdRef.current

    setEditorState((previous) => {
      const current = previous.draftConfigItem

      if (!current || _.isEqual(current, nextConfigItem)) {
        return previous
      }

      const sameTextEdit =
        textEditId !== null && previous.lastTextEditId === textEditId

      return {
        draftConfigItem: structuredClone(nextConfigItem),
        undoHistory: sameTextEdit
          ? previous.undoHistory
          : [...previous.undoHistory, structuredClone(current)].slice(
              -MAX_UNDO_STEPS,
            ),
        lastTextEditId: textEditId,
      }
    })
  }

  const undoConfigItemChange = () => {
    setEditorState((previous) => {
      if (previous.undoHistory.length === 0) {
        return previous
      }

      const previousConfigItem =
        previous.undoHistory[previous.undoHistory.length - 1]

      return {
        draftConfigItem: structuredClone(previousConfigItem),
        undoHistory: previous.undoHistory.slice(0, -1),
        lastTextEditId: null,
      }
    })
  }

  const handleUndoKeyDown = (event: React.KeyboardEvent<HTMLDivElement>) => {
    if (
      !(event.ctrlKey || event.metaKey) ||
      event.altKey ||
      event.shiftKey ||
      event.key.toLowerCase() !== "z"
    ) {
      return
    }

    // Preserve native Undo while editing text.
    if (isTextEditingTarget(event.target)) {
      return
    }

    if (!canUndo) {
      return
    }

    event.preventDefault()
    event.stopPropagation()

    undoConfigItemChange()
  }

  const saveChanges = () => {
    const { publish } = messageExchange

    const finalConfigItem = resetTriggersBasedOnDeviceType(draftConfigItem)
    setCommittedConfigItem(finalConfigItem)
    setShowPendingChangesMessage(false)

    publish({
      key: "CommandUpdateConfigItem",
      payload: {
        item: finalConfigItem,
      },
    })

    closeDialog()
  }

  // Automatically start editing the config item name if it is the default name
  const configItemName = configItem?.Name
  // this is a simple test to check if the config item is a newly created default config item
  const defaultLabel = t("ConfigList.Actions.InputConfigItem.DefaultName")
  const configIsDefaultConfig =
    configItem?.Controller === null && configItemName === defaultLabel

  const hasChanged = () => {
    return !_.isEqual(committedConfigItem, draftConfigItem)
  }

  useEffect(() => {
    setTimeout(() => {
      if (configIsDefaultConfig) {
        inlineEditRef.current?.startEditing()
      }
    }, 500)
  }, [inlineEditRef, configIsDefaultConfig])

  const wiggleIt = () => {
    setTimeout(() => {
      setPreventDialogAnimation(true)
      setIsWiggling(true)
      setShowPendingChangesMessage(true)
      setTimeout(() => {
        setIsWiggling(false)
      }, 150)
    }, 100)
  }

  return (
    <Dialog open={true} onOpenChange={closeDialog}>
      <DialogContent
        data-testid="inputconfigwizard-dialog"
        onKeyDownCapture={handleUndoKeyDown}
        // prevents focusing on the inactive inline edit label
        onFocusCapture={(e) => {
          if (isTextEditingTarget(e.target)) {
            nextTextEditIdRef.current += 1
            activeTextEditIdRef.current = nextTextEditIdRef.current
          }
        }}
        onBlurCapture={(e) => {
          if (isTextEditingTarget(e.target)) {
            activeTextEditIdRef.current = null
          }
        }}
        onInteractOutside={(e) => {
          if (!hasChanged()) return
          wiggleIt()
          e.preventDefault()
        }}
        onEscapeKeyDown={(e) => {
          if (hasChanged()) {
            e.preventDefault()
            wiggleIt()
            return
          }
          if ((e.target as HTMLElement).dataset.preventModalCloseOnEscape) {
            e.preventDefault()
            return
          }
        }}
        ref={containerRef}
        className={cn(
          "vlg:min-h-[80%] flex min-h-full flex-col justify-between overflow-x-hidden overflow-y-auto select-none sm:max-w-full lg:max-w-300",
          isWiggling && "data-[state=open]:animate-wiggle!",
          preventDialogAnimation ? "data-[state=open]:animate-none!" : "",
        )}
      >
        <DialogHeader>
          <DialogTitle
            tabIndex={undefined}
            className="flex flex-row items-center gap-2 text-2xl"
            data-testid="dialog-config-name"
          >
            {t("Dialog.InputConfigWizard.Title")}{" "}
            <IconChevronRight className="mt-1" />
            <InlineEditLabel
              labelClassName="text-2xl px-4.5 -ml-5"
              inputClassName="h-8 w-fit text-2xl! -ml-3 h-12 -my-2"
              value={draftConfigItem?.Name || ""}
              onSave={(newName) => {
                if (draftConfigItem) {
                  updateDraftConfigItem({ ...draftConfigItem, Name: newName })
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
              {draftConfigItem && (
                <ConfigWizard
                  configItem={draftConfigItem}
                  onConfigChange={(item) => {
                    updateDraftConfigItem(item)
                  }}
                  drawerContainer={containerRef}
                  liveData={{
                    rawValue: runtimeState?.RawValue ?? configItem?.RawValue,
                    finalValue: runtimeState?.Value ?? configItem?.Value,
                  }}
                />
              )}
            </div>
          </ScrollArea>
        </div>
        <DialogFooter
          className="flex flex-row justify-between"
          data-testid="inputconfigwizard-dialog-footer"
        >
          <div className="flex grow flex-row items-center gap-1">
            {showPendingChangesMessage && (
              <>
                <IconExclamationCircle className="text-primary mr-2 inline h-8 w-8" />
                <span className="text-primary">
                  {t("Dialog.General.PendingChangesMessage")}
                </span>
              </>
            )}
          </div>
          <div className="flex flex-row gap-2">
            <Button variant="outline" onClick={closeDialog}>
              {t("Dialog.General.Cancel")}
            </Button>
            <Button variant="default" onClick={saveChanges}>
              {t("Dialog.General.ApplyChanges")}
            </Button>
          </div>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
export default InputConfigDialog
