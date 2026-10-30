import { IConfigItem } from "@/types/config"
import _ from "lodash"
import { useState, type KeyboardEvent } from "react"
import useTextEditSession, {
  isTextEditingTarget,
} from "@/lib/hooks/useTextEditSession"
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

const useInputConfigHistory = (configItem: IConfigItem | undefined) => {
  const { t } = useTranslation()

  const {
    getCurrentTextEditId,
    resetTextEditSession,
    handleFocusCapture,
    handleBlurCapture,
  } = useTextEditSession()

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

  const recordHistoryChange = (
    history: HistoryEntry[],
    cursor: number,
    historyStart: number,
    next: IConfigItem,
    textEditId: number | null,
    lastTextEditId: number | null,
    actionLabel?: string,
  ) => {
    const updatedHistory = history.slice(0, cursor + 1)
    const snapshot = structuredClone(next)

    const sameTextEdit =
      textEditId !== null &&
      lastTextEditId === textEditId &&
      cursor > 0 &&
      updatedHistory.length > 1

    if (sameTextEdit) {
      const lastEntry = updatedHistory[updatedHistory.length - 1]

      updatedHistory[updatedHistory.length - 1] = {
        snapshot,
        action: actionLabel ?? lastEntry.action,
      }
    } else {
      updatedHistory.push({
        snapshot,
        action:
          actionLabel ??
          t("Dialog.InputConfigWizard.History.ConfigurationChanged"),
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

  const updateDraftConfigItem = (
    nextConfigItem: IConfigItem,
    actionLabel?: string,
  ) => {
    const textEditId = getCurrentTextEditId()

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
          snapshot,
          textEditId,
          previous.lastTextEditId,
          actionLabel,
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
        snapshot,
        textEditId,
        previous.lastTextEditId,
        actionLabel,
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
    resetTextEditSession()

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
    resetTextEditSession()

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
    resetTextEditSession()

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
