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
  Over,
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
  dragItem: IConfigItem | null
  draggedItems: IConfigItem[] // The actual items being dragged (full objects)
  sourceConfigIndex: number // Which config file the drag started from, needed for restore
  currentConfigIndex: number // Which config file the drag is currently over
  isDragging: boolean // Whether a drag is currently active
  isInsideTable: boolean // Whether the drag is currently over a valid table
  tabIndex: number // If dragging over a tab, which tab index
  tracking: {
    lastOver: Over | null // Last known "over" element
  }
  direction: "up" | "down" | null // Direction of drag movement, for reordering
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

  const { moveItemsBetweenConfigs } = useProjectStoreActions()

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

      // Create drag state that will persist throughout the operation
      const newDragState: DragState = {
        dragItem: dragItem,
        draggedItems: draggedItems,
        sourceConfigIndex: initialConfigIndex,
        currentConfigIndex: initialConfigIndex,
        isDragging: true,
        isInsideTable: true,
        tabIndex: -1,
        tracking: {
          lastOver: null,
        },
        direction: null,
      }

      setDragState(newDragState)

      console.log("📋 Dragging items:", {
        count: newDragState.draggedItems.length,
        items: newDragState.draggedItems.map((item) => item.Name || item.GUID),
        sourceConfig: newDragState.sourceConfigIndex,
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

    const isSameTab =
      currentDragState.sourceConfigIndex === currentDragState.currentConfigIndex

    if (isSameTab) return

    console.log("🔄 Restoring items to initial config:", {
      from: currentDragState.currentConfigIndex,
      to: currentDragState.sourceConfigIndex,
      itemCount: currentDragState.draggedItems.length,
    })

    // Move items back to where they started
    moveItemsBetweenConfigs(
      currentDragState.draggedItems,
      currentDragState.currentConfigIndex,
      currentDragState.sourceConfigIndex,
    )
  }, [dragState, moveItemsBetweenConfigs])

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
      if (!currentDragState || !over?.id || !active.id) {
        console.log("❌ Drag cancelled or invalid")
        return
      }

      // No-op: item dropped on itself
      if (active.id === over.id) {
        console.log("↩️ Item dropped on itself - no change needed")
        return
      }

      // Final drop is always within the current config (thanks to useEffect handling cross-config moves)
      const targetConfigIndex = currentDragState.currentConfigIndex
      const dropTargetItemId = over.id as string

      console.log("📍 Final positioning within config:", {
        config: targetConfigIndex,
        dropTarget: dropTargetItemId,
        draggedItems: currentDragState.draggedItems.map((i) => i.GUID),
      })

      // Use store function for final positioning within the same config
      moveItemsBetweenConfigs(
        currentDragState.draggedItems,
        targetConfigIndex, // source = current config
        targetConfigIndex, // target = same config (just repositioning)
        dropTargetItemId, // for positioning within config
        currentDragState.direction == "up" ? 0 : 1, // position relative to target based on direction
      )

      // Notify backend about the final state
      publishOnMessageExchange().publish({
        key: "CommandResortConfigItem",
        payload: {
          items: currentDragState.draggedItems,
          sourceFileIndex: currentDragState.sourceConfigIndex, // Original source for backend context
          targetFileIndex: targetConfigIndex, // Final destination
          newIndex: 0, // Let backend recalculate based on dropTargetItemId
        },
      } as CommandResortConfigItem)

      console.log(
        "✅ Final drop complete - items positioned in config",
        targetConfigIndex,
      )
    },
    [dragState, moveItemsBetweenConfigs],
  )

  // Simplified useEffect - just move items, don't update dragState
  useEffect(() => {
    if (!dragState || dragState.tabIndex === -1) return

    // Only move items when tabIndex changes and it's different from current config
    const shouldMoveItems = dragState.tabIndex !== dragState.currentConfigIndex

    if (!shouldMoveItems) return

    console.log("🔄 Effect triggered - moving items:", {
      from: dragState.currentConfigIndex,
      to: dragState.tabIndex,
      items: dragState.draggedItems.map((i) => i.GUID),
    })

    // Execute the store operation - that's it!
    moveItemsBetweenConfigs(
      dragState.draggedItems,
      dragState.currentConfigIndex,
      dragState.tabIndex,
    )

    // Update drag state to reflect the new location
    setDragState((prev) =>
      prev
        ? {
            ...prev,
            currentConfigIndex: dragState.tabIndex,
          }
        : null,
    )
  }, [dragState, moveItemsBetweenConfigs])

  const handleDragMove = useCallback(
    (event: DragMoveEvent) => {
      if (!dragState || !tableContainerRef) return

      // Collect all state changes first
      const stateUpdates: Partial<DragState> = {}

      // Track movement direction based on over item changes
      const currentOverId = event.over?.id as string
      let movementDirection = dragState.direction

      if (currentOverId && currentOverId !== dragState.tracking.lastOver?.id) {
        // Get the items from current config to determine order
        const currentItems = getConfigItems(dragState.currentConfigIndex)
        const lastIndex = currentItems.findIndex(
          (item) => item.GUID === dragState.tracking.lastOver?.id,
        )
        const currentIndex = currentItems.findIndex(
          (item) => item.GUID === currentOverId,
        )

        if (lastIndex !== -1 && currentIndex !== -1) {
          movementDirection = currentIndex > lastIndex ? "down" : "up"
          console.log(`📐 Movement direction: ${movementDirection}`, {
            from: dragState.tracking.lastOver?.id,
            to: currentOverId,
            fromIndex: lastIndex,
            toIndex: currentIndex,
          })
        }

        stateUpdates.tracking = { lastOver: event.over }
        stateUpdates.direction = movementDirection
      }

      // Only update UI state - no store operations here
      if (event.over?.data?.current?.type === "tab") {
        const hoveredTabIndex = event.over?.data?.current?.index

        if (hoveredTabIndex !== dragState.tabIndex) {
          console.log("🎯 Tab hover detected:", hoveredTabIndex)
          stateUpdates.tabIndex = hoveredTabIndex
        }
      } else {
        // Left tab area
        if (dragState.tabIndex !== -1) {
          console.log("⬅️ Left tab area")
          stateUpdates.tabIndex = -1
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

      if (isInsideTable !== dragState.isInsideTable) {
        stateUpdates.isInsideTable = isInsideTable
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
    [dragState, getConfigItems, tableContainerRef],
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
    if (!(dragState?.isInsideTable ?? true)) {
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
