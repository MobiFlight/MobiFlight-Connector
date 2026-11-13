import { create } from "zustand"

interface HistoryState {
  canUndoConfigList: boolean
  configListUndoHistory: string[]
  configListUndoHistoryStart: number

  setCanUndoConfigList: (canUndo: boolean) => void

  setConfigListUndoHistory: (history: string[], historyStart: number) => void
}

export const useHistoryStore = create<HistoryState>((set) => ({
  canUndoConfigList: false,
  configListUndoHistory: [],
  configListUndoHistoryStart: 0,

  setCanUndoConfigList: (canUndo) => set({ canUndoConfigList: canUndo }),

  setConfigListUndoHistory: (history, historyStart) =>
    set({
      configListUndoHistory: history,
      configListUndoHistoryStart: historyStart,
    }),
}))
