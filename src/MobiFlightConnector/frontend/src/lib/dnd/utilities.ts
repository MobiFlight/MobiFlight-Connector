import type { DragEndEvent } from "@dnd-kit/core"
import type { IConfigItem } from "@/types"
import type { DragState } from "@/components/providers/DragDropProvider"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { CommandResortConfigItem } from "@/types/commands"

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

/**
 * Validates if a drag end event should be processed
 */
export const validateDragEnd = (
  event: DragEndEvent,
  dragState: DragState | null
): DragEndValidation => {
  const { active, over } = event

  if (!dragState?.items || !dragState.configs) {
    return { isValid: false, reason: "Invalid drag state" }
  }

  if (!active.id) {
    return { isValid: false, reason: "No active item" }
  }

  if (!over?.id) {
    return { isValid: false, reason: "Dropped outside valid zone" }
  }

  if (
    active.id === over.id &&
    dragState.configs.source === dragState.configs.current
  ) {
    return { isValid: false, reason: "Item dropped on itself" }
  }

  return { isValid: true }
}

/**
 * Calculates insertion index based on drop context
 */
export const calculateInsertionIndex = (dropContext: DropContext): number => {
  const {
    hoveringOverTab,
    dropOnPlaceholder,
    dropTargetItemId,
    itemsWithoutDragged,
    currentItems,
    draggedItems,
  } = dropContext

  // Special cases: tab drops and placeholder drops always go to position 0
  if (hoveringOverTab || dropOnPlaceholder) {
    return 0
  }

  // Empty list: insert at position 0
  if (itemsWithoutDragged.length === 0) {
    return 0
  }

  // Find target position in filtered list
  const dropTargetIndex = itemsWithoutDragged.findIndex(
    (item) => item.GUID === dropTargetItemId
  )

  if (dropTargetIndex === -1) {
    return 0
  }

  // Calculate based on movement direction
  const originalDraggedIndex = currentItems.findIndex(
    (item) => item.GUID === draggedItems[0].GUID
  )
  const originalTargetIndex = currentItems.findIndex(
    (item) => item.GUID === dropTargetItemId
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
    insertionIndex: number
  ) => void
): void => {
  const { sourceConfigIndex, targetConfigIndex, insertionIndex } = dropConfig

  // Move items in store
  moveItemsBetweenConfigs(
    dragState.items.draggedItems,
    sourceConfigIndex,
    targetConfigIndex,
    insertionIndex
  )

  // Notify backend
  publishOnMessageExchange().publish({
    key: "CommandResortConfigItem",
    payload: {
      items: dragState.items.draggedItems,
      newIndex: insertionIndex,
      sourceFileIndex: dragState.configs.source,
      targetFileIndex: targetConfigIndex,
    },
  } as CommandResortConfigItem)

  console.log("✅ Drop complete - items positioned in config", targetConfigIndex)
}

/**
 * Extracts drop context information from drag end event
 */
export const extractDropContext = (
  event: DragEndEvent,
  dragState: DragState,
  getConfigItems: (configIndex: number) => IConfigItem[]
): {
  dropContext: DropContext
  sourceConfigIndex: number
  targetConfigIndex: number
} => {
  const { over } = event

  const hoveringOverTab = over?.data?.current?.type === "tab"
  const dropOnPlaceholder = over?.data?.current?.type === "placeholder"
  const dropTargetItemId = over!.id as string

  // Determine config indices
  const sourceConfigIndex = !hoveringOverTab
    ? dragState.configs.current
    : dragState.configs.source
  const targetConfigIndex = !hoveringOverTab
    ? dragState.configs.current
    : dragState.ui.hoveredTabIndex

  // Get items and filter out dragged ones
  const currentItems = getConfigItems(sourceConfigIndex)
  const draggedItemIds = dragState.items.draggedItems.map(item => item.GUID)
  const itemsWithoutDragged = currentItems.filter(
    item => !draggedItemIds.includes(item.GUID)
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