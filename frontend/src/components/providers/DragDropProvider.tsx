import React, { useCallback, useEffect, useMemo, useState } from "react"
import {
  DndContext,
  DragStartEvent,
  DragEndEvent,
  useSensors,
  useSensor,
  MouseSensor,
  TouchSensor,
  DragMoveEvent,
  pointerWithin,
  Modifier,
  closestCenter,
} from "@dnd-kit/core"
import { restrictToVerticalAxis } from "@dnd-kit/modifiers"
import { snapToCursor } from "@/lib/dnd-kit/snap-to-cursor"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { CommandResortConfigItem } from "@/types/commands"
import { IConfigItem } from "@/types"
import { Table } from "@tanstack/react-table"
import { ConfigItemDragOverlay } from "@/components/dnd/ConfigItemDragOverlay"
import { ConfigItemDragContext } from "./ConfigItemContext"
import { useProjectStore, useProjectStoreActions } from "@/stores/projectStore"
import { restrictToBottomOfParentElement } from "../dnd/modifiers/restrictToBottomOfParentElement"

/**
 * The drag state that persists throughout the entire drag operation
 * This data survives tab switches and component re-renders
 */
export interface DragState {
  items: {
    dragItem: IConfigItem | null
    draggedItems: IConfigItem[] // The actual items being dragged (full objects)
    originalPositions: Map<string, number> // GUID -> original index
  }

  configs: {
    source: number // Which config file the drag started from, needed for restore
    current: number // Which config file the drag is currently over
  }

  ui: {
    isDragging: boolean // Whether a drag is currently active
    isInsideTable: boolean // Whether the drag is currently over a valid table
    hoveredTabIndex: number // If dragging over a tab, which tab index
    activeTabIndex?: number // If drag ended over a tab, which tab index
  }
}

/**
 * Context interface - what components can access via useConfigItemDragContext()
 */
export interface ConfigItemDragContextType {
  dragState: DragState | null
  table: Table<IConfigItem> | null
  setTable: (table: Table<IConfigItem> | null) => void
  setTableContainerRef: (element: Element | null) => void
}

/**
 * Props for the drag provider component
 */
interface ConfigItemDragProviderProps {
  children: React.ReactNode
  initialConfigIndex: number
  // Function to update config items in the project store
  // This makes drag-drop independent of table implementation
  updateConfigItems: (configIndex: number, items: IConfigItem[]) => void
  // Function to get current config items from project store
  getConfigItems: (configIndex: number) => IConfigItem[]
  selectActiveFile: (index: number) => void
}

/**
 * Provider component that manages ALL drag-and-drop logic
 */
