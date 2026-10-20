import { Action, IConfigItem } from "@/types/config"
import _ from "lodash"
import { useRef, useState, type FocusEvent, type KeyboardEvent } from "react"
import { useTranslation } from "react-i18next"

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

const useInputConfigHistory = (configItem: IConfigItem | undefined) => {
  const { t } = useTranslation()

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
      const before = previous.ConfigRefs ?? []
      const after = next.ConfigRefs ?? []

      if (after.length > before.length) {
        return t(`${base}.ConfigReferenceAdded`)
      }

      if (after.length < before.length) {
        return t(`${base}.ConfigReferenceRemoved`)
      }

      const changedIndex = after.findIndex(
        (item, index) => !_.isEqual(item, before[index]),
      )

      if (changedIndex !== -1) {
        const oldReference = before[changedIndex]
        const newReference = after[changedIndex]

        if (oldReference.Active !== newReference.Active) {
          return t(
            newReference.Active
              ? `${base}.ConfigReferenceEnabled`
              : `${base}.ConfigReferenceDisabled`,
          )
        }
      }

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

  const handleUndoKeyDown = (event: KeyboardEvent<HTMLDivElement>) => {
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

  const handleFocusCapture = (event: FocusEvent<HTMLDivElement>) => {
    if (isTextEditingTarget(event.target)) {
      nextTextEditIdRef.current += 1
      activeTextEditIdRef.current = nextTextEditIdRef.current
    }
  }

  const handleBlurCapture = (event: FocusEvent<HTMLDivElement>) => {
    if (isTextEditingTarget(event.target)) {
      activeTextEditIdRef.current = null
    }
  }

  return {
    editorState,
    draftConfigItem,
    updateDraftConfigItem,
    beginDrawerSession,
    finishDrawerSession,
    jumpToHistory,
    undoConfigItemChange,
    handleUndoKeyDown,
    handleFocusCapture,
    handleBlurCapture,
  }
}

export default useInputConfigHistory
