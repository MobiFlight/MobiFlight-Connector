import { create } from 'zustand'
import { HubHopState } from '@/types/messages'

interface StateStore {
  hubHopState: HubHopState | null
  missingControllers: {
    visible: boolean
    controllerType: string
  } | null
}

interface StateActions {
  setHubHopState: (state: HubHopState) => void
  setMissingControllers: (visible: boolean, controllerType?: string) => void
}

export const useStateStore = create<StateStore & StateActions>((set) => ({
  // State
  hubHopState: null,
  missingControllers: null,

  // Actions
  setHubHopState: (state) => set({ hubHopState: state }),
  setMissingControllers: (visible, controllerType = "Board") => 
    set({ missingControllers: visible ? { visible, controllerType } : null }),
}))

export const useHubHopStateActions = () => useStateStore((state) => state.setHubHopState)

export const useHubHopState = () => useStateStore((state) => state.hubHopState)