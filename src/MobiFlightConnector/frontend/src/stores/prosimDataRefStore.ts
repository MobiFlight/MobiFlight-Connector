import { create } from "zustand"

export type ProSimDataRefDefinition = {
  Name: string
  Description: string
  CanRead: boolean
  CanWrite: boolean
  DataType: string
  DataUnit: string
}

interface ProSimDataRefState {
  dataRefs: Record<string, ProSimDataRefDefinition>
  setDataRefs: (dataRefs: Record<string, ProSimDataRefDefinition>) => void
}

export const useProSimDataRefStore = create<ProSimDataRefState>((set) => ({
  dataRefs: {},
  setDataRefs: (dataRefs) => set({ dataRefs })
}))

