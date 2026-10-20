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
  IconChevronDown,
} from "@tabler/icons-react"
import _ from "lodash"
import { useEffect, useRef, useState } from "react"
import { useTranslation } from "react-i18next"
import { useNavigate } from "react-router"
import {
  DropdownMenu,
  DropdownMenuTrigger,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
} from "@/components/ui/dropdown-menu"

export type InputConfigDialogProps = {
  configId: string
}

const MAX_UNDO_STEPS = 50

type HistoryEntry = {
  snapshot: IConfigItem
  action: string
}

type DrawerSession = {
  view: string
  originalSnapshot: IConfigItem
  history: HistoryEntry[]
  cursor: number
  historyStart: number
}

type EditorState = {
  draftConfigItem: IConfigItem | undefined
  history: HistoryEntry[]
  cursor: number
  historyStart: number
  lastTextEditId: number | null
  drawerSession: DrawerSession | null
}

type HistoryControlsProps = {
  history: HistoryEntry[]
  cursor: number
  historyStart: number
  onUndo: () => void
  onJump: (index: number) => void
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
}: HistoryControlsProps) => {
  const { t } = useTranslation()
  const canUndo = cursor > 0
  const hasHistory = history.length > 1

  const undoTitle = t("Dialog.InputConfigWizard.History.Undo")
  const historyTitle = t("Dialog.InputConfigWizard.History.Title")

  return (
    <div
      data-testid="config-history-controls"
      className="group/undo border-input bg-background relative inline-flex h-9 shrink-0 items-stretch rounded-md border"
    >
      <Button
        data-testid="config-undo-button"
        type="button"
        variant="ghost"
        size="sm"
        className={cn("h-full rounded-md px-3", hasHistory && "pr-10")}
        aria-label={undoTitle}
        title={`${undoTitle} (Ctrl+Z)`}
        disabled={!canUndo}
        onClick={onUndo}
      >
        <IconArrowBackUp size={16} />
        {undoTitle}
      </Button>

      {hasHistory && (
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button
              data-testid="config-history-button"
              type="button"
              variant="ghost"
              size="sm"
              className={cn(
                "border-input bg-background pointer-events-none absolute inset-y-0 right-0 h-full w-8 rounded-l-none rounded-r-md border-l p-0 opacity-0 transition-opacity",
                "group-hover/undo:pointer-events-auto group-hover/undo:opacity-100",
                "group-focus-within/undo:pointer-events-auto group-focus-within/undo:opacity-100",
                "data-[state=open]:pointer-events-auto data-[state=open]:opacity-100",
                "[@media(hover:none)]:pointer-events-auto [@media(hover:none)]:opacity-100",
              )}
              aria-label={historyTitle}
              title={historyTitle}
            >
              <IconChevronDown size={16} />
            </Button>
          </DropdownMenuTrigger>

          <DropdownMenuContent
            data-testid="config-history-panel"
            side="top"
            align="start"
            sideOffset={8}
            className="z-60 w-64 max-w-[85vw]"
          >
            <DropdownMenuLabel className="flex items-center justify-between text-sm font-semibold">
              {historyTitle}

              <span className="text-muted-foreground text-xs font-normal tabular-nums">
                {historyStart + cursor}
              </span>
            </DropdownMenuLabel>

            <DropdownMenuSeparator />

            <div className="max-h-56 overflow-y-auto">
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
                    <DropdownMenuItem
                      key={historyStart + index}
                      data-testid="config-history-entry"
                      aria-current={isCurrent ? "step" : undefined}
                      title={label}
                      className={cn(
                        "text-xs font-normal",
                        isCurrent && "bg-accent font-medium",
                      )}
                      onSelect={() => {
                        if (index < cursor) {
                          onJump(index)
                        }
                      }}
                    >
                      <span className="text-muted-foreground w-5 shrink-0 tabular-nums">
                        {historyStart + index}
                      </span>

                      <span className="min-w-0 flex-1 truncate">{label}</span>

                      {isCurrent && (
                        <IconCheck
                          size={14}
                          className="text-primary shrink-0"
                          aria-label={t(
                            "Dialog.InputConfigWizard.History.Current",
                          )}
                        />
                      )}
                    </DropdownMenuItem>
                  )
                })}
            </div>
          </DropdownMenuContent>
        </DropdownMenu>
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
    drawerSession: null,
  }))

  const draftConfigItem = editorState.draftConfigItem
  const canUndo = editorState.drawerSession
    ? editorState.drawerSession.cursor > 0
    : editorState.cursor > 0

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

  const recordHistoryChange = (
    history: HistoryEntry[],
    cursor: number,
    historyStart: number,
    current: IConfigItem,
    next: IConfigItem,
    textEditId: number | null,
    lastTextEditId: number | null,
  ) => {
    const updatedHistory = history.slice(0, cursor + 1)
    const snapshot = structuredClone(next)

    const sameTextEdit =
      textEditId !== null &&
      lastTextEditId === textEditId &&
      cursor > 0 &&
      updatedHistory.length > 1

    if (sameTextEdit) {
      const original = updatedHistory[updatedHistory.length - 2].snapshot

      updatedHistory[updatedHistory.length - 1] = {
        snapshot,
        action: describeConfigChange(original, snapshot),
      }
    } else {
      updatedHistory.push({
        snapshot,
        action: describeConfigChange(current, snapshot),
      })
    }

    const overflow = Math.max(0, updatedHistory.length - (MAX_UNDO_STEPS + 1))

    const boundedHistory =
      overflow > 0 ? updatedHistory.slice(overflow) : updatedHistory

    return {
      history: boundedHistory,
      cursor: boundedHistory.length - 1,
      historyStart: historyStart + overflow,
    }
  }

  const updateDraftConfigItem = (nextConfigItem: IConfigItem) => {
    const textEditId = activeTextEditIdRef.current

    setEditorState((previous) => {
      const current = previous.draftConfigItem

      if (!current || _.isEqual(current, nextConfigItem)) {
        return previous
      }

      const snapshot = structuredClone(nextConfigItem)
      const session = previous.drawerSession

      if (session) {
        const recorded = recordHistoryChange(
          session.history,
          session.cursor,
          session.historyStart,
          current,
          snapshot,
          textEditId,
          previous.lastTextEditId,
        )

        return {
          ...previous,
          draftConfigItem: snapshot,
          drawerSession: {
            ...session,
            ...recorded,
          },
          lastTextEditId: textEditId,
        }
      }

      // Outside a Drawer, record changes in Wizard history.
      const recorded = recordHistoryChange(
        previous.history,
        previous.cursor,
        previous.historyStart,
        current,
        snapshot,
        textEditId,
        previous.lastTextEditId,
      )

      return {
        ...previous,
        draftConfigItem: snapshot,
        ...recorded,
        lastTextEditId: textEditId,
      }
    })
  }

  const beginDrawerSession = (view: string) => {
    activeTextEditIdRef.current = null

    setEditorState((previous) => {
      if (!previous.draftConfigItem || previous.drawerSession) {
        return previous
      }

      const snapshot = structuredClone(previous.draftConfigItem)

      return {
        ...previous,
        drawerSession: {
          view,
          originalSnapshot: structuredClone(snapshot),
          history: [{ snapshot, action: "" }],
          cursor: 0,
          historyStart: 0,
        },
        lastTextEditId: null,
      }
    })
  }

  const finishDrawerSession = () => {
    activeTextEditIdRef.current = null

    setEditorState((previous) => {
      const session = previous.drawerSession
      const current = previous.draftConfigItem

      if (!session) {
        return previous
      }

      if (!current || _.isEqual(session.originalSnapshot, current)) {
        return {
          ...previous,
          drawerSession: null,
          lastTextEditId: null,
        }
      }

      const base = "Dialog.InputConfigWizard"

      const drawerLabels: Record<string, string> = {
        precondition: t(`${base}.Preconditions.Title`),
        configReference: t(`${base}.ConfigReferences.Title`),
        modifier: t("Dialog.Modifiers.Title"),
        action: t(`${base}.Action.Title`),
      }

      const label =
        drawerLabels[session.view] ?? t(`${base}.History.ConfigurationChanged`)

      const updatedHistory = [
        ...previous.history.slice(0, previous.cursor + 1),
        {
          snapshot: structuredClone(current),
          action: label,
        },
      ]

      const overflow = Math.max(0, updatedHistory.length - (MAX_UNDO_STEPS + 1))

      const history =
        overflow > 0 ? updatedHistory.slice(overflow) : updatedHistory

      return {
        ...previous,
        history,
        cursor: history.length - 1,
        historyStart: previous.historyStart + overflow,
        drawerSession: null,
        lastTextEditId: null,
      }
    })
  }

  const jumpToHistory = (
    index: number,
    scope: "wizard" | "drawer" = "wizard",
  ) => {
    activeTextEditIdRef.current = null

    setEditorState((previous) => {
      if (scope === "drawer") {
        const session = previous.drawerSession

        if (!session || index < 0 || index >= session.cursor) {
          return previous
        }

        const restored = session.history[index].snapshot
        const history = session.history.slice(0, index + 1)

        return {
          ...previous,
          draftConfigItem: structuredClone(restored),
          drawerSession: {
            ...session,
            history,
            cursor: index,
          },
          lastTextEditId: null,
        }
      }

      if (previous.drawerSession) {
        return previous
      }

      if (index < 0 || index >= previous.cursor) {
        return previous
      }

      const restored = previous.history[index].snapshot
      const history = previous.history.slice(0, index + 1)

      return {
        ...previous,
        draftConfigItem: structuredClone(restored),
        history,
        cursor: index,
        lastTextEditId: null,
      }
    })
  }

  const undoConfigItemChange = () => {
    if (!canUndo) return

    if (editorState.drawerSession) {
      jumpToHistory(editorState.drawerSession.cursor - 1, "drawer")
    } else {
      jumpToHistory(editorState.cursor - 1, "wizard")
    }
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
                  updateDraftConfigItem({
                    ...draftConfigItem,
                    Name: newName,
                  })
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
                      history={editorState.drawerSession?.history ?? []}
                      cursor={editorState.drawerSession?.cursor ?? -1}
                      historyStart={
                        editorState.drawerSession?.historyStart ?? 0
                      }
                      onUndo={undoConfigItemChange}
                      onJump={(index) => jumpToHistory(index, "drawer")}
                    />
                  }
                  onDrawerOpen={beginDrawerSession}
                  onDrawerClose={finishDrawerSession}
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
              history={editorState.history}
              cursor={editorState.cursor}
              historyStart={editorState.historyStart}
              onUndo={undoConfigItemChange}
              onJump={(index) => jumpToHistory(index, "wizard")}
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
