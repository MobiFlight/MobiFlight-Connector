import { useRef, ReactNode } from "react"
import { RowInteractionContext, RowInteractionContextValue } from "./RowInteractionContextDef"

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