import { MobiFlightVariable } from '@/types/config'
import { create } from 'zustand'

interface VariableState {
  variables: MobiFlightVariable[]
  setVariables: (variables: MobiFlightVariable[]) => void
}

export const useVariableStore = create<VariableState>((set) => ({
  variables: [],
  setVariables: (variables) => set({ variables }),
}))