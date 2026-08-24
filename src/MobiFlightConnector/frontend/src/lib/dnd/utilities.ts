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
): DragEndValidation => {
  const { operation, canceled } = event
   if (canceled) {
    return { isValid: false, reason: "Drag was canceled" }  
  }

  const activeId = operation.source?.id
  const overId = operation.target?.id

  if (!dragState?.items || !dragState.configs) {
    return { isValid: false, reason: "Invalid drag state" }
  }

  if (!activeId) {
    return { isValid: false, reason: "No active item" }
  }

  if (!overId) {
    return { isValid: false, reason: "Dropped outside valid zone" }
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
): number => {
  const {
    hoveringOverTab,
    dropOnPlaceholder,
    dropTargetItemId,
    itemsWithoutDragged,
    currentItems,
    draggedItems,
  } = dropContext

  if (hoveringOverTab || dropOnPlaceholder || itemsWithoutDragged.length === 0) {
    return 0
  }

  const draggedGuid = (activeId as string) || draggedItems[0]?.GUID
  const originalDraggedIndex = currentItems.findIndex(
    (item) => item.GUID === draggedGuid,
  )

  // If dropped on the table body container below rows
  if (dropTargetItemId === "config-item-table-body") {
    return itemsWithoutDragged.length
  }

  // If dropped on itself, retain original position
  if (dropTargetItemId === draggedGuid) {
    return originalDraggedIndex !== -1 ? originalDraggedIndex : 0
  }

  const dropTargetIndex = itemsWithoutDragged.findIndex(
    (item) => item.GUID === dropTargetItemId,
  )

  // If target item not found in filtered list
  if (dropTargetIndex === -1) {
    if (!isCrossConfig && originalDraggedIndex !== -1) {
      return originalDraggedIndex
    }
    return itemsWithoutDragged.length
  }

  if (isCrossConfig) return dropTargetIndex + 1

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
  const { sourceConfigIndex, targetConfigIndex, insertionIndex } = dropConfig

  // Move items in store
  moveItemsBetweenConfigs(
    dragState.items.draggedItems,
    sourceConfigIndex,
    targetConfigIndex,
    insertionIndex,
  )

  // Notify backend
  messageExchange.publish({
    key: "CommandResortConfigItem",
    payload: {
      items: dragState.items.draggedItems,
      newIndex: insertionIndex,
      sourceFileIndex: sourceConfigIndex,
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
): {
  dropContext: DropContext
  sourceConfigIndex: number
  targetConfigIndex: number
} => {
  const { operation } = event
  const target = operation.target

  const hoveringOverTab = target?.data?.type === "tab"
  const dropOnPlaceholder = target?.data?.type === "placeholder"
  const dropTargetItemId = target?.id as string

  const sourceConfigIndex = dragState.configs.source
  const targetConfigIndex = hoveringOverTab
    ? ((target?.data?.index as number | undefined) ?? dragState.ui.hoveredTabIndex)
    : dragState.configs.current

  const currentItems = getConfigItems(targetConfigIndex)

  const draggedItemIds = dragState.items.draggedItems.map(
    (item) => item.GUID,
  )

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
