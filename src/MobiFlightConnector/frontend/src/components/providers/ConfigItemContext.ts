import { createContext } from "react"
import {
  ConfigItemDragContextType,
  ConfigItemDragVisualContextType,
} from "./DragDropProvider"

// Create the React context with default values
export const ConfigItemDragContext = createContext<ConfigItemDragContextType>({
  dragState: null,
  table: null,
  setTable: () => {},
  setTableContainerRef: () => {},
})

export const ConfigItemDragVisualContext =
  createContext<ConfigItemDragVisualContextType>({
    draggedItemIds: new Set<string>(),
    isInsideTable: true,
  })
