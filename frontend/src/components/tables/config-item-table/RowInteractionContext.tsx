import { createContext, useContext, useRef, ReactNode } from "react"

interface RowInteractionContextValue {
  startNameEdit?: () => void
  registerNameEdit: (editFn: () => void) => void
}

const RowInteractionContext = createContext<RowInteractionContextValue | undefined>(undefined)

interface RowInteractionProviderProps {
  children: ReactNode
}

export const RowInteractionProvider = ({ children }: RowInteractionProviderProps) => {
  const nameEditRef = useRef<(() => void) | undefined>(undefined)

  const registerNameEdit = (editFn: () => void) => {
    nameEditRef.current = editFn
  }

  const startNameEdit = () => {
    nameEditRef.current?.()
  }

  const value: RowInteractionContextValue = {
    startNameEdit,
    registerNameEdit,
  }

  return (
    <RowInteractionContext.Provider value={value}>
      {children}
    </RowInteractionContext.Provider>
  )
}

export const useRowInteraction = () => {
  const context = useContext(RowInteractionContext)
  if (context === undefined) {
    throw new Error("useRowInteraction must be used within a RowInteractionProvider")
  }
  return context
}
