import { create } from 'zustand';

export type ExecutionState = "Stopped" | "Running" | "Paused" | "Testing"

interface ExecutionStateState {
    state: ExecutionState
    setState: (state: ExecutionState) => void
}

export const useExecutionStateStore = create<ExecutionStateState>((set) => ({
    state: "Stopped",
    setState: (newState) => set(() => ({ state : newState }))
}))