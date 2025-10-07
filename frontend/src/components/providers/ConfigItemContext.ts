import { createContext } from "react";
import { ConfigItemDragContextType } from "./DragDropProvider";

// Create the React context with default values
export const ConfigItemDragContext = createContext<ConfigItemDragContextType>({
  dragState: null,
  table: null,
  setTable: () => {},
  setTableContainerRef: () => {}
})