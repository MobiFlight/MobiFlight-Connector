import { create } from "zustand"

interface ExecutionState {
  isRunning: boolean
  isTesting: boolean
  setIsRunning: (isExecuting: boolean) => void
  setIsTesting: (isTesting: boolean) => void
}

export const useExecutionStateStore = create<ExecutionState>((set) => ({
  isRunning: false,
  isTesting: false,
  setIsRunning: (isRunning) => set({ isRunning: isRunning }),
  setIsTesting: (isTesting) => set({ isTesting: isTesting })
}))