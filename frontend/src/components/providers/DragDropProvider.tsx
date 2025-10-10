import React, { useCallback, useEffect, useState } from "react"
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
} from "@dnd-kit/core"
import { restrictToVerticalAxis } from "@dnd-kit/modifiers"
import { snapToCursor } from "@/lib/dnd-kit/snap-to-cursor"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { CommandResortConfigItem } from "@/types/commands"
import { IConfigItem } from "@/types"
import { Table } from "@tanstack/react-table"
import { ConfigItemDragOverlay } from "@/components/dnd/ConfigItemDragOverlay"
import { ConfigItemDragContext } from "./ConfigItemContext"
import { useProjectStoreActions } from "@/stores/projectStore"

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
 * Key insight: Works directly with project store, not table data
 * This makes it independent of table implementation, virtualization, filters, etc.
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

    // Switch back to original tab first
    selectActiveFile(currentDragState.configs.source)

    // Single store operation that handles everything
    restoreItemsToOriginalPositions(
      currentDragState.items.draggedItems,
      currentDragState.configs.current,
      currentDragState.configs.source,
      currentDragState.items.originalPositions,
    )
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

      // Final drop is always within the current config
      const targetConfigIndex = currentDragState.configs.current
      const dropTargetItemId = over.id as string

      // Get current items and filter out the ones being dragged
      const currentItems = getConfigItems(targetConfigIndex)
      const draggedItemIds = currentDragState.items.draggedItems.map(
        (item) => item.GUID,
      )

      const itemsWithoutDragged = currentItems.filter(
        (item) => !draggedItemIds.includes(item.GUID),
      )

      const hoveringOverTab = event.over?.data?.current?.type === "tab"

      // Find the target position in the filtered list
      const dropTargetIndex = hoveringOverTab
        ? 0
        : itemsWithoutDragged.findIndex(
            (item) => item.GUID === dropTargetItemId,
          )

      if (dropTargetIndex === -1) {
        console.error("❌ Drop target not found in filtered list")
        return
      }

      // Get original positions to determine movement direction
      const originalDraggedIndex = currentItems.findIndex(
        (item) => item.GUID === currentDragState.items.draggedItems[0].GUID,
      )
      const originalTargetIndex = currentItems.findIndex(
        (item) => item.GUID === dropTargetItemId,
      )

      // Determine insertion position like useSortable does
      const movingUp = originalDraggedIndex > originalTargetIndex
      const insertionIndex = movingUp
        ? dropTargetIndex // Insert before target when moving up
        : dropTargetIndex + 1 // Insert after target when moving down

      // Use the calculated insertion index
      moveItemsBetweenConfigs(
        currentDragState.items.draggedItems,
        targetConfigIndex,
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
    const shouldMoveItems =
      dragState.ui.hoveredTabIndex !== dragState.configs.current

    if (!shouldMoveItems) return

    console.log("🔄 Effect triggered - moving items:", {
      from: dragState.configs.current,
      to: dragState.ui.hoveredTabIndex,
      items: dragState.items.draggedItems.map((i) => i.GUID),
    })

    // Execute the store operation - that's it!
    moveItemsBetweenConfigs(
      dragState.items.draggedItems,
      dragState.configs.current,
      dragState.ui.hoveredTabIndex,
      0,
    )

    // Update drag state to reflect the new location
    setDragState((prev) =>
      prev
        ? {
            ...prev,
            configs: {
              ...prev.configs,
              current: dragState.ui.hoveredTabIndex,
            },
          }
        : null,
    )
  }, [dragState, moveItemsBetweenConfigs])

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
          isInsideTable,
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

  // Dynamic modifiers based on drag state
  const getModifiers = () => {
    if (!(dragState?.ui.isInsideTable ?? true)) {
      // Outside container: free movement for cross-tab dragging
      return [snapToCursor]
    } else {
      // Inside container: restrict to vertical for row reordering
      return [snapToCursor, restrictToVerticalAxis]
    }
  }

  return (
    // Provide context to child components
    <ConfigItemDragContext.Provider value={contextValue}>
      {/* The actual DnD functionality wrapper */}
      <DndContext
        sensors={sensors}
        // pointer within is needed for tab hover detection
        collisionDetection={pointerWithin}
        modifiers={getModifiers()}
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
