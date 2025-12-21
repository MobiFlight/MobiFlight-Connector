import { create } from "zustand"

interface ExecutionState {
  isRunning: boolean
  isTesting: boolean
  project: string | null
  setIsRunning: (isExecuting: boolean) => void
  setIsTesting: (isTesting: boolean) => void
}

export const useExecutionStateStore = create<ExecutionState>((set) => ({
  isRunning: false,
  isTesting: false,
  project: null,
  setIsRunning: (isRunning) => set({ isRunning: isRunning }),
  setIsTesting: (isTesting) => set({ isTesting: isTesting }),
  setProject: (project: string | null) => set({ project: project })
}))