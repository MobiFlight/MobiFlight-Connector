import { createContext } from "react"

interface ConfigItemContextType {
  onDuplicate: () => void
}

export const ConfigItemTableContext =
  createContext<ConfigItemContextType | null>(null)
