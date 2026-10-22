import { createContext } from "react"
import type { ConfigItemDragContextType } from "./DragDropProvider"

export interface ConfigItemDragVisualContextType {
  draggedItemIds: ReadonlySet<string>
  isInsideTable: boolean
}

// Create the React context with default values
export const ConfigItemDragContext = createContext<ConfigItemDragContextType>({
  dragState: null,
  table: null,
  setTable: () => {},
  setTableContainerRef: () => {},
})

export const ConfigItemDragVisualContext =
  createContext<ConfigItemDragVisualContextType>({
    draggedItemIds: new Set(),
    isInsideTable: true,
  })
