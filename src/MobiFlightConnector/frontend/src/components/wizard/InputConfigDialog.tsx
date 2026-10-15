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
import { Action, IConfigItem } from "@/types/config"
import {
  IconChevronRight,
  IconExclamationCircle,
  IconArrowBackUp,
  IconCheck,
  IconHistory,
} from "@tabler/icons-react"
import _ from "lodash"
import { useEffect, useId, useRef, useState } from "react"
import { useTranslation } from "react-i18next"
import { useNavigate } from "react-router"

export type InputConfigDialogProps = {
  configId: string
}

const MAX_UNDO_STEPS = 50

type HistoryEntry = {
  snapshot: IConfigItem
  action: string
}

type EditorState = {
  draftConfigItem: IConfigItem | undefined
  history: HistoryEntry[]
  cursor: number
  historyStart: number
  lastTextEditId: number | null
}

type HistoryControlsProps = {
  history: HistoryEntry[]
  cursor: number
  historyStart: number
  onUndo: () => void
  onJump: (index: number) => void
  placement?: "up" | "down"
}

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

const HistoryControls = ({
  history,
  cursor,
  historyStart,
  onUndo,
  onJump,
  placement = "up",
}: HistoryControlsProps) => {
  const { t } = useTranslation()
  const [open, setOpen] = useState(false)
  const panelId = useId()

  const canUndo = cursor > 0
  const latestStep = historyStart + history.length - 1
  const currentStep = historyStart + cursor

  return (
    <div
      className="relative flex shrink-0 items-center gap-2"
      data-testid="config-history-controls"
    >
      <Button
        type="button"
        variant="outline"
        size="sm"
        aria-label={t("Dialog.InputConfigWizard.History.Undo")}
        title={t("Dialog.InputConfigWizard.History.Undo")}
        disabled={!canUndo}
        onClick={onUndo}
      >
        <IconArrowBackUp size={16} />
        {t("Dialog.InputConfigWizard.History.Undo")}
      </Button>

      <Button
        type="button"
        variant="outline"
        size="sm"
        disabled={history.length === 0}
        aria-label={t("Dialog.InputConfigWizard.History.Title")}
        aria-expanded={open}
        aria-controls={panelId}
        onClick={() => setOpen((previous) => !previous)}
      >
        <IconHistory size={16} />
        {t("Dialog.InputConfigWizard.History.Title")}
        <span className="text-muted-foreground text-xs">
          {Math.max(currentStep, 0)}/{Math.max(latestStep, 0)}
        </span>
      </Button>

      {open && (
        <div
          id={panelId}
          data-testid="config-history-panel"
          className={cn(
            "bg-popover text-popover-foreground absolute right-0 z-60 w-80 max-w-[85vw] rounded-md border shadow-lg",
            placement === "up" ? "bottom-full mb-2" : "top-full mt-2",
          )}
        >
          <div className="border-b px-3 py-2 text-sm font-semibold">
            {t("Dialog.InputConfigWizard.History.Title")}
          </div>

          <div className="max-h-64 overflow-y-auto p-1">
            {history
              .map((entry, index) => ({ entry, index }))
              .reverse()
              .map(({ entry, index }) => {
                const isCurrent = index === cursor
                const label =
                  index === 0
                    ? historyStart === 0
                      ? t("Dialog.InputConfigWizard.History.Original")
                      : t("Dialog.InputConfigWizard.History.Oldest")
                    : entry.action

                return (
                  <button
                    key={historyStart + index}
                    type="button"
                    data-testid="config-history-entry"
                    aria-current={isCurrent ? "step" : undefined}
                    className={cn(
                      "hover:bg-accent flex w-full items-center gap-2 rounded-sm px-3 py-2 text-left text-sm",
                      isCurrent && "bg-accent font-medium",
                    )}
                    onClick={() => onJump(index)}
                  >
                    <span className="text-muted-foreground w-6 shrink-0 text-xs">
                      {historyStart + index}
                    </span>

                    <span className="min-w-0 flex-1 wrap-break-word">
                      {label}
                    </span>

                    {isCurrent && (
                      <IconCheck
                        size={16}
                        className="text-primary shrink-0"
                        aria-label={t(
                          "Dialog.InputConfigWizard.History.Current",
                        )}
                      />
                    )}
                  </button>
                )
              })}
          </div>
        </div>
      )}
    </div>
  )
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
    history: configItem
      ? [{ snapshot: structuredClone(configItem), action: "" }]
      : [],
    cursor: configItem ? 0 : -1,
    historyStart: 0,
    lastTextEditId: null,
  }))

  const draftConfigItem = editorState.draftConfigItem
  const canUndo = editorState.cursor > 0

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

  const describeConfigChange = (
    previous: IConfigItem,
    next: IConfigItem,
  ): string => {
    const base = "Dialog.InputConfigWizard.History"

    if (previous.Name !== next.Name) {
      return t(`${base}.NameChanged`, { name: next.Name })
    }

    if (!_.isEqual(previous.Preconditions, next.Preconditions)) {
      const before = previous.Preconditions ?? []
      const after = next.Preconditions ?? []

      if (after.length > before.length) {
        return t(`${base}.PreconditionAdded`)
      }

      if (after.length < before.length) {
        return t(`${base}.PreconditionRemoved`)
      }

      return t(`${base}.PreconditionChanged`)
    }

    if (!_.isEqual(previous.Modifiers, next.Modifiers)) {
      const before = previous.Modifiers?.Items ?? []
      const after = next.Modifiers?.Items ?? []

      if (after.length > before.length) {
        const added =
          after.find(
            (modifier) =>
              modifier.Id && !before.some((item) => item.Id === modifier.Id),
          ) ?? after[after.length - 1]

        return t(`${base}.ModifierAdded`, {
          type: added?.Type ?? "",
        })
      }

      if (after.length < before.length) {
        const removed =
          before.find(
            (modifier) =>
              modifier.Id && !after.some((item) => item.Id === modifier.Id),
          ) ?? before[before.length - 1]

        return t(`${base}.ModifierRemoved`, {
          type: removed?.Type ?? "",
        })
      }

      const changedIndex = after.findIndex(
        (item, index) => !_.isEqual(item, before[index]),
      )

      if (changedIndex !== -1) {
        const oldModifier = before[changedIndex]
        const modifier = after[changedIndex]
        const type = modifier.Type

        if (oldModifier.Active !== modifier.Active) {
          return t(
            modifier.Active
              ? `${base}.ModifierEnabled`
              : `${base}.ModifierDisabled`,
            { type },
          )
        }

        if (
          oldModifier.Type === "Transformation" &&
          modifier.Type === "Transformation" &&
          oldModifier.Expression !== modifier.Expression
        ) {
          return t(`${base}.ModifierExpressionChanged`, { type })
        }

        return t(`${base}.ModifierChanged`, { type })
      }

      return t(`${base}.ConfigurationChanged`)
    }

    if (!_.isEqual(previous.ConfigRefs, next.ConfigRefs)) {
      return t(`${base}.ConfigReferenceChanged`)
    }

    const actionChanges: {
      event: string
      before: Action | null | undefined
      after: Action | null | undefined
    }[] = [
      {
        event: t("Dialog.InputConfigWizard.button.Event.onPress.label"),
        before: previous.button?.onPress,
        after: next.button?.onPress,
      },
      {
        event: t("Dialog.InputConfigWizard.button.Event.onRelease.label"),
        before: previous.button?.onRelease,
        after: next.button?.onRelease,
      },
      {
        event: t("Dialog.InputConfigWizard.button.Event.onHold.label"),
        before: previous.button?.onHold,
        after: next.button?.onHold,
      },
      {
        event: t("Dialog.InputConfigWizard.button.Event.onLongRelease.label"),
        before: previous.button?.onLongRelease,
        after: next.button?.onLongRelease,
      },
      {
        event: t("Dialog.InputConfigWizard.encoder.Event.onLeft.label"),
        before: previous.encoder?.onLeft,
        after: next.encoder?.onLeft,
      },
      {
        event: t("Dialog.InputConfigWizard.encoder.Event.onRight.label"),
        before: previous.encoder?.onRight,
        after: next.encoder?.onRight,
      },
      {
        event: t("Dialog.InputConfigWizard.analog.Event.onChange.label"),
        before: previous.analog?.onChange,
        after: next.analog?.onChange,
      },
    ]

    for (const change of actionChanges) {
      if (_.isEqual(change.before, change.after)) continue

      if (change.before?.Type !== change.after?.Type) {
        return t(`${base}.ActionTypeChanged`, {
          event: change.event,
        })
      }
      if (_.get(change.before, "Command") !== _.get(change.after, "Command")) {
        return t(`${base}.ActionCodeChanged`, {
          event: change.event,
        })
      }
      return t(`${base}.ActionChanged`, {
        event: change.event,
      })
    }

    if (
      !_.isEqual(previous.button, next.button) ||
      !_.isEqual(previous.encoder, next.encoder) ||
      !_.isEqual(previous.analog, next.analog) ||
      !_.isEqual(previous.Device, next.Device) ||
      !_.isEqual(previous.Controller, next.Controller)
    ) {
      return t(`${base}.TriggerChanged`)
    }

    return t(`${base}.ConfigurationChanged`)
  }

  const updateDraftConfigItem = (nextConfigItem: IConfigItem) => {
    const textEditId = activeTextEditIdRef.current

    setEditorState((previous) => {
      const current = previous.draftConfigItem

      if (!current || _.isEqual(current, nextConfigItem)) {
        return previous
      }

      const sameTextEdit =
        textEditId !== null &&
        previous.lastTextEditId === textEditId &&
        previous.cursor > 0

      const history = previous.history.slice(0, previous.cursor + 1)
      const snapshot = structuredClone(nextConfigItem)

      if (sameTextEdit) {
        const original = history[history.length - 2].snapshot

        history[history.length - 1] = {
          snapshot,
          action: describeConfigChange(original, snapshot),
        }
      } else {
        history.push({
          snapshot,
          action: describeConfigChange(current, snapshot),
        })
      }

      const overflow = Math.max(0, history.length - (MAX_UNDO_STEPS + 1))

      const boundedHistory = overflow > 0 ? history.slice(overflow) : history

      return {
        draftConfigItem: snapshot,
        history: boundedHistory,
        cursor: boundedHistory.length - 1,
        historyStart: previous.historyStart + overflow,
        lastTextEditId: textEditId,
      }
    })
  }

  const jumpToHistory = (index: number) => {
    activeTextEditIdRef.current = null
    setEditorState((previous) => {
      if (index < 0 || index >= previous.history.length) {
        return previous
      }

      const history = previous.history.slice(0, index + 1)

      return {
        ...previous,
        draftConfigItem: structuredClone(previous.history[index].snapshot),
        history,
        cursor: index,
        lastTextEditId: null,
      }
    })
  }

  const undoConfigItemChange = () => {
    if (!canUndo) return
    jumpToHistory(editorState.cursor - 1)
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

  const hasChanged = () => !_.isEqual(committedConfigItem, draftConfigItem)

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
                  historyControls={
                    <HistoryControls
                      placement="down"
                      history={editorState.history}
                      cursor={editorState.cursor}
                      historyStart={editorState.historyStart}
                      onUndo={undoConfigItemChange}
                      onJump={jumpToHistory}
                    />
                  }
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
          className="flex flex-row justify-between gap-2"
          data-testid="inputconfigwizard-dialog-footer"
        >
          <div className="flex grow flex-row items-center gap-2">
            <HistoryControls
              placement="up"
              history={editorState.history}
              cursor={editorState.cursor}
              historyStart={editorState.historyStart}
              onUndo={undoConfigItemChange}
              onJump={jumpToHistory}
            />

            {showPendingChangesMessage && (
              <>
                <IconExclamationCircle className="text-primary ml-2 inline h-8 w-8" />
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
