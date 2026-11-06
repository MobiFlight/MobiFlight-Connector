import {
  ConfigItemDragContext,
  ConfigItemDragVisualContext,
} from "@/components/providers/ConfigItemContext"
import { useContext } from "react"

/**
 * Hook for components to access the drag context
 */
export const useConfigItemDragContext = () => useContext(ConfigItemDragContext)

export const useConfigItemDragVisualContext = () =>
  useContext(ConfigItemDragVisualContext)
