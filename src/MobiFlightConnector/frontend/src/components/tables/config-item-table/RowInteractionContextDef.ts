import { createContext } from "react"

export interface RowInteractionContextValue {
  startNameEdit?: () => void
  registerNameEdit: (editFn: () => void) => void
}

export const RowInteractionContext = createContext<RowInteractionContextValue | undefined>(undefined)