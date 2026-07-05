import { ILogMessage } from "@/types"
import { create } from "zustand"

interface LogState {
  logs: ILogMessage[]
  addLog: (log: ILogMessage) => void
  clearLogs: () => void
}

export const useLogsStore = create<LogState>((set) => ({
  logs: [],
  addLog: (log) => set((state) => ({ logs: [...state.logs, log] })),
  clearLogs: () => set({ logs: [] }),
}))
