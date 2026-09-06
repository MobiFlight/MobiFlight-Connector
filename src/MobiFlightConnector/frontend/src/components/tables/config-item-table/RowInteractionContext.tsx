import { useRef, ReactNode } from "react"
import { RowInteractionContext, RowInteractionContextValue } from "./RowInteractionContextDef"

interface RowInteractionProviderProps {
  children: ReactNode
  handleRef?: (element: Element | null) => void
}

export const RowInteractionProvider = ({
  children,
  handleRef,
}: RowInteractionProviderProps) => {
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
    handleRef,
  }

  return (
    <RowInteractionContext.Provider value={value}>
      {children}
    </RowInteractionContext.Provider>
  )
}