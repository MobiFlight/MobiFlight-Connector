import { useContext } from "react"
import { RowInteractionContext } from "@/components/tables/config-item-table/RowInteractionContextDef"

export const useRowInteraction = () => {
  const context = useContext(RowInteractionContext)
  if (context === undefined) {
    throw new Error("useRowInteraction must be used within a RowInteractionProvider")
  }
  return context
}