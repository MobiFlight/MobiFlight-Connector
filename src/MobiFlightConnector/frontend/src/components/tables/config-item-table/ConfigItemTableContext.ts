import { createContext } from "react"

interface ConfigItemContextType {
  onDuplicate: () => void
  registerNameEdit: (itemId: string, editFn: (() => void) | null) => void
  startNameEdit: (itemId: string) => void
}

export const ConfigItemTableContext =
  createContext<ConfigItemContextType | null>(null)
