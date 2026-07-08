import { LogEntry } from "@/types/log"
import { create } from "zustand"

const MAX_LOGS = 500

interface LogState {
  logs: LogEntry[]
  addLog: (log: LogEntry) => void
  clearLogs: () => void
}

export const useLogsStore = create<LogState>((set) => ({
  logs: [],
  addLog: (log) =>
    set((state) => ({
      logs: [...state.logs, log].slice(-MAX_LOGS),
    })),
  clearLogs: () => set({ logs: [] }),
}))