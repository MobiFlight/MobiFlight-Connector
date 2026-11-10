import type { IConfigItem } from "@/types"
import type { DragState } from "@/components/providers/DragDropProvider"
import messageExchange from "@/lib/messageExchange"
import { CommandResortConfigItem } from "@/types/commands"
import { DragOperation } from "@dnd-kit/abstract"
/**
 * Validation result for drag end operations
 */
export interface DragEndValidation {
  isValid: boolean
  reason?: string
}

/**
 * Context information for calculating insertion index
 */
export interface DropContext {
  hoveringOverTab: boolean
  dropOnPlaceholder: boolean
  dropTargetItemId: string
  itemsWithoutDragged: IConfigItem[]
  currentItems: IConfigItem[]
  draggedItems: IConfigItem[]
}

/**
 * Configuration for executing a drop operation
 */
export interface DropConfig {
  sourceConfigIndex: number
  targetConfigIndex: number
  insertionIndex: number
}
type DragEndEvent = {
  operation: DragOperation
  canceled: boolean
}

/**
 * Validates if a drag end event should be processed
 */
export const validateDragEnd = (
  event: DragEndEvent,
  dragState: DragState | null,
  effectiveOverId?: string | number,
): DragEndValidation => {
  const { operation } = event

  const activeId = operation.source?.id
  const overId = effectiveOverId ?? operation.target?.id

  if (!dragState?.items || !dragState.configs) {
    return { isValid: false, reason: "Invalid drag state" }
  }

  if (!activeId) {
    return { isValid: false, reason: "No active item" }
  }

  if (!overId) {
    return { isValid: false, reason: "Dropped outside valid zone" }
  }

  if (
    activeId === overId &&
    dragState.configs.source === dragState.configs.current
  ) {
    return { isValid: false, reason: "Item dropped on itself" }
  }

  return { isValid: true }
}

/**
 * Calculates insertion index based on drop context
 */
export const calculateInsertionIndex = (
  dropContext: DropContext,
  isCrossConfig: boolean,
  activeId?: string | number,
  tableContainer?: Element | null,
): number => {
  const {
    hoveringOverTab,
    dropOnPlaceholder,
    dropTargetItemId,
    itemsWithoutDragged,
    currentItems,
    draggedItems,
  } = dropContext

  // Dropping on tab, placeholder, or table header always inserts at index 0 (top of table)
  if (
    hoveringOverTab ||
    dropOnPlaceholder ||
    dropTargetItemId === "config-item-table-header" ||
    itemsWithoutDragged.length === 0
  ) {
    return 0
  }

  // Dropping on the table body container below rows inserts at the end of the table
  if (dropTargetItemId === "config-item-table-body") {
    return itemsWithoutDragged.length
  }

  const draggedGuid = (activeId as string) || draggedItems[0]?.GUID

  // 1. If we have the table container and this is a same-config drop inside the table,
  // the DOM order of rows in tbody (reordered dynamically by OptimisticSortingPlugin)
  // is the exact visual ground truth of where the user positioned the row.
  if (!isCrossConfig && tableContainer) {
    const domRows = Array.from(tableContainer.querySelectorAll("tr[dnd-itemid]"))
    const draggedGuids = new Set(
      draggedItems.map((item) => item.GUID).concat(draggedGuid ? [draggedGuid] : []),
    )
    const firstDraggedDomIndex = domRows.findIndex((row) =>
      draggedGuids.has(row.getAttribute("dnd-itemid") || ""),
    )

    if (firstDraggedDomIndex !== -1) {
      let nonDraggedBefore = 0
      for (let i = 0; i < firstDraggedDomIndex; i++) {
        const id = domRows[i].getAttribute("dnd-itemid")
        if (id && !draggedGuids.has(id)) {
          nonDraggedBefore++
        }
      }
      return nonDraggedBefore
    }
  }

  const originalDraggedIndex = currentItems.findIndex(
    (item) => item.GUID === draggedGuid,
  )

  // If dropped on itself, retain original position
  if (dropTargetItemId === draggedGuid) {
    return originalDraggedIndex !== -1 ? originalDraggedIndex : 0
  }

  const dropTargetIndex = itemsWithoutDragged.findIndex(
    (item) => item.GUID === dropTargetItemId,
  )

  // If target item not found in filtered list
  if (dropTargetIndex === -1) {
    return 0
  }

  if (isCrossConfig) {
    return dropTargetIndex 
  }

  const originalTargetIndex = currentItems.findIndex(
    (item) => item.GUID === dropTargetItemId,
  )

  const movingUp = originalDraggedIndex > originalTargetIndex
  return movingUp ? dropTargetIndex : dropTargetIndex + 1
}

/**
 * Executes the final drop operation - moves items and notifies backend
 */
export const executeDrop = (
  dragState: DragState,
  dropConfig: DropConfig,
  moveItemsBetweenConfigs: (
    draggedItems: IConfigItem[],
    sourceConfigIndex: number,
    targetConfigIndex: number,
    insertionIndex: number,
  ) => void,
): void => {
  const { targetConfigIndex, insertionIndex } = dropConfig

  // Move items in store
  moveItemsBetweenConfigs(
    dragState.items.draggedItems,
    dragState.configs.current,
    targetConfigIndex,
    insertionIndex,
  )

  // Notify backend
  messageExchange.publish({
    key: "CommandResortConfigItem",
    payload: {
      items: dragState.items.draggedItems,
      newIndex: insertionIndex,
      sourceFileIndex: dragState.configs.source,
      targetFileIndex: targetConfigIndex,
    },
  } as CommandResortConfigItem)

  console.log(
    "✅ Drop complete - items positioned in config",
    targetConfigIndex,
  )
}

/**
 * Extracts drop context information from drag end event
 */
export const extractDropContext = (
  event: DragEndEvent,
  dragState: DragState,
  getConfigItems: (configIndex: number) => IConfigItem[],
  effectiveOverId?: string | number,
): {
  dropContext: DropContext
  sourceConfigIndex: number
  targetConfigIndex: number
} => {
  const { operation } = event
  const target = operation.target

  const hoveringOverTab = target?.data?.type === "tab"
  const dropOnPlaceholder = target?.data?.type === "placeholder"
  const dropTargetItemId =
    effectiveOverId !== undefined
      ? String(effectiveOverId)
      : String(target?.id ?? "")

  const sourceConfigIndex = dragState.configs.source
  const targetConfigIndex = hoveringOverTab
    ? ((target?.data?.index as number | undefined) ??
      dragState.ui.hoveredTabIndex)
    : dragState.configs.current

  const currentItems = getConfigItems(targetConfigIndex)

  const draggedItemIds = dragState.items.draggedItems.map((item) => item.GUID)

  const itemsWithoutDragged = currentItems.filter(
    (item) => !draggedItemIds.includes(item.GUID),
  )

  const dropContext: DropContext = {
    hoveringOverTab,
    dropOnPlaceholder,
    dropTargetItemId,
    itemsWithoutDragged,
    currentItems,
    draggedItems: dragState.items.draggedItems,
  }

  return {
    dropContext,
    sourceConfigIndex,
    targetConfigIndex,
  }
}