export function ConfigItemDragProvider({
  children,
  initialConfigIndex,
  getConfigItems,
  selectActiveFile,
}: ConfigItemDragProviderProps) {
  // State: Current table instance (set by ConfigItemTable when it mounts)
  const [table, setTable] = useState<Table<IConfigItem> | null>(null)

  // State: Current drag operation (null when not dragging)
  const [dragState, setDragState] = useState<DragState | null>(null)

  const [tableContainerRef, setTableContainerRefInternal] =
    useState<Element | null>(null)

  // Configure what input methods can trigger drag operations
  const sensors = useSensors(
    useSensor(MouseSensor, {}), // Mouse drag support
    useSensor(TouchSensor, {}), // Touch drag support for mobile
  )

  const setTableContainerRef = useCallback((element: Element | null) => {
    setTableContainerRefInternal(element)
  }, [])

  const { moveItemsBetweenConfigs, restoreItemsToOriginalPositions } =
    useProjectStoreActions()

  // Subscribe to the actual active config index from store
  const activeConfigFileIndex = useProjectStore(
    (state) => state.activeConfigFileIndex,
  )

  /**
   * Called when user starts dragging an item
   * Captures what items are being dragged and from which config
   */
  const handleDragStart = useCallback(
    (event: DragStartEvent) => {
      console.log("🚀 Drag start - Initial config:", initialConfigIndex)

      if (!table) {
        console.warn("No table available for drag start")
        return
      }

      // Get all currently selected items from the table
      // These are the items that will be moved as a group
      const selectedRows = table.getSelectedRowModel().rows
      let draggedItems = selectedRows.map((row) => row.original)

      // Special case: If the dragged item isn't already selected, select it
      // This allows single-click-drag without requiring pre-selection
      const draggedRow = table
        .getRowModel()
        .rows.find((row) => row.id === event.active.id)
      if (draggedRow && !draggedRow.getIsSelected()) {
        // Select the dragged item
        table.setRowSelection({ [event.active.id]: true })
        // Use just this item as the dragged items
        draggedItems = [draggedRow.original]
      }

      const dragItem = draggedRow ? draggedRow.original : null
      const draggedItemsIds = draggedItems.map((item) => item.GUID)

      const originalPositions = new Map<string, number>()
      table.getRowModel().rows.forEach((row, index) => {
        if (!draggedItemsIds.includes(row.original.GUID)) return
        originalPositions.set(row.original.GUID, index)
      })

      // Create drag state that will persist throughout the operation
      const newDragState: DragState = {
        items: {
          dragItem: dragItem,
          draggedItems: draggedItems,
          originalPositions: originalPositions,
        },

        configs: {
          source: initialConfigIndex,
          current: initialConfigIndex,
        },

        ui: {
          isDragging: true,
          isInsideTable: true,
          hoveredTabIndex: -1,
          activeTabIndex: initialConfigIndex,
        },
      }

      setDragState(newDragState)

      console.log("📋 Dragging items:", {
        count: newDragState.items.draggedItems.length,
        items: newDragState.items.draggedItems.map(
          (item) => item.Name || item.GUID,
        ),
        sourceConfig: newDragState.configs.source,
      })
    },
    [table, initialConfigIndex],
  )

  /**
   * Simplified drag cancellation - restore items to initial config
   */
  const handleDragCancel = useCallback(() => {
    const currentDragState = dragState

    console.log("❌ Drag cancelled")
    setDragState(null)

    if (!currentDragState) return

    console.log("🔄 Restoring items to original positions:", {
      from: currentDragState.configs.current,
      to: currentDragState.configs.source,
      itemCount: currentDragState.items.draggedItems.length,
    })

    // Single store operation that handles everything
    restoreItemsToOriginalPositions(
      currentDragState.items.draggedItems,
      currentDragState.configs.current,
      currentDragState.configs.source,
      currentDragState.items.originalPositions,
    )

    // Switch back to original tab first
    selectActiveFile(currentDragState.configs.source)
  }, [dragState, restoreItemsToOriginalPositions, selectActiveFile])

  /**
   * Called when user drops the dragged items
   * Universal handler for both same-config and cross-config drops
   * Works directly with project store, independent of table implementation
   */
  const handleDragEnd = useCallback(
    (event: DragEndEvent) => {
      const { active, over } = event

      console.log("🎯 Drag end:", {
        activeId: active.id,
        overId: over?.id,
        dragState,
      })

      // Clean up: Always clear drag state when drag ends
      const currentDragState = dragState
      setDragState(null)

      // Bail out if drag was cancelled or invalid
      if (
        !currentDragState ||
        !currentDragState.items ||
        !currentDragState.configs
      ) {
        console.log("❌ Drag cancelled or invalid (currentDragState is null)")
        return
      }

      if (!active.id) {
        console.log("❌ Drag cancelled or invalid (active.id is null)")
        return
      }

      if (!over?.id) {
        console.log(
          "🚫 Dropped outside valid drop zone - treating as cancellation",
        )
        handleDragCancel()
        return
      }

      // No-op: item dropped on itself
      if (active.id === over.id) {
        console.log("↩️ Item dropped on itself - no change needed")
        return
      }

      // Final drop is
      // a) within the current config when we are over the table
      // b) or on a tab (which means move to that config and put at end)

      const hoveringOverTab = event.over?.data?.current?.type === "tab"
      const dropOnPlaceholder =
        event.over?.data?.current?.type === "placeholder"

      // only if we are ending over tab we have to still move between configs
      // in all other cases source config and target config are the same
      const sourceConfigIndex = !hoveringOverTab
        ? currentDragState.configs.current
        : currentDragState.configs.source
      const targetConfigIndex = !hoveringOverTab
        ? currentDragState.configs.current
        : currentDragState.ui.hoveredTabIndex

      const dropTargetItemId = over.id as string

      // Get current items and filter out the ones being dragged
      const currentItems = getConfigItems(sourceConfigIndex)
      const draggedItemIds = currentDragState.items.draggedItems.map(
        (item) => item.GUID,
      )
      const itemsWithoutDragged = currentItems.filter(
        (item) => !draggedItemIds.includes(item.GUID),
      )

      // Find the target position in the filtered list
      // insert at the beginning if dropped on a tab or placeholder
      const dropTargetIndex =
        hoveringOverTab || dropOnPlaceholder
          ? 0
          : itemsWithoutDragged.findIndex(
              (item) => item.GUID === dropTargetItemId,
            )

      if (dropTargetIndex === -1) {
        console.error("❌ Drop target not found in filtered list")
        return
      }

      // FIXED: Handle empty list case for tab/placeholder drops
      let insertionIndex: number

      if (hoveringOverTab || dropOnPlaceholder) {
        // Always insert at position 0 for tab/placeholder drops
        insertionIndex = 0
      } else if (itemsWithoutDragged.length === 0) {
        // If no items remain after filtering, insert at position 0
        insertionIndex = 0
      } else {
        // Normal case: calculate based on movement direction
        const originalDraggedIndex = currentItems.findIndex(
          (item) => item.GUID === currentDragState.items.draggedItems[0].GUID,
        )
        const originalTargetIndex = currentItems.findIndex(
          (item) => item.GUID === dropTargetItemId,
        )

        const movingUp = originalDraggedIndex > originalTargetIndex
        insertionIndex = movingUp
          ? dropTargetIndex // Insert before target when moving up
          : dropTargetIndex + 1 // Insert after target when moving down
      }

      console.log("📍 Insertion calculation:", {
        hoveringOverTab,
        dropOnPlaceholder,
        itemsWithoutDraggedLength: itemsWithoutDragged.length,
        dropTargetIndex,
        insertionIndex,
      })

      // Use the calculated insertion index
      moveItemsBetweenConfigs(
        currentDragState.items.draggedItems,
        sourceConfigIndex,
        targetConfigIndex,
        insertionIndex,
      )

      // Notify backend about the final state
      publishOnMessageExchange().publish({
        key: "CommandResortConfigItem",
        payload: {
          items: currentDragState.items.draggedItems,
          newIndex: insertionIndex, // Let backend recalculate based on dropTargetItemId
          sourceFileIndex: currentDragState.configs.source, // Original source for backend context
          targetFileIndex: targetConfigIndex, // Final destination
        },
      } as CommandResortConfigItem)

      console.log(
        "✅ Final drop complete - items positioned in config",
        targetConfigIndex,
      )
    },
    [dragState, getConfigItems, handleDragCancel, moveItemsBetweenConfigs],
  )

  // Simplified useEffect - just move items, don't update dragState
  useEffect(() => {
    if (!dragState || dragState.ui.hoveredTabIndex === -1) return

    // Only move items when tabIndex changes and it's different from current config
    const shouldMoveItems = activeConfigFileIndex !== dragState.configs.current

    if (!shouldMoveItems) return

    console.log("🔄 Effect triggered - moving items:", {
      from: dragState.configs.current,
      to: activeConfigFileIndex,
      items: dragState.items.draggedItems.map((i) => i.GUID),
    })

    // Execute the store operation - that's it!
    moveItemsBetweenConfigs(
      dragState.items.draggedItems,
      dragState.configs.current,
      activeConfigFileIndex,
      0,
    )

    // Update drag state to reflect the new location
    setDragState((prev) =>
      prev
        ? {
            ...prev,
            configs: {
              ...prev.configs,
              current: activeConfigFileIndex,
            },
          }
        : null,
    )
  }, [activeConfigFileIndex, dragState, moveItemsBetweenConfigs])

  const handleDragMove = useCallback(
    (event: DragMoveEvent) => {
      if (!dragState || !tableContainerRef) return

      // Collect all state changes first
      const stateUpdates: Partial<DragState> = {}

      const hoveringOverTab = event.over?.data?.current?.type === "tab"

      // Only update UI state - no store operations here
      if (hoveringOverTab) {
        const hoveredTabIndex = event.over?.data?.current?.index

        if (hoveredTabIndex !== dragState.ui.hoveredTabIndex) {
          console.log("🎯 Tab hover detected:", hoveredTabIndex)
          stateUpdates.ui = {
            ...dragState.ui,
            hoveredTabIndex,
          }
        }
      } else {
        // Left tab area
        if (dragState.ui.hoveredTabIndex !== -1) {
          console.log("⬅️ Left tab area")
          stateUpdates.ui = {
            ...dragState.ui,
            hoveredTabIndex: -1,
          }
        }
      }

      // Get the current mouse position
      const { x, y } = event.activatorEvent as MouseEvent
      const currentX = event.delta.x + x
      const currentY = event.delta.y + y

      // Check if cursor is outside the table container
      const containerRect = tableContainerRef.getBoundingClientRect()
      const isInsideTable =
        currentX >= containerRect.left &&
        currentX <= containerRect.right &&
        currentY >= containerRect.top &&
        currentY <= containerRect.bottom

      if (isInsideTable !== dragState.ui.isInsideTable) {
        stateUpdates.ui = {
          ...(stateUpdates.ui || dragState.ui),
          isInsideTable: isInsideTable,
        }
      }

      // Single state update at the end
      if (Object.keys(stateUpdates).length > 0) {
        setDragState((prev) =>
          prev
            ? {
                ...prev,
                ...stateUpdates,
              }
            : null,
        )
      }
    },
    [dragState, tableContainerRef],
  )

  // Context value that child components can access
  const contextValue: ConfigItemDragContextType = {
    dragState, // Current drag operation state (null when not dragging)
    table, // Current table instance
    setTable, // Function for table to register itself,
    setTableContainerRef, // Function for table to register its container element
  }

  const createDynamicModifier = useCallback(
    (dragState: DragState | null): Modifier => {
      return (args) => {
        const isInsideTable = dragState?.ui.isInsideTable ?? true

        let transform = snapToCursor(args)

        if (isInsideTable) {
          transform = restrictToVerticalAxis({ ...args, transform })
          transform = restrictToBottomOfParentElement({ ...args, transform })
        }

        return transform
      }
    },
    [],
  )

  const modifiers = useMemo(
    () => [createDynamicModifier(dragState)],
    [dragState, createDynamicModifier],
  )

  const collisionDetection = useMemo(() => {
    return dragState?.ui.isInsideTable
      ? // makes the rows snap to center of other rows
        closestCenter
      : // allows to detect when the pointer is over a tab
        pointerWithin
  }, [dragState])

  return (
    // Provide context to child components
    <ConfigItemDragContext.Provider value={contextValue}>
      {/* The actual DnD functionality wrapper */}
      <DndContext
        sensors={sensors}
        // pointer within is needed for tab hover detection
        collisionDetection={collisionDetection}
        modifiers={modifiers}
        onDragStart={handleDragStart}
        onDragMove={handleDragMove}
        onDragEnd={handleDragEnd}
        onDragCancel={handleDragCancel}
      >
        {children}
        <ConfigItemDragOverlay />
      </DndContext>
    </ConfigItemDragContext.Provider>
  )
}
