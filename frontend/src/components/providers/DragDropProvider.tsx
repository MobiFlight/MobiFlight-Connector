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
  closestCorners,
} from "@dnd-kit/core"
import { restrictToVerticalAxis } from "@dnd-kit/modifiers"
import { snapToCursor } from "@/lib/dnd-kit/snap-to-cursor"
import { IConfigItem } from "@/types"
import { Table } from "@tanstack/react-table"
import { ConfigItemDragOverlay } from "@/components/dnd/ConfigItemDragOverlay"
import { ConfigItemDragContext } from "./ConfigItemContext"
import { useProjectStore, useProjectStoreActions } from "@/stores/projectStore"
import { restrictToBottomOfParentElement } from "../dnd/modifiers/restrictToBottomOfParentElement"
import {
  calculateInsertionIndex,
  executeDrop,
  extractDropContext,
  validateDragEnd,
} from "@/lib/dnd/utilities"

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

      // Validate the drag operation
      const validation = validateDragEnd(event, currentDragState)
      if (!validation.isValid) {
        console.log(`❌ ${validation.reason}`)
        if (validation.reason === "Dropped outside valid zone") {
          handleDragCancel()
        }
        return
      }

      // Extract drop context and config information
      const { dropContext, sourceConfigIndex, targetConfigIndex } =
        extractDropContext(event, currentDragState!, getConfigItems)

      // Calculate where to insert the items
      const insertionIndex = calculateInsertionIndex(dropContext)

      console.log("📍 Insertion calculation:", {
        hoveringOverTab: dropContext.hoveringOverTab,
        dropOnPlaceholder: dropContext.dropOnPlaceholder,
        itemsWithoutDraggedLength: dropContext.itemsWithoutDragged.length,
        insertionIndex,
      })

      // Execute the drop operation
      executeDrop(
        currentDragState!,
        { sourceConfigIndex, targetConfigIndex, insertionIndex },
        moveItemsBetweenConfigs,
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

      const defaultType =
        event.over?.data?.current?.type ??
        (Math.abs(event.delta.y) < 15 && "row")
      const hoveringOverTable = ["table", "row"].includes(
        event.over?.data?.current?.type ?? defaultType,
      )

      if (hoveringOverTable && !dragState.ui.isInsideTable) {
        console.log("➡️ Entered table area")
        stateUpdates.ui = {
          ...dragState.ui,
          isInsideTable: true,
        }
      } else if (!hoveringOverTable && dragState.ui.isInsideTable) {
        console.log("⬅️ Left table area")
        stateUpdates.ui = {
          ...dragState.ui,
          isInsideTable: false,
        }
      }

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
        closestCorners
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
